using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MCForge {
	public class PortalDB {
		public static List<Portal> portals = new List<Portal>();
		static string portalFile = "portalDB.txt";

		public static void Load() {
			if ( File.Exists( portalFile ) ) {
				foreach ( string line in File.ReadAllLines( portalFile ) ) {
					string[] props = line.Split( ':' );
					string foundName, foundEntrance, foundExit, foundX1, foundY1, foundZ1, foundX2, foundY2, foundZ2;

					foundName = props[0];
					foundEntrance = props[1];
					foundExit = props[2];
					foundX1 = props[3];
					foundY1 = props[4];
					foundZ1 = props[5];
					foundX2 = props[6];
					foundY2 = props[7];
					foundZ2 = props[8];

					portals.Add( new Portal( foundName, foundEntrance, foundExit, foundX1, foundY1, foundZ1, foundX2, foundY2, foundZ2 ) );
				}
			}
		}

		public static void Save() {
			StreamWriter sw = new StreamWriter( File.Create( portalFile ) );
			foreach ( Portal p in portals ) {
				sw.WriteLine( p.name + ":" + p.entrance + ":" + p.exit + ":" + p.x1 + ":" + p.y1 + ":" + p.z1 + ":" + p.x2 + ":" + p.y2 + ":" + p.z2 );
			}
			sw.Flush();
			sw.Close();
			sw.Dispose();
		}
	}

	public class Portal {
		public string name, entrance, exit;
		public ushort x1, y1, z1, x2, y2, z2;

		public Portal( string a, string b, string c, string d, string e, string f, string g, string h, string i ) {
			name = a;
			entrance = b;
			exit = c;
			x1 = ushort.Parse( d );
			y1 = ushort.Parse( e );
			z1 = ushort.Parse( f );
			x2 = ushort.Parse( g );
			y2 = ushort.Parse( h );
			z2 = ushort.Parse( i );
		}
	}
}
