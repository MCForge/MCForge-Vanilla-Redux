using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
//From the Overv team, ty HeroCane

namespace MCForge
{
    public class CTF
    {
        public static Level currLevel; // The map which is currently being played on.
        public static CTFTeam redTeam; // Red Team initializer.
        public static CTFTeam blueTeam; // Blue Team initializer.
        public static bool gameOn; // Whether the game is running or not.
        public static int scoreLimit = 3; // How many flag captures it takes to win a round.

        public static Thread gameThread; // Thread for the game (so it doesn't interrupt the general flow of the server)
        public static Thread flagThread; // Thread for updating the flag positions.
        public static Thread returnThread; // Thread for returning the flags.

        public static string mainLevel = "ctf_bridge"; // The first map which will be used for the very first round before voting kicks in.
        public static int returnTime = 15; // The time the flag can be left unattended before being returned to base.
        public static int voteTime = 20; // The time allowed for players to vote for the next map.
        public static int drownTime = 14; // The time you're allowed to be underwater before drowning.
        public static int mineActivationTime = 3; // How long it takes after placing a mine before the mine can blow people up.
        public static int takeFlagReward = 2; // Reward for taking the Flag.
        public static int captureFlagReward = 20; // Reward for capturing the Flag.
        public static int returnFlagReward = 4; // Reward for returning the flag.
        public static int killPlayerReward = 1; // Reward for killing a player.
        public static int mineBlastRadius = 2; // Mine activation radius.
        public static int tntBlastRadius = 3; // TNT blast radius.
        public static bool mineDestroyBlocks = false; // Do mines destroy blocks?
        public static bool tntDestroyBlocks = false; // Does TNT destroy blocks?
        public static bool allowOpHax = false; // Should ops be allowed hacks?

        static int redReturnCount;
        static int blueReturnCount;

        public static int redWins = 0;
        public static int blueWins = 0;

        public static void Setup(Level level, bool resetTeams = false)
        {
            if (resetTeams)
            {
                CTFTeam tempRed = redTeam;
                CTFTeam tempBlue = blueTeam;
                redTeam = new CTFTeam("&c", Block.red);
                blueTeam = new CTFTeam("&9", Block.deepblue);
                tempRed.Replace(redTeam);
                tempBlue.Replace(blueTeam);
            }

            currLevel = level;
            redTeam.spawn = currLevel.redSpawn;
            redTeam.spawnrot = currLevel.redRotation;
            redTeam.flagBase = currLevel.redFlag;
            blueTeam.spawn = currLevel.blueSpawn;
            blueTeam.spawnrot = currLevel.blueRotation;
            blueTeam.flagBase = currLevel.blueFlag;
            redTeam.flagLocation = redTeam.flagBase;
            blueTeam.flagLocation = blueTeam.flagBase;

            Server.s.Log("CTF set up, waiting for players...");

            GameStart();
        }

        public static void GameStart()
        {
            gameThread = new Thread(new ThreadStart(delegate
            {
                Player.GlobalMessage("&f-&S The game has started! Defend your flags!");

                while ((redTeam.players.Count + blueTeam.players.Count) < 1)
                {
                    Thread.Sleep(500);
                }

                redReturnCount = 0;
                blueReturnCount = 0;
                redTeam.SpawnPlayers();
                blueTeam.SpawnPlayers();

                Player.players.ForEach(delegate(Player p)
                {
                    p.captureStreak = 0;
                    p.captureCount = 0;
                    p.kills = 0;
                });

                UpdateScore();
                Server.s.Log("Game was started.");
                gameOn = true;

                returnThread = new Thread(new ThreadStart(delegate
                {
                    while (gameOn)
                    {
                        if (redTeam.hasFlag == null && !redTeam.flagIsHome)
                        {
                            redReturnCount++;
                            if (redReturnCount >= returnTime)
                            {
                                ReturnFlag(redTeam);
                                redReturnCount = 0;
                            }
                        }

                        if (blueTeam.hasFlag == null && !blueTeam.flagIsHome)
                        {
                            blueReturnCount++;
                            if (blueReturnCount >= returnTime)
                            {
                                ReturnFlag(blueTeam);
                                blueReturnCount = 0;
                            }
                        }

                        Thread.Sleep(1000);
                    }
                }));
                returnThread.Start();

                flagThread = new Thread(new ThreadStart(delegate
                {
                    while (gameOn)
                    {
                        redTeam.DrawFlag();
                        blueTeam.DrawFlag();
                        Thread.Sleep(200);
                    }
                }));
                flagThread.Start();
            }));
            gameThread.Start();
        }

