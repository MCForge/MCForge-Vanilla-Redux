using System;
using System.IO;
using System.Linq;

namespace MCForge
{
    public class CmdBlue : Command
    {
        public override string name { get { return "join"; } }
        public override string shortcut { get { return "jo"; } }
        public override string type { get { return "ctf"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdBlue() { }
        public override void Use(Player p, string message)
        {
            if (p == null) { Player.SendMessage(p, "This command can only be used in-game!"); return; }
            if (Server.pctf.CTFStatus() == 0) { Player.SendMessage(p, "There is no CTF game currently in progress."); return; }
            if (!Server.ctfRound) { p.SendMessage("The current ctf round hasn't started yet!"); return; }
            int diff = Server.pctf.red.Count() - Server.pctf.blu.Count();
            bool unbalanced = false;
            Random random = new Random();
            if (message == "blue")
            {
                if (p.level.name == Server.pctf.currentLevelName) return;
                if (p.pteam == 0)
                {
                    int a = random.Next(-2, -1);
                    if (diff <= a)
                        unbalanced = true;
                    if (unbalanced)
                    {
                        p.SendMessage(c.gray + " - " + Server.DefaultColor + "You have been autobalanced!" + c.gray + " - ");
                        Server.pctf.joinTeam(p, "red");
                    }
                    Server.pctf.joinTeam(p, "blue");
                }
            }
            else
            {
                if (p.level.name == Server.pctf.currentLevelName) return;
                if (p.pteam == 0)
                {
                    int a = random.Next(1, 2);
                    if (diff >= a)
                        unbalanced = true;
                    if (unbalanced)
                    {
                        p.SendMessage(c.gray + " - " + Server.DefaultColor + "You have been autobalanced!" + c.gray + " - ");
                        Server.pctf.joinTeam(p, "blue");
                    }
                    Server.pctf.joinTeam(p, "red");
                }
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/join blue/red - Joins the blu or red team.");
        }
    }
}