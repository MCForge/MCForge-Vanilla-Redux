using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCForge.Commands
{
    public class CmdLeave : Command
    {
        public override string name { get { return "leave"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override void Use(Player p, string message)
        {
            foreach (Game g in Server.games)
                if (g.players.Contains(p))
                {
                    g.Leave(p);
                    Player.SendMessage(p, "You have left " + g.name + "!");
                    return;
                }
            Player.SendMessage(p, "Looks like you weren't playing a game.");
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/leave -- leaves your game");
        }
    }
}