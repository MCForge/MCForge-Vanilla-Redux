using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MCForge {
	public class EconomyDB {
		public static bool Load( Economy.EcoStats p ) {
			if ( File.Exists( "players/economy/" + p.playerName.ToLower() + "DB.txt" ) ) {
				foreach ( string line in File.ReadAllLines( "players/economy/" + p.playerName.ToLower() + "DB.txt" ) ) {
					if ( !string.IsNullOrEmpty( line ) && !line.StartsWith( "#" ) ) {
						string key = line.Split( '=' )[0].Trim();
						string value = line.Split( '=' )[1].Trim();
						string section = "nowhere yet...";

						try {
							switch ( key.ToLower() ) {
								case "money":
								p.money = int.Parse(value);
								section = key;
								break;
								case "total":
								p.totalSpent = int.Parse(value);
								section = key;
								break;
								case "purchase":
								p.purchase = value;
								section = key;
								break;
								case "payment":
								p.payment = value;
								section = key;
								break;
								case "salary":
								p.salary = value;
								section = key;
								break;
								case "fine":
								p.fine = value;
								section = key;
								break;
							}
						} catch(Exception e) {
							Server.s.Log( "Loading " + p.playerName + "'s economy database failed at section: " + section );
							Server.ErrorLog( e );
						}
					}
				}
				return true;
			} else {
				p.fine = "";
				p.money = Player.Find(p.playerName).money;
				p.payment = "";
				p.purchase = "";
				p.salary = "";
				p.totalSpent = 0;
				Save( p );
				return false;
			}
		}

		public static void Save( Economy.EcoStats p ) {
			StreamWriter sw = new StreamWriter( File.Create( "players/economy/" + p.playerName.ToLower() + "DB.txt" ) );
			sw.WriteLine( "Money = " + p.money);
			sw.WriteLine( "Total = " + p.totalSpent);
			sw.WriteLine( "Purchase = " + p.purchase);
			sw.WriteLine( "Payment = " + p.payment);
			sw.WriteLine( "Salary = " + p.salary);
			sw.WriteLine( "Fine = " + p.fine);
			sw.Flush();
			sw.Close();
			sw.Dispose();
		}
	}
}
