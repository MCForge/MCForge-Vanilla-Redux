/*
	Copyright © 2011-2014 MCForge-Redux
		
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.osedu.org/licenses/ECL-2.0
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MCForge
{
    public class CmdStore : Command
    {
        public override string name { get { return "store"; } }
        public override string shortcut { get { return "buy"; } }
        public override string type { get { return "zombie"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdStore() { }
        public ZombieGame zs;
        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                p.SendMessage("To purchase an item, type /buy [item number] (eg. /buy 5 for a rank)");
                p.SendMessage("With title and title color, use /buy 1 [title] OR /buy 2 [color]");
                p.SendMessage("With prefix/login message, logout message use /buy 6/7/8 [message]");
                p.SendMessage("With blocks, use /buy 4 [blocks]");
                p.SendMessage(c.blue + "1. " + Server.DefaultColor + "Title - 150 " + Server.moneys);
                p.SendMessage(c.blue + "2. " + Server.DefaultColor + "Title Color - 75 " + Server.moneys);
                p.SendMessage(c.blue + "3. " + Server.DefaultColor + "Disinfectant - 25 " + Server.moneys);
                p.SendMessage(c.blue + "4. " + Server.DefaultColor + "Extra Blocks - 0.1 " + Server.moneys + " Per Block (Rounds to nearest noodle)");
                if (p.group.name.ToLower() == "advbuilder" || p.group.CanExecute(Command.all.Find("shank")))
                {
                    p.SendMessage(c.blue + "5. " + Server.DefaultColor + "Out of stock");
                }
                else
                {
                    p.SendMessage(c.blue + "5. " + Server.DefaultColor + "Rank Up - 250 " + Server.moneys);
                }

                p.SendMessage(c.blue + "6. " + Server.DefaultColor + "Login Message - 100 " + Server.moneys);
                p.SendMessage(c.blue + "7. " + Server.DefaultColor + "Logout Message - 100 " + Server.moneys);
                p.SendMessage(c.blue + "8. " + Server.DefaultColor + "Infect Message - 100 " + Server.moneys);
            }
            else
            {

                string message1 = "";
                int pos = message.IndexOf(' ');
                if (message.Split(' ').Length == 1) { }
                else
                {
                    if (message.Split(' ').Length > 1) message1 = message.Substring(pos + 1);
                    message = message.Split(' ')[0];
                }

                int id = int.Parse(message);

                switch (id)
                {
                    case 1:
                        if (p.money >= 150)
                        {
                            if (!message1.Equals(""))
                            {
                                if (message1.Length < 30)
                                {
                                    Command.all.Find("title").Use(p, p.name + " " + message1);
                                    p.money = p.money - 150;
                                }
                                else
                                {
                                    p.SendMessage("Title must be less than 30 characters!");
                                }
                            }
                            else
                            {
                                p.SendMessage("You have to specify a title (e.g /buy 1 hello)");
                            }
                        }
                        else
                        {
                            p.SendMessage("You do not have enough " + Server.moneys);
                        }
                        break;
                    case 2:
                        if (p.money >= 75)
                        {
                            if (!message1.Equals(""))
                            {
                                string color = c.Parse(message1);
                                if (color == "") { Player.SendMessage(p, "There is no color \"" + message1 + "\"."); break; }
                                if (p.prefix == "") { Player.SendMessage(p, "You must have a title."); break; }
                                Command.all.Find("tcolor").Use(p, p.name + " " + message1);
                                p.money = p.money - 75;
                            }
                            else
                            {
                                p.SendMessage("You have to specify a color (e.g /buy 2 gold)");
                            }
                        }
                        else
                        {
                            p.SendMessage("You do not have enough " + Server.moneys);
                        }
                        break;
                    case 3:
                        if (Server.zombie.GameInProgess())
                        {
                            if (p.money >= 25)
                            {
                                if (ZombieGame.infectd.Contains(p))
                                {
                                    Command.all.Find("disinfect").Use(p, p.name);
                                    p.money = p.money - 25;
                                    p.SendMessage("You are now human");
                                }
                                else
                                {
                                    p.SendMessage("You must be infected to disinfect!");
                                }
                            }
                            else
                            {
                                p.SendMessage("You do not have enough " + Server.moneys);
                            }
                        }
                        break;
                    case 4:
                        if (message1 != null)
                        {
                            if (message1 != "")
                            {
                                int message12 = int.Parse(message1);
                                if (p.money >= (Math.Round(0.1 * message12)))
                                {
                                    p.blockCount = p.blockCount + message12;
                                    p.money = p.money - (Convert.ToInt32(Math.Round(0.1 * message12)));
                                    p.SendMessage("You now have " + c.maroon + p.blockCount + Server.DefaultColor + " blocks!");
                                }
                                else
                                {
                                    p.SendMessage("You do not have enough " + Server.moneys);
                                }
                            }
                            else
                            {
                                int message12 = 10;
                                if (p.money >= (Math.Round(0.1 * message12)))
                                {
                                    p.blockCount = p.blockCount + message12;
                                    p.money = p.money - (Convert.ToInt32(Math.Round(0.1 * message12)));
                                    p.SendMessage("You now have " + c.maroon + p.blockCount + Server.DefaultColor + " blocks!");
                                }
                                else
                                {
                                    p.SendMessage("You do not have enough " + Server.moneys);
                                }
                            }
                        }
                        else
                        {
                            p.SendMessage("You need to set an amount of blocks to purchase!");
                        }
                        break;
                    case 5:
                        if (p.group.name.ToLower() == Server.maxrank.ToLower() || p.group.CanExecute(Command.all.Find("shank")))
                        {
                            p.SendMessage("You already have the max rank!");
                            return;
                        }
                        if (p.money >= 250)
                        {
                            p.money = p.money - 250;
                            Command.all.Find("promote").Use(null, p.name);
                        }
                        else
                        {
                            p.SendMessage("You do not have enough " + Server.moneys);
                        }
                        break;
                    case 6:
                        if (p.money >= 100)
                        {
                            if (!message1.Equals(""))
                            {
                                if (message1.Length < 30)
                                {
                                    Command.all.Find("loginmessage").Use(p, p.name + " " + message1);
                                    p.money = p.money - 100;
                                }
                                else
                                {
                                    p.SendMessage("Login Message must be less than 30 characters!");
                                }
                            }
                            else
                            {
                                p.SendMessage("You have to specify a login message (e.g /buy 6 hello)");
                            }
                        }
                        else
                        {
                            p.SendMessage("You do not have enough " + Server.moneys);
                        }
                        break;
                    case 7:
                        if (p.money >= 100)
                        {
                            if (!message1.Equals(""))
                            {
                                if (message1.Length < 30)
                                {
                                    Command.all.Find("logoutmessage").Use(p, p.name + " " + message1);
                                    p.money = p.money - 100;
                                }
                                else
                                {
                                    p.SendMessage("Logout message must be less than 30 characters!");
                                }
                            }
                            else
                            {
                                p.SendMessage("You have to specify a login message (e.g /buy 8 hello)");
                            }
                        }
                        else
                        {
                            p.SendMessage("You do not have enough " + Server.moneys);
                        }
                        break;
                    case 8:
                        if (p.money >= 100)
                        {
                            if (!message1.Equals(""))
                            {
                                if (message1.Length < 30)
                                {
                                    Command.all.Find("infectmessage").Use(p, p.name + " " + message1);
                                    p.money = p.money - 100;
                                }
                                else
                                {
                                    p.SendMessage("Infect message must be less than 30 characters!");
                                }
                            }
                            else
                            {
                                p.SendMessage("You have to specify a infect message (e.g /buy 9 hello)");
                            }
                        }
                        else
                        {
                            p.SendMessage("You do not have enough " + Server.moneys);
                        }
                        break;
                    default:
                        p.SendMessage("To purchase an item, type /buy [item number] (eg. /buy 5 for a rank)");
                        p.SendMessage("With title and title color, use /buy 1 [title] OR /buy 2 [color]");
                        p.SendMessage("With prefix/login message, logout message use /buy 6/7/8 [message]");
                        p.SendMessage("With blocks, use /buy 4 [blocks]");
                        p.SendMessage(c.blue + "1. " + Server.DefaultColor + "Title - 150 " + Server.moneys);
                        p.SendMessage(c.blue + "2. " + Server.DefaultColor + "Title Color - 75 " + Server.moneys);
                        p.SendMessage(c.blue + "3. " + Server.DefaultColor + "Disinfectant - 25 " + Server.moneys);
                        p.SendMessage(c.blue + "4. " + Server.DefaultColor + "Extra Blocks - 0.1 " + Server.moneys + " Per Block (Rounds to nearest noodle)");
                        if (p.group.name.ToLower() == "advbuilder" || p.group.CanExecute(Command.all.Find("shank")))
                        {
                            p.SendMessage(c.blue + "5. " + Server.DefaultColor + "Out of stock");
                        }
                        else
                        {
                            p.SendMessage(c.blue + "5. " + Server.DefaultColor + "Rank Up - 250 " + Server.moneys);
                        }

                        p.SendMessage(c.blue + "6. " + Server.DefaultColor + "Login Message - 100 " + Server.moneys);
                        p.SendMessage(c.blue + "7. " + Server.DefaultColor + "Logout Message - 100 " + Server.moneys);
                        p.SendMessage(c.blue + "8. " + Server.DefaultColor + "Infect Message - 100 " + Server.moneys);
                        break;
                }

            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/store - shows the store");
        }
    }
}