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
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace MCForge.Commands
{
    class CmdBlueTeam : Command
    {
        public override string shortcut { get { return ""; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override string name { get { return "blue"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdBlueTeam() { }

        public override void Use(Player p, string message)
        {
            switch (message.ToLower())
            {
                case "":
                    if (p.team == CTF.blueTeam)
                    {
                        p.SendMessage("You're already on that team!");
                        return;
                    }

                    if (p.team == CTF.redTeam)
                    {
                        if (CTF.blueTeam.players.Count <= CTF.redTeam.players.Count)
                        {
                            CTF.redTeam.DelPlayer(p);
                            CTF.blueTeam.AddPlayer(p);
                        }
                        else
                        {
                            p.SendMessage("Joining the blue team would cause imbalance!");
                        }
                        return;
                    }

                    CTF.blueTeam.AddPlayer(p);
                    break;
            }
        }

        public override void Help(Player p)
        {
            p.SendMessage("/blue - joins the blue team.");
            p.SendMessage("&cThese require Op+:");
            p.SendMessage("/blue spawn - sets the blue teams spawn location for that map.");
            p.SendMessage("/blue flag - sets the blue teams flag location for that map.");
        }
    }
}
