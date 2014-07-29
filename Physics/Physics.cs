using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading;

namespace MCForge
{
    public class Physics
    {
        public readonly Dictionary<int, sbyte> leaves = new Dictionary<int, sbyte>();
        // Holds block state for leaf decay
        public delegate void OnPhysicsUpdate(ushort x, ushort y, ushort z, byte time, string extraInfo, Level l);
        public delegate void OnPhysicsStateChanged(object sender, PhysicsState state);
        public static event OnPhysicsStateChanged PhysicsStateChanged;
        public event OnPhysicsUpdate PhysicsUpdate = null;
        public static bool cancelphysics;
        public bool physPause;
        public DateTime physResume;
        public Thread physThread;
        public System.Timers.Timer physTimer = new System.Timers.Timer(1000);
        public readonly Dictionary<int, bool[]> liquids = new Dictionary<int, bool[]>();
        // Holds random flow data for liqiud physics
        public bool physicssate = false;
        public ExtrasCollection Extras = new ExtrasCollection();
        private readonly object physThreadLock = new object();
        public void Physic(Level level)
        {
            int wait = level.speedphysics;
            while (true)
            {

                if (!level.PhysicsEnabled)
                {
                    Thread.Sleep(500);
                    continue;
                }

                try
                {
                    if (wait > 0) Thread.Sleep(wait);
                    if (level.physics == 0 || level.ListCheck.Count == 0)
                    {
                        level.lastCheck = 0;
                        wait = level.speedphysics;
                        if (level.physics == 0) break;
                        continue;
                    }

                    DateTime Start = DateTime.Now;

                    if (level.physics > 0) CalcPhysics(level);

                    TimeSpan Took = DateTime.Now - Start;
                    wait = level.speedphysics - (int)Took.TotalMilliseconds;
                    if (wait < (int)(-level.overload * 0.75f))
                    {
                        Level Cause = level;

                        if (wait < -level.overload)
                        {
                            if (!Server.physicsRestart) Cause.setPhysics(0);
                            Cause.physic.ClearPhysics(level);

                            Player.GlobalMessage("level.physics shutdown on &b" + Cause.name);
                            Server.s.Log("level.physics shutdown on " + level.name);
                            if (PhysicsStateChanged != null)
                                PhysicsStateChanged(level, PhysicsState.Stopped);

                            wait = level.speedphysics;
                        }
                        else
                        {
                            foreach (Player p in Player.players.Where(p => p.level == level))
                            {
                                Player.SendMessage(p, "Physics warning!");
                            }
                            Server.s.Log("level.physics warning on " + level.name);

                            if (PhysicsStateChanged != null)
                                PhysicsStateChanged(level, PhysicsState.Warning);
                        }
                    }
                }
                catch
                {
                    wait = level.speedphysics;
                }
            }
            physicssate = false;
            physThread.Abort();
        }
        public void StartPhysics(Level level)
        {
            lock (physThreadLock)
            {
                if (physThread != null)
                {
                    if (physThread.ThreadState == System.Threading.ThreadState.Running)
                        return;
                }
                if (level.ListCheck.Count == 0 || physicssate)
                    return;
                physThread = new Thread(() => Physic(level));
                level.PhysicsEnabled = true;
                physThread.Start();
                physicssate = true;
            }
        }

