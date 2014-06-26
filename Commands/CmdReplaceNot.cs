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
using System.Linq;
namespace MCForge.Commands
{
    public sealed class CmdReplaceNot : Command
    {
        public override string name { get { return "replacenot"; } }
        public override string shortcut { get { return  "rn"; } }
        public override string type { get { return "build"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdReplaceNot() { }
        public static byte wait;

        public override void Use(Player p, string message)
        {
            wait = 0;
            string[] args = message.Split(' ');
            if (args.Length != 2) { p.SendMessage("Invalid number of arguments!"); Help(p); wait = 1; return; }

            CatchPos cpos = new CatchPos();
            List<string> ignore;

            if (args[0].Contains(","))
                ignore = new List<string>(args[0].Split(','));
            else
                ignore = new List<string>() { args[0] };

            ignore = ignore.Distinct().ToList(); // Remove duplicates

            List<string> invalid = new List<string>(); //Check for invalid blocks
            foreach (string name in ignore)
                if (Block.Ushort(name) == Block.maxblocks)
                    invalid.Add(name);
            if (Block.Ushort(args[1]) == Block.maxblocks)
                invalid.Add(args[1]);
            if (invalid.Count > 0)
            {
                p.SendMessage(String.Format("Invalid block{0}: {1}", invalid.Count == 1 ? "" : "s", String.Join(", ", invalid.ToArray())));
                return;
            }

            if (ignore.Contains(args[1]))
                ignore.Remove(args[1]);
            if (ignore.Count == 0)
                p.SendMessage("Next time just use cuboid if you're not going to ignore anything!");
            
            if (Block.Ushort(message.Split(' ')[1]) == Block.maxblocks) { p.SendMessage(message.Split(' ')[1] + " does not exist, please spell it correctly."); wait = 1; return; }

            cpos.ignore = new List<ushort>();
            foreach (string name in ignore)
                cpos.ignore.Add(Block.Ushort(name));
            cpos.newType = Block.Ushort(args[1]);

            if (!Block.canPlace(p, cpos.newType)) { p.SendMessage("Cannot place this block type!"); wait = 1; return; }

            p.blockchangeObject = cpos;
            Player.SendMessage(p, "Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public override void Help(Player p)
        {
            p.SendMessage("/rn [block,block2,...] [new] - replace everything but [block] with [new] inside a selected cuboid");
            p.SendMessage("If multiple [block]s are specified they will all be ignored.");
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

            for (ushort xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); ++xx)
                for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                    for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                        if (!cpos.ignore.Contains((ushort)p.level.GetTile(xx, yy, zz))) { BufferAdd(buffer, xx, yy, zz); }

            if (buffer.Count > p.group.maxBlocks)
            {
                p.SendMessage("You tried to replace " + buffer.Count + " blocks.");
                p.SendMessage("You cannot replace more than " + p.group.maxBlocks + ".");
                wait = 1;
                return;
            }

            Player.SendMessage(p, buffer.Count.ToString() + " blocks.");

            if (p.level.bufferblocks && !p.level.Instant)
            {
                buffer.ForEach(delegate(Pos pos)
                {
                    BlockQueue.Addblock(p, pos.x, pos.y, pos.z, cpos.newType);                  //update block for everyone
                });
            }
            else
            {
                buffer.ForEach(delegate(Pos pos)
                {
                    p.level.Blockchange(p, pos.x, pos.y, pos.z, cpos.newType);                  //update block for everyone
                });
            }
            wait = 2;
            if (p.staticCommands) p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        void BufferAdd(List<Pos> list, ushort x, ushort y, ushort z)
        {
            Pos pos; pos.x = x; pos.y = y; pos.z = z; list.Add(pos);
        }

        struct Pos { public ushort x, y, z; }
        struct CatchPos
        {
            public List<ushort> ignore;
            public ushort newType;
            public ushort x, y, z;
        }

    }
}