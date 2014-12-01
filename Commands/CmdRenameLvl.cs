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

namespace MCForge.Commands
{
    public class CmdRenameLvl : Command
    {
        public override string name { get { return "renamelvl"; } }
        public override string shortcut { get { return  ""; } }
        public override string type { get { return "mod"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdRenameLvl() { }

        public override void Use(Player p, string message)
        {
            if (message == "" || message.IndexOf(' ') == -1) { Help(p); return; }
            Level foundLevel = Level.FindExact(message.Split(' ')[0]);
            if (foundLevel == null)
            {
                Player.SendMessage(p, "Level not found");
                return;
            }

            string newName = message.Split(' ')[1];

            if (File.Exists("levels/" + newName + ".mcf")) { Player.SendMessage(p, "Level already exists."); return; }
            if (foundLevel == Server.mainLevel) { Player.SendMessage(p, "Cannot rename the main level."); return; }

            foundLevel.Unload();

            try
            {
                File.Move("levels/" + foundLevel.name + ".mcf", "levels/" + newName + ".mcf");
                File.Move("levels/" + foundLevel.name + ".mcf.backup", "levels/" + newName + ".mcf.backup");

                try
                {
                    File.Move("levels/level properties/" + foundLevel.name + ".properties", "levels/level properties/" + newName + ".properties");
                }
                catch { }
                try
                {
                    File.Move("levels/level properties/" + foundLevel.name, "levels/level properties/" + newName + ".properties");
                }
                catch { }

                //Move and rename backups
                try
                {
                    string foundLevelDir, newNameDir;
                    for (int i = 1; ; i++)
                    {
                        foundLevelDir = @Server.backupLocation + "/" + foundLevel.name + "/" + i + "/";
                        newNameDir = @Server.backupLocation + "/" + newName + "/" + i + "/";

                        if (File.Exists(foundLevelDir + foundLevel.name + ".mcf"))
                        {
                            Directory.CreateDirectory(newNameDir);
                            File.Move(foundLevelDir + foundLevel.name + ".mcf", newNameDir + newName + ".mcf");
                            if (DirectoryEmpty(foundLevelDir))
                                Directory.Delete(foundLevelDir);
                        }
                        else
                        {
                            if (DirectoryEmpty(@Server.backupLocation + "/" + foundLevel.name + "/"))
                                Directory.Delete(@Server.backupLocation + "/" + foundLevel.name + "/");
                            break;
                        }
                    }
                }
                catch { }
                try { Command.all.Find("load").Use(p, newName); }
                catch { }
                Player.GlobalMessage("Renamed " + foundLevel.name + " to " + newName);
            }
            catch (Exception e) { Player.SendMessage(p, "Error when renaming."); Server.ErrorLog(e); }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/renamelvl <level> <new name> - Renames <level> to <new name>");
            Player.SendMessage(p, "Portals going to <level> will be lost");
        }

        public static bool DirectoryEmpty(string dir)
        {
            if (!Directory.Exists(dir))
                return true;
            if (Directory.GetDirectories(dir).Length > 0)
                return false;
            if (Directory.GetFiles(dir).Length > 0)
                return false;

            return true;
        }
    }
}