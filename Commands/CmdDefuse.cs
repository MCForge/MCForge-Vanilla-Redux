using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCForge.Commands
{
    class CmdDefuse : Command
    {
        public override string shortcut { get { return  "d"; } }
        public override bool museumUsable { get { return true; } }
        public override string name { get { return "defuse"; } }
        public override string type { get { return "other"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdDefuse() { }

        public override void Use(Player p, string message)
        {
            p.placedMine.isActive = false;
            p.SendMessage("&f- &SMine was defused!");
        }
        public override void Help(Player p)
        {
            p.SendMessage("/defuse - defuses last placed mine.");
        }
    }
}
