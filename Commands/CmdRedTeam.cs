/*
	Copyright 2010 MCSharp Team Licensed under the
	Educational Community License, Version 2.0 (the "License"); you may
	not use this file except in compliance with the License. You may
	obtain a copy of the License at
	
	http://www.osedu.org/licenses/ECL-2.0
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the License is distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the License for the specific language governing
	permissions and limitations under the License.
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace MCForge.Commands
{
    class CmdRedTeam : Command
    {
        public override string shortcut { get { return ""; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override string name { get { return "red"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdRedTeam() { }

        public override void Use(Player p, string message)
        {
            switch (message.ToLower())
            {
                case "":
                    if (p.team == CTF.redTeam)
                    {
                        p.SendMessage("You're already on that team!");
                        return;
                    }

                    if (p.team == CTF.blueTeam)
                    {
                        if (CTF.redTeam.players.Count <= CTF.blueTeam.players.Count)
                        {
                            CTF.blueTeam.DelPlayer(p);
                            CTF.redTeam.AddPlayer(p);
                        }
                        else
                        {
                            p.SendMessage("Joining the red team would cause imbalance!");
                        }
                        return;
                    }

                    CTF.redTeam.AddPlayer(p);
                    break;
            }
        }

        public override void Help(Player p)
        {
            p.SendMessage("/red - joins the red team.");
            p.SendMessage("&cThese require Op+:");
            p.SendMessage("/red spawn - sets the red teams spawn location for that map.");
            p.SendMessage("/red flag - sets the red teams flag location for that map.");
        }
    }
}
