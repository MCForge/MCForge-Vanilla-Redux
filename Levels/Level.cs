/*
Copyright (C) 2010-2013 David Mitchell

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using MCForge.Levels.Textures;
using MCForge.SQL;
using Timer = System.Timers.Timer;
//WARNING! DO NOT CHANGE THE WAY THE LEVEL IS SAVED/LOADED!
//You MUST make it able to save and load as a new version other wise you will make old levels incompatible!

namespace MCForge
{
    public enum LevelPermission //int is default
    {
        Banned = -20,
        Guest = 0,
        Builder = 30,
        AdvBuilder = 50,
        Operator = 80,
        Admin = 100,
        Nobody = 120,
        Null = 150
    }
    public enum PhysicsState
    {
        Stopped,
        Warning,
        Other
    }

    public enum MapType
    {
        General,
        Game,
    }

    public sealed class Level : IDisposable
    {
        #region Delegates

        public delegate void OnLevelLoad(string level);

        public delegate void OnLevelLoaded(Level l);

        public delegate void OnLevelSave(Level l);

        public delegate void OnLevelUnload(Level l);

        public delegate void OnPhysicsUpdate(ushort x, ushort y, ushort z, byte time, string extraInfo, Level l);

        public delegate void OnPhysicsStateChanged(object sender, PhysicsState state);

        #endregion
        public int speedphysics = 250;
        public bool Death;
        public bool GrassDestroy = true;
        public bool GrassGrow = true;
        public bool Instant;
        public bool Killer = true;

        public static bool cancelload;
        public static bool cancelsave;
        public readonly List<Check> ListCheck = new List<Check>(); //A list of blocks that need to be updated
        public readonly List<Update> ListUpdate = new List<Update>(); //A list of block to change after calculation

        public List<UndoPos> UndoBuffer = new List<UndoPos>();
        public List<Zone> ZoneList;
        public bool ai = true;
        public bool backedup;
        public List<BlockPos> blockCache = new List<BlockPos>();
        public ushort[] blocks;
        public Physics physic = new Physics();
        public bool cancelsave1;
        public bool cancelunload;
        public bool changed;
        public bool physicschanged
        {
            get { return ListCheck.Count > 0; }
        }
        public bool countdowninprogress;
        public bool ctfmode;
        public int currentUndo;
        public ushort depth; // y       THIS IS STUPID, SHOULD HAVE BEEN Z
        public int drown = 70;
        public bool edgeWater;
        public int fall = 9;
        public bool finite;
        public bool fishstill;
        public bool growTrees;
        public bool guns = true;
        public ushort height; // z      THIS IS STUPID, SHOULD HAVE BEEN Y
        public int id;
        public byte jailrotx, jailroty;

        public ushort jailx, jaily, jailz;
        public int lastCheck;
        public int lastUpdate;
        public bool leafDecay;
        public bool loadOnGoto = true;
        public string motd = "ignore";
        public string name;
        public int overload = 1500;
		public byte weather;
        public ushort[] redSpawn = { 0, 0, 0 };
        public ushort[] redRotation = { 0, 0 };
        public ushort[] blueSpawn = { 0, 0, 0 };
        public ushort[] blueRotation = { 0, 0 };
        public ushort[] redFlag = { 0, 0, 0 };
        public ushort[] blueFlag = { 0, 0, 0 };

        public string author = "nobody";
        public int likes = 0;
        public int dislikes = 0;

        public int maxBuildHeight;
        public ushort divider;

        // IsoCat
        public ushort[,] shadows;
        public void CalculateShadows()
        {
            try
            {
                if (shadows != null) return;

                shadows = new ushort[width, height];
                for (ushort x = 0; x < width; x++)
                {
                    for (ushort y = 0; y < height; y++)
                    {
                        for (ushort z = (ushort)(depth - 1); z >= 0; z--)
                        {
                            switch (GetTile(x, y, z))
                            {
                                case Block.air:
                                case Block.mushroom:
                                case Block.glass:
                                case Block.leaf:
                                case Block.redflower:
                                case Block.redmushroom:
                                case Block.yellowflower:
                                    continue;
                                default:
                                    shadows[x, y] = z;
                                    break;
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception ex) { Server.ErrorLog(ex); shadows = new ushort[width, height]; }
        }
        public LevelPermission perbuildmax = LevelPermission.Nobody;

        public LevelPermission permissionbuild = LevelPermission.Builder;
        // What ranks can go to this map (excludes banned)

        public LevelPermission permissionvisit = LevelPermission.Guest;
        public LevelPermission pervisitmax = LevelPermission.Nobody;

        //public Timer physChecker = new Timer(1000);
        public int physics
        {
            get { return Physicsint; }
            set
            {
                if (value > 0 && Physicsint == 0)
                    physic.StartPhysics(this);
                Physicsint = value;
            }
        }
        int Physicsint;
        public bool randomFlow = true;
        public bool realistic = true;
        public byte rotx;
        public byte roty;
        public bool rp = true;
        public ushort spawnx;
        public ushort spawny;
        public ushort spawnz;
        public ushort[] backupBlocks;

        public LevelTextures textures;

        public string theme = "Normal";
        public bool unload = true;
        public ushort width; // x
        public bool worldChat = true;
        public bool bufferblocks = Server.bufferblocks;
        public List<BlockQueue.block> blockqueue = new List<BlockQueue.block>();

        public List<C4.C4s> C4list = new List<C4.C4s>();

        public Level(string n, ushort x, ushort y, ushort z, string type, int seed = 0, bool useSeed = false, MapType mt = MapType.General)
        {
            //onLevelSave += null;
            width = x;
            depth = y;
            height = z;
            if (width < 16)
            {
                width = 16;
            }
            if (depth < 16)
            {
                depth = 16;
            }
            if (height < 16)
            {
                height = 16;
            }

            name = n;
            blocks = new ushort[width * depth * height];
            ZoneList = new List<Zone>();

            var half = (ushort)(depth / 2);
            switch (type)
            {
                case "flat":
                    for (x = 0; x < width; ++x)
                        for (z = 0; z < height; ++z)
                            for (y = 0; y <= half; ++y)
                                SetTile(x, y, z, y < half ? Block.dirt : Block.grass);
                    //SetTile(x, y, z, (byte)(y != half ? (y >= half) ? 0 : 3 : 2));
                    break;
                case "pixel":
                    for (x = 0; x < width; ++x)
                        for (z = 0; z < height; ++z)
                            for (y = 0; y < depth; ++y)
                                if (y == 0)
                                    SetTile(x, y, z, 7);
                                else if (x == 0 || x == width - 1 || z == 0 || z == height - 1)
                                    SetTile(x, y, z, 36);
                    break;

                case "space":
                    Random rand = useSeed ? new Random(seed) : new Random();

                    for (x = 0; x < width; ++x)
                        for (z = 0; z < height; ++z)
                            for (y = 0; y < depth; ++y)
                                if (y == 0)
                                    SetTile(x, y, z, 7);
                                else if (x == 0 || x == width - 1 || z == 0 || z == height - 1 || y == 1 ||
                                         y == depth - 1)
                                    SetTile(x, y, z, rand.Next(100) == 0 ? Block.iron : Block.obsidian);
                    break;

                case "rainbow":
                    Random random = useSeed ? new Random(seed) : new Random();
                    for (x = 0; x < width; ++x)
                        for (z = 0; z < height; ++z)
                            for (y = 0; y < depth; ++y)
                                if (y == 0 || y == depth - 1 || x == 0 || x == width - 1 || z == 0 || z == height - 1)
                                    SetTile(x, y, z, (byte)random.Next(21, 36));

                    break;


                case "hell":
                    Random random2 = useSeed ? new Random(seed) : new Random();
                    for (x = 0; x < width; ++x)
                        for (z = 0; z < height; ++z)
                            for (y = 0; y < depth; ++y)
                                if (y == 0)
                                    SetTile(x, y, z, 7);
                                else if (x == 0 || x == width - 1 || z == 0 || z == height - 1 || y == 0 ||
                                         y == depth - 1)
                                    SetTile(x, y, z, Block.obsidian);
                                else if (x == 1 || x == width - 2 || z == 1 || z == height - 2)
                                {
                                    if (random2.Next(1000) == 7)
                                    {
                                        for (int i = 1; i < (depth - y); ++i)
                                        {
                                            SetTile(x, (ushort)(depth - i), z, Block.lava);
                                        }
                                    }
                                }

                    Server.MapGen.GenerateMap(this, type, seed, useSeed);
                    break;
                case "island":
                case "mountains":
                case "ocean":
                case "forest":
                case "desert":
                    Server.MapGen.GenerateMap(this, type, seed, useSeed);
                    break;

                //no need for default
            }
            spawnx = (ushort)(width / 2);
            spawny = (ushort)(depth * 0.75f);
            spawnz = (ushort)(height / 2);
            rotx = 0;
            roty = 0;
            redFlag = new ushort[3] { 0, (ushort)(depth / 2), 0 };
            blueFlag = new ushort[3] { (ushort)(width - 1), (ushort)(depth / 2), (ushort)(height - 1) };
            textures = new LevelTextures(this);
            //season = new SeasonsCore(this);
        }

        public ushort length
        {
            get { return height; }
        }

        public List<Player> players
        {
            get { return getPlayers(); }
        }

        public static event OnLevelUnload LevelUnload = null;
        public static event OnLevelSave LevelSave = null;
        //public static event OnLevelSave onLevelSave = null;
  //      public event OnLevelUnload onLevelUnload = null;
        public static event OnLevelLoad LevelLoad = null;
        public static event OnLevelLoaded LevelLoaded;

        public void CopyBlocks(byte[] source, int offset)
        {
            blocks = new ushort[width * depth * height];
            Array.Copy(source, offset, blocks, 0, blocks.Length);

            for (int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i] >= 50) blocks[i] = 0;
                switch (blocks[i])
                {
                    case Block.waterstill:
                        blocks[i] = Block.water;
                        break;
                    case Block.water:
                        blocks[i] = Block.waterstill;
                        break;
                    case Block.lava:
                        blocks[i] = Block.lavastill;
                        break;
                    case Block.lavastill:
                        blocks[i] = Block.lava;
                        break;
                }
            }
        }

        public bool Unload(bool silent = false, bool save = true)
        {
            if (Server.mainLevel == this) return false;
            if (name.Contains("&cMuseum ")) return false;
            if (Server.lava.active && Server.lava.map == this) return false;
            if (LevelUnload != null)
                LevelUnload(this);
            OnLevelUnloadEvent.Call(this);
            if (cancelunload)
            {
                Server.s.Log("Unload canceled by Plugin! (Map: " + name + ")");
                cancelunload = false;
                return false;
            }
            Player.players.ForEach(
                delegate(Player pl) { if (pl.level == this) Command.all.Find("goto").Use(pl, Server.mainLevel.name); });

            if (changed && (!Server.ZombieModeOn || !Server.noLevelSaving) && mapType != MapType.Game)
            {
                    if ((!Server.lava.active || !Server.lava.HasMap(name)) && save) Save(false, true);
                    saveChanges();
            }
            if (TntWarsGame.Find(this) != null)
            {
                foreach (TntWarsGame.player pl in TntWarsGame.Find(this).Players)
                {
                    pl.p.CurrentTntGameNumber = -1;
                    Player.SendMessage(pl.p, "TNT Wars: The TNT Wars game you are currently playing has been deleted!");
                    pl.p.PlayingTntWars = false;
                    pl.p.canBuild = true;
                    TntWarsGame.SetTitlesAndColor(pl, true);
                }
                Server.s.Log("TNT Wars: Game deleted on " + name);
                TntWarsGame.GameList.Remove(TntWarsGame.Find(this));

            }

            Server.levels.Remove(this);

            try
            {
                //physChecker.Stop();
                //physChecker.Dispose();
                physic.physThread.Abort();
                physic.physThread.Join();

            }
            catch
            {
            }

            finally
            {
                Dispose();
                GC.Collect();
                GC.WaitForPendingFinalizers();

                if (!silent) Player.GlobalMessageOps("&3" + name + Server.DefaultColor + " was unloaded.");
                Server.s.Log(string.Format("{0} was unloaded.", name));
            }
            return true;
        }

        public void Dispose()
        {
            physic.Extras.Clear();
            physic.liquids.Clear();
            physic.leaves.Clear();
            ListCheck.Clear();
            ListUpdate.Clear();
            UndoBuffer.Clear();
            blockCache.Clear();
            ZoneList.Clear();
            blockqueue.Clear();
            blocks = null;
        }

        public void saveChanges()
        {
            //if (!Server.useMySQL) return;
            if (blockCache.Count == 0) return;
            List<BlockPos> tempCache = blockCache;
            blockCache = new List<BlockPos>();

            string template = "INSERT INTO `Block" + name +
                              "` (Username, TimePerformed, X, Y, Z, type, deleted) VALUES ('{0}', '{1}', {2}, {3}, {4}, {5}, {6})";
            DatabaseTransactionHelper transaction = DatabaseTransactionHelper.Create();
            using (transaction)
            {
                foreach (BlockPos bP in tempCache)
                {
                    int deleted = bP.deleted ? 1 : 0;
                    transaction.Execute(String.Format(template, bP.name,
                                                      bP.TimePerformed.ToString("yyyy-MM-dd HH:mm:ss"), (int)bP.x,
                                                      (int)bP.y, (int)bP.z, bP.type, deleted));
                }
                transaction.Commit();
            }
            tempCache.Clear();
        }

        public ushort GetTile(ushort x, ushort y, ushort z)
        {
            if (blocks == null) return Block.Zero;
            //if (PosToInt(x, y, z) >= blocks.Length) { return null; }
            //Avoid internal overflow
            return !InBound(x, y, z) ? Block.Zero : blocks[PosToInt(x, y, z)];
        }

        public ushort GetTile(int b)
        {
            ushort x = 0, y = 0, z = 0;
            IntToPos(b, out x, out y, out z);
            return GetTile(x, y, z);
        }
        public void SetTile(int b, ushort type)
        {
            if (blocks == null) return;
            if (b >= blocks.Length) return;
            if (b < 0) return;
            blocks[b] = (ushort)type;
            //blockchanges[x + width * z + width * height * y] = pName;
        }
        public void SetTile(ushort x, ushort y, ushort z, ushort type)
        {
            if (blocks == null) return;
            if (!InBound(x, y, z)) return;
            blocks[PosToInt(x, y, z)] = (ushort)type;
            //blockchanges[x + width * z + width * height * y] = pName;
        }

        public bool InBound(ushort x, ushort y, ushort z)
        {
            return x >= 0 && y >= 0 && z >= 0 && x < width && y < depth && z < height;
        }

        public static Level Find(string levelName)
        {
            Level tempLevel = null;
            bool returnNull = false;

            foreach (Level level in Server.levels)
            {
                if (level.name.ToLower() == levelName) return level;
                if (level.name.ToLower().IndexOf(levelName.ToLower(), System.StringComparison.Ordinal) == -1) continue;
                if (tempLevel == null) tempLevel = level;
                else returnNull = true;
            }

            return returnNull ? null : tempLevel;
        }

        public static Level FindExact(string levelName)
        {
            return Server.levels.Find(lvl => levelName.ToLower() == lvl.name.ToLower());
        }


        public void Blockchange(Player p, ushort x, ushort y, ushort z, ushort type, bool addaction = true)
        {
            string errorLocation = "start";
        retry:
            try
            {
                if (x < 0 || y < 0 || z < 0) return;
                if (x >= width || y >= depth || z >= height) return;

                ushort b = GetTile(x, y, z);

                errorLocation = "Block rank checking";
                if (!Block.AllowBreak(b))
                {
                    if (!Block.canPlace(p, b) && !Block.BuildIn(b))
                    {
                        p.SendBlockchange(x, y, z, b);
                        return;
                    }
                }
                errorLocation = "Allowed to place tnt there (TNT Wars)";
                if (type == Block.tnt || type == Block.smalltnt || type == Block.bigtnt || type == Block.nuketnt)
                {
                    if (p.PlayingTntWars)
                    {
                        if (TntWarsGame.GetTntWarsGame(p).InZone(x, y, z, true))
                        {
                            p.SendBlockchange(x, y, z, b);
                            return;
                        }
                    }
                }
                errorLocation = "Max tnt for TNT Wars checking";
                if (type == Block.tnt || type == Block.smalltnt || type == Block.bigtnt || type == Block.nuketnt)
                {
                    if (p.PlayingTntWars)
                    {
                        if (p.CurrentAmountOfTnt == TntWarsGame.GetTntWarsGame(p).TntPerPlayerAtATime)
                        {
                            p.SendBlockchange(x, y, z, b);
                            Player.SendMessage(p, "TNT Wars: Maximum amount of TNT placed");
                            return;
                        }
                        if (p.CurrentAmountOfTnt > TntWarsGame.GetTntWarsGame(p).TntPerPlayerAtATime)
                        {
                            p.SendBlockchange(x, y, z, b);
                            Player.SendMessage(p, "TNT Wars: You have passed the maximum amount of TNT that can be placed!");
                            return;
                        }
                        else
                        {
                            p.TntAtATime();
                        }
                    }
                }

                errorLocation = "TNT Wars switch TNT block to smalltnt";
                if ((type == Block.tnt || type == Block.bigtnt || type == Block.nuketnt || type == Block.smalltnt) && p.PlayingTntWars)
                {
                    type = Block.smalltnt;
                }

                errorLocation = "Zone checking";

                #region zones

                bool AllowBuild = true, foundDel = false, inZone = false;
                string Owners = "";
                var toDel = new List<Zone>();
                if ((p.group.Permission < LevelPermission.Admin || p.ZoneCheck || p.zoneDel) && !Block.AllowBreak(b))
                {
                    if (ZoneList.Count == 0) AllowBuild = true;
                    else
                    {
                        for (int index = 0; index < ZoneList.Count; index++)
                        {
                            Zone Zn = ZoneList[index];
                            if (Zn.smallX <= x && x <= Zn.bigX && Zn.smallY <= y && y <= Zn.bigY && Zn.smallZ <= z &&
                                z <= Zn.bigZ)
                            {
                                inZone = true;
                                if (p.zoneDel)
                                {
                                    //DB
                                    Database.executeQuery("DELETE FROM `Zone" + p.level.name + "` WHERE Owner='" +
                                                          Zn.Owner + "' AND SmallX='" + Zn.smallX + "' AND SMALLY='" +
                                                          Zn.smallY + "' AND SMALLZ='" + Zn.smallZ + "' AND BIGX='" +
                                                          Zn.bigX + "' AND BIGY='" + Zn.bigY + "' AND BIGZ='" + Zn.bigZ +
                                                          "'");
                                    toDel.Add(Zn);

                                    p.SendBlockchange(x, y, z, b);
                                    Player.SendMessage(p, "Zone deleted for &b" + Zn.Owner);
                                    foundDel = true;
                                }
                                else
                                {
                                    if (Zn.Owner.Substring(0, 3) == "grp")
                                    {
                                        if (Group.Find(Zn.Owner.Substring(3)).Permission <= p.group.Permission &&
                                            !p.ZoneCheck)
                                        {
                                            AllowBuild = true;
                                            break;
                                        }
                                        AllowBuild = false;
                                        Owners += ", " + Zn.Owner.Substring(3);
                                    }
                                    else
                                    {
                                        if (Zn.Owner.ToLower() == p.name.ToLower() && !p.ZoneCheck)
                                        {
                                            AllowBuild = true;
                                            break;
                                        }
                                        AllowBuild = false;
                                        Owners += ", " + Zn.Owner;
                                    }
                                }
                            }
                        }
                    }

                    if (p.zoneDel)
                    {
                        if (!foundDel) Player.SendMessage(p, "No zones found to delete.");
                        else
                        {
                            foreach (Zone Zn in toDel)
                            {
                                ZoneList.Remove(Zn);
                            }
                        }
                        p.zoneDel = false;
                        return;
                    }

                    if (!AllowBuild || p.ZoneCheck)
                    {
                        if (Owners != "") Player.SendMessage(p, "This zone belongs to &b" + Owners.Remove(0, 2) + ".");
                        else Player.SendMessage(p, "This zone belongs to no one.");

                        p.ZoneSpam = DateTime.Now;
                        p.SendBlockchange(x, y, z, b);

                        if (p.ZoneCheck) if (!p.staticCommands) p.ZoneCheck = false;
                        return;
                    }
                }

                #endregion

                errorLocation = "Map rank checking";
                if (Owners == "")
                {
                    if (p.group.Permission < permissionbuild && (!inZone || !AllowBuild))
                    {
                        p.SendBlockchange(x, y, z, b);
                        Player.SendMessage(p, "Must be at least " + PermissionToName(permissionbuild) + " to build here");
                        return;
                    }
                }

                errorLocation = "Map Max Rank Checking";
                if (Owners == "")
                {
                    if (p.group.Permission > perbuildmax && (!inZone || !AllowBuild))
                    {
                        if (!p.group.CanExecute(Command.all.Find("perbuildmax")))
                        {
                            p.SendBlockchange(x, y, z, b);
                            Player.SendMessage(p, "Your rank must be " + perbuildmax + " or lower to build here!");
                            return;
                        }
                    }
                }

                errorLocation = "Block sending";
                if (Block.Convert(b) != Block.Convert(type) && !Instant)
                    Player.GlobalBlockchange(this, x, y, z, type);

                if (b == Block.sponge && physics > 0 && type != Block.sponge) physic.PhysSpongeRemoved(this, PosToInt(x, y, z));
                if (b == Block.lava_sponge && physics > 0 && type != Block.lava_sponge)
                    physic.PhysSpongeRemoved(p.level, PosToInt(x, y, z), true);

                errorLocation = "Undo buffer filling";
                Player.UndoPos Pos;
                Pos.x = x;
                Pos.y = y;
                Pos.z = z;
                Pos.mapName = name;
                Pos.type = b;
                Pos.newtype = type;
                Pos.timePlaced = DateTime.Now;
                p.UndoBuffer.Add(Pos);

                errorLocation = "Setting tile";
                p.loginBlocks++;
                p.overallBlocks++;
                SetTile(x, y, z, (ushort)type); //Updates server level blocks

                errorLocation = "Growing grass";
                if (GetTile(x, (ushort)(y - 1), z) == Block.grass && GrassDestroy && !Block.LightPass(type))
                {
                    Blockchange(p, x, (ushort)(y - 1), z, Block.dirt);
                }

                errorLocation = "Adding physics";
                if (p.PlayingTntWars && type == Block.smalltnt) AddCheck(PosToInt(x, y, z), "", false, p);
                if (physics > 0) if (Block.Physics(type)) AddCheck(PosToInt(x, y, z), "", false, p);

                changed = true;
                backedup = false;
            }
            catch (OutOfMemoryException)
            {
                Player.SendMessage(p, "Undo buffer too big! Cleared!");
                p.UndoBuffer.Clear();
                goto retry;
            }
            catch (Exception e)
            {
                Server.ErrorLog(e);
                Player.GlobalMessageOps(p.name + " triggered a non-fatal error on " + name);
                Player.GlobalMessageOps("Error location: " + errorLocation);
                Server.s.Log(p.name + " triggered a non-fatal error on " + name);
                Server.s.Log("Error location: " + errorLocation);
            }

            //if (addaction)
            //{
            //    if (edits.Count == edits.Capacity) { edits.Capacity += 1024; }
            //    if (p.actions.Count == p.actions.Capacity) { p.actions.Capacity += 128; }
            //    if (b.lastaction.Count == 5) { b.lastaction.RemoveAt(0); }
            //    Edit foo = new Edit(this); foo.block = b; foo.from = p.name;
            //    foo.before = b.type; foo.after = type;
            //    b.lastaction.Add(foo); edits.Add(foo); p.actions.Add(foo);
            //} b.type = type;
        }

        public static void SaveSettings(Level level)
        {
            try
            {
                File.Create("levels/level properties/" + level.name + ".properties").Dispose();
                using (StreamWriter SW = File.CreateText("levels/level properties/" + level.name + ".properties"))
                {
                    SW.WriteLine("#Level properties for " + level.name);
                    SW.WriteLine("#Drown-time in seconds is [drown time] * 200 / 3 / 1000");
                    SW.WriteLine("Theme = " + level.theme);
                    SW.WriteLine("Physics = " + level.physics.ToString());
                    SW.WriteLine("Physics speed = " + level.speedphysics.ToString());
                    SW.WriteLine("Physics overload = " + level.overload.ToString());
                    SW.WriteLine("Finite mode = " + level.finite.ToString());
                    SW.WriteLine("Animal AI = " + level.ai.ToString());
                    SW.WriteLine("Edge water = " + level.edgeWater.ToString());
                    SW.WriteLine("Survival death = " + level.Death.ToString());
                    SW.WriteLine("Fall = " + level.fall.ToString());
                    SW.WriteLine("Drown = " + level.drown.ToString());
                    SW.WriteLine("MOTD = " + level.motd);
                    SW.WriteLine("JailX = " + level.jailx.ToString());
                    SW.WriteLine("JailY = " + level.jaily.ToString());
                    SW.WriteLine("JailZ = " + level.jailz.ToString());
                    SW.WriteLine("Unload = " + level.unload.ToString());
                    SW.WriteLine("WorldChat = " + level.worldChat.ToString());
                    SW.WriteLine("PerBuild = " +
                                 (Group.Exists(PermissionToName(level.permissionbuild).ToLower())
                                      ? PermissionToName(level.permissionbuild).ToLower()
                                      : PermissionToName(LevelPermission.Guest)));
                    SW.WriteLine("PerVisit = " +
                                 (Group.Exists(PermissionToName(level.permissionvisit).ToLower())
                                      ? PermissionToName(level.permissionvisit).ToLower()
                                      : PermissionToName(LevelPermission.Guest)));
                    SW.WriteLine("PerBuildMax = " +
                                 (Group.Exists(PermissionToName(level.perbuildmax).ToLower())
                                      ? PermissionToName(level.perbuildmax).ToLower()
                                      : PermissionToName(LevelPermission.Nobody)));
                    SW.WriteLine("PerVisitMax = " +
                                 (Group.Exists(PermissionToName(level.pervisitmax).ToLower())
                                      ? PermissionToName(level.pervisitmax).ToLower()
                                      : PermissionToName(LevelPermission.Nobody)));
                    SW.WriteLine("Guns = " + level.guns.ToString());
                    SW.WriteLine("Type = " + level.mapType.ToString());
                    SW.WriteLine("LoadOnGoto = " + level.loadOnGoto.ToString());
                    SW.WriteLine("LeafDecay = " + level.leafDecay.ToString());
                    SW.WriteLine("RandomFlow = " + level.randomFlow.ToString());
                    SW.WriteLine("GrowTrees = " + level.growTrees.ToString());
                    SW.WriteLine("weather = " + level.weather.ToString());
                    SW.WriteLine("author = " + level.author);
                    SW.WriteLine("likes = " + level.likes);
                    SW.WriteLine("dislikes = " + level.dislikes);
                    SW.WriteLine("redSpawnX = " + level.redSpawn[0]);
                    SW.WriteLine("redSpawnY = " + level.redSpawn[1]);
                    SW.WriteLine("redSpawnZ = " + level.redSpawn[2]);
                    SW.WriteLine("redSpawnRotX = " + level.redRotation[0]);
                    SW.WriteLine("redSpawnRotY = " + level.redRotation[1]);
                    SW.WriteLine("blueSpawnX = " + level.blueSpawn[0]);
                    SW.WriteLine("blueSpawnY = " + level.blueSpawn[1]);
                    SW.WriteLine("blueSpawnZ = " + level.blueSpawn[2]);
                    SW.WriteLine("blueSpawnRotX = " + level.blueRotation[0]);
                    SW.WriteLine("blueSpawnRotY = " + level.blueRotation[1]);
                    SW.WriteLine("redFlagX = " + level.redFlag[0]);
                    SW.WriteLine("redFlagY = " + level.redFlag[1]);
                    SW.WriteLine("redFlagZ = " + level.redFlag[2]);
                    SW.WriteLine("blueFlagX = " + level.blueFlag[0]);
                    SW.WriteLine("blueFlagY = " + level.blueFlag[1]);
                    SW.WriteLine("blueFlagZ = " + level.blueFlag[2]);
                    SW.WriteLine("buildCeiling = " + level.maxBuildHeight);
                    SW.WriteLine("# divider is always along the X axis! Bear this in mind when making your maps.");
                    SW.WriteLine("divider = " + level.divider);
                }
            }
            catch (Exception)
            {
                Server.s.Log("Failed to save level properties!");
            }
        }
        public void Blockchange(int b, ushort type, bool overRide = false, string extraInfo = "")
        //Block change made by physics
        {
            if (b < 0) return;
            if (b >= blocks.Length) return;
            ushort bb = GetTile(b);

            try
            {
                if (!overRide)
                    if (Block.OPBlocks(bb) || (Block.OPBlocks(type) && extraInfo != "")) return;

                if (Block.Convert(bb) != Block.Convert(type))
                    //Should save bandwidth sending identical looking blocks, like air/op_air changes.
                    Player.GlobalBlockchange(this, b, type);

                if (b == Block.sponge && physics > 0 && type != Block.sponge)
                    physic.PhysSpongeRemoved(this, b);

                if (b == Block.lava_sponge && physics > 0 && type != Block.lava_sponge)
                    physic.PhysSpongeRemoved(this, b, true);

                try
                {
                    UndoPos uP;
                    uP.location = b;
                    uP.newType = (ushort)type;
                    uP.oldType = bb;
                    uP.timePerformed = DateTime.Now;

                    if (currentUndo > Server.physUndo)
                    {
                        currentUndo = 0;
                        UndoBuffer[currentUndo] = uP;
                    }
                    else if (UndoBuffer.Count < Server.physUndo)
                    {
                        currentUndo++;
                        UndoBuffer.Add(uP);
                    }
                    else
                    {
                        currentUndo++;
                        UndoBuffer[currentUndo] = uP;
                    }
                }
                catch
                {
                }

                SetTile(b, type); //Updates server level blocks

                if (physics > 0)
                    if (Block.Physics(type) || extraInfo != "") AddCheck(b, extraInfo);
            }
            catch
            {
                SetTile(b, type);
            }
        }
        public void Blockchange(ushort x, ushort y, ushort z, ushort type, bool overRide = false, string extraInfo = "")
        //Block change made by physics
        {
            if (x < 0 || y < 0 || z < 0) return;
            if (x >= width || y >= depth || z >= height) return;
            ushort b = GetTile(x, y, z);

            try
            {
                if (!overRide)
                    if (Block.OPBlocks(b) || (Block.OPBlocks(type) && extraInfo != "")) return;

                if (Block.Convert(b) != Block.Convert(type))
                    //Should save bandwidth sending identical looking blocks, like air/op_air changes.
                    Player.GlobalBlockchange(this, x, y, z, type);

                if (b == Block.sponge && physics > 0 && type != Block.sponge)
                    physic.PhysSpongeRemoved(this, PosToInt(x, y, z));

                if (b == Block.lava_sponge && physics > 0 && type != Block.lava_sponge)
                    physic.PhysSpongeRemoved(this, PosToInt(x, y, z), true);

                try
                {
                    UndoPos uP;
                    uP.location = PosToInt(x, y, z);
                    uP.newType = (ushort)type;
                    uP.oldType = b;
                    uP.timePerformed = DateTime.Now;

                    if (currentUndo > Server.physUndo)
                    {
                        currentUndo = 0;
                        UndoBuffer[currentUndo] = uP;
                    }
                    else if (UndoBuffer.Count < Server.physUndo)
                    {
                        currentUndo++;
                        UndoBuffer.Add(uP);
                    }
                    else
                    {
                        currentUndo++;
                        UndoBuffer[currentUndo] = uP;
                    }
                }
                catch
                {
                }

                SetTile(x, y, z, type); //Updates server level blocks

                if (physics > 0)
                    if (Block.Physics(type) || extraInfo != "") AddCheck(PosToInt(x, y, z), extraInfo);
            }
            catch
            {
                SetTile(x, y, z, type);
            }
        }

        // Returns true if ListCheck does not already have an check in the position.
        // Useful for fireworks, which depend on two physics blocks being checked, one with extraInfo.
        public bool CheckClear(ushort x, ushort y, ushort z)
        {
            int b = PosToInt(x, y, z);
            return !ListCheck.Exists(Check => Check.b == b);
        }

        public void skipChange(ushort x, ushort y, ushort z, ushort type)
        {
            if (x < 0 || y < 0 || z < 0) return;
            if (x >= width || y >= depth || z >= height) return;

            SetTile(x, y, z, type);
        }

        public void Save(bool Override = false, bool clearPhysics = false)
        {
            if(mapType == MapType.Game || (Server.noLevelSaving && Server.ZombieModeOn))
            {
                return;
            }
            //if (season.started)
            //    season.Stop(this);
            if (blocks == null) return;
            string path = "levels/" + name + ".mcf";
            if (LevelSave != null)
                LevelSave(this);
            OnLevelSaveEvent.Call(this);
            if (cancelsave1)
            {
                cancelsave1 = false;
                return;
            }
            if (cancelsave)
            {
                cancelsave = false;
                return;
            }
            try
            {
                if (!Directory.Exists("levels")) Directory.CreateDirectory("levels");
                if (!Directory.Exists("levels/level properties")) Directory.CreateDirectory("levels/level properties");

                if (changed || !File.Exists(path) || Override || (physicschanged && clearPhysics))
                {
                    if (clearPhysics)
                    {
                        physic.ClearPhysics(this);
                    }
                    
                    string backFile = string.Format("{0}.back", path);
                    string backupFile = string.Format("{0}.backup", path);
                    
                    using (FileStream fs = File.OpenWrite(backFile))
                    {
                        using (GZipStream gs = new GZipStream(fs, CompressionMode.Compress))
                        {
                            var header = new byte[16];
                            BitConverter.GetBytes(1874).CopyTo(header, 0);
                            gs.Write(header, 0, 2);

                            BitConverter.GetBytes(width).CopyTo(header, 0);
                            BitConverter.GetBytes(height).CopyTo(header, 2);
                            BitConverter.GetBytes(depth).CopyTo(header, 4);
                    	    changed = false;
                            BitConverter.GetBytes(spawnx).CopyTo(header, 6);
                            BitConverter.GetBytes(spawnz).CopyTo(header, 8);
                            BitConverter.GetBytes(spawny).CopyTo(header, 10);
                            header[12] = rotx;
                            header[13] = roty;
                            header[14] = (byte)permissionvisit;
                            header[15] = (byte)permissionbuild;
                            gs.Write(header, 0, header.Length);
                            var level = new byte[blocks.Length * 2];
                            for (int i = 0; i < blocks.Length; ++i)
                            {
                            	ushort blockVal = 0;
                                if (blocks[i] < 57)
                                //CHANGED THIS TO INCOPARATE SOME MORE SPACE THAT I NEEDED FOR THE door_orange_air ETC.
                                {
                                    if(blocks[i] != Block.air)
                                        blockVal = (ushort)blocks[i];
                                }
                                else
                                {
                                    if (Block.SaveConvert(blocks[i]) != Block.air)
                                        blockVal = (ushort)Block.SaveConvert(blocks[i]);
                                }
                                level[i*2] = (byte)blockVal;
                                level[i*2 + 1] = (byte)(blockVal >> 8);
                            }
                            gs.Write(level, 0, level.Length);
                        }
                    }

					// Safely replace the original file (if it exists) after making a backup.
                    if (File.Exists(path))
                    {
                    	File.Delete(backupFile);
                    	File.Replace(backFile, path, backupFile);
                    }
                    else
                    {
                    	File.Move(backFile, path);
                    }

                    SaveSettings(this);

                    Server.s.Log(string.Format("SAVED: Level \"{0}\". ({1}/{2}/{3})", name, players.Count,
                                               Player.players.Count, Server.players));

                    // UNCOMPRESSED LEVEL SAVING! DO NOT USE!
                    /*using (FileStream fs = File.Create(path + ".wtf"))
                    {
                        byte[] header = new byte[16];
                        BitConverter.GetBytes(1874).CopyTo(header, 0);
                        fs.Write(header, 0, 2);

                        BitConverter.GetBytes(width).CopyTo(header, 0);
                        BitConverter.GetBytes(height).CopyTo(header, 2);
                        BitConverter.GetBytes(depth).CopyTo(header, 4);
                        BitConverter.GetBytes(spawnx).CopyTo(header, 6);
                        BitConverter.GetBytes(spawnz).CopyTo(header, 8);
                        BitConverter.GetBytes(spawny).CopyTo(header, 10);
                        header[12] = rotx; header[13] = roty;
                        header[14] = (byte)permissionvisit;
                        header[15] = (byte)permissionbuild;
                        fs.Write(header, 0, header.Length);
                        byte[] level = new byte[blocks.Length];
                        for (int i = 0; i < blocks.Length; ++i)
                        {
                            if (blocks[i] < 80)
                            {
                                level[i] = blocks[i];
                            }
                            else
                            {
                                level[i] = Block.SaveConvert(blocks[i]);
                            }
                        } fs.Write(level, 0, level.Length); fs.Close();
                    }*/
                }
                else
                {
                    Server.s.Log("Skipping level save for " + name + ".");
                }
            }
            catch (OutOfMemoryException e)
            {
                Server.ErrorLog(e);
                if (Server.mono)
                {
                    Process[] prs = Process.GetProcesses();
                    foreach (Process pr in prs)
                    {
                        if (pr.ProcessName == "MCForge")
                            pr.Kill();

                    }
                }
                else
                    Command.all.Find("restart").Use(null, "");
            }
            catch (Exception e)
            {
                Server.s.Log("FAILED TO SAVE :" + name);
                Player.GlobalMessage("FAILED TO SAVE :" + name);

                Server.ErrorLog(e);
                return;
            }
            //season.Start(this);
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public MapType mapType;

        public int Backup(bool Forced = false, string backupName = "")
        {
            if (!backedup || Forced)
            {
                int backupNumber = 1;
                string backupPath = @Server.backupLocation;
                if (Directory.Exists(string.Format("{0}/{1}", backupPath, name)))
                {
                    backupNumber = Directory.GetDirectories(string.Format("{0}/" + name, backupPath)).Length + 1;
                }
                else
                {
                    Directory.CreateDirectory(backupPath + "/" + name);
                }
                string path = string.Format("{0}/" + name + "/" + backupNumber, backupPath);
                if (backupName != "")
                {
                    path = string.Format("{0}/" + name + "/" + backupName, backupPath);
                }
                Directory.CreateDirectory(path);

                string BackPath = string.Format("{0}/{1}.mcf", path, name);
                string current = string.Format("levels/{0}.mcf", name);
                try
                {
                    File.Copy(current, BackPath, true);
                    backedup = true;
                    return backupNumber;
                }
                catch (Exception e)
                {
                    Server.ErrorLog(e);
                    Server.s.Log(string.Format("FAILED TO INCREMENTAL BACKUP :{0}", name));
                    return -1;
                }
            }
            Server.s.Log("Level unchanged, skipping backup");
            return -1;
        }

        public static void CreateLeveldb(string givenName)
        {
            Database.executeQuery("CREATE TABLE if not exists `Block" + givenName +
                                  "` (Username CHAR(20), TimePerformed DATETIME, X SMALLINT UNSIGNED, Y SMALLINT UNSIGNED, Z SMALLINT UNSIGNED, Type TINYINT UNSIGNED, Deleted " +
                                  (Server.useMySQL ? "BOOL" : "INT") + ")");
            Database.executeQuery("CREATE TABLE if not exists `Portals" + givenName +
                                  "` (EntryX SMALLINT UNSIGNED, EntryY SMALLINT UNSIGNED, EntryZ SMALLINT UNSIGNED, ExitMap CHAR(20), ExitX SMALLINT UNSIGNED, ExitY SMALLINT UNSIGNED, ExitZ SMALLINT UNSIGNED)");
            Database.executeQuery("CREATE TABLE if not exists `Messages" + givenName +
                                  "` (X SMALLINT UNSIGNED, Y SMALLINT UNSIGNED, Z SMALLINT UNSIGNED, Message CHAR(255));");
            Database.executeQuery("CREATE TABLE if not exists `Zone" + givenName +
                                  "` (SmallX SMALLINT UNSIGNED, SmallY SMALLINT UNSIGNED, SmallZ SMALLINT UNSIGNED, BigX SMALLINT UNSIGNED, BigY SMALLINT UNSIGNED, BigZ SMALLINT UNSIGNED, Owner VARCHAR(20));");
        }

        public static Level Load(string givenName)
        {
            return Load(givenName, 0);
        }

        //givenName is safe against SQL injections, it gets checked in CmdLoad.cs
        public static Level Load(string givenName, byte phys, bool bite = false) 
        {
			GC.Collect();
            if (LevelLoad != null)
                LevelLoad(givenName);
            OnLevelLoadEvent.Call(givenName);
            if (cancelload)
            {
                cancelload = false;
                return null;
            }
            CreateLeveldb(givenName);

            string path = string.Format("levels/{0}.{1}", bite ? "byte/" + givenName : givenName, bite ? "lvl" : "mcf"); //OMG RLLY? .MCF??
            if (File.Exists(path))
            {
                FileStream fs = File.OpenRead(path);
                try
                {
                    var gs = new GZipStream(fs, CompressionMode.Decompress);
                    var ver = new byte[2];
                    gs.Read(ver, 0, ver.Length);
                    ushort version = BitConverter.ToUInt16(ver, 0);
                    var vars = new ushort[6];
                    var rot = new byte[2];

                    if (version == 1874)
                    {
                        var header = new byte[16];
                        gs.Read(header, 0, header.Length);

                        vars[0] = BitConverter.ToUInt16(header, 0);
                        vars[1] = BitConverter.ToUInt16(header, 2);
                        vars[2] = BitConverter.ToUInt16(header, 4);
                        vars[3] = BitConverter.ToUInt16(header, 6);
                        vars[4] = BitConverter.ToUInt16(header, 8);
                        vars[5] = BitConverter.ToUInt16(header, 10);

                        rot[0] = header[12];
                        rot[1] = header[13];

                        //level.permissionvisit = (LevelPermission)header[14];
                        //level.permissionbuild = (LevelPermission)header[15];
                    }
                    else
                    {
                        var header = new byte[12];
                        gs.Read(header, 0, header.Length);

                        vars[0] = version;
                        vars[1] = BitConverter.ToUInt16(header, 0);
                        vars[2] = BitConverter.ToUInt16(header, 2);
                        vars[3] = BitConverter.ToUInt16(header, 4);
                        vars[4] = BitConverter.ToUInt16(header, 6);
                        vars[5] = BitConverter.ToUInt16(header, 8);

                        rot[0] = header[10];
                        rot[1] = header[11];
                    }

                    var level = new Level(givenName, vars[0], vars[2], vars[1], "empty")
                                    {
                                        permissionbuild = (LevelPermission)30,
                                        spawnx = vars[3],
                                        spawnz = vars[4],
                                        spawny = vars[5],
                                        rotx = rot[0],
                                        roty = rot[1],
                                        name = givenName
                                    };


                    level.setPhysics(phys);
                    var blocks = new byte[(bite ? 1 : 2) * level.width * level.height * level.depth];
                    gs.Read(blocks, 0, blocks.Length);
                    if(!bite)
                        for (int i = 0; i < (blocks.Length / 2); ++i)
                            level.blocks[i] = BitConverter.ToUInt16(new byte[] { blocks[i * 2], blocks[(i * 2) + 1] }, 0);
                    else
                        for (int i = 0; i < blocks.Length; ++i)
                            level.blocks[i] = (ushort)blocks[i];
                    gs.Close();
                    gs.Dispose();
                    //level.textures = new LevelTextures(level);
                    level.backedup = true;

                    using (DataTable ZoneDB = Database.fillData("SELECT * FROM `Zone" + givenName + "`"))
                    {
                        Zone Zn;
                        for (int i = 0; i < ZoneDB.Rows.Count; ++i)
                        {
                            Zn.smallX = ushort.Parse(ZoneDB.Rows[i]["SmallX"].ToString());
                            Zn.smallY = ushort.Parse(ZoneDB.Rows[i]["SmallY"].ToString());
                            Zn.smallZ = ushort.Parse(ZoneDB.Rows[i]["SmallZ"].ToString());
                            Zn.bigX = ushort.Parse(ZoneDB.Rows[i]["BigX"].ToString());
                            Zn.bigY = ushort.Parse(ZoneDB.Rows[i]["BigY"].ToString());
                            Zn.bigZ = ushort.Parse(ZoneDB.Rows[i]["BigZ"].ToString());
                            Zn.Owner = ZoneDB.Rows[i]["Owner"].ToString();
                            level.ZoneList.Add(Zn);
                        }
                    }

                    level.jailx = (ushort)(level.spawnx * 32);
                    level.jaily = (ushort)(level.spawny * 32);
                    level.jailz = (ushort)(level.spawnz * 32);
                    level.jailrotx = level.rotx;
                    level.jailroty = level.roty;
                    level.physic.StartPhysics(level);
                    //level.physChecker.Elapsed += delegate
                    //{
                    //    if (!level.physicssate && level.physics > 0)
                    //        level.physic.StartPhysics(level);
                    //};
                    //level.physChecker.Start();
                    //level.season = new SeasonsCore(level);
                    try
                    {
                        DataTable foundDB = Database.fillData("SELECT * FROM `Portals" + givenName + "`");

                        for (int i = 0; i < foundDB.Rows.Count; ++i)
                        {
                            if (
                                !Block.portal(level.GetTile(ushort.Parse(foundDB.Rows[i]["EntryX"].ToString()),
                                                            ushort.Parse(foundDB.Rows[i]["EntryY"].ToString()),
                                                            ushort.Parse(foundDB.Rows[i]["EntryZ"].ToString()))))
                            {
                                Database.executeQuery("DELETE FROM `Portals" + givenName + "` WHERE EntryX=" +
                                                      foundDB.Rows[i]["EntryX"] + " AND EntryY=" +
                                                      foundDB.Rows[i]["EntryY"] + " AND EntryZ=" +
                                                      foundDB.Rows[i]["EntryZ"]);
                            }
                        }
                        foundDB = Database.fillData("SELECT * FROM `Messages" + givenName + "`");

                        for (int i = 0; i < foundDB.Rows.Count; ++i)
                        {
                            if (
                                !Block.mb(level.GetTile(ushort.Parse(foundDB.Rows[i]["X"].ToString()),
                                                        ushort.Parse(foundDB.Rows[i]["Y"].ToString()),
                                                        ushort.Parse(foundDB.Rows[i]["Z"].ToString()))))
                            {
                                //givenName is safe against SQL injections, it gets checked in CmdLoad.cs
                                Database.executeQuery("DELETE FROM `Messages" + givenName + "` WHERE X=" +
                                                      foundDB.Rows[i]["X"] + " AND Y=" + foundDB.Rows[i]["Y"] +
                                                      " AND Z=" + foundDB.Rows[i]["Z"]);
                            }
                        }
                        foundDB.Dispose();
                    }
                    catch (Exception e)
                    {
                        Server.ErrorLog(e);
                    }

                    try
                    {
                        string foundLocation;
                        foundLocation = "levels/level properties/" + level.name + ".properties";
                        if (!File.Exists(foundLocation))
                        {
                            foundLocation = "levels/level properties/" + level.name;
                        }

                        foreach (string line in File.ReadAllLines(foundLocation))
                        {
                            try
                            {
                                if (line[0] == '#') continue;
                                string value = line.Substring(line.IndexOf(" = ") + 3);

                                switch (line.Substring(0, line.IndexOf(" = ")).ToLower())
                                {
                                    case "theme":
                                        level.theme = value;
                                        break;
                                    case "physics":
                                        level.setPhysics(int.Parse(value));
                                        break;
                                    case "physics speed":
                                        level.speedphysics = int.Parse(value);
                                        break;
                                    case "physics overload":
                                        level.overload = int.Parse(value);
                                        break;
                                    case "finite mode":
                                        level.finite = bool.Parse(value);
                                        break;
                                    case "animal ai":
                                        level.ai = bool.Parse(value);
                                        break;
                                    case "edge water":
                                        level.edgeWater = bool.Parse(value);
                                        break;
                                    case "survival death":
                                        level.Death = bool.Parse(value);
                                        break;
                                    case "fall":
                                        level.fall = int.Parse(value);
                                        break;
                                    case "drown":
                                        level.drown = int.Parse(value);
                                        break;
                                    case "motd":
                                        level.motd = value;
                                        break;
                                    case "jailx":
                                        level.jailx = ushort.Parse(value);
                                        break;
                                    case "jaily":
                                        level.jaily = ushort.Parse(value);
                                        break;
                                    case "jailz":
                                        level.jailz = ushort.Parse(value);
                                        break;
                                    case "unload":
                                        level.unload = bool.Parse(value);
                                        break;
                                    case "worldchat":
                                        level.worldChat = bool.Parse(value);
                                        break;
                                    case "perbuild":
                                        level.permissionbuild = PermissionFromName(value) != LevelPermission.Null ? PermissionFromName(value) : LevelPermission.Guest;
                                        break;
                                    case "pervisit":
                                        level.permissionvisit = PermissionFromName(value) != LevelPermission.Null ? PermissionFromName(value) : LevelPermission.Guest;
                                        break;
                                    case "perbuildmax":
                                        level.perbuildmax = PermissionFromName(value) != LevelPermission.Null ? PermissionFromName(value) : LevelPermission.Guest;
                                        break;
                                    case "pervisitmax":
                                        level.pervisitmax = PermissionFromName(value) != LevelPermission.Null ? PermissionFromName(value) : LevelPermission.Guest;
                                        break;
                                    case "guns":
                                        level.guns = bool.Parse(value);
                                        break;
                                    case "type":
                                        level.mapType = (MapType)Enum.Parse(typeof(MapType), value);
                                        break;
                                    case "loadongoto":
                                        level.loadOnGoto = bool.Parse(value);
                                        break;
                                    case "leafdecay":
                                        level.leafDecay = bool.Parse(value);
                                        break;
                                    case "randomflow":
                                        level.randomFlow = bool.Parse(value);
                                        break;
                                    case "growtrees":
                                        level.growTrees = bool.Parse(value);
                                        break;
                                    case "weather": level.weather = byte.Parse(value); break;
                                    case "author": level.author = value; break;
                                    case "likes": level.likes = int.Parse(value); break;
                                    case "dislikes": level.dislikes = int.Parse(value); break;
                                    case "redspawnx": level.redSpawn[0] = ushort.Parse(value); break;
                                    case "redspawny": level.redSpawn[1] = ushort.Parse(value); break;
                                    case "redspawnz": level.redSpawn[2] = ushort.Parse(value); break;
                                    case "redspawnrotx": level.redRotation[0] = byte.Parse(value); break;
                                    case "redspawnroty": level.redRotation[1] = byte.Parse(value); break;
                                    case "bluespawnx": level.blueSpawn[0] = ushort.Parse(value); break;
                                    case "bluespawny": level.blueSpawn[1] = ushort.Parse(value); break;
                                    case "bluespawnz": level.blueSpawn[2] = ushort.Parse(value); break;
                                    case "bluespawnrotx": level.blueRotation[0] = byte.Parse(value); break;
                                    case "bluespawnroty": level.blueRotation[1] = byte.Parse(value); break;
                                    case "redflagx": level.redFlag[0] = ushort.Parse(value); break;
                                    case "redflagy": level.redFlag[1] = ushort.Parse(value); break;
                                    case "redflagz": level.redFlag[2] = ushort.Parse(value); break;
                                    case "blueflagx": level.blueFlag[0] = ushort.Parse(value); break;
                                    case "blueflagy": level.blueFlag[1] = ushort.Parse(value); break;
                                    case "blueflagz": level.blueFlag[2] = ushort.Parse(value); break;
                                    case "buildceiling": level.maxBuildHeight = int.Parse(value); break;
                                    case "divider": level.divider = ushort.Parse(value); break;
                                }
                            }
                            catch (Exception e)
                            {
                                Server.ErrorLog(e);
                            }
                        }
                    }
                    catch
                    {
                    }
                    level.CalculateShadows();
                    Server.s.Log(string.Format("Level \"{0}\" loaded.", level.name));
                    if (LevelLoaded != null)
                        LevelLoaded(level);
                    OnLevelLoadedEvent.Call(level);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    return level;
                }
                catch (Exception ex)
                {
                    Server.ErrorLog(ex);
                    return null;
                }
                finally
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    fs.Close();
                    fs.Dispose();
                }
            }
            Server.s.Log("ERROR loading level.");
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return null;
        }

        public static bool CheckLoadOnGoto(string givenName)
        {
            try
            {
                string foundLocation;
                foundLocation = "levels/level properties/" + givenName + ".properties";
                if (!File.Exists(foundLocation))
                    foundLocation = "levels/level properties/" + givenName;
                if (!File.Exists(foundLocation))
                    return true;

                foreach (string line in File.ReadAllLines(foundLocation))
                {
                    try
                    {
                        if (line[0] == '#') continue;
                        string value = line.Substring(line.IndexOf(" = ") + 3);

                        switch (line.Substring(0, line.IndexOf(" = ")).ToLower())
                        {
                            case "loadongoto":
                                return bool.Parse(value);
                        }
                    }
                    catch (Exception e)
                    {
                        Server.ErrorLog(e);
                    }
                }
            }
            catch
            {
            }
            return true;
        }

        public void ChatLevel(string message)
        {
            foreach (Player pl in Player.players.Where(pl => pl.level == this))
            {
                pl.SendMessage(message);
            }
        }

        public void ChatLevelOps(string message)
        {
            foreach (
                Player pl in
                    Player.players.Where(
                        pl =>
                        pl.level == this &&
                        (pl.group.Permission >= Server.opchatperm || pl.isStaff )))
            {
                pl.SendMessage(message);
            }
        }

        public void ChatLevelAdmins(string message)
        {
            foreach (
                Player pl in
                    Player.players.Where(
                        pl =>
                        pl.level == this &&
                        (pl.group.Permission >= Server.adminchatperm || pl.isStaff)))
            {
                pl.SendMessage(message);
            }
        }

        public void setPhysics(int newValue)
        {
            if (physics == 0 && newValue != 0 && blocks != null)
            {
                for (int i = 0; i < blocks.Length; i++)
                    // Optimization hack, since no blocks under 183 ever need a restart
                    if (blocks[i] > 183)
                        if (Block.NeedRestart(blocks[i]))
                            AddCheck(i);
            }
            physics = newValue;
            //physic.StartPhysics(level); This isnt needed, the physics will start when we set the new value above
        }

        /// <summary>
        /// Gets or sets a value indicating whether physics are enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if physics are enabled; otherwise, <c>false</c>.
        /// </value>
        public bool PhysicsEnabled { get; set; }

        public int PosToInt(ushort x, ushort y, ushort z)
        {
            if (x < 0 || x >= width || y < 0 || y >= depth || z < 0 || z >= height)
                return -1;
            return x + (z * width) + (y * width * height);
            //alternate method: (h * widthY + y) * widthX + x;
        }

        public void IntToPos(int pos, out ushort x, out ushort y, out ushort z)
        {
            y = (ushort)(pos / width / height);
            pos -= y * width * height;
            z = (ushort)(pos / width);
            pos -= z * width;
            x = (ushort)pos;
        }

        public int IntOffset(int pos, int x, int y, int z)
        {
            return pos + x + z * width + y * width * height;
        }

        public static LevelPermission PermissionFromName(string name)
        {
            Group foundGroup = Group.Find(name);
            return foundGroup != null ? foundGroup.Permission : LevelPermission.Null;
        }

        public static string PermissionToName(LevelPermission perm)
        {
            Group foundGroup = Group.findPerm(perm);
            return foundGroup != null ? foundGroup.name : ((int)perm).ToString();
        }

        public List<Player> getPlayers()
        {
            return Player.players.Where(p => p.level == this).ToList();
        }

        #region ==Physics==

        public string foundInfo(ushort x, ushort y, ushort z)
        {
            Check foundCheck = null;
            try
            {
                foundCheck = ListCheck.Find(Check => Check.b == PosToInt(x, y, z));
            }
            catch
            {
            }
            if (foundCheck != null)
                return foundCheck.extraInfo;
            return "";
        }

        public void AddCheck(int b, string extraInfo = "", bool overRide = false, MCForge.Player Placer = null)
        {
            try
            {
                if (!ListCheck.Exists(Check => Check.b == b))
                {
                    ListCheck.Add(new Check(b, extraInfo, Placer)); //Adds block to list to be updated
                }
                else
                {
                    if (overRide)
                    {
                        foreach (Check C2 in ListCheck)
                        {
                            if (C2.b == b)
                            {
                                C2.extraInfo = extraInfo; //Dont need to check physics here because if the list is active, then physics is active :)
                                return;
                            }
                        }
                    }
                }
                if (!physic.physicssate && physics > 0)
                    physic.StartPhysics(this);
            }
            catch
            {
                //s.Log("Warning-PhysicsCheck");
                //ListCheck.Add(new Check(b));    //Lousy back up plan
            }
        }

        public bool AddUpdate(int b, ushort type, bool overRide = false, string extraInfo = "")
        {
            try
            {
                if (overRide)
                {
                    ushort x, y, z;
                    IntToPos(b, out x, out y, out z);
                    AddCheck(b, extraInfo, true); //Dont need to check physics here....AddCheck will do that
                    Blockchange(x, y, z, (ushort)type, true, extraInfo);
                    return true;
                }

                if (!ListUpdate.Exists(Update => Update.b == b))
                {
                    ListUpdate.Add(new Update(b, (byte)type, extraInfo));
                    if (!physic.physicssate && physics > 0)
                        physic.StartPhysics(this);
                    return true;
                }
                else
                {
                    if (type == 12 || type == 13)
                    {
                        ListUpdate.RemoveAll(Update => Update.b == b);
                        ListUpdate.Add(new Update(b, (byte)type, extraInfo));
                        if (!physic.physicssate && physics > 0)
                            physic.StartPhysics(this);
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                //s.Log("Warning-PhysicsUpdate");
                //ListUpdate.Add(new Update(b, (byte)type));    //Lousy back up plan
                return false;
            }
        }

        public void odoor(Check C)
        {
            if (C.time == 0)
            {
                ushort foundBlock;

                foundBlock = Block.odoor(GetTile(IntOffset(C.b, -1, 0, 0)));
                if (foundBlock == blocks[C.b])
                {
                    AddUpdate(IntOffset(C.b, -1, 0, 0), foundBlock, true);
                }
                foundBlock = Block.odoor(GetTile(IntOffset(C.b, 1, 0, 0)));
                if (foundBlock == blocks[C.b])
                {
                    AddUpdate(IntOffset(C.b, 1, 0, 0), foundBlock, true);
                }
                foundBlock = Block.odoor(GetTile(IntOffset(C.b, 0, -1, 0)));
                if (foundBlock == blocks[C.b])
                {
                    AddUpdate(IntOffset(C.b, 0, -1, 0), foundBlock, true);
                }
                foundBlock = Block.odoor(GetTile(IntOffset(C.b, 0, 1, 0)));
                if (foundBlock == blocks[C.b])
                {
                    AddUpdate(IntOffset(C.b, 0, 1, 0), foundBlock, true);
                }
                foundBlock = Block.odoor(GetTile(IntOffset(C.b, 0, 0, -1)));
                if (foundBlock == blocks[C.b])
                {
                    AddUpdate(IntOffset(C.b, 0, 0, -1), foundBlock, true);
                }
                foundBlock = Block.odoor(GetTile(IntOffset(C.b, 0, 0, 1)));
                if (foundBlock == blocks[C.b])
                {
                    AddUpdate(IntOffset(C.b, 0, 0, 1), foundBlock, true);
                }
            }
            else
            {
                C.time = 255;
            }
            C.time++;
        }

        public void AnyDoor(Check C, ushort x, ushort y, ushort z, int timer, bool instaUpdate = false)
        {
            if (C.time == 0)
            {
                try
                {
                    PhysDoor((ushort)(x + 1), y, z, instaUpdate);
                }
                catch
                {
                }
                try
                {
                    PhysDoor((ushort)(x - 1), y, z, instaUpdate);
                }
                catch
                {
                }
                try
                {
                    PhysDoor(x, y, (ushort)(z + 1), instaUpdate);
                }
                catch
                {
                }
                try
                {
                    PhysDoor(x, y, (ushort)(z - 1), instaUpdate);
                }
                catch
                {
                }
                try
                {
                    PhysDoor(x, (ushort)(y - 1), z, instaUpdate);
                }
                catch
                {
                }
                try
                {
                    PhysDoor(x, (ushort)(y + 1), z, instaUpdate);
                }
                catch
                {
                }

                try
                {
                    if (blocks[C.b] == Block.door8_air)
                    {
                        for (int xx = -1; xx <= 1; xx++)
                        {
                            for (int yy = -1; yy <= 1; yy++)
                            {
                                for (int zz = -1; zz <= 1; zz++)
                                {
                                    ushort b = GetTile(IntOffset(C.b, xx, yy, zz));
                                    if (b == Block.rocketstart)
                                    {
                                        if (physics == 5)
                                        {
                                            Blockchange(x, y, z, Block.air);
                                            return;
                                        }
                                        int b1 = IntOffset(C.b, xx * 3, yy * 3, zz * 3);
                                        int b2 = IntOffset(C.b, xx * 2, yy * 2, zz * 2);
                                        bool unblocked = blocks[b1] == Block.air&& blocks[b2] == Block.air&&
                                                         !ListUpdate.Exists(Update => Update.b == b1) &&
                                                         !ListUpdate.Exists(Update => Update.b == b2);
                                        if (unblocked)
                                        {
                                            AddUpdate(IntOffset(C.b, xx * 3, yy * 3, zz * 3), Block.rockethead);
                                            AddUpdate(IntOffset(C.b, xx * 2, yy * 2, zz * 2), Block.fire);
                                        }
                                    }
                                    else if (b == Block.firework)
                                    {
                                        if (physics == 5)
                                        {
                                            Blockchange(x, y, z, Block.air);
                                            return;
                                        }
                                        int b1 = IntOffset(C.b, xx, yy + 1, zz);
                                        int b2 = IntOffset(C.b, xx, yy + 2, zz);
                                        bool unblocked = blocks[b1] == Block.air&& blocks[b2] == Block.air&&
                                                         !ListUpdate.Exists(Update => Update.b == b1) &&
                                                         !ListUpdate.Exists(Update => Update.b == b2);
                                        if (unblocked)
                                        {
                                            AddUpdate(b2, Block.firework);
                                            AddUpdate(b1, Block.lavastill, false, "dissipate 100");
                                        }
                                    }
                                    else if (b == Block.tnt)
                                    {
                                        if (physics == 5)
                                        {
                                            Blockchange(x, y, z, Block.air);
                                            return;
                                        }
                                        MakeExplosion((ushort)(x + xx), (ushort)(y + yy), (ushort)(z + zz), 0);
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            if (C.time < timer) C.time++;
            else
            {
                AddUpdate(C.b, Block.SaveConvert(blocks[C.b])); //turn back into door
                C.time = 255;
            }
        }

        public void PhysDoor(ushort x, ushort y, ushort z, bool instaUpdate)
        {
            int foundInt = PosToInt(x, y, z);
            ushort FoundAir = Block.DoorAirs(blocks[foundInt]);

            if (FoundAir != 0)
            {
                if (!instaUpdate) AddUpdate(foundInt, FoundAir);
                else Blockchange(x, y, z, FoundAir);
                return;
            }

            if (Block.tDoor(blocks[foundInt]))
            {
                AddUpdate(foundInt, Block.air, false, "wait 16 door 1 revert " + blocks[foundInt].ToString());
            }

            if (Block.odoor(blocks[foundInt]) != Block.Zero) AddUpdate(foundInt, Block.odoor(blocks[foundInt]), true);
        }

        public void MakeExplosion(ushort x, ushort y, ushort z, int size, bool force = false, TntWarsGame CheckForExplosionZone = null)
        {
            //DateTime start = DateTime.Now;
            int xx, yy, zz;
            var rand = new Random();
            ushort b;

            if (physics < 2 && force == false) return;
            if (physics == 5 && force == false) return;
            AddUpdate(PosToInt(x, y, z), Block.tntexplosion, true);

            for (xx = (x - (size + 1)); xx <= (x + (size + 1)); ++xx)
                for (yy = (y - (size + 1)); yy <= (y + (size + 1)); ++yy)
                    for (zz = (z - (size + 1)); zz <= (z + (size + 1)); ++zz)
                        try
                        {
                            b = GetTile((ushort)xx, (ushort)yy, (ushort)zz);
                            if (b == Block.tnt)
                            {
                                AddUpdate(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), Block.smalltnt);
                            }
                            else if (b != Block.smalltnt && b != Block.bigtnt && b != Block.nuketnt)
                            {
                                if (CheckForExplosionZone != null && b != Block.air)
                                {
                                    if (CheckForExplosionZone.InZone((ushort)xx, (ushort)yy, (ushort)zz, false))
                                    {
                                        continue;
                                    }
                                }
                                if (rand.Next(1, 11) <= 4)
                                    AddUpdate(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), Block.tntexplosion);
                                else if (rand.Next(1, 11) <= 8)
                                    AddUpdate(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), Block.air);
                                else
                                    AddCheck(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), "drop 50 dissipate 8");
                            }
                            else
                            {
                                AddCheck(PosToInt((ushort)xx, (ushort)yy, (ushort)zz));
                            }
                        }
                        catch
                        {
                        }

            for (xx = (x - (size + 2)); xx <= (x + (size + 2)); ++xx)
                for (yy = (y - (size + 2)); yy <= (y + (size + 2)); ++yy)
                    for (zz = (z - (size + 2)); zz <= (z + (size + 2)); ++zz)
                    {
                        b = GetTile((ushort)xx, (ushort)yy, (ushort)zz);
                        if (rand.Next(1, 10) < 7)
                            if (Block.Convert(b) != Block.tnt)
                            {
                                if (CheckForExplosionZone != null && b != Block.air)
                                {
                                    if (CheckForExplosionZone.InZone((ushort)xx, (ushort)yy, (ushort)zz, false))
                                    {
                                        continue;
                                    }
                                }
                                if (rand.Next(1, 11) <= 4)
                                    AddUpdate(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), Block.tntexplosion);
                                else if (rand.Next(1, 11) <= 8)
                                    AddUpdate(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), Block.air);
                                else
                                    AddCheck(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), "drop 50 dissipate 8");
                            }
                        if (b == Block.tnt)
                        {
                            AddUpdate(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), Block.smalltnt);
                        }
                        else if (b == Block.smalltnt || b == Block.bigtnt || b == Block.nuketnt)
                        {
                            AddCheck(PosToInt((ushort)xx, (ushort)yy, (ushort)zz));
                        }
                    }

            for (xx = (x - (size + 3)); xx <= (x + (size + 3)); ++xx)
                for (yy = (y - (size + 3)); yy <= (y + (size + 3)); ++yy)
                    for (zz = (z - (size + 3)); zz <= (z + (size + 3)); ++zz)
                    {
                        b = GetTile((ushort)xx, (ushort)yy, (ushort)zz);
                        if (rand.Next(1, 10) < 3)
                            if (Block.Convert(b) != Block.tnt)
                            {
                                if (CheckForExplosionZone != null && b != Block.air)
                                {
                                    if (CheckForExplosionZone.InZone((ushort)xx, (ushort)yy, (ushort)zz, false))
                                    {
                                        continue;
                                    }
                                }
                                if (rand.Next(1, 11) <= 4)
                                    AddUpdate(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), Block.tntexplosion);
                                else if (rand.Next(1, 11) <= 8)
                                    AddUpdate(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), Block.air);
                                else
                                    AddCheck(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), "drop 50 dissipate 8");
                            }
                        if (b == Block.tnt)
                        {
                            AddUpdate(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), Block.smalltnt);
                        }
                        else if (b == Block.smalltnt || b == Block.bigtnt || b == Block.nuketnt)
                        {
                            AddCheck(PosToInt((ushort)xx, (ushort)yy, (ushort)zz));
                        }
                    }
            //Server.s.Log("Explosion: " + (DateTime.Now - start).TotalMilliseconds.ToString());
        }

        public void MakeExplosion(string name, ushort x, ushort y, ushort z, int size, bool bigtnt, bool nuke, bool tnt)
        {
            //DateTime start = DateTime.Now;
            Player p = Player.Find(name);
            Server.killed.Clear();
            int xx, yy, zz; Random rand = new Random(); ushort b;

            ushort bc = GetTile((ushort)x, (ushort)y, (ushort)z);
            if (!Block.OPBlocks(bc) && bc != Block.blueflag && bc != Block.redflag)
            {
                AddUpdate(PosToInt(x, y, z), Block.tntexplosion, true);
            }

            if (p.tntUpgrade > 1 && !bigtnt && !nuke)
            {
                for (xx = (x - (size + 4)); xx <= (x + (size + 4)); ++xx)
                    for (yy = (y - (size + 4)); yy <= (y + (size + 4)); ++yy)
                        for (zz = (z - (size + 4)); zz <= (z + (size + 4)); ++zz)
                        {
                            b = GetTile((ushort)xx, (ushort)yy, (ushort)zz);
                            if (Block.Convert(b) != Block.tnt)
                            {
                                p.killingPeople = true;
                                Player pl = Server.pctf.killedPlayer(p, (ushort)xx, (ushort)yy, (ushort)zz, tnt, "tnt");
                                if (pl != null) Server.killed.Add(pl);
                                if (!Block.OPBlocks(b) && b != Block.blueflag && b != Block.redflag && b != Block.air)
                                {
                                    AddUpdate(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), Block.air, true);
                                }
                            }
                            if (b == Block.tnt)
                            {
                                AddUpdate(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), Block.smalltnt);
                            }
                            else if (b == Block.smalltnt || b == Block.bigtnt || b == Block.nuketnt)
                            {
                                AddCheck(PosToInt((ushort)xx, (ushort)yy, (ushort)zz));
                            }
                        }
            }
            else if (p.tntUpgrade > 0 && !bigtnt && !nuke)
            {
                for (xx = (x - (size + 3)); xx <= (x + (size + 3)); ++xx)
                    for (yy = (y - (size + 3)); yy <= (y + (size + 3)); ++yy)
                        for (zz = (z - (size + 3)); zz <= (z + (size + 3)); ++zz)
                        {
                            b = GetTile((ushort)xx, (ushort)yy, (ushort)zz);
                            if (Block.Convert(b) != Block.tnt)
                            {
                                p.killingPeople = true;
                                Player pl = Server.pctf.killedPlayer(p, (ushort)xx, (ushort)yy, (ushort)zz, tnt, "tnt");
                                if (pl != null) Server.killed.Add(pl);
                                if (!Block.OPBlocks(b) && b != Block.blueflag && b != Block.redflag && b != Block.air)
                                {
                                    AddUpdate(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), Block.air, true);
                                }
                            }
                            if (b == Block.tnt)
                            {
                                AddUpdate(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), Block.smalltnt);
                            }
                            else if (b == Block.smalltnt || b == Block.bigtnt || b == Block.nuketnt)
                            {
                                AddCheck(PosToInt((ushort)xx, (ushort)yy, (ushort)zz));
                            }
                        }
            }
            else if (!bigtnt && !nuke)
            {
                for (xx = (x - (size + 2)); xx <= (x + (size + 2)); ++xx)
                    for (yy = (y - (size + 2)); yy <= (y + (size + 2)); ++yy)
                        for (zz = (z - (size + 2)); zz <= (z + (size + 2)); ++zz)
                        {
                            b = GetTile((ushort)xx, (ushort)yy, (ushort)zz);
                            if (Block.Convert(b) != Block.tnt)
                            {
                                p.killingPeople = true;
                                Player pl = Server.pctf.killedPlayer(p, (ushort)xx, (ushort)yy, (ushort)zz, tnt, "tnt");
                                if (pl != null) Server.killed.Add(pl);
                                if (!Block.OPBlocks(b) && b != Block.blueflag && b != Block.redflag && b != Block.air)
                                {
                                    AddUpdate(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), Block.air, true);
                                }
                            }
                            if (b == Block.tnt)
                            {
                                AddUpdate(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), Block.smalltnt);
                            }
                            else if (b == Block.smalltnt || b == Block.bigtnt || b == Block.nuketnt)
                            {
                                AddCheck(PosToInt((ushort)xx, (ushort)yy, (ushort)zz));
                            }
                        }
            }

            if (bigtnt)
            {
                for (xx = (x - (size + 5)); xx <= (x + (size + 5)); ++xx)
                    for (yy = (y - (size + 5)); yy <= (y + (size + 5)); ++yy)
                        for (zz = (z - (size + 5)); zz <= (z + (size + 5)); ++zz)
                        {
                            b = GetTile((ushort)xx, (ushort)yy, (ushort)zz);
                            if (Block.Convert(b) != Block.tnt)
                            {
                                p.killingPeople = true;
                                Player pl = Server.pctf.killedPlayer(p, (ushort)xx, (ushort)yy, (ushort)zz, tnt, "tnt");
                                if (pl != null) Server.killed.Add(pl);
                                if (!Block.OPBlocks(b) && b != Block.blueflag && b != Block.redflag && b != Block.air)
                                {
                                    AddUpdate(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), Block.air, true);
                                }
                            }
                            if (b == Block.tnt)
                            {
                                AddUpdate(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), Block.smalltnt);
                            }
                            else if (b == Block.smalltnt || b == Block.bigtnt || b == Block.nuketnt)
                            {
                                AddCheck(PosToInt((ushort)xx, (ushort)yy, (ushort)zz));
                            }
                        }
            }

            if (nuke)
            {
                for (xx = (x - (size + 10)); xx <= (x + (size + 10)); ++xx)
                    for (yy = (y - (size + 10)); yy <= (y + (size + 10)); ++yy)
                        for (zz = (z - (size + 10)); zz <= (z + (size + 10)); ++zz)
                        {
                            b = GetTile((ushort)xx, (ushort)yy, (ushort)zz);
                            if (Block.Convert(b) != Block.tnt)
                            {
                                p.killingPeople = true;
                                Player pl = Server.pctf.killedPlayer(p, (ushort)xx, (ushort)yy, (ushort)zz, tnt, "tnt");
                                if (pl != null) Server.killed.Add(pl);
                                if (!Block.OPBlocks(b) && b != Block.blueflag && b != Block.redflag && b != Block.air)
                                {
                                    AddUpdate(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), Block.air, true);
                                }
                            }
                            if (b == Block.tnt)
                            {
                                AddUpdate(PosToInt((ushort)xx, (ushort)yy, (ushort)zz), Block.smalltnt);
                            }
                            else if (b == Block.smalltnt || b == Block.bigtnt || b == Block.nuketnt)
                            {
                                AddCheck(PosToInt((ushort)xx, (ushort)yy, (ushort)zz));
                            }
                        }
            }
            p.killingPeople = false;
            //p.amountKilled = 0;
            //Server.s.Log("Explosion: " + (DateTime.Now - start).TotalMilliseconds.ToString());
        }

        public void placeBlock(ushort x, ushort y, ushort z, ushort b)
        {
            AddUpdate(PosToInt((ushort)x, (ushort)y, (ushort)z), b, true);
            AddCheck(PosToInt((ushort)x, (ushort)y, (ushort)z));
        }

        public void makeLinesplosion(string name, ushort x, ushort y, ushort z, int width, bool lazer)
        {
            Player p = Player.Find(name);
            Server.killed.Clear();
            ushort b; int length = 0; int length2 = 0;
            length = p.level.width;
            length2 = p.level.width - p.level.width - p.level.width;

            int num = p.rot[0];
            int rot360 = (int)Math.Round(p.rot[0] * 1.40625f);
            if (rot360 >= 218 && rot360 <= 319)
            {
                //north
                for (int xx = x; xx >= length2; xx--)
                {
                    if (p.pistolUpgrade > 2)
                    {
                        if (!lazer && xx == x - 30) { AddCheck(PosToInt((ushort)xx, (ushort)y, (ushort)z)); break; }
                    }
                    else if (p.pistolUpgrade > 0)
                    {
                        if (!lazer && xx == x - 20) { AddCheck(PosToInt((ushort)xx, (ushort)y, (ushort)z)); break; }
                    }
                    else
                    {
                        if (!lazer && xx == x - 10) { AddCheck(PosToInt((ushort)xx, (ushort)y, (ushort)z)); break; }
                    }
                    b = GetTile((ushort)xx, (ushort)y, (ushort)z);
                    if ((Block.Convert(b) == Block.air && !lazer) || (!lazer && p.pistolUpgrade > 1 && !Block.OPBlocks(b)))
                    {
                        p.killingPeople = true;
                        Player pl = Server.pctf.killedPlayer(p, (ushort)xx, (ushort)y, (ushort)z, false, "pistol");
                        if (pl != null) Server.killed.Add(pl);
                        ushort bb = GetTile((ushort)xx, (ushort)y, (ushort)z);
                        if (!Block.OPBlocks(bb))
                        {
                            AddUpdate(PosToInt((ushort)xx, (ushort)y, (ushort)z), Block.tntexplosion, lazer);
                        }
                    }
                    else if (lazer && p.lazerUpgrade > 1)
                    {
                        p.killingPeople = true;
                        Player pl = Server.pctf.killedPlayer(p, (ushort)xx, (ushort)y, (ushort)z, false, "lazer");
                        if (pl != null) Server.killed.Add(pl);
                        ushort bb = GetTile((ushort)xx, (ushort)y, (ushort)z);
                        if (!Block.OPBlocks(bb))
                        {
                            AddUpdate(PosToInt((ushort)xx, (ushort)y, (ushort)z), Block.tntexplosion, lazer);
                        }
                    }
                    else if (lazer && !Block.OPBlocks(b))
                    {
                        p.killingPeople = true;
                        Player pl = Server.pctf.killedPlayer(p, (ushort)xx, (ushort)y, (ushort)z, false, "lazer");
                        if (pl != null) Server.killed.Add(pl);
                        ushort bb = GetTile((ushort)xx, (ushort)y, (ushort)z);
                        if (!Block.OPBlocks(bb))
                        {
                            AddUpdate(PosToInt((ushort)xx, (ushort)y, (ushort)z), Block.tntexplosion, lazer);
                        }
                    }
                    else
                    {
                        AddCheck(PosToInt((ushort)xx, (ushort)y, (ushort)z));
                        break;
                    }
                }
                p.killingPeople = false;
            }
            else if ((rot360 >= 320 && rot360 <= 360) || (rot360 >= 0 && rot360 <= 47))
            {
                //east
                for (int zz = z; zz >= length2; zz--)
                {
                    if (p.pistolUpgrade > 2)
                    {
                        if (!lazer && zz == z - 30) { AddCheck(PosToInt((ushort)x, (ushort)y, (ushort)zz)); break; }
                    }
                    else if (p.pistolUpgrade > 0)
                    {
                        if (!lazer && zz == z - 20) { AddCheck(PosToInt((ushort)x, (ushort)y, (ushort)zz)); break; }
                    }
                    else
                    {
                        if (!lazer && zz == z - 10) { AddCheck(PosToInt((ushort)x, (ushort)y, (ushort)zz)); break; }
                    }
                    b = GetTile((ushort)x, (ushort)y, (ushort)zz);
                    if ((Block.Convert(b) == Block.air && !lazer) || (!lazer && p.pistolUpgrade > 1 && !Block.OPBlocks(b)))
                    {
                        p.killingPeople = true;
                        Player pl = Server.pctf.killedPlayer(p, (ushort)x, (ushort)y, (ushort)zz, false, "pistol");
                        if (pl != null) Server.killed.Add(pl);
                        ushort bb = GetTile((ushort)x, (ushort)y, (ushort)zz);
                        if (!Block.OPBlocks(bb))
                        {
                            AddUpdate(PosToInt((ushort)x, (ushort)y, (ushort)zz), Block.tntexplosion, lazer);
                        }
                    }
                    else if (lazer && p.lazerUpgrade > 1)
                    {
                        p.killingPeople = true;
                        Player pl = Server.pctf.killedPlayer(p, (ushort)x, (ushort)y, (ushort)zz, false, "lazer");
                        if (pl != null) Server.killed.Add(pl);
                        ushort bb = GetTile((ushort)x, (ushort)y, (ushort)zz);
                        if (!Block.OPBlocks(bb))
                        {
                            AddUpdate(PosToInt((ushort)x, (ushort)y, (ushort)zz), Block.tntexplosion, lazer);
                        }
                    }
                    else if (lazer && !Block.OPBlocks(b))
                    {
                        p.killingPeople = true;
                        Player pl = Server.pctf.killedPlayer(p, (ushort)x, (ushort)y, (ushort)zz, false, "lazer");
                        if (pl != null) Server.killed.Add(pl);
                        ushort bb = GetTile((ushort)x, (ushort)y, (ushort)zz);
                        if (!Block.OPBlocks(bb))
                        {
                            AddUpdate(PosToInt((ushort)x, (ushort)y, (ushort)zz), Block.tntexplosion, lazer);
                        }
                    }
                    else
                    {
                        AddCheck(PosToInt((ushort)x, (ushort)y, (ushort)zz));
                        break;
                    }
                }
                p.killingPeople = false;
            }
            else if (rot360 >= 48 && rot360 <= 142)
            {
                for (int xx = x; xx < length; xx++)
                {
                    if (p.pistolUpgrade > 2)
                    {
                        if (!lazer && xx == x - 30) { AddCheck(PosToInt((ushort)xx, (ushort)y, (ushort)z)); break; }
                    }
                    else if (p.pistolUpgrade > 0)
                    {
                        if (!lazer && xx == x + 20) { AddCheck(PosToInt((ushort)xx, (ushort)y, (ushort)z)); break; }
                    }
                    else
                    {
                        if (!lazer && xx == x + 10) { AddCheck(PosToInt((ushort)xx, (ushort)y, (ushort)z)); break; }
                    }
                    b = GetTile((ushort)xx, (ushort)y, (ushort)z);
                    if ((Block.Convert(b) == Block.air && !lazer) || (!lazer && p.pistolUpgrade > 1 && !Block.OPBlocks(b)))
                    {
                        p.killingPeople = true;
                        Player pl = Server.pctf.killedPlayer(p, (ushort)xx, (ushort)y, (ushort)z, false, "pistol");
                        if (pl != null) Server.killed.Add(pl);
                        ushort bb = GetTile((ushort)xx, (ushort)y, (ushort)z);
                        if (!Block.OPBlocks(bb))
                        {
                            AddUpdate(PosToInt((ushort)xx, (ushort)y, (ushort)z), Block.tntexplosion, lazer);
                        }
                    }
                    else if (lazer && p.lazerUpgrade > 1)
                    {
                        p.killingPeople = true;
                        Player pl = Server.pctf.killedPlayer(p, (ushort)xx, (ushort)y, (ushort)z, false, "lazer");
                        if (pl != null) Server.killed.Add(pl);
                        ushort bb = GetTile((ushort)xx, (ushort)y, (ushort)z);
                        if (!Block.OPBlocks(bb))
                        {
                            AddUpdate(PosToInt((ushort)xx, (ushort)y, (ushort)z), Block.tntexplosion, lazer);
                        }
                    }
                    else if (lazer && !Block.OPBlocks(b))
                    {
                        p.killingPeople = true;
                        Player pl = Server.pctf.killedPlayer(p, (ushort)xx, (ushort)y, (ushort)z, false, "lazer");
                        if (pl != null) Server.killed.Add(pl);
                        ushort bb = GetTile((ushort)xx, (ushort)y, (ushort)z);
                        if (!Block.OPBlocks(bb))
                        {
                            AddUpdate(PosToInt((ushort)xx, (ushort)y, (ushort)z), Block.tntexplosion, lazer);
                        }
                    }
                    else if (lazer && xx == length)
                    {
                        AddCheck(PosToInt((ushort)xx, (ushort)y, (ushort)z));
                        break;
                    }
                    else
                    {
                        AddCheck(PosToInt((ushort)xx, (ushort)y, (ushort)z));
                        break;
                    }
                }
                p.killingPeople = false;
            }
            else if (rot360 >= 141 && rot360 <= 217)
            {
                for (int zz = z; zz <= length; zz++)
                {
                    if (p.pistolUpgrade > 2)
                    {
                        if (!lazer && zz == z + 30) { AddCheck(PosToInt((ushort)x, (ushort)y, (ushort)zz)); break; }
                    }
                    else if (p.pistolUpgrade > 0)
                    {
                        if (!lazer && zz == z + 20) { AddCheck(PosToInt((ushort)x, (ushort)y, (ushort)zz)); break; }
                    }
                    else
                    {
                        if (!lazer && zz == z + 10) { AddCheck(PosToInt((ushort)x, (ushort)y, (ushort)zz)); break; }
                    }
                    b = GetTile((ushort)x, (ushort)y, (ushort)zz);
                    if ((Block.Convert(b) == Block.air && !lazer) || (!lazer && p.pistolUpgrade > 1 && !Block.OPBlocks(b)))
                    {
                        p.killingPeople = true;
                        Player pl = Server.pctf.killedPlayer(p, (ushort)x, (ushort)y, (ushort)zz, false, "pistol");
                        if (pl != null) Server.killed.Add(pl);
                        ushort bb = GetTile((ushort)x, (ushort)y, (ushort)zz);
                        if (!Block.OPBlocks(bb))
                        {
                            AddUpdate(PosToInt((ushort)x, (ushort)y, (ushort)zz), Block.tntexplosion, lazer);
                        }
                    }
                    else if (lazer && p.lazerUpgrade > 1)
                    {
                        p.killingPeople = true;
                        Player pl = Server.pctf.killedPlayer(p, (ushort)x, (ushort)y, (ushort)zz, false, "lazer");
                        if (pl != null) Server.killed.Add(pl);
                        ushort bb = GetTile((ushort)x, (ushort)y, (ushort)zz);
                        if (!Block.OPBlocks(bb))
                        {
                            AddUpdate(PosToInt((ushort)x, (ushort)y, (ushort)zz), Block.tntexplosion, lazer);
                        }
                    }
                    else if (lazer && !Block.OPBlocks(b))
                    {
                        p.killingPeople = true;
                        Player pl = Server.pctf.killedPlayer(p, (ushort)x, (ushort)y, (ushort)zz, false, "lazer");
                        if (pl != null) Server.killed.Add(pl);
                        ushort bb = GetTile((ushort)x, (ushort)y, (ushort)zz);
                        if (!Block.OPBlocks(bb))
                        {
                            AddUpdate(PosToInt((ushort)x, (ushort)y, (ushort)zz), Block.tntexplosion, lazer);
                        }
                    }
                    else
                    {
                        AddCheck(PosToInt((ushort)x, (ushort)y, (ushort)zz));
                        break;
                    }
                }
                p.killingPeople = false;
                // p.amountKilled = 0;
            }
        }

        public void makeFreezeRay(string name, ushort x, ushort y, ushort z, int width)
        {
            Player p = Player.Find(name);
            Server.killed.Clear();
            ushort b; int length = 0; int length2 = 0;
            length = p.level.width;
            length2 = p.level.width - p.level.width - p.level.width;

            int num = p.rot[0];
            int rot360 = (int)Math.Round(p.rot[0] * 1.40625f);
            if (rot360 >= 218 && rot360 <= 319)
            {
                //north
                for (int xx = x; xx >= length2; xx--)
                {
                    b = GetTile((ushort)xx, (ushort)y, (ushort)z);
                    if (Block.Convert(b) == Block.air)
                    {
                        p.killingPeople = true;
                        p.freezePlayer((ushort)xx, (ushort)y, (ushort)z);
                        ushort bb = GetTile((ushort)xx, (ushort)y, (ushort)z);
                        if (!Block.OPBlocks(bb))
                        {
                            AddUpdate(PosToInt((ushort)xx, (ushort)y, (ushort)z), Block.WaterFaucet, false);
                        }
                    }
                    else
                    {
                        AddCheck(PosToInt((ushort)xx, (ushort)y, (ushort)z));
                        break;
                    }
                }
                p.killingPeople = false;
            }
            else if ((rot360 >= 320 && rot360 <= 360) || (rot360 >= 0 && rot360 <= 47))
            {
                //east
                for (int zz = z; zz >= length2; zz--)
                {
                    b = GetTile((ushort)x, (ushort)y, (ushort)zz);
                    if (Block.Convert(b) == Block.air)
                    {
                        p.killingPeople = true;
                        p.freezePlayer((ushort)x, (ushort)y, (ushort)zz);
                        ushort bb = GetTile((ushort)x, (ushort)y, (ushort)zz);
                        if (!Block.OPBlocks(bb))
                        {
                            AddUpdate(PosToInt((ushort)x, (ushort)y, (ushort)zz), Block.WaterFaucet, false);
                        }
                    }
                    else
                    {
                        AddCheck(PosToInt((ushort)x, (ushort)y, (ushort)zz));
                        break;
                    }
                }
                p.killingPeople = false;
            }
            else if (rot360 >= 48 && rot360 <= 142)
            {
                for (int xx = x; xx < length; xx++)
                {
                    b = GetTile((ushort)xx, (ushort)y, (ushort)z);
                    if (Block.Convert(b) == Block.air)
                    {
                        p.killingPeople = true;
                        p.freezePlayer((ushort)xx, (ushort)y, (ushort)z);
                        ushort bb = GetTile((ushort)xx, (ushort)y, (ushort)z);
                        if (!Block.OPBlocks(bb))
                        {
                            AddUpdate(PosToInt((ushort)xx, (ushort)y, (ushort)z), Block.WaterFaucet, false);
                        }
                    }
                    else if (xx == length)
                    {
                        AddCheck(PosToInt((ushort)xx, (ushort)y, (ushort)z));
                        break;
                    }
                    else
                    {
                        AddCheck(PosToInt((ushort)xx, (ushort)y, (ushort)z));
                        break;
                    }
                }
                p.killingPeople = false;
            }
            else if (rot360 >= 141 && rot360 <= 217)
            {
                for (int zz = z; zz <= length; zz++)
                {
                    b = GetTile((ushort)x, (ushort)y, (ushort)zz);
                    if (Block.Convert(b) == Block.air)
                    {
                        p.killingPeople = true;
                        p.freezePlayer((ushort)x, (ushort)y, (ushort)zz);
                        ushort bb = GetTile((ushort)x, (ushort)y, (ushort)zz);
                        if (!Block.OPBlocks(bb))
                        {
                            AddUpdate(PosToInt((ushort)x, (ushort)y, (ushort)zz), Block.WaterFaucet, false);
                        }
                    }
                    else
                    {
                        AddCheck(PosToInt((ushort)x, (ushort)y, (ushort)zz));
                        break;
                    }
                }
                p.killingPeople = false;
                // p.amountKilled = 0;
            }
        }

        public void makeLine(string name, ushort x, ushort y, ushort z, int width, bool lazer)
        {
            Player p = Player.Find(name);
            Server.killed.Clear();
            ushort b; int length = 0; int length2 = 0;
            length = p.level.width;
            length2 = p.level.width - p.level.width - p.level.width;
            lazer = false;

            int num = p.rot[0];
            int rot360 = (int)Math.Round(p.rot[0] * 1.40625f);
            if (rot360 >= 218 && rot360 <= 319)
            {
                //north
                for (int xx = x; xx >= length2; xx--)
                {
                    b = GetTile((ushort)xx, (ushort)y, (ushort)z);
                    if (Block.Convert(b) == Block.air && !lazer)
                    {
                        ushort bb = GetTile((ushort)xx, (ushort)y, (ushort)z);
                        if (!Block.OPBlocks(bb))
                        {
                            AddUpdate(PosToInt((ushort)xx, (ushort)y, (ushort)z), Block.stone, lazer);
                        }
                    }
                    else if (lazer && !Block.OPBlocks(b))
                    {
                        ushort bb = GetTile((ushort)xx, (ushort)y, (ushort)z);
                        if (!Block.OPBlocks(bb))
                        {
                            AddUpdate(PosToInt((ushort)xx, (ushort)y, (ushort)z), Block.stone, lazer);
                        }
                    }
                    else
                    {
                        AddCheck(PosToInt((ushort)xx, (ushort)y, (ushort)z));
                        break;
                    }
                }
            }
            else if ((rot360 >= 320 && rot360 <= 360) || (rot360 >= 0 && rot360 <= 47))
            {
                //east
                for (int zz = z; zz >= length2; zz--)
                {
                    b = GetTile((ushort)x, (ushort)y, (ushort)zz);
                    if (Block.Convert(b) == Block.air && !lazer)
                    {
                        ushort bb = GetTile((ushort)x, (ushort)y, (ushort)zz);
                        if (!Block.OPBlocks(bb))
                        {
                            AddUpdate(PosToInt((ushort)x, (ushort)y, (ushort)zz), Block.stone, lazer);
                        }
                    }
                    else if (lazer && !Block.OPBlocks(b))
                    {
                        ushort bb = GetTile((ushort)x, (ushort)y, (ushort)zz);
                        if (!Block.OPBlocks(bb))
                        {
                            AddUpdate(PosToInt((ushort)x, (ushort)y, (ushort)zz), Block.stone, lazer);
                        }
                    }
                    else
                    {
                        AddCheck(PosToInt((ushort)x, (ushort)y, (ushort)zz));
                        break;
                    }
                }
            }
            else if (rot360 >= 48 && rot360 <= 142)
            {
                for (int xx = x; xx < length; xx++)
                {
                    b = GetTile((ushort)xx, (ushort)y, (ushort)z);
                    if (Block.Convert(b) == Block.air && !lazer)
                    {
                        ushort bb = GetTile((ushort)xx, (ushort)y, (ushort)z);
                        if (!Block.OPBlocks(bb))
                        {
                            AddUpdate(PosToInt((ushort)xx, (ushort)y, (ushort)z), Block.stone, lazer);
                        }
                    }
                    else if (lazer && !Block.OPBlocks(b))
                    {
                        ushort bb = GetTile((ushort)xx, (ushort)y, (ushort)z);
                        if (!Block.OPBlocks(bb))
                        {
                            AddUpdate(PosToInt((ushort)xx, (ushort)y, (ushort)z), Block.stone, lazer);
                        }
                    }
                    else if (lazer && xx == length)
                    {
                        AddCheck(PosToInt((ushort)xx, (ushort)y, (ushort)z));
                        break;
                    }
                    else
                    {
                        AddCheck(PosToInt((ushort)xx, (ushort)y, (ushort)z));
                        break;
                    }
                }
            }
            else if (rot360 >= 141 && rot360 <= 217)
            {
                for (int zz = z; zz <= length; zz++)
                {
                    b = GetTile((ushort)x, (ushort)y, (ushort)zz);
                    if (Block.Convert(b) == Block.air && !lazer)
                    {
                        ushort bb = GetTile((ushort)x, (ushort)y, (ushort)zz);
                        if (!Block.OPBlocks(bb))
                        {
                            AddUpdate(PosToInt((ushort)x, (ushort)y, (ushort)zz), Block.stone, lazer);
                        }
                    }
                    else if (lazer && !Block.OPBlocks(b))
                    {
                        ushort bb = GetTile((ushort)x, (ushort)y, (ushort)zz);
                        if (!Block.OPBlocks(bb))
                        {
                            AddUpdate(PosToInt((ushort)x, (ushort)y, (ushort)zz), Block.stone, lazer);
                        }
                    }
                    else
                    {
                        AddCheck(PosToInt((ushort)x, (ushort)y, (ushort)zz));
                        break;
                    }
                }
            }
        }

        public void makeLightningsplosion(string name, ushort x, ushort y, ushort z, int width)
        {
            Player p = Player.Find(name);
            Server.killed.Clear();
            ushort b; int length2 = 0;
            length2 = p.level.width - p.level.width - p.level.width;

            for (int yy = y; yy >= length2; yy++)
            {
                b = GetTile((ushort)x, (ushort)yy, (ushort)z);
                if (!Block.OPBlocks(b))
                {
                    p.killingPeople = true;
                    Player pl = Server.pctf.killedPlayer(p, (ushort)x, (ushort)yy, (ushort)z, false, "lightning");
                    if (pl != null) Server.killed.Add(pl);
                    ushort bb = GetTile((ushort)x, (ushort)yy, (ushort)z);
                    if (!Block.OPBlocks(bb))
                    {
                        AddUpdate(PosToInt((ushort)x, (ushort)yy, (ushort)z), Block.tntexplosion, true);
                    }
                }
                else
                {
                    AddCheck(PosToInt((ushort)x, (ushort)yy, (ushort)z));
                    break;
                }
            }
            p.killingPeople = false;
        }

        public void makeUpsideDownLightning(string name, ushort x, ushort y, ushort z, int width)
        {
            Player p = Player.Find(name);
            Server.killed.Clear();
            ushort b; int length2 = 0;
            length2 = 0;

            for (int yy = y; yy >= length2; yy--)
            {
                b = GetTile((ushort)x, (ushort)yy, (ushort)z);
                if (!Block.OPBlocks(b))
                {
                    p.killingPeople = true;
                    Player pl = Server.pctf.killedPlayer(p, (ushort)x, (ushort)yy, (ushort)z, false, "lightning");
                    if (pl != null) Server.killed.Add(pl);
                    ushort bb = GetTile((ushort)x, (ushort)yy, (ushort)z);
                    if (!Block.OPBlocks(bb))
                    {
                        AddUpdate(PosToInt((ushort)x, (ushort)yy, (ushort)z), Block.tntexplosion, true);
                    }
                }
                else
                {
                    AddCheck(PosToInt((ushort)x, (ushort)yy, (ushort)z));
                    break;
                }
            }
            p.killingPeople = false;
        }

        public void Firework(ushort x, ushort y, ushort z, int size)
        {
            ushort xx, yy, zz;
            var rand = new Random();
            int storedRand1, storedRand2;

            if (physics < 1) return;
            if (physics == 5) return;
            storedRand1 = rand.Next(21, 36);
            storedRand2 = rand.Next(21, 36);
            // Not using override, since override = true makes it more likely that a colored block will be generated with no extraInfo, because it sets a Check for that position with no extraInfo.
            AddUpdate(PosToInt(x, y, z), Block.air);

            for (xx = (ushort)(x - (size + 1)); xx <= (ushort)(x + (size + 1)); ++xx)
                for (yy = (ushort)(y - (size + 1)); yy <= (ushort)(y + (size + 1)); ++yy)
                    for (zz = (ushort)(z - (size + 1)); zz <= (ushort)(z + (size + 1)); ++zz)
                        if (GetTile(xx, yy, zz) == Block.air)
                            if (rand.Next(1, 40) < 2)
                                AddUpdate(PosToInt(xx, yy, zz),
                                          (byte)
                                          rand.Next(Math.Min(storedRand1, storedRand2),
                                                    Math.Max(storedRand1, storedRand2)), false, "drop 100 dissipate 25");
        }

        public void finiteMovement(Check C, ushort x, ushort y, ushort z)
        {
            var rand = new Random();

            var bufferfiniteWater = new List<int>();
            var bufferfiniteWaterList = new List<Pos>();

            if (GetTile(x, (ushort)(y - 1), z) == Block.air)
            {
                AddUpdate(PosToInt(x, (ushort)(y - 1), z), blocks[C.b], false, C.extraInfo);
                AddUpdate(C.b, Block.air);
                C.extraInfo = "";
            }
            else if (GetTile(x, (ushort)(y - 1), z) == Block.waterstill ||
                     GetTile(x, (ushort)(y - 1), z) == Block.lavastill)
            {
                AddUpdate(C.b, Block.air);
                C.extraInfo = "";
            }
            else
            {
                for (int i = 0; i < 25; ++i) bufferfiniteWater.Add(i);

                for (int k = bufferfiniteWater.Count - 1; k > 1; --k)
                {
                    int randIndx = rand.Next(k); //
                    int temp = bufferfiniteWater[k];
                    bufferfiniteWater[k] = bufferfiniteWater[randIndx]; // move random num to end of list.
                    bufferfiniteWater[randIndx] = temp;
                }

                Pos pos;

                for (var xx = (ushort)(x - 2); xx <= x + 2; ++xx)
                {
                    for (var zz = (ushort)(z - 2); zz <= z + 2; ++zz)
                    {
                        pos.x = xx;
                        pos.z = zz;
                        bufferfiniteWaterList.Add(pos);
                    }
                }

                foreach (int i in bufferfiniteWater)
                {
                    pos = bufferfiniteWaterList[i];
                    if (GetTile(pos.x, (ushort)(y - 1), pos.z) == Block.air&&
                        GetTile(pos.x, y, pos.z) == Block.air)
                    {
                        if (pos.x < x) pos.x = (ushort)(Math.Floor((double)(pos.x + x) / 2));
                        else pos.x = (ushort)(Math.Ceiling((double)(pos.x + x) / 2));
                        if (pos.z < z) pos.z = (ushort)(Math.Floor((double)(pos.z + z) / 2));
                        else pos.z = (ushort)(Math.Ceiling((double)(pos.z + z) / 2));

                        if (GetTile(pos.x, y, pos.z) == Block.air)
                        {
                            if (AddUpdate(PosToInt(pos.x, y, pos.z), blocks[C.b], false, C.extraInfo))
                            {
                                AddUpdate(C.b, Block.air);
                                C.extraInfo = "";
                                break;
                            }
                        }
                    }
                }
            }
        }

        public struct Pos
        {
            public ushort x, z;
        }

        #endregion

        #region Nested type: BlockPos

        public struct BlockPos
        {
            public DateTime TimePerformed;
            public bool deleted;
            public string name;
            public ushort type;
            public ushort x, y, z;
        }

        #endregion

        #region Nested type: UndoPos

        public struct UndoPos
        {
            public int location;
            public ushort newType;
            public ushort oldType;
            public DateTime timePerformed;
        }

        #endregion

        #region Nested type: Zone

        public struct Zone
        {
            public string Owner;
            public ushort bigX, bigY, bigZ;
            public ushort smallX, smallY, smallZ;
        }

        #endregion
    }
}

//-------------------------------------------------------------------------------------------------------------------------------------------------------
public class Check
{
    public int b;
    public string extraInfo = "";
    public byte time;
    public MCForge.Player p;

    public Check(int b, string extraInfo = "", MCForge.Player placer = null)
    {
        this.b = b;
        time = 0;
        this.extraInfo = extraInfo;
        p = placer;
    }
}

//-------------------------------------------------------------------------------------------------------------------------------------------------------
public class Update
{
    public int b;
    public string extraInfo = "";
    public ushort type;

    public Update(int b, ushort type, string extraInfo = "")
    {
        this.b = b;
        this.type = type;
        this.extraInfo = extraInfo;
    }
}
