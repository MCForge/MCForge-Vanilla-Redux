using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCForge.Commands
{
    public class CmdJoin : Command
    {
        public override string name { get { return "join"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override void Use(Player p, string message)
        {
            if (Server.CTFModeOn)
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
            else
            {
                if (message == "")
                {
                    Help(p);
                    return;
                }
                foreach (Game g in Server.games)
                    if (Game.Games().Contains(message))
                    {
                        g.Join(p);
                        return;
                    }
                Player.SendMessage(p, message + " was not a a valid game");
                Player.SendMessage(p, "Valid : " + Game.Games());
            }
        }
        public override void Help(Player p)
        {
            if (Server.CTFModeOn)
            {
                Player.SendMessage(p, "Joins a CTF Team, valid options are red and blue"); return;
            }
            Player.SendMessage(p, "/join [game] -- joins a game!");
            Player.SendMessage(p, "Available options : " + Game.Games());
        }
    }
}
