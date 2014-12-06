using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MCForge {
	public class PlayerDB {
		public static bool Load( Player p ) {
			if ( File.Exists( "players/" + p.name.ToLower() + "DB.txt" ) ) {
				foreach ( string line in File.ReadAllLines( "players/" + p.name.ToLower() + "DB.txt" ) ) {
					if ( !string.IsNullOrEmpty( line ) && !line.StartsWith( "#" ) ) {
						string key = line.Split( '=' )[0].Trim();
						string value = line.Split( '=' )[1].Trim();
						string section = "nowhere yet...";

						try {
							switch ( key.ToLower() ) {
								case "title":
								p.title = value;
								section = key;
								break;
								case "titlecolor":
								p.titlecolor = value;
								section = key;
								break;
								case "color":
								p.color = value;
								section = key;
								break;
								case "money":
								p.money = int.Parse( value );
								section = key;
								break;
								case "timespent":
								p.time = value ;
								section = key;
								break;
								case "firstlogin":
								p.firstLogin = DateTime.Parse( value );
								section = key;
								break;
								case "lastlogin":
								p.lastlogin = DateTime.Parse( value );
								section = key;
								break;
								case "totallogins":
								p.totalLogins = int.Parse( value ) + 1;
								section = key;
								break;
								case "totalkicked":
								p.totalKicked = int.Parse( value );
								section = key;
								break;
								case "overalldeath":
								p.overallDeath = int.Parse( value );
								section = key;
								break;
								case "overallblocks":
								p.overallBlocks = int.Parse( value );
								section = key;
								break;
								case "nick":
								p.DisplayName = value;
								section = key;
								break;
							}

							EXPLevel currLevel = null;
							foreach ( EXPLevel lvl in EXPLevel.levels ) {
								if ( lvl.requiredEXP <= p.points ) {
									currLevel = lvl;
								}
							}

							if ( currLevel != null ) {
								p.explevel = currLevel;
							} else {
								p.explevel = EXPLevel.levels[0];
							}
						} catch(Exception e) {
							Server.s.Log( "Loading " + p.name + "'s database failed at section: " + section );
							Server.ErrorLog( e );
						}

						p.timeLogged = DateTime.Now;
					}
				}
				return true;
			} else {
				p.title = "";
				p.titlecolor = "";
				p.color = p.group.color;
				p.money = 0;
				p.firstLogin = DateTime.Now;
				p.lastlogin = DateTime.Now;
				p.totalLogins = 1;
				p.totalKicked = 0;
				p.overallDeath = 0;
				p.overallBlocks = 0;
				p.points = 0;
				p.time = "0 0 0 1";
				p.timeLogged = DateTime.Now;
				p.explevel = EXPLevel.levels[0];
				Save( p );
				return false;
			}
		}

		public static void Save( Player p ) {
			StreamWriter sw = new StreamWriter( File.Create( "players/" + p.name.ToLower() + "DB.txt" ) );
			sw.WriteLine( "IP = " + p.ip);
			sw.WriteLine( "TimeSpent = " + p.time);
			sw.WriteLine( "Title = " + p.title);
			sw.WriteLine( "TitleColor = " + p.titlecolor);
			sw.WriteLine( "Color = " + p.color);
			sw.WriteLine( "Money = " + p.money);
			sw.WriteLine( "FirstLogin = " + p.firstLogin);
			sw.WriteLine( "LastLogin = " + p.lastlogin);
			sw.WriteLine( "TotalLogins = " + p.totalLogins);
			sw.WriteLine( "TotalKicked = " + p.totalKicked);
			sw.WriteLine( "OverallDeaths = " + p.overallDeath);
			sw.WriteLine( "OverallBlocks = " + p.overallBlocks);
			sw.WriteLine ("Nick = " + p.DisplayName );
			sw.Flush();
			sw.Close();
			sw.Dispose();
		}
	}
}
