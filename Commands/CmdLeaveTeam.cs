using System;
using System.IO;
using System.Linq;

namespace MCForge
{
    public class CmdLeaveTeam : Command
    {
        public override string name { get { return "leaveteam"; } }
        public override string shortcut { get { return "lt"; } }
        public override string type { get { return "ctf"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdLeaveTeam() { }
        public override void Use(Player p, string message)
        {
            if (p == null) { Player.SendMessage(p, "This command can only be used in-game!"); return; }
            if (Server.pctf.CTFStatus() == 0) { Player.SendMessage(p, "There is no CTF game currently in progress."); return; }
            if (!Server.ctfRound) { p.SendMessage("The current ctf round hasn't started yet!"); return; }
            Command.all.Find("goto").Use(p, Server.level);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/leaveteam - Leaves the CTF team you are on.");
        }
    }
}