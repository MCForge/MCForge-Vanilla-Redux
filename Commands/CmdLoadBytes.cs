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

using System;
using System.IO;
using System.Threading;
namespace MCForge.Commands
{
    public sealed class CmdLoadByte : Command
    {
        public override string name { get { return "loadbyte"; } }
        public override string shortcut { get { return  "lb"; } }
        public override string type { get { return "mod"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdLoadByte() { }

        public override void Use(Player p, string message)
        {
            try
            {
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

                while (Server.levels == null) Thread.Sleep(100); // Do nothing while we wait on the levels list...

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

                if (!File.Exists("levels/byte/" + message + ".lvl"))
                {
                    Player.SendMessage(p, "Level \"" + message + "\" doesn't exist!"); return;
                }

                Level level = Level.Load(message, 0, true);

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

                lock (Server.levels)
                {
                    Server.addLevel(level);
                }
                Player.GlobalMessage("Level \"" + level.name + "\" loaded.");
                /*try
                {
                    Gui.Window.thisWindow.UpdatePlayerMapCombo();
                    Gui.Window.thisWindow.UnloadedlistUpdate();
                    Gui.Window.thisWindow.UpdateMapList("'");
                   
                    
                }
                catch { }*/
                try
                {
                    int temp = int.Parse(phys);
                    if (temp >= 1 && temp <= 5)
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
            Player.SendMessage(p, "/loadbyte <level> <physics> - Converts and loads a level");
        }
    }
}