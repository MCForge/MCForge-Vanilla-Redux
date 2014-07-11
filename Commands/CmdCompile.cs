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
using System.IO;
namespace MCForge.Commands
{
    public class CmdCompile : Command
    {
        public override string name { get { return "compile"; } }
        public override string shortcut { get { return  ""; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdCompile() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            bool success = false;
            string[] param = message.Split(' ');
            string name = param[0];

            if (param.Length == 1)
            {
                if (File.Exists("extra/commands/source/Cmd" + message + ".cs"))
                {
                    try
                    {
                        success = Scripting.CSharpProvider.Compile(message);
                    }
                    catch (Exception e)
                    {
                        Server.ErrorLog(e);
                        Player.SendMessage(p, "An exception was thrown during compilation.");
                        return;
                    }
                    if (success)
                    {
                        Player.SendMessage(p, "Compiled successfully.");
                    }
                    else
                    {
                        Player.SendMessage(p, "Compilation error.  Please check compile.log for more information.");
                    }
                    return;
                }
                else
                {
                    Player.SendMessage(p, "file &9Cmd" + message + ".cs " + Server.DefaultColor + "not found!");
                }
            }
            if (param[1] == "vb")
            {
                message = message.Remove(message.Length - 3, 3);
                if (File.Exists("extra/commands/source/Cmd" + message + ".vb"))
                {
                    try
                    {
                        //success = ScriptingVB.Compile(name);
                        success = Scripting.VBProvider.Compile(name);
                    }
                    catch (Exception e)
                    {
                        Server.ErrorLog(e);
                        Player.SendMessage(p, "An exception was thrown during compilation.");
                        return;
                    }
                    if (success)
                    {
                        Player.SendMessage(p, "Compiled successfully.");
                    }
                    else
                    {
                        Player.SendMessage(p, "Compilation error.  Please check compile.log for more information.");
                    }
                    return;
                }
                else
                {
                    Player.SendMessage(p, "file &9Cmd" + message + ".vb " + Server.DefaultColor + "not found!");
                }
            }

        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/compile <class name> - Compiles a command class file into a DLL.");
            Player.SendMessage(p, "/compile <class name> vb - Compiles a command class (that was written in visual basic) file into a DLL.");
            Player.SendMessage(p, "class name: &9Cmd&e<class name>&9.cs");
        }
    }
}
