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
    class CmdDrop : Command
    {
        public override string shortcut { get { return  ""; } }
        public override bool museumUsable { get { return true; } }
        public override string name { get { return "drop"; } }
        public override string type { get { return "other"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdDrop() { }

        public override void Use(Player p, string message)
        {
            bool silent = false;
            if (message == "silent") { silent = true; }
            if (CTF.redTeam.hasFlag == p)
            {
                CTF.redTeam.hasFlag = null;
                p.carryingFlag = false;
                if (!silent) Player.GlobalMessage("&f- " + p.color + p.name + "&S dropped the &cRed&S flag!");
                p.justDroppedFlag = true;
                Thread.Sleep(1000);
                p.justDroppedFlag = false;
            }
            else if (CTF.blueTeam.hasFlag == p)
            {
                CTF.blueTeam.hasFlag = null;
                p.carryingFlag = false;
                if (!silent) Player.GlobalMessage("&f- " + p.color + p.name + "&S dropped the &9Blue&S flag!");
                p.justDroppedFlag = true;
                Thread.Sleep(1000);
                p.justDroppedFlag = false;
            }
            else
            {
                if (!silent) p.SendMessage("You're not carrying a flag...");
            }

            p.carryingFlag = false;
        }
        public override void Help(Player p)
        {
            p.SendMessage("/drop - drops the flag.");
        }
    }
}