        public void CalcPhysics(Level level)
        {
            try
            {
                if (level.physics == 5)
                {
                    #region == INCOMING ==

                    ushort x, y, z;

                    level.lastCheck = level.ListCheck.Count;
                    level.ListCheck.ForEach(delegate(Check C)
                    {
                        try
                        {
                            level.IntToPos(C.b, out x, out y, out z);
                            string foundInfo = C.extraInfo;
                            if (PhysicsUpdate != null)
                            {
                                PhysicsUpdate(x, y, z, C.time, C.extraInfo, level);
                            }
                        newPhysic:
                            if (foundInfo != "")
                            {
                                int currentLoop = 0;
                                if (!foundInfo.Contains("wait"))
                                    if (level.blocks[C.b] == Block.air) C.extraInfo = "";

                                bool wait = false;
                                int waitnum = 0;
                                bool door = false;

                                foreach (string s in C.extraInfo.Split(' '))
                                {
                                    if (currentLoop % 2 == 0)
                                    {
                                        //Type of code
                                        switch (s)
                                        {
                                            case "wait":
                                                wait = true;
                                                waitnum =
                                                    int.Parse(
                                                        C.extraInfo.Split(' ')[currentLoop + 1]);
                                                break;
                                            case "door":
                                                door = true;
                                                break;
                                        }
                                    }
                                    currentLoop++;
                                }

                            startCheck:
                                if (wait)
                                {
                                    int storedInt = 0;
                                    if (door && C.time < 2)
                                    {
                                        storedInt = level.IntOffset(C.b, -1, 0, 0);
                                        if (Block.tDoor(level.blocks[storedInt]))
                                        {
                                            level.AddUpdate(storedInt, Block.air, false,
                                                      "wait 10 door 1 revert " +
                                                      level.blocks[storedInt].ToString());
                                        }
                                        storedInt = level.IntOffset(C.b, 1, 0, 0);
                                        if (Block.tDoor(level.blocks[storedInt]))
                                        {
                                            level.AddUpdate(storedInt, Block.air, false,
                                                      "wait 10 door 1 revert " +
                                                      level.blocks[storedInt].ToString());
                                        }
                                        storedInt = level.IntOffset(C.b, 0, 1, 0);
                                        if (Block.tDoor(level.blocks[storedInt]))
                                        {
                                            level.AddUpdate(storedInt, Block.air, false,
                                                      "wait 10 door 1 revert " +
                                                      level.blocks[storedInt].ToString());
                                        }
                                        storedInt = level.IntOffset(C.b, 0, -1, 0);
                                        if (Block.tDoor(level.blocks[storedInt]))
                                        {
                                            level.AddUpdate(storedInt, Block.air, false,
                                                      "wait 10 door 1 revert " +
                                                      level.blocks[storedInt].ToString());
                                        }
                                        storedInt = level.IntOffset(C.b, 0, 0, 1);
                                        if (Block.tDoor(level.blocks[storedInt]))
                                        {
                                            level.AddUpdate(storedInt, Block.air, false,
                                                      "wait 10 door 1 revert " +
                                                      level.blocks[storedInt].ToString());
                                        }
                                        storedInt = level.IntOffset(C.b, 0, 0, -1);
                                        if (Block.tDoor(level.blocks[storedInt]))
                                        {
                                            level.AddUpdate(storedInt, Block.air, false,
                                                      "wait 10 door 1 revert " +
                                                      level.blocks[storedInt].ToString());
                                        }
                                    }

                                    if (waitnum <= C.time)
                                    {
                                        wait = false;
                                        C.extraInfo =
                                            C.extraInfo.Substring(0, C.extraInfo.IndexOf("wait ")) +
                                            C.extraInfo.Substring(
                                                C.extraInfo.IndexOf(' ',
                                                                    C.extraInfo.IndexOf("wait ") +
                                                                    5) + 1);
                                        //C.extraInfo = C.extraInfo.Substring(8);
                                        goto startCheck;
                                    }
                                    C.time++;
                                    foundInfo = "";
                                    goto newPhysic;
                                }
                            }
                            else
                            {
                                switch (level.blocks[C.b])
                                {
                                    case Block.door_air:
                                    //door_air         Change any door blocks nearby into door_air
                                    case Block.door2_air:
                                    //door_air         Change any door blocks nearby into door_air
                                    case Block.door3_air:
                                    //door_air         Change any door blocks nearby into door_air
                                    case Block.door4_air:
                                    //door_air         Change any door blocks nearby into door_air
                                    case Block.door5_air:
                                    //door_air         Change any door blocks nearby into door_air
                                    case Block.door6_air:
                                    //door_air         Change any door blocks nearby into door_air
                                    case Block.door7_air:
                                    //door_air         Change any door blocks nearby into door_air
                                    case Block.door8_air:
                                    //door_air         Change any door blocks nearby into door_air
                                    case Block.door10_air:
                                    //door_air         Change any door blocks nearby into door_air
                                    case Block.door12_air:
                                    case Block.door13_air:
                                    case Block.door_iron_air:
                                    case Block.door_gold_air:
                                    case Block.door_cobblestone_air:
                                    case Block.door_red_air:



                                    case Block.door_dirt_air:
                                    case Block.door_grass_air:
                                    case Block.door_blue_air:
                                    case Block.door_book_air:
                                        level.AnyDoor(C, x, y, z, 16);
                                        break;
                                    case Block.door11_air:
                                    case Block.door14_air:
                                        level.AnyDoor(C, x, y, z, 4, true);
                                        break;
                                    case Block.door9_air:
                                        //door_air         Change any door blocks nearby into door_air
                                        level.AnyDoor(C, x, y, z, 4);
                                        break;

                                    case Block.odoor1_air:
                                    case Block.odoor2_air:
                                    case Block.odoor3_air:
                                    case Block.odoor4_air:
                                    case Block.odoor5_air:
                                    case Block.odoor6_air:
                                    case Block.odoor7_air:
                                    case Block.odoor8_air:
                                    case Block.odoor9_air:
                                    case Block.odoor10_air:
                                    case Block.odoor11_air:
                                    case Block.odoor12_air:

                                    case Block.odoor1:
                                    case Block.odoor2:
                                    case Block.odoor3:
                                    case Block.odoor4:
                                    case Block.odoor5:
                                    case Block.odoor6:
                                    case Block.odoor7:
                                    case Block.odoor8:
                                    case Block.odoor9:
                                    case Block.odoor10:
                                    case Block.odoor11:
                                    case Block.odoor12:
                                        level.odoor(C);
                                        break;
                                    default:
                                        //non special blocks are then ignored, maybe it would be better to avoid getting here and cutting down the list
                                        if (!C.extraInfo.Contains("wait")) C.time = 255;
                                        break;
                                }
                            }
                        }
                        catch
                        {
                            level.ListCheck.Remove(C);
                            //Server.s.Log(e.Message);
                        }
                    });

                    level.ListCheck.RemoveAll(delegate(Check Check) { return Check.time == 255; }); //Remove all that are finished with 255 time

                    level.lastUpdate = level.ListUpdate.Count;
                    level.ListUpdate.ForEach(delegate(Update C)
                    {
                        try
                        {
                            level.IntToPos(C.b, out x, out y, out z);
                            level.Blockchange(x, y, z, C.type, false, C.extraInfo);
                        }
                        catch
                        {
                            Server.s.Log("Phys update issue");
                        }
                    });

                    level.ListUpdate.Clear();

                    #endregion

                    return;
                }
                if (level.physics > 0)
                {
                    #region == INCOMING ==

                    ushort x, y, z;
                    int mx, my, mz;

                    var rand = new Random();
                    level.lastCheck = level.ListCheck.Count;
                    level.ListCheck.ForEach(delegate(Check C)
                    {
                        try
                        {
                            level.IntToPos(C.b, out x, out y, out z);
                            bool InnerChange = false;
                            bool skip = false;
                            int storedRand = 0;
                            Player foundPlayer = null;
                            int foundNum = 75, currentNum;
                            int oldNum;
                            string foundInfo = C.extraInfo;
                            if (PhysicsUpdate != null)
                                PhysicsUpdate(x, y, z, C.time, C.extraInfo, level);
                            OnPhysicsUpdateEvent.Call(x, y, z, C.time, C.extraInfo, level);
                        newPhysic:
                            if (foundInfo != "")
                            {
                                int currentLoop = 0;
                                if (!foundInfo.Contains("wait"))
                                    if (level.blocks[C.b] == Block.air) C.extraInfo = "";

                                bool drop = false;
                                int dropnum = 0;
                                bool wait = false;
                                int waitnum = 0;
                                bool dissipate = false;
                                int dissipatenum = 0;
                                bool revert = false;
                                byte reverttype = 0;
                                bool explode = false;
                                int explodenum = 0;
                                bool finiteWater = false;
                                bool rainbow = false;
                                int rainbownum = 0;
                                bool door = false;

                                foreach (string s in C.extraInfo.Split(' '))
                                {
                                    if (currentLoop % 2 == 0)
                                    {
                                        //Type of code
                                        switch (s)
                                        {
                                            case "wait":
                                                wait = true;
                                                waitnum =
                                                    int.Parse(
                                                        C.extraInfo.Split(' ')[currentLoop + 1]);
                                                break;
                                            case "drop":
                                                drop = true;
                                                dropnum =
                                                    int.Parse(
                                                        C.extraInfo.Split(' ')[currentLoop + 1]);
                                                break;
                                            case "dissipate":
                                                dissipate = true;
                                                dissipatenum =
                                                    int.Parse(
                                                        C.extraInfo.Split(' ')[currentLoop + 1]);
                                                break;
                                            case "revert":
                                                revert = true;
                                                reverttype =
                                                    byte.Parse(
                                                        C.extraInfo.Split(' ')[currentLoop + 1]);
                                                break;
                                            case "explode":
                                                explode = true;
                                                explodenum =
                                                    int.Parse(
                                                        C.extraInfo.Split(' ')[currentLoop + 1]);
                                                break;

                                            case "finite":
                                                finiteWater = true;
                                                break;

                                            case "rainbow":
                                                rainbow = true;
                                                rainbownum =
                                                    int.Parse(
                                                        C.extraInfo.Split(' ')[currentLoop + 1]);
                                                break;

                                            case "door":
                                                door = true;
                                                break;
                                        }
                                    }
                                    currentLoop++;
                                }

                            startCheck:
                                if (wait)
                                {
                                    int storedInt = 0;
                                    if (door && C.time < 2)
                                    {
                                        storedInt = level.IntOffset(C.b, -1, 0, 0);
                                        if (Block.tDoor(level.blocks[storedInt]))
                                        {
                                            level.AddUpdate(storedInt, Block.air, false,
                                                      "wait 10 door 1 revert " +
                                                      level.blocks[storedInt].ToString());
                                        }
                                        storedInt = level.IntOffset(C.b, 1, 0, 0);
                                        if (Block.tDoor(level.blocks[storedInt]))
                                        {
                                            level.AddUpdate(storedInt, Block.air, false,
                                                      "wait 10 door 1 revert " +
                                                      level.blocks[storedInt].ToString());
                                        }
                                        storedInt = level.IntOffset(C.b, 0, 1, 0);
                                        if (Block.tDoor(level.blocks[storedInt]))
                                        {
                                            level.AddUpdate(storedInt, Block.air, false,
                                                      "wait 10 door 1 revert " +
                                                      level.blocks[storedInt].ToString());
                                        }
                                        storedInt = level.IntOffset(C.b, 0, -1, 0);
                                        if (Block.tDoor(level.blocks[storedInt]))
                                        {
                                            level.AddUpdate(storedInt, Block.air, false,
                                                      "wait 10 door 1 revert " +
                                                      level.blocks[storedInt].ToString());
                                        }
                                        storedInt = level.IntOffset(C.b, 0, 0, 1);
                                        if (Block.tDoor(level.blocks[storedInt]))
                                        {
                                            level.AddUpdate(storedInt, Block.air, false,
                                                      "wait 10 door 1 revert " +
                                                      level.blocks[storedInt].ToString());
                                        }
                                        storedInt = level.IntOffset(C.b, 0, 0, -1);
                                        if (Block.tDoor(level.blocks[storedInt]))
                                        {
                                            level.AddUpdate(storedInt, Block.air, false,
                                                      "wait 10 door 1 revert " +
                                                      level.blocks[storedInt].ToString());
                                        }
                                    }

                                    if (waitnum <= C.time)
                                    {
                                        wait = false;
                                        C.extraInfo =
                                            C.extraInfo.Substring(0, C.extraInfo.IndexOf("wait ")) +
                                            C.extraInfo.Substring(
                                                C.extraInfo.IndexOf(' ',
                                                                    C.extraInfo.IndexOf("wait ") +
                                                                    5) + 1);
                                        //C.extraInfo = C.extraInfo.Substring(8);
                                        goto startCheck;
                                    }
                                    C.time++;
                                    foundInfo = "";
                                    goto newPhysic;
                                }
                                if (finiteWater)
                                    level.finiteMovement(C, x, y, z);
                                else if (rainbow)
                                    if (C.time < 4)
                                    {
                                        C.time++;
                                    }
                                    else
                                    {
                                        if (rainbownum > 2)
                                        {
                                            if (level.blocks[C.b] < Block.red ||
                                                level.blocks[C.b] > Block.darkpink)
                                            {
                                                level.AddUpdate(C.b, Block.red, true);
                                            }
                                            else
                                            {
                                                if (level.blocks[C.b] == Block.darkpink)
                                                    level.AddUpdate(C.b, Block.red);
                                                else level.AddUpdate(C.b, (ushort)(level.blocks[C.b] + 1));
                                            }
                                        }
                                        else
                                        {
                                            level.AddUpdate((int)C.b, (ushort)rand.Next(21, 33));
                                        }
                                    }
                                else
                                {
                                    if (revert)
                                    {
                                        level.AddUpdate(C.b, reverttype);
                                        C.extraInfo = "";
                                    }
                                    // Not setting drop = false can cause occasional leftover blocks, since C.extraInfo is emptied, so
                                    // drop can generate another block with no dissipate/explode information.
                                    if (dissipate)
                                        if (rand.Next(1, 100) <= dissipatenum)
                                        {
                                            if (!level.ListUpdate.Exists(Update => Update.b == C.b))
                                            {
                                                level.AddUpdate(C.b, Block.air);
                                                C.extraInfo = "";
                                                drop = false;
                                            }
                                            else
                                            {
                                                level.AddUpdate(C.b, level.blocks[C.b], false, C.extraInfo);
                                            }
                                        }
                                    if (explode)
                                        if (rand.Next(1, 100) <= explodenum)
                                        {
                                            level.MakeExplosion(x, y, z, 0);
                                            C.extraInfo = "";
                                            drop = false;
                                        }
                                    if (drop)
                                        if (rand.Next(1, 100) <= dropnum)
                                            if (level.GetTile(x, (ushort)(y - 1), z) == Block.air ||
                                                level.GetTile(x, (ushort)(y - 1), z) == Block.lava ||
                                                level.GetTile(x, (ushort)(y - 1), z) == Block.water)
                                            {
                                                if (rand.Next(1, 100) <
                                                    int.Parse(C.extraInfo.Split(' ')[1]))
                                                {
                                                    if (
                                                        level.AddUpdate(
                                                            level.PosToInt(x, (ushort)(y - 1), z),
                                                            level.blocks[C.b], false, C.extraInfo))
                                                    {
                                                        level.AddUpdate(C.b, Block.air);
                                                        C.extraInfo = "";
                                                    }
                                                }
                                            }
                                }
                            }
                            else
                            {
                                int newNum;
                                switch (level.blocks[C.b])
                                {
                                    case Block.air: //Placed air
                                        //initialy checks if block is valid
                                        Physair(level, level.PosToInt((ushort)(x + 1), y, z));
                                        Physair(level, level.PosToInt((ushort)(x - 1), y, z));
                                        Physair(level, level.PosToInt(x, y, (ushort)(z + 1)));
                                        Physair(level, level.PosToInt(x, y, (ushort)(z - 1)));
                                        Physair(level, level.PosToInt(x, (ushort)(y + 1), z));
                                        //Check block above the air
                                        Physair(level, level.PosToInt(x, (ushort)(y - 1), z));
                                        // Check block below the air

                                        //Edge of map water
                                        if (level.edgeWater)
                                        {
                                            if (y < level.depth / 2 && y >= (level.depth / 2) - 2)
                                            {
                                                if (x == 0 || x == level.width - 1 || z == 0 ||
                                                    z == level.height - 1)
                                                {
                                                    level.AddUpdate(C.b, Block.water);
                                                }
                                            }
                                        }

                                        if (!C.extraInfo.Contains("wait")) C.time = 255;
                                        break;

                                    case Block.dirt: //Dirt
                                        if (!level.GrassGrow)
                                        {
                                            C.time = 255;
                                            break;
                                        }

                                        if (C.time > 20)
                                        {
                                            if (Block.LightPass(level.GetTile(x, (ushort)(y + 1), z)))
                                            {
                                                level.AddUpdate(C.b, Block.grass);
                                            }
                                            C.time = 255;
                                        }
                                        else
                                        {
                                            C.time++;
                                        }
                                        break;

                                    case Block.leaf:
                                        if (level.physics > 1)
                                        //Adv level.physics kills flowers and mushroos in water/lava
                                        {
                                            Physair(level, level.PosToInt((ushort)(x + 1), y, z));
                                            Physair(level, level.PosToInt((ushort)(x - 1), y, z));
                                            Physair(level, level.PosToInt(x, y, (ushort)(z + 1)));
                                            Physair(level, level.PosToInt(x, y, (ushort)(z - 1)));
                                            Physair(level, level.PosToInt(x, (ushort)(y + 1), z));
                                            //Check block above
                                        }

                                        if (!level.leafDecay)
                                        {
                                            C.time = 255;
                                            leaves.Clear();
                                            break;
                                        }
                                        if (C.time < 5)
                                        {
                                            if (rand.Next(10) == 0) C.time++;
                                            break;
                                        }
                                        if (PhysLeaf(level, C.b)) level.AddUpdate(C.b, 0);
                                        C.time = 255;
                                        break;

                                    case Block.shrub:
                                        if (level.physics > 1)
                                        //Adv level.physics kills flowers and mushroos in water/lava
                                        {
                                            Physair(level, level.PosToInt((ushort)(x + 1), y, z));
                                            Physair(level, level.PosToInt((ushort)(x - 1), y, z));
                                            Physair(level, level.PosToInt(x, y, (ushort)(z + 1)));
                                            Physair(level, level.PosToInt(x, y, (ushort)(z - 1)));
                                            Physair(level, level.PosToInt(x, (ushort)(y + 1), z));
                                            //Check block above
                                        }

                                        if (!level.growTrees)
                                        {
                                            C.time = 255;
                                            break;
                                        }
                                        if (C.time < 20)
                                        {
                                            if (rand.Next(20) == 0) C.time++;
                                            break;
                                        }
                                        Server.MapGen.AddTree(level, x, y, z, rand, true, false);
                                        C.time = 255;
                                        break;

                                    case Block.water: //Active_water
                                    case Block.activedeathwater:
                                        //initialy checks if block is valid
                                        if (!level.finite)
                                        {
                                            if (level.randomFlow)
                                            {
                                                if (!PhysSpongeCheck(level, C.b))
                                                {
                                                    if (!liquids.ContainsKey(C.b))
                                                        liquids.Add(C.b, new bool[5]);

                                                    if (level.GetTile(x, (ushort)(y + 1), z) !=
                                                        Block.Zero)
                                                    {
                                                        PhysSandCheck(level, level.PosToInt(x, (ushort)(y + 1),
                                                                               z));
                                                    }
                                                    if (!liquids[C.b][0] && rand.Next(4) == 0)
                                                    {
                                                        PhysWater(level, 
                                                            level.PosToInt((ushort)(x + 1), y, z),
                                                            level.blocks[C.b]);
                                                        liquids[C.b][0] = true;
                                                    }
                                                    if (!liquids[C.b][1] && rand.Next(4) == 0)
                                                    {
                                                        PhysWater(level, 
                                                            level.PosToInt((ushort)(x - 1), y, z),
                                                            level.blocks[C.b]);
                                                        liquids[C.b][1] = true;
                                                    }
                                                    if (!liquids[C.b][2] && rand.Next(4) == 0)
                                                    {
                                                        PhysWater(level, 
                                                            level.PosToInt(x, y, (ushort)(z + 1)),
                                                            level.blocks[C.b]);
                                                        liquids[C.b][2] = true;
                                                    }
                                                    if (!liquids[C.b][3] && rand.Next(4) == 0)
                                                    {
                                                        PhysWater(level, 
                                                            level.PosToInt(x, y, (ushort)(z - 1)),
                                                            level.blocks[C.b]);
                                                        liquids[C.b][3] = true;
                                                    }
                                                    if (!liquids[C.b][4] && rand.Next(4) == 0)
                                                    {
                                                        PhysWater(level, 
                                                            level.PosToInt(x, (ushort)(y - 1), z),
                                                            level.blocks[C.b]);
                                                        liquids[C.b][4] = true;
                                                    }

                                                    if (!liquids[C.b][0] &&
                                                        !PhysWaterCheck(level, level.PosToInt(
                                                            (ushort)(x + 1), y, z)))
                                                        liquids[C.b][0] = true;
                                                    if (!liquids[C.b][1] &&
                                                        !PhysWaterCheck(level, level.PosToInt(
                                                            (ushort)(x - 1), y, z)))
                                                        liquids[C.b][1] = true;
                                                    if (!liquids[C.b][2] &&
                                                        !PhysWaterCheck(level, level.PosToInt(x, y,
                                                                                 (ushort)(z + 1))))
                                                        liquids[C.b][2] = true;
                                                    if (!liquids[C.b][3] &&
                                                        !PhysWaterCheck(level, level.PosToInt(x, y,
                                                                                 (ushort)(z - 1))))
                                                        liquids[C.b][3] = true;
                                                    if (!liquids[C.b][4] &&
                                                        !PhysWaterCheck(level, level.PosToInt(x,
                                                                                 (ushort)(y - 1),
                                                                                 z)))
                                                        liquids[C.b][4] = true;
                                                }
                                                else
                                                {
                                                    level.AddUpdate(C.b, Block.air);
                                                    //was placed near sponge
                                                    if (C.extraInfo.IndexOf("wait") == -1)
                                                        C.time = 255;
                                                }

                                                if (C.extraInfo.IndexOf("wait") == -1 &&
                                                    liquids.ContainsKey(C.b))
                                                    if (liquids[C.b][0] && liquids[C.b][1] &&
                                                        liquids[C.b][2] && liquids[C.b][3] &&
                                                        liquids[C.b][4])
                                                    {
                                                        liquids.Remove(C.b);
                                                        C.time = 255;
                                                    }
                                            }
                                            else
                                            {
                                                if (liquids.ContainsKey(C.b)) liquids.Remove(C.b);
                                                if (!PhysSpongeCheck(level, C.b))
                                                {
                                                    if (level.GetTile(x, (ushort)(y + 1), z) !=
                                                        Block.Zero)
                                                    {
                                                        PhysSandCheck(level, level.PosToInt(x, (ushort)(y + 1),
                                                                               z));
                                                    }
                                                    PhysWater(level, level.PosToInt((ushort)(x + 1), y, z),
                                                              level.blocks[C.b]);
                                                    PhysWater(level, level.PosToInt((ushort)(x - 1), y, z),
                                                              level.blocks[C.b]);
                                                    PhysWater(level, level.PosToInt(x, y, (ushort)(z + 1)),
                                                              level.blocks[C.b]);
                                                    PhysWater(level, level.PosToInt(x, y, (ushort)(z - 1)),
                                                              level.blocks[C.b]);
                                                    PhysWater(level, level.PosToInt(x, (ushort)(y - 1), z),
                                                              level.blocks[C.b]);
                                                }
                                                else
                                                {
                                                    level.AddUpdate(C.b, Block.air);
                                                    //was placed near sponge
                                                }

                                                if (C.extraInfo.IndexOf("wait") == -1)
                                                    C.time = 255;
                                            }
                                        }
                                        else
                                        {
                                            if (liquids.ContainsKey(C.b)) liquids.Remove(C.b);
                                            goto case Block.finiteWater;
                                        }
                                        break;

                                    case Block.WaterDown:
                                        rand = new Random();

                                        switch (level.GetTile(x, (ushort)(y - 1), z))
                                        {
                                            case Block.air:
                                                level.AddUpdate(level.PosToInt(x, (ushort)(y - 1), z),
                                                          Block.WaterDown);
                                                if (C.extraInfo.IndexOf("wait") == -1) C.time = 255;
                                                break;
                                            case Block.air_flood_down:
                                                break;
                                            case Block.lavastill:
                                            case Block.waterstill:
                                                break;
                                            default:
                                                if (level.GetTile(x, (ushort)(y - 1), z) !=
                                                    Block.WaterDown)
                                                {
                                                    PhysWater(level, 
                                                        level.PosToInt((ushort)(x + 1), y, z),
                                                        level.blocks[C.b]);
                                                    PhysWater(level, 
                                                        level.PosToInt((ushort)(x - 1), y, z),
                                                        level.blocks[C.b]);
                                                    PhysWater(level, 
                                                        level.PosToInt(x, y, (ushort)(z + 1)),
                                                        level.blocks[C.b]);
                                                    PhysWater(level, 
                                                        level.PosToInt(x, y, (ushort)(z - 1)),
                                                        level.blocks[C.b]);
                                                    if (C.extraInfo.IndexOf("wait") == -1)
                                                        C.time = 255;
                                                }
                                                break;
                                        }
                                        break;

                                    case Block.LavaDown:
                                        rand = new Random();

                                        switch (level.GetTile(x, (ushort)(y - 1), z))
                                        {
                                            case Block.air:
                                                level.AddUpdate(level.PosToInt(x, (ushort)(y - 1), z),
                                                          Block.LavaDown);
                                                if (C.extraInfo.IndexOf("wait") == -1) C.time = 255;
                                                break;
                                            case Block.air_flood_down:
                                                break;
                                            case Block.lavastill:
                                            case Block.waterstill:
                                                break;
                                            default:
                                                if (level.GetTile(x, (ushort)(y - 1), z) !=
                                                    Block.LavaDown)
                                                {
                                                    PhysLava(level,
                                                        level.PosToInt((ushort)(x + 1), y, z),
                                                        level.blocks[C.b]);
                                                    PhysLava(level,
                                                        level.PosToInt((ushort)(x - 1), y, z),
                                                        level.blocks[C.b]);
                                                    PhysLava(level,
                                                        level.PosToInt(x, y, (ushort)(z + 1)),
                                                        level.blocks[C.b]);
                                                    PhysLava(level, 
                                                        level.PosToInt(x, y, (ushort)(z - 1)),
                                                        level.blocks[C.b]);
                                                    if (C.extraInfo.IndexOf("wait") == -1)
                                                        C.time = 255;
                                                }
                                                break;
                                        }
                                        break;

                                    case Block.WaterFaucet:
                                        //rand = new Random();
                                        C.time++;
                                        if (C.time < 2) break;

                                        C.time = 0;

                                        switch (level.GetTile(x, (ushort)(y - 1), z))
                                        {
                                            case Block.WaterDown:
                                            case Block.air:
                                                if (rand.Next(1, 10) > 7)
                                                    level.AddUpdate(level.PosToInt(x, (ushort)(y - 1), z),
                                                              Block.air_flood_down);
                                                break;
                                            case Block.air_flood_down:
                                                if (rand.Next(1, 10) > 4)
                                                    level.AddUpdate(level.PosToInt(x, (ushort)(y - 1), z),
                                                              Block.WaterDown);
                                                break;
                                        }
                                        break;

                                    case Block.LavaFaucet:
                                        //rand = new Random();
                                        C.time++;
                                        if (C.time < 2) break;

                                        C.time = 0;

                                        switch (level.GetTile(x, (ushort)(y - 1), z))
                                        {
                                            case Block.LavaDown:
                                            case Block.air:
                                                if (rand.Next(1, 10) > 7)
                                                    level.AddUpdate(level.PosToInt(x, (ushort)(y - 1), z),
                                                              Block.air_flood_down);
                                                break;
                                            case Block.air_flood_down:
                                                if (rand.Next(1, 10) > 4)
                                                    level.AddUpdate(level.PosToInt(x, (ushort)(y - 1), z),
                                                              Block.LavaDown);
                                                break;
                                        }
                                        break;

                                    case Block.lava: //Active_lava
                                    case Block.activedeathlava:
                                        //initialy checks if block is valid
                                        if (C.time < 4)
                                        {
                                            C.time++;
                                            break;
                                        }
                                        if (!level.finite)
                                        {
                                            if (level.randomFlow)
                                            {
                                                if (!PhysSpongeCheck(level, C.b, true))
                                                {
                                                    C.time = (byte)rand.Next(3);
                                                    if (!liquids.ContainsKey(C.b))
                                                        liquids.Add(C.b, new bool[5]);

                                                    if (!liquids[C.b][0] && rand.Next(4) == 0)
                                                    {
                                                        PhysLava(level, 
                                                            level.PosToInt((ushort)(x + 1), y, z),
                                                            level.blocks[C.b]);
                                                        liquids[C.b][0] = true;
                                                    }
                                                    if (!liquids[C.b][1] && rand.Next(4) == 0)
                                                    {
                                                        PhysLava(level, 
                                                            level.PosToInt((ushort)(x - 1), y, z),
                                                            level.blocks[C.b]);
                                                        liquids[C.b][1] = true;
                                                    }
                                                    if (!liquids[C.b][2] && rand.Next(4) == 0)
                                                    {
                                                        PhysLava(level, 
                                                            level.PosToInt(x, y, (ushort)(z + 1)),
                                                            level.blocks[C.b]);
                                                        liquids[C.b][2] = true;
                                                    }
                                                    if (!liquids[C.b][3] && rand.Next(4) == 0)
                                                    {
                                                        PhysLava(level, 
                                                            level.PosToInt(x, y, (ushort)(z - 1)),
                                                            level.blocks[C.b]);
                                                        liquids[C.b][3] = true;
                                                    }
                                                    if (!liquids[C.b][4] && rand.Next(4) == 0)
                                                    {
                                                        PhysLava(level, 
                                                            level.PosToInt(x, (ushort)(y - 1), z),
                                                            level.blocks[C.b]);
                                                        liquids[C.b][4] = true;
                                                    }

                                                    if (!liquids[C.b][0] &&
                                                        !PhysLavaCheck(level, level.PosToInt((ushort)(x + 1),
                                                                                y, z)))
                                                        liquids[C.b][0] = true;
                                                    if (!liquids[C.b][1] &&
                                                        !PhysLavaCheck(level, level.PosToInt((ushort)(x - 1),
                                                                                y, z)))
                                                        liquids[C.b][1] = true;
                                                    if (!liquids[C.b][2] &&
                                                        !PhysLavaCheck(level, level.PosToInt(x, y,
                                                                                (ushort)(z + 1))))
                                                        liquids[C.b][2] = true;
                                                    if (!liquids[C.b][3] &&
                                                        !PhysLavaCheck(level, level.PosToInt(x, y,
                                                                                (ushort)(z - 1))))
                                                        liquids[C.b][3] = true;
                                                    if (!liquids[C.b][4] &&
                                                        !PhysLavaCheck(level, level.PosToInt(x,
                                                                                (ushort)(y - 1),
                                                                                z)))
                                                        liquids[C.b][4] = true;
                                                }
                                                else
                                                {
                                                    level.AddUpdate(C.b, Block.air);
                                                    //was placed near sponge
                                                    if (C.extraInfo.IndexOf("wait") == -1)
                                                        C.time = 255;
                                                }

                                                if (C.extraInfo.IndexOf("wait") == -1 &&
                                                    liquids.ContainsKey(C.b))
                                                    if (liquids[C.b][0] && liquids[C.b][1] &&
                                                        liquids[C.b][2] && liquids[C.b][3] &&
                                                        liquids[C.b][4])
                                                    {
                                                        liquids.Remove(C.b);
                                                        C.time = 255;
                                                    }
                                            }
                                            else
                                            {
                                                if (liquids.ContainsKey(C.b)) liquids.Remove(C.b);
                                                if (!PhysSpongeCheck(level, C.b, true))
                                                {
                                                    PhysLava(level, level.PosToInt((ushort)(x + 1), y, z),
                                                             level.blocks[C.b]);
                                                    PhysLava(level, level.PosToInt((ushort)(x - 1), y, z),
                                                             level.blocks[C.b]);
                                                    PhysLava(level, level.PosToInt(x, y, (ushort)(z + 1)),
                                                             level.blocks[C.b]);
                                                    PhysLava(level, level.PosToInt(x, y, (ushort)(z - 1)),
                                                             level.blocks[C.b]);
                                                    PhysLava(level, level.PosToInt(x, (ushort)(y - 1), z),
                                                             level.blocks[C.b]);
                                                }
                                                else
                                                {
                                                    level.AddUpdate(C.b, Block.air);
                                                    //was placed near sponge
                                                }

                                                if (C.extraInfo.IndexOf("wait") == -1)
                                                    C.time = 255;
                                            }
                                        }
                                        else
                                        {
                                            if (liquids.ContainsKey(C.b)) liquids.Remove(C.b);
                                            goto case Block.finiteWater;
                                        }
                                        break;

                                    #region fire

                                    case Block.fire:
                                        if (C.time < 2)
                                        {
                                            C.time++;
                                            break;
                                        }

                                        storedRand = rand.Next(1, 20);
                                        if (storedRand < 2 && C.time % 2 == 0)
                                        {
                                            storedRand = rand.Next(1, 18);

                                            if (storedRand <= 3 &&
                                                level.GetTile((ushort)(x - 1), y, z) == Block.air)
                                                level.AddUpdate(level.PosToInt((ushort)(x - 1), y, z),
                                                          Block.fire);
                                            else if (storedRand <= 6 &&
                                                     level.GetTile((ushort)(x + 1), y, z) == Block.air)
                                                level.AddUpdate(level.PosToInt((ushort)(x + 1), y, z),
                                                          Block.fire);
                                            else if (storedRand <= 9 &&
                                                     level.GetTile(x, (ushort)(y - 1), z) ==
                                                     Block.air)
                                                level.AddUpdate(
                                                    level.PosToInt(x, (ushort)(y - 1), z),
                                                    Block.fire);
                                            else if (storedRand <= 12 &&
                                                     level.GetTile(x, (ushort)(y + 1), z) ==
                                                     Block.air)
                                                level.AddUpdate(
                                                    level.PosToInt(x, (ushort)(y + 1), z),
                                                    Block.fire);
                                            else if (storedRand <= 15 &&
                                                     level.GetTile(x, y, (ushort)(z - 1)) ==
                                                     Block.air)
                                                level.AddUpdate(
                                                    level.PosToInt(x, y,
                                                             (ushort)(z - 1)),
                                                    Block.fire);
                                            else if (storedRand <= 18 &&
                                                     level.GetTile(x, y, (ushort)(z + 1)) ==
                                                     Block.air)
                                                level.AddUpdate(
                                                    level.PosToInt(x, y,
                                                             (ushort)(z + 1)),
                                                    Block.fire);
                                        }

                                        if (
                                            Block.LavaKill(level.GetTile((ushort)(x - 1), y,
                                                                   (ushort)(z - 1))))
                                        {
                                            if (level.GetTile((ushort)(x - 1), y, z) == Block.air)
                                                level.AddUpdate(level.PosToInt((ushort)(x - 1), y, z),
                                                          Block.fire);
                                            if (level.GetTile(x, y, (ushort)(z - 1)) == Block.air)
                                                level.AddUpdate(level.PosToInt(x, y, (ushort)(z - 1)),
                                                          Block.fire);
                                        }
                                        if (
                                            Block.LavaKill(level.GetTile((ushort)(x + 1), y,
                                                                   (ushort)(z - 1))))
                                        {
                                            if (level.GetTile((ushort)(x + 1), y, z) == Block.air)
                                                level.AddUpdate(level.PosToInt((ushort)(x + 1), y, z),
                                                          Block.fire);
                                            if (level.GetTile(x, y, (ushort)(z - 1)) == Block.air)
                                                level.AddUpdate(level.PosToInt(x, y, (ushort)(z - 1)),
                                                          Block.fire);
                                        }
                                        if (
                                            Block.LavaKill(level.GetTile((ushort)(x - 1), y,
                                                                   (ushort)(z + 1))))
                                        {
                                            if (level.GetTile((ushort)(x - 1), y, z) == Block.air)
                                                level.AddUpdate(level.PosToInt((ushort)(x - 1), y, z),
                                                          Block.fire);
                                            if (level.GetTile(x, y, (ushort)(z + 1)) == Block.air)
                                                level.AddUpdate(level.PosToInt(x, y, (ushort)(z + 1)),
                                                          Block.fire);
                                        }
                                        if (
                                            Block.LavaKill(level.GetTile((ushort)(x + 1), y,
                                                                   (ushort)(z + 1))))
                                        {
                                            if (level.GetTile((ushort)(x + 1), y, z) == Block.air)
                                                level.AddUpdate(level.PosToInt((ushort)(x + 1), y, z),
                                                          Block.fire);
                                            if (level.GetTile(x, y, (ushort)(z + 1)) == Block.air)
                                                level.AddUpdate(level.PosToInt(x, y, (ushort)(z + 1)),
                                                          Block.fire);
                                        }
                                        if (
                                            Block.LavaKill(level.GetTile(x, (ushort)(y - 1),
                                                                   (ushort)(z - 1))))
                                        {
                                            if (level.GetTile(x, (ushort)(y - 1), z) == Block.air)
                                                level.AddUpdate(level.PosToInt(x, (ushort)(y - 1), z),
                                                          Block.fire);
                                            if (level.GetTile(x, y, (ushort)(z - 1)) == Block.air)
                                                level.AddUpdate(level.PosToInt(x, y, (ushort)(z - 1)),
                                                          Block.fire);
                                        }
                                        else if (level.GetTile(x, (ushort)(y - 1), z) == Block.grass)
                                            level.AddUpdate(level.PosToInt(x, (ushort)(y - 1), z), Block.dirt);

                                        if (
                                            Block.LavaKill(level.GetTile(x, (ushort)(y + 1),
                                                                   (ushort)(z - 1))))
                                        {
                                            if (level.GetTile(x, (ushort)(y + 1), z) == Block.air)
                                                level.AddUpdate(level.PosToInt(x, (ushort)(y + 1), z),
                                                          Block.fire);
                                            if (level.GetTile(x, y, (ushort)(z - 1)) == Block.air)
                                                level.AddUpdate(level.PosToInt(x, y, (ushort)(z - 1)),
                                                          Block.fire);
                                        }
                                        if (
                                            Block.LavaKill(level.GetTile(x, (ushort)(y - 1),
                                                                   (ushort)(z + 1))))
                                        {
                                            if (level.GetTile(x, (ushort)(y - 1), z) == Block.air)
                                                level.AddUpdate(level.PosToInt(x, (ushort)(y - 1), z),
                                                          Block.fire);
                                            if (level.GetTile(x, y, (ushort)(z + 1)) == Block.air)
                                                level.AddUpdate(level.PosToInt(x, y, (ushort)(z + 1)),
                                                          Block.fire);
                                        }
                                        if (
                                            Block.LavaKill(level.GetTile(x, (ushort)(y + 1),
                                                                   (ushort)(z + 1))))
                                        {
                                            if (level.GetTile(x, (ushort)(y + 1), z) == Block.air)
                                                level.AddUpdate(level.PosToInt(x, (ushort)(y + 1), z),
                                                          Block.fire);
                                            if (level.GetTile(x, y, (ushort)(z + 1)) == Block.air)
                                                level.AddUpdate(level.PosToInt(x, y, (ushort)(z + 1)),
                                                          Block.fire);
                                        }
                                        if (
                                            Block.LavaKill(level.GetTile((ushort)(x - 1),
                                                                   (ushort)(y - 1), z)))
                                        {
                                            if (level.GetTile(x, (ushort)(y - 1), z) == Block.air)
                                                level.AddUpdate(level.PosToInt(x, (ushort)(y - 1), z),
                                                          Block.fire);
                                            if (level.GetTile((ushort)(x - 1), y, z) == Block.air)
                                                level.AddUpdate(level.PosToInt((ushort)(x - 1), y, z),
                                                          Block.fire);
                                        }
                                        if (
                                            Block.LavaKill(level.GetTile((ushort)(x - 1),
                                                                   (ushort)(y + 1), z)))
                                        {
                                            if (level.GetTile(x, (ushort)(y + 1), z) == Block.air)
                                                level.AddUpdate(level.PosToInt(x, (ushort)(y + 1), z),
                                                          Block.fire);
                                            if (level.GetTile((ushort)(x - 1), y, z) == Block.air)
                                                level.AddUpdate(level.PosToInt((ushort)(x - 1), y, z),
                                                          Block.fire);
                                        }
                                        if (
                                            Block.LavaKill(level.GetTile((ushort)(x + 1),
                                                                   (ushort)(y - 1), z)))
                                        {
                                            if (level.GetTile(x, (ushort)(y - 1), z) == Block.air)
                                                level.AddUpdate(level.PosToInt(x, (ushort)(y - 1), z),
                                                          Block.fire);
                                            if (level.GetTile((ushort)(x + 1), y, z) == Block.air)
                                                level.AddUpdate(level.PosToInt((ushort)(x + 1), y, z),
                                                          Block.fire);
                                        }
                                        if (
                                            Block.LavaKill(level.GetTile((ushort)(x + 1),
                                                                   (ushort)(y + 1), z)))
                                        {
                                            if (level.GetTile(x, (ushort)(y + 1), z) == Block.air)
                                                level.AddUpdate(level.PosToInt(x, (ushort)(y + 1), z),
                                                          Block.fire);
                                            if (level.GetTile((ushort)(x + 1), y, z) == Block.air)
                                                level.AddUpdate(level.PosToInt((ushort)(x + 1), y, z),
                                                          Block.fire);
                                        }

                                        if (level.physics >= 2)
                                        {
                                            if (C.time < 4)
                                            {
                                                C.time++;
                                                break;
                                            }

                                            if (Block.LavaKill(level.GetTile((ushort)(x - 1), y, z)))
                                                level.AddUpdate(level.PosToInt((ushort)(x - 1), y, z),
                                                          Block.fire);
                                            else if (level.GetTile((ushort)(x - 1), y, z) == Block.tnt)
                                                level.MakeExplosion((ushort)(x - 1), y, z, -1);

                                            if (Block.LavaKill(level.GetTile((ushort)(x + 1), y, z)))
                                                level.AddUpdate(level.PosToInt((ushort)(x + 1), y, z),
                                                          Block.fire);
                                            else if (level.GetTile((ushort)(x + 1), y, z) == Block.tnt)
                                                level.MakeExplosion((ushort)(x + 1), y, z, -1);

                                            if (Block.LavaKill(level.GetTile(x, (ushort)(y - 1), z)))
                                                level.AddUpdate(level.PosToInt(x, (ushort)(y - 1), z),
                                                          Block.fire);
                                            else if (level.GetTile(x, (ushort)(y - 1), z) == Block.tnt)
                                                level.MakeExplosion(x, (ushort)(y - 1), z, -1);

                                            if (Block.LavaKill(level.GetTile(x, (ushort)(y + 1), z)))
                                                level.AddUpdate(level.PosToInt(x, (ushort)(y + 1), z),
                                                          Block.fire);
                                            else if (level.GetTile(x, (ushort)(y + 1), z) == Block.tnt)
                                                level.MakeExplosion(x, (ushort)(y + 1), z, -1);

                                            if (Block.LavaKill(level.GetTile(x, y, (ushort)(z - 1))))
                                                level.AddUpdate(level.PosToInt(x, y, (ushort)(z - 1)),
                                                          Block.fire);
                                            else if (level.GetTile(x, y, (ushort)(z - 1)) == Block.tnt)
                                                level.MakeExplosion(x, y, (ushort)(z - 1), -1);

                                            if (Block.LavaKill(level.GetTile(x, y, (ushort)(z + 1))))
                                                level.AddUpdate(level.PosToInt(x, y, (ushort)(z + 1)),
                                                          Block.fire);
                                            else if (level.GetTile(x, y, (ushort)(z + 1)) == Block.tnt)
                                                level.MakeExplosion(x, y, (ushort)(z + 1), -1);
                                        }

                                        C.time++;
                                        if (C.time > 5)
                                        {
                                            storedRand = (rand.Next(1, 10));
                                            if (storedRand <= 2)
                                            {
                                                level.AddUpdate(C.b, Block.coal);
                                                C.extraInfo = "drop 63 dissipate 10";
                                            }
                                            else if (storedRand <= 4)
                                            {
                                                level.AddUpdate(C.b, Block.obsidian);
                                                C.extraInfo = "drop 63 dissipate 10";
                                            }
                                            else if (storedRand <= 8) level.AddUpdate(C.b, Block.air);
                                            else C.time = 3;
                                        }

                                        break;

                                    #endregion

                                    case Block.finiteWater:
                                    case Block.finiteLava:
                                        level.finiteMovement(C, x, y, z);
                                        break;
                                    case Block.finiteLavaFaucet:
                                        var bufferfinitefaucet1 = new List<ushort>();

                                        for (ushort i = 0; i < 6; ++i) bufferfinitefaucet1.Add(i);

                                        for (int k = bufferfinitefaucet1.Count - 1; k > 1; --k)
                                        {
                                            int randIndx = rand.Next(k);
                                            ushort temp = bufferfinitefaucet1[k];
                                            bufferfinitefaucet1[k] = bufferfinitefaucet1[randIndx];
                                            // move random num to end of list.
                                            bufferfinitefaucet1[randIndx] = temp;
                                        }

                                        foreach (ushort i in bufferfinitefaucet1)
                                        {
                                            switch (i)
                                            {
                                                case Block.air:
                                                    if (level.GetTile((ushort)(x - 1), y, z) ==
                                                        Block.air)
                                                    {
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt((ushort)(x - 1), y, z),
                                                                Block.finiteLava))
                                                            InnerChange = true;
                                                    }
                                                    break;
                                                case 1:
                                                    if (level.GetTile((ushort)(x + 1), y, z) ==
                                                        Block.air)
                                                    {
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt((ushort)(x + 1), y, z),
                                                                Block.finiteLava))
                                                            InnerChange = true;
                                                    }
                                                    break;
                                                case 2:
                                                    if (level.GetTile(x, (ushort)(y - 1), z) ==
                                                        Block.air)
                                                    {
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt(x, (ushort)(y - 1), z),
                                                                Block.finiteLava))
                                                            InnerChange = true;
                                                    }
                                                    break;
                                                case 3:
                                                    if (level.GetTile(x, (ushort)(y + 1), z) ==
                                                        Block.air)
                                                    {
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt(x, (ushort)(y + 1), z),
                                                                Block.finiteLava))
                                                            InnerChange = true;
                                                    }
                                                    break;
                                                case 4:
                                                    if (level.GetTile(x, y, (ushort)(z - 1)) ==
                                                        Block.air)
                                                    {
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt(x, y, (ushort)(z - 1)),
                                                                Block.finiteLava))
                                                            InnerChange = true;
                                                    }
                                                    break;
                                                case 5:
                                                    if (level.GetTile(x, y, (ushort)(z + 1)) ==
                                                        Block.air)
                                                    {
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt(x, y, (ushort)(z + 1)),
                                                                Block.finiteLava))
                                                            InnerChange = true;
                                                    }
                                                    break;
                                            }

                                            if (InnerChange) break;
                                        }

                                        break;
                                    case Block.finiteFaucet:
                                        var bufferfinitefaucet = new List<ushort>();

                                        for (ushort i = 0; i < 6; ++i) bufferfinitefaucet.Add(i);

                                        for (int k = bufferfinitefaucet.Count - 1; k > 1; --k)
                                        {
                                            int randIndx = rand.Next(k);
                                            ushort temp = bufferfinitefaucet[k];
                                            bufferfinitefaucet[k] = bufferfinitefaucet[randIndx];
                                            // move random num to end of list.
                                            bufferfinitefaucet[randIndx] = temp;
                                        }

                                        foreach (ushort i in bufferfinitefaucet)
                                        {
                                            switch (i)
                                            {
                                                case Block.air:
                                                    if (level.GetTile((ushort)(x - 1), y, z) ==
                                                        Block.air)
                                                    {
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt((ushort)(x - 1), y, z),
                                                                Block.finiteWater))
                                                            InnerChange = true;
                                                    }
                                                    break;
                                                case 1:
                                                    if (level.GetTile((ushort)(x + 1), y, z) ==
                                                        Block.air)
                                                    {
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt((ushort)(x + 1), y, z),
                                                                Block.finiteWater))
                                                            InnerChange = true;
                                                    }
                                                    break;
                                                case 2:
                                                    if (level.GetTile(x, (ushort)(y - 1), z) ==
                                                        Block.air)
                                                    {
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt(x, (ushort)(y - 1), z),
                                                                Block.finiteWater))
                                                            InnerChange = true;
                                                    }
                                                    break;
                                                case 3:
                                                    if (level.GetTile(x, (ushort)(y + 1), z) ==
                                                        Block.air)
                                                    {
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt(x, (ushort)(y + 1), z),
                                                                Block.finiteWater))
                                                            InnerChange = true;
                                                    }
                                                    break;
                                                case 4:
                                                    if (level.GetTile(x, y, (ushort)(z - 1)) ==
                                                        Block.air)
                                                    {
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt(x, y, (ushort)(z - 1)),
                                                                Block.finiteWater))
                                                            InnerChange = true;
                                                    }
                                                    break;
                                                case 5:
                                                    if (level.GetTile(x, y, (ushort)(z + 1)) ==
                                                        Block.air)
                                                    {
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt(x, y, (ushort)(z + 1)),
                                                                Block.finiteWater))
                                                            InnerChange = true;
                                                    }
                                                    break;
                                            }

                                            if (InnerChange) break;
                                        }

                                        break;

                                    case Block.sand: //Sand
                                        if (PhysSand(level, C.b, Block.sand))
                                        {
                                            Physair(level, level.PosToInt((ushort)(x + 1), y, z));
                                            Physair(level, level.PosToInt((ushort)(x - 1), y, z));
                                            Physair(level, level.PosToInt(x, y, (ushort)(z + 1)));
                                            Physair(level, level.PosToInt(x, y, (ushort)(z - 1)));
                                            Physair(level, level.PosToInt(x, (ushort)(y + 1), z));
                                            //Check block above
                                        }
                                        C.time = 255;
                                        break;

                                    case Block.gravel: //Gravel
                                        if (PhysSand(level, C.b, Block.gravel))
                                        {
                                            Physair(level, level.PosToInt((ushort)(x + 1), y, z));
                                            Physair(level, level.PosToInt((ushort)(x - 1), y, z));
                                            Physair(level, level.PosToInt(x, y, (ushort)(z + 1)));
                                            Physair(level, level.PosToInt(x, y, (ushort)(z - 1)));
                                            Physair(level, level.PosToInt(x, (ushort)(y + 1), z));
                                            //Check block above
                                        }
                                        C.time = 255;
                                        break;

                                    case Block.sponge: //SPONGE
                                        PhysSponge(level, C.b);
                                        C.time = 255;
                                        break;

                                    case Block.lava_sponge: //SPONGE
                                        PhysSponge(level, C.b, true);
                                        C.time = 255;
                                        break;

                                    //Adv level.physics updating anything placed next to water or lava
                                    case Block.wood: //Wood to die in lava
                                    case Block.trunk: //Wood to die in lava
                                    case Block.yellowflower:
                                    case Block.redflower:
                                    case Block.mushroom:
                                    case Block.redmushroom:
                                    case Block.bookcase: //bookcase
                                    case Block.red: //Shitload of cloth
                                    case Block.orange:
                                    case Block.yellow:
                                    case Block.lightgreen:
                                    case Block.green:
                                    case Block.aquagreen:
                                    case Block.cyan:
                                    case Block.lightblue:
                                    case Block.blue:
                                    case Block.purple:
                                    case Block.lightpurple:
                                    case Block.pink:
                                    case Block.darkpink:
                                    case Block.darkgrey:
                                    case Block.lightgrey:
                                    case Block.white:
                                        if (level.physics > 1)
                                        //Adv level.physics kills flowers and mushroos in water/lava
                                        {
                                            Physair(level, level.PosToInt((ushort)(x + 1), y, z));
                                            Physair(level, level.PosToInt((ushort)(x - 1), y, z));
                                            Physair(level, level.PosToInt(x, y, (ushort)(z + 1)));
                                            Physair(level, level.PosToInt(x, y, (ushort)(z - 1)));
                                            Physair(level, level.PosToInt(x, (ushort)(y + 1), z));
                                            //Check block above
                                        }
                                        C.time = 255;
                                        break;

                                    case Block.staircasestep:
                                        PhysStair(level, C.b);
                                        C.time = 255;
                                        break;

                                    case Block.wood_float: //wood_float
                                        PhysFloatwood(level, C.b);
                                        C.time = 255;
                                        break;

                                    case Block.lava_fast: //lava_fast
                                    case Block.fastdeathlava:
                                        //initialy checks if block is valid
                                        if (level.randomFlow)
                                        {
                                            if (!PhysSpongeCheck(level, C.b, true))
                                            {
                                                if (!liquids.ContainsKey(C.b))
                                                    liquids.Add(C.b, new bool[5]);

                                                if (!liquids[C.b][0] && rand.Next(4) == 0)
                                                {
                                                    PhysLava(level, level.PosToInt((ushort)(x + 1), y, z),
                                                             level.blocks[C.b]);
                                                    liquids[C.b][0] = true;
                                                }
                                                if (!liquids[C.b][1] && rand.Next(4) == 0)
                                                {
                                                    PhysLava(level, level.PosToInt((ushort)(x - 1), y, z),
                                                             level.blocks[C.b]);
                                                    liquids[C.b][1] = true;
                                                }
                                                if (!liquids[C.b][2] && rand.Next(4) == 0)
                                                {
                                                    PhysLava(level, level.PosToInt(x, y, (ushort)(z + 1)),
                                                             level.blocks[C.b]);
                                                    liquids[C.b][2] = true;
                                                }
                                                if (!liquids[C.b][3] && rand.Next(4) == 0)
                                                {
                                                    PhysLava(level, level.PosToInt(x, y, (ushort)(z - 1)),
                                                             level.blocks[C.b]);
                                                    liquids[C.b][3] = true;
                                                }
                                                if (!liquids[C.b][4] && rand.Next(4) == 0)
                                                {
                                                    PhysLava(level, level.PosToInt(x, (ushort)(y - 1), z),
                                                             level.blocks[C.b]);
                                                    liquids[C.b][4] = true;
                                                }

                                                if (!liquids[C.b][0] &&
                                                    !PhysLavaCheck(level, level.PosToInt((ushort)(x + 1), y, z)))
                                                    liquids[C.b][0] = true;
                                                if (!liquids[C.b][1] &&
                                                    !PhysLavaCheck(level, level.PosToInt((ushort)(x - 1), y, z)))
                                                    liquids[C.b][1] = true;
                                                if (!liquids[C.b][2] &&
                                                    !PhysLavaCheck(level, level.PosToInt(x, y, (ushort)(z + 1))))
                                                    liquids[C.b][2] = true;
                                                if (!liquids[C.b][3] &&
                                                    !PhysLavaCheck(level, level.PosToInt(x, y, (ushort)(z - 1))))
                                                    liquids[C.b][3] = true;
                                                if (!liquids[C.b][4] &&
                                                    !PhysLavaCheck(level, level.PosToInt(x, (ushort)(y - 1), z)))
                                                    liquids[C.b][4] = true;
                                            }
                                            else
                                            {
                                                level.AddUpdate(C.b, Block.air);
                                                //was placed near sponge
                                                C.time = 255;
                                            }

                                            if (liquids.ContainsKey(C.b))
                                                if (liquids[C.b][0] && liquids[C.b][1] &&
                                                    liquids[C.b][2] && liquids[C.b][3] &&
                                                    liquids[C.b][4])
                                                {
                                                    liquids.Remove(C.b);
                                                    C.time = 255;
                                                }
                                        }
                                        else
                                        {
                                            if (liquids.ContainsKey(C.b)) liquids.Remove(C.b);
                                            if (!PhysSpongeCheck(level, C.b, true))
                                            {
                                                PhysLava(level, level.PosToInt((ushort)(x + 1), y, z),
                                                         level.blocks[C.b]);
                                                PhysLava(level, level.PosToInt((ushort)(x - 1), y, z),
                                                         level.blocks[C.b]);
                                                PhysLava(level, level.PosToInt(x, y, (ushort)(z + 1)),
                                                         level.blocks[C.b]);
                                                PhysLava(level, level.PosToInt(x, y, (ushort)(z - 1)),
                                                         level.blocks[C.b]);
                                                PhysLava(level, level.PosToInt(x, (ushort)(y - 1), z),
                                                         level.blocks[C.b]);
                                            }
                                            else
                                                level.AddUpdate(C.b, Block.air);
                                            //was placed near sponge

                                            C.time = 255;
                                        }
                                        break;

                                    //Special blocks that are not saved
                                    case Block.air_flood: //air_flood
                                        if (C.time < 1)
                                        {
                                            PhysairFlood(level, level.PosToInt((ushort)(x + 1), y, z),
                                                         Block.air_flood);
                                            PhysairFlood(level, level.PosToInt((ushort)(x - 1), y, z),
                                                         Block.air_flood);
                                            PhysairFlood(level, level.PosToInt(x, y, (ushort)(z + 1)),
                                                         Block.air_flood);
                                            PhysairFlood(level, level.PosToInt(x, y, (ushort)(z - 1)),
                                                         Block.air_flood);
                                            PhysairFlood(level, level.PosToInt(x, (ushort)(y - 1), z),
                                                         Block.air_flood);
                                            PhysairFlood(level, level.PosToInt(x, (ushort)(y + 1), z),
                                                         Block.air_flood);

                                            C.time++;
                                        }
                                        else
                                        {
                                            level.AddUpdate(C.b, 0); //Turn back into normal air
                                            C.time = 255;
                                        }
                                        break;

                                    case Block.door_air:
                                    //door_air         Change any door blocks nearby into door_air
                                    case Block.door2_air:
                                    //door_air         Change any door blocks nearby into door_air
                                    case Block.door3_air:
                                    //door_air         Change any door blocks nearby into door_air
                                    case Block.door4_air:
                                    //door_air         Change any door blocks nearby into door_air
                                    case Block.door5_air:
                                    //door_air         Change any door blocks nearby into door_air
                                    case Block.door6_air:
                                    //door_air         Change any door blocks nearby into door_air
                                    case Block.door7_air:
                                    //door_air         Change any door blocks nearby into door_air
                                    case Block.door8_air:
                                    //door_air         Change any door blocks nearby into door_air
                                    case Block.door10_air:
                                    //door_air         Change any door blocks nearby into door_air
                                    case Block.door12_air:
                                    case Block.door13_air:
                                    case Block.door_iron_air:
                                    case Block.door_gold_air:
                                    case Block.door_cobblestone_air:
                                    case Block.door_red_air:



                                    case Block.door_dirt_air:
                                    case Block.door_grass_air:
                                    case Block.door_blue_air:
                                    case Block.door_book_air:
                                        level.AnyDoor(C, x, y, z, 16);
                                        break;
                                    case Block.door11_air:
                                    case Block.door14_air:
                                        level.AnyDoor(C, x, y, z, 4, true);
                                        break;
                                    case Block.door9_air:
                                        //door_air         Change any door blocks nearby into door_air
                                        level.AnyDoor(C, x, y, z, 4);
                                        break;

                                    case Block.odoor1_air:
                                    case Block.odoor2_air:
                                    case Block.odoor3_air:
                                    case Block.odoor4_air:
                                    case Block.odoor5_air:
                                    case Block.odoor6_air:
                                    case Block.odoor7_air:
                                    case Block.odoor8_air:
                                    case Block.odoor9_air:
                                    case Block.odoor10_air:
                                    case Block.odoor11_air:
                                    case Block.odoor12_air:

                                    case Block.odoor1:
                                    case Block.odoor2:
                                    case Block.odoor3:
                                    case Block.odoor4:
                                    case Block.odoor5:
                                    case Block.odoor6:
                                    case Block.odoor7:
                                    case Block.odoor8:
                                    case Block.odoor9:
                                    case Block.odoor10:
                                    case Block.odoor11:
                                    case Block.odoor12:
                                        level.odoor(C);
                                        break;

                                    case Block.air_flood_layer: //air_flood_layer
                                        if (C.time < 1)
                                        {
                                            PhysairFlood(level, level.PosToInt((ushort)(x + 1), y, z),
                                                         Block.air_flood_layer);
                                            PhysairFlood(level, level.PosToInt((ushort)(x - 1), y, z),
                                                         Block.air_flood_layer);
                                            PhysairFlood(level, level.PosToInt(x, y, (ushort)(z + 1)),
                                                         Block.air_flood_layer);
                                            PhysairFlood(level, level.PosToInt(x, y, (ushort)(z - 1)),
                                                         Block.air_flood_layer);

                                            C.time++;
                                        }
                                        else
                                        {
                                            level.AddUpdate(C.b, 0); //Turn back into normal air
                                            C.time = 255;
                                        }
                                        break;

                                    case Block.air_flood_down: //air_flood_down
                                        if (C.time < 1)
                                        {
                                            PhysairFlood(level, level.PosToInt((ushort)(x + 1), y, z),
                                                         Block.air_flood_down);
                                            PhysairFlood(level, level.PosToInt((ushort)(x - 1), y, z),
                                                         Block.air_flood_down);
                                            PhysairFlood(level, level.PosToInt(x, y, (ushort)(z + 1)),
                                                         Block.air_flood_down);
                                            PhysairFlood(level, level.PosToInt(x, y, (ushort)(z - 1)),
                                                         Block.air_flood_down);
                                            PhysairFlood(level, level.PosToInt(x, (ushort)(y - 1), z),
                                                         Block.air_flood_down);

                                            C.time++;
                                        }
                                        else
                                        {
                                            level.AddUpdate(C.b, 0); //Turn back into normal air
                                            C.time = 255;
                                        }
                                        break;

                                    case Block.air_flood_up: //air_flood_up
                                        if (C.time < 1)
                                        {
                                            PhysairFlood(level, level.PosToInt((ushort)(x + 1), y, z),
                                                         Block.air_flood_up);
                                            PhysairFlood(level, level.PosToInt((ushort)(x - 1), y, z),
                                                         Block.air_flood_up);
                                            PhysairFlood(level, level.PosToInt(x, y, (ushort)(z + 1)),
                                                         Block.air_flood_up);
                                            PhysairFlood(level, level.PosToInt(x, y, (ushort)(z - 1)),
                                                         Block.air_flood_up);
                                            PhysairFlood(level, level.PosToInt(x, (ushort)(y + 1), z),
                                                         Block.air_flood_up);

                                            C.time++;
                                        }
                                        else
                                        {
                                            level.AddUpdate(C.b, 0); //Turn back into normal air
                                            C.time = 255;
                                        }
                                        break;

                                    case Block.smalltnt:
                                        //For TNT Wars
                                        if (C.p != null && C.p.PlayingTntWars)
                                        {
                                            int ExplodeDistance = -1;
                                            switch (TntWarsGame.GetTntWarsGame(C.p).GameDifficulty)
                                            {
                                                case TntWarsGame.TntWarsDifficulty.Easy:
                                                    if (C.time < 7)
                                                    {
                                                        C.time += 1;
                                                        level.Blockchange(x, (ushort)(y + 1), z, level.GetTile(x, (ushort)(y + 1), z) == Block.lavastill ? (ushort)Block.air : Block.lavastill);
                                                    }
                                                    else ExplodeDistance = 2;
                                                    break;

                                                case TntWarsGame.TntWarsDifficulty.Normal:
                                                    if (C.time < 5)
                                                    {
                                                        C.time += 1;
                                                        level.Blockchange(x, (ushort)(y + 1), z,
                                                                    level.GetTile(x, (ushort)(y + 1), z) ==
                                                                    Block.lavastill
                                                                        ? (ushort)Block.air
                                                                        : Block.lavastill);
                                                    }
                                                    else ExplodeDistance = 2;
                                                    break;

                                                case TntWarsGame.TntWarsDifficulty.Hard:
                                                    if (C.time < 3)
                                                    {
                                                        C.time += 1;
                                                        level.Blockchange(x, (ushort)(y + 1), z,
                                                                    level.GetTile(x, (ushort)(y + 1), z) ==
                                                                    Block.lavastill
                                                                        ? (ushort)Block.air
                                                                        : Block.lavastill);
                                                    }
                                                    else
                                                    {
                                                        ExplodeDistance = 2;
                                                    }
                                                    break;

                                                case TntWarsGame.TntWarsDifficulty.Extreme:
                                                    if (C.time < 3)
                                                    {
                                                        C.time += 1;
                                                        level.Blockchange(x, (ushort)(y + 1), z,
                                                                    level.GetTile(x, (ushort)(y + 1), z) ==
                                                                    Block.lavastill
                                                                        ? (ushort)Block.air
                                                                        : Block.lavastill);
                                                    }
                                                    else
                                                    {
                                                        ExplodeDistance = 3;
                                                    }
                                                    break;
                                            }
                                            if (ExplodeDistance != -1)
                                            {
                                                if (C.p.TntWarsKillStreak >= TntWarsGame.Properties.DefaultStreakTwoAmount && TntWarsGame.GetTntWarsGame(C.p).Streaks)
                                                {
                                                    ExplodeDistance += 1;
                                                }
                                                level.MakeExplosion(x, y, z, ExplodeDistance - 2, true, TntWarsGame.GetTntWarsGame(C.p));
                                                List<Player> Killed = new List<Player>();
                                                level.players.ForEach(delegate(Player p1)
                                                {
                                                    if (p1.PlayingTntWars && p1 != C.p && Math.Abs((int)(p1.pos[0] / 32) - x) + Math.Abs((int)(p1.pos[1] / 32) - y) + Math.Abs((int)(p1.pos[2] / 32) - z) < ((ExplodeDistance * 3) + 1))
                                                    {
                                                        Killed.Add(p1);
                                                    }
                                                });
                                                TntWarsGame.GetTntWarsGame(C.p).HandleKill(C.p, Killed);
                                            }
                                        }
                                        //Normal
                                        else
                                        {
                                            if (level.physics < 3) level.Blockchange(x, y, z, Block.air);

                                            if (level.physics >= 3)
                                            {
                                                rand = new Random();

                                                if (C.time < 5 && level.physics == 3)
                                                {
                                                    C.time += 1;
                                                    level.Blockchange(x, (ushort)(y + 1), z,
                                                                level.GetTile(x, (ushort)(y + 1), z) ==
                                                                Block.lavastill
                                                                    ? (ushort)Block.air
                                                                    : Block.lavastill);
                                                    break;
                                                }

                                                level.MakeExplosion(x, y, z, 0);
                                            }
                                            else
                                            {
                                                level.Blockchange(x, y, z, Block.air);
                                            }
                                        }
                                        break;

                                    case Block.bigtnt:
                                        if (level.physics < 3) level.Blockchange(x, y, z, Block.air);

                                        if (level.physics >= 3)
                                        {
                                            rand = new Random();

                                            if (C.time < 5 && level.physics == 3)
                                            {
                                                C.time += 1;
                                                level.Blockchange(x, (ushort)(y + 1), z,
                                                            level.GetTile(x, (ushort)(y + 1), z) ==
                                                            Block.lavastill
                                                                ? (ushort)Block.air
                                                                : Block.lavastill);
                                                level.Blockchange(x, (ushort)(y - 1), z,
                                                            level.GetTile(x, (ushort)(y - 1), z) ==
                                                            Block.lavastill
                                                                ? (ushort)Block.air
                                                                : Block.lavastill);
                                                level.Blockchange((ushort)(x + 1), y, z,
                                                            level.GetTile((ushort)(x + 1), y, z) ==
                                                            Block.lavastill
                                                                ? (ushort)Block.air
                                                                : Block.lavastill);
                                                level.Blockchange((ushort)(x - 1), y, z,
                                                            level.GetTile((ushort)(x - 1), y, z) ==
                                                            Block.lavastill
                                                                ? (ushort)Block.air
                                                                : Block.lavastill);
                                                level.Blockchange(x, y, (ushort)(z + 1),
                                                            level.GetTile(x, y, (ushort)(z + 1)) ==
                                                            Block.lavastill
                                                                ? (ushort)Block.air
                                                                : Block.lavastill);
                                                level.Blockchange(x, y, (ushort)(z - 1),
                                                            level.GetTile(x, y, (ushort)(z - 1)) ==
                                                            Block.lavastill
                                                                ? (ushort)Block.air
                                                                : Block.lavastill);

                                                break;
                                            }

                                            level.MakeExplosion(x, y, z, 1);
                                        }
                                        else
                                        {
                                            level.Blockchange(x, y, z, Block.air);
                                        }
                                        break;

                                    case Block.nuketnt:
                                        if (level.physics < 3) level.Blockchange(x, y, z, Block.air);

                                        if (level.physics >= 3)
                                        {
                                            rand = new Random();

                                            if (C.time < 5 && level.physics == 3)
                                            {
                                                C.time += 1;
                                                level.Blockchange(x, (ushort)(y + 2), z,
                                                            level.GetTile(x, (ushort)(y + 2), z) ==
                                                            Block.lavastill
                                                                ? (ushort)Block.air
                                                                : Block.lavastill);
                                                level.Blockchange(x, (ushort)(y - 2), z,
                                                            level.GetTile(x, (ushort)(y - 2), z) ==
                                                            Block.lavastill
                                                                ? (ushort)Block.air
                                                                : Block.lavastill);
                                                level.Blockchange((ushort)(x + 1), y, z,
                                                            level.GetTile((ushort)(x + 1), y, z) ==
                                                            Block.lavastill
                                                                ? (ushort)Block.air
                                                                : Block.lavastill);
                                                level.Blockchange((ushort)(x - 1), y, z,
                                                            level.GetTile((ushort)(x - 1), y, z) ==
                                                            Block.lavastill
                                                                ? (ushort)Block.air
                                                                : Block.lavastill);
                                                level.Blockchange(x, y, (ushort)(z + 1),
                                                            level.GetTile(x, y, (ushort)(z + 1)) ==
                                                            Block.lavastill
                                                                ? (ushort)Block.air
                                                                : Block.lavastill);
                                                level.Blockchange(x, y, (ushort)(z - 1),
                                                            level.GetTile(x, y, (ushort)(z - 1)) ==
                                                            Block.lavastill
                                                                ? (ushort)Block.air
                                                                : Block.lavastill);

                                                break;
                                            }

                                            level.MakeExplosion(x, y, z, 4);
                                        }
                                        else
                                        {
                                            level.Blockchange(x, y, z, Block.air);
                                        }
                                        break;

                                    case Block.tntexplosion:
                                        if (rand.Next(1, 11) <= 7) level.AddUpdate(C.b, Block.air);
                                        break;

                                    case Block.train:
                                        if (rand.Next(1, 10) <= 5) mx = 1;
                                        else mx = -1;
                                        if (rand.Next(1, 10) <= 5) my = 1;
                                        else my = -1;
                                        if (rand.Next(1, 10) <= 5) mz = 1;
                                        else mz = -1;

                                        for (int cx = (-1 * mx);
                                             cx != ((1 * mx) + mx);
                                             cx = cx + (1 * mx))
                                            for (int cy = (-1 * my);
                                                 cy != ((1 * my) + my);
                                                 cy = cy + (1 * my))
                                                for (int cz = (-1 * mz);
                                                     cz != ((1 * mz) + mz);
                                                     cz = cz + (1 * mz))
                                                {
                                                    if (
                                                        level.GetTile((ushort)(x + cx),
                                                                (ushort)(y + cy - 1),
                                                                (ushort)(z + cz)) == Block.red &&
                                                        (level.GetTile((ushort)(x + cx),
                                                                 (ushort)(y + cy),
                                                                 (ushort)(z + cz)) == Block.air ||
                                                         level.GetTile((ushort)(x + cx),
                                                                 (ushort)(y + cy),
                                                                 (ushort)(z + cz)) == Block.water) &&
                                                        !InnerChange)
                                                    {
                                                        level.AddUpdate(
                                                            level.PosToInt((ushort)(x + cx),
                                                                     (ushort)(y + cy),
                                                                     (ushort)(z + cz)),
                                                            Block.train);
                                                        level.AddUpdate(level.PosToInt(x, y, z), Block.air);
                                                        level.AddUpdate(level.IntOffset(C.b, 0, -1, 0),
                                                                  Block.obsidian, true,
                                                                  "wait 5 revert " +
                                                                  Block.red.ToString());

                                                        InnerChange = true;
                                                        break;
                                                    }
                                                    if (
                                                        level.GetTile((ushort)(x + cx),
                                                                (ushort)(y + cy - 1),
                                                                (ushort)(z + cz)) == Block.op_air &&
                                                        (level.GetTile((ushort)(x + cx),
                                                                 (ushort)(y + cy),
                                                                 (ushort)(z + cz)) == Block.air ||
                                                         level.GetTile((ushort)(x + cx),
                                                                 (ushort)(y + cy),
                                                                 (ushort)(z + cz)) == Block.water) &&
                                                        !InnerChange)
                                                    {
                                                        level.AddUpdate(
                                                            level.PosToInt((ushort)(x + cx),
                                                                     (ushort)(y + cy),
                                                                     (ushort)(z + cz)),
                                                            Block.train);
                                                        level.AddUpdate(level.PosToInt(x, y, z), Block.air);
                                                        level.AddUpdate(level.IntOffset(C.b, 0, -1, 0),
                                                                  Block.glass, true,
                                                                  "wait 5 revert " +
                                                                  Block.op_air.ToString());

                                                        InnerChange = true;
                                                        break;
                                                    }
                                                }
                                        break;

                                    case Block.magma:
                                        C.time++;
                                        if (C.time < 3) break;

                                        if (level.GetTile(x, (ushort)(y - 1), z) == Block.air)
                                            level.AddUpdate(level.PosToInt(x, (ushort)(y - 1), z),
                                                      Block.magma);
                                        else if (level.GetTile(x, (ushort)(y - 1), z) != Block.magma)
                                        {
                                            PhysLava(level, level.PosToInt((ushort)(x + 1), y, z), level.blocks[C.b]);
                                            PhysLava(level, level.PosToInt((ushort)(x - 1), y, z), level.blocks[C.b]);
                                            PhysLava(level, level.PosToInt(x, y, (ushort)(z + 1)), level.blocks[C.b]);
                                            PhysLava(level, level.PosToInt(x, y, (ushort)(z - 1)), level.blocks[C.b]);
                                        }

                                        if (level.physics > 1)
                                        {
                                            if (C.time > 10)
                                            {
                                                C.time = 0;

                                                if (Block.LavaKill(level.GetTile((ushort)(x + 1), y, z)))
                                                {
                                                    level.AddUpdate(level.PosToInt((ushort)(x + 1), y, z),
                                                              Block.magma);
                                                    InnerChange = true;
                                                }
                                                if (Block.LavaKill(level.GetTile((ushort)(x - 1), y, z)))
                                                {
                                                    level.AddUpdate(level.PosToInt((ushort)(x - 1), y, z),
                                                              Block.magma);
                                                    InnerChange = true;
                                                }
                                                if (Block.LavaKill(level.GetTile(x, y, (ushort)(z + 1))))
                                                {
                                                    level.AddUpdate(level.PosToInt(x, y, (ushort)(z + 1)),
                                                              Block.magma);
                                                    InnerChange = true;
                                                }
                                                if (Block.LavaKill(level.GetTile(x, y, (ushort)(z - 1))))
                                                {
                                                    level.AddUpdate(level.PosToInt(x, y, (ushort)(z - 1)),
                                                              Block.magma);
                                                    InnerChange = true;
                                                }
                                                if (Block.LavaKill(level.GetTile(x, (ushort)(y - 1), z)))
                                                {
                                                    level.AddUpdate(level.PosToInt(x, (ushort)(y - 1), z),
                                                              Block.magma);
                                                    InnerChange = true;
                                                }

                                                if (InnerChange)
                                                {
                                                    if (
                                                        Block.LavaKill(level.GetTile(x, (ushort)(y + 1),
                                                                               z)))
                                                        level.AddUpdate(
                                                            level.PosToInt(x, (ushort)(y + 1), z),
                                                            Block.magma);
                                                }
                                            }
                                        }

                                        break;
                                    case Block.geyser:
                                        C.time++;

                                        if (level.GetTile(x, (ushort)(y - 1), z) == Block.air)
                                            level.AddUpdate(level.PosToInt(x, (ushort)(y - 1), z),
                                                      Block.geyser);
                                        else if (level.GetTile(x, (ushort)(y - 1), z) != Block.geyser)
                                        {
                                            PhysWater(level, level.PosToInt((ushort)(x + 1), y, z),
                                                      level.blocks[C.b]);
                                            PhysWater(level, level.PosToInt((ushort)(x - 1), y, z),
                                                      level.blocks[C.b]);
                                            PhysWater(level, level.PosToInt(x, y, (ushort)(z + 1)),
                                                      level.blocks[C.b]);
                                            PhysWater(level, level.PosToInt(x, y, (ushort)(z - 1)),
                                                      level.blocks[C.b]);
                                        }

                                        if (level.physics > 1)
                                        {
                                            if (C.time > 10)
                                            {
                                                C.time = 0;

                                                if (
                                                    Block.WaterKill(level.GetTile((ushort)(x + 1), y, z)))
                                                {
                                                    level.AddUpdate(level.PosToInt((ushort)(x + 1), y, z),
                                                              Block.geyser);
                                                    InnerChange = true;
                                                }
                                                if (
                                                    Block.WaterKill(level.GetTile((ushort)(x - 1), y, z)))
                                                {
                                                    level.AddUpdate(level.PosToInt((ushort)(x - 1), y, z),
                                                              Block.geyser);
                                                    InnerChange = true;
                                                }
                                                if (
                                                    Block.WaterKill(level.GetTile(x, y, (ushort)(z + 1))))
                                                {
                                                    level.AddUpdate(level.PosToInt(x, y, (ushort)(z + 1)),
                                                              Block.geyser);
                                                    InnerChange = true;
                                                }
                                                if (
                                                    Block.WaterKill(level.GetTile(x, y, (ushort)(z - 1))))
                                                {
                                                    level.AddUpdate(level.PosToInt(x, y, (ushort)(z - 1)),
                                                              Block.geyser);
                                                    InnerChange = true;
                                                }
                                                if (
                                                    Block.WaterKill(level.GetTile(x, (ushort)(y - 1), z)))
                                                {
                                                    level.AddUpdate(level.PosToInt(x, (ushort)(y - 1), z),
                                                              Block.geyser);
                                                    InnerChange = true;
                                                }

                                                if (InnerChange)
                                                {
                                                    if (
                                                        Block.WaterKill(level.GetTile(x,
                                                                                (ushort)(y + 1),
                                                                                z)))
                                                        level.AddUpdate(
                                                            level.PosToInt(x, (ushort)(y + 1), z),
                                                            Block.geyser);
                                                }
                                            }
                                        }
                                        break;

                                    case Block.birdblack:
                                    case Block.birdwhite:
                                    case Block.birdlava:
                                    case Block.birdwater:
                                        switch (rand.Next(1, 15))
                                        {
                                            case 1:
                                                if (level.GetTile(x, (ushort)(y - 1), z) == Block.air)
                                                    level.AddUpdate(level.PosToInt(x, (ushort)(y - 1), z),
                                                              level.blocks[C.b]);
                                                else goto case 3;
                                                break;
                                            case 2:
                                                if (level.GetTile(x, (ushort)(y + 1), z) == Block.air)
                                                    level.AddUpdate(level.PosToInt(x, (ushort)(y + 1), z),
                                                              level.blocks[C.b]);
                                                else goto case 6;
                                                break;
                                            case 3:
                                            case 4:
                                            case 5:
                                                switch (level.GetTile((ushort)(x - 1), y, z))
                                                {
                                                    case Block.air:
                                                        level.AddUpdate(level.PosToInt((ushort)(x - 1), y, z),
                                                                  level.blocks[C.b]);
                                                        break;
                                                    case Block.op_air:
                                                        break;
                                                    default:
                                                        level.AddUpdate(C.b, Block.red, false,
                                                                  "dissipate 25");
                                                        break;
                                                }
                                                break;
                                            case 6:
                                            case 7:
                                            case 8:
                                                switch (level.GetTile((ushort)(x + 1), y, z))
                                                {
                                                    case Block.air:
                                                        level.AddUpdate(level.PosToInt((ushort)(x + 1), y, z),
                                                                  level.blocks[C.b]);
                                                        break;
                                                    case Block.op_air:
                                                        break;
                                                    default:
                                                        level.AddUpdate(C.b, Block.red, false,
                                                                  "dissipate 25");
                                                        break;
                                                }
                                                break;
                                            case 9:
                                            case 10:
                                            case 11:
                                                switch (level.GetTile(x, y, (ushort)(z - 1)))
                                                {
                                                    case Block.air:
                                                        level.AddUpdate(level.PosToInt(x, y, (ushort)(z - 1)),
                                                                  level.blocks[C.b]);
                                                        break;
                                                    case Block.op_air:
                                                        break;
                                                    default:
                                                        level.AddUpdate(C.b, Block.red, false,
                                                                  "dissipate 25");
                                                        break;
                                                }
                                                break;
                                            default:
                                                switch (level.GetTile(x, y, (ushort)(z + 1)))
                                                {
                                                    case Block.air:
                                                        level.AddUpdate(level.PosToInt(x, y, (ushort)(z + 1)),
                                                                  level.blocks[C.b]);
                                                        break;
                                                    case Block.op_air:
                                                        break;
                                                    default:
                                                        level.AddUpdate(C.b, Block.red, false,
                                                                  "dissipate 25");
                                                        break;
                                                }
                                                break;
                                        }
                                        level.AddUpdate(C.b, Block.air);
                                        C.time = 255;

                                        break;

                                    case Block.snaketail:
                                        if (level.GetTile(level.IntOffset(C.b, -1, 0, 0)) != Block.snake ||
                                            level.GetTile(level.IntOffset(C.b, 1, 0, 0)) != Block.snake ||
                                            level.GetTile(level.IntOffset(C.b, 0, 0, 1)) != Block.snake ||
                                            level.GetTile(level.IntOffset(C.b, 0, 0, -1)) != Block.snake)
                                            C.extraInfo = "revert 0";
                                        break;
                                    case Block.snake:

                                        #region SNAKE

                                        if (level.ai)
                                            Player.players.ForEach(delegate(Player p)
                                            {
                                                if (p.level == level&&
                                                    !p.invincible)
                                                {
                                                    currentNum =
                                                        Math.Abs(
                                                            (p.pos[0] /
                                                             32) - x) +
                                                        Math.Abs(
                                                            (p.pos[1] /
                                                             32) - y) +
                                                        Math.Abs(
                                                            (p.pos[2] /
                                                             32) - z);
                                                    if (currentNum <
                                                        foundNum)
                                                    {
                                                        foundNum =
                                                            currentNum;
                                                        foundPlayer = p;
                                                    }
                                                }
                                            });

                                    randomMovement_Snake:
                                        if (foundPlayer != null && rand.Next(1, 20) < 19)
                                        {
                                            currentNum = rand.Next(1, 10);
                                            foundNum = 0;

                                            switch (currentNum)
                                            {
                                                case 1:
                                                case 2:
                                                case 3:
                                                    if ((foundPlayer.pos[0] / 32) - x != 0)
                                                    {
                                                        newNum =
                                                            level.PosToInt(
                                                                (ushort)
                                                                (x +
                                                                 Math.Sign((foundPlayer.pos[0] / 32) -
                                                                           x)), y, z);
                                                        if (level.GetTile(newNum) == Block.air)
                                                            if (level.IntOffset(newNum, -1, 0, 0) ==
                                                                Block.grass ||
                                                                level.IntOffset(newNum, -1, 0, 0) ==
                                                                Block.dirt)
                                                                if (level.AddUpdate(newNum, level.blocks[C.b]))
                                                                    goto removeSelf_Snake;
                                                    }
                                                    foundNum++;
                                                    if (foundNum >= 3) goto default;
                                                    goto case 4;

                                                case 4:
                                                case 5:
                                                case 6:
                                                    if ((foundPlayer.pos[1] / 32) - y != 0)
                                                    {
                                                        newNum = level.PosToInt(x,
                                                                          (ushort)
                                                                          (y +
                                                                           Math.Sign(
                                                                               (foundPlayer.pos[1] /
                                                                                32) - y)), z);
                                                        if (level.GetTile(newNum) == Block.air)
                                                            if (newNum > 0)
                                                            {
                                                                if (level.IntOffset(newNum, 0, 1, 0) ==
                                                                    Block.grass ||
                                                                    level.IntOffset(newNum, 0, 1, 0) ==
                                                                    Block.dirt &&
                                                                    level.IntOffset(newNum, 0, 2, 0) ==
                                                                    Block.air)
                                                                    if (level.AddUpdate(newNum,
                                                                                  level.blocks[C.b]))
                                                                        goto removeSelf_Snake;
                                                            }
                                                            else if (newNum < 0)
                                                            {
                                                                if (level.IntOffset(newNum, 0, -2, 0) ==
                                                                    Block.grass ||
                                                                    level.IntOffset(newNum, 0, -2, 0) ==
                                                                    Block.dirt &&
                                                                    level.IntOffset(newNum, 0, -1, 0) ==
                                                                    Block.air)
                                                                    if (level.AddUpdate(newNum,
                                                                                  level.blocks[C.b]))
                                                                        goto removeSelf_Snake;
                                                            }
                                                    }
                                                    foundNum++;
                                                    if (foundNum >= 3) goto default;
                                                    goto case 7;

                                                case 7:
                                                case 8:
                                                case 9:
                                                    if ((foundPlayer.pos[2] / 32) - z != 0)
                                                    {
                                                        newNum = level.PosToInt(x, y,
                                                                          (ushort)
                                                                          (z +
                                                                           Math.Sign(
                                                                               (foundPlayer.pos[2] /
                                                                                32) - z)));
                                                        if (level.GetTile(newNum) == Block.air)
                                                            if (level.IntOffset(newNum, 0, 0, -1) ==
                                                                Block.grass ||
                                                                level.IntOffset(newNum, 0, 0, -1) ==
                                                                Block.dirt)
                                                                if (level.AddUpdate(newNum, level.blocks[C.b]))
                                                                    goto removeSelf_Snake;
                                                    }
                                                    foundNum++;
                                                    if (foundNum >= 3) goto default;
                                                    else goto case 1;
                                                default:
                                                    foundPlayer = null;
                                                    goto randomMovement_Snake;
                                            }
                                        }
                                        else
                                        {
                                            switch (rand.Next(1, 13))
                                            {
                                                case 1:
                                                case 2:
                                                case 3:
                                                    newNum = level.IntOffset(C.b, -1, 0, 0);
                                                    oldNum = level.PosToInt(x, y, z);

                                                    if (level.GetTile(level.IntOffset(newNum, 0, -1, 0)) ==
                                                        Block.air && level.GetTile(newNum) == Block.air)
                                                        newNum = level.IntOffset(newNum, 0, -1, 0);
                                                    else if (level.GetTile(newNum) == Block.air &&
                                                             level.GetTile(level.IntOffset(newNum, 0, 1, 0)) ==
                                                             Block.air)
                                                    {
                                                    }

                                                    else if (
                                                        level.GetTile(level.IntOffset(newNum, 0, 2, 0)) ==
                                                        Block.air &&
                                                        level.GetTile(level.IntOffset(newNum, 0, 1, 0)) ==
                                                        Block.air)
                                                        newNum = level.IntOffset(newNum, 0, 1, 0);
                                                    else skip = true; //Not used...

                                                    if (level.AddUpdate(newNum, level.blocks[C.b]))
                                                    {
                                                        level.AddUpdate(level.IntOffset(oldNum, 0, 0, 0),
                                                                  Block.snaketail, true,
                                                                  string.Format("wait 5 revert {0}", Block.air));
                                                        goto removeSelf_Snake;
                                                    }

                                                    foundNum++;
                                                    if (foundNum >= 4) InnerChange = true;
                                                    else goto case 4;
                                                    break;

                                                case 4:
                                                case 5:
                                                case 6:
                                                    newNum = level.IntOffset(C.b, 1, 0, 0);
                                                    oldNum = level.PosToInt(x, y, z);

                                                    if (level.GetTile(level.IntOffset(newNum, 0, -1, 0)) ==
                                                        Block.air && level.GetTile(newNum) == Block.air)
                                                        newNum = level.IntOffset(newNum, 0, -1, 0);
                                                    else if (level.GetTile(newNum) == Block.air &&
                                                             level.GetTile(level.IntOffset(newNum, 0, 1, 0)) ==
                                                             Block.air)
                                                    {
                                                    }

                                                    else if (
                                                        level.GetTile(level.IntOffset(newNum, 0, 2, 0)) ==
                                                        Block.air &&
                                                        level.GetTile(level.IntOffset(newNum, 0, 1, 0)) ==
                                                        Block.air)
                                                        newNum = level.IntOffset(newNum, 0, 1, 0);
                                                    else skip = true;

                                                    if (level.AddUpdate(newNum, level.blocks[C.b]))
                                                    {
                                                        level.AddUpdate(level.IntOffset(oldNum, 0, 0, 0),
                                                                  Block.snaketail, true,
                                                                  "wait 5 revert 0");
                                                        goto removeSelf_Snake;
                                                    }

                                                    foundNum++;
                                                    if (foundNum >= 4) InnerChange = true;
                                                    else goto case 7;
                                                    break;

                                                case 7:
                                                case 8:
                                                case 9:
                                                    newNum = level.IntOffset(C.b, 0, 0, 1);
                                                    oldNum = level.PosToInt(x, y, z);

                                                    if (level.GetTile(level.IntOffset(newNum, 0, -1, 0)) ==
                                                        Block.air && level.GetTile(newNum) == Block.air)
                                                        newNum = level.IntOffset(newNum, 0, -1, 0);
                                                    else if (level.GetTile(newNum) == Block.air &&
                                                             level.GetTile(level.IntOffset(newNum, 0, 1, 0)) ==
                                                             Block.air)
                                                    {
                                                    }

                                                    else if (
                                                        level.GetTile(level.IntOffset(newNum, 0, 2, 0)) ==
                                                        Block.air &&
                                                        level.GetTile(level.IntOffset(newNum, 0, 1, 0)) ==
                                                        Block.air)
                                                        newNum = level.IntOffset(newNum, 0, 1, 0);
                                                    else skip = true;

                                                    if (level.AddUpdate(newNum, level.blocks[C.b]))
                                                    {
                                                        level.AddUpdate(level.IntOffset(oldNum, 0, 0, 0),
                                                                  Block.snaketail, true,
                                                                  "wait 5 revert 0");
                                                        goto removeSelf_Snake;
                                                    }

                                                    foundNum++;
                                                    if (foundNum >= 4) InnerChange = true;
                                                    else goto case 10;
                                                    break;
                                                case 10:
                                                case 11:
                                                case 12:
                                                default:
                                                    newNum = level.IntOffset(C.b, 0, 0, -1);
                                                    oldNum = level.PosToInt(x, y, z);

                                                    if (level.GetTile(level.IntOffset(newNum, 0, -1, 0)) ==
                                                        Block.air && level.GetTile(newNum) == Block.air)
                                                        newNum = level.IntOffset(newNum, 0, -1, 0);
                                                    else if (level.GetTile(newNum) == Block.air &&
                                                             level.GetTile(level.IntOffset(newNum, 0, 1, 0)) ==
                                                             Block.air)
                                                    {
                                                    }

                                                    else if (
                                                        level.GetTile(level.IntOffset(newNum, 0, 2, 0)) ==
                                                        Block.air &&
                                                        level.GetTile(level.IntOffset(newNum, 0, 1, 0)) ==
                                                        Block.air)
                                                        newNum = level.IntOffset(newNum, 0, 1, 0);
                                                    else skip = true;

                                                    if (level.AddUpdate(newNum, level.blocks[C.b]))
                                                    {
                                                        level.AddUpdate(level.IntOffset(oldNum, 0, 0, 0),
                                                                  Block.snaketail, true,
                                                                  "wait 5 revert 0");
                                                        goto removeSelf_Snake;
                                                    }

                                                    foundNum++;
                                                    if (foundNum >= 4) InnerChange = true;
                                                    else goto case 1;
                                                    break;
                                            }
                                        }

                                    removeSelf_Snake:
                                        if (!InnerChange)
                                            level.AddUpdate(C.b, Block.air);
                                        break;

                                        #endregion

                                    case Block.birdred:
                                    case Block.birdblue:
                                    case Block.birdkill:

                                        #region HUNTER BIRDS

                                        if (level.ai)
                                            Player.players.ForEach(delegate(Player p)
                                            {
                                                if (p.level == level&&
                                                    !p.invincible)
                                                {
                                                    currentNum =
                                                        Math.Abs(
                                                            (p.pos[0] /
                                                             32) - x) +
                                                        Math.Abs(
                                                            (p.pos[1] /
                                                             32) - y) +
                                                        Math.Abs(
                                                            (p.pos[2] /
                                                             32) - z);
                                                    if (currentNum <
                                                        foundNum)
                                                    {
                                                        foundNum =
                                                            currentNum;
                                                        foundPlayer = p;
                                                    }
                                                }
                                            });

                                    randomMovement:
                                        if (foundPlayer != null && rand.Next(1, 20) < 19)
                                        {
                                            currentNum = rand.Next(1, 10);
                                            foundNum = 0;

                                            switch (currentNum)
                                            {
                                                case 1:
                                                case 2:
                                                case 3:
                                                    if ((foundPlayer.pos[0] / 32) - x != 0)
                                                    {
                                                        newNum =
                                                            level.PosToInt(
                                                                (ushort)
                                                                (x +
                                                                 Math.Sign((foundPlayer.pos[0] / 32) -
                                                                           x)), y, z);
                                                        if (level.GetTile(newNum) == Block.air)
                                                            if (level.AddUpdate(newNum, level.blocks[C.b]))
                                                                goto removeSelf;
                                                    }

                                                    foundNum++;
                                                    if (foundNum >= 3) goto default;
                                                    else goto case 4;
                                                case 4:
                                                case 5:
                                                case 6:
                                                    if ((foundPlayer.pos[1] / 32) - y != 0)
                                                    {
                                                        newNum = level.PosToInt(x,
                                                                          (ushort)
                                                                          (y +
                                                                           Math.Sign(
                                                                               (foundPlayer.pos[1] /
                                                                                32) - y)), z);
                                                        if (level.GetTile(newNum) == Block.air)
                                                            if (level.AddUpdate(newNum, level.blocks[C.b]))
                                                                goto removeSelf;
                                                    }

                                                    foundNum++;
                                                    if (foundNum >= 3) goto default;
                                                    else goto case 7;
                                                case 7:
                                                case 8:
                                                case 9:
                                                    if ((foundPlayer.pos[2] / 32) - z != 0)
                                                    {
                                                        newNum = level.PosToInt(x, y,
                                                                          (ushort)
                                                                          (z +
                                                                           Math.Sign(
                                                                               (foundPlayer.pos[2] /
                                                                                32) - z)));
                                                        if (level.GetTile(newNum) == Block.air)
                                                            if (level.AddUpdate(newNum, level.blocks[C.b]))
                                                                goto removeSelf;
                                                    }

                                                    foundNum++;
                                                    if (foundNum >= 3) goto default;
                                                    else goto case 1;
                                                default:
                                                    foundPlayer = null;
                                                    goto randomMovement;
                                            }
                                        }
                                        else
                                        {
                                            switch (rand.Next(1, 15))
                                            {
                                                case 1:
                                                    if (level.GetTile(x, (ushort)(y - 1), z) ==
                                                        Block.air)
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt(x, (ushort)(y - 1), z),
                                                                level.blocks[C.b])) break;
                                                        else goto case 3;
                                                    else goto case 3;
                                                case 2:
                                                    if (level.GetTile(x, (ushort)(y + 1), z) ==
                                                        Block.air)
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt(x, (ushort)(y + 1), z),
                                                                level.blocks[C.b])) break;
                                                        else goto case 6;
                                                    else goto case 6;
                                                case 3:
                                                case 4:
                                                case 5:
                                                    if (level.GetTile((ushort)(x - 1), y, z) ==
                                                        Block.air)
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt((ushort)(x - 1), y, z),
                                                                level.blocks[C.b])) break;
                                                        else goto case 9;
                                                    else goto case 9;
                                                case 6:
                                                case 7:
                                                case 8:
                                                    if (level.GetTile((ushort)(x + 1), y, z) ==
                                                        Block.air)
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt((ushort)(x + 1), y, z),
                                                                level.blocks[C.b])) break;
                                                        else goto case 12;
                                                    else goto case 12;
                                                case 9:
                                                case 10:
                                                case 11:
                                                    if (level.GetTile(x, y, (ushort)(z - 1)) ==
                                                        Block.air)
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt(x, y, (ushort)(z - 1)),
                                                                level.blocks[C.b])) break;
                                                        else InnerChange = true;
                                                    else InnerChange = true;
                                                    break;
                                                case 12:
                                                case 13:
                                                case 14:
                                                default:
                                                    if (level.GetTile(x, y, (ushort)(z + 1)) ==
                                                        Block.air)
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt(x, y, (ushort)(z + 1)),
                                                                level.blocks[C.b])) break;
                                                        else InnerChange = true;
                                                    else InnerChange = true;
                                                    break;
                                            }
                                        }

                                    removeSelf:
                                        if (!InnerChange)
                                            level.AddUpdate(C.b, Block.air);
                                        break;

                                        #endregion

                                    case Block.fishbetta:
                                    case Block.fishgold:
                                    case Block.fishsalmon:
                                    case Block.fishshark:
                                    case Block.fishsponge:

                                        #region FISH

                                        if (level.ai)
                                            Player.players.ForEach(delegate(Player p)
                                            {
                                                if (p.level == level&&
                                                    !p.invincible)
                                                {
                                                    currentNum =
                                                        Math.Abs(
                                                            (p.pos[0] /
                                                             32) - x) +
                                                        Math.Abs(
                                                            (p.pos[1] /
                                                             32) - y) +
                                                        Math.Abs(
                                                            (p.pos[2] /
                                                             32) - z);
                                                    if (currentNum <
                                                        foundNum)
                                                    {
                                                        foundNum =
                                                            currentNum;
                                                        foundPlayer = p;
                                                    }
                                                }
                                            });

                                    randomMovement_fish:
                                        if (foundPlayer != null && rand.Next(1, 20) < 19)
                                        {
                                            currentNum = rand.Next(1, 10);
                                            foundNum = 0;

                                            switch (currentNum)
                                            {
                                                case 1:
                                                case 2:
                                                case 3:
                                                    if ((foundPlayer.pos[0] / 32) - x != 0)
                                                    {
                                                        if (level.blocks[C.b] == Block.fishbetta ||
                                                            level.blocks[C.b] == Block.fishshark)
                                                            newNum =
                                                                level.PosToInt(
                                                                    (ushort)
                                                                    (x +
                                                                     Math.Sign(
                                                                         (foundPlayer.pos[0] / 32) -
                                                                         x)), y, z);
                                                        else
                                                            newNum =
                                                                level.PosToInt(
                                                                    (ushort)
                                                                    (x -
                                                                     Math.Sign(
                                                                         (foundPlayer.pos[0] / 32) -
                                                                         x)), y, z);


                                                        if (level.GetTile(newNum) == Block.water)
                                                            if (level.AddUpdate(newNum, level.blocks[C.b]))
                                                                goto removeSelf_fish;
                                                    }

                                                    foundNum++;
                                                    if (foundNum >= 3) goto default;
                                                    else goto case 4;
                                                case 4:
                                                case 5:
                                                case 6:
                                                    if ((foundPlayer.pos[1] / 32) - y != 0)
                                                    {
                                                        if (level.blocks[C.b] == Block.fishbetta ||
                                                            level.blocks[C.b] == Block.fishshark)
                                                            newNum = level.PosToInt(x,
                                                                              (ushort)
                                                                              (y +
                                                                               Math.Sign(
                                                                                   (foundPlayer.
                                                                                        pos[1] / 32) -
                                                                                   y)), z);
                                                        else
                                                            newNum = level.PosToInt(x,
                                                                              (ushort)
                                                                              (y -
                                                                               Math.Sign(
                                                                                   (foundPlayer.
                                                                                        pos[1] / 32) -
                                                                                   y)), z);

                                                        if (level.GetTile(newNum) == Block.water)
                                                            if (level.AddUpdate(newNum, level.blocks[C.b]))
                                                                goto removeSelf_fish;
                                                    }

                                                    foundNum++;
                                                    if (foundNum >= 3) goto default;
                                                    else goto case 7;
                                                case 7:
                                                case 8:
                                                case 9:
                                                    if ((foundPlayer.pos[2] / 32) - z != 0)
                                                    {
                                                        if (level.blocks[C.b] == Block.fishbetta ||
                                                            level.blocks[C.b] == Block.fishshark)
                                                            newNum = level.PosToInt(x, y,
                                                                              (ushort)
                                                                              (z +
                                                                               Math.Sign(
                                                                                   (foundPlayer.
                                                                                        pos[2] / 32) -
                                                                                   z)));
                                                        else
                                                            newNum = level.PosToInt(x, y,
                                                                              (ushort)
                                                                              (z -
                                                                               Math.Sign(
                                                                                   (foundPlayer.
                                                                                        pos[2] / 32) -
                                                                                   z)));

                                                        if (level.GetTile(newNum) == Block.water)
                                                            if (level.AddUpdate(newNum, level.blocks[C.b]))
                                                                goto removeSelf_fish;
                                                    }

                                                    foundNum++;
                                                    if (foundNum >= 3) goto default;
                                                    else goto case 1;
                                                default:
                                                    foundPlayer = null;
                                                    goto randomMovement_fish;
                                            }
                                        }
                                        else
                                        {
                                            switch (rand.Next(1, 15))
                                            {
                                                case 1:
                                                    if (level.GetTile(x, (ushort)(y - 1), z) ==
                                                        Block.water)
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt(x, (ushort)(y - 1), z),
                                                                level.blocks[C.b])) break;
                                                        else goto case 3;
                                                    else goto case 3;
                                                case 2:
                                                    if (level.GetTile(x, (ushort)(y + 1), z) ==
                                                        Block.water)
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt(x, (ushort)(y + 1), z),
                                                                level.blocks[C.b])) break;
                                                        else goto case 6;
                                                    else goto case 6;
                                                case 3:
                                                case 4:
                                                case 5:
                                                    if (level.GetTile((ushort)(x - 1), y, z) ==
                                                        Block.water)
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt((ushort)(x - 1), y, z),
                                                                level.blocks[C.b])) break;
                                                        else goto case 9;
                                                    else goto case 9;
                                                case 6:
                                                case 7:
                                                case 8:
                                                    if (level.GetTile((ushort)(x + 1), y, z) ==
                                                        Block.water)
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt((ushort)(x + 1), y, z),
                                                                level.blocks[C.b])) break;
                                                        else goto case 12;
                                                    else goto case 12;
                                                case 9:
                                                case 10:
                                                case 11:
                                                    if (level.GetTile(x, y, (ushort)(z - 1)) ==
                                                        Block.water)
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt(x, y, (ushort)(z - 1)),
                                                                level.blocks[C.b])) break;
                                                        else InnerChange = true;
                                                    else InnerChange = true;
                                                    break;
                                                case 12:
                                                case 13:
                                                case 14:
                                                default:
                                                    if (level.GetTile(x, y, (ushort)(z + 1)) ==
                                                        Block.water)
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt(x, y, (ushort)(z + 1)),
                                                                level.blocks[C.b])) break;
                                                        else InnerChange = true;
                                                    else InnerChange = true;
                                                    break;
                                            }
                                        }

                                    removeSelf_fish:
                                        if (!InnerChange)
                                            level.AddUpdate(C.b, Block.water);
                                        break;

                                        #endregion

                                    case Block.fishlavashark:

                                        #region lavafish

                                        if (level.ai)
                                            Player.players.ForEach(delegate(Player p)
                                            {
                                                if (p.level == level&&
                                                    !p.invincible)
                                                {
                                                    currentNum =
                                                        Math.Abs(
                                                            (p.pos[0] /
                                                             32) - x) +
                                                        Math.Abs(
                                                            (p.pos[1] /
                                                             32) - y) +
                                                        Math.Abs(
                                                            (p.pos[2] /
                                                             32) - z);
                                                    if (currentNum <
                                                        foundNum)
                                                    {
                                                        foundNum =
                                                            currentNum;
                                                        foundPlayer = p;
                                                    }
                                                }
                                            });

                                    randomMovement_lavafish:
                                        if (foundPlayer != null && rand.Next(1, 20) < 19)
                                        {
                                            currentNum = rand.Next(1, 10);
                                            foundNum = 0;

                                            switch (currentNum)
                                            {
                                                case 1:
                                                case 2:
                                                case 3:
                                                    if ((foundPlayer.pos[0] / 32) - x != 0)
                                                    {
                                                        if (level.blocks[C.b] == Block.fishlavashark)
                                                            newNum =
                                                                level.PosToInt(
                                                                    (ushort)
                                                                    (x +
                                                                     Math.Sign(
                                                                         (foundPlayer.pos[0] / 32) -
                                                                         x)), y, z);
                                                        else
                                                            newNum =
                                                                level.PosToInt(
                                                                    (ushort)
                                                                    (x -
                                                                     Math.Sign(
                                                                         (foundPlayer.pos[0] / 32) -
                                                                         x)), y, z);


                                                        if (level.GetTile(newNum) == Block.lava)
                                                            if (level.AddUpdate(newNum, level.blocks[C.b]))
                                                                goto removeSelf_lavafish;
                                                    }

                                                    foundNum++;
                                                    if (foundNum >= 3) goto default;
                                                    else goto case 4;
                                                case 4:
                                                case 5:
                                                case 6:
                                                    if ((foundPlayer.pos[1] / 32) - y != 0)
                                                    {
                                                        if (level.blocks[C.b] == Block.fishlavashark)
                                                            newNum = level.PosToInt(x,
                                                                              (ushort)
                                                                              (y +
                                                                               Math.Sign(
                                                                                   (foundPlayer.
                                                                                        pos[1] / 32) -
                                                                                   y)), z);
                                                        else
                                                            newNum = level.PosToInt(x,
                                                                              (ushort)
                                                                              (y -
                                                                               Math.Sign(
                                                                                   (foundPlayer.
                                                                                        pos[1] / 32) -
                                                                                   y)), z);

                                                        if (level.GetTile(newNum) == Block.lava)
                                                            if (level.AddUpdate(newNum, level.blocks[C.b]))
                                                                goto removeSelf_lavafish;
                                                    }

                                                    foundNum++;
                                                    if (foundNum >= 3) goto default;
                                                    else goto case 7;
                                                case 7:
                                                case 8:
                                                case 9:
                                                    if ((foundPlayer.pos[2] / 32) - z != 0)
                                                    {
                                                        if (level.blocks[C.b] == Block.fishlavashark)
                                                            newNum = level.PosToInt(x, y,
                                                                              (ushort)
                                                                              (z +
                                                                               Math.Sign(
                                                                                   (foundPlayer.
                                                                                        pos[2] / 32) -
                                                                                   z)));
                                                        else
                                                            newNum = level.PosToInt(x, y,
                                                                              (ushort)
                                                                              (z -
                                                                               Math.Sign(
                                                                                   (foundPlayer.
                                                                                        pos[2] / 32) -
                                                                                   z)));

                                                        if (level.GetTile(newNum) == Block.lava)
                                                            if (level.AddUpdate(newNum, level.blocks[C.b]))
                                                                goto removeSelf_lavafish;
                                                    }

                                                    foundNum++;
                                                    if (foundNum >= 3) goto default;
                                                    else goto case 1;
                                                default:
                                                    foundPlayer = null;
                                                    goto randomMovement_lavafish;
                                            }
                                        }
                                        else
                                        {
                                            switch (rand.Next(1, 15))
                                            {
                                                case 1:
                                                    if (level.GetTile(x, (ushort)(y - 1), z) ==
                                                        Block.lava)
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt(x, (ushort)(y - 1), z),
                                                                level.blocks[C.b])) break;
                                                        else goto case 3;
                                                    else goto case 3;
                                                case 2:
                                                    if (level.GetTile(x, (ushort)(y + 1), z) ==
                                                        Block.lava)
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt(x, (ushort)(y + 1), z),
                                                                level.blocks[C.b])) break;
                                                        else goto case 6;
                                                    else goto case 6;
                                                case 3:
                                                case 4:
                                                case 5:
                                                    if (level.GetTile((ushort)(x - 1), y, z) ==
                                                        Block.lava)
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt((ushort)(x - 1), y, z),
                                                                level.blocks[C.b])) break;
                                                        else goto case 9;
                                                    else goto case 9;
                                                case 6:
                                                case 7:
                                                case 8:
                                                    if (level.GetTile((ushort)(x + 1), y, z) ==
                                                        Block.lava)
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt((ushort)(x + 1), y, z),
                                                                level.blocks[C.b])) break;
                                                        else goto case 12;
                                                    else goto case 12;
                                                case 9:
                                                case 10:
                                                case 11:
                                                    if (level.GetTile(x, y, (ushort)(z - 1)) ==
                                                        Block.lava)
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt(x, y, (ushort)(z - 1)),
                                                                level.blocks[C.b])) break;
                                                        else InnerChange = true;
                                                    else InnerChange = true;
                                                    break;
                                                case 12:
                                                case 13:
                                                case 14:
                                                default:
                                                    if (level.GetTile(x, y, (ushort)(z + 1)) ==
                                                        Block.lava)
                                                        if (
                                                            level.AddUpdate(
                                                                level.PosToInt(x, y, (ushort)(z + 1)),
                                                                level.blocks[C.b])) break;
                                                        else InnerChange = true;
                                                    else InnerChange = true;
                                                    break;
                                            }
                                        }

                                    removeSelf_lavafish:
                                        if (!InnerChange)
                                            level.AddUpdate(C.b, Block.lava);
                                        break;

                                        #endregion

                                    case Block.rockethead:
                                        if (rand.Next(1, 10) <= 5) mx = 1;
                                        else mx = -1;
                                        if (rand.Next(1, 10) <= 5) my = 1;
                                        else my = -1;
                                        if (rand.Next(1, 10) <= 5) mz = 1;
                                        else mz = -1;

                                        for (int cx = (-1 * mx);
                                             cx != ((1 * mx) + mx) && InnerChange == false;
                                             cx = cx + (1 * mx))
                                            for (int cy = (-1 * my);
                                                 cy != ((1 * my) + my) && InnerChange == false;
                                                 cy = cy + (1 * my))
                                                for (int cz = (-1 * mz);
                                                     cz != ((1 * mz) + mz) && InnerChange == false;
                                                     cz = cz + (1 * mz))
                                                {
                                                    if (
                                                        level.GetTile((ushort)(x + cx),
                                                                (ushort)(y + cy),
                                                                (ushort)(z + cz)) == Block.fire)
                                                    {
                                                        int bp1 = level.PosToInt((ushort)(x - cx),
                                                                           (ushort)(y - cy),
                                                                           (ushort)(z - cz));
                                                        int bp2 = level.PosToInt(x, y, z);
                                                        bool unblocked =
                                                            !level.ListUpdate.Exists(
                                                                Update => Update.b == bp1) &&
                                                            !level.ListUpdate.Exists(
                                                                Update => Update.b == bp2);
                                                        if (unblocked &&
                                                            level.GetTile((ushort)(x - cx),
                                                                    (ushort)(y - cy),
                                                                    (ushort)(z - cz)) ==
                                                            Block.air ||
                                                            level.GetTile((ushort)(x - cx),
                                                                    (ushort)(y - cy),
                                                                    (ushort)(z - cz)) ==
                                                            Block.rocketstart)
                                                        {
                                                            level.AddUpdate(
                                                                level.PosToInt((ushort)(x - cx),
                                                                         (ushort)(y - cy),
                                                                         (ushort)(z - cz)),
                                                                Block.rockethead);
                                                            level.AddUpdate(level.PosToInt(x, y, z),
                                                                      Block.fire);
                                                        }
                                                        else if (
                                                            level.GetTile((ushort)(x - cx),
                                                                    (ushort)(y - cy),
                                                                    (ushort)(z - cz)) ==
                                                            Block.fire)
                                                        {
                                                        }
                                                        else
                                                        {
                                                            if (level.physics > 2)
                                                                level.MakeExplosion(x, y, z, 2);
                                                            else
                                                                level.AddUpdate(level.PosToInt(x, y, z),
                                                                          Block.fire);
                                                        }
                                                        InnerChange = true;
                                                    }
                                                }
                                        break;

                                    case Block.firework:
                                        if (level.GetTile(x, (ushort)(y - 1), z) == Block.lavastill)
                                        {
                                            if (level.GetTile(x, (ushort)(y + 1), z) == Block.air)
                                            {
                                                if ((level.depth / 100) * 80 < y) mx = rand.Next(1, 20);
                                                else mx = 5;

                                                if (mx > 1)
                                                {
                                                    int bp = level.PosToInt(x, (ushort)(y + 1), z);
                                                    bool unblocked =
                                                        !level.ListUpdate.Exists(
                                                            Update => Update.b == bp);
                                                    if (unblocked)
                                                    {
                                                        level.AddUpdate(
                                                            level.PosToInt(x, (ushort)(y + 1), z),
                                                            Block.firework, false);
                                                        level.AddUpdate(level.PosToInt(x, y, z),
                                                                  Block.lavastill, false,
                                                                  "wait 1 dissipate 100");
                                                        // level.AddUpdate(level.PosToInt(x, (ushort)(y - 1), z), Block.air);
                                                        C.extraInfo = "wait 1 dissipate 100";
                                                        break;
                                                    }
                                                }
                                            }
                                            level.Firework(x, y, z, 4);
                                            break;
                                        }
                                        break;
                                    //Zombie + creeper stuff
                                    case Block.zombiehead:
                                        if (level.GetTile(level.IntOffset(C.b, 0, -1, 0)) != Block.zombiebody &&
                                            level.GetTile(level.IntOffset(C.b, 0, -1, 0)) != Block.creeper)
                                            C.extraInfo = "revert 0";
                                        break;
                                    case Block.zombiebody:
                                    case Block.creeper:

                                        #region ZOMBIE

                                        if (level.GetTile(x, (ushort)(y - 1), z) == Block.air)
                                        {
                                            level.AddUpdate(C.b, Block.zombiehead);
                                            level.AddUpdate(level.IntOffset(C.b, 0, -1, 0), level.blocks[C.b]);
                                            level.AddUpdate(level.IntOffset(C.b, 0, 1, 0), Block.air);
                                            break;
                                        }

                                        if (level.ai)
                                            Player.players.ForEach(delegate(Player p)
                                            {
                                                if (p.level == level&&
                                                    !p.invincible)
                                                {
                                                    currentNum =
                                                        Math.Abs(
                                                            (p.pos[0] /
                                                             32) - x) +
                                                        Math.Abs(
                                                            (p.pos[1] /
                                                             32) - y) +
                                                        Math.Abs(
                                                            (p.pos[2] /
                                                             32) - z);
                                                    if (currentNum <
                                                        foundNum)
                                                    {
                                                        foundNum =
                                                            currentNum;
                                                        foundPlayer = p;
                                                    }
                                                }
                                            });

                                    randomMovement_zomb:
                                        if (foundPlayer != null && rand.Next(1, 20) < 18)
                                        {
                                            currentNum = rand.Next(1, 7);
                                            foundNum = 0;

                                            switch (currentNum)
                                            {
                                                case 1:
                                                case 2:
                                                case 3:
                                                    if ((foundPlayer.pos[0] / 32) - x != 0)
                                                    {
                                                        skip = false;
                                                        newNum =
                                                            level.PosToInt(
                                                                (ushort)
                                                                (x +
                                                                 Math.Sign((foundPlayer.pos[0] / 32) -
                                                                           x)), y, z);

                                                        if (
                                                            level.GetTile(level.IntOffset(newNum, 0, -1, 0)) ==
                                                            Block.air &&
                                                            level.GetTile(newNum) == Block.air)
                                                            newNum = level.IntOffset(newNum, 0, -1, 0);
                                                        else if (level.GetTile(newNum) == Block.air &&
                                                                 level.GetTile(level.IntOffset(newNum, 0, 1, 0)) ==
                                                                 Block.air)
                                                        {
                                                        }

                                                        else if (
                                                            level.GetTile(level.IntOffset(newNum, 0, 2,
                                                                              0)) ==
                                                            Block.air &&
                                                            level.GetTile(level.IntOffset(newNum, 0, 1,
                                                                              0)) ==
                                                            Block.air)
                                                            newNum = level.IntOffset(newNum, 0,
                                                                               1, 0);
                                                        else skip = true;

                                                        if (!skip)
                                                            if (level.AddUpdate(newNum, level.blocks[C.b]))
                                                            {
                                                                level.AddUpdate(
                                                                    level.IntOffset(newNum, 0, 1, 0),
                                                                    Block.zombiehead);
                                                                goto removeSelf_zomb;
                                                            }
                                                    }

                                                    foundNum++;
                                                    if (foundNum >= 2) goto default;
                                                    else goto case 4;
                                                case 4:
                                                case 5:
                                                case 6:
                                                    if ((foundPlayer.pos[2] / 32) - z != 0)
                                                    {
                                                        skip = false;
                                                        newNum = level.PosToInt(x, y,
                                                                          (ushort)
                                                                          (z +
                                                                           Math.Sign(
                                                                               (foundPlayer.pos[2] /
                                                                                32) - z)));

                                                        if (
                                                            level.GetTile(level.IntOffset(newNum, 0, -1, 0)) ==
                                                            Block.air &&
                                                            level.GetTile(newNum) == Block.air)
                                                            newNum = level.IntOffset(newNum, 0, -1, 0);
                                                        else if (level.GetTile(newNum) == Block.air &&
                                                                 level.GetTile(level.IntOffset(newNum, 0, 1, 0)) ==
                                                                 Block.air)
                                                        {
                                                        }

                                                        else if (
                                                            level.GetTile(level.IntOffset(newNum, 0, 2,
                                                                              0)) ==
                                                            Block.air &&
                                                            level.GetTile(level.IntOffset(newNum, 0, 1,
                                                                              0)) ==
                                                            Block.air)
                                                            newNum = level.IntOffset(newNum, 0,
                                                                               1, 0);
                                                        else skip = true;

                                                        if (!skip)
                                                            if (level.AddUpdate(newNum, level.blocks[C.b]))
                                                            {
                                                                level.AddUpdate(
                                                                    level.IntOffset(newNum, 0, 1, 0),
                                                                    Block.zombiehead);
                                                                goto removeSelf_zomb;
                                                            }
                                                    }

                                                    foundNum++;
                                                    if (foundNum >= 2) goto default;
                                                    else goto case 1;
                                                default:
                                                    foundPlayer = null;
                                                    skip = true;
                                                    goto randomMovement_zomb;
                                            }
                                        }
                                        else
                                        {
                                            if (!skip)
                                                if (C.time < 3)
                                                {
                                                    C.time++;
                                                    break;
                                                }

                                            foundNum = 0;
                                            switch (rand.Next(1, 13))
                                            {
                                                case 1:
                                                case 2:
                                                case 3:
                                                    skip = false;
                                                    newNum = level.IntOffset(C.b, -1, 0, 0);

                                                    if (level.GetTile(level.IntOffset(newNum, 0, -1, 0)) ==
                                                        Block.air && level.GetTile(newNum) == Block.air)
                                                        newNum = level.IntOffset(newNum, 0, -1, 0);
                                                    else if (level.GetTile(newNum) == Block.air &&
                                                             level.GetTile(level.IntOffset(newNum, 0, 1, 0)) ==
                                                             Block.air)
                                                    {
                                                    }

                                                    else if (
                                                        level.GetTile(level.IntOffset(newNum, 0, 2, 0)) ==
                                                        Block.air &&
                                                        level.GetTile(level.IntOffset(newNum, 0, 1, 0)) ==
                                                        Block.air)
                                                        newNum = level.IntOffset(newNum, 0, 1, 0);
                                                    else skip = true;

                                                    if (!skip)
                                                        if (level.AddUpdate(newNum, level.blocks[C.b]))
                                                        {
                                                            level.AddUpdate(level.IntOffset(newNum, 0, 1, 0),
                                                                      Block.zombiehead);
                                                            goto removeSelf_zomb;
                                                        }

                                                    foundNum++;
                                                    if (foundNum >= 4) InnerChange = true;
                                                    else goto case 4;
                                                    break;

                                                case 4:
                                                case 5:
                                                case 6:
                                                    skip = false;
                                                    newNum = level.IntOffset(C.b, 1, 0, 0);

                                                    if (level.GetTile(level.IntOffset(newNum, 0, -1, 0)) ==
                                                        Block.air && level.GetTile(newNum) == Block.air)
                                                        newNum = level.IntOffset(newNum, 0, -1, 0);
                                                    else if (level.GetTile(newNum) == Block.air &&
                                                             level.GetTile(level.IntOffset(newNum, 0, 1, 0)) ==
                                                             Block.air)
                                                    {
                                                    }

                                                    else if (
                                                        level.GetTile(level.IntOffset(newNum, 0, 2, 0)) ==
                                                        Block.air &&
                                                        level.GetTile(level.IntOffset(newNum, 0, 1, 0)) ==
                                                        Block.air)
                                                        newNum = level.IntOffset(newNum, 0, 1, 0);
                                                    else skip = true;

                                                    if (!skip)
                                                        if (level.AddUpdate(newNum, level.blocks[C.b]))
                                                        {
                                                            level.AddUpdate(level.IntOffset(newNum, 0, 1, 0),
                                                                      Block.zombiehead);
                                                            goto removeSelf_zomb;
                                                        }

                                                    foundNum++;
                                                    if (foundNum >= 4) InnerChange = true;
                                                    else goto case 7;
                                                    break;

                                                case 7:
                                                case 8:
                                                case 9:
                                                    skip = false;
                                                    newNum = level.IntOffset(C.b, 0, 0, 1);

                                                    if (level.GetTile(level.IntOffset(newNum, 0, -1, 0)) ==
                                                        Block.air && level.GetTile(newNum) == Block.air)
                                                        newNum = level.IntOffset(newNum, 0, -1, 0);
                                                    else if (level.GetTile(newNum) == Block.air &&
                                                             level.GetTile(level.IntOffset(newNum, 0, 1, 0)) ==
                                                             Block.air)
                                                    {
                                                    }

                                                    else if (
                                                        level.GetTile(level.IntOffset(newNum, 0, 2, 0)) ==
                                                        Block.air &&
                                                        level.GetTile(level.IntOffset(newNum, 0, 1, 0)) ==
                                                        Block.air)
                                                        newNum = level.IntOffset(newNum, 0, 1, 0);
                                                    else skip = true;

                                                    if (!skip)
                                                        if (level.AddUpdate(newNum, level.blocks[C.b]))
                                                        {
                                                            level.AddUpdate(level.IntOffset(newNum, 0, 1, 0),
                                                                      Block.zombiehead);
                                                            goto removeSelf_zomb;
                                                        }

                                                    foundNum++;
                                                    if (foundNum >= 4) InnerChange = true;
                                                    else goto case 10;
                                                    break;
                                                case 10:
                                                case 11:
                                                case 12:
                                                default:
                                                    skip = false;
                                                    newNum = level.IntOffset(C.b, 0, 0, -1);

                                                    if (level.GetTile(level.IntOffset(newNum, 0, -1, 0)) ==
                                                        Block.air && level.GetTile(newNum) == Block.air)
                                                        newNum = level.IntOffset(newNum, 0, -1, 0);
                                                    else if (level.GetTile(newNum) == Block.air &&
                                                             level.GetTile(level.IntOffset(newNum, 0, 1, 0)) ==
                                                             Block.air)
                                                    {
                                                    }

                                                    else if (
                                                        level.GetTile(level.IntOffset(newNum, 0, 2, 0)) ==
                                                        Block.air &&
                                                        level.GetTile(level.IntOffset(newNum, 0, 1, 0)) ==
                                                        Block.air)
                                                        newNum = level.IntOffset(newNum, 0, 1, 0);
                                                    else skip = true;

                                                    if (!skip)
                                                        if (level.AddUpdate(newNum, level.blocks[C.b]))
                                                        {
                                                            level.AddUpdate(level.IntOffset(newNum, 0, 1, 0),
                                                                      Block.zombiehead);
                                                            goto removeSelf_zomb;
                                                        }

                                                    foundNum++;
                                                    if (foundNum >= 4) InnerChange = true;
                                                    else goto case 1;
                                                    break;
                                            }
                                        }

                                    removeSelf_zomb:
                                        if (!InnerChange)
                                        {
                                            level.AddUpdate(C.b, Block.air);
                                            level.AddUpdate(level.IntOffset(C.b, 0, 1, 0), Block.air);
                                        }
                                        break;

                                        #endregion

                                    case Block.c4:
                                        C4.C4s c4 = C4.Find(level, C.p.c4circuitNumber);
                                        if (c4 != null)
                                        {
                                            C4.C4s.OneC4 one = new C4.C4s.OneC4(x, y, z);
                                            c4.list.Add(one);
                                        }
                                        C.time = 255;
                                        break;

                                    case Block.c4det:
                                        C4.C4s c = C4.Find(level, C.p.c4circuitNumber);
                                        if (c != null)
                                        {
                                            c.detenator[0] = x;
                                            c.detenator[1] = y;
                                            c.detenator[2] = z;
                                        }
                                        C.p.c4circuitNumber = -1;
                                        C.time = 255;
                                        break;

                                    default:
                                        //non special blocks are then ignored, maybe it would be better to avoid getting here and cutting down the list
                                        if (!C.extraInfo.Contains("wait")) C.time = 255;
                                        break;
                                }
                            }
                        }
                        catch
                        {
                            level.ListCheck.Remove(C);
                            //Server.s.Log(e.Message);
                        }
                    });

                    level.ListCheck.RemoveAll(Check => Check.time == 255); //Remove all that are finished with 255 time

                    level.lastUpdate = level.ListUpdate.Count;
                    level.ListUpdate.ForEach(delegate(Update C)
                    {
                        try
                        {
                            //level.IntToPos(C.b, out x, out y, out z); NO!
                            level.Blockchange(C.b, C.type, false, C.extraInfo);
                        }
                        catch
                        {
                            Server.s.Log("Phys update issue");
                        }
                    });

                    level.ListUpdate.Clear();

                    #endregion
                }
            }
            catch (Exception e)
            {
                Server.s.Log("Level level.physics error");
                Server.ErrorLog(e);
            }
        }
        public void ClearPhysics(Level level)
        {
            ushort x, y, z;
            level.ListCheck.ForEach(delegate(Check C)
            {
                level.IntToPos(C.b, out x, out y, out z);
                //attemps on shutdown to change blocks back into normal selves that are active, hopefully without needing to send into to clients.
                switch (level.blocks[C.b])
                {
                    case 200:
                    case 202:
                    case 203:
                        level.blocks[C.b] = 0;
                        break;
                    case 201:
                        //level.blocks[C.b] = 111;
                        level.Blockchange(x, y, z, 111);
                        break;
                    case 205:
                        //level.blocks[C.b] = 113;
                        level.Blockchange(x, y, z, 113);
                        break;
                    case 206:
                        //level.blocks[C.b] = 114;
                        level.Blockchange(x, y, z, 114);
                        break;
                    case 207:
                        //level.blocks[C.b] = 115;
                        level.Blockchange(x, y, z, 115);
                        break;
                }

                try
                {
                    if (C.extraInfo.Contains("revert"))
                    {
                        int i = 0;
                        foreach (string s in C.extraInfo.Split(' '))
                        {
                            if (s == "revert")
                            {
                                level.Blockchange(x, y, z, Byte.Parse(C.extraInfo.Split(' ')[i + 1]), true);
                                break;
                            }
                            i++;
                        }
                    }
                }
                catch (Exception e)
                {
                    Server.ErrorLog(e);
                }
            });

            level.ListCheck.Clear();
            level.ListUpdate.Clear();
        }

        //================================================================================================================
        private void PhysWater(Level level, int b, ushort type)
        {
            if (b == -1)
            {
                return;
            }
            ushort x, y, z;
            level.IntToPos(b, out x, out y, out z);
            if (Server.lava.active && Server.lava.map == level&& Server.lava.InSafeZone(x, y, z))
            {
                return;
            }

            switch (level.blocks[b])
            {
                case Block.air:
                    if (!PhysSpongeCheck(level, b))
                    {
                        level.AddUpdate(b, type);
                    }
                    break;

                case 10: //hit active_lava
                case 112: //hit lava_fast
                case Block.activedeathlava:
                    if (!PhysSpongeCheck(level, b))
                    {
                        level.AddUpdate(b, 1);
                    }
                    break;

                case 6:
                case 37:
                case 38:
                case 39:
                case 40:
                case 50:
                case 51:
                case 52:
                case 53:
                case 54:
                case 55:
                case 56:
                case 57:
                case 58:
                case 59:
                case 60:
                case 61:
                case 62:
                case 63:
                case 64:
                case 65:
                    if (level.physics > 1) //Adv level.physics kills flowers and mushrooms in water
                    {
                        if (level.physics != 5)
                        {
                            if (!PhysSpongeCheck(level, b))
                            {
                                level.AddUpdate(b, 0);
                            }
                        }
                    }
                    break;
                case 12: //sand
                case 13: //gravel
                case 110: //woodfloat
                    level.AddCheck(b);
                    break;

                default:
                    break;
            }
        }

        //================================================================================================================
        private bool PhysWaterCheck(Level level, int b)
        {
            if (b == -1)
            {
                return false;
            }
            ushort x, y, z;
            level.IntToPos(b, out x, out y, out z);
            if (Server.lava.active && Server.lava.map == level&& Server.lava.InSafeZone(x, y, z))
            {
                return false;
            }

            switch (level.blocks[b])
            {
                case Block.air:
                    if (!PhysSpongeCheck(level, b))
                    {
                        return true;
                    }
                    break;

                case 10: //hit active_lava
                case 112: //hit lava_fast
                case Block.activedeathlava:
                    if (!PhysSpongeCheck(level, b))
                    {
                        return true;
                    }
                    break;

                case 6:
                case 37:
                case 38:
                case 39:
                case 40:
                case 50:
                case 51:
                case 52:
                case 53:
                case 54:
                case 55:
                case 56:
                case 57:
                case 58:
                case 59:
                case 60:
                case 61:
                case 62:
                case 63:
                case 64:
                case 65:
                    if (level.physics > 1) //Adv level.physics kills flowers and mushrooms in water
                    {
                        if (level.physics != 5)
                        {
                            if (!PhysSpongeCheck(level, b))
                            {
                                return true;
                            }
                        }
                    }
                    break;

                case 12: //sand
                case 13: //gravel
                case 110: //woodfloat
                    return true;
            }
            return false;
        }

        //================================================================================================================
        private void PhysLava(Level level, int b, ushort type)
        {
            if (b == -1)
            {
                return;
            }
            ushort x, y, z;
            level.IntToPos(b, out x, out y, out z);
            if (Server.lava.active && Server.lava.map == level&& Server.lava.InSafeZone(x, y, z))
            {
                return;
            }

            if (level.physics > 1 && level.physics != 5 && !PhysSpongeCheck(level, b, true) && level.blocks[b] >= 21 && level.blocks[b] <= 36)
            {
                level.AddUpdate(b, 0);
                return;
            } // Adv level.physics destroys cloth
            switch (level.blocks[b])
            {
                case Block.air:
                    if (!PhysSpongeCheck(level, b, true)) level.AddUpdate(b, type);
                    break;

                case 8: //hit active_water
                case Block.activedeathwater:
                    if (!PhysSpongeCheck(level, b, true)) level.AddUpdate(b, 1);
                    break;

                case 12: //sand
                    if (level.physics > 1) //Adv level.physics changes sand to glass next to lava
                    {
                        if (level.physics != 5)
                        {
                            level.AddUpdate(b, 20);
                        }
                    }
                    else
                    {
                        level.AddCheck(b);
                    }
                    break;

                case 13: //gravel
                    level.AddCheck(b);
                    break;

                case 5:
                case 6:
                case 17:
                case 18:
                case 37:
                case 38:
                case 39:
                case 40:
                case 50:
                case 51:
                case 52:
                case 53:
                case 54:
                case 55:
                case 56:
                case 57:
                case 58:
                case 59:
                case 60:
                case 61:
                case 62:
                case 63:
                case 64:
                case 65:
                    if (level.physics > 1 && level.physics != 5) //Adv level.physics kills flowers and mushrooms plus wood in lava
                        if (!PhysSpongeCheck(level, b, true)) level.AddUpdate(b, 0);
                    break;

                default:
                    break;
            }
        }

        //================================================================================================================
        private bool PhysLavaCheck(Level level, int b)
        {
            if (b == -1)
            {
                return false;
            }
            ushort x, y, z;
            level.IntToPos(b, out x, out y, out z);
            if (Server.lava.active && Server.lava.map == level&& Server.lava.InSafeZone(x, y, z))
            {
                return false;
            }

            if (level.physics > 1 && level.physics != 5 && !PhysSpongeCheck(level, b, true) && level.blocks[b] >= 21 && level.blocks[b] <= 36)
            {
                return true;
            } // Adv level.physics destroys cloth
            switch (level.blocks[b])
            {
                case Block.air:
                    return true;

                case 8: //hit active_water
                case Block.activedeathwater:
                    if (!PhysSpongeCheck(level, b, true)) return true;
                    break;

                case 12: //sand
                    if (level.physics > 1) //Adv level.physics changes sand to glass next to lava
                    {
                        if (level.physics != 5)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                    break;

                case 13: //gravel
                    return true;

                case 5:
                case 6:
                case 17:
                case 18:
                case 37:
                case 38:
                case 39:
                case 40:
                case 50:
                case 51:
                case 52:
                case 53:
                case 54:
                case 55:
                case 56:
                case 57:
                case 58:
                case 59:
                case 60:
                case 61:
                case 62:
                case 63:
                case 64:
                case 65:
                    if (level.physics > 1 && level.physics != 5) //Adv level.physics kills flowers and mushrooms plus wood in lava
                        if (!PhysSpongeCheck(level, b, true)) return true;
                    break;
            }
            return false;
        }

        //================================================================================================================
        private void Physair(Level level, int b)
        {
            if (b == -1)
            {
                return;
            }
            if (Block.Convert(level.blocks[b]) == Block.water || Block.Convert(level.blocks[b]) == Block.lava ||
                (level.blocks[b] >= 21 && level.blocks[b] <= 36))
            {
                level.AddCheck(b);
                return;
            }

            switch (level.blocks[b])
            {
                //case 8:     //active water
                //case 10:    //active_lava
                case 6: //shrub
                case 12: //sand
                case 13: //gravel
                case 18: //leaf
                case 110: //wood_float
                    /*case 112:   //lava_fast
                    case Block.WaterDown:
                    case Block.LavaDown:
                    case Block.deathlava:
                    case Block.deathwater:
                    case Block.geyser:
                    case Block.magma:*/
                    level.AddCheck(b);
                    break;

                default:
                    break;
            }
        }

        //================================================================================================================
        private bool PhysSand(Level level, int b, ushort type) //also does gravel
        {
            if (b == -1 || level.physics == 0) return false;
            if (level.physics == 5) return false;

            int tempb = b;
            bool blocked = false;
            bool moved = false;

            do
            {
                tempb = level.IntOffset(tempb, 0, -1, 0); //Get block below each loop
                if (level.GetTile(tempb) != Block.Zero)
                {
                    switch (level.blocks[tempb])
                    {
                        case Block.air: //air lava water
                        case 8:
                        case 10:
                            moved = true;
                            break;

                        case 6:
                        case 37:
                        case 38:
                        case 39:
                        case 40:
                            if (level.physics > 1 && level.physics != 5) //Adv level.physics crushes plants with sand
                            {
                                moved = true;
                            }
                            else
                            {
                                blocked = true;
                            }
                            break;

                        default:
                            blocked = true;
                            break;
                    }
                    if (level.physics > 1)
                    {
                        if (level.physics != 5)
                        {
                            blocked = true;
                        }
                    }
                }
                else
                {
                    blocked = true;
                }
            } while (!blocked);

            if (moved)
            {
                level.AddUpdate(b, 0);
                if (level.physics > 1)
                {
                    level.AddUpdate(tempb, type);
                }
                else
                {
                    level.AddUpdate(level.IntOffset(tempb, 0, 1, 0), type);
                }
            }

            return moved;
        }

        private void PhysSandCheck(Level level, int b) //also does gravel
        {
            if (b == -1)
            {
                return;
            }
            switch (level.blocks[b])
            {
                case 12: //sand
                case 13: //gravel
                case 110: //wood_float
                    level.AddCheck(b);
                    break;

                default:
                    break;
            }
        }

        //================================================================================================================
        private void PhysStair(Level level, int b)
        {
            int tempb = level.IntOffset(b, 0, -1, 0); //Get block below
            if (level.GetTile(tempb) != Block.Zero)
            {
                if (level.GetTile(tempb) == Block.staircasestep)
                {
                    level.AddUpdate(b, 0);
                    level.AddUpdate(tempb, 43);
                }
            }
        }

        //================================================================================================================
        private bool PhysSpongeCheck(Level level, int b, bool lava = false) //return true if sponge is near
        {
            int temp = 0;
            for (int x = -2; x <= +2; ++x)
            {
                for (int y = -2; y <= +2; ++y)
                {
                    for (int z = -2; z <= +2; ++z)
                    {
                        temp = level.IntOffset(b, x, y, z);
                        if (level.GetTile(temp) != Block.Zero)
                        {
                            if ((!lava && level.GetTile(temp) == 19) || (lava && level.GetTile(temp) == 109))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        //================================================================================================================
        private void PhysSponge(Level level, int b, bool lava = false) //turn near water into air when placed
        {
            int temp = 0;
            for (int x = -2; x <= +2; ++x)
            {
                for (int y = -2; y <= +2; ++y)
                {
                    for (int z = -2; z <= +2; ++z)
                    {
                        temp = level.IntOffset(b, x, y, z);
                        if (level.GetTile(temp) != Block.Zero)
                        {
                            if ((!lava && Block.Convert(level.GetTile(temp)) == 8) ||
                                (lava && Block.Convert(level.GetTile(temp)) == 10))
                            {
                                level.AddUpdate(temp, 0);
                            }
                        }
                    }
                }
            }
        }

        //================================================================================================================
        public void PhysSpongeRemoved(Level level, int b, bool lava = false) //Reactivates near water
        {
            int temp = 0;
            for (int x = -3; x <= +3; ++x)
            {
                for (int y = -3; y <= +3; ++y)
                {
                    for (int z = -3; z <= +3; ++z)
                    {
                        if (Math.Abs(x) == 3 || Math.Abs(y) == 3 || Math.Abs(z) == 3) //Calc only edge
                        {
                            temp = level.IntOffset(b, x, y, z);
                            if (level.GetTile(temp) != Block.Zero)
                            {
                                if ((!lava && Block.Convert(level.GetTile(temp)) == 8) ||
                                    (lava && Block.Convert(level.GetTile(temp)) == 10))
                                {
                                    level.AddCheck(temp);
                                }
                            }
                        }
                    }
                }
            }
        }

        //================================================================================================================
        private void PhysFloatwood(Level level, int b)
        {
            int tempb = level.IntOffset(b, 0, -1, 0); //Get block below
            if (level.GetTile(tempb) != Block.Zero)
            {
                if (level.GetTile(tempb) == 0)
                {
                    level.AddUpdate(b, 0);
                    level.AddUpdate(tempb, 110);
                    return;
                }
            }

            tempb = level.IntOffset(b, 0, 1, 0); //Get block above
            if (level.GetTile(tempb) != Block.Zero)
            {
                if (Block.Convert(level.GetTile(tempb)) == 8)
                {
                    level.AddUpdate(b, 8);
                    level.AddUpdate(tempb, 110);
                    return;
                }
            }
        }

        //================================================================================================================
        private void PhysairFlood(Level level, int b, ushort type)
        {
            if (b == -1)
            {
                return;
            }
            if (Block.Convert(level.blocks[b]) == Block.water || Block.Convert(level.blocks[b]) == Block.lava) level.AddUpdate(b, type);
        }

        //================================================================================================================
        private void PhysFall(Level level, ushort newBlock, ushort x, ushort y, ushort z, bool random)
        {
            var randNum = new Random();
            ushort b;
            if (!random)
            {
                b = level.GetTile((ushort)(x + 1), y, z);
                if (b == Block.air || b == Block.waterstill) level.Blockchange((ushort)(x + 1), y, z, newBlock);
                b = level.GetTile((ushort)(x - 1), y, z);
                if (b == Block.air || b == Block.waterstill) level.Blockchange((ushort)(x - 1), y, z, newBlock);
                b = level.GetTile(x, y, (ushort)(z + 1));
                if (b == Block.air || b == Block.waterstill) level.Blockchange(x, y, (ushort)(z + 1), newBlock);
                b = level.GetTile(x, y, (ushort)(z - 1));
                if (b == Block.air || b == Block.waterstill) level.Blockchange(x, y, (ushort)(z - 1), newBlock);
            }
            else
            {
                if (level.GetTile((ushort)(x + 1), y, z) == Block.air && randNum.Next(1, 10) < 3)
                    level.Blockchange((ushort)(x + 1), y, z, newBlock);
                if (level.GetTile((ushort)(x - 1), y, z) == Block.air && randNum.Next(1, 10) < 3)
                    level.Blockchange((ushort)(x - 1), y, z, newBlock);
                if (level.GetTile(x, y, (ushort)(z + 1)) == Block.air && randNum.Next(1, 10) < 3)
                    level.Blockchange(x, y, (ushort)(z + 1), newBlock);
                if (level.GetTile(x, y, (ushort)(z - 1)) == Block.air && randNum.Next(1, 10) < 3)
                    level.Blockchange(x, y, (ushort)(z - 1), newBlock);
            }
        }

        //================================================================================================================
        private void PhysReplace(Level level, int b, byte typeA, byte typeB) //replace any typeA with typeB
        {
            if (b == -1)
            {
                return;
            }
            if (level.blocks[b] == typeA)
            {
                level.AddUpdate(b, typeB);
            }
        }

        //================================================================================================================
        private bool PhysLeaf(Level level, int b)
        {
            ushort type, dist = 4;
            int? i, xx, yy, zz;
            ushort x, y, z;
            level.IntToPos(b, out x, out y, out z);

            for (xx = -dist; xx <= dist; xx++)
            {
                for (yy = -dist; yy <= dist; yy++)
                {
                    for (zz = -dist; zz <= dist; zz++)
                    {
                        type = level.GetTile((ushort)(x + xx), (ushort)(y + yy), (ushort)(z + zz));
                        if (type == Block.trunk)
                            leaves[level.PosToInt((ushort)(x + xx), (ushort)(y + yy), (ushort)(z + zz))] = 0;
                        else if (type == Block.leaf)
                            leaves[level.PosToInt((ushort)(x + xx), (ushort)(y + yy), (ushort)(z + zz))] = -2;
                        else
                            leaves[level.PosToInt((ushort)(x + xx), (ushort)(y + yy), (ushort)(z + zz))] = -1;
                    }
                }
            }

            for (i = 1; i <= dist; i++)
            {
                for (xx = -dist; xx <= dist; xx++)
                {
                    for (yy = -dist; yy <= dist; yy++)
                    {
                        for (zz = -dist; zz <= dist; zz++)
                        {
                            try
                            {
                                if (leaves[level.PosToInt((ushort)(x + xx), (ushort)(y + yy), (ushort)(z + zz))] == i - 1)
                                {
                                    if (
                                        leaves.ContainsKey(level.PosToInt((ushort)(x + xx - 1), (ushort)(y + yy),
                                                                    (ushort)(z + zz))) &&
                                        leaves[level.PosToInt((ushort)(x + xx - 1), (ushort)(y + yy), (ushort)(z + zz))] ==
                                        -2)
                                        leaves[level.PosToInt((ushort)(x + xx - 1), (ushort)(y + yy), (ushort)(z + zz))] =
                                            (sbyte)i;

                                    if (
                                        leaves.ContainsKey(level.PosToInt((ushort)(x + xx + 1), (ushort)(y + yy),
                                                                    (ushort)(z + zz))) &&
                                        leaves[level.PosToInt((ushort)(x + xx + 1), (ushort)(y + yy), (ushort)(z + zz))] ==
                                        -2)
                                        leaves[level.PosToInt((ushort)(x + xx + 1), (ushort)(y + yy), (ushort)(z + zz))] =
                                            (sbyte)i;

                                    if (
                                        leaves.ContainsKey(level.PosToInt((ushort)(x + xx), (ushort)(y + yy - 1),
                                                                    (ushort)(z + zz))) &&
                                        leaves[level.PosToInt((ushort)(x + xx), (ushort)(y + yy - 1), (ushort)(z + zz))] ==
                                        -2)
                                        leaves[level.PosToInt((ushort)(x + xx), (ushort)(y + yy - 1), (ushort)(z + zz))] =
                                            (sbyte)i;

                                    if (
                                        leaves.ContainsKey(level.PosToInt((ushort)(x + xx), (ushort)(y + yy + 1),
                                                                    (ushort)(z + zz))) &&
                                        leaves[level.PosToInt((ushort)(x + xx), (ushort)(y + yy + 1), (ushort)(z + zz))] ==
                                        -2)
                                        leaves[level.PosToInt((ushort)(x + xx), (ushort)(y + yy + 1), (ushort)(z + zz))] =
                                            (sbyte)i;

                                    if (
                                        leaves.ContainsKey(level.PosToInt((ushort)(x + xx), (ushort)(y + yy),
                                                                    (ushort)(z + zz - 1))) &&
                                        leaves[level.PosToInt((ushort)(x + xx), (ushort)(y + yy), (ushort)(z + zz - 1))] ==
                                        -2)
                                        leaves[level.PosToInt((ushort)(x + xx), (ushort)(y + yy), (ushort)(z + zz - 1))] =
                                            (sbyte)i;

                                    if (
                                        leaves.ContainsKey(level.PosToInt((ushort)(x + xx), (ushort)(y + yy),
                                                                    (ushort)(z + zz + 1))) &&
                                        leaves[level.PosToInt((ushort)(x + xx), (ushort)(y + yy), (ushort)(z + zz + 1))] ==
                                        -2)
                                        leaves[level.PosToInt((ushort)(x + xx), (ushort)(y + yy), (ushort)(z + zz + 1))] =
                                            (sbyte)i;
                                }
                            }
                            catch
                            {
                                Server.s.Log("Leaf decay error!");
                            }
                        }
                    }
                }
            }

            //Server.s.Log((leaves[b] < 0).ToString()); // levelis a debug line that spams the console to hell!
            return leaves[b] < 0;
        }

        //================================================================================================================
        private byte PhysFlowDirections(Level level, int b, bool down = true, bool up = false)
        {
            byte dir = 0;
            ushort x, y, z;
            level.IntToPos(b, out x, out y, out z);

            if (level.GetTile((ushort)(x + 1), y, z) == Block.air) dir++;
            if (level.GetTile((ushort)(x - 1), y, z) == Block.air) dir++;
            if (up && level.GetTile(x, (ushort)(y + 1), z) == Block.air) dir++;
            if (down && level.GetTile(x, (ushort)(y - 1), z) == Block.air) dir++;
            if (level.GetTile(x, y, (ushort)(z + 1)) == Block.air) dir++;
            if (level.GetTile(x, y, (ushort)(z - 1)) == Block.air) dir++;

            return dir;
        }

        //================================================================================================================
        private void PhysAir(Level level, int b)
        {
            if (b == -1)
            {
                return;
            }
            if (Block.Convert(level.blocks[b]) == Block.water || Block.Convert(level.blocks[b]) == Block.lava ||
                (level.blocks[b] >= 21 && level.blocks[b] <= 36))
            {
                level.AddCheck(b);
                return;
            }

            switch (level.blocks[b])
            {
                //case 8:     //active water
                //case 10:    //active_lava
                case 6: //shrub
                case 12: //sand
                case 13: //gravel
                case 18: //leaf
                case 110: //wood_float
                    /*case 112:   //lava_fast
                    case Block.WaterDown:
                    case Block.LavaDown:
                    case Block.deathlava:
                    case Block.deathwater:
                    case Block.geyser:
                    case Block.magma:*/
                    level.AddCheck(b);
                    break;

                default:
                    break;
            }
        }

        //================================================================================================================
        private void PhysAirFlood(Level level, int b, ushort type)
        {
            if (b == -1)
            {
                return;
            }
            if (Block.Convert(level.blocks[b]) == Block.water || Block.Convert(level.blocks[b]) == Block.lava) level.AddUpdate(b, type);
        }

        //================================================================================================================
    }
    public static class C4
    {
        public static void BlowUp(ushort[] detenator, Level lvl)
        {
            try
            {
                foreach (C4s c4 in lvl.C4list)
                {
                    if (c4.detenator[0] == detenator[0] && c4.detenator[1] == detenator[1] && c4.detenator[2] == detenator[2])
                    {
                        foreach (C4s.OneC4 c in c4.list)
                        {
                            lvl.MakeExplosion(c.pos[0], c.pos[1], c.pos[2], 0);
                        }
                        lvl.C4list.Remove(c4);
                    }
                }
            }
            catch { }
        }
        public static sbyte NextCircuit(Level lvl)
        {
            sbyte number = 1;
            foreach (C4s c4 in lvl.C4list)
            {
                number++;
            }
            return number;
        }
        public static C4s Find(Level lvl, sbyte CircuitNumber)
        {
            foreach (C4s c4 in lvl.C4list)
            {
                if (c4.CircuitNumb == CircuitNumber)
                {
                    return c4;
                }
            }
            return null;
        }
        public class C4s
        {
            public sbyte CircuitNumb;
            public ushort[] detenator;
            public List<OneC4> list;
            public class OneC4
            {
                public ushort[] pos = new ushort[3];
                public OneC4(ushort x, ushort y, ushort z)
                {
                    pos[0] = x;
                    pos[1] = y;
                    pos[2] = z;
                }
            }
            public C4s(sbyte num)
            {
                CircuitNumb = num;
                list = new List<OneC4>();
                detenator = new ushort[3];
            }
        }
    }
}
