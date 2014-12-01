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
