/*
	Copyright � 2011-2014 MCForge-Redux
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.opensource.org/licenses/ecl2.php
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
namespace MCForge.Commands
{
    /// <summary>
    /// This is the command /referee
    /// use /help referee in-game for more info
    /// </summary>
    public class CmdReferee : Command
    {
        public override string name { get { return "ref"; } }
        public override string shortcut { get { return  ""; } }
        public override string type { get { return "mod"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdReferee() { }
        public override void Use(Player p, string message)
        {
            if (p == null) { Player.SendMessage(p, "This command can only be used in-game!"); return; }
            if(Server.CTFOnlyServer)
            {
                Player.SendMessage(p, "Ref is not available in CTF");
                return;
            }
            if (p.referee)
            {
                p.referee = false;
      //          LevelPermission perm = Group.findPlayerGroup(name).Permission;
                Player.GlobalDie(p, false);
                Player.GlobalChat(p, p.color + p.name + Server.DefaultColor + " is no longer a referee", false);
                if (Server.zombie.GameInProgess())
                {
                    Server.zombie.InfectPlayer(p);
                }
                else
                {
                    Player.GlobalDie(p, false);
                    Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
                    ZombieGame.infectd.Remove(p);
                    ZombieGame.alive.Add(p);
                    p.color = p.group.color;
                }
            }
            else
            {
                p.referee = true;
                Player.GlobalChat(p, p.color + p.name + Server.DefaultColor + " is now a referee", false);
                Player.GlobalDie(p, false);
                if (Server.zombie.GameInProgess())
                {
                    p.color = p.group.color;
                    try
                    {
                        ZombieGame.infectd.Remove(p);
                        ZombieGame.alive.Remove(p);
                    }
                    catch { }
                    Server.zombie.InfectedPlayerDC();
                }
                else
                {
                    ZombieGame.infectd.Remove(p);
                    ZombieGame.alive.Remove(p);
                    p.color = p.group.color;
                }
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/referee - Turns referee mode on/off.");
        }
    }
}