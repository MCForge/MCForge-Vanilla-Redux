﻿/*
	Copyright © 2009-2014 MCSharp team (Modified for use with MCZall/MCLawl/MCForge/MCForge-Redux)
	
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
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection;
namespace Starter
{
    class Program
    {
        // Attempt to read the location from the DLL, if available.
        public static string DLLLocation
        {
            get
            {
                return "http://www.mcforge.net/MCForge_.dll";
            }
        }
        static int tries = 0;
        static bool needsToRestart = false;
        static string parent = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
        //Console.ReadLine() is ignored while Starter is set as Windows Application in properties. (At least on Windows)

        [STAThread]
        static void Main(string[] args)
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                if (File.Exists("Updater.exe"))
                    File.Delete("Updater.exe");
                if (File.Exists("MCForge_.dll.backup"))
                    File.Delete("MCForge_.dll.backup");
            }
            catch { }
            if (tries > 4)
            {
                Console.WriteLine("I'm afraid I can't download the file for some reason!");
                Console.WriteLine("Go to " + DLLLocation + " yourself and download it, please");
                Console.WriteLine("Place it inside my folder, near me, and restart me.");
                Console.WriteLine("If you have any issues, get the files from the www.mcforge.net download page and try again.");
                Console.WriteLine("Press any key to close me...");
                MessageBox.Show("Unable to download MCForge_.dll.  Please download it manually at " + DLLLocation + ", place it in the same folder as this executable, and restart this application", "Required DLL Missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.ReadLine();
                Console.WriteLine("Bye!");
            }
            else if (File.Exists("MCForge_.dll"))
            {
                string[] args1 = new string[] { "cli" };
                //Crash issue fixed by re-executing the exe to properly load MCForge_.dll.
                if (!needsToRestart)
                    openServer(args1);
                else
                    Process.Start(parent, "cli");
            }
            else
            {
                needsToRestart = true;
                tries++;
                Console.WriteLine("This is try number " + tries);
                Console.WriteLine("You do not have the required DLL!");
                Console.WriteLine("I'll download it for you. Just wait.");
                Console.WriteLine("Downloading from " + DLLLocation);
                try
                {
                    WebClient Client = new WebClient();
                    Client.DownloadFile(DLLLocation, "MCForge_.dll");
                    Client.Dispose();
                    Console.WriteLine("Finished downloading! Let's try this again, shall we.");
                    for (int i = 0; i < 5; i++)
                    {
                        Thread.Sleep(100);
                        Console.Write(".");
                    }
                    Console.WriteLine("Go!");
                    Console.WriteLine();
                    Main(args);
                }
                catch
                {
                    tries = 5;
                    Console.WriteLine("\nAn error occured while attempting to download MCForge_.dll\n");
                    Main(args);
                }
            }
        }
        static void openServer(string[] args)
        {
            MCForge_.Gui.Program.Main(args);
        }
    }
}
