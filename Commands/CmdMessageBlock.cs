/*
    Copyright © 2011-2014 MCForge-Redux
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.opensource.org/licenses/ecl2.php
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
*/
using System;
using System.Data;
using System.Text.RegularExpressions;

namespace MCForge.Commands
{
    public class CmdMessageBlock : Command
    {
        public override string name { get { return "mb"; } }
        public override string shortcut { get { return  ""; } }
        public override string type { get { return "build"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdMessageBlock() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }

            CatchPos cpos;
            cpos.message = "";

            try
            {
                switch (message.Split(' ')[0])
                {
                    case "air": cpos.type = Block.MsgAir; break;
                    case "water": cpos.type = Block.MsgWater; break;
                    case "lava": cpos.type = Block.MsgLava; break;
                    case "black": cpos.type = Block.MsgBlack; break;
                    case "white": cpos.type = Block.MsgWhite; break;
                  //  case "show": showMBs(p); return;
                    default: cpos.type = Block.MsgWhite; cpos.message = message; break;
                }
            }
            catch { cpos.type = Block.MsgWhite; cpos.message = message; }

            string current = "";
            string cmdparts = "";

            /*Fix by alecdent*/
            try
            {
                foreach (var com in Command.all.commands)
                {
                    if (com.type.Contains("mod"))
                    {
                        current = "/" + com.name;

                        cmdparts = message.Split(' ')[0].ToLower().ToString();
                        if (cmdparts[0] == '/')
                        {
                            if (current == cmdparts.ToLower())
                            {
                                p.SendMessage("You can't use that command in your messageblock!");
                                return;
                            }
                        }

                        cmdparts = message.Split(' ')[1].ToLower().ToString();
                        if (cmdparts[0] == '/')
                        {
                            if (current == cmdparts.ToLower())
                            {
                                p.SendMessage("You can't use that command in your messageblock!");
                                return;
                            }
                        }
                        if (com.shortcut != "")
                        {
                            current = "/" + com.name;

                            cmdparts = message.Split(' ')[0].ToLower().ToString();
                            if (cmdparts[0] == '/')
                            {
                                if (current == cmdparts.ToLower())
                                {
                                    p.SendMessage("You can't use that command in your messageblock!");
                                    return;
                                }
                            }

                            cmdparts = message.Split(' ')[1].ToLower().ToString();
                            if (cmdparts[0] == '/')
                            {
                                if (current == cmdparts.ToLower())
                                {
                                    p.SendMessage("You can't use that command in your messageblock!");
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            if (cpos.message == "") cpos.message = message.Substring(message.IndexOf(' ') + 1);
            p.blockchangeObject = cpos;

            Player.SendMessage(p, "Place where you wish the message block to go."); p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/mb [block] [message] - Places a message in your next block.");
            Player.SendMessage(p, "Valid blocks: white, black, air, water, lava");
            Player.SendMessage(p, "/mb show shows or hides MBs");
        }

        public void Blockchange1(Player p, ushort x, ushort y, ushort z, ushort type)
        {
            p.ClearBlockchange();
            CatchPos cpos = (CatchPos)p.blockchangeObject;

            if ( p.level.permissionbuild > p.group.Permission ) {
                Player.SendMessage( p, "You do not have permission to build here!" );
                return;
            }
            ushort currblock = p.level.GetTile( x, y, z );
            if ( Block.Mover( currblock ) ) {
                Player.SendMessage( p, "Messageblock cannot be placed here!" );
                return;
            }

            cpos.message = cpos.message.Replace("'", "\\'");

            if (!Regex.IsMatch(cpos.message.ToLower(), @".*%([0-9]|[a-f]|[k-r])%([0-9]|[a-f]|[k-r])%([0-9]|[a-f]|[k-r])"))
            {
                if (Regex.IsMatch(cpos.message.ToLower(), @".*%([0-9]|[a-f]|[k-r])(.+?).*"))
                {
                    Regex rg = new Regex(@"%([0-9]|[a-f]|[k-r])(.+?)");
                    MatchCollection mc = rg.Matches(cpos.message.ToLower());
                    if (mc.Count > 0)
                    {
                        Match ma = mc[0];
                        GroupCollection gc = ma.Groups;
                        cpos.message.Replace("%" + gc[1].ToString().Substring(1), "&" + gc[1].ToString().Substring(1));
                    }
                }
            }
			MessageBlock foundMessageBlock = null;

			foreach ( MessageBlock mb in MessageBlockDB.messageBlocks ) {
				if ( mb.level == p.level.name && mb.x == x && mb.y == y && mb.z == z ) {
					mb.message = cpos.message;
					foundMessageBlock = mb;
				}
			}

			if ( foundMessageBlock == null ) {
				MessageBlock newMessageBlock = new MessageBlock( x, y, z, p.level.name, cpos.message );
				MessageBlockDB.messageBlocks.Add( newMessageBlock );
				MessageBlockDB.Save();
			}
            Player.SendMessage(p, "Message block placed.");
            p.level.Blockchange(p, x, y, z, cpos.type);
            p.SendBlockchange(x, y, z, cpos.type);

            if (p.staticCommands) p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }

        struct CatchPos { public string message; public ushort type; }

    }
}