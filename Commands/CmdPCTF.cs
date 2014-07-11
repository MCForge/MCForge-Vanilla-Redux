/*
	Copyright 2011 MCForge Team - 
    Created by Snowl (David D.) and Cazzar (Cayde D.)

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

namespace MCForge
{
    public class CmdPCTF : Command
    {
        public override string name { get { return "pctf"; } }
        public override string shortcut { get { return "pc"; } }
        public override string type { get { return "game"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdPCTF() { }
        public override void Use(Player p, string message)
        {
            if (String.IsNullOrEmpty(message)) { Help(p); return; }
            string[] s = message.ToLower().Split(' ');
            if (s[0] == "status")
            {
                switch (Server.pctf.CTFStatus())
                {
                    case 0:
                        Player.GlobalMessage("There is no CTF game currently in progress.");
                        return;
                    case 1:
                        Player.SendMessage(p, "There is a CTF game currently in progress with infinite rounds.");
                        return;
                    case 2:
                        Player.SendMessage(p, "There is a one-time CTF game currently in progress.");
                        return;
                    case 3:
                        Player.SendMessage(p, "There is a CTF game currently in progress with a " + Server.pctf.limitRounds + " amount of rounds.");
                        return;
                    case 4:
                        Player.SendMessage(p, "There is a CTF game currently in progress, scheduled to stop after this round.");
                        return;
                    default:
                        Player.SendMessage(p, "An unknown error occurred.");
                        return;
                }
            }
            else if (s[0] == "start")
            {
                if (Server.pctf.CTFStatus() != 0) { Player.SendMessage(p, "There is already a CTF game currently in progress."); return; }
                if (s.Length == 2)
                {
                    int i = 1;
                    bool result = int.TryParse(s[1], out i);
                    if (result == false) { Player.SendMessage(p, "You need to specify a valid option!"); return; }
                    if (s[1] == "0")
                    {
                        Server.pctf.StartGame(1, 0);
                    }
                    else
                    {
                        Server.pctf.StartGame(3, i);
                    }
                }
                else
                    Server.pctf.StartGame(2, 0);
            }
            else if (s[0] == "stop")
            {
                if (Server.pctf.CTFStatus() == 0) { Player.SendMessage(p, "There is no CTF game currently in progress."); return; }
                Player.GlobalMessage("The current game of CTF will end this round!");
                Server.ctfGameStatus = 4;
            }
            else if (s[0] == "force")
            {
                if (Server.pctf.CTFStatus() == 0) { Player.SendMessage(p, "There is no CTF game currently in progress."); return; }
                Server.s.Log("CTF ended forcefully by " + p.name);
                ushort x, y, z; int xx, yy, zz;
                x = Convert.ToUInt16((int)p.pos[0] / 32);
                y = Convert.ToUInt16((int)p.pos[1] / 32 - 1);
                z = Convert.ToUInt16((int)p.pos[2] / 32);
                xx = p.pos[0];
                yy = p.pos[1];
                zz = p.pos[2];
                Server.pctf.dropFlag(p, x, y, z, xx, yy, zz);
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/pctf - Shows this help menu.");
            Player.SendMessage(p, "/pctf start - Starts a CTF game for one round.");
            Player.SendMessage(p, "/pctf start 0 - Starts a CTF game for an unlimited amount of rounds.");
            Player.SendMessage(p, "/pctf start [x] - Starts a CTF game for [x] amount of rounds.");
            Player.SendMessage(p, "/pctf stop - Stops the CTF game after the round has finished.");
            Player.SendMessage(p, "/pctf force - Force stops the CTF game immediately.");
        }
    }
}