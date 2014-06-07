/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCFlame) Licensed under the
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
using System.IO;
using System.Collections.Generic;
using System.Threading;

namespace MCFlame
{
    public class CmdLoad : Command
    {
        public override string name { get { return "load"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "mod"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdLoad() { }

        public override void Use(Player p, string message)
        {
            try
            {
                if (!Directory.Exists("levels/byte")) Directory.CreateDirectory("levels/byte");
                bool bite = message[0] == '#';
                if (bite) message = message.Substring(1);
                if (message == "") { Help(p); return; }
                if (message.Split(' ').Length > 2) { Help(p); return; }
                int pos = message.IndexOf(' ');
                string phys = "0";
                if (pos != -1)
                {
                    phys = message.Substring(pos + 1);
                    message = message.Substring(0, pos).ToLower();
                }
                else
                {
                    message = message.ToLower();
                }

                foreach (Level l in Server.levels)
                {
                    if (l.name == message) { Player.SendMessage(p, message + " is already loaded!"); return; }
                }

                if (Server.levels.Count == Server.levels.Capacity)
                {
                    if (Server.levels.Capacity == 1)
                    {
                        Player.SendMessage(p, "You can't load any levels!");
                    }
                    else
                    {
                        Command.all.Find("unload").Use(p, "empty");
                        if (Server.levels.Capacity == 1)
                        {
                            Player.SendMessage(p, "No maps are empty to unload. Cannot load map.");
                            return;
                        }
                    }
                }
                if (bite) Player.SendMessage(p, c.red + "Loading byte map!");
                if (!bite)
                {
                    if (!File.Exists("levels/" + message + ".lvl"))
                    {
                        Player.SendMessage(p, "Level \"" + message + "\" doesn't exist!!"); return;
                    }
                }
                else
                {
                    if (!File.Exists("levels/byte/" + message + ".lvl"))
                    {
                        Player.SendMessage(p, "Level \"" + message + "\" doesn't exist!"); return;
                    }
                }

                Level level = Level.Load(message, 0, bite);

                if (level == null)
                {
                    if (File.Exists("levels/" + message + ".lvl.backup"))
                    {
                        Server.s.Log("Attempting to load backup.");
                        File.Copy("levels/" + message + ".lvl.backup", "levels/" + message + ".lvl", true);
                        level = Level.Load(message);
                        if (level == null)
                        {
                            Player.SendMessage(p, "Backup of " + message + " failed.");
                            return;
                        }
                    }
                    else
                    {
                        Player.SendMessage(p, "Backup of " + message + " does not exist.");
                        return;
                    }
                }

                if (p != null) if (level.permissionvisit > p.group.Permission)
                    {
                        Player.SendMessage(p, "This map is for " + Level.PermissionToName(level.permissionvisit) + " only!");
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        return;
                    }

                foreach (Level l in Server.levels)
                {
                    if (l.name == message)
                    {
                        Player.SendMessage(p, message + " is already loaded!");
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        return;
                    }
                }

                lock (Server.levels) {
                    Server.addLevel(level);
                }

                level.physThread.Start();
                Player.GlobalMessage("Level \"" + level.name + "\" loaded.");
                try
                {
                    int temp = int.Parse(phys);
                    if (temp >= 1 && temp <= 4)
                    {
                        level.setPhysics(temp);
                    }
                }
                catch
                {
                    Player.SendMessage(p, "Physics variable invalid");
                }
            }
            catch (Exception e)
            {
                Player.GlobalMessage("An error occured with /load");
                Server.ErrorLog(e);
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/load #<level> <physics> - Loads a level, # indicates it's a byte formatted level.");
        }
    }
}