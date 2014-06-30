using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MCForge.SQL;
namespace MCForge
{
    public class Game
    {
        public virtual string name;
        public Level level;
        public List<string> doneLevels = new List<string>();
        public List<Player> players = new List<Player>();
        public virtual void Start() { }
        public virtual void Join(Player p) { }
        public virtual void Leave(Player p) { }
        public void Save(Player p)
        {
            if (players.Contains(p))
            {
                SQLite.executeQuery("UPDATE `" + name + "Scores` SET Points=" + p.gamePoints[this] + " WHERE Username='" + p.name + "'");
            }
        }
        public static Game Find(string message)
        {
            foreach (Game g in Server.games)
                if (g.name.Replace("MCForge.", "").Contains(message))

                    return g;
            return null;
        }
        public static string Games()
        {
            string builder = "";
            foreach (Game g in Server.games)
                builder += g + ",";
            return builder.Substring(0, builder.Length - 1).Replace("MCForge.", "");
        }
        public void AddStats(Player p)
        {
            try
            {
                using (System.Data.DataTable dt = SQLite.fillData("SELECT * FROM `" + name + "Scores` WHERE Username='" + p.name + "'"))
                {
                    if (!p.gamePoints.ContainsKey(this))
                        p.gamePoints.Add(this, (double)dt.Rows[0]["Points"]);
                }
            }
            catch
            {
                if (!p.gamePoints.ContainsKey(this)) //check again
                    p.gamePoints.Add(this, 0.0);
                SQLite.executeQuery("INSERT INTO `" + name + "Scores` (Username, Points) VALUES ('" + p.name + "', " + p.gamePoints[this] + ");");
            }
        }
        public static void Initialize()
        {
            if (!Directory.Exists("extra/games"))
                Directory.CreateDirectory("extra/games");
            foreach (string game in Directory.GetFiles("extra/games", "*.dll"))
            {
                string name = game.Substring(game.LastIndexOf('\\') + 1);

                Assembly asm = Assembly.LoadFrom(game);
                var gamewithoutext = System.IO.Path.GetFileNameWithoutExtension(game).ToString();
                Server.s.Log(gamewithoutext);
                Type type = asm.GetTypes()[0];
                object instance = Activator.CreateInstance(type);
                Server.games.Add((Game)instance);
            }
            for (int i = Server.games.Count - 1; i >= 0; i--)
            {
                Server.games[i].Start();
            }

            }
        public static string LevelName(string directory)
        {
            return directory.Substring(directory.LastIndexOf('\\') + 1).Replace(".mcf", "");
        }
        public static void SaveGames(Player p)
        {
            Server.games.ForEach(delegate(Game g)
            {
                g.Save(p);
            });
        }
    }
}