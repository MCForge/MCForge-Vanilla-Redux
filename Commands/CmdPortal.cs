/*
	Copyright © 2011-2014 MCForge-Redux
		
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
using System.Collections.Generic;
using System.Data;

namespace MCForge.Commands
{
    public class CmdPortal : Command
    {
        public override string name { get { return "portal"; } }
        public override string shortcut { get { return  "o"; } }
        public override string type { get { return "build"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdPortal() { }

        public override void Use(Player p, string message)
        {
            portalPos portalPos;

            portalPos.Multi = false;

            if (message.IndexOf(' ') != -1)
            {
                if (message.Split(' ')[1].ToLower() == "multi")
                {
                    portalPos.Multi = true;
                    message = message.Split(' ')[0];
                }
                else
                {
                    Player.SendMessage(p, "Invalid parameters");
                    return;
                }
            }

            if (message.ToLower() == "blue" || message == "") { portalPos.type = Block.blue_portal; }
            else if (message.ToLower() == "orange") { portalPos.type = Block.orange_portal; }
            else if (message.ToLower() == "air") { portalPos.type = Block.air_portal; }
            else if (message.ToLower() == "water") { portalPos.type = Block.water_portal; }
            else if (message.ToLower() == "lava") { portalPos.type = Block.lava_portal; }
          //  else if (message.ToLower() == "show") { showPortals(p); return; }
            else { Help(p); return; }

            p.ClearBlockchange();

            portPos port;

            port.x = 0; port.y = 0; port.z = 0; port.portMapName = "";
            portalPos.port = new List<portPos>();

            p.blockchangeObject = portalPos;
            Player.SendMessage(p, "Place an &aEntry block" + Server.DefaultColor + " for the portal"); p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(EntryChange);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/portal [orange/blue/air/water/lava] [multi] - Activates Portal mode.");
            Player.SendMessage(p, "/portal [type] multi - Place Entry blocks until exit is wanted.");
            Player.SendMessage(p, "/portal show - Shows portals, green = in, red = out.");
        }

        public void EntryChange(Player p, ushort x, ushort y, ushort z, ushort type)
        {
            p.ClearBlockchange();
            portalPos bp = (portalPos)p.blockchangeObject;

            if ( p.level.permissionbuild > p.group.Permission ) {
                Player.SendMessage( p, "You do not have permission to build here!" );
                return;
            }
            ushort currblock = p.level.GetTile( x, y, z );
            if ( Block.Mover( currblock ) ) {
                Player.SendMessage( p, "Portal cannot be placed here!" );
                return;
            }

            if (bp.Multi && type == Block.red && bp.port.Count > 0) { ExitChange(p, x, y, z, type); return; }

          //  ushort b = p.level.GetTile(x, y, z);
            p.level.Blockchange(p, x, y, z, bp.type);
            p.SendBlockchange(x, y, z, Block.green);
            portPos Port;

            Port.portMapName = p.level.name;
            Port.x = x; Port.y = y; Port.z = z;

            bp.port.Add(Port);

            p.blockchangeObject = bp;

            if (!bp.Multi)
            {
                p.Blockchange += ExitChange;
                Player.SendMessage(p, "&aEntry block placed");
            }
            else
            {
                p.Blockchange += EntryChange;
                Player.SendMessage(p, "&aEntry block placed. &cRed block for exit");
            }
        }
        public void ExitChange(Player p, ushort x, ushort y, ushort z, ushort type)
        {
            p.ClearBlockchange();
            ushort b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            portalPos bp = (portalPos)p.blockchangeObject;
			foreach ( portPos pos in bp.port ) {
			Portal foundPortal = null;
			Portal newPortal = null;

			foreach ( Portal po in PortalDB.portals ) {
				if ( po.entrance.ToLower() == pos.portMapName.ToLower() && po.x1 == pos.x && po.y1 == pos.y && po.z1 == pos.z ) {
					foundPortal = po;
				}
			}

			if ( foundPortal != null ) {
				foundPortal.exit = p.level.name.ToLower();
				foundPortal.x2 = x;
				foundPortal.y2 = y;
				foundPortal.z2 = z;
			} else {
				newPortal = new Portal( pos.portMapName.ToLower() + "-" + p.level.name.ToLower(), pos.portMapName.ToLower(), p.level.name, pos.x.ToString(), pos.y.ToString(), pos.z.ToString(), x.ToString(), y.ToString(), z.ToString() );
				PortalDB.portals.Add( newPortal );
			}
				if ( pos.portMapName == p.level.name ) p.SendBlockchange( pos.x, pos.y, pos.z, bp.type );
			}
			PortalDB.Save ();
			Player.SendMessage( p, "&cExit" + Server.DefaultColor + " block placed." );
            if (!p.staticCommands)
                return;
            bp.port.Clear(); p.blockchangeObject = bp; p.Blockchange += EntryChange;
        }

        public struct portalPos { public List<portPos> port; public ushort type; public bool Multi; }
        public struct portPos { public ushort x, y, z; public string portMapName; }
    }
}