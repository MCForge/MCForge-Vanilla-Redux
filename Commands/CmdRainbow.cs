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
    public sealed class CmdRainbow : Command
    {
        public override string name { get { return "rainbow"; } }
        public override string shortcut { get { return  ""; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdRainbow() { }

        public override void Use(Player p, string message)
        {
            CatchPos cpos;
            cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;
            Player.SendMessage(p, "Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/rainbow - Taste the rainbow");
        }
        public void Blockchange1(Player p, ushort x, ushort y, ushort z, ushort type)
        {
            p.ClearBlockchange();
            ushort b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            CatchPos bp = (CatchPos)p.blockchangeObject;
            bp.x = x; bp.y = y; bp.z = z; p.blockchangeObject = bp;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }
        public void Blockchange2(Player p, ushort x, ushort y, ushort z, ushort type)
        {
            p.ClearBlockchange();
            ushort b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            List<Pos> buffer = new List<Pos>();

            ushort newType = Block.darkpink;

            int xdif = Math.Abs(cpos.x - x);
            int ydif = Math.Abs(cpos.y - y);
            int zdif = Math.Abs(cpos.z - z);

            if (xdif >= ydif && xdif >= zdif)
            {
                for (ushort xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); xx++)
                {
                    newType += 1;
                    if (newType > Block.darkpink) newType = Block.red;
                    for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); yy++)
                    {
                        for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); zz++)
                        {
                            if (p.level.GetTile(xx, yy, zz) != Block.air)
                                BufferAdd(buffer, xx, yy, zz, newType);
                        }
                    }
                }
            }
            else if (ydif > xdif && ydif > zdif)
            {
                for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); yy++)
                {
                    newType += 1;
                    if (newType > Block.darkpink) newType = Block.red;
                    for (ushort xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); xx++)
                    {
                        for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); zz++)
                        {
                            if (p.level.GetTile(xx, yy, zz) != Block.air)
                                BufferAdd(buffer, xx, yy, zz, newType);
                        }
                    }
                }
            }
            else if (zdif > ydif && zdif > xdif)
            {
                for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); zz++)
                {
                    newType += 1;
                    if (newType > Block.darkpink) newType = Block.red;
                    for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); yy++)
                    {
                        for (ushort xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); xx++)
                        {
                            if (p.level.GetTile(xx, yy, zz) != Block.air)
                                BufferAdd(buffer, xx, yy, zz, newType);
                        }
                    }
                }
            }

            if (buffer.Count > p.group.maxBlocks)
            {
                Player.SendMessage(p, "You tried to replace " + buffer.Count + " blocks.");
                Player.SendMessage(p, "You cannot replace more than " + p.group.maxBlocks + ".");
                return;
            }

            Player.SendMessage(p, buffer.Count.ToString() + " blocks.");
            buffer.ForEach(delegate(Pos pos)
            {
                p.level.Blockchange(p, pos.x, pos.y, pos.z, pos.newType);                  //update block for everyone
            });

            if (p.staticCommands) p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        void BufferAdd(List<Pos> list, ushort x, ushort y, ushort z, ushort newType)
        {
            Pos pos;
            pos.x = x; pos.y = y; pos.z = z; pos.newType = newType;
            list.Add(pos);
        }

        struct Pos { public ushort x, y, z; public ushort newType; }
        struct CatchPos { public ushort x, y, z; }
    }
}