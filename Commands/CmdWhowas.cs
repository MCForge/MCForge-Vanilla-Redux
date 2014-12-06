/*
	Copyright © 2009-2014 MCSharp team (Modified for use with MCZall/MCLawl/MCForge/MCForge-Redux)
	
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
using System.Data;
using System.IO;
using System.Text;
namespace MCForge.Commands
{
    public class CmdWhowas : Command
    {
        public override string name { get { return "whowas"; } }
        public override string shortcut { get { return  ""; } }
        public override string type { get { return "information"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdWhowas() { }

        public override void Use(Player p, string message)
        {
			message = message.ToLower ();
            if (message == "") { Help(p); return; }
            Player pl = Player.Find(message);
            if (pl != null && !pl.hidden)
            {
                Player.SendMessage(p, pl.color + pl.name + Server.DefaultColor + " is online, using /whois instead.");
                Command.all.Find("whois").Use(p, message);
                return;
            }

            if (message.IndexOf("'") != -1) { Player.SendMessage(p, "Cannot parse request."); return; }

            string FoundRank = Group.findPlayer(message.ToLower());
            if (!Load(message)) { Player.SendMessage(p, Group.Find(FoundRank).color + message + Server.DefaultColor + " has the rank of " + Group.Find(FoundRank).color + FoundRank); return; }

            if (title == "" || title == null || String.IsNullOrEmpty(title))
                Player.SendMessage(p, color + message + Server.DefaultColor + " has :");
            else
                Player.SendMessage(p, color + "[" + titlecolor + title + color + "] " + message + Server.DefaultColor + " has :");
            Player.SendMessage(p, "> > the rank of " + Group.Find(FoundRank).color + FoundRank);
            try
            {
                if (!Group.Find("Nobody").commands.Contains("pay") && !Group.Find("Nobody").commands.Contains("give") && !Group.Find("Nobody").commands.Contains("take")) Player.SendMessage(p, "> > &a" + money + Server.DefaultColor + " " + Server.moneys);
            }
            catch { }
            Player.SendMessage(p, "> > &cdied &a" + overalldeaths + Server.DefaultColor + " times");
            Player.SendMessage(p, "> > &bmodified &a" + overallblocks + " &eblocks.");
            Player.SendMessage(p, "> > was last seen on &a" + lastlogin);
            Player.SendMessage(p, "> > first logged into the server on &a" + firstlogin);
            Player.SendMessage(p, "> > logged in &a" + totallogins + Server.DefaultColor + " times, &c" + totalkicks + Server.DefaultColor + " of which ended in a kick.");
            Player.SendMessage(p, "> > " + Awards.awardAmount(message) + " awards");
            if (Ban.Isbanned(message))
            {
                string[] data = Ban.Getbandata(message);
                Player.SendMessage(p, "> > was banned by " + data[0] + " for " + data[1] + " on " + data[2]);
            }

            if (Server.Devs.Contains(message.ToLower()))
            {
                Player.SendMessage(p, Server.DefaultColor + "> > Player is a &9Developer");
                if (Server.forgeProtection == ForgeProtection.Mod || Server.forgeProtection == ForgeProtection.Dev)
                    Player.SendMessage(p, Server.DefaultColor + "> > Player is &CPROTECTED" + Server.DefaultColor + " under MCForge Staff protection");
            }
            else if (Server.Mods.Contains(message.ToLower()))
            {
                Player.SendMessage(p, Server.DefaultColor + "> > Player is a &9MCForge Moderator");
                if (Server.forgeProtection == ForgeProtection.Mod)
                    Player.SendMessage(p, Server.DefaultColor + "> > Player is &CPROTECTED" + Server.DefaultColor + " under MCForge Staff protection");
            }
            else if (Server.GCmods.Contains(message.ToLower()))
                Player.SendMessage(p, Server.DefaultColor + "> > Player is a &9Global Chat Moderator");

            if (!(p != null && (int)p.group.Permission <= CommandOtherPerms.GetPerm(this)))
			{
                if (Server.useWhitelist)
                {
                    if (Server.whiteList.Contains(message.ToLower()))
                    {
                        Player.SendMessage(p, "> > Player is &fWhitelisted");
                    }
                }
            }
        }
		string title, titlecolor, color;
		int money, totallogins, totalkicks, overalldeaths, overallblocks;
		DateTime firstlogin, lastlogin;
		public bool Load(string playerName)
		{			
			if ( File.Exists( "players/" + playerName.ToLower() + "DB.txt" ) ) {
				foreach (string line in File.ReadAllLines( "players/" + playerName.ToLower() + "DB.txt" )) {
					if (!string.IsNullOrEmpty (line) && !line.StartsWith ("#")) {
						string key = line.Split ('=') [0].Trim ();
						string value = line.Split ('=') [1].Trim ();
						string section = "nowhere yet...";

						try {
							switch (key.ToLower ()) {
							case "title":
								title = value;
								section = key;
								break;
							case "titlecolor":
								titlecolor = value;
								section = key;
								break;
							case "color":
								color = value;
								section = key;
								break;
							case "money":
								money = int.Parse (value);
								section = key;
								break;
							case "firstlogin":
								firstlogin = DateTime.Parse (value);
								section = key;
								break;
							case "lastlogin":
								lastlogin = DateTime.Parse (value);
								section = key;
								break;
							case "totallogins":
								totallogins = int.Parse (value) + 1;
								section = key;
								break;
							case "totalkicked":
								totalkicks = int.Parse (value);
								section = key;
								break;
							case "overalldeath":
								overalldeaths = int.Parse (value);
								section = key;
								break;
							case "overallblocks":
								overallblocks = int.Parse (value);
								section = key;
								break;
							}
							return true;
						} catch (Exception e) {
							return false;
						}
					}
				}
			}
			
			return false;
		}
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/whowas <name> - Displays information about someone who left.");
        }
    }
}
