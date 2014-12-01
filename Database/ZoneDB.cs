using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MCForge {
	public class ZoneDB {
		public static List<Zone> zones = new List<Zone>();
		static string zoneFile = "zoneDB.txt";

		public static void Load() {
			if ( File.Exists( zoneFile ) ) {
				foreach ( string line in File.ReadAllLines( zoneFile ) ) {
					string[] props = line.Split( ':' );
					ushort smallX, smallY, smallZ, bigX, bigY, bigZ;
					string owner, level;

					smallX = ushort.Parse( props[0] );
					smallY = ushort.Parse( props[1] );
					smallZ = ushort.Parse( props[2] );
					bigX = ushort.Parse( props[3] );
					bigY = ushort.Parse( props[4] );
					bigZ = ushort.Parse( props[5] );
					owner = props[6];
					level = props[7];

					zones.Add( new Zone( level, owner, smallX, smallY, smallZ, bigX, bigY, bigZ ) );
				}
			}
		}

		public static void Save() {
			StreamWriter sw = new StreamWriter( File.Create( zoneFile ) );
			foreach ( Zone z in zones ) {
				sw.WriteLine( z.smallX + ":" + z.smallY + ":" + z.smallZ + ":" + z.bigX + ":" + z.bigY + ":" + z.bigZ + ":" + z.owner + ":" + z.level );
			}
			sw.Flush();
			sw.Close();
			sw.Dispose();
		}
	}

	public class Zone {
		public string level, owner;
		public ushort smallX, smallY, smallZ, bigX, bigY, bigZ;

		public Zone() { }

		public Zone( string lvl, string own, ushort sx, ushort sy, ushort sz, ushort bx, ushort by, ushort bz ) {
			level = lvl;
			owner = own;
			smallX = sx;
			smallY = sy;
			smallZ = sz;
			bigX = bx;
			bigY = by;
			bigZ = bz;
		}
	}
}
