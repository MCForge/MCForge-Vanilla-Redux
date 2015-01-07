/*
	Copyright � 2011-2014 MCForge-Redux
	
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
namespace MCForge.Commands
{
    public class CmdFollow : Command
    {
        public override string name { get { return "follow"; } }
        public override string shortcut { get { return  ""; } }
        public override string type { get { return "mod"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdFollow() { }

        public override void Use(Player p, string message)
        {
            if (p == null)
            {
                Player.SendMessage(p, "This command can only be used in-game");
                return;
            }
            if (!p.canBuild)
            {
                Player.SendMessage(p, "You're currently being &4possessed" + Server.DefaultColor + "!");
                return;
            }
            try
            {
                bool stealth = false;

                if (message != "")
                {
                    if (message == "#")
                    {
                        if (p.following != "")
                        {
                            stealth = true;
                            message = "";
                        }
                        else
                        {
                            Help(p);
                            return;
                        }
                    }
                    else if (message.IndexOf(' ') != -1)
                    {
                        if (message.Split(' ')[0] == "#")
                        {
                            if (p.hidden) stealth = true;
                            message = message.Split(' ')[1];
                        }
                    }
                }

                Player who = Player.Find(message);
                if (message == "" && p.following == "") {
                    Help(p);
                    return;
                }
                else if (message == "" && p.following != "" || message == p.following)
                {
                    who = Player.Find(p.following);
                    p.following = "";
                    if (p.hidden)
                    {
                        if (who != null)
                            p.SendSpawn(who.id, who.color + who.name, who.pos[0], who.pos[1], who.pos[2], who.rot[0], who.rot[1], who.DisplayName, who.SkinName);
                        if (!stealth)
                        {
                            Command.all.Find("hide").Use(p, "");
                        }
                        else
                        {
                            if (who != null)
                            {
                                Player.SendMessage(p, "You have stopped following " + who.color + who.name + Server.DefaultColor + " and remained hidden.");
                            }
                            else
                            {
                                Player.SendMessage(p, "Following stopped.");
                            }
                        }
                        return;
                    }
                }
                if (who == null) { Player.SendMessage(p, "Could not find player."); return; }
                else if (who == p) { Player.SendMessage(p, "Cannot follow yourself."); return; }
                else if (who.group.Permission >= p.group.Permission) { Player.SendMessage(p, "Cannot follow someone of equal or greater rank."); return; }
                else if (who.following != "") { Player.SendMessage(p, who.name + " is already following " + who.following); return; }

                if (!p.hidden) Command.all.Find("hide").Use(p, "");

                if (p.level != who.level) Command.all.Find("tp").Use(p, who.name);
                if (p.following != "")
                {
                    who = Player.Find(p.following);
                    p.SendSpawn(who.id, who.color + who.name, who.pos[0], who.pos[1], who.pos[2], who.rot[0], who.rot[1], who.DisplayName, who.SkinName);
                }
                who = Player.Find(message);
                p.following = who.name;
                Player.SendMessage(p, "Following " + who.name + ". Use \"/follow\" to stop.");
                p.SendDie(who.id);
            }
            catch (Exception e) { Server.ErrorLog(e); Player.SendMessage(p, "Error occured"); }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/follow <name> - Follows <name> until the command is cancelled");
            Player.SendMessage(p, "/follow # <name> - Will cause /hide not to be toggled");
        }
    }
}