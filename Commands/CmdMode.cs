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
    public sealed class CmdMode : Command
    {
        public override string name { get { return "mode"; } }
        public override string shortcut { get { return  ""; } }
        public override string type { get { return "build"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdMode() { }

        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                if (p.modeType != 0)
                {
                    Player.SendMessage(p, "&b" + Block.Name(p.modeType)[0].ToString().ToUpper() + Block.Name(p.modeType).Remove(0, 1).ToLower() + Server.DefaultColor + " mode: &cOFF");
                    p.modeType = 0;
                    p.blockAction = 0;
                }
                else
                {
                    Help(p); return;
                }
            }
            else
            {
                ushort b = Block.Ushort(message);
                if (b == Block.Zero) { Player.SendMessage(p, "Could not find block given."); return; }
                if (b == Block.air) { Player.SendMessage(p, "Cannot use Air Mode."); return; }
                if (p.allowTnt == false)
                {
                    if (b == Block.tnt)
                    {
                        Player.SendMessage(p, "Tnt usage is not allowed at the moment");
                        return;
                    }
                }

                if (p.allowTnt == false)
                {
                    if (b == Block.bigtnt)
                    {
                        Player.SendMessage(p, "Tnt usage is not allowed at the moment");
                        return;
                    }
                }

                if (p.allowTnt == false)
                {
                    if (b == Block.nuketnt)
                    {
                        Player.SendMessage(p, "Tnt usage is not allowed at the moment");
                        return;
                    }
                }

                if (p.allowTnt == false)
                {
                    if (b == Block.fire)
                    {
                        Player.SendMessage(p, "Tnt usage is not allowed at the moment, fire is a lighter for tnt and is also disabled");
                        return;
                    }
                }

                if (p.allowTnt == false)
                {
                    if (b == Block.tntexplosion)
                    {
                        Player.SendMessage(p, "Tnt usage is not allowed at the moment");
                        return;
                    }
                }

                if (p.allowTnt == false)
                {
                    if (b == Block.smalltnt)
                    {
                        Player.SendMessage(p, "Tnt usage is not allowed at the moment");
                        return;
                    }
                }
                        
                if (!Block.canPlace(p, b)) { Player.SendMessage(p, "Cannot place this block at your rank."); return; }

                if (p.modeType == b)
                {
                    Player.SendMessage(p, "&b" + Block.Name(p.modeType)[0].ToString().ToUpper() + Block.Name(p.modeType).Remove(0, 1).ToLower() + Server.DefaultColor + " mode: &cOFF");
                    p.modeType = 0;
                    p.blockAction = 0;
                }
                else
                {
                    p.blockAction = 6;
                    p.modeType = b;
                    Player.SendMessage(p, "&b" + Block.Name(p.modeType)[0].ToString().ToUpper() + Block.Name(p.modeType).Remove(0, 1).ToLower() + Server.DefaultColor + " mode: &aON");
                }
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/mode [block] - Makes every block placed into [block].");
            Player.SendMessage(p, "/[block] also works");
        }
    }
}