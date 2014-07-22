using System;

namespace MCForge
{
    public class CmdAutoTNT : Command
    {
        public override string name { get { return "autotnt"; } }
        public override string shortcut { get { return "at"; } }
        public override string type { get { return "ctf"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdAutoTNT() { }

        public override void Use(Player p, string message)
        {
            p.autoTNT = !p.autoTNT;
            if (p.autoTNT) Player.SendMessage(p, "TNT will now explode automatically");
            else Player.SendMessage(p, "TNT will now explode when placing purple or typing /t");
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/autoTNT - enables/disables automatic TNT (enable is default)");
        }
    }
}