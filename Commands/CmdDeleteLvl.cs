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
using System.IO;

namespace MCForge.Commands {
    public class CmdDeleteLvl : Command {
        public override string name { get { return "deletelvl"; } }
        public override string shortcut { get { return  ""; } }
        public override string type { get { return "mod"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdDeleteLvl() { }

        public override void Use(Player p, string message) {
            if (message == "" || message.Split().Length > 1) { Help(p); return; }
            Level foundLevel = Level.FindExact(message);
            if (foundLevel != null) {
                if (foundLevel.permissionbuild > p.group.Permission) {
                    Player.SendMessage(p, "%cYou can't delete levels with a perbuild rank higher than yours!");
                    return;
                }
                foundLevel.Unload();
            }

            if (foundLevel == Server.mainLevel) { Player.SendMessage(p, "Cannot delete the main level."); return; }

            try {
                if (!Directory.Exists("levels/deleted")) Directory.CreateDirectory("levels/deleted");

                if (File.Exists("levels/" + message + ".mcf")) {

                    using (StreamReader reader = new StreamReader("levels/level properties/" + message + ".properties")) {
                        string line;
                        while ((line = reader.ReadLine()) != null) {
                            if (line[0] == '#') continue;
                            if (line.Split()[0].ToLower() == "perbuild") {
                                if (Level.PermissionFromName(line.Split()[2].ToLower()) > p.group.Permission) {
                                    Player.SendMessage(p, "%cYou can't delete levels with a perbuild rank higher than yours!");
                                    return;
                                }
                                break;
                            }
                        }
                    }

                    if (File.Exists("levels/deleted/" + message + ".mcf")) {
                        int currentNum = 0;
                        while (File.Exists("levels/deleted/" + message + currentNum + ".mcf")) currentNum++;

                        File.Move("levels/" + message + ".mcf", "levels/deleted/" + message + currentNum + ".mcf");
                    } else {
                        File.Move("levels/" + message + ".mcf", "levels/deleted/" + message + ".mcf");
                    }
                    Player.SendMessage(p, "Created backup.");

                    try { File.Delete("levels/level properties/" + message + ".properties"); } catch { }
                    try { File.Delete("levels/level properties/" + message); } catch { }


                    Player.GlobalMessage("Level " + message + " was deleted.");
                } else {
                    Player.SendMessage(p, "Could not find specified level.");
                }
            } catch (Exception e) { Player.SendMessage(p, "Error when deleting."); Server.ErrorLog(e); }
        }
        public override void Help(Player p) {
            Player.SendMessage(p, "/deletelvl [map] - Completely deletes [map] (portals, MBs, everything");
            Player.SendMessage(p, "A backup of the map will be placed in the levels/deleted folder");
        }
    }
}