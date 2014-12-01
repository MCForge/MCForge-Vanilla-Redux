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
        int message12 = 0;
        public override string name { get { return "store"; } }
        public override string shortcut { get { return "buy"; } }
        public override string type { get { return "zombie"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdStore() { }
        public ZombieGame zs;
        public override void Use(Player p, string message)
        {
            if(Server.lava.active && p.level == Server.lava.map)
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
                        if (p.money >= 250)
                        {
                            if (!message1.Equals(""))
                            {
                                if (message1.Length < 17)
                                {
                                    Command.all.Find("title").Use(p, p.name + " " + message1);
                                    p.money = p.money - 250;
                                }
                                else
                                {
                                    p.SendMessage("Title must be less than 17 characters!");
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
                        if (p.money >= 250)
                        {
                            if (!message1.Equals(""))
                            {
                                string color = c.Parse(message1);
                                if (color == "") { Player.SendMessage(p, "There is no color \"" + message1 + "\"."); break; }
                                if (p.prefix == "") { Player.SendMessage(p, "You must have a title."); break; }
                                Command.all.Find("tcolor").Use(p, p.name + " " + message1);
                                p.money = p.money - 250;
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
                        message12 = 1;
                        try
                        {
                            message12 = int.Parse(message1);
                        }
                        catch { message12 = 1; }
                        if (p.money >= (1 * message12))
                        {
                            if (!message1.Equals(""))
                            {
                                Command.all.Find("morelives").Use(p, Convert.ToString(message12));
                                p.money = p.money - 1;
                            }
                            else
                            {
                                Command.all.Find("morelives").Use(p, Convert.ToString(message12));
                                p.money = p.money - (1 * message12);
                            }
                        }
                        else
                        {
                            p.SendMessage("You do not have enough " + Server.moneys);
                        }
                        break;
                    case 4:
                        message12 = 1;
                        try
                        {
                            message12 = int.Parse(message1);
                        }
                        catch { message12 = 1; }
                        if (p.money >= (10 * message12))
                        {
                            if (!message1.Equals(""))
                            {
                                Command.all.Find("moresponges").Use(p, Convert.ToString(message12));
                                p.money = p.money - 1;
                            }
                            else
                            {
                                Command.all.Find("moresponges").Use(p, Convert.ToString(message12));
                                p.money = p.money - (10 * message12);
                            }
                        }
                        else
                        {
                            p.SendMessage("You do not have enough " + Server.moneys);
                        }
                        break;
                    case 5:
                        if (p.money >= 55)
                        {
                            Command.all.Find("moresponges").Use(p, "8");
                            p.money = p.money - 55;
                        }
                        else
                        {
                            p.SendMessage("You do not have enough " + Server.moneys);
                        }
                        break;
                    case 6:
                        if (p.group.name.ToLower() == "beginner" && p.money >= 50)
                        { p.money = p.money - 50; Command.all.Find("promote").Use(null, p.name); }
                        else if (p.group.name.ToLower() == "novice" && p.money >= 150)
                        { p.money = p.money - 150; Command.all.Find("promote").Use(null, p.name); }
                        else if (p.group.name.ToLower() == "naturalist" && p.money >= 300)
                        { p.money = p.money - 300; Command.all.Find("promote").Use(null, p.name); }
                        else if (p.group.name.ToLower() == "beginner" && p.money <= 50)
                            p.SendMessage("You do not have enough " + Server.moneys);
                        else if (p.group.name.ToLower() == "novice" && p.money <= 150)
                            p.SendMessage("You do not have enough " + Server.moneys);
                        else if (p.group.name.ToLower() == "naturalist" && p.money <= 300)
                            p.SendMessage("You do not have enough " + Server.moneys);
                        else
                            p.SendMessage("You already have the max rank!");
                        break;
                    case 7:
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
                    case 8:
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
                    case 9:
                        if (p.money >= 3)
                        {
                            Command.all.Find("roll").Use(p, "");
                        }
                        else
                        {
                            p.SendMessage("You do not have enough " + Server.moneys);
                        }
                        break;
                    default:
                        Help(p);
                        break;
                }
            }
            if (Server.ZombieModeOn && p.level.name == Server.zombie.currentLevelName)
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
                p.SendMessage(c.blue + "5. " + Server.DefaultColor + "Login Message - 100 " + Server.moneys);
                p.SendMessage(c.blue + "6. " + Server.DefaultColor + "Logout Message - 100 " + Server.moneys);
                p.SendMessage(c.blue + "7. " + Server.DefaultColor + "Infect Message - 100 " + Server.moneys);
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
                    case 6:
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
                    case 7:
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

                        p.SendMessage(c.blue + "5. " + Server.DefaultColor + "Login Message - 100 " + Server.moneys);
                        p.SendMessage(c.blue + "6. " + Server.DefaultColor + "Logout Message - 100 " + Server.moneys);
                        p.SendMessage(c.blue + "7. " + Server.DefaultColor + "Infect Message - 100 " + Server.moneys);
                        break;
                }
            }
            return;
            }/*
            if(p.level.name == Server.pctf.currentLevelName && Server.CTFModeOn)
                message.ToLower();
                string[] msg = message.Split(' ');
                if (message.Split(' ').Length == 1)
                {
                    switch (message)
                    {
                        case "weapons":
                            Player.SendMessage(p, c.purple + "##################################");
                            Player.SendMessage(p, "To buy a weapon, type /store buy [weapon], or to see more information, type /store weapons [weapon]");
                            Player.SendMessage(p, "Available weapons: " + c.lime + "lazer[10], lightning[64], trap[120], bigtnt[96], line[40], grapple[150], rocket[136]");
                            Player.SendMessage(p, "Available weapons: " + c.lime + "jetpack[30], freezeray[110]");
                            Player.SendMessage(p, c.purple + "##################################");
                            break;
                        case "upgrades":
                            Player.SendMessage(p, c.purple + "##################################");
                            Player.SendMessage(p, "To buy an upgrade, type /store upgrade [weapon], or to see more, type /store upgrades [upgrade]");
                            Player.SendMessage(p, "Available weapons to upgrade: " + c.lime + "lazer, lightning, rocket, trap, mine, tnt, pistol");
                            Player.SendMessage(p, c.purple + "##################################");
                            break;
                        case "novelties":
                            Player.SendMessage(p, c.purple + "##################################");
                            Player.SendMessage(p, "To buy a novelty, type /store buy [novelty] [option], or to see more information, type /store novelty [novelty]");
                            Player.SendMessage(p, "Available novelties: " + c.lime + "title[5000], tcolor[3500]");//, customflagcolor[10000]");
                            Player.SendMessage(p, c.purple + "##################################");
                            break;
                        case "shitty_wizard":
                            Player.SendMessage(p, "The shitty wizard loves you with all Genuine_Imation's love.");
                            break;
                        default:
                            Player.SendMessage(p, c.purple + "##################################");
                            Player.SendMessage(p, "To see what you can purchase, type /store [category]");
                            Player.SendMessage(p, "Store categories: " + c.lime + "weapons, upgrades, novelties");
                            Player.SendMessage(p, c.purple + "##################################");
                            break;
                    }
                }
                else if (message.Split(' ').Length == 2 || message.Split(' ').Length == 3)
                {
                    string a = message.Split(' ')[0];
                    string b = message.Split(' ')[1];
                    int e = 1;
                    try
                    {
                        e = Convert.ToInt32(message.Split(' ')[2]);
                    }
                    catch { e = 1; }
                    bool returen = false;
                    if (message.Split(' ').Length == 3)
                    {
                        string d = message.Split(' ')[2];
                        if (d.Length > 17) { p.SendMessage("Your title must be shorter than 17 characters!"); return; }
                        switch (b)
                        {
                            case "title":
                                Player.SendMessage(p, c.purple + "##################################");
                                if (p.money >= 5000) { Command.all.Find("title").Use(p, p.name + " " + d); p.money = p.money - 5000; Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You bought a title!" + c.gray + " - "); }
                                else Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You do not have enough money!" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                returen = true;
                                break;
                            case "tcolor":
                                Player.SendMessage(p, c.purple + "##################################");
                                string color = c.Parse(d);
                                if (color == "") { Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "There is no color \"" + d + "\"." + c.gray + " - "); break; }
                                if (p.money >= 3500) { Command.all.Find("tcolor").Use(p, p.name + " " + d); p.money = p.money - 3500; Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You bought a title color!" + c.gray + " - "); }
                                else Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You do not have enough money!" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                returen = true;
                                break;
                            case "default":
                                break;
                        }
                        if (returen)
                            return;
                    }
                    if (a == "buy" || a == "b")
                    {
                        switch (b)
                        {
                            #region weapons
                            case "lazer":
                                Player.SendMessage(p, c.purple + "##################################");
                                if (p.money >= (10 * e)) { Command.all.Find("giveitem").Use(p, p.name + " lazer " + e + " #"); p.money = p.money - (10 * e); Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You bought " + e + " lazer/s!" + c.gray + " - "); }
                                else Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You do not have enough money!" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "lightning":
                                Player.SendMessage(p, c.purple + "##################################");
                                if (p.money >= (64 * e)) { Command.all.Find("giveitem").Use(p, p.name + " lightning " + e + " #"); p.money = p.money - (64 * e); Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You bought " + e + " lightning/s!" + c.gray + " - "); }
                                else Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You do not have enough money!" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "trap":
                                Player.SendMessage(p, c.purple + "##################################");
                                if (p.money >= (120 * e)) { Command.all.Find("giveitem").Use(p, p.name + " trap " + e + " #"); p.money = p.money - (120 * e); Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You bought " + e + " trap/s!" + c.gray + " - "); }
                                else Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You do not have enough money!" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "bigtnt":
                                Player.SendMessage(p, c.purple + "##################################");
                                if (p.money >= (96 * e)) { Command.all.Find("giveitem").Use(p, p.name + " bigtnt " + e + " #"); p.money = p.money - (96 * e); Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You bought " + e + " bigtnt/s!" + c.gray + " - "); }
                                else Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You do not have enough money!" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "line":
                                Player.SendMessage(p, c.purple + "##################################");
                                if (p.money >= (40 * e)) { Command.all.Find("giveitem").Use(p, p.name + " line " + e + " #"); p.money = p.money - (40 * e); Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You bought " + e + " line/s!" + c.gray + " - "); }
                                else Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You do not have enough money!" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "grapple":
                                Player.SendMessage(p, c.purple + "##################################");
                                if (p.money > (150 * e)) { Command.all.Find("giveitem").Use(p, p.name + " grapple " + e + " #"); p.money = p.money - (150 * e); Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You bought " + e + " grapple/s!" + c.gray + " - "); }
                                else Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You do not have enough money!" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "rocket":
                                Player.SendMessage(p, c.purple + "##################################");
                                if (p.money >= (136  * e)) { Command.all.Find("giveitem").Use(p, p.name + " rocket " + e + " #"); p.money = p.money - (136 * e); Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You bought " + e + " rocket/s!" + c.gray + " - "); }
                                else Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You do not have enough money!" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "jetpack":
                                Player.SendMessage(p, c.purple + "##################################");
                                if (p.money >= (30 * e)) { Command.all.Find("giveitem").Use(p, p.name + " jetpack " + e + " #"); p.money = p.money - (30 * e); Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You bought " + e + " jetpacks/s!" + c.gray + " - "); }
                                else Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You do not have enough money!" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "freezeray":
                                Player.SendMessage(p, c.purple + "##################################");
                                if (p.money >= (110 * e)) { Command.all.Find("giveitem").Use(p, p.name + " freeze " + e + " #"); p.money = p.money - (110 * e); Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You bought " + e + " freezerays/s!" + c.gray + " - "); }
                                else Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You do not have enough money!" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            #endregion

                        }
                    }
                    if (a == "weapons")
                    {
                        switch (b)
                        {
                            case "lazer":
                                Player.SendMessage(p, c.purple + "##################################");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Lazer (Sand) - Fires a lava line forward that breaks through blocks and kills players." + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "lightning":
                                Player.SendMessage(p, c.purple + "##################################");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Lightning (LightBlue) - Fires a lava line upwards that breaks through blocks and kills players" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "trap":
                                Player.SendMessage(p, c.purple + "##################################");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Trap (Mushroom) - Traps a player when they walk within a 1 block radius." + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "bigtnt":
                                Player.SendMessage(p, c.purple + "##################################");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "BigTNT (TNT) - Makes a bigger TNT explosion when placed." + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "line":
                                Player.SendMessage(p, c.purple + "##################################");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Line (Coal) - Creates a straight line of cobblestone to walk on." + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "jetpack":
                                Player.SendMessage(p, c.purple + "##################################");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Jetpack (White) - When placed, rockets you in the air." + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "freezeray":
                                Player.SendMessage(p, c.purple + "##################################");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Freezeray (Blue) - When placed shoots a water line that freezes anyone who crosses the path." + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "grapple":
                                Player.SendMessage(p, c.purple + "##################################");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Grapple (/g) - Fires a bridge that you can walk on straight to where you are pointing at." + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "rocket":
                                Player.SendMessage(p, c.purple + "##################################");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Rocket (/r) - Fires a rocket that explodes where you are pointing at." + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                        }
                    }
                    if (a == "upgrades")
                    {
                        switch (b)
                        {
                            case "lazer":
                                Player.SendMessage(p, c.purple + "##################################");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Lazer Upgrades" + c.gray + " - ");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Level 1 [5000]: You widen your lazer to two blocks wide." + c.gray + " - ");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Level 2 [15000]: Your lazer teleports through bedrock" + c.gray + " - ");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Level 3 [25000]: Your lazer fires again 2 second after." + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "lightning":
                                Player.SendMessage(p, c.purple + "##################################");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Lightning Upgrades" + c.gray + " - ");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Level 1 [2500]: Your lighting now fires in a plus shape" + c.gray + " - ");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Level 2 [17000]: Your lightning has a 1 in 75 chance of striking someone not near you" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "trap":
                                Player.SendMessage(p, c.purple + "##################################");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Trap Upgrades" + c.gray + " - ");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Level 1 [7500]: Your trap is now in a 2 block radius" + c.gray + " - ");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Level 2 [19000]: Your trap is now in a 3 block radius" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "rocket":
                                Player.SendMessage(p, c.purple + "##################################");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Rocket Upgrades" + c.gray + " - ");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Level 1 [5500]: Your rocket travels as fast as a concorde" + c.gray + " - ");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Level 2 [12000]: Your rocket explodes on contact" + c.gray + " - ");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Level 3 [32000]: Your rocket travels near the speed of light" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "tnt":
                                Player.SendMessage(p, c.purple + "##################################");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "TNT Upgrades" + c.gray + " - ");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Level 1 [5500]: Your TNT explodes a tiny bit bigger" + c.gray + " - ");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Level 2 [13000]: Your TNT is now 1 block smaller than a big tnt" + c.gray + " - ");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Level 3 [25000]: Your TNT is invisible when you place it" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "pistol":
                                Player.SendMessage(p, c.purple + "##################################");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Pistol Upgrades" + c.gray + " - ");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Level 1 [6500]: Your pistol gains more power, shooting further" + c.gray + " - ");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Level 2 [7000]: Your pistol breaks blocks now" + c.gray + " - ");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Level 3 [8900]: Your pistol gains nuclear power, shooting much further" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "mine":
                                Player.SendMessage(p, c.purple + "##################################");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Mine Upgrades" + c.gray + " - ");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Level 1 [7500]: Your mines are now proximity mines!" + c.gray + " - ");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Level 2 [12000]: Your mine turns into a better proximity mines" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                        }
                    }
                    if (a == "upgrade")
                    {
                        switch (b)
                        {
                            case "lazer":
                                Player.SendMessage(p, c.purple + "##################################");
                                if (p.money >= 5000 && p.lazerUpgrade == 0) { p.lazerUpgrade = 1; p.money = p.money - 5000; Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You upgraded your lazor to level 1!" + c.gray + " - "); }
                                else if (p.money >= 15000 && p.lazerUpgrade == 1) { p.lazerUpgrade = 2; p.money = p.money - 15000; Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You upgraded your lazor to level 2!" + c.gray + " - "); }
                                else if (p.money >= 15000 && p.lazerUpgrade == 2) { p.lazerUpgrade = 3; p.money = p.money - 25000; Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You upgraded your lazor to level 3! Nice!" + c.gray + " - "); }
                                else if (p.lazerUpgrade >= 3) { Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You already have the maximum upgrade for lazer." + c.gray + " - "); }
                                else Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You do not have enough money!" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "lightning":
                                Player.SendMessage(p, c.purple + "##################################");
                                if (p.money >= 2500 && p.lightningUpgrade == 0) { p.lightningUpgrade = 1; p.money = p.money - 2500; Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You upgraded your lightning to level 1!" + c.gray + " - "); }
                                else if (p.money >= 17000 && p.lightningUpgrade == 1) { p.lightningUpgrade = 2; p.money = p.money - 17000; Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You upgraded your lightning to level 2! Nice!" + c.gray + " - "); }
                                else if (p.lightningUpgrade >= 2) { Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You already have the maximum upgrade for lightning." + c.gray + " - "); }
                                else Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You do not have enough money!" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "trap":
                                Player.SendMessage(p, c.purple + "##################################");
                                if (p.money >= 7500 && p.trapUpgrade == 0) { p.trapUpgrade = 1; p.money = p.money - 7500; Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You upgraded your trap to level 1!" + c.gray + " - "); }
                                else if (p.money >= 19000 && p.trapUpgrade == 1) { p.trapUpgrade = 2; p.money = p.money - 19000; Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You upgraded your trap to level 2! Nice!" + c.gray + " - "); }
                                else if (p.trapUpgrade >= 2) { Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You already have the maximum upgrade for trap." + c.gray + " - "); }
                                else Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You do not have enough money!" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "rocket":
                                Player.SendMessage(p, c.purple + "##################################");
                                if (p.money >= 5500 && p.rocketUpgrade == 0) { p.rocketUpgrade = 1; p.money = p.money - 5500; Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You upgraded your rocket to level 1!" + c.gray + " - "); }
                                else if (p.money >= 12000 && p.rocketUpgrade == 1) { p.rocketUpgrade = 2; p.money = p.money - 12000; Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You upgraded your rocket to level 2!" + c.gray + " - "); }
                                else if (p.money >= 32000 && p.rocketUpgrade == 2) { p.rocketUpgrade = 3; p.money = p.money - 32000; Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You upgraded your rocket to level 3! Nice!" + c.gray + " - "); }
                                else if (p.rocketUpgrade >= 3) { Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You already have the maximum upgrade for rocket." + c.gray + " - "); }
                                else Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You do not have enough money!" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "tnt":
                                Player.SendMessage(p, c.purple + "##################################");
                                if (p.money >= 5500 && p.tntUpgrade == 0) { p.tntUpgrade = 1; p.money = p.money - 5500; Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You upgraded your tnt to level 1!" + c.gray + " - "); }
                                else if (p.money >= 13000 && p.tntUpgrade == 1) { p.tntUpgrade = 2; p.money = p.money - 13000; Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You upgraded your tnt to level 2!" + c.gray + " - "); }
                                else if (p.money >= 25000 && p.tntUpgrade == 2) { p.tntUpgrade = 3; p.money = p.money - 25000; Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You upgraded your tnt to level 3! Nice!" + c.gray + " - "); }
                                else if (p.tntUpgrade >= 3) { Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You already have the maximum upgrade for tnt." + c.gray + " - "); }
                                else Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You do not have enough money!" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "pistol":
                                Player.SendMessage(p, c.purple + "##################################");
                                if (p.money >= 6500 && p.pistolUpgrade == 0) { p.pistolUpgrade = 1; p.money = p.money - 6500; Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You upgraded your pistol to level 1!" + c.gray + " - "); }
                                else if (p.money >= 7000 && p.pistolUpgrade == 1) { p.pistolUpgrade = 2; p.money = p.money - 7000; Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You upgraded your pistol to level 2!" + c.gray + " - "); }
                                else if (p.money >= 8900 && p.pistolUpgrade == 2) { p.pistolUpgrade = 3; p.money = p.money - 8900; Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You upgraded your pistol to level 3! Nice!" + c.gray + " - "); }
                                else if (p.pistolUpgrade >= 3) { Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You already have the maximum upgrade for pistol." + c.gray + " - "); }
                                else Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You do not have enough money!" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "mine":
                                Player.SendMessage(p, c.purple + "##################################");
                                if (p.money >= 7500 && p.mineUpgrade == 0) { p.mineUpgrade = 2; p.money = p.money - 7500; Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You upgraded your mine to level 1!" + c.gray + " - "); }
                                else if (p.money >= 12000 && p.mineUpgrade == 2) { p.mineUpgrade = 3; p.money = p.money - 12000; Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You upgraded your mine to level 2! Nice!" + c.gray + " - "); }
                                else if (p.mineUpgrade >= 3) { Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You already have the maximum upgrade for mine." + c.gray + " - "); }
                                else Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You do not have enough money!" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                        }
                    }
                    if (a == "buff")
                    {
                        switch (b)
                        {
                            case "untouchable":
                                Player.SendMessage(p, c.purple + "##################################");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Untouchable - You cannot get tagged for 25 seconds." + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "invisible":
                                Player.SendMessage(p, c.purple + "##################################");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Invisible - You go invisible for 18 seconds." + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "iceshield":
                                Player.SendMessage(p, c.purple + "##################################");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Iceshield - You cannot get hurt by TNT for 35 seconds." + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "makeaura":
                                Player.SendMessage(p, c.purple + "##################################");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Makeaura - You tag people in a 2 block radius." + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "oneup":
                                Player.SendMessage(p, c.purple + "##################################");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Oneup - You revive once you die (does not apply while carrying the flag)" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                        }
                    }
                    if (a == "novelty")
                    {
                        switch (b)
                        {
                            case "title":
                                Player.SendMessage(p, c.purple + "##################################");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Title - You get a custom title next to your name!" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "tcolor":
                                Player.SendMessage(p, c.purple + "##################################");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "T(itle)color - Your title gets a special color!" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                            case "customflagcolor":
                                Player.SendMessage(p, c.purple + "##################################");
                                Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "Custom Flag Color - When you pick up a flag, it changes to a different color (no red or blue)" + c.gray + " - ");
                                Player.SendMessage(p, c.purple + "##################################");
                                break;
                        }
                    }
                }*/
        }
        public override void Help(Player p)
        {
            if (Server.CTFModeOn)
            {
                Player.SendMessage(p, c.purple + "##################################");
                Player.SendMessage(p, "To see what you can purchase, type /store [category]");
                Player.SendMessage(p, "Store categories: " + c.lime + "buffs, weapons, upgrades, novelties, rank");
                Player.SendMessage(p, "A shortcut to buy is /s b [item] [amount]");
                Player.SendMessage(p, c.purple + "##################################");
                return;
            }
            else
            {
                Player.SendMessage(p, "/store - shows the store");
            }
        }
    }
}
