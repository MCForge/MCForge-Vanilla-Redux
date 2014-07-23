/*
	Copyright 2011 MCForge Team - 
    Created by Snowl (David D.)

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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Timers;
using System.Threading;

namespace MCForge
{
    public class ProperCTFSetup
    {
        public int amountOfRounds = 0;
        public int limitRounds = 0;
        public int aliveCount = 0;
        public int amountOfMilliseconds = 0;
        public DateTime CTFTime = new DateTime();
        public static System.Timers.Timer timer;
        public static System.Timers.Timer timer2;
        public bool initialChangeLevel = false;
        public int blueTeamCaptures = 0;
        public int redTeamCaptures = 0;
        public string currentLevelName = "";
        public List<Player> red = new List<Player>();
        public List<Player> blu = new List<Player>();
        public int[] blueSpawn = { 0, 0, 0 };
        public int[] redSpawn = { 0, 0, 0 };
        public int[] blueFlag = { 0, 0, 0 };
        public int[] redFlag = { 0, 0, 0 };
        public int[] blueDroppedFlag = { 0, 0, 0 };
        public int[] redDroppedFlag = { 0, 0, 0 };
        public int halfway;
        public CatchPos blueFlagblock;
        public CatchPos redFlagblock;
        public int buildCeiling = 180;
        public bool firstblood = false;
        public int bluDeaths = 0;
        public int redDeaths = 0;
        public ProperCTFSetup() { }

        public struct CatchPos { public ushort x, y, z; public ushort type; }

        public void StartGame(int status, int amount)
        {
            //status: 0 = not started, 1 = always on, 2 = one time, 3 = certain amount of rounds, 4 = stop round next round

            if (status == 0) return;

            //SET ALL THE VARIABLES!
            Server.CTFModeOn = true;
            Server.ctfGameStatus = status;
            Server.ctfRound = false;
            initialChangeLevel = false;
            limitRounds = amount + 1;
            amountOfRounds = 0;
            //SET ALL THE VARIABLES?!?

            //Start the main CTF thread
            Thread t = new Thread(MainLoop);
            t.Start();
        }

        private void MainLoop()
        {
            if (Server.ctfGameStatus == 0) return;
            bool cutVariable = true;

            if (initialChangeLevel == false)
            {
                ChangeLevel();
                initialChangeLevel = true;
            }

            while (cutVariable == true)
            {
                int gameStatus = Server.ctfGameStatus;
                Server.ctfRound = false;
                amountOfRounds = amountOfRounds + 1;

                if (gameStatus == 0) { cutVariable = false; return; }
                else if (gameStatus == 1) { MainGame(); ChangeLevel(); }
                else if (gameStatus == 2) { MainGame(); ChangeLevel(); cutVariable = false; Server.ctfGameStatus = 0; return; }
                else if (gameStatus == 3)
                {
                    if (limitRounds == amountOfRounds) { cutVariable = false; Server.ctfGameStatus = 0; limitRounds = 0; initialChangeLevel = false; Server.CTFModeOn = false; Server.ctfRound = false; return; }
                    else { MainGame(); ChangeLevel(); }
                }
                else if (gameStatus == 4)
                { cutVariable = false; Server.gameStatus = 0; Server.ctfGameStatus = 0; limitRounds = 0; initialChangeLevel = false; Server.CTFModeOn = false; Server.ctfRound = false; return; }
            }
        }

        private void MainGame()
        {
        unload_loop:
            try
            {
                if (Server.levels != null)
                {
                    foreach (Level l in Server.levels)
                    {
                        if (l.name.ToLower() != Server.mainLevel.name && l.name.ToLower() != currentLevelName.ToLower() && l.name.ToLower() != "tutorial")
                        {
                            l.Unload();
                            goto unload_loop;
                        }
                    }
                }
            }
            catch { goto unload_loop; }
            if (Server.ctfGameStatus == 0) return;
            if (!Server.CTFModeOn) { return; }
            Player.players.ForEach(delegate(Player player)
            {
                //RESET ALL THE VARIABLES!
                player.pteam = 0;
                player.isHoldingFlag = false;
                player.PlacedNukeThisRound = false;
                player.BoughtOneUpThisRound = false;
                player.overallKilled = 0;
                player.overallDied = 0;
                player.killingPeople = false;
                player.amountKilled = 0;
                player.minePlacement[0] = 0; player.minePlacement[1] = 0; player.minePlacement[2] = 0;
                player.minesPlaced = 0;
                player.trapPlacement[0] = 0; player.trapPlacement[1] = 0; player.trapPlacement[2] = 0;
                player.trapsPlaced = 0;
                player.deathTimerOn = false;
                player.hasBeenTrapped = false;
                player.ironman = false;
                Server.killed.Clear();
                Server.blueFlagDropped = false;
                Server.redFlagDropped = false;
                Server.blueFlagHeld = false;
                Server.redFlagHeld = false;
                firstblood = false;
                redDeaths = 0;
                bluDeaths = 0;
                red.Clear();
                blu.Clear();
            });
            Player.GlobalMessage(c.gray + " - " + Server.DefaultColor + "30 seconds till the round starts!" + c.gray + " - ");
            Thread.Sleep(1000 * 30);
            CTFGame();
        }

        private void CTFGame()
        {
            Level level = Level.Find(currentLevelName);
            if (level == null) return;
            level.placeBlock((ushort)((int)redFlag[0]), (ushort)((int)redFlag[1]), (ushort)((int)redFlag[2]), Block.redflag);
            level.placeBlock((ushort)((int)blueFlag[0]), (ushort)((int)blueFlag[1]), (ushort)((int)blueFlag[2]), Block.blueflag);
            Server.ctfRound = true;
            Player.GlobalMessage(c.gray + " - " + Server.DefaultColor + "The round has started!" + c.gray + " - ");

            Random random = new Random();
            int amountOfMinutes = random.Next(25, 40);
            Player.GlobalMessage(c.gray + " - " + Server.DefaultColor + "The round will last for " + amountOfMinutes + " minutes!" + c.gray + " - ");
            amountOfMilliseconds = (60000 * amountOfMinutes);

            timer = new System.Timers.Timer(amountOfMilliseconds);
            timer.Elapsed += new ElapsedEventHandler(EndRound);
            timer.Enabled = true;
            CTFTime = DateTime.Now + new TimeSpan(0, amountOfMinutes, 0);
            Server.vulnerable = 1;

            while (Server.ctfRound)
            {
                red.ForEach(delegate(Player player1)
                {
                    blu.ForEach(delegate(Player player2)
                    {
                        if (player2.pos[0] / 32 == player1.pos[0] / 32 || player2.pos[0] / 32 == player1.pos[0] / 32 + 1 || player2.pos[0] / 32 == player1.pos[0] / 32 - 1)
                        {
                            if (player2.pos[1] / 32 == player1.pos[1] / 32 || player2.pos[1] / 32 == player1.pos[1] / 32 - 1 || player2.pos[1] / 32 == player1.pos[1] / 32 + 1)
                            {
                                if (player2.pos[2] / 32 == player1.pos[2] / 32 || player2.pos[2] / 32 == player1.pos[2] / 32 + 1 || player2.pos[2] / 32 == player1.pos[2] / 32 - 1)
                                {
                                    if (!player2.referee && !player1.referee && player1 != player2 && player1.level.name == currentLevelName && player2.level.name == currentLevelName)
                                    {
                                        bool one = false;
                                        bool two = false;
                                        if (player1.makeaura || player2.makeaura)
                                        {
                                            if (player2.pos[0] / 32 == player1.pos[0] / 32 + 2 || player2.pos[0] / 32 == player1.pos[0] / 32 - 2)
                                                if (player2.pos[1] / 32 == player1.pos[1] / 32 - 2 || player2.pos[1] / 32 == player1.pos[1] / 32 + 2)
                                                    if (player2.pos[2] / 32 == player1.pos[2] / 32 + 2 || player2.pos[2] / 32 == player1.pos[2] / 32 - 2)
                                                        if (!player2.referee && !player1.referee && player1 != player2 && player1.level.name == currentLevelName && player2.level.name == currentLevelName && !player1.untouchable && !player2.untouchable)
                                                        {
                                                            one = OnSide(player1, getTeam(player1));
                                                            two = OnSide(player2, getTeam(player2));
                                                            if (one == two) { return; }
                                                            if (one && !player2.deathTimerOn && !player2.untouchable)
                                                            {
                                                                killedPlayer(player1, (ushort)(player1.pos[0] / 32), (ushort)(player1.pos[1] / 32), (ushort)(player1.pos[2] / 32), false, "tag");
                                                            }
                                                            else if (two && !player1.deathTimerOn && !player1.untouchable)
                                                            {
                                                                killedPlayer(player2, (ushort)(player2.pos[0] / 32), (ushort)(player2.pos[1] / 32), (ushort)(player2.pos[2] / 32), false, "tag");
                                                            }
                                                            return;
                                                        }
                                        }
                                        one = OnSide(player1, getTeam(player1));
                                        two = OnSide(player2, getTeam(player2));
                                        if (one == two) { return; }
                                        if (one && !player2.deathTimerOn && !player2.untouchable)
                                        {
                                            killedPlayer(player1, (ushort)(player1.pos[0] / 32), (ushort)(player1.pos[1] / 32), (ushort)(player1.pos[2] / 32), false, "tag");
                                        }
                                        else if (two && !player1.deathTimerOn && !player1.untouchable)
                                        {
                                            killedPlayer(player2, (ushort)(player2.pos[0] / 32), (ushort)(player2.pos[1] / 32), (ushort)(player2.pos[2] / 32), false, "tag");
                                        }
                                    }
                                }
                            }
                        }
                    });
                });
                Player.players.ForEach(delegate(Player player)
                {
                    if (player.isHoldingFlag && player.pteam != 0)
                    {
                        if (getTeam(player) == "blue")
                        {
                            tempFlagBlock(player, "blue");
                        }
                        else if (getTeam(player) == "red")
                        {
                            tempFlagBlock(player, "red");
                        }
                    }
                });
                Thread.Sleep(500);
            }

            HandOutRewards();
        }

        public void EndRound(object sender, ElapsedEventArgs e)
        {
            if (Server.ctfGameStatus == 0) return;
            Player.GlobalMessage("%4Round End:%f 5"); Thread.Sleep(1000);
            Player.GlobalMessage("%4Round End:%f 4"); Thread.Sleep(1000);
            Player.GlobalMessage("%4Round End:%f 3"); Thread.Sleep(1000);
            Player.GlobalMessage("%4Round End:%f 2"); Thread.Sleep(1000);
            Player.GlobalMessage("%4Round End:%f 1"); Thread.Sleep(1000);
            HandOutRewards();
        }

        public void HandOutRewards()
        {
            if (!Server.ctfRound) goto lol;
            Server.ctfRound = false;
            if (blueTeamCaptures > 4) Player.GlobalMessage(c.gray + " - " + Server.DefaultColor + "Congratulations to the blue team for winning with " + blueTeamCaptures + " captures!" + c.gray + " - ");
            else if (redTeamCaptures > 4) Player.GlobalMessage(c.gray + " - " + Server.DefaultColor + "Congratulations to the red team for winning with " + redTeamCaptures + " captures!" + c.gray + " - ");
            else Player.GlobalMessage(c.gray + " - " + Server.DefaultColor + "No team managed to capture 5 flags! Good luck next time!" + c.gray + " - ");
            Player.GlobalMessage(c.gray + " - " + c.blue + "Blue: " + Server.DefaultColor + blueTeamCaptures + c.red + " Red: " + Server.DefaultColor + redTeamCaptures + c.gray + " - ");
            Player.GlobalMessage(c.gray + " - " + c.blue + "Blue kills: " + Server.DefaultColor + bluDeaths + c.red + " Red kills: " + Server.DefaultColor + redDeaths + c.gray + " - ");

            var lengths = from element in Player.players
                          orderby (element.overallDied - element.overallKilled)
                          select element;

            int loop = 0;

            foreach (Player z in lengths)
            {
                loop++;
                if (loop >= 6)
                    break;
                else if (loop == 1)
                    Player.GlobalMessage(c.gray + " - " + Server.DefaultColor + "Most Valued Player: " + z.name + " - " + z.overallKilled + ":" + z.overallDied + c.gray + " - ");
                else
                    Player.GlobalMessage(c.gray + " - " + Server.DefaultColor + "#" + loop + ": " + z.name + " - " + z.overallKilled + ":" + z.overallDied + c.gray + " - ");
            }
        lol:
            removeTempFlagBlocks();
            blueTeamCaptures = 0;
            redTeamCaptures = 0;
            Player.players.ForEach(delegate(Player player)
            {
                if (player.level.name == currentLevelName)
                {
                    player.pteam = 0;
                    player.isHoldingFlag = false;

                    red.Clear();
                    blu.Clear();
                    player.color = player.group.color;
                }
            });
            timer.Enabled = false;
            return;
        }

        //Main Game Finishes Here - support functions after this

        public int CTFStatus()
        {
            return Server.ctfGameStatus;
        }

        public bool GameInProgess()
        {
            return Server.ctfRound;
        }

        public void tempFlagBlock(Player player, string color)
        {
            Level levell = Level.Find(currentLevelName);
            if (levell == null) return;
            ushort x; ushort y; ushort z;
            if (getTeam(player) == "blue" && color == "blue")
            {
                //DRAW ON PLAYER HEAD
                x = (ushort)(player.pos[0] / 32);
                y = (ushort)(player.pos[1] / 32 + 4);
                z = (ushort)(player.pos[2] / 32);

                if (blueFlagblock.x == x && blueFlagblock.y == y && blueFlagblock.z == z) { return; }
                int loop = 0;
                while (loop < Server.vulnerable)
                {
                    levell.Blockchange(blueFlagblock.x, (ushort)((blueFlagblock.y + loop)), blueFlagblock.z, Block.air);
                    loop++;
                }
                loop = 0;
                while (loop != Server.vulnerable)
                {

                    blueFlagblock.type = levell.GetTile(x, y, z);

                    if (levell.GetTile(x, (ushort)(y + loop), z) == Block.air)
                        levell.Blockchange(x, (ushort)(y + loop), z, Block.red);

                    loop++;
                }

                blueFlagblock.x = x;
                blueFlagblock.y = y;
                blueFlagblock.z = z;
            }
            else if (getTeam(player) == "red" && color == "red")
            {
                //DRAW ON PLAYER HEAD
                x = (ushort)(player.pos[0] / 32);
                y = (ushort)(player.pos[1] / 32 + 4);
                z = (ushort)(player.pos[2] / 32);

                if (redFlagblock.x == x && redFlagblock.y == y && redFlagblock.z == z) { return; }

                int loop = 0;
                while (loop < Server.vulnerable)
                {
                    levell.Blockchange(redFlagblock.x, (ushort)((redFlagblock.y + loop)), redFlagblock.z, Block.air);
                    loop++;
                }
                loop = 0;
                while (loop != Server.vulnerable)
                {
                    redFlagblock.type = levell.GetTile(x, y, z);

                    if (levell.GetTile(x, (ushort)(y + loop), z) == Block.air)
                        levell.Blockchange(x, (ushort)(y + loop), z, Block.blue);

                    loop++;
                }

                redFlagblock.x = x;
                redFlagblock.y = y;
                redFlagblock.z = z;
            }
        }

        public void removeTempFlagBlock(string color)
        {
            Level levell = Level.Find(currentLevelName);
            if (levell == null) return;
            if (color == "blue")
            {
                int loop = 0;
                while (loop < Server.vulnerable)
                {
                    levell.Blockchange(blueFlagblock.x, (ushort)((blueFlagblock.y + loop)), blueFlagblock.z, Block.air);
                    loop++;
                }
            }
            else if (color == "red")
            {
                int loop = 0;
                while (loop < Server.vulnerable)
                {
                    levell.Blockchange(redFlagblock.x, (ushort)((redFlagblock.y + loop)), redFlagblock.z, Block.air);
                    loop++;
                }
            }
        }

        public void removeTempFlagBlocks()
        {
            Level levell = Level.Find(currentLevelName);
            if (levell == null) return;
            int loop2 = 0;
            while (loop2 > Server.vulnerable)
            {
                levell.Blockchange(redFlagblock.x, (ushort)((redFlagblock.y + loop2)), redFlagblock.z, Block.air);
                loop2++;
            }
            int loop = 0;
            while (loop != Server.vulnerable)
            {
                levell.Blockchange(blueFlagblock.x, (ushort)((blueFlagblock.y + loop)), blueFlagblock.z, Block.air);
                loop++;
            }
        }

        public void ChangeLevel(string LevelName, bool changeMainLevel, bool announce)
        {
            String next = LevelName;
            Server.queLevel = false;
            Server.nextLevel = "";
            Command.all.Find("load").Use(null, next.ToLower() + " 1");
            Level.Find(next).unload = false;
            Level.Find(next).mapType = MapType.Game;
            if (announce) Player.GlobalMessage("The next map has been chosen - " + c.red + next.ToLower());
            Server.currentLevel = next;
            try
            {
                string foundLocation;
                foundLocation = "levels/ctf/" + next + ".properties";
                if (!File.Exists(foundLocation))
                {
                    foundLocation = "levels/ctf/" + next;
                }
                foreach (string line in File.ReadAllLines(foundLocation))
                {
                    if (line[0] != '#')
                    {
                        string value = line.Substring(line.IndexOf(" = ") + 3);

                        switch (line.Substring(0, line.IndexOf(" = ")).ToLower())
                        {
                            case "divider": halfway = Convert.ToInt32(value); break;
                            case "buildceiling": buildCeiling = Convert.ToInt32(value); break;
                            case "redspawnx": redSpawn[0] = Convert.ToInt32(value); break;
                            case "redspawny": redSpawn[1] = Convert.ToInt32(value); break;
                            case "redspawnz": redSpawn[2] = Convert.ToInt32(value); break;
                            case "bluespawnx": blueSpawn[0] = Convert.ToInt32(value); break;
                            case "bluespawny": blueSpawn[1] = Convert.ToInt32(value); break;
                            case "bluespawnz": blueSpawn[2] = Convert.ToInt32(value); break;
                            case "redflagx": redFlag[0] = Convert.ToInt32(value); break;
                            case "redflagy": redFlag[1] = Convert.ToInt32(value); break;
                            case "redflagz": redFlag[2] = Convert.ToInt32(value); break;
                            case "blueflagx": blueFlag[0] = Convert.ToInt32(value); break;
                            case "blueflagy": blueFlag[1] = Convert.ToInt32(value); break;
                            case "blueflagz": blueFlag[2] = Convert.ToInt32(value); break;
                        }
                    }
                }
            }
            catch { }
            Player.players.ForEach(delegate(Player player)
            {
                if (player.level.name != next && player.level.name == currentLevelName)
                {
                    Command.all.Find("goto").Use(player, Server.level);
                    while (player.Loading) { Thread.Sleep(890); }
                }
            });
            currentLevelName = next;
            return;
        }

        public void ChangeLevel()
        {
            if (Server.queLevel == true)
            {
                ChangeLevel(Server.nextLevel, Server.CTFOnlyServer, true);
            }
            try
            {
                ArrayList al = new ArrayList();
                DirectoryInfo di = new DirectoryInfo("levels/");
                FileInfo[] fi = di.GetFiles("*.mcf");
                foreach (FileInfo fil in fi)
                {
                    al.Add(fil.Name.Split('.')[0]);
                }

                if (al.Count <= 2 && !Server.UseLevelList) { Server.s.Log("You must have more than 2 levels to change levels in CTF"); return; }

                if (Server.LevelList.Count < 2 && Server.UseLevelList) { Server.s.Log("You must have more than 2 levels in your level list to change levels in CTF"); return; }

                string selectedLevel1 = "";
                string selectedLevel2 = "";

            LevelChoice:
                Random r = new Random();
                int x = 0;
                int x2 = 1;
                string level = ""; string level2 = "";
                if (!Server.UseLevelList)
                {
                    x = r.Next(0, al.Count);
                    x2 = r.Next(0, al.Count);
                    level = al[x].ToString();
                    level2 = al[x2].ToString();
                }
                else
                {
                    x = r.Next(0, Server.LevelList.Count());
                    x2 = r.Next(0, Server.LevelList.Count());
                    level = Server.LevelList[x].ToString();
                    level2 = Server.LevelList[x2].ToString();
                }
                Level current = Server.mainLevel;

                if (Server.lastLevelVote1 == level || Server.lastLevelVote2 == level2 || Server.lastLevelVote1 == level2 || Server.lastLevelVote2 == level || current == Level.Find(level) || currentLevelName == level || current == Level.Find(level2) || currentLevelName == level2 || "main" == level || "main" == level2 || "tutorial" == level || "tutorial" == level2)
                    goto LevelChoice;
                else if (selectedLevel1 == "") { selectedLevel1 = level; goto LevelChoice; }
                else
                    selectedLevel2 = level2;

                Server.Level1Vote = 0; Server.Level2Vote = 0; Server.Level3Vote = 0;
                Server.lastLevelVote1 = selectedLevel1; Server.lastLevelVote2 = selectedLevel2;
                if (Server.ctfGameStatus == 4 || Server.ctfGameStatus == 0) { return; }

                if (initialChangeLevel == true)
                {
                    Server.votingforlevel = true;
                    Player.GlobalMessage(" " + c.black + "Level Vote: " + Server.DefaultColor + selectedLevel1 + ", " + selectedLevel2 + " or random " + "(" + c.lime + "1" + Server.DefaultColor + "/" + c.red + "2" + Server.DefaultColor + "/" + c.blue + "3" + Server.DefaultColor + ")");
                    System.Threading.Thread.Sleep(15000);
                    Server.votingforlevel = false;
                }

                else { Server.Level1Vote = 1; Server.Level2Vote = 0; Server.Level3Vote = 0; }


                if (Server.ctfGameStatus == 4 || Server.ctfGameStatus == 0) { return; }

                if (Server.Level1Vote >= Server.Level2Vote)
                {
                    if (Server.Level3Vote > Server.Level1Vote && Server.Level3Vote > Server.Level2Vote)
                    {
                        r = new Random();
                        int x3 = r.Next(0, al.Count);
                        ChangeLevel(al[x3].ToString(), Server.CTFOnlyServer, true);
                    }
                    ChangeLevel(selectedLevel1, Server.CTFOnlyServer, true);
                }
                else
                {
                    if (Server.Level3Vote > Server.Level1Vote && Server.Level3Vote > Server.Level2Vote)
                    {
                        r = new Random();
                        int x4 = r.Next(0, al.Count);
                        ChangeLevel(al[x4].ToString(), Server.CTFOnlyServer, true);
                    }
                    ChangeLevel(selectedLevel2, Server.CTFOnlyServer, true);
                }
                Player.players.ForEach(delegate(Player winners)
                {
                    winners.voted = false;
                });
            }
            catch { }
            return;

        }

        public void ChangeTime(object sender, ElapsedEventArgs e)
        {
            amountOfMilliseconds = amountOfMilliseconds - 10;
        }

        public bool IsInCTFGameLevel(Player p)
        {
            return p.level.name == currentLevelName;
        }

        public Player killedPlayer(Player p, ushort x, ushort y, ushort z, bool tnt, string type)
        {
            bool killed = false;
            Player pp = null;
            int money = 5;
            p.killingPeople = true;

            if (!GameInProgess()) return null;
            if (getTeam(p) == null) return null;

            foreach (Player ppp in Player.players)
            {
                bool cutoff = false;
                if (tnt)
                {
                    if (ppp.iceshield)
                        cutoff = true;
                }
                if (ppp.pos[0] / 32 == x && !cutoff && !ppp.invinciblee)
                    if ((ppp.pos[1] / 32 == y) || ((ppp.pos[1] / 32) - 1 == y) || ((ppp.pos[1] / 32) + 1 == y))
                        if (ppp.pos[2] / 32 == z)
                        {
                            if (!Server.killed.Contains(ppp) && !ppp.deathTimerOn && !p.referee && !ppp.referee && !InSpawn(ppp, ppp.pos) && ppp != p && (getTeam(p) != getTeam(ppp)))
                            {
                                if (ppp.oneup && !ppp.isHoldingFlag)
                                {
                                    ppp.oneup = false;
                                    ppp.deathTimerOn = true;
                                    ppp.deathTimer = new System.Timers.Timer(3500);
                                    ppp.deathTimer.Elapsed += new ElapsedEventHandler(ppp.resetDeathTimer);
                                    ppp.deathTimer.Enabled = true;
                                    ppp.lastDeath = DateTime.Now;
                                }
                                else
                                {
                                    if (!firstblood)
                                    {
                                        firstblood = true;
                                        Player.GlobalMessage(c.gray + " - " + p.color + p.name + c.gray + " got " + c.red + "\"firsty bloody\"! " + c.gray + " - ");
                                        money += 20;
                                    }
                                    Random r = new Random();
                                    if (r.Next(1, 3) != 2)
                                    {
                                        switch (type)
                                        {
                                            case "pistol":
                                                Player.GlobalMessage(c.gray + " - " + ppp.color + ppp.name + Server.DefaultColor + " was killed by " + p.color + p.name + c.gray + "'s amazing pistol shot! - ");
                                                break;
                                            case "lazer":
                                                Player.GlobalMessage(c.gray + " - " + ppp.color + ppp.name + Server.DefaultColor + " was lazered by " + p.color + p.name + c.gray + " - ");
                                                break;
                                            case "mine":
                                                Player.GlobalMessage(c.gray + " - " + ppp.color + ppp.name + Server.DefaultColor + " didn't look where they were walking - ");
                                                break;
                                            case "lightning":
                                                Player.GlobalMessage(c.gray + " - " + ppp.color + ppp.name + Server.DefaultColor + " got struck down by Zeus (aka " + p.color + p.name + c.gray + ") - ");
                                                break;
                                            case "gun":
                                                Player.GlobalMessage(c.gray + " - " + ppp.color + ppp.name + Server.DefaultColor + " got shot with a rocket fired by " + p.color + p.name + c.gray + " - ");
                                                break;
                                            case "tnt":
                                                Player.GlobalMessage(c.gray + " - " + ppp.color + ppp.name + Server.DefaultColor + " was exploded into bits by " + p.color + p.name + c.gray + " - ");
                                                break;
                                            case "tag":
                                                Player.GlobalMessage(c.gray + " - " + p.color + p.name + Server.DefaultColor + " tagged " + ppp.color + ppp.name + c.gray + " - ");
                                                break;
                                            default:
                                                Player.GlobalMessage(c.gray + " - " + ppp.color + ppp.name + Server.DefaultColor + " was killed by " + p.color + p.name + c.gray + " - ");
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        switch (type)
                                        {
                                            case "pistol":
                                                Player.SendMessage(p, c.gray + " - " + ppp.color + ppp.name + Server.DefaultColor + " was killed by " + p.color + p.name + c.gray + "'s amazing pistol shot! - ");
                                                Player.SendMessage(ppp, c.gray + " - " + ppp.color + ppp.name + Server.DefaultColor + " was killed by " + p.color + p.name + c.gray + "'s amazing pistol shot! - ");
                                                break;
                                            case "lazer":
                                                Player.SendMessage(p, c.gray + " - " + ppp.color + ppp.name + Server.DefaultColor + " was lazered by " + p.color + p.name + c.gray + " - ");
                                                Player.SendMessage(ppp, c.gray + " - " + ppp.color + ppp.name + Server.DefaultColor + " was lazered by " + p.color + p.name + c.gray + " - ");
                                                break;
                                            case "mine":
                                                Player.SendMessage(p, c.gray + " - " + ppp.color + ppp.name + Server.DefaultColor + " didn't look where they were walking - ");
                                                Player.SendMessage(ppp, c.gray + " - " + ppp.color + ppp.name + Server.DefaultColor + " didn't look where they were walking - ");
                                                break;
                                            case "lightning":
                                                Player.SendMessage(p, c.gray + " - " + ppp.color + ppp.name + Server.DefaultColor + " got struck down by Zeus (aka " + p.color + p.name + c.gray + ") - ");
                                                Player.SendMessage(ppp, c.gray + " - " + ppp.color + ppp.name + Server.DefaultColor + " got struck down by Zeus (aka " + p.color + p.name + c.gray + ") - ");
                                                break;
                                            case "gun":
                                                Player.SendMessage(p, c.gray + " - " + ppp.color + ppp.name + Server.DefaultColor + " got shot with a rocket fired by " + p.color + p.name + c.gray + " - ");
                                                Player.SendMessage(ppp, c.gray + " - " + ppp.color + ppp.name + Server.DefaultColor + " got shot with a rocket fired by " + p.color + p.name + c.gray + " - ");
                                                break;
                                            case "tnt":
                                                Player.SendMessage(p, c.gray + " - " + ppp.color + ppp.name + Server.DefaultColor + " was exploded into bits by " + p.color + p.name + c.gray + " - ");
                                                Player.SendMessage(ppp, c.gray + " - " + ppp.color + ppp.name + Server.DefaultColor + " was exploded into bits by " + p.color + p.name + c.gray + " - ");
                                                break;
                                            case "tag":
                                                Player.SendMessage(p, c.gray + " - " + p.color + p.name + Server.DefaultColor + " tagged " + ppp.color + ppp.name + c.gray + " - ");
                                                Player.SendMessage(ppp, c.gray + " - " + p.color + p.name + Server.DefaultColor + " tagged " + ppp.color + ppp.name + c.gray + " - ");
                                                break;
                                            default:
                                                Player.SendMessage(p, c.gray + " - " + ppp.color + ppp.name + Server.DefaultColor + " was killed by " + p.color + p.name + c.gray + " - ");
                                                Player.SendMessage(ppp, c.gray + " - " + ppp.color + ppp.name + Server.DefaultColor + " was killed by " + p.color + p.name + c.gray + " - ");
                                                break;
                                        }
                                    }
                                    if (getTeam(p) == "red")
                                        redDeaths += 1;
                                    else if (getTeam(p) == "blue")
                                        bluDeaths += 1;
                                    ppp.overallDeath++;
                                    ppp.overallDied++;
                                    if (ppp.ironman)
                                    {
                                        if (ppp.money > -150)
                                            ppp.money = ppp.money - 50;
                                    }
                                    ppp.amountKilled = 0;
                                    ppp.deathTimerOn = true;
                                    ppp.deathTimer = new System.Timers.Timer(7500);
                                    ppp.deathTimer.Elapsed += new ElapsedEventHandler(ppp.resetDeathTimer);
                                    ppp.deathTimer.Enabled = true;
                                    ppp.lastDeath = DateTime.Now;
                                    ppp.hasBeenTrapped = false;
                                    if (Server.deathcount)
                                        if (ppp.overallDeath > 0 && ppp.overallDeath % 10 == 0) Player.GlobalChat(ppp, c.gray + " - " + ppp.color + ppp.prefix + ppp.name + Server.DefaultColor + " has died &3" + ppp.overallDeath + " times!" + c.gray + " - ", false);
                                    killed = true;
                                    Server.killed.Add(ppp);
                                    pp = ppp;
                                    sendToTeamSpawn(ppp);
                                    p.amountKilled++;
                                    p.overallKilled++;
                                    if (p.killingPeople)
                                    {
                                        if (p.amountKilled >= 3)
                                        {
                                            if (p.amountKilled == 3) { Player.GlobalMessage(c.gray + " - " + p.color + p.name + Server.DefaultColor + " got a triple kill! " + c.gray + " - "); money = money + 1; }
                                            if (p.amountKilled == 4) { Player.GlobalMessage(c.gray + " - " + p.color + p.name + Server.DefaultColor + " got a quadra kill! " + c.gray + " - "); money = money + 1; }
                                            if (p.amountKilled == 5) { Player.GlobalMessage(c.gray + " - " + p.color + p.name + Server.DefaultColor + " got a penta kill! " + c.gray + " - "); money = money + 2; }
                                            if (p.amountKilled == 6) { Player.GlobalMessage(c.gray + " - " + p.color + p.name + Server.DefaultColor + " is crazy! Sextuple kill!" + c.gray + " - "); money = money + 2; }
                                            if (p.amountKilled == 7) { Player.GlobalMessage(c.gray + " - " + p.color + p.name + Server.DefaultColor + " is insane! Seputuple kill!" + c.gray + " - "); money = money + 3; }
                                            if (p.amountKilled == 8) { Player.GlobalMessage(c.gray + " - " + p.color + p.name + Server.DefaultColor + " is amazing! Octuple kill!" + c.gray + " - "); money = money + 3; }
                                            if (p.amountKilled == 9) { Player.GlobalMessage(c.gray + " - " + p.color + p.name + Server.DefaultColor + " is bonkers! Nonuple kill!" + c.gray + " - "); money = money + 3; }
                                            if (p.amountKilled == 10) { Player.GlobalMessage(c.gray + " - " + p.color + p.name + Server.DefaultColor + " is nutty! Decuple kill!" + c.gray + " - "); money = money + 5; }
                                            if (p.amountKilled == 11) { money = money + 5; }
                                            if (p.amountKilled == 12) { Player.GlobalMessage(c.gray + " - " + p.color + p.name + Server.DefaultColor + " IS LEGENDARY! DUODECUPLE KILL! " + c.gray + " - "); money = money + 7; }
                                            if (p.amountKilled == 13) { money = money + 8; }
                                            if (p.amountKilled == 14) { money = money + 9; }
                                            if (p.amountKilled == 15) { Player.GlobalMessage(c.gray + " - " + p.color + p.name + Server.DefaultColor + " IS KILLING EVERYONE! 15-TUPLE KILL! " + c.gray + " - "); money = money + 10; }
                                            if (p.amountKilled == 16) { Player.GlobalMessage(c.gray + " - " + p.color + p.name + Server.DefaultColor + " IS MAGIC! GEN-TUPLE KILL! " + c.gray + " - "); money = money + 12; }
                                            if (p.amountKilled > 16 && p.amountKilled < 100) { money = money + 12; }
                                            if (p.amountKilled == 100) { Player.GlobalMessage(c.gray + " - " + p.color + p.name + Server.DefaultColor + " OWNS CTF! 100+ KILL STREAK! " + c.gray + " - "); money = money + 50; }
                                            if (p.amountKilled > 100) { money = money + 50; } 
                                            // TODO: Add events to when a player kills a player with a high kill streak
                                        }
                                        addMoney(p, money);
                                    }
                                }
                            }
                            else { }
                        }
            }
            p.killingPeople = false;
            Server.killed.Clear();
            if (killed) return pp;
            else return null;
        }

        public string getTeam(Player p)
        {
            if (!GameInProgess()) return null;
            if (p.pteam == 2)
                return "red";
            else if (p.pteam == 1)
                return "blue";
            else
                return null;
        }

        public void joinTeam(Player p, string name)
        {
            if (!GameInProgess()) return;
            if (p.pteam != 0) return;
            if (name == "blue")
            {
                Player.GlobalMessage(c.gray + " - " + p.color + p.prefix + p.name + Server.DefaultColor + " joined the " + c.blue + "blue " + Server.DefaultColor + "team!" + c.gray + " - ");
                p.pteam = 1;
                p.color = c.blue;
                Command.all.Find("goto").Use(p, currentLevelName);
                blu.Add(p);
                sendToTeamSpawn(p);
            }
            else
            {
                Player.GlobalMessage(c.gray + " - " + p.color + p.prefix + p.name + Server.DefaultColor + " joined the " + c.red + "red " + Server.DefaultColor + "team!" + c.gray + " - ");
                p.pteam = 2;
                p.color = c.red;
                Command.all.Find("goto").Use(p, currentLevelName);
                red.Add(p);
                sendToTeamSpawn(p);
            }
        }

        public void resetFlags(Player p)
        {
            if (!GameInProgess()) return;
            if (getTeam(p) == null) return;
            Level level = Level.Find(currentLevelName);
            if (level == null) return;
            level.placeBlock((ushort)((int)redFlag[0]), (ushort)((int)redFlag[1]), (ushort)((int)redFlag[2]), Block.redflag);
            level.placeBlock((ushort)((int)blueFlag[0]), (ushort)((int)blueFlag[1]), (ushort)((int)blueFlag[2]), Block.blueflag);
            foreach (Player ppp in Player.players)
            {
                ppp.isHoldingFlag = false;
            }
            Server.blueFlagDropped = false;
            Server.redFlagDropped = false;
            Server.blueFlagHeld = false;
            Server.redFlagHeld = false;
            Server.blueFlagTimer.Enabled = false;
            Server.redFlagTimer.Enabled = false;
            Player.GlobalMessage(c.gray + " - " + Server.DefaultColor + "The flags have been reset! " + c.gray + " - ");
            removeTempFlagBlocks();
        }

        public void resetFlag(Player p)
        {
            if (!GameInProgess()) return;
            if (getTeam(p) == null) return;
            if (p.pteam != 0)
            {
                Level level = Level.Find(currentLevelName);
                if (level == null) return;
                if (getTeam(p) == "red") level.placeBlock(Convert.ToUInt16((int)redFlag[0]), Convert.ToUInt16((int)redFlag[1]), Convert.ToUInt16((int)redFlag[2]), Block.redflag);
                else if (getTeam(p) == "blue") level.placeBlock(Convert.ToUInt16((int)blueFlag[0]), Convert.ToUInt16((int)blueFlag[1]), Convert.ToUInt16((int)blueFlag[2]), Block.blueflag);
                if (getTeam(p) == "red" && Server.redFlagDropped) level.placeBlock(Convert.ToUInt16((int)redDroppedFlag[0] / 32), Convert.ToUInt16((int)redDroppedFlag[1] / 32 - 1), Convert.ToUInt16((int)redDroppedFlag[2] / 32), Block.air);
                else if (getTeam(p) == "blue" && Server.blueFlagDropped) level.placeBlock(Convert.ToUInt16((int)blueDroppedFlag[0] / 32), Convert.ToUInt16((int)blueDroppedFlag[1] / 32 - 1), Convert.ToUInt16((int)blueDroppedFlag[2] / 32), Block.air);
                removeTempFlagBlocks();
                if (getTeam(p) == "blue") Server.blueFlagDropped = false;
                else if (getTeam(p) == "red") Server.redFlagDropped = false;
                if (getTeam(p) == "red") Server.blueFlagHeld = false;
                else if (getTeam(p) == "blue") Server.redFlagHeld = false;
                if (getTeam(p) == "blue") { Player.GlobalMessage(c.gray + " - " + Server.DefaultColor + "The blue flag has been returned! " + c.gray + " - "); Server.blueFlagTimer.Enabled = false; blueDroppedFlag[0] = 0; blueDroppedFlag[1] = 0; blueDroppedFlag[2] = 0; }
                else if (getTeam(p) == "red") { Player.GlobalMessage(c.gray + " - " + Server.DefaultColor + "The red flag has been returned! " + c.gray + " - "); Server.redFlagTimer.Enabled = false; redDroppedFlag[0] = 0; redDroppedFlag[1] = 0; redDroppedFlag[2] = 0; }
            }
        }

        public void resetFlag(string str)
        {
            if (!GameInProgess()) return;
            Level level = Level.Find(currentLevelName);
            if (level == null) return;
            if (str == "red") level.placeBlock(Convert.ToUInt16((int)redFlag[0]), Convert.ToUInt16((int)redFlag[1]), Convert.ToUInt16((int)redFlag[2]), Block.redflag);
            else if (str == "blue") level.placeBlock(Convert.ToUInt16((int)blueFlag[0]), Convert.ToUInt16((int)blueFlag[1]), Convert.ToUInt16((int)blueFlag[2]), Block.blueflag);
            if (str == "red") level.placeBlock(Convert.ToUInt16((int)redDroppedFlag[0] / 32), Convert.ToUInt16((int)redDroppedFlag[1] / 32 - 1), Convert.ToUInt16((int)redDroppedFlag[2] / 32), Block.air);
            else if (str == "blue") level.placeBlock(Convert.ToUInt16((int)blueDroppedFlag[0] / 32), Convert.ToUInt16((int)blueDroppedFlag[1] / 32 - 1), Convert.ToUInt16((int)blueDroppedFlag[2] / 32), Block.air);
            if (str == "blue") Server.blueFlagDropped = false;
            else if (str == "red") Server.redFlagDropped = false;
            if (str == "blue") Server.blueFlagHeld = false;
            else if (str == "red") Server.redFlagHeld = false;
            if (str == "blue") { Player.GlobalMessage(c.gray + " - " + Server.DefaultColor + "The blue flag has been returned! " + c.gray + " - "); Server.blueFlagTimer.Enabled = false; blueDroppedFlag[0] = 0; blueDroppedFlag[1] = 0; blueDroppedFlag[2] = 0; }
            else if (str == "red") { Player.GlobalMessage(c.gray + " - " + Server.DefaultColor + "The red flag has been returned! " + c.gray + " - "); Server.redFlagTimer.Enabled = false; redDroppedFlag[0] = 0; redDroppedFlag[1] = 0; redDroppedFlag[2] = 0; }
        }

        public void PlayerDC(Player p)
        {
            ushort x, y, z; int xx, yy, zz;
            x = (ushort)((int)p.pos[0] / 32);
            y = (ushort)((int)p.pos[1] / 32 - 1);
            z = (ushort)((int)p.pos[2] / 32);
            xx = p.pos[0];
            yy = p.pos[1];
            zz = p.pos[2];
            dropFlag(p, x, y, z, xx, yy, zz);
            if (Server.pctf.red.Contains(p)) { Server.pctf.red.Remove(p); p.pteam = 0; }
            if (Server.pctf.blu.Contains(p)) { Server.pctf.blu.Remove(p); p.pteam = 0; }
        }

        public void sendToTeamSpawn(Player p)
        {
            if (!GameInProgess()) return;
            ushort x, y, z; int xx, yy, zz;
            x = (ushort)((int)p.pos[0] / 32);
            y = (ushort)((int)p.pos[1] / 32 - 1);
            z = (ushort)((int)p.pos[2] / 32);
            xx = p.pos[0];
            yy = p.pos[1];
            zz = p.pos[2];
            if (p.pteam == 2)
            {
                Player.GlobalDie(p, false);
                if (!p.hidden) Player.GlobalSpawn(p, (ushort)((int)redSpawn[0] * 32), (ushort)((int)redSpawn[1] * 32), (ushort)((int)redSpawn[2] * 32), p.rot[0], p.rot[1], true, "");
                else unchecked { p.SendPos((byte)-1, (ushort)((int)redSpawn[0] * 32), (ushort)((int)redSpawn[1] * 32), (ushort)((int)redSpawn[2] * 32), p.rot[0], 0); }
            }
            else
            {
                Player.GlobalDie(p, false);
                if (!p.hidden) Player.GlobalSpawn(p, (ushort)((int)blueSpawn[0] * 32), (ushort)((int)blueSpawn[1] * 32), (ushort)((int)blueSpawn[2] * 32), p.rot[0], p.rot[1], true, "");
                else unchecked { p.SendPos((byte)-1, (ushort)((int)blueSpawn[0] * 32), (ushort)((int)blueSpawn[1] * 32), (ushort)((int)blueSpawn[2] * 32), p.rot[0], 0); }
            }
            Thread dropThread = new Thread(new ThreadStart(delegate
            {
                Thread.Sleep(500);
                dropFlag(p, x, y, z, xx, yy, zz);
            }));
            dropThread.Start();
        }

        public void captureFlag(Player p)
        {
            if (!GameInProgess()) return;
            if (getTeam(p) == null) return;
            if (getTeam(p) == "blue" && !Server.blueFlagDropped && !Server.blueFlagHeld)
            {
                Server.blueFlagTimer.Enabled = false;
                blueTeamCaptures++;
                Player.GlobalMessage(c.gray + " - " + p.color + p.prefix + p.name + Server.DefaultColor + " captured the " + c.red + "red " + Server.DefaultColor + "flag!" + c.gray + " - ");
                addMoney(p, 50);
                blu.ForEach(delegate(Player player1)
                {
                    if (player1 != p)
                    {
                        addMoney(player1, 5);
                    }
                });
                resetFlags(p);
                if (blueTeamCaptures > 4) { HandOutRewards(); return; }
                Player.GlobalMessage(c.gray + " - " + c.blue + "Blue: " + Server.DefaultColor + blueTeamCaptures + c.red + " Red: " + Server.DefaultColor + redTeamCaptures);
                removeTempFlagBlock("blue");
            }
            else if (getTeam(p) == "red" && !Server.redFlagDropped && !Server.redFlagHeld)
            {
                Server.redFlagTimer.Enabled = false;
                redTeamCaptures++;
                Player.GlobalMessage(c.gray + " - " + p.color + p.prefix + p.name + Server.DefaultColor + " captured the " + c.blue + "blue " + Server.DefaultColor + "flag!" + c.gray + " - ");
                addMoney(p, 50);
                red.ForEach(delegate(Player player1)
                {
                    if (player1 != p)
                    {
                        addMoney(player1, 5);
                    }
                });
                resetFlags(p);
                if (redTeamCaptures > 4) { HandOutRewards(); return; }
                Player.GlobalMessage(c.gray + " - " + c.blue + "Blue: " + Server.DefaultColor + blueTeamCaptures + c.red + " Red: " + Server.DefaultColor + redTeamCaptures);
                removeTempFlagBlock("red");
            }
            Server.vulnerable = 1;
        }

        public void dropFlag(Player p, ushort x, ushort y, ushort z, int xx, int yy, int zz)
        {
            if (!GameInProgess()) return;
            if (getTeam(p) == null) return;
            Level level = Level.Find(currentLevelName);
            if (level == null) return;
            if (p.isHoldingFlag)
            {
                if (getTeam(p) == "red" && Server.blueFlagHeld) level.placeBlock(x, y, z, Block.blueflag);
                if (getTeam(p) == "blue" && Server.redFlagHeld) level.placeBlock(x, y, z, Block.redflag);
                if (getTeam(p) == "red") Server.blueFlagDropped = true;
                else if (getTeam(p) == "blue") Server.redFlagDropped = true;
                if (getTeam(p) == "red") { Server.blueFlagHeld = false; removeTempFlagBlock("red"); Server.blueFlagTimer.Enabled = true; blueDroppedFlag[0] = xx; blueDroppedFlag[1] = yy; blueDroppedFlag[2] = zz; }
                else if (getTeam(p) == "blue") { Server.redFlagHeld = false; removeTempFlagBlock("blue"); Server.redFlagTimer.Enabled = true; redDroppedFlag[0] = xx; redDroppedFlag[1] = yy; redDroppedFlag[2] = zz; }
                p.isHoldingFlag = false;
                Player.GlobalMessage(c.gray + " - " + c.blue + p.name + Server.DefaultColor + " dropped the flag!" + c.gray + " - ");
            }
        }

        public void returnFlag(Player p)
        {
            if (!GameInProgess()) return;
            if (getTeam(p) == null) return;
            resetFlag(p);
            if (getTeam(p) == "red" && !Server.blueFlagHeld) Player.GlobalMessage(c.gray + " - " + p.color + p.prefix + p.name + Server.DefaultColor + " returned the " + c.red + "red " + Server.DefaultColor + "flag!" + c.gray + " - ");
            if (getTeam(p) == "blue" && !Server.redFlagHeld) Player.GlobalMessage(c.gray + " - " + p.color + p.prefix + p.name + Server.DefaultColor + " returned the " + c.blue + "blue " + Server.DefaultColor + "flag!" + c.gray + " - ");
        }

        public bool pickupFlag(Player p)
        {
            if (!GameInProgess()) return true;
            if (getTeam(p) == null) return true;
            if (p.deathTimerOn) return true;
            if (getTeam(p) == "blue" && !Server.redFlagHeld)
            {
                p.isHoldingFlag = true;
                Server.redFlagHeld = true;
                Server.redFlagDropped = false;
                Server.redFlagTimer.Enabled = false;
                Player.GlobalMessage(c.gray + " - " + p.color + p.prefix + p.name + Server.DefaultColor + " grabbed the " + c.red + "red " + Server.DefaultColor + "flag!" + c.gray + " - ");
                return false;
            }
            else if (getTeam(p) == "red" && !Server.blueFlagHeld)
            {
                p.isHoldingFlag = true;
                Server.blueFlagHeld = true;
                Server.blueFlagDropped = false;
                Server.blueFlagTimer.Enabled = false;
                Player.GlobalMessage(c.gray + " - " + p.color + p.prefix + p.name + Server.DefaultColor + " grabbed the " + c.blue + "blue " + Server.DefaultColor + "flag!" + c.gray + " - ");
                return false;
            }
            else
            {
                return true;
            }
        }

        public void disarm(Player p, string strng)
        {
            if (strng == "mine")
            {
                p.minePlacement[0] = 0; p.minePlacement[1] = 0; p.minePlacement[2] = 0;
                p.minesPlaced = 0;
                p.SendMessage(c.gray + " - " + Server.DefaultColor + "Your mine has been disarmed!" + c.gray + " - ");
            }
            else if (strng == "trap")
            {
                if (p.trapsPlaced == 0) return;
                Player.players.ForEach(delegate(Player player)
                {
                    player.hasBeenTrapped = false;
                });
                p.trapPlacement[0] = 0; p.trapPlacement[1] = 0; p.trapPlacement[2] = 0;
                p.trapsPlaced = 0;
                p.SendMessage(c.gray + " - " + Server.DefaultColor + "Your trap has been disarmed!" + c.gray + " - ");
            }
            if (strng == "tnt")
            {
                p.tntPlacement[0] = 0; p.tntPlacement[1] = 0; p.tntPlacement[2] = 0;
                p.tntPlaced = 0;
                p.SendMessage(c.gray + " - " + Server.DefaultColor + "Your tnt has been disarmed!" + c.gray + " - ");
            }
            else
                return;
        }

        public bool grabFlag(Player p, string color, ushort x, ushort y, ushort z)
        {
            bool returne = false;
            if (red.Count < 1 || blu.Count < 1) { p.SendMessage("There must be at least 2 players on both teams to grab a flag!"); return true; }
            if (getTeam(p) == color)
            {
                if (getTeam(p) == "blue" && Server.blueFlagDropped && !Server.blueFlagHeld)
                    returnFlag(p);
                else if (getTeam(p) == "red" && Server.redFlagDropped && !Server.redFlagHeld)
                    returnFlag(p);
                else
                {
                    if (p.isHoldingFlag)
                    {
                        captureFlag(p);
                    }
                    returne = true;
                }
            }
            else
            {
                if (getTeam(p) == "blue" && !Server.redFlagHeld)
                    returne = pickupFlag(p);
                else if (getTeam(p) == "red" && !Server.blueFlagHeld)
                    returne = pickupFlag(p);
                else
                {
                    returne = false;
                }
            }
            return returne; //return true if you dont want to destroy block
        }


        public void addMoney(Player p, int amount)
        {
            Player.SendMessage(p, c.gray + " - " + Server.DefaultColor + "You gained " + amount + " " + Server.moneys + "!" + c.gray + " - ");
            p.money = p.money + amount;
        }

        bool OnSide(Player p, string name)
        {
            int[] b = { 0 };
            if (name == "red")
            {
                b[0] = redSpawn[0];
            }
            else
            {
                b[0] = blueSpawn[0];
            }
            if (b[0] < halfway && p.pos[0] / 32 < halfway)
                return true;
            else if (b[0] > halfway && p.pos[0] / 32 > halfway)
                return true;
            else
                return false;
        }

        public bool InSpawn(Player p, ushort[] pos)
        {
            if (getTeam(p) == "blue")
            {
                if (Math.Abs((pos[0] / 32) - blueSpawn[0]) <= 5
                    && Math.Abs((pos[1] / 32) - blueSpawn[1]) <= 5
                    && Math.Abs((pos[2] / 32) - blueSpawn[2]) <= 5)
                {
                    return true;
                }
            }
            if (getTeam(p) == "red")
            {
                if (Math.Abs((pos[0] / 32) - redSpawn[0]) <= 5
                    && Math.Abs((pos[1] / 32) - redSpawn[1]) <= 5
                    && Math.Abs((pos[2] / 32) - redSpawn[2]) <= 5)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
