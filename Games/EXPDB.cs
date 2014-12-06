using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MCForge
{
    public class EXPDB
    {
        public static bool Load( Player p ) {
            if ( File.Exists( "players/" + p.name + "DB.txt" ) ) {
                foreach ( string line in File.ReadAllLines( "players/" + p.name.ToLower() + "DB.txt" ) ) {
                    if ( !string.IsNullOrEmpty( line ) && !line.StartsWith( "#" ) ) {
                        string key = line.Split( '=' )[0].Trim();
                        string value = line.Split( '=' )[1].Trim();
                        string section = "nowhere yet...";

                        try {
                            switch ( key.ToLower() ) {
                                case "points":
                                    p.points = int.Parse( value );
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
                            Server.s.Log( "Loading " + p.name + "'s EXP database failed at section: " + section );
                            Server.ErrorLog( e );
                        }

                        p.timeLogged = DateTime.Now;
                    }
                }

                p.SetPrefix();
                return true;
            } else {
                p.points = 0;
                p.timeLogged = DateTime.Now;
                p.explevel = EXPLevel.levels[0];
                Save( p );
                return false;
            }
        }

        public static void Save( Player p ) {
            StreamWriter sw = new StreamWriter( File.Create( "exp/" + p.name.ToLower() + "DB.txt" ) );
            sw.WriteLine( "Points = " + p.points );
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
    }
}
