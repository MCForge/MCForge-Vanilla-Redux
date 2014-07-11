/*
	Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCForge)
 
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
using System.IO;

namespace MCForge
{
    public class CmdGiveItem : Command
    {
        public override string name { get { return "giveitem"; } }
        public override string shortcut { get { return "gi"; } }
        public override string type { get { return "ctf"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdGiveItem() { }

        public override void Use(Player p, string message)
        {
            int number = message.Split(' ').Length;
            if (number > 5) { Help(p); return; }
            if (number < 3) { Help(p); return; }
            int e = 1; string s = ""; string t = "";
            try
            {
                t = message.Split(' ')[0];
                s = message.Split(' ')[1];
                e = Convert.ToInt32(message.Split(' ')[2]);
            }
            catch { p.SendMessage("You must specify all essential variables!"); return; }
            bool test = true;
            try
            {
                string f = message.Split(' ')[3];
                if (f == "#") test = false;
            }
            catch { test = true; }
            switch (s.ToLower())
            {
                case "lazer":
                    Player tp = Player.Find(t);
                    if (tp == null) { p.SendMessage("Player is offline!"); return; }
                    tp.lazers = tp.lazers + e;
                    if (test)
                    {
                        p.SendMessage("Gave player " + e + " extra lazer/s!");
                        tp.SendMessage("You gained " + e + " free lazer/s!");
                    }
                    break;
                case "lightning":
                    Player tpt = Player.Find(t);
                    if (tpt == null) { p.SendMessage("Player is offline!"); return; }
                    tpt.lightnings = tpt.lightnings + e;
                    if (test)
                    {
                        p.SendMessage("Gave player " + e + " extra lightning/s!");
                        tpt.SendMessage("You gained " + e + " free lightning/s!");
                    }
                    break;
                case "trap":
                    Player tptz = Player.Find(t);
                    if (tptz == null) { p.SendMessage("Player is offline!"); return; }
                    tptz.traps = tptz.traps + e;
                    if (test)
                    {
                        p.SendMessage("Gave player " + e + " extra trap/s!");
                        tptz.SendMessage("You gained " + e + " free trap/s!");
                    }
                    break;
                case "line":
                    Player tpthz = Player.Find(t);
                    if (tpthz == null) { p.SendMessage("Player is offline!"); return; }
                    tpthz.lines = tpthz.lines + e;
                    if (test)
                    {
                        p.SendMessage("Gave player " + e + " extra line/s!");
                        tpthz.SendMessage("You gained " + e + " free line/s!");
                    }
                    break;
                case "grapple":
                    Player ztpthz = Player.Find(t);
                    if (ztpthz == null) { p.SendMessage("Player is offline!"); return; }
                    ztpthz.grapple = ztpthz.grapple + e;
                    if (test)
                    {
                        p.SendMessage("Gave player " + e + " extra grapple/s!");
                        ztpthz.SendMessage("You gained " + e + " free grapple/s!");
                    }
                    break;
                case "bigtnt":
                    Player ztpbthz = Player.Find(t);
                    if (ztpbthz == null) { p.SendMessage("Player is offline!"); return; }
                    ztpbthz.bigtnt = ztpbthz.bigtnt + e;
                    if (test)
                    {
                        p.SendMessage("Gave player " + e + " extra bigtnt/s!");
                        ztpbthz.SendMessage("You gained " + e + " free bigtnt/s!");
                    }
                    break;
                case "rocket":
                    Player ztpbthze = Player.Find(t);
                    if (ztpbthze == null) { p.SendMessage("Player is offline!"); return; }
                    ztpbthze.rockets = ztpbthze.rockets + e;
                    if (test)
                    {
                        p.SendMessage("Gave player " + e + " extra rocket/s!");
                        ztpbthze.SendMessage("You gained " + e + " free rocket/s!");
                    }
                    break;
                case "jetpack":
                    Player ztpbthzre = Player.Find(t);
                    if (ztpbthzre == null) { p.SendMessage("Player is offline!"); return; }
                    ztpbthzre.jetpack = ztpbthzre.jetpack + e;
                    if (test)
                    {
                        p.SendMessage("Gave player " + e + " extra jetpacks/s!");
                        ztpbthzre.SendMessage("You gained " + e + " free jetpacks/s!");
                    }
                    break;
                case "freeze":
                    Player ztpbthzrev = Player.Find(t);
                    if (ztpbthzrev == null) { p.SendMessage("Player is offline!"); return; }
                    ztpbthzrev.freeze = ztpbthzrev.freeze + e;
                    if (test)
                    {
                        p.SendMessage("Gave player " + e + " extra freezerays/s!");
                        ztpbthzrev.SendMessage("You gained " + e + " free freezerays/s!");
                    }
                    break;
            }
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/giveitem [player] [item] [amount] <#> - Give player [amount] free lazer/lightning/trap/line/grapple/bigtnt/rockets/jetpack/freeze (use # for silent)");
        }
    }
}