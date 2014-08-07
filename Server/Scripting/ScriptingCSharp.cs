/*
	Copyright © 2014 MCForge-Redux Team
	Written by Gamemakergm (gamemakergmdev@gmail.com)

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
namespace MCForge.Core.Scripting
{
    public sealed class ScriptingCSharp : ScriptingEngine
    {
        public override string providerName { get { return "CSharp"; } }
        public override string extension { get { return ".cs"; } }
        public override string GetSkeleton(string cmdName)
        {
            return "/*" + Environment.NewLine +
                    "\tAuto-generated command skeleton class." + Environment.NewLine +
                    Environment.NewLine +
                    "\tUse this as a basis for custom commands implemented via the MCForge scripting framework." + Environment.NewLine +
                    "\tFile and class should be named a specific way.  For example, /update is named 'CmdUpdate.cs' for the file, and 'CmdUpdate' for the class." + Environment.NewLine +
                    "*/" + Environment.NewLine +
                    Environment.NewLine +
                    "// Add any other using statements you need up here, of course." + Environment.NewLine +
                    "// As a note, MCForge is designed for .NET 3.5." + Environment.NewLine +
                    "using System;" + Environment.NewLine +
                    Environment.NewLine +
                    "namespace MCForge" + Environment.NewLine +
                    "{" + Environment.NewLine +
                    "\tpublic class " + cmdName.UppercaseFirst() + " : Command" + Environment.NewLine +
                    "\t{" + Environment.NewLine +
                    "\t\t// The command's name, in all lowercase.  What you'll be putting behind the slash when using it." + Environment.NewLine +
                    "\t\tpublic override string name { get { return \"" + cmdName.ToLower() + "\"; } }" + Environment.NewLine +
                    Environment.NewLine +
                    "\t\t// Command's shortcut (please take care not to use an existing one, or you may have issues." + Environment.NewLine +
                    "\t\tpublic override string shortcut { get { return \"\"; } }" + Environment.NewLine +
                    Environment.NewLine +
                    "\t\t// Determines which submenu the command displays in under /help." + Environment.NewLine +
                    "\t\tpublic override string type { get { return \"other\"; } }" + Environment.NewLine +
                    Environment.NewLine +
                    "\t\t// Determines whether or not this command can be used in a museum.  Block/map altering commands should be made false to avoid errors." + Environment.NewLine +
                    "\t\tpublic override bool museumUsable { get { return false; } }" + Environment.NewLine +
                    Environment.NewLine +
                    "\t\t// Determines the command's default rank.  Valid values are:" + Environment.NewLine +
                    "\t\t// LevelPermission.Nobody, LevelPermission.Banned, LevelPermission.Guest" + Environment.NewLine +
                    "\t\t// LevelPermission.Builder, LevelPermission.AdvBuilder, LevelPermission.Operator, LevelPermission.Admin" + Environment.NewLine +
                    "\t\tpublic override LevelPermission defaultRank { get { return LevelPermission.Banned; } }" + Environment.NewLine +
                    Environment.NewLine +
                    "\t\t// This is where the magic happens, naturally." + Environment.NewLine +
                    "\t\t// p is the player object for the player executing the command.  message is everything after the command invocation itself." + Environment.NewLine +
                    "\t\tpublic override void Use(Player p, string message)" + Environment.NewLine +
                    "\t\t{" + Environment.NewLine +
                    "\t\t\tPlayer.SendMessage(p, \"Hello World!\");" + Environment.NewLine +
                    "\t\t}" + Environment.NewLine +
                    Environment.NewLine +
                    "\t\t// This one controls what happens when you use /help [commandname]." + Environment.NewLine +
                    "\t\tpublic override void Help(Player p)" + Environment.NewLine +
                    "\t\t{" + Environment.NewLine +
                    "\t\t\tPlayer.SendMessage(p, \"/" + cmdName.ToLower() + " - Does stuff.  Example command.\");" + Environment.NewLine +
                    "\t\t}" + Environment.NewLine +
                    "\t}" + Environment.NewLine +
                    "}";
        }
    }
}
