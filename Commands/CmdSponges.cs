/*
	Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCForge)
 
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.osedu.org/licenses/ECL-2.0
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/

using System;

namespace MCForge
{
    public class CmdSponges : Command
    {
        public override string name { get { return "sponges"; } }
        public override string shortcut { get { return "sp"; } }
        public override string type { get { return "lava"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdSponges() { }
        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                Player.SendMessage(p, "You currently have " + p.spongesLeft + " sponges.");
            }
            else
            {
                Player who = Player.Find(message);
                if (who == null)
                {
                    Player.SendMessage(p, "Error: Player is not online.");
                    return;
                }
                if (who.group.Permission >= p.group.Permission)
                {
                    Player.SendMessage(p, "Cannot see the sponges of someone of equal or greater rank.");
                    return;
                }

                Player.SendMessage(p, who.color + who.name + Server.DefaultColor + " currently has " + who.spongesLeft + " sponges.");
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/sponges [player] - Checks how many sponges you have (or [player has)!");
        }
    }
}