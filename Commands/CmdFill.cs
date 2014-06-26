/*
Copyright (C) 2010-2013 David Mitchell

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
namespace MCForge.Commands
{
    public sealed class CmdFill : Command
    {
        public override string name { get { return "fill"; } }
        public override string shortcut { get { return  "f"; } }
        public override string type { get { return "build"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdFill() { }

        public override void Use(Player p, string message)
        {
            CatchPos cpos;

            int number = message.Split(' ').Length;
            if (number > 2) { Help(p); return; }
            if (number == 2)
            {
                int pos = message.IndexOf(' ');
                string t = message.Substring(0, pos).ToLower();
                string s = message.Substring(pos + 1).ToLower();
                cpos.type = Block.Ushort(t);
                if (cpos.type == Block.maxblocks) { Player.SendMessage(p, "There is no block \"" + t + "\"."); return; }

                if (!Block.canPlace(p, cpos.type)) { Player.SendMessage(p, "Cannot place that."); return; }

                if (s == "up") cpos.fillType = FillType.Up;
                else if (s == "down") cpos.fillType = FillType.Down;
                else if (s == "layer") cpos.fillType = FillType.Layer;
                else if (s == "vertical_x") cpos.fillType = FillType.VerticalX;
                else if (s == "vertical_z") cpos.fillType = FillType.VerticalZ;
                else { Player.SendMessage(p, "Invalid fill type"); return; }
            }
            else if (message != "")
            {
                message = message.ToLower();
                if (message == "up") { cpos.fillType = FillType.Up; cpos.type = Block.Zero; }
                else if (message == "down") { cpos.fillType = FillType.Down; cpos.type = Block.Zero; }
                else if (message == "layer") { cpos.fillType = FillType.Layer; cpos.type = Block.Zero; }
                else if (message == "vertical_x") { cpos.fillType = FillType.VerticalX; cpos.type = Block.Zero; }
                else if (message == "vertical_z") { cpos.fillType = FillType.VerticalZ; cpos.type = Block.Zero; }
                else
                {
                    cpos.type = Block.Ushort(message);
                    if (cpos.type == 255) { Player.SendMessage(p, "Invalid block or fill type"); return; } //Just use Block.Zero or byte.MaxValue??
                    if (!Block.canPlace(p, cpos.type)) { Player.SendMessage(p, "Cannot place that."); return; }

                    cpos.fillType = FillType.Default;
                }
            }
            else
            {
                cpos.type = Block.Zero; cpos.fillType = FillType.Default;
            }

            cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;

            Player.SendMessage(p, "Destroy the block you wish to fill."); p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/fill [block] [type] - Fills the area specified with [block].");
            Player.SendMessage(p, "[types] - up, down, layer, vertical_x, vertical_z");
        }

        public void Blockchange1(Player p, ushort x, ushort y, ushort z, ushort type)
        {
            try
            {
                p.ClearBlockchange();
                CatchPos cpos = (CatchPos)p.blockchangeObject;
                if (cpos.type == Block.Zero) cpos.type = (ushort)p.bindings[(int)type];

                ushort oldType = p.level.GetTile(x, y, z);
                p.SendBlockchange(x, y, z, oldType);

                if (cpos.type == oldType) { Player.SendMessage(p, "Cannot fill with the same type."); return; }
                if (!Block.canPlace(p, oldType) && !Block.BuildIn(oldType)) { Player.SendMessage(p, "Cannot fill with that."); return; }

                ushort[] mapBlocks = new ushort[p.level.blocks.Length];
                List<Pos> buffer = new List<Pos>();
                p.level.blocks.CopyTo(mapBlocks, 0);

                fromWhere.Clear();
                deep = 0;
                FloodFill(p, x, y, z, cpos.type, oldType, cpos.fillType, ref mapBlocks, ref buffer);

                int totalFill = fromWhere.Count;
                for (int i = 0; i < totalFill; i++)
                {
                    totalFill = fromWhere.Count;
                    Pos pos = fromWhere[i];
                    deep = 0;
                    FloodFill(p, pos.x, pos.y, pos.z, cpos.type, oldType, cpos.fillType, ref mapBlocks, ref buffer);
                    totalFill = fromWhere.Count;
                }
                fromWhere.Clear();

                if (buffer.Count > p.group.maxBlocks)
                {
                    Player.SendMessage(p, "You tried to fill " + buffer.Count + " blocks.");
                    Player.SendMessage(p, "You cannot fill more than " + p.group.maxBlocks + ".");
                    return;
                }

                if (p.level.bufferblocks && !p.level.Instant)
                {
                    foreach (Pos pos in buffer)
                    {
                      BlockQueue.Addblock(p, pos.x, pos.y, pos.z, cpos.type);
                    }
                }
                else
                {
                    foreach (Pos pos in buffer)
                    {
                        p.level.Blockchange(p, pos.x, pos.y, pos.z, cpos.type);
                    }
                }

                Player.SendMessage(p, "Filled " + buffer.Count + " blocks.");
                buffer.Clear();
                buffer = null;
                mapBlocks = null;

                if (p.staticCommands) p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
            }
            catch (Exception e)
            {
                Server.ErrorLog(e);
            }
        }

        int deep;
        List<Pos> fromWhere = new List<Pos>();
        public void FloodFill(Player p, ushort x, ushort y, ushort z, ushort b, ushort oldType, FillType fillType, ref ushort[] blocks, ref List<Pos> buffer)
        {
            try
            {
                Pos pos;
                pos.x = x; pos.y = y; pos.z = z;

                if (deep > 4000)
                {
                    fromWhere.Add(pos);
                    return;
                }

                blocks[x + p.level.width * z + p.level.width * p.level.height * y] = b;
                buffer.Add(pos);

                //x
                if (fillType != FillType.VerticalX)
                {
                    if (GetTile((ushort)(x + 1), y, z, p.level, blocks) == oldType)
                    {
                        deep++;
                        FloodFill(p, (ushort)(x + 1), y, z, b, oldType, fillType, ref blocks, ref buffer);
                        deep--;
                    }

                    if (x > 0)
                        if (GetTile((ushort)(x - 1), y, z, p.level, blocks) == oldType)
                        {
                            deep++;
                            FloodFill(p, (ushort)(x - 1), y, z, b, oldType, fillType, ref blocks, ref buffer);
                            deep--;
                        }
                }

                //z
                if (fillType != FillType.VerticalZ)
                {
                    if (GetTile(x, y, (ushort)(z + 1), p.level, blocks) == oldType)
                    {
                        deep++;
                        FloodFill(p, x, y, (ushort)(z + 1), b, oldType, fillType, ref blocks, ref buffer);
                        deep--;
                    }

                    if (z > 0)
                        if (GetTile(x, y, (ushort)(z - 1), p.level, blocks) == oldType)
                        {
                            deep++;
                            FloodFill(p, x, y, (ushort)(z - 1), b, oldType, fillType, ref blocks, ref buffer);
                            deep--;
                        }
                }

                //y
                if (fillType == 0 || fillType == FillType.Up || fillType > FillType.Layer)
                {
                    if (GetTile(x, (ushort)(y + 1), z, p.level, blocks) == oldType)
                    {
                        deep++;
                        FloodFill(p, x, (ushort)(y + 1), z, b, oldType, fillType, ref blocks, ref buffer);
                        deep--;
                    }
                }

                if (fillType == 0 || fillType == FillType.Down || fillType > FillType.Layer)
                {
                    if (y > 0)
                        if (GetTile(x, (ushort)(y - 1), z, p.level, blocks) == oldType)
                        {
                            deep++;
                            FloodFill(p, x, (ushort)(y - 1), z, b, oldType, fillType, ref blocks, ref buffer);
                            deep--;
                        }
                }
            } catch (Exception e) { Server.ErrorLog(e); }
        }

        public ushort GetTile(ushort x, ushort y, ushort z, Level l, ushort[] blocks)
        {
            //if (PosToInt(x, y, z) >= blocks.Length) { return null; }
            //Avoid internal overflow
            if (x < 0) { return Block.Zero; }
            if (x >= l.width) { return Block.Zero; }
            if (y < 0) { return Block.Zero; }
            if (y >= l.depth) { return Block.Zero; }
            if (z < 0) { return Block.Zero; }
            if (z >= l.height) { return Block.Zero; }
            try
            {
                return blocks[l.PosToInt(x, y, z)];
            }
            catch (Exception e) { Server.ErrorLog(e); return Block.Zero; }
        }

        struct CatchPos { public ushort x, y, z; public ushort type; public FillType fillType; }
        public struct Pos { public ushort x, y, z; }
        public enum FillType : int
        {
            Default = 0,
            Up = 1,
            Down = 2,
            Layer = 3,
            VerticalX = 4,
            VerticalZ = 5
        }
    }
}