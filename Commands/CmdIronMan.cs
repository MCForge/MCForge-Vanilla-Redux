using System;
using System.IO;

namespace MCForge
{
    public class CmdIronman : Command
    {
        public override string name { get { return "ironman"; } }
        public override string shortcut { get { return "im"; } }
        public override string type { get { return "lava"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdIronman() { }
        public override void Use(Player p, string message)
        {
            if (p == null) { Player.SendMessage(p, "You can't use this from console/irc!"); return; }
            if (Server.lava.active == false)
            {
                if (p.ironmanActivated)
                {
                    p.SendMessage("De-Activated Ironman mode!");
                    p.ironmanActivated = false;
                }
                else
                {
                    p.SendMessage("Activated Ironman mode!");
                    p.ironmanActivated = true;
                }
            }
            else
            {
                p.SendMessage("You cannot use IRONMAN while round is in progress!");
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/ironman - Enables/disables Ironman Mode!");
        }
    }
}