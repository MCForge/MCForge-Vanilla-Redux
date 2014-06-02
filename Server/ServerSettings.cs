/*
	Copyright 2011 MCForge
	
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
using System.Net;

namespace MCForge {
	//derp idk just need to edit this so I can commit :/
	public static class ServerSettings {
        /// <summary>
        /// The url MCForge downloads additional URL's from
        /// </summary>
		public const string UrlsUrl = "http://server.mcforge.org/urls.txt";

		private static string _RevisionList = "http://www.update.mcforge.org/revs.txt";
		private static string _HeartbeatAnnounce = "http://server.mcforge.org/hbannounce.php";
		private static string _ArchivePath = "http://www.update.mcforge.org/archives/exe/";

		static ServerSettings() {
			using ( var client = new WebClient() ) {
				client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
				client.DownloadStringAsync(new Uri(UrlsUrl));
			}
		}

		static void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e) {
			if ( e.Cancelled || e.Error != null ) {
				Server.s.Log("Error getting urls. Using defaults.");
				return;
			}

			if ( e.Result.Split('@').Length != 3 ) {
				Server.s.Log("Recieved Malformed data from server...");
				return;
			}

			string[] lines = e.Result.Split('@');

			_RevisionList = lines[0];
			_HeartbeatAnnounce = lines[1];
			_ArchivePath = lines[2];
		}
        /// <summary>
        /// returns the MCForge archives url
        /// </summary>
		public static string ArchivePath {
			get {
				return _ArchivePath;
			}
		}
        /// <summary>
        /// returns the MCForge heartbeat announce URL
        /// </summary>
		public static string HeartbeatAnnounce {
			get {
				return _HeartbeatAnnounce;
			}
		}
        /// <summary>
        ///  returns the MCForge revision list URL
        /// </summary>
		public static string RevisionList {
			get {
				return _RevisionList;
			}
		}
	}
}
