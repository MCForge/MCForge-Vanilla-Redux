using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Text;

namespace MCForge
{
	public class BlocksDB {
		public static List<Blockchange> blockchanges = new List<Blockchange>();
		static string blockchangeFile = "blockchangeDB.gz";
		static string uncompressedblockchangeFile = "blockchangeDB.txt";

		public static void Load() {
			if ( File.Exists( blockchangeFile ) ) {
				foreach ( string line in Unzip(File.ReadAllBytes( blockchangeFile)).Split(new string[] { Environment.NewLine }, StringSplitOptions.None) ) {
					string[] props = line.Split( ':' );
					ushort x, y, z;
					ushort type;
					string username, level;
					DateTime timePerformed;
					bool deleted;

					if ( props.Length == 8 ) {
						level = props[0];
						x = ushort.Parse( props[1] );
						y = ushort.Parse( props[2] );
						z = ushort.Parse( props[3] );
						type = ushort.Parse( props[4] );
						username = props[5];
						timePerformed = DateTime.ParseExact( props[6], "dd/MM/yy HH-mm-ss", null );
						try {
							deleted = bool.Parse( props[7] );

							blockchanges.Add( new Blockchange( level, x, y, z, type, username, timePerformed, deleted ) );
						} catch(Exception e) {
							Logger.WriteError (e);
						}
					}
				}
			}
		}

		public static void Save() {
			try {
				StreamWriter sw = new StreamWriter( File.Create( uncompressedblockchangeFile ) );
				blockchanges.ForEach( delegate( Blockchange b ) {
					sw.WriteLine( b.level + ":" + b.x + ":" + b.y + ":" + b.z + ":" + b.type.ToString() + ":" + b.username + ":" + b.timePerformed.ToString( "dd/MM/yy HH-mm-ss" ) + ":" + b.deleted.ToString().ToLower() );
				} );
				sw.Flush();
				sw.Close();
				sw.Dispose();
				File.WriteAllBytes(blockchangeFile, Zip(File.ReadAllText(uncompressedblockchangeFile)));
				File.Delete(uncompressedblockchangeFile);
			} catch(Exception e) {
				Logger.WriteError (e);
			}
		}
		public static void CopyTo(Stream src, Stream dest) {
			byte[] bytes = new byte[4096];

			int cnt;

			while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0) {
				dest.Write(bytes, 0, cnt);
			}
		}

		public static byte[] Zip(string str) {
			var bytes = Encoding.UTF8.GetBytes(str);

			using (var msi = new MemoryStream(bytes))
				using (var mso = new MemoryStream()) {
				using (var gs = new GZipStream(mso, CompressionMode.Compress)) {
					//msi.CopyTo(gs);
					CopyTo(msi, gs);
				}

				return mso.ToArray();
			}
		}

		public static string Unzip(byte[] bytes) {
			using (var msi = new MemoryStream(bytes))
				using (var mso = new MemoryStream()) {
				using (var gs = new GZipStream(msi, CompressionMode.Decompress)) {
					//gs.CopyTo(mso);
					CopyTo(gs, mso);
				}

				return Encoding.UTF8.GetString(mso.ToArray());
			}
		}

	}

	public class Blockchange {
		public string level;
		public ushort x, y, z;
		public ushort type;
		public string username;
		public DateTime timePerformed;
		public bool deleted;

		public Blockchange() { }

		public Blockchange( string lvl, ushort xx, ushort yy, ushort zz, ushort block, string user, DateTime time, bool del ) {
			level = lvl;
			x = xx;
			y = yy;
			z = zz;
			type = block;
			username = user;
			timePerformed = time;
			deleted = del;
		}
	}
}