        public static void GameEnd(CTFTeam winners)
        {
            gameOn = false;
            UpdateScore(winners);
            Player.GlobalMessage("&f-&S The game has ended. Winners are the " + winners.color + winners.name + " team!");
            Server.s.Log("Game was ended. Winning team: " + winners.name);

            Thread.Sleep(5000);

            Player.GlobalMessage("&f- &SThis games top performers:");
            List<Player> ordered = Player.players.OrderByDescending(ply => ply.captureCount).ThenBy(ply => ply.captureStreak).ToList();
            ordered.ForEach(delegate(Player p)
            {
                Player.GlobalMessage("     " + p.color + p.name + " &a(" + p.captureCount + " captures, " + p.kills + " kills, " + p.deaths + " deaths)");

            });

            winners.points = 0;

            if (winners == redTeam) { redWins++; }
            if (winners == blueTeam) { blueWins++; }

            ReturnFlag(redTeam, false);
            ReturnFlag(blueTeam, false);

            currLevel.blocks = currLevel.backupBlocks;

            Thread.Sleep(5000);

            VoteMap();
        }

        static int optionCount;
        static int voted1 = 0;
        static int voted2 = 0;
        static int voted3 = 0;
        static string option1 = "N/A";
        static string option2 = "N/A";
        static string option3 = "N/A";

