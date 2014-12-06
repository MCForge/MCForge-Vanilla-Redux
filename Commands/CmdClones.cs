/*
	Copyright © 2011-2014 MCForge-Redux
	
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
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
namespace MCForge.Commands
{
	public class CmdClones : Command
	{
		public override string name { get { return "clones"; } }
		public override string shortcut { get { return  "alts"; } }
		public override string type { get { return "information"; } }
		public override bool museumUsable { get { return true; } }
		public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
		public CmdClones() { }

		public override void Use(Player p, string message)
		{
			if ( !Regex.IsMatch(message.ToLower(), @".*%([0-9]|[a-f]|[k-r])%([0-9]|[a-f]|[k-r])%([0-9]|[a-f]|[k-r])") ) {
				if (Regex.IsMatch(message.ToLower(), @".*%([0-9]|[a-f]|[k-r])(.+?).*")) {
					Regex rg = new Regex(@"%([0-9]|[a-f]|[k-r])(.+?)");
					MatchCollection mc = rg.Matches(message.ToLower());
					if (mc.Count > 0) {
						Match ma = mc[0];
						GroupCollection gc = ma.Groups;
						message.Replace("%" + gc[1].ToString().Substring(1), "&" + gc[1].ToString().Substring(1));
					}
				}
			}
			if (message == "") message = p.name;

			string originalName = message.ToLower();

			Player who = Player.Find(message);
			if (who == null)
			{
				Player.SendMessage(p, "Could not find player.");
				return;
			}
			else
			{
				message = who.ip;
			}
			Command.all.Find ("whoip").Use (p, message);
		}

		public override void Help(Player p)
		{
			Player.SendMessage(p, "/clones <name> - Finds everyone with the same IP as <name>"); //Fixed typo
		}
	}
}
