using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MCForge {
    public class EXPLevel {
        public static List<EXPLevel> levels = new List<EXPLevel>();
        public static string levelsFile = "properties/explevel.properties";

        public int levelID;
        public int requiredEXP;
        public int reward;

        public EXPLevel( int id, int exp, int rew ) {
            levelID = id;
            requiredEXP = exp;
            reward = rew;
        }

        public static void Load() {
            levels = new List<EXPLevel>();
            if ( File.Exists( levelsFile ) ) {
                foreach ( string line in File.ReadAllLines( levelsFile ) ) {
                    if ( !string.IsNullOrEmpty( line ) && !line.StartsWith( "#" ) ) {
                        if ( line.Split( ':' ).Length == 3 ) {
                            levels.Add( new EXPLevel( int.Parse( line.Split( ':' )[0].Trim() ), int.Parse( line.Split( ':' )[1].Trim() ), int.Parse( line.Split( ':' )[2].Trim() ) ) );
                        }
                    }
                }
            } else {
                levels.Add( new EXPLevel( 1, 0, 0 ) );
                levels.Add( new EXPLevel( 2, 1000, 50 ) );
                levels.Add( new EXPLevel( 3, 5000, 50) );
                levels.Add( new EXPLevel( 4, 9000, 50 ) );
                levels.Add( new EXPLevel( 5, 13000, 50 ) );
                levels.Add( new EXPLevel( 6, 17000, 50 ) );
                levels.Add( new EXPLevel( 7, 21000, 50 ) );
                levels.Add( new EXPLevel( 8, 25000, 50 ) );
                levels.Add( new EXPLevel( 9, 29000, 50 ) );
                levels.Add( new EXPLevel( 10, 33000, 75 ) );
                levels.Add( new EXPLevel( 11, 37000, 100 ) );
                levels.Add( new EXPLevel( 12, 41000, 100 ) );
                levels.Add( new EXPLevel( 13, 45000, 100 ) );
                levels.Add( new EXPLevel( 14, 49000, 100 ) );
                levels.Add( new EXPLevel( 15, 53000, 100 ) );
                levels.Add( new EXPLevel( 16, 57000, 100 ) );
                levels.Add( new EXPLevel( 17, 61000, 100 ) );
                levels.Add( new EXPLevel( 18, 65000, 100 ) );
                levels.Add( new EXPLevel( 19, 69000, 100 ) );
                levels.Add( new EXPLevel( 20, 73000, 100 ) );

                Save();
            }
        }

        public static void Save() {
            StreamWriter sw = new StreamWriter( File.Create( levelsFile ) );
            sw.WriteLine( "# The format goes levelID : requiredEXP : reward" );
            foreach ( EXPLevel lvl in levels ) {
                sw.WriteLine( lvl.levelID + ":" + lvl.requiredEXP + ":" + lvl.reward );
            }
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }

        public static EXPLevel Find( int id ) {
            foreach ( EXPLevel level in levels ) {
                if ( level.levelID == id ) {
                    return level;
                }
            }
            return null;
        }
    }
}