        public static void VoteMap()
        {
            Random rnd = new Random();
            optionCount = 0;
            List<string> levels = new List<string>();
            Level newLevel = currLevel;

            voted1 = 0;
            voted2 = 0;
            voted3 = 0;

            DirectoryInfo di = new DirectoryInfo("levels/");
            FileInfo[] fi = di.GetFiles("ctf_*.mcf");

            foreach (FileInfo level in fi)
            {
                if (!level.Name.Contains("backup"))
                {
                    levels.Add(level.Name.Replace(".mcf", ""));
                }
            }

            switch (levels.Count)
            {
                case 1:
                    option1 = levels[0].Replace("ctf_", "");
                    optionCount = 1;
                    break;
                case 2:
                    option1 = levels[0].Replace("ctf_", "");
                    option2 = levels[1].Replace("ctf_", "");
                    optionCount = 2;
                    break;
                case 3:
                    option1 = levels[0].Replace("ctf_", "");
                    option2 = levels[1].Replace("ctf_", "");
                    option3 = levels[2].Replace("ctf_", "");
                    optionCount = 3;
                    break;
                default:
                    option1 = levels[rnd.Next(0, levels.Count - 1)].Replace("ctf_", "");
                    levels.Remove(option1);
                    option2 = levels[rnd.Next(0, levels.Count - 1)].Replace("ctf_", "");
                    levels.Remove(option2);
                    option3 = levels[rnd.Next(0, levels.Count - 1)].Replace("ctf_", "");
                    optionCount = 3;
                    break;
            }

            if (optionCount == 1)
            {
                currLevel.blocks = currLevel.backupBlocks;
                Player.players.ForEach(delegate(Player p)
                {
                    Command.all.Find("goto").Use(p, currLevel.name);
                    Thread.Sleep(500);
                });
                Setup(currLevel);
                return;
            }

            Player.players.ForEach(delegate(Player p)
            {
                p.SendMessage("&f- &SVote for the next map:");
                if (optionCount == 2)
                {
                    p.SendMessage("     1 - &b" + option1 + "&S &f|&S 2 - &b" + option2);
                    p.SendMessage("&f- &SType &a1&S or &a2&S to vote.");
                }
                else
                {
                    p.SendMessage("     1 - &b" + option1 + "&S &f|&S 2 - &b" + option2 + "&S &f|&S 3 - &b" + option3);
                    p.SendMessage("&f- &SType &a1&S, &a2&S or &a3&S to vote.");
                }
                p.OnChat += ProcessVote;
            });

            Thread.Sleep(voteTime * 1000);

            CTFTeam.teams.ForEach(delegate(CTFTeam team)
            {
                team.players.ForEach(delegate(Player p)
                {
                    p.ClearChat();
                });
            });

            if (optionCount == 2)
            {
                if (voted1 > voted2)
                {
                    if (Level.Find("ctf_" + option1) == null)
                    {
                        newLevel = Level.Load("ctf_" + option1);
                    }
                    else
                    {
                        newLevel = Level.Find("ctf_" + option1);
                    }
                }
                else if (voted2 > voted1)
                {
                    if (Level.Find("ctf_" + option2) == null)
                    {
                        newLevel = Level.Load("ctf_" + option2);
                    }
                    else
                    {
                        newLevel = Level.Find("ctf_" + option2);
                    }
                }
            }
            else if (optionCount == 3)
            {
                if (voted1 > voted2 && voted1 > voted3)
                {
                    if (Level.Find("ctf_" + option1) == null)
                    {
                        newLevel = Level.Load("ctf_" + option1);
                    }
                    else
                    {
                        newLevel = Level.Find("ctf_" + option1);
                    }
                }
                else if (voted2 > voted1 && voted2 > voted3)
                {
                    if (Level.Find("ctf_" + option2) == null)
                    {
                        newLevel = Level.Load("ctf_" + option2);
                    }
                    else
                    {
                        newLevel = Level.Find("ctf_" + option2);
                    }
                }
                else if (voted3 > voted1 && voted3 > voted2)
                {
                    if (Level.Find("ctf_" + option3) == null)
                    {
                        newLevel = Level.Load("ctf_" + option3);
                    }
                    else
                    {
                        newLevel = Level.Find("ctf_" + option3);
                    }
                }
            }

            if (newLevel != null)
            {
                Player.GlobalMessage("&f- &2Next map selected: &b" + newLevel.name + ".");
                Server.s.Log("Next map is: " + newLevel.name + ".");
                Thread.Sleep(7500);
                currLevel.blocks = currLevel.backupBlocks;
                newLevel.blocks = newLevel.backupBlocks;
                Player.players.ForEach(delegate(Player p)
                {
                    Command.all.Find("goto").Use(p, newLevel.name);
                    Thread.Sleep(500);
                });
                Setup(newLevel, true);
            }
            else
            {
                Player.GlobalMessage("Failed to load next level! Replaying on current level.");
                Setup(currLevel, true);
            }
        }

        static void ProcessVote(Player p, string message)
        {
            switch (message)
            {
                case "1":
                    voted1++;
                    p.ClearChat();
                    p.SendMessage("&f- &SYou voted for &b" + option1 + ".");
                    Server.s.Log(p.name + " voted for " + option1 + ".");
                    break;
                case "2":
                    voted2++;
                    p.ClearChat();
                    p.SendMessage("&f- &SYou voted for &b" + option2 + ".");
                    Server.s.Log(p.name + " voted for " + option2 + ".");
                    break;
                case "3":
                    if (optionCount > 2)
                    {
                        voted3++;
                        p.ClearChat();
                        p.SendMessage("&f- &SYou voted for &b" + option3 + ".");
                        Server.s.Log(p.name + " voted for " + option3 + ".");
                    }
                    else
                    {
                        goto default;
                    }
                    break;
                default:
                    if (optionCount == 2)
                    {
                        p.SendMessage("&f- &SInvalid option! Type either 1 or 2.");
                    }
                    else
                    {
                        p.SendMessage("&f- &SInvalid option! Type either 1, 2 or 3.");
                    }
                    break;
            }
        }


