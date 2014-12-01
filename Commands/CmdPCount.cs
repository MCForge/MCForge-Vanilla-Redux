/*
	Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCForge)
 
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
using System.Data;
using System.IO;

namespace MCForge.Commands
{
    public class CmdPCount : Command
    {
        public override string name { get { return "pcount"; } }
        public override string shortcut { get { return  ""; } }
        public override string type { get { return "information"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdPCount() { }

        public override void Use(Player p, string message)
        {
            int bancount = Group.findPerm(LevelPermission.Banned).playerList.All().Count;

			int count = Directory.GetFiles("players/", "*.txt", SearchOption.TopDirectoryOnly).Length;
            Player.SendMessage(p, "A total of " + count + " unique players have visited this server.");
            Player.SendMessage(p, "Of these players, " + bancount + " have been banned.");

            int playerCount = 0;
            int hiddenCount = 0;
           
            foreach (Player pl in Player.players)
            {
                if (!pl.hidden || p == null || p.group.Permission > LevelPermission.AdvBuilder)
                {
                    playerCount++;
                    if (pl.hidden && pl.group.Permission <= p.group.Permission && (p == null || p.group.Permission > LevelPermission.AdvBuilder))
                    {
                        hiddenCount++;
                    }
                }
            }
            if (playerCount == 1)
            {
                if (hiddenCount == 0)
                {
                    Player.SendMessage(p, "There is 1 player currently online.");
                }
                else
                {
                    Player.SendMessage(p, "There is 1 player currently online (" + hiddenCount + " hidden).");
                }
            }
            else
            {
                if (hiddenCount == 0)
                {
                    Player.SendMessage(p, "There are " + playerCount + " players online.");
                }
                else
                {
                    Player.SendMessage(p, "There are " + playerCount + " players online (" + hiddenCount + " hidden).");
                }
            }
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/pcount - Displays the number of players online and total.");
        }
    }
}
