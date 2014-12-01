using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCForge.Commands
{
    public class CmdScores : Command
    {
        public override string name { get { return "scores"; } }
        public override string shortcut { get { return "stats"; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override void Use(Player p, string message)
        {
      /*      if (message == "") { Help(p); }
            else if (Player.Find(message) != null)
            {
                Player who = Player.Find(message);
                Player.SendMessage(p, who.name + "'s stats!");
                foreach (Game g in Server.games)
                    if (who.gamePoints.ContainsKey(g))
                        Player.SendMessage(p, g.name + " - " + who.gamePoints[g].ToString());
                    else
                    {
                        using (System.Data.DataTable dt = SQLite.fillData("SELECT * FROM `" + g.name + "Scores` WHERE Username='" + who.name + "'"))
                        {
                            if (dt.Rows.Count == 0)
                                Player.SendMessage(p, g.name + " - 0");
                            else
                                Player.SendMessage(p, g.name + " - " + dt.Rows[0]["Points"].ToString());
                        }
                    }
            }
 /*           else if (Game.Find(message) != null)
            {
                Game g = Game.Find(message);
                Server.s.Log(g.ToString());
                Player.SendMessage(p, "Top players from " + g.name + "!");
                using (System.Data.DataTable dt = SQLite.fillData("SELECT * FROM `" + g.name + "Scores` ORDER BY Points DESC LIMIT 10"))
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Player.SendMessage(p, string.Format(i + 1 + ".) " + dt.Rows[i]["Username"] + " - " + dt.Rows[i]["Points"]));
                    }
                }
            }
            else
            {
                if (p == null)
                {
                    Player.SendMessage(null, "Your stats are over 9000");
                    return;
                }
                Player.SendMessage(p, "Your stats!");
                foreach (Game g in Server.games)
                    if (p.gamePoints.ContainsKey(g))
                        Player.SendMessage(p, g.name + " - " + p.gamePoints[g].ToString());
                    else
                    {
                        using (System.Data.DataTable dt = SQLite.fillData("SELECT * FROM `" + g.name + "Scores` WHERE Username='" + p.name + "'"))
                        {
                            if (dt.Rows.Count == 0)
                                Player.SendMessage(p, g.name + " - 0");
                            else
                                Player.SendMessage(p, g.name + " - " + dt.Rows[0]["Points"].ToString());
                        }
                    }
            }*/
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/scores [player] -- sends you all of a player's scores.");
           // Player.SendMessage(p, "/scores [game] -- sends you the top 10 players of each game.");
            Player.SendMessage(p, "Games: " + Game.Games());
        }
    }
}