using System;
using System.Data;
using System.Collections.Generic;
//using MySql.Data.MySqlClient;
//using MySql.Data.Types;

namespace MCForge
{
	public class CmdAbout : Command
	{
		public override string name { get { return "about"; } }
		public override string shortcut { get { return "b"; } }
		public override string type { get { return "information"; } }
		public override bool museumUsable { get { return false; } }
		public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
		public CmdAbout() { }

		public override void Use(Player p, string message)
		{
			Player.SendMessage(p, "Break/build a block to display information.");
			p.ClearBlockchange();
			p.Blockchange += new Player.BlockchangeEventHandler(AboutBlockchange);
		}
		public override void Help(Player p)
		{
			Player.SendMessage(p, "/about - Displays information about a block.");
		}

		public void AboutBlockchange(Player p, ushort x, ushort y, ushort z, ushort type)
		{
			if (!p.staticCommands) p.ClearBlockchange();
			ushort b = p.level.GetTile(x, y, z);
			if (b == Block.Zero) { Player.SendMessage(p, "Invalid Block(" + x + "," + y + "," + z + ")!"); return; }
			p.SendBlockchange(x, y, z, b);

			string message = "Block (" + x + "," + y + "," + z + "): ";
			message += "&f" + b + " = " + Block.Name(b);
			Player.SendMessage(p, message + Server.DefaultColor + ".");
			message = p.level.foundInfo(x, y, z);
			if (message != "") Player.SendMessage(p, "Physics information: &a" + message);

			string Username = "", TimePerformed = "", BlockUsed = "";
			bool Deleted = false, foundOne = false;

		foreach (Blockchange bc in BlocksDB.blockchanges) {
			if (bc.level == p.level.name && bc.x == x && bc.y == y && bc.z == z) {
				foundOne = true;
				Username = bc.username;
				TimePerformed = bc.timePerformed.ToString ("HH:mm:ss (dd/MM/yy)");
				BlockUsed = Block.Name (bc.type);
				Deleted = bc.deleted;
			}
		}

			if ( foundOne ) {
				if ( !Deleted ) {
					Player.SendMessage( p, "&3Created by " + Server.FindColor( Username.Trim() ) + Username.Trim() + Server.DefaultColor + ", using &3" + BlockUsed );
				} else {
					Player.SendMessage( p, "&4Destroyed by " + Server.FindColor( Username.Trim() ) + Username.Trim() + Server.DefaultColor + ", using &3" + BlockUsed );
				}

				Player.SendMessage( p, "Date and time modified: &2" + TimePerformed );
			} else {

				List<Blockchange> inCache = p.level.blockCache.FindAll( bP => bP.x == x && bP.y == y && bP.z == z );

				for ( int i = 0; i < inCache.Count; i++ ) {
					foundOne = true;
					Deleted = inCache[i].deleted;
					Username = inCache[i].username;
					TimePerformed = inCache[i].timePerformed.ToString( "HH:mm:ss (dd/MM/yy)" );
					BlockUsed = Block.Name( inCache[i].type );
				}

				if ( foundOne ) {
					if ( !Deleted ) {
						Player.SendMessage( p, "&3Created by " + Server.FindColor( Username.Trim() ) + Username.Trim() + Server.DefaultColor + ", using &3" + BlockUsed );
					} else {
						Player.SendMessage( p, "&4Destroyed by " + Server.FindColor( Username.Trim() ) + Username.Trim() + Server.DefaultColor + ", using &3" + BlockUsed );
					}

					Player.SendMessage( p, "Date and time modified: &2" + TimePerformed );
				}
			}

			if ( !foundOne ) {
				Player.SendMessage( p, "This block has not been modified since the map was cleared." );
			}

			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
	}
}