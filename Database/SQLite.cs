/*
	Copyright � 2011-2014 MCForge-Redux
		
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
using System;
using System.Data;
using System.Data.SQLite;
namespace MCForge
{
    namespace SQL
    {
        public static class SQLite //: Database //Extending for future improvement (Making it object oriented later).
        {
            private static string connStringFormat = "Data Source =" + Server.apppath + "/MCForge.db; Version =3; Pooling ={0}; Max Pool Size =1000;";
            private static SQLiteParameterCollection parameters = new SQLiteCommand().Parameters;

            public static string connString { get { return String.Format(connStringFormat, Server.DatabasePooling); } }
            //[Obsolete("Preferably use Database.executeQuery instead")]
            public static void executeQuery(string queryString)
            {
                Database.executeQuery(queryString);
            }
            //[Obsolete("Preferably use Database.executeQuery instead")]
            public static DataTable fillData(string queryString, bool skipError = false)
            {
                return Database.fillData(queryString, skipError);
            }

            /// <summary>
            /// Adds a parameter to the parameterized SQLite query.
            /// Use this before executing the query.
            /// </summary>
            /// <param name="name">The name of the parameter</param>
            /// <param name="param">The value of the parameter</param>
            public static void AddParams(string name, object param) {
                parameters.AddWithValue(name, param);
            }
            /// <summary>
            /// Clears the parameters added with <see cref="MCForge.SQL.MySQL.AddParams(System.string, System.string)"/>
            /// <seealso cref="MCForge.SQL.MySQL"/>
            /// </summary>
            public static void ClearParams() {
                parameters.Clear();
            }
            private static void AddSQLiteParameters(SQLiteCommand command) {
                foreach (SQLiteParameter param in parameters)
                    command.Parameters.Add(param);
            }
            private static void AddSQLiteParameters(SQLiteDataAdapter dAdapter) {
                foreach (SQLiteParameter param in parameters)
                    dAdapter.SelectCommand.Parameters.Add(param);
            }

            internal static void execute(string queryString) {
                using (var conn = new SQLiteConnection(SQLite.connString)) {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(queryString, conn)) {
                        AddSQLiteParameters(cmd);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }

            internal static void fill(string queryString, DataTable toReturn) {
                using (var conn = new SQLiteConnection(SQLite.connString)) {
                    conn.Open();
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(queryString, conn)) {
                        AddSQLiteParameters(da);
                        da.Fill(toReturn);
                    }
                    conn.Close();
                }
            }
            public static bool SqliteColumnExists(this SQLiteCommand cmd, string table, string column)
            {
                lock (cmd.Connection)
                {
                    // make sure table exists
                    cmd.CommandText = string.Format("SELECT sql FROM sqlite_master WHERE type = 'table' AND name = '{0}'", table);
                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        //does column exists?
                        bool hascol = reader.GetString(0).Contains(String.Format("\"{0}\"", column));
                        reader.Close();
                        return hascol;
                    }
                    reader.Close();
                    return false;
                }
            }
        }
    }
}
