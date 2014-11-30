using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MCForge {
	public class MessageBlockDB {
		public static List<MessageBlock> messageBlocks = new List<MessageBlock>();
		static string messageBlockFile = "messageBlockDB.txt";

		public static void Load() {
			if ( File.Exists( messageBlockFile ) ) {
				foreach ( string line in File.ReadAllLines( messageBlockFile ) ) {
					string[] props = line.Split( ':' );
					ushort x, y, z;
					string level, message;

					x = ushort.Parse( props[0] );
					y = ushort.Parse( props[1] );
					z = ushort.Parse( props[2] );
					level = props[3];
					message = props[4];

					messageBlocks.Add( new MessageBlock( x, y, z, level, message ) );
				}
			}
		}

		public static void Save() {
			StreamWriter sw = new StreamWriter( File.Create( messageBlockFile ) );
			foreach ( MessageBlock mb in messageBlocks ) {
				sw.WriteLine( mb.x + ":" + mb.y + ":" + mb.z + ":" + mb.level + ":" + mb.message );
			}
			sw.Flush();
			sw.Close();
			sw.Dispose();
		}
	}

	public class MessageBlock {
		public ushort x, y, z;
		public string level, message;

		public MessageBlock( ushort xx, ushort yy, ushort zz, string lvl, string msg ) {
			x = xx;
			y = yy;
			z = zz;
			level = lvl;
			message = msg;
		}
	}
}
