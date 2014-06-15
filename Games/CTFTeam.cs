using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//Once again from the Overv team, ty HeroCane
namespace MCForge
{
    public class CTFTeam
    {
        public static List<CTFTeam> teams = new List<CTFTeam>();
        public string name;
        public string color;
        public ushort flagBlock;
        public int points;
        public List<Player> players;
        public ushort[] flagBase;
        public ushort[] flagLocation;
        public ushort[] spawn;
        public ushort[] spawnrot;
        public FlagPos oldPos;
        public Player hasFlag;
        public Player capturedFlag;
        public bool flagIsHome;

        public CTFTeam(string newColor, ushort newFlagBlock, bool add = true)
        {
            name = c.Name(newColor);
            name = name[0].ToString().ToUpper() + name.Remove(0, 1);
            color = newColor;
            points = 0;
            players = new List<Player>();
            hasFlag = null;
            flagIsHome = true;
            flagBlock = newFlagBlock;
            if (add)
            {
                teams.Add(this);
            }
        }

        public void AddPlayer(Player p, bool announce = true)
        {
            bool hasSwitched = false;
            if (p.team != null)
            {
                hasSwitched = true;
            }

            p.team = this;
            p.color = color;
            Player.GlobalDie(p, false);
            Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
            players.Add(p);
            p.placedTNT.isActive = false;
            p.placedMine.isActive = false;

            if (CTF.gameOn)
            {
                SpawnPlayer(p);
            }

            if (announce)
            {
                if (!hasSwitched)
                {
                    Player.GlobalMessage("&f- " + p.color + p.name + "&S joined the " + color + name + "&S team.");
                    Server.s.Log(p.name + " joined the " + name + " team!");
                }
                else
                {
                    Player.GlobalMessage("&f- " + p.color + p.name + "&S switched to the " + color + name + "&S team.");
                    Server.s.Log(p.name + " switched to the " + name + " team!");
                }
            }
        }

        public void DelPlayer(Player p)
        {
            p.color = p.group.color;
            players.Remove(p);
            if (p.carryingFlag)
            {
                Command.all.Find("drop").Use(p, "");
            }
        }

        public void SpawnPlayers()
        {
            players.ForEach(delegate(Player p)
            {
                unchecked
                {
                    ushort x = spawn[0];
                    ushort y = spawn[1];
                    ushort z = spawn[2];
                    x *= 32; x += 16;
                    y *= 32; y += 0;
                    z *= 32; z += 16;
                    p.SendSpawn((byte)-1, p.color + p.name, x, y, z, (byte)spawnrot[0], (byte)spawnrot[1]);
                }
            });
        }

        public void SpawnPlayer(Player p)
        {
            unchecked
            {
                ushort x = spawn[0];
                ushort y = spawn[1];
                ushort z = spawn[2];
                x *= 32; x += 16;
                y *= 32; y += 0;
                z *= 32; z += 16;
                p.SendSpawn((byte)-1, p.color + p.name, x, y, z, (byte)spawnrot[0], (byte)spawnrot[1]);
            }
        }

        public void Replace(CTFTeam replaceMent)
        {
            teams.Remove(this);
            players.ForEach(delegate(Player p)
            {
                p.team = replaceMent;
                DelPlayer(p);
                replaceMent.AddPlayer(p, false);
            });
        }

        public void DrawFlag()
        {
            ushort x, y, z;
            x = flagLocation[0];
            y = flagLocation[1];
            z = flagLocation[2];

            try
            {
                if (oldPos.playerWasHolding)
                {
                    CTF.currLevel.Blockchange(oldPos.x, oldPos.y, oldPos.z, oldPos.type);
                }
                else
                {
                    CTF.currLevel.Blockchange(oldPos.x, oldPos.y, oldPos.z, Block.air);
                    CTF.currLevel.Blockchange(oldPos.x, (ushort)(oldPos.y - 1), oldPos.z, Block.air);
                    CTF.currLevel.Blockchange(oldPos.x, (ushort)(oldPos.y - 2), oldPos.z, Block.air);
                }
            }
            catch { }

            if (hasFlag == null)
            {
                ushort below = CTF.currLevel.GetTile(x, (ushort)(y - 1), z);
                if (below == Block.air)
                {
                    y = (ushort)(y - 1);
                }

                CTF.currLevel.Blockchange(x, y, z, Block.flagbase);
                CTF.currLevel.Blockchange(x, (ushort)(y + 1), z, Block.rope);
                CTF.currLevel.Blockchange(x, (ushort)(y + 2), z, flagBlock);

                ushort b = CTF.currLevel.GetTile(x, (ushort)(y + 2), z);
                oldPos.x = x; oldPos.y = (ushort)(y + 2); oldPos.z = z; oldPos.type = b; oldPos.playerWasHolding = false;
            }
            else
            {
                x = (ushort)(hasFlag.pos[0] / 32);
                y = (ushort)(hasFlag.pos[1] / 32);
                z = (ushort)(hasFlag.pos[2] / 32);

                ushort b = CTF.currLevel.GetTile(x, (ushort)(y + 2), z);
                oldPos.x = x; oldPos.y = (ushort)(y + 2); oldPos.z = z; oldPos.type = b; oldPos.playerWasHolding = true;

                CTF.currLevel.Blockchange(x, (ushort)(y + 2), z, flagBlock);
            }

            flagLocation = new ushort[3] { x, y, z };
        }

        public struct FlagPos
        {
            public ushort x, y, z;
            public ushort type;
            public bool playerWasHolding;
        }
    }
}
