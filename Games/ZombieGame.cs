/*
	Copyright 2010 MCLawl Team - 
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;

namespace MCForge
{
    public class ZombieGame
    {
        public int amountOfRounds = 0;
        public int limitRounds = 0;
        public int aliveCount = 0;
        public int amountOfMilliseconds = 0;
        public string currentZombieLevel = "";
        public static System.Timers.Timer timer;
        public bool initialChangeLevel = false;
        public string currentLevelName = "";
        public static List<Player> alive = new List<Player>();
        public static List<Player> infectd = new List<Player>();
        string[] infectMessages = new string[] { " WIKIWOO'D ", " stuck their teeth into ", " licked ", " danubed ", " made ", " tripped ", " made some zombie babies with ", " made ", " tweeted ", " made ", " infected ", " iDotted ", "", " transplanted " };
        string[] infectMessages2 = new string[] { "", "", "'s brain", "", " meet their maker", "", "", " see the dark side", "", " open source", "", "", " got nommed on", "'s living brain" };
        public ZombieGame() { }
        public static System.Timers.Timer checkTimer = new System.Timers.Timer(5000);

        public void StartGame(int status, int amount)
        {
            //status: 0 = not started, 1 = always on, 2 = one time, 3 = certain amount of rounds, 4 = stop round next round

            if (status == 0) return;

            //SET ALL THE VARIABLES!
            if (Server.UseLevelList && Server.LevelList == null)
                Server.ChangeLevels = false;
            Server.ZombieModeOn = true;
            Server.gameStatus = status;
            Server.zombieRound = false;
            initialChangeLevel = false;
            limitRounds = amount + 1;
            amountOfRounds = 0;
            //SET ALL THE VARIABLES?!?

            //Start the main Zombie thread
            Thread t = new Thread(MainLoop);
            t.Start();
            checkTimer.Elapsed += delegate { CheckXP(); };
            checkTimer.Start();

        }

        private void MainLoop()
        {
            if (Server.gameStatus == 0) return;
            bool cutVariable = true;

            if (initialChangeLevel == false)
            {
                ChangeLevel();
                initialChangeLevel = true;
            }

            while (cutVariable == true)
            {
                int gameStatus = Server.gameStatus;
                Server.zombieRound = false;
                amountOfRounds = amountOfRounds + 1;

                if (gameStatus == 0) { cutVariable = false; return; }
                else if (gameStatus == 1) { MainGame(); if (Server.ChangeLevels) ChangeLevel(); }
                else if (gameStatus == 2) { MainGame(); if (Server.ChangeLevels) ChangeLevel(); cutVariable = false; Server.gameStatus = 0; return; }
                else if (gameStatus == 3)
                {
                    if (limitRounds == amountOfRounds) { cutVariable = false; Server.gameStatus = 0; limitRounds = 0; initialChangeLevel = false; Server.ZombieModeOn = false; Server.zombieRound = false; return; }
                    else { MainGame(); if (Server.ChangeLevels) ChangeLevel(); }
                }
                else if (gameStatus == 4)
                { cutVariable = false; Server.gameStatus = 0; Server.gameStatus = 0; limitRounds = 0; initialChangeLevel = false; Server.ZombieModeOn = false; Server.zombieRound = false; return; }
            }
        }

        private void MainGame ()
		{
			if (Server.gameStatus == 0)
				return;
			GoBack:
			Player.GlobalMessageLevel (Level.Find (currentLevelName), Player.MessageType.Announcement, "&4Round Start:&f 1:00");
			Thread.Sleep (55000);
			if (!Server.ZombieModeOn) {
				return;
			}
			Server.s.Log (Convert.ToString (Server.ChangeLevels) + " " + Convert.ToString (Server.ZombieOnlyServer) + " " + Convert.ToString (Server.UseLevelList) + " " + string.Join (",", Server.LevelList.ToArray ()));
			Player.GlobalMessageLevel (Level.Find (currentLevelName), Player.MessageType.Announcement, "&4Round Start:&f 5...");
			Thread.Sleep (1000);
			if (!Server.ZombieModeOn) {
				return;
			}
			Player.GlobalMessageLevel (Level.Find (currentLevelName), Player.MessageType.Announcement, "&4Round Start:&f 4...");
			Thread.Sleep (1000);
			if (!Server.ZombieModeOn) {
				return;
			}
			Player.GlobalMessageLevel (Level.Find (currentLevelName), Player.MessageType.Announcement, "&4Round Start:&f 3...");
			Thread.Sleep (1000);
			if (!Server.ZombieModeOn) {
				return;
			}
			Player.GlobalMessageLevel (Level.Find (currentLevelName), Player.MessageType.Announcement, "&4Round Start:&f 2...");
			Thread.Sleep (1000);
			if (!Server.ZombieModeOn) {
				return;
			}
			Player.GlobalMessageLevel (Level.Find (currentLevelName), Player.MessageType.Announcement, "&4Round Start:&f 1...");
			Thread.Sleep (1000);
			if (!Server.ZombieModeOn) {
				return;
			}
			Server.zombieRound = true;
			int playerscountminusref = 0;
			List<Player> players = new List<Player> ();
			foreach (Player playere in Player.players) {
				if (playere.referee) {
					playere.color = playere.group.color;
				} else {
					if (playere.level.name == currentLevelName) {
						playere.color = playere.group.color;
						players.Add (playere);
						playerscountminusref++;
					}
				}
			}
			if (playerscountminusref < 2) {
				Player.GlobalMessageLevel (Level.Find (currentLevelName), Player.MessageType.Announcement, c.red + "ERROR: Need more than 2 players to play");
				goto GoBack;
			}

			theEnd:
			Random random = new Random ();
			int firstinfect = random.Next (players.Count ());
			Player player = null;
			if (Server.queZombie == true)
				player = Player.Find (Server.nextZombie);
			else
				player = players [firstinfect];

			if (player.level.name != currentLevelName)
				goto theEnd;

			Player.GlobalMessageLevel (Level.Find (currentLevelName), Player.MessageType.Announcement, player.color + player.name + Server.DefaultColor + " started the infection!");
			Thread.Sleep (3000);
			Player.GlobalMessageLevel (Level.Find (currentLevelName), Player.MessageType.Announcement, "");
			player.infected = true;
			player.color = c.red;
			Player.GlobalDie (player, false);
			Player.GlobalSpawn (player, player.pos [0], player.pos [1], player.pos [2], player.rot [0], player.rot [1], false);

			Server.zombieRound = true;
			int amountOfMinutes = random.Next (4, 8);
			Player.GlobalMessageLevel (Level.Find (currentLevelName), Player.MessageType.Status1, "The round will last for " + amountOfMinutes + " minutes!");
			amountOfMilliseconds = (60000 * amountOfMinutes);

			timer = new System.Timers.Timer (amountOfMilliseconds);
			timer.Elapsed += new ElapsedEventHandler (EndRound);
			timer.Enabled = true;

			foreach (Player playaboi in Player.players) {
				if (playaboi != player)
					alive.Add (playaboi);
			}

			infectd.Clear ();
				foreach (Player pl in Player.players) {
					if (pl.level.name == currentLevelName) {
                        pl.model = "steve";
                        pl.SendChangeModel(pl.id, "steve");
					}
				}
			if (Server.queZombie == true) {
				infectd.Add (Player.Find (Server.nextZombie));
                Player.Find(Server.nextZombie).model = "zombie";
                Player.Find(Server.nextZombie).SendChangeModel(Player.Find(Server.nextZombie).id, "zombie");
			} else {
				infectd.Add (player);
                player.model = "zombie";
                player.SendChangeModel(player.id, "zombie");
			}
            aliveCount = alive.Count;

            while (aliveCount > 0)
            {
                aliveCount = alive.Count;
                infectd.ForEach(delegate(Player player1)
                {
                    if (player1.color != c.red)
                    {
                        player1.color = c.red;
                        Player.GlobalDie(player1, false);
                        Player.GlobalSpawn(player1, player1.pos[0], player1.pos[1], player1.pos[2], player1.rot[0], player1.rot[1], false);
                    }
                    alive.ForEach(delegate(Player player2)
                    {
                        if (player2.color != player2.group.color)
                        {
                            player2.color = player2.group.color;
                            Player.GlobalDie(player2, false);
                            Player.GlobalSpawn(player2, player2.pos[0], player2.pos[1], player2.pos[2], player2.rot[0], player2.rot[1], false);
                        }
                        if (player2.pos[0] / 32 == player1.pos[0] / 32 || player2.pos[0] / 32 == player1.pos[0] / 32 + 1 || player2.pos[0] / 32 == player1.pos[0] / 32 - 1)
                        {
                            if (player2.pos[1] / 32 == player1.pos[1] / 32 || player2.pos[1] / 32 == player1.pos[1] / 32 - 1 || player2.pos[1] / 32 == player1.pos[1] / 32 + 1)
                            {
                                if (player2.pos[2] / 32 == player1.pos[2] / 32 || player2.pos[2] / 32 == player1.pos[2] / 32 + 1 || player2.pos[2] / 32 == player1.pos[2] / 32 - 1)
                                {
                                    if (!player2.infected && player1.infected && !player2.referee && !player1.referee && player1 != player2 && player1.level.name == currentLevelName && player2.level.name == currentLevelName)
                                    {
                                        player2.infected = true;
                                        infectd.Add(player2);
                                        player2.model = "zombie";
                                        player2.SendChangeModel(player2.id, "zombie");
                                    }
                                        alive.Remove(player2);
                                        players.Remove(player2);
                                        player2.blockCount = 25;
                                        if (Server.lastPlayerToInfect == player1.name)
                                        {
                                            Server.infectCombo++;
                                            if (Server.infectCombo >= 2)
                                            {
                                                player1.SendMessage("You gained " + (4 - Server.infectCombo) + " " + Server.moneys);
                                                player1.money = player1.money + 4 - Server.infectCombo;
                                                Player.GlobalMessageLevel(Level.Find(currentLevelName), Player.MessageType.Status2, player1.color + player1.name + " is on a rampage! " + (Server.infectCombo + 1) + " infections in a row!");
                                            }
                                        }
                                        else
                                        {
                                            Server.infectCombo = 0;
                                        }
                                        Server.lastPlayerToInfect = player1.name;
                                        player1.infectThisRound++;
                                        int cazzar = random.Next(0, infectMessages.Length);
                                        if (File.Exists("text/infect/" + player1.name + ".txt"))
                                        {
                                            Player.GlobalMessageLevel(Level.Find(currentLevelName), Player.MessageType.Status2, c.red + player1.Original + c.yellow + " " + File.ReadAllText("text/infect/" + player1.name + ".txt") + " " + c.red + player2.name);
                                        }
                                        if (infectMessages2[cazzar] == "")
                                        {
                                            Player.GlobalMessageLevel(Level.Find(currentLevelName), Player.MessageType.Status2, c.red + player1.name + c.yellow + infectMessages[cazzar] + c.red + player2.name);
                                        }
                                        else if (infectMessages[cazzar] == "")
                                        {
                                            Player.GlobalMessageLevel(Level.Find(currentLevelName), Player.MessageType.Status2, c.red + player2.name + c.yellow + infectMessages2[cazzar]);
                                        }
                                        else
                                        {
                                            Player.GlobalMessageLevel(Level.Find(currentLevelName), Player.MessageType.Status2, c.red + player1.name + c.yellow + infectMessages[cazzar] + c.red + player2.name + c.yellow + infectMessages2[cazzar]);
                                        }
                                        player2.color = c.red;
                                        player1.playersInfected = player1.playersInfected++;
                                        Player.GlobalDie(player2, false);
                                        Player.GlobalSpawn(player2, player2.pos[0], player2.pos[1], player2.pos[2], player2.rot[0], player2.rot[1], false);
                                        Thread.Sleep(500);
                                    }
                                }
                            }
                    });
                });
                Thread.Sleep(500);
            }
            if (Server.gameStatus == 0)
            {
                Server.gameStatus = 4;
                return;
            }
            else
            {
                HandOutRewards();
            }
        }

        public void EndRound(object sender, ElapsedEventArgs e)
        {
            if (Server.gameStatus == 0) return;
            Player.GlobalMessageLevel(Level.Find(currentLevelName), Player.MessageType.Announcement, "&4Round End:&f 5"); Thread.Sleep(1000);
            Player.GlobalMessageLevel(Level.Find(currentLevelName), Player.MessageType.Announcement, "&4Round End:&f 4"); Thread.Sleep(1000);
            Player.GlobalMessageLevel(Level.Find(currentLevelName), Player.MessageType.Announcement, "&4Round End:&f 3"); Thread.Sleep(1000);
            Player.GlobalMessageLevel(Level.Find(currentLevelName), Player.MessageType.Announcement, "&4Round End:&f 2"); Thread.Sleep(1000);
            Player.GlobalMessageLevel(Level.Find(currentLevelName), Player.MessageType.Announcement, "&4Round End:&f 1"); Thread.Sleep(1000);
            HandOutRewards();
        }

        public void HandOutRewards()
        {
            Server.zombieRound = false; amountOfMilliseconds = 0;
            if (Server.gameStatus == 0) return;
            Player.GlobalMessageLevel(Level.Find(currentLevelName), Player.MessageType.Announcement, c.lime + "The game has ended!");
            if (aliveCount == 0)
                Player.GlobalMessageLevel(Level.Find(currentLevelName), Player.MessageType.Status2, c.maroon + "Zombies have won this round.");
            else
                Player.GlobalMessageLevel(Level.Find(currentLevelName), Player.MessageType.Status2, c.green + "Congratulations to our survivor(s)");
            timer.Enabled = false;
            string playersString = "";
            if (aliveCount == 0)
            {
                foreach (Player winners in Player.players)
                {
                    if (winners.level.name == currentLevelName)
                    {
                        winners.blockCount = 50;
                        winners.infected = false;
                        winners.infectThisRound = 0;
                        if (winners.level.name == currentLevelName)
                        {
                            winners.color = winners.group.color;
                            playersString += winners.group.color + winners.name + c.white + ", ";
                        }
                    }
                }
            }
            else
            {
                alive.ForEach(delegate(Player winners)
                {
                    winners.blockCount = 50;
                    winners.infected = false;
                    winners.infectThisRound = 0;
                    if (winners.level.name == currentLevelName)
                    {
                        winners.color = winners.group.color;
                        playersString += winners.group.color + winners.name + c.white + ", ";
                    }
                });
            }
            Player.GlobalMessageLevel(Level.Find(currentLevelName), Player.MessageType.Status3, playersString);
            foreach (Player winners in Player.players)
            {
                if (aliveCount == 0 && winners.level.name == currentLevelName)
                {
                    Player.GlobalDie(winners, false);
                    Player.GlobalSpawn(winners, winners.pos[0], winners.pos[1], winners.pos[2], winners.rot[0], winners.rot[1], false);
                    Random random2 = new Random();
                    int randomInt = 0;
                    if (winners.playersInfected > 5)
                    {
                        randomInt = random2.Next(1, winners.playersInfected);
                    }
                    else
                    {
                        randomInt = random2.Next(1, 5);
                    }
                    Player.SendMessage(winners, c.gold + "You gained " + randomInt + " " + Server.moneys);
                    winners.blockCount = 50;
                    winners.playersInfected = 0;
                    winners.money = winners.money + randomInt;
                    winners.points += ( 100 * winners.playersInfected ) + 25;
                }
                else if ((aliveCount == 1 && !winners.infected) && winners.level.name == currentLevelName)
                {
                    Player.GlobalDie(winners, false);
                    Player.GlobalSpawn(winners, winners.pos[0], winners.pos[1], winners.pos[2], winners.rot[0], winners.rot[1], false);
                    Random random2 = new Random();
                    int randomInt = 0;
                    randomInt = random2.Next(1, 15);
                    Player.SendMessage(winners, c.gold + "You gained " + randomInt + " " + Server.moneys);
                    winners.blockCount = 50;
                    winners.playersInfected = 0;
                    winners.money = winners.money + randomInt;
                    winners.points += 500;
                }
            }
            try { alive.Clear(); infectd.Clear(); 
				foreach (Player pl in Player.players) {
					if (pl.level == Level.Find(currentLevelName)) {
						pl.model = "steve";
                        pl.SendChangeModel(pl.id, "steve");
					}
				}
			}
            catch { }
            foreach (Player player in Player.players)
            {
                player.infected = false;
                player.color = player.group.color;
                Player.GlobalDie(player, false);
                Player.GlobalSpawn(player, player.pos[0], player.pos[1], player.pos[2], player.rot[0], player.rot[1], false);
                if (player.level.name == currentLevelName)
                {
                    if (player.referee)
                    {
                        player.SendMessage("You gained one " + Server.moneys + " because you're a ref. Would you like a medal as well?");
                        player.money++;
                    }
                }
            }
            return;
        }

        public void ChangeLevel()
        {
            if (Server.queLevel == true)
            {
                ChangeLevel(Server.nextLevel, Server.ZombieOnlyServer);
            }
            try
            {
                if (Server.ChangeLevels)
                {
                    ArrayList al = new ArrayList();
                    DirectoryInfo di = new DirectoryInfo("levels/");
                    FileInfo[] fi = di.GetFiles("*.mcf");
                    foreach (FileInfo fil in fi)
                    {
                        al.Add(fil.Name.Split('.')[0]);
                    }
                    if (al.Count <= 2 && !Server.UseLevelList) { Server.s.Log("You must have more than 2 levels to change levels in Zombie Survival"); return; }

                    if (Server.LevelList.Count < 2 && Server.UseLevelList) { Server.s.Log("You must have more than 2 levels in your level list to change levels in Zombie Survival"); return; }

                    string selectedLevel1 = "";
                    string selectedLevel2 = "";
                    goto LevelChoice;

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

                    if (Server.lastLevelVote1 == level || Server.lastLevelVote2 == level2 || Server.lastLevelVote1 == level2 || Server.lastLevelVote2 == level || current == Level.Find(level) || currentZombieLevel == level || current == Level.Find(level2) || currentZombieLevel == level2)
                        goto LevelChoice;
                    else if (selectedLevel1 == "") { selectedLevel1 = level; goto LevelChoice; }
                    else
                        selectedLevel2 = level2;

                    Server.Level1Vote = 0; Server.Level2Vote = 0; Server.Level3Vote = 0;
                    Server.lastLevelVote1 = selectedLevel1; Server.lastLevelVote2 = selectedLevel2;

                    if (Server.gameStatus == 4 || Server.gameStatus == 0) { return; }

                    if (initialChangeLevel == true)
                    {
                        Server.votingforlevel = true;
                        Player.GlobalMessage(" " + c.black + "Level Vote: " + Server.DefaultColor + selectedLevel1 + ", " + selectedLevel2 + " or random " + "(" + c.lime + "1" + Server.DefaultColor + "/" + c.red + "2" + Server.DefaultColor + "/" + c.blue + "3" + Server.DefaultColor + ")");
                        System.Threading.Thread.Sleep(30000);
                        Server.votingforlevel = false;
                    }
                    else { Server.Level1Vote = 1; Server.Level2Vote = 0; Server.Level3Vote = 0; }

                    if (Server.gameStatus == 4 || Server.gameStatus == 0) { return; }

                    if (Server.Level1Vote >= Server.Level2Vote)
                    {
                        if (Server.Level3Vote > Server.Level1Vote && Server.Level3Vote > Server.Level2Vote)
                        {
                            r = new Random();
                            int x3 = r.Next(0, al.Count);
                            ChangeLevel(al[x3].ToString(), Server.ZombieOnlyServer);
                        }
                        ChangeLevel(selectedLevel1, Server.ZombieOnlyServer);
                    }
                    else
                    {
                        if (Server.Level3Vote > Server.Level1Vote && Server.Level3Vote > Server.Level2Vote)
                        {
                            r = new Random();
                            int x4 = r.Next(0, al.Count);
                            ChangeLevel(al[x4].ToString(), Server.ZombieOnlyServer);
                        }
                        ChangeLevel(selectedLevel2, Server.ZombieOnlyServer);
                    }
                    Player.players.ForEach(delegate(Player winners)
                    {
                        winners.voted = false;
                    });
                }
            }
            catch { }
        }

        //Main Game Finishes Here - support functions after this

        public void InfectedPlayerDC()
        {
            if (Server.gameStatus == 0)
                return;
            //This is for when the first zombie disconnects
            Random random = new Random();
            if (Server.players == 0 || Server.players == 1) { HandOutRewards(); return; }
            if ((Server.gameStatus != 0 && Server.zombieRound) && infectd.Count <= 0)
            {
                int firstinfect = random.Next(alive.Count) - 1;
                if (firstinfect < 0) firstinfect = 0;
                while (alive[firstinfect].referee == true || alive[firstinfect].level.name == Server.zombie.currentLevelName)
                {
                    int tries = 0;
                    if (firstinfect == alive.Count - 1)
                    {
                        firstinfect = 0;
                        tries++;
                    }
                    else
                    {
                        firstinfect++;
                    }
                    if (tries > 3) { HandOutRewards(); return; }
                }
                Player.GlobalMessageLevel(Level.Find(currentLevelName), Player.MessageType.Announcement, alive[firstinfect].color + alive[firstinfect].name + Server.DefaultColor + " continued the infection!");
                Thread.Sleep(3000);
                Player.GlobalMessageLevel(Level.Find(currentLevelName), Player.MessageType.Announcement, "");
                alive[firstinfect].color = c.red;
                Player.GlobalDie(alive[firstinfect], false);
                Player.GlobalSpawn(alive[firstinfect], alive[firstinfect].pos[0], alive[firstinfect].pos[1], alive[firstinfect].pos[2], alive[firstinfect].rot[0], alive[firstinfect].rot[1], false);
                infectd.Add(alive[firstinfect]);
                alive[firstinfect].model = "zombie";
                alive.Remove(alive[firstinfect]);
                return;
            }
            if ((Server.gameStatus != 0 && Server.zombieRound) && alive.Count == 0)
            {
                HandOutRewards();
            }
        }

        public bool InfectedPlayerLogin(Player p)
        {
            if (p != null && p.level.name == Server.zombie.currentLevelName && Server.zombie.GameInProgess() && infectd.Count != 0)
            {
                p.blockCount = 50;
                try
                {
                    p.SendMessage(Player.MessageType.Status1, "You have joined in the middle of a round. You are now infected!", true);
                    Server.zombie.InfectPlayer(p);
                }
                catch { }
                return true;
            }
            if (Server.ZombieModeOn && p.level.name == Server.zombie.currentLevelName) alive.Add(p);
            return false;
        }

        public int ZombieStatus()
        {
            return Server.gameStatus;
        }

        public bool GameInProgess()
        {
            return Server.zombieRound;
        }

        public void InfectPlayer (Player p)
		{
			if (Server.zombieRound == false)
				return;
			if (p == null)
				return;
			infectd.Add(p);
            p.model = "zombie";
            p.SendChangeModel(p.id, "zombie");
            alive.Remove(p);
            p.infected = true;
            p.color = c.red;
            Player.GlobalDie(p, false);
            Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
            aliveCount = alive.Count;
        }

        public void DisinfectPlayer(Player p)
        {
            if (Server.zombieRound == false) return;
            if (p == null) return;
            infectd.Remove(p);
            p.model = "steve";
            p.SendChangeModel(p.id, "steve");
            alive.Add(p);
            p.infected = false;
            p.color = p.group.color;
            Player.GlobalDie(p, false);
            Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
            aliveCount = alive.Count;
        }

        public void ChangeLevel(string LevelName, bool changeMainLevel)
        {
            String next = LevelName;
            currentLevelName = next;
            Server.queLevel = false;
            Server.nextLevel = "";
            Command.all.Find("load").Use(null, next.ToLower() + " 4");
            Level.Find(next).mapType = MapType.Game;
            Level.Find(next).unload = false;
            Level.Find(next).mapType = MapType.Game;
            Player.GlobalMessageLevel(Level.Find(currentLevelName), Player.MessageType.Status2, "The next map has been chosen - " + c.red + next.ToLower());
            Player.GlobalMessageLevel(Level.Find(currentLevelName), Player.MessageType.Status3, "Please wait while you are transfered.");
            String oldLevel = Server.mainLevel.name;
            if (changeMainLevel)
            {
                Server.mainLevel = Level.Find(next.ToLower());
            }
            {
                Player.players.ForEach(delegate(Player player)
                {
                    if (player.level.name != next && player.level.name == currentLevelName)
                    {
                        player.SendMessage("Going to the next map!");
                        Command.all.Find("goto").Use(player, next);
                        while (player.Loading) { Thread.Sleep(890); }
                    }
                });
                Command.all.Find("unload").Use(null, oldLevel);
            }
            Player.GlobalMessageLevel(Level.Find(currentLevelName), Player.MessageType.Status2, "&f");
            Player.GlobalMessageLevel(Level.Find(currentLevelName), Player.MessageType.Status3, "&f");
            return;
        }

        public void ChangeTime(object sender, ElapsedEventArgs e)
        {
            amountOfMilliseconds = amountOfMilliseconds - 10;
        }

        public bool IsInZombieGameLevel(Player p)
        {
            return p.level.name == currentLevelName;
        }
        static void CheckXP()
        {
            Player.players.ForEach(delegate(Player pl)
            {
                try
                {
                    EXPLevel nextLevel = EXPLevel.Find(pl.explevel.levelID + 1);
                    if (nextLevel != null && pl.points >= nextLevel.requiredEXP)
                    {
                        pl.explevel = nextLevel;
                        pl.money += nextLevel.reward;
                        Player.GlobalMessageLevel(Level.Find(Server.zombie.currentLevelName), Player.MessageType.Chat, pl.color + pl.name + Server.DefaultColor + " has leveled up to level &a" + nextLevel.levelID + "!");
                        pl.SendMessage(Player.MessageType.Announcement, "You have just leveled up to level &a" + nextLevel.levelID + "!", true);
                        pl.SendMessage("&6You were rewarded &a" + nextLevel.reward + " " + Server.moneys + ".", true);
                        Thread.Sleep(3000);
                        pl.SendMessage(Player.MessageType.Announcement, "", true);
                    }
                }
                catch
                {
                    // user probably only just connected- so explevel wont have been set yet.
                }
            });
        }

    }
}

