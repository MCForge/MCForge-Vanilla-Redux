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
    public sealed class CmdReplace : Command
    {
        public override string name { get { return "replace"; } }
        public override string shortcut { get { return  "re"; } }
        public override string type { get { return "build"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdReplace() { }
        public static byte wait;

        public override void Use(Player p, string message)
        {
            wait = 0;
            string[] args = message.Split(' ');
            if (args.Length != 2)
            {
                p.SendMessage("Invail number of arguments!");
                wait = 1;
                Help(p);
                return;
            }

            CatchPos cpos = new CatchPos();
            List<string> oldType;

            oldType = new List<string>(args[0].Split(','));

            oldType = oldType.Distinct().ToList(); // Remove duplicates

            List<string> invalid = new List<string>(); //Check for invalid blocks
            foreach (string name in oldType)
                if (Block.Ushort(name) == Block.maxblocks)
                    invalid.Add(name);
            if (Block.Ushort(args[1]) == Block.maxblocks)
                invalid.Add(args[1]);
            if (invalid.Count > 0)
            {
                p.SendMessage(String.Format("Invalid block{0}: {1}", invalid.Count == 1 ? "" : "s", String.Join(", ", invalid.ToArray())));
                wait = 1;
                return;
            }

            if (oldType.Contains(args[1]))
                oldType.Remove(args[1]);
            if (oldType.Count < 1)
            {
                p.SendMessage("Replacing a block with the same one would be pointless!");
                return;
            }

            cpos.oldType = new List<ushort>();
            foreach (string name in oldType)
                cpos.oldType.Add(Block.Ushort(name));
            cpos.newType = Block.Ushort(args[1]);

            foreach (byte type in cpos.oldType)
                if (!Block.canPlace(p, type) && !Block.BuildIn(type)) { p.SendMessage("Cannot replace that."); wait = 1; return; }
            if (!Block.canPlace(p, cpos.newType)) { p.SendMessage("Cannot place that."); wait = 1; return; }

            p.blockchangeObject = cpos;
            Player.SendMessage(p, "Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public override void Help(Player p)
        {
            p.SendMessage("/replace [block,block2,...] [new] - replace block with new inside a selected cuboid");
            p.SendMessage("If more than one block is specified, they will all be replaced.");
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
                        if (cpos.oldType.Contains(p.level.GetTile(xx, yy, zz))) { BufferAdd(buffer, xx, yy, zz); }

            if (buffer.Count > p.group.maxBlocks)
            {
                Player.SendMessage(p, "You tried to replace " + buffer.Count + " blocks.");
                Player.SendMessage(p, "You cannot replace more than " + p.group.maxBlocks + ".");
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

        struct Pos
        {
            public ushort x, y, z;
        }

        struct CatchPos
        {
            public List<ushort> oldType;
            public ushort newType;
            public ushort x, y, z;
        }

    }
}