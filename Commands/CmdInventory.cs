using System;

namespace MCForge
{
    public class CmdInventory : Command
    {
        public override string name { get { return "inventory"; } }
        public override string shortcut { get { return "inv"; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                Player.SendMessage(p, c.red + Server.moneys + Server.DefaultColor + ": " + p.money);
                Player.SendMessage(p, c.red + "Lives" + Server.DefaultColor + ": " + p.lavasurvival.lifeNum);
                Player.SendMessage(p, c.red + "Sponges" + Server.DefaultColor + ": " + p.spongesLeft);
            }
            else
            {
                Player who = Player.Find(message);
                if (who == null)
                {
                    Player.SendMessage(p, "Players is not online!");
                    return;
                }
                else
                {
                    Player.SendMessage(p, who.color + who.name + Server.DefaultColor + "'s inventory:");
                    Player.SendMessage(p, c.red + Server.moneys + Server.DefaultColor + ": " + who.money);
                    Player.SendMessage(p, c.red + "Lives" + Server.DefaultColor + ": " + who.lavasurvival.lifeNum);
                    Player.SendMessage(p, c.red + "Sponges" + Server.DefaultColor + ": " + who.spongesLeft);
                }
            }
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/inventory <player> - Shows the inventory of <player>");
        }
    }
}