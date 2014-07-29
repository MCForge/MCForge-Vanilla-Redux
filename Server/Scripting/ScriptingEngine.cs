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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.CodeDom.Compiler;
using MCForge;
namespace MCForge.Core.Scripting
{
    /// <summary>
    /// Represents a scripting language that can be used to make, load and save commands.
    /// </summary>
    public abstract class ScriptingEngine
    {
        #region Static Constructor
        /// <summary>
        /// Creates missing directories if they don't exist
        /// </summary>
        static ScriptingEngine()
        {
            if (!Directory.Exists(sourcePath))
                Directory.CreateDirectory(sourcePath);
            if (!Directory.Exists(dllPath))
                Directory.CreateDirectory(dllPath);
        }
        #endregion

        #region Static Readonly Variables
        private static readonly string divider = new string('-', 25);
        private static readonly string compilerLogFilename = "logs/errors/compiler.log";
        private static readonly string sourcePath = "extra/commands/source/";
        private static readonly string dllPath = "extra/commands/dll/";
        #endregion

        #region Abstract Getters
        public abstract string providerName { get; }
        public abstract string extension { get; }
        #endregion

        #region Abstract Methods
        /// <summary>
        /// Creates a new, empty command class
        /// </summary>
        /// <param name="cmdName">Name of the command</param>
        /// <returns>The automatically generated skeleton class</returns>
        public abstract string GetSkeleton(string cmdName);
        #endregion

        #region Static Utilities
        /// <summary>
        /// Writes errors to the compiler log
        /// Every String given will be a different line
        /// </summary>
        /// <param name="errorMessage">Variable String error messages</param>
        private static void WriteErrors(params string[] errorMessage)
        {
            if (!File.Exists(compilerLogFilename))
            {
                File.WriteAllLines(compilerLogFilename, errorMessage);
            }
            // File exists. Append
            else
            {
                File.AppendAllText(compilerLogFilename, divider + "\n");
                File.AppendAllLines(compilerLogFilename, errorMessage);
            }
        }
        #endregion

        #region Implemented Methods
        /// <summary>
        /// Creates a new empty/skeleton class
        /// </summary>
        /// <param name="cmdName">Name of the command</param>
        public void CreateNew(string cmdName)
        {
            string path = sourcePath + "Cmd" + cmdName + extension;
            File.WriteAllText(path, GetSkeleton(cmdName));
        }
        /// <summary>
        /// Compiles a command
        /// </summary>
        /// <param name="commandName">Name of the command to compile</param>
        /// <returns>True if the compilation is succesful</returns>
        public bool Compile(string commandName)
        {
            if (!File.Exists(sourcePath + "Cmd" + commandName + extension))
            {
                WriteErrors("File not found: Cmd" + commandName + extension);
                return false;
            }
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = false;
            parameters.ReferencedAssemblies.Add("MCForge_.dll");
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add("System.Xml.dll");
            parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");

            parameters.MainClass = commandName;
            parameters.OutputAssembly = dllPath + "Cmd" + commandName + ".dll";

            CompilerResults results;
            StringBuilder source = new StringBuilder(File.ReadAllText(sourcePath + "cmd" + commandName + extension));
            source.Replace("namespace MCLawl", "namespace MCForge");
            // Don't forget to dispose of the compiler.
            using (CodeDomProvider compiler = CodeDomProvider.CreateProvider(providerName))
            {
                results = compiler.CompileAssemblyFromSource(parameters, source.ToString());
            }
            if (results.Errors.Count == 0)
                return true;
            else
            {
                foreach (CompilerError error in results.Errors)
                {
                    WriteErrors("Error: " + error.ErrorNumber,
                                "Message: " + error.ErrorText,
                                "Line: " + error.Line);
                }
                return false;
            }
        }
        /// <summary>
        /// Loads a command
        /// </summary>
        /// <param name="command">Name of the command to load</param>
        /// <returns>A String with the error message that occured or nothing if it was succesful.</returns>
        public string Load(string command)
        {
            if (command.Length < 3 || command.Substring(0, 3).ToLower() != "cmd")
            {
                return "Invalid command name specified.";
            }
            try
            {
                Assembly asm = Assembly.LoadFrom("extra/commands/dll/" + command + ".dll");
                foreach (var type in asm.GetTypes())
                {
                    // interfaceType.IsAssignableFrom(implementingObject.GetType())
                    // Check to see if the implementing type is a Command
                    if (typeof(Command).IsAssignableFrom(type))
                    {
                        var instance = Activator.CreateInstance(type);
                        Command.all.Add((Command)instance);
                        try
                        {
                            ((Command)instance).Init();
                        }
                        catch (Exception ex)
                        {
                            Server.ErrorLog(ex);
                        }
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                Server.ErrorLog(e);
                return command + ".dll does not exist in the DLL folder, or is missing a dependency.  Details in the error log.";
            }
            catch (BadImageFormatException e)
            {
                Server.ErrorLog(e);
                return command + ".dll is not a valid assembly, or has an invalid dependency.  Details in the error log.";
            }
            catch (PathTooLongException)
            {
                return "Class name is too long.";
            }
            catch (FileLoadException e)
            {
                Server.ErrorLog(e);
                return command + ".dll or one of its dependencies could not be loaded.  Details in the error log.";
            }
            catch (InvalidCastException e)
            {
                //if the structure of the code is wrong, or it has syntax error or other code problems
                Server.ErrorLog(e);
                return command + ".dll has invalid code structure, please check code again for errors.";
            }
            catch (Exception e)
            {
                Server.ErrorLog(e);
                return "An unknown error occured and has been logged.";
            }
            return "";
        }

        /// <summary>
        /// Loads all commands provided in the name list
        /// </summary>
        /// <param name="commandList">List of commands to load</param>
        public void AutoLoad(string[] commandList)
        {
            foreach (string command in commandList)
            {
                string loadError = Load("Cmd" + command.ToLower());
                if (string.IsNullOrEmpty(loadError))
                {
                    Server.s.Log("AUTOLOAD: Loaded " + command.ToLower() + ".dll");
                }
                else
                {
                    Server.s.Log(loadError);
                    continue;
                }
            }
        }
        #endregion
    }
}
