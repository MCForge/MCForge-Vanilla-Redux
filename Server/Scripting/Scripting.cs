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
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;

using MCForge.Core.Scripting;
namespace MCForge
{
    /// <summary>
    /// Handles all the scripting stuff for the Server
    /// </summary>
    public static class Scripting
    {
        private static readonly string autoLoadFilenameCSharp = "text/cmdautoload.txt";
        private static readonly string autoLoadFilenameVB = "text/cmdautoloadVB.txt";
        /// <summary>
        /// Creates the missing autoload files if they don't exist.
        /// </summary>
        static Scripting()
        {
            if (!File.Exists(autoLoadFilenameCSharp))
                File.Create(autoLoadFilenameCSharp);
            if (!File.Exists(autoLoadFilenameVB))
                File.Create(autoLoadFilenameVB);
        }
        /// <summary>
        /// The CSharp Scripting Provider/Engine
        /// </summary>
        public static ScriptingEngine CSharpProvider = new ScriptingCSharp();
        /// <summary>
        /// The Visual Basic Scripting Provider/Engine
        /// </summary>
        public static ScriptingEngine VBProvider = new ScriptingVB();

        /// <summary>
        /// AutoLoads the commands the user wants to load at startup.
        /// </summary>
        public static void AutoLoad()
        {
            try {
                CSharpProvider.AutoLoad( File.ReadAllLines( autoLoadFilenameCSharp ) );
                VBProvider.AutoLoad( File.ReadAllLines( autoLoadFilenameVB ) );
            } catch { }
        }
    }
}
