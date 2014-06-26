using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;

namespace MCForge.Commands
{
    public class CmdConvertAll : Command
    {
        public override string name { get { return "convertall"; } }
        public override string shortcut { get { return  "cvta"; } }
        public override string type { get { return "mod"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdConvertAll() { }

        public override void Use(Player p, string message)
        {
            List<string> mapsToConvert = new List<string>(Directory.GetFiles("levels/byte", "*.lvl"));
            mapsToConvert.ForEach(m =>
            {
                m = m.Remove(0, m.LastIndexOf('\\') + 1);
                Command.all.Find("loadbyte").Use(p, m.Substring(0, m.LastIndexOf('.')));
                Command.all.Find("save").Use(p, m.Substring(0, m.LastIndexOf('.')));
                Command.all.Find("unload").Use(p, m.Substring(0, m.LastIndexOf('.')));

                GC.Collect();
            });
            GC.WaitForPendingFinalizers();
            Player.SendMessage(p, c.lime + "Converted " + mapsToConvert.Count + " maps!");
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/convertall - Converts all the byte formatted maps to ushort format");
        }
    }
}