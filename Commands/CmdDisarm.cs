using System;
using System.IO;

namespace MCForge
{
    public class CmdDisarm : Command
    {
        public override string name { get { return "disarm"; } }
        public override string shortcut { get { return "d"; } }
        public override string type { get { return "ctf"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdDisarm() { }
        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            if (p == null) return;
            if (message == "mine" || message == "trap" || message == "tnt")
                Server.pctf.disarm(p, message);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/disarm [mine/trap/tnt] - disables your current mine/trap/tnt so you can place another one.");
        }
    }
}