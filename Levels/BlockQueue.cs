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
namespace MCForge
{
    public static class BlockQueue
    {
        public static int time { get { return (int)blocktimer.Interval; } set { blocktimer.Interval = value; } }
        public static int blockupdates = 200;
        static block b = new block();
        static System.Timers.Timer blocktimer = new System.Timers.Timer(100);
        static byte started = 0;

        public static void Start()
        {
            blocktimer.Elapsed += delegate
            {
                if (started == 1) return;
                started = 1;
                Server.levels.ForEach((l) =>
                {
                    try
                    {
                        if (l.blockqueue.Count < 1) return;
                        int count;
                        if (l.blockqueue.Count < blockupdates || l.players.Count == 0) count = l.blockqueue.Count;
                        else count = blockupdates;
						var blockchange = new Blockchange();
                        for (int c = 0; c < count; c++)
                        {
							l.blockqueue[c].p.manualChange(l.blockqueue[c].x, l.blockqueue[c].y, l.blockqueue[c].z, 1, l.blockqueue[c].type);
                        }
                        l.blockqueue.RemoveRange(0, count);
                    }
                    catch (Exception e)
                    {
                        Server.s.ErrorCase("error:" + e);
                        Server.s.Log(String.Format("Block cache failed for map: {0}. {1} lost.", l.name, l.blockqueue.Count));
                        l.blockqueue.Clear();
                    }
                });
                started = 0;
            };
            blocktimer.Start();
        }
        public static void pause() { blocktimer.Enabled = false; }
        public static void resume() { blocktimer.Enabled = true; }

        public static void Addblock(Player P, ushort X, ushort Y, ushort Z, ushort type)
        {
            b.x = X; b.y = Y; b.z = Z; b.type = type; b.p = P;
            P.level.blockqueue.Add(b);
        }

        public struct block { public Player p; public ushort x; public ushort y; public ushort z; public ushort type; }
    }
}