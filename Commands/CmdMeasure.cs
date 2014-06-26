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
namespace MCForge.Commands
{
    public sealed class CmdMeasure : Command
    {
        public override string name { get { return "measure"; } }
        public override string shortcut { get { return  "ms"; } }
        public override string type { get { return "information"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdMeasure() { }

        public override void Use(Player p, string message)
        {
            if (message.IndexOf(' ') != -1) { Help(p); return; }

            CatchPos cpos;
            cpos.toIgnore = Block.Ushort(message);
            if (cpos.toIgnore == Block.Zero && message != "") { Player.SendMessage(p, "Could not find block specified"); return; }

            cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;

            Player.SendMessage(p, "Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/measure [ignore] - Measures all the blocks between two points");
            Player.SendMessage(p, "/measure [ignore] - Enter a block to ignore them");
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

            ushort xx, yy, zz; int foundBlocks = 0;

            for (xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); ++xx)
                for (yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                    for (zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                    {
                        if (p.level.GetTile(xx, yy, zz) != cpos.toIgnore) foundBlocks++;
                    }

            Player.SendMessage(p, foundBlocks + " blocks are between (" + cpos.x + ", " + cpos.y + ", " + cpos.z + ") and (" + x + ", " + y + ", " + z + ")");
            if (p.staticCommands) p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        struct CatchPos
        {
            public ushort x, y, z; public ushort toIgnore;
        }
    }
}