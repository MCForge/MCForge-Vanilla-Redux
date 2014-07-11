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
    public sealed class ScriptingVB : ScriptingEngine
    {
        public override string providerName { get { return "VisualBasic"; } }
        public override string extension { get { return ".vb"; } }
        public override string GetSkeleton(string cmdName)
        {
            return "Imports MCForge" + Environment.NewLine +
                         "'Auto-generated command skeleton class." + Environment.NewLine +
                         Environment.NewLine +
                         "'Use this as a basis for custom commands implemented via the MCForge scripting framework." + Environment.NewLine +
                         "'File and class should be named a specific way.  For example, /update is named 'CmdUpdate.vb' for the file, and 'CmdUpdate' for the class." + Environment.NewLine +
                         "'" + Environment.NewLine +
                         Environment.NewLine +
                         "' Add any other using statements you need up here, of course." + Environment.NewLine +
                         "' As a note, MCForge is designed for .NET 3.5." + Environment.NewLine +

                         Environment.NewLine +
                         "Namespace MCForge" + Environment.NewLine +
                         "\tPublic Class " + cmdName.UppercaseFirst() + Environment.NewLine +
                         "\t\tInherits Command " + Environment.NewLine +
                         "' The command's name, IN ALL LOWERCASE.  What you'll be putting behind the slash when using it." + Environment.NewLine +

                         "\t\tPublic Overrides ReadOnly Property name() As String" + Environment.NewLine +
                         "\t\t\tGet" + Environment.NewLine +
                         "\t\t\t\tReturn \"" + cmdName.ToLower() + "\"" + Environment.NewLine +
                         "\t\t\tEnd Get" + Environment.NewLine +
                         "\t\tEnd Property" + Environment.NewLine +
                         Environment.NewLine +
                         "' Command's shortcut (please take care not to use an existing one, or you may have issues." + Environment.NewLine +
                         "\t\tPublic Overrides ReadOnly Property shortcut() As String" + Environment.NewLine +
                         "\t\t\tGet" + Environment.NewLine +
                         "\t\t\t\tReturn \"\"" + Environment.NewLine +
                         "\t\t\tEnd Get" + Environment.NewLine +
                         "\t\tEnd Property" + Environment.NewLine +
                         Environment.NewLine +
                         "' Determines which submenu the command displays in under /help." + Environment.NewLine +
                         "\t\tPublic Overrides ReadOnly Property type() As String" + Environment.NewLine +
                         "\t\t\tGet" + Environment.NewLine +
                         "\t\t\t\tReturn \"other\"" + Environment.NewLine +
                         "\t\t\tEnd Get" + Environment.NewLine +
                         "\t\t End Property" + Environment.NewLine +
                         Environment.NewLine +
                         "' Determines whether or not this command can be used in a museum.  Block/map altering commands should be made false to avoid errors." + Environment.NewLine +
                         "\t\tPublic Overrides ReadOnly Property museumUsable() As Boolean" + Environment.NewLine +
                         "\t\t\tGet" + Environment.NewLine +
                         "\t\t\t\tReturn False" + Environment.NewLine +
                         "\t\t\tEnd Get" + Environment.NewLine +
                         "\t\tEnd Property" + Environment.NewLine +
                         Environment.NewLine +
                         "' Determines the command's default rank.  Valid values are:" + Environment.NewLine + "' LevelPermission.Nobody, LevelPermission.Banned, LevelPermission.Guest" +
                         Environment.NewLine + "' LevelPermission.Builder, LevelPermission.AdvBuilder, LevelPermission.Operator, LevelPermission.Admin" + Environment.NewLine +
                         "\t\tPublic Overrides ReadOnly Property defaultRank() As LevelPermission" + Environment.NewLine +
                         "\t\t\tGet" + Environment.NewLine +
                         "\t\t\t\tReturn LevelPermission.Banned" + Environment.NewLine +
                         "\t\t\tEnd Get" + Environment.NewLine +
                         "\t\tEnd Property" + Environment.NewLine +
                         Environment.NewLine +
                         "' This is where the magic happens, naturally." + Environment.NewLine +
                         "' p is the player object for the player executing the command.  message is everything after the command invocation itself." + Environment.NewLine +
                         "\t\tPublic Overrides Sub Use(p As Player, message As String)" + Environment.NewLine +
                         "\t\t\tPlayer.SendMessage(p, \"Hello World!\")" + Environment.NewLine +
                         "\t\tEnd Sub" + Environment.NewLine +
                         Environment.NewLine +
                         "' This one controls what happens when you use /help [commandname]." + Environment.NewLine +
                         "\t\tPublic Overrides Sub Help(p As Player)" + Environment.NewLine +
                         "\t\t\tPlayer.SendMessage(p, \"/" + cmdName.ToLower() + " - Does stuff.  Example command.\")" + Environment.NewLine +

                         "\t\tEnd Sub" + Environment.NewLine +
                         "\tEnd Class" + Environment.NewLine +
                         "End Namespace";
        }
    }
}