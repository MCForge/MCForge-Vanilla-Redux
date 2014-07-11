using System;

namespace MCForge
{
    public class CmdTeamChat : Command
    {
        public override string name { get { return "teamchat"; } }
        public override string shortcut { get { return "tc"; } }
        public override string type { get { return "ctf"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdTeamChat() { }

        public override void Use(Player p, string message)
        {
            p.teamchat = !p.teamchat;
            if (p.teamchat) Player.SendMessage(p, "All messages will now be sent to Team members only.");
            else Player.SendMessage(p, "Team chat turned off");
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/teamchat - Makes all messages sent go to team members by default");
        }
    }
}