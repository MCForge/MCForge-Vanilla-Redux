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

namespace MCForge.Commands
{
    public sealed class CmdBind : Command
    {
        public override string name { get { return "bind"; } }
        public override string shortcut { get { return  ""; } }
        public override string type { get { return "build"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdBind() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            if (p == null)
            {
                Player.SendMessage(p, "This command can only be used in-game");
                return;
            }
            if (message.Split(' ').Length > 2) { Help(p); return; }
            message = message.ToLower();
            if (message == "clear")
            {
                for (byte d = 0; d < 128; d++) p.bindings[d] = d;
                Player.SendMessage(p, "All bindings were unbound.");
                return;
            }

            int pos = message.IndexOf(' ');
            if (pos != -1)
            {
                ushort b1 = Block.Ushort(message.Substring(0, pos));
                ushort b2 = Block.Ushort(message.Substring(pos + 1));
                if (b1 == Block.maxblocks) { Player.SendMessage(p, "There is no block \"" + message.Substring(0, pos) + "\"."); return; }
                if (b2 == Block.maxblocks) { Player.SendMessage(p, "There is no block \"" + message.Substring(pos + 1) + "\"."); return; }

                if (!Block.Placable(b1)) { Player.SendMessage(p, Block.Name(b1) + " isn't a special block."); return; }
                if (!Block.canPlace(p, b2)) { Player.SendMessage(p, "You can't bind " + Block.Name(b2) + "."); return; }
                if (b1 > (byte)64) { Player.SendMessage(p, "Cannot bind anything to this block."); return; }

                if (p.bindings[b1] == b2) { Player.SendMessage(p, Block.Name(b1) + " is already bound to " + Block.Name(b2) + "."); return; }

                p.bindings[b1] = b2;
                message = Block.Name(b1) + " bound to " + Block.Name(b2) + ".";

                Player.SendMessage(p, message);
            }
            else
            {
                ushort b = Block.Ushort(message);
                if (b > 100) { Player.SendMessage(p, "This block cannot be bound"); return; }

                if (p.bindings[b] == b) { Player.SendMessage(p, Block.Name(b) + " isn't bound."); return; }
                p.bindings[b] = b; Player.SendMessage(p, "Unbound " + Block.Name(b) + ".");
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/bind <block> [type] - Replaces block with type.");
            Player.SendMessage(p, "/bind clear - Clears all binds.");
        }
    }
}