        public static void ExplodeTNT(Player p, ushort x, ushort y, ushort z, int radius)
        {
            Player.players.ForEach(delegate(Player pl)
            {
                if (pl == p || !p.placedTNT.isActive || !CTF.gameOn || pl.team == p.team)
                {
                    return;
                }

                currLevel.Blockchange(x, y, z, Block.air);
                p.placedTNT.isActive = false;

                ushort px = (ushort)(pl.pos[0] / 32);
                ushort py = (ushort)(pl.pos[1] / 32);
                ushort pz = (ushort)(pl.pos[2] / 32);

                if ((Math.Max(px, x) - Math.Min(px, x)) <= radius)
                {
                    if ((Math.Max(py, y) - Math.Min(py, y)) <= radius)
                    {
                        if ((Math.Max(pz, z) - Math.Min(pz, z)) <= radius)
                        {
                            Player.GlobalMessage("&f- " + pl.color + pl.name + "&S was exploded by " + p.color + p.name + "&S's TNT!");
                            p.killPlayer(pl);
                            Server.s.Log(p.name + " exploded " + pl.name + "!");
                        }
                    }
                }

                ushort minX = (ushort)(x - radius);
                ushort minY = (ushort)(y - radius);
                ushort minZ = (ushort)(z - radius);
                ushort maxX = (ushort)(x + radius);
                ushort maxY = (ushort)(y + radius);
                ushort maxZ = (ushort)(z + radius);

                for (ushort xx = minX; xx <= maxX; xx++)
                {
                    for (ushort yy = minY; yy <= maxY; yy++)
                    {
                        for (ushort zz = minZ; zz <= maxZ; zz++)
                        {
                            if (currLevel.GetTile(xx, yy, zz) != Block.blackrock && tntDestroyBlocks)
                            {
                                currLevel.Blockchange(xx, yy, zz, Block.air);
                                Player.GlobalBlockchange(currLevel, xx, yy, zz, Block.air);
                            }
                        }
                    }
                }
            });
        }

        public static void TakeFlag(Player p, CTFTeam team)
        {
            if (!gameOn) { return; }
            if (p.justDroppedFlag) { return; }
            if (p.carryingFlag) { return; }
            if (redTeam.players.Count < 1 || blueTeam.players.Count < 1)
            {
                    p.SendMessage("&f- &SYou cannot take the flag with no opposition!");
                return;
            }
            team.hasFlag = p;
            team.flagIsHome = false;
            p.carryingFlag = true;
            Player.GlobalMessage("&f- " + p.color + p.name + "&S took the " + team.color + team.name + "&S flag!");
            Server.s.Log(p.name + " took the " + team.name + " flag!");
            p.Reward(takeFlagReward);
        }

        public static void CaptureFlag(Player p, CTFTeam team)
        {
            if (!gameOn) { return; }
            team.hasFlag = null;
            ReturnFlag(team, false);
            p.carryingFlag = false;
            Player.GlobalMessage("&f- " + p.color + p.name + "&S captured the " + team.color + team.name + "&S flag!");
            Server.s.Log(p.name + " captured the " + team.name + " flag!");
            p.Reward(captureFlagReward);
            p.team.points++;
            UpdateScore();
            p.captureStreak++;
            p.captureCount++;
            if (team.capturedFlag == p)
            {
                Player.GlobalMessage("&f- " + p.color + p.name + "&6 is on a streak of &5" + p.captureStreak + "&6!");
            }
            else
            {
                p.captureStreak = 1;
            }
            team.capturedFlag = p;
            if (p.team.points >= scoreLimit)
            {
                GameEnd(p.team);
            }
        }

        public static void ReturnFlag(CTFTeam team, bool message = true)
        {
            team.flagLocation = team.flagBase;
            team.flagIsHome = true;
            team.DrawFlag();
            if (message)
            {
                Player.GlobalMessage("&f-&S The " + team.color + team.name + "&S team's flag was returned.");
                Server.s.Log(team.name + " flag was returned to base.");
            }
        }

        public static void UpdateScore(CTFTeam winners = null)
        {
            if (winners != null)
            {
                Player.GlobalMessage("^detail.user=Game over. Winners: " + winners.color + winners.name + " team!");
            }
            else
            {
                Player.GlobalMessage("^detail.user= &cRed: " + redTeam.points + "&f | &9Blue: " + blueTeam.points);
            }
        }
    }
}
