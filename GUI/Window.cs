/*	
    Copyright � 2009-2014 MCSharp team (Modified for use with MCZall/MCLawl/MCForge/MCForge-Redux)
    
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
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace MCForge.Gui {
    public partial class Window : Form {
        // What is this???
        /*Regex regex = new Regex(@"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\." +
                                "([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$");*/
        
        // for cross thread use
        delegate void StringCallback(string s);
        delegate void PlayerListCallback(List<Player> players);
        delegate void ReportCallback(Report r);
        delegate void VoidDelegate();
        public static bool fileexists = false;
        bool mapgen = false;
        public static Window thisWindow;

        PlayerCollection pc = new PlayerCollection(new PlayerListView());
        LevelCollection lc = new LevelCollection(new LevelListView());
        LevelCollection lcTAB = new LevelCollection(new LevelListViewForTab());

        //public static event EventHandler Minimize;
        public NotifyIcon notifyIcon1 = new NotifyIcon();
        //  public static bool Minimized = false;

        Level prpertiesoflvl;
        Player prpertiesofplyer;

        internal static Server s;

        readonly System.Timers.Timer UpdateListTimer = new System.Timers.Timer(10000);

        public Window() {
            InitializeComponent();
        }

        private GroupBox grpMapEditor;
        private TextBox txtMapEditorLevelName;
        private TextBox txtMapEditorX;
        private TextBox txtMapEditorZ;
        private TextBox txtMapEditorY;
        private TextBox txtMapEditorCurrentBlock;
        private TextBox txtMapEditorChangeBlock;
        private Button btnMapEditorChange;
        private Button btnMapEditorUpdate;
        private GroupBox grpMapViewer;
        private PictureBox picMapViewer;
        private TextBox txtMapViewerLevelName;

        private void Window_Load(object sender, EventArgs e) {
            btnProperties.Enabled = false;
            //thisWindow = this;
            MaximizeBox = false;
            this.Text = "Starting MCForge...";
            this.Show();
            this.BringToFront();
            WindowState = FormWindowState.Normal;
            new Thread(() => {
                s = new Server();
                s.OnLog += WriteLine;
                s.OnCommand += newCommand;
                s.OnError += newError;
                s.OnSystem += newSystem;
                s.OnAdmin += WriteAdmin;
                s.OnOp += WriteOp;


                s.HeartBeatFail += HeartBeatFail;
                s.OnURLChange += UpdateUrl;
                s.OnPlayerListChange += UpdateClientList;
                s.OnSettingsUpdate += SettingsUpdate;
                s.Start();

                Player.PlayerConnect += Player_PlayerConnect;
                Player.PlayerDisconnect += Player_PlayerDisconnect;

                Level.LevelLoaded += Level_LevelLoaded;
                Level.LevelUnload += Level_LevelUnload;

                GlobalChatBot.OnNewRecieveGlobalMessage += GlobalChatRecieve;
                GlobalChatBot.OnNewSayGlobalMessage += GlobalChatSay;

                RunOnUiThread(() => btnProperties.Enabled = true);

            }).Start();


            notifyIcon1.Text = ( "MCForge Server" ).Truncate(64);

            this.notifyIcon1.ContextMenuStrip = this.iconContext;
            this.notifyIcon1.Icon = this.Icon;
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);

            if ( File.Exists("Changelog.txt") ) {
                txtChangelog.Text = "Changelog for " + Server.Version + ":";
                foreach ( string line in File.ReadAllLines(( "Changelog.txt" )) ) {
                    txtChangelog.AppendText("\r\n           " + line);
                }
            }

            // Map Viewer/Editor
            txtMapEditorLevelName.Text = Server.level;
            txtMapEditorX.Text = "0"; txtMapEditorY.Text = "0"; txtMapEditorZ.Text = "0";

            // Bind player list
            dgvPlayers.DataSource = pc;
            dgvPlayers.Font = new Font("Calibri", 8.25f);

            dgvMaps.DataSource = new LevelCollection(new LevelListView());
            dgvMaps.Font = new Font("Calibri", 8.25f);

            dgvMapsTab.DataSource = new LevelCollection(new LevelListViewForTab());
            dgvMapsTab.Font = new Font("Calibri", 8.25f);

            /*using (System.Timers.Timer UpdateListTimer = new System.Timers.Timer(10000))
            {
                UpdateListTimer.Elapsed += delegate
                {
                    UpdateClientList(Player.players);
                    UpdateMapList("'");
                    Server.s.Log("Lists updated!");
                }; UpdateListTimer.Start();
            }*/

            UpdateListTimer.Elapsed += delegate {
                try {
                    UpdateSettings();
                    UpdateClientList(Player.players);
                    UpdateMapList();
                }
                catch { } // needed for slower computers
                //Server.s.Log("Lists updated!");
            }; UpdateListTimer.Start();

        }

        public void RunOnUiThread(Action act) {
            var d = new VoidDelegate(() => Invoke(new VoidDelegate(act)));  //SOME ADVANCED STUFF RIGHT HERR
            d.Invoke();
        }
        void Player_PlayerConnect(Player p) {
            UpdatePlyersListBox();
        }
        void Player_PlayerDisconnect(Player p, string reason) {
            UpdatePlyersListBox();
        }
        void GlobalChatRecieve(string nick, string message) {
            this.LogGlobalChat("> " + nick + ": " + message);
        }
        void GlobalChatSay(string player, string message) {
            this.LogGlobalChat("< " + player + ": " + message);
        }
        void Level_LevelUnload(Level l) {
            RunOnUiThread(() => {
                UpdateMapList();
                UpdatePlayerMapCombo();
                UnloadedlistUpdate();

            });

        }
        void Level_LevelLoaded(Level l) {
            RunOnUiThread(() => {
                UpdatePlayerMapCombo();
                UnloadedlistUpdate();

            });
        }

        void SettingsUpdate() {
            if ( Server.shuttingDown ) return;
            if ( txtLog.InvokeRequired ) {
                this.Invoke(new VoidDelegate(SettingsUpdate));
            }
            else {
                this.Text = "MCForge " + Server.VersionString;
                notifyIcon1.Text = ("MCForge Server").Truncate(64);
            }
        }

        void HeartBeatFail() {
            WriteLine("Recent Heartbeat Failed");
        }

        void newError(string message) {
            try {
                if ( txtErrors.InvokeRequired ) {
                    this.Invoke(new LogDelegate(newError), new object[] { message });
                }
                else {
                    txtErrors.AppendText(Environment.NewLine + message);
                }
            }
            catch { }
        }
        void newSystem(string message) {
            try {
                if ( txtSystem.InvokeRequired ) {
                    this.Invoke(new LogDelegate(newSystem), new object[] { message });
                }
                else {
                    txtSystem.AppendText(Environment.NewLine + message);
                }
            }
            catch { }
        }

        delegate void LogDelegate(string message);

        /// <summary>
        /// Does the same as Console.WriteLine() only in the form
        /// </summary>
        /// <param name="s">The line to write</param>
        public void WriteLine(string s) {
            if ( Server.shuttingDown ) return;
            if ( this.InvokeRequired ) {
                this.Invoke(new LogDelegate(WriteLine), new object[] { s });
            }
            else {

                string cleaned = s;
                //Begin substring of crappy date stamp

                int substr = s.IndexOf(')');
                if ( substr == -1 ) {
                    cleaned = s;
                }
                else {
                    cleaned = s.Substring(substr + 1);
                }

                //end substring

                txtLog.AppendLog(cleaned + Environment.NewLine);
                // ColorBoxes(txtLog);
            }
        }


        public void WriteOp(string s) {
            if ( Server.shuttingDown ) return;
            if ( this.InvokeRequired ) {
                this.Invoke(new LogDelegate(WriteOp), new object[] { s });
            }
            else {
                //txtLog.AppendText(Environment.NewLine + s);
                txtOpLog.AppendTextAndScroll(s);
            }
        }

        public void WriteAdmin(string s) {
            if ( Server.shuttingDown ) return;
            if ( this.InvokeRequired ) {
                this.Invoke(new LogDelegate(WriteAdmin), new object[] { s });
            }
            else {
                //txtLog.AppendText(Environment.NewLine + s);
                txtAdminLog.AppendTextAndScroll(s);
            }
        }

        /// <summary>
        /// Updates the list of client names in the window
        /// </summary>
        /// <param name="players">The list of players to add</param>
        public void UpdateClientList(List<Player> players) {

            if ( InvokeRequired ) {
                Invoke(new PlayerListCallback(UpdateClientList), players);
            }
            else {

                if ( dgvPlayers.DataSource == null )
                    dgvPlayers.DataSource = pc;

                // Try to keep the same selection on update
                string selected = null;
                if ( pc.Count > 0 && dgvPlayers.SelectedRows.Count > 0 ) {
                    selected = ( from DataGridViewRow row in dgvPlayers.Rows where row.Selected select pc[row.Index] ).First().name;
                }

                // Update the data source and control
                //dgvPlayers.SuspendLayout();

                pc = new PlayerCollection(new PlayerListView());
                Player.players.ForEach(p => pc.Add(p));

                //dgvPlayers.Invalidate();
                dgvPlayers.DataSource = pc;
                // Reselect player
                if ( selected != null ) {
                    foreach ( Player t in Player.players )
                        for ( int j = 0; j < dgvPlayers.Rows.Count; j++ )
                            if ( Equals(dgvPlayers.Rows[j].Cells[0].Value, selected) )
                                dgvPlayers.Rows[j].Selected = true;
                }

                dgvPlayers.Refresh();
                //dgvPlayers.ResumeLayout();
            }

        }

        public void PopupNotify(string message, ToolTipIcon icon = ToolTipIcon.Info) {
            notifyIcon1.ShowBalloonTip(3000, Server.name, message, icon);
        }

        public delegate void UpdateList();

        public void UpdateSettings() {
            if (InvokeRequired) {
                Invoke(new UpdateList(UpdateSettings));
            } else {
                txtServerrName.Text = Server.name;
            }
        }

        public void UpdateMapList() {
            if ( InvokeRequired )
                Invoke(new UpdateList(UpdateMapList));
            else {

                if ( dgvMaps.DataSource == null )
                    dgvMaps.DataSource = lc;

                // Try to keep the same selection on update
                string selected = null;
                if ( lc.Count > 0 && dgvMaps.SelectedRows.Count > 0 ) {
                    selected = ( from DataGridViewRow row in dgvMaps.Rows where row.Selected select lc[row.Index] ).First().name;
                }

                // Update the data source and control
                //dgvPlayers.SuspendLayout();
                lc.Clear();
                //lc = new LevelCollection(new LevelListView());
                Server.levels.ForEach(l => lc.Add(l));

                //dgvPlayers.Invalidate();
                dgvMaps.DataSource = null;
                dgvMaps.DataSource = lc;
                // Reselect map
                if ( selected != null ) {
                    foreach ( DataGridViewRow row in Server.levels.SelectMany(l => dgvMaps.Rows.Cast<DataGridViewRow>().Where(row => (string)row.Cells[0].Value == selected)) )
                        row.Selected = true;
                }

                dgvMaps.Refresh();
                //dgvPlayers.ResumeLayout();

                if ( dgvMapsTab.DataSource == null )
                    dgvMapsTab.DataSource = lcTAB;

                // Try to keep the same selection on update
                string selected2 = null;
                if ( lcTAB.Count > 0 && dgvMapsTab.SelectedRows.Count > 0 ) {
                    selected2 = ( from DataGridViewRow row in dgvMapsTab.Rows where row.Selected select lcTAB[row.Index] ).First().name;
                }

                // Update the data source and control
                //dgvPlayers.SuspendLayout();
                lcTAB.Clear();
                //lcTAB = new LevelCollection(new LevelListViewForTab());
                Server.levels.ForEach(l => lcTAB.Add(l));

                //dgvPlayers.Invalidate();
                dgvMapsTab.DataSource = null;
                dgvMapsTab.DataSource = lcTAB;
                // Reselect map
                if ( selected2 != null ) {
                    foreach ( DataGridViewRow row in from l in Server.levels from DataGridViewRow row in dgvMapsTab.Rows where Equals(row.Cells[0].Value, selected2) select row )
                        row.Selected = true;
                }

                dgvMapsTab.Refresh();
            }
        }

        /// <summary>
        /// Places the server's URL at the top of the window
        /// </summary>
        /// <param name="s">The URL to display</param>
        public void UpdateUrl(string s) {
            if ( this.InvokeRequired ) {
                StringCallback d = UpdateUrl;
                this.Invoke(d, new object[] { s });
            }
            else
                txtUrl.Text = s;
        }

        private void Window_FormClosing(object sender, FormClosingEventArgs e) {
            if (e.CloseReason == CloseReason.WindowsShutDown) {
                MCForge_.Gui.Program.ExitProgram(false);
            }
            if (Server.shuttingDown || MessageBox.Show("Really Shutdown the Server? All Connections will break!", "Exit", MessageBoxButtons.OKCancel) == DialogResult.OK) {
                if (!Server.shuttingDown) {
                    MCForge_.Gui.Program.ExitProgram(false);
                }
            }
            else {
                // Prevents form from closing when user clicks the X and then hits 'cancel'
                e.Cancel = true;
            }
        }

        private void txtInput_KeyDown(object sender, KeyEventArgs e) {
            if ( e.KeyCode == Keys.Enter ) {
                string text = txtInput.Text.Trim();
                if ( String.IsNullOrEmpty(text) ) return;
                switch ( text[0] ) {
                    case '#':
                        text = text.Remove(0, 1);
                        Player.GlobalMessageOps(text);
                        Server.s.Log("(OPs): Console: " + text, false, LogType.Op);
                        Server.IRC.Say("Console: " + text, true);
                        break;
                    case '+':
                        text = text.Remove(0, 1);
                        Player.GlobalMessageAdmins(text);
                        Server.s.Log("(Admins): Console: " + text, false, LogType.Admin);
                        Server.IRC.Say("Console: " + text, true);
                        break;
                    default:
                        Player.GlobalMessage("Console [&a" + Server.ZallState + Server.DefaultColor + "]:&f " + text);
                        Server.IRC.Say("Console [" + Server.ZallState + "]: " + text);
                        WriteLine("<CONSOLE> " + text);
                        break;
                }
                txtInput.Clear();
            }
        }

        private void txtCommands_KeyDown(object sender, KeyEventArgs e) {
            if ( e.KeyCode != Keys.Enter )
                return;
            string sentCmd, sentMsg = "";

            if ( txtCommands.Text == null || txtCommands.Text.Trim() == "" ) {
                newCommand("CONSOLE: Whitespace commands are not allowed.");
                txtCommands.Clear();
                return;
            }

            if ( txtCommands.Text[0] == '/' && txtCommands.Text.Length > 1 )
                txtCommands.Text = txtCommands.Text.Substring(1);

            if ( txtCommands.Text.IndexOf(' ') != -1 ) {
                sentCmd = txtCommands.Text.Split(' ')[0];
                sentMsg = txtCommands.Text.Substring(txtCommands.Text.IndexOf(' ') + 1);
            }
            else if ( txtCommands.Text != "" ) {
                sentCmd = txtCommands.Text;
            }
            else {
                return;
            }

            new Thread(() => {
                try {
                    Command commandcmd = Command.all.Find(sentCmd);
                    if ( commandcmd == null ) {
                        Server.s.Log("No such command!");
                        return;
                    }
                    if (!Player.CommandProtected(sentCmd, sentMsg)) {
                        commandcmd.Use(null, sentMsg);
                    } else { Server.s.Log("Cannot use command, player has protection level: " + Server.forgeProtection); };
                    newCommand("CONSOLE: USED /" + sentCmd + " " + sentMsg);

                }
                catch ( Exception ex ) {
                    Server.ErrorLog(ex);
                    newCommand("CONSOLE: Failed command.");
                }
            }).Start();

            txtCommands.Clear();
        }

        private void btnClose_Click_1(object sender, EventArgs e) {
            Close();
        }

        public void newCommand(string p) {
            if ( txtCommandsUsed.InvokeRequired ) {
                LogDelegate d = newCommand;
                this.Invoke(d, new object[] { p });
            }
            else {
                txtCommandsUsed.AppendTextAndScroll(p);
            }
        }

        private void btnProperties_Click_1(object sender, EventArgs e) {
            if ( !prevLoaded ) { PropertyForm = new PropertyWindow(); prevLoaded = true; }
            PropertyForm.Show();
            if (!PropertyForm.Focused) {
                PropertyForm.Focus();
            }
        }

        public static bool prevLoaded = false;
        Form PropertyForm;

        private void Window_Resize(object sender, EventArgs e) {
            this.ShowInTaskbar = ( this.WindowState != FormWindowState.Minimized );
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e) {
            this.Show();
            this.BringToFront();
            WindowState = FormWindowState.Normal;
        }

        private void openConsole_Click(object sender, EventArgs e) {
            this.Show();
            this.BringToFront();
            WindowState = FormWindowState.Normal;
        }

        private void shutdownServer_Click(object sender, EventArgs e) {
            Close();
        }

        public void WriteAdminLine(string s) { txtAdminLog.AppendText("\r\n" + s); }
        public void WriteOpLine(string s) { txtOpLog.AppendText("\r\n" + s); }

        private Player GetSelectedPlayer() {

            if ( this.dgvPlayers.SelectedRows.Count <= 0 )
                return null;

            return (Player)( this.dgvPlayers.SelectedRows[0].DataBoundItem );
        }

        private Level GetSelectedLevel() {

            if ( this.dgvMaps.SelectedRows.Count <= 0 )
                return null;

            return (Level)( this.dgvMaps.SelectedRows[0].DataBoundItem );
        }

        private Level GetSelectedLevelTab() {

            if ( dgvMapsTab.SelectedRows.Count == 0 )
                return null;

            return (Level)( dgvMapsTab.SelectedRows[0].DataBoundItem );
        }

        private void clonesToolStripMenuItem_Click(object sender, EventArgs e) {
            playerselect("clones");
        }

        private void voiceToolStripMenuItem_Click(object sender, EventArgs e) {
            playerselect("voice");
        }

        private void whoisToolStripMenuItem_Click(object sender, EventArgs e) {
            playerselect("whois");
        }

        private void kickToolStripMenuItem_Click(object sender, EventArgs e) {
            playerselect("kick", " You have been kicked by the console.");
        }


        private void banToolStripMenuItem_Click(object sender, EventArgs e) {
            playerselect("ban");
        }

        private void playerselect(string com) {
            if ( GetSelectedPlayer() != null )
                Command.all.Find(com).Use(null, GetSelectedPlayer().name);
        }
        private void playerselect(string com, string args) {
            if ( GetSelectedPlayer() != null )
                Command.all.Find(com).Use(null, GetSelectedPlayer().name + args);
        }

        private void finiteModeToolStripMenuItem_Click(object sender, EventArgs e) {
            levelcommand("map", " finite");
        }

        private void animalAIToolStripMenuItem_Click(object sender, EventArgs e) {
            levelcommand("map", " ai");
        }

        private void edgeWaterToolStripMenuItem_Click(object sender, EventArgs e) {
            levelcommand("map", " edge");
        }

        private void growingGrassToolStripMenuItem_Click(object sender, EventArgs e) {
            levelcommand("map", " grass");
        }

        private void survivalDeathToolStripMenuItem_Click(object sender, EventArgs e) {
            levelcommand("map", " death");
        }

        private void killerBlocksToolStripMenuItem_Click(object sender, EventArgs e) {
            levelcommand("map", " killer");
        }

        private void rPChatToolStripMenuItem_Click(object sender, EventArgs e) {
            levelcommand("map", " chat");
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            levelcommand("save");
        }

        private void levelcommand(string com) {
            if ( GetSelectedLevel() != null )
                Command.all.Find(com).Use(null, GetSelectedLevel().name);
        }

        private void levelcommand(string com, string args) {
            if ( GetSelectedLevel() != null )
                Command.all.Find(com).Use(null, GetSelectedLevel().name + args);
        }

        private void tabControl1_Click(object sender, EventArgs e) {
            try { UnloadedlistUpdate(); }
            catch { }
            try { UpdatePlyersListBox(); }
            catch { }
            try {
                if ( LogsTxtBox.Text == "" ) {
                    dateTimePicker1.Value = DateTime.Now;
                }
            }
            catch { }
            foreach ( TextBox txtBox in ( from TabPage tP in tcMain.TabPages from Control ctrl in tP.Controls select ctrl ).OfType<TextBox>() ) {
                txtBox.Update();
            }
            tcMain.Update();
        }

        private void Restart_Click(object sender, EventArgs e) {
            if ( MessageBox.Show("Are you sure you want to restart?", "Restart", MessageBoxButtons.OKCancel) == DialogResult.OK ) {
                MCForge_.Gui.Program.ExitProgram(true);
            }

        }

        private void restartServerToolStripMenuItem_Click(object sender, EventArgs e) {
            Restart_Click(sender, e);
        }

        private void DatePicker1_ValueChanged(object sender, EventArgs e) {
            string dayofmonth = dateTimePicker1.Value.Day.ToString().PadLeft(2, '0');
            string year = dateTimePicker1.Value.Year.ToString();
            string month = dateTimePicker1.Value.Month.ToString().PadLeft(2, '0');

            string ymd = year + "-" + month + "-" + dayofmonth;
            string filename = ymd + ".txt";

            if ( !File.Exists(Path.Combine("logs/", filename)) ) {
                //MessageBox.Show("Sorry, the log for " + ymd + " doesn't exist, please select another one");
                LogsTxtBox.Text = "No logs found for: " + ymd;
            }
            else {
                LogsTxtBox.Text = null;
                LogsTxtBox.Text = File.ReadAllText(Path.Combine("logs/", filename));
            }

        }

        private void txtUrl_DoubleClick(object sender, EventArgs e) {
            txtUrl.SelectAll();
        }

        private void dgvPlayers_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e) {
            e.PaintParts &= ~DataGridViewPaintParts.Focus;
        }

        private void promoteToolStripMenuItem_Click(object sender, EventArgs e) {
            playerselect("promote");
        }

        private void demoteToolStripMenuItem_Click(object sender, EventArgs e) {
            playerselect("demote");
        }

        #region Tabs
        #region mapsTab
        private void editPropertiesToolStripMenuItem_Click(object sender, EventArgs e) //Ok actually i deleted this.......................... but it's still needed for the other stuff (like cliking on the cell etc.)
        {
            Level l = GetSelectedLevelTab();
            if ( l != null ) {
                prpertiesoflvl = l;
                MOTDtxt.Text = l.motd;
                physlvlnumeric.Value = l.physics;
                grasschk.Checked = l.GrassGrow;
                chatlvlchk.Checked = l.worldChat;
                Killerbloxchk.Checked = l.Killer;
                SurvivalStyleDeathchk.Checked = l.Death;
                finitechk.Checked = l.finite;
                edgewaterchk.Checked = l.edgeWater;
                if ( Server.UseTextures )
                    WoM.Enabled = true;
                Aicombo.SelectedItem = l.ai ? "Hunt" : "Flee";
                Gunschk.Checked = l.guns;
                Fallnumeric.Value = l.fall;
                drownNumeric.Value = l.drown;
                LoadOnGotoChk.Checked = l.loadOnGoto;
                UnloadChk.Checked = l.unload;
                chkRndFlow.Checked = l.randomFlow;
                leafDecayChk.Checked = l.leafDecay;
                TreeGrowChk.Checked = l.growTrees;
                AutoLoadChk.Checked = false;
                if ( File.Exists("text/autoload.txt") ) {
                    using ( StreamReader r = new StreamReader("text/autoload.txt") ) {
                        string line;
                        while ( ( line = r.ReadLine() ) != null ) {
                            if ( line.Contains(l.name) || line.Contains(l.name.ToLower()) ) {
                                AutoLoadChk.Checked = true;
                            }
                        }
                    }
                }
            }
            else
                WoM.Enabled = false;
            UpdateMapList();
        }

        private void SaveMap_Click(object sender, EventArgs e) {
            if ( prpertiesoflvl == null ) return;
            Level l = prpertiesoflvl;
            l.motd = MOTDtxt.Text;
            if ( MOTDtxt.Text == "" ) {
                l.motd = "ignore";
            }
            l.physics = (int)physlvlnumeric.Value;
            l.GrassGrow = grasschk.Checked;
            l.worldChat = chatlvlchk.Checked;
            l.Killer = Killerbloxchk.Checked;
            l.Death = SurvivalStyleDeathchk.Checked;
            l.finite = finitechk.Checked;
            l.edgeWater = edgewaterchk.Checked;
            switch ( Aicombo.SelectedItem.ToString() ) {
                case "Hunt":
                    l.ai = true;
                    break;
                case "Flee":
                    l.ai = false;
                    break;
            }
            l.guns = Gunschk.Checked;
            l.fall = (int)Fallnumeric.Value;
            l.drown = (int)drownNumeric.Value;
            l.loadOnGoto = LoadOnGotoChk.Checked;
            l.unload = UnloadChk.Checked;
            l.randomFlow = chkRndFlow.Checked;
            l.leafDecay = leafDecayChk.Checked;
            l.growTrees = TreeGrowChk.Checked;
            {
                List<string> oldlines = new List<string>();
                if ( !File.Exists("text/autoload.txt") )
                    using ( var nulled = File.CreateText("text/autoload.txt") ) { }

                using ( StreamReader r = new StreamReader("text/autoload.txt") ) {
                    bool done = false;
                    string line;
                    while ( ( line = r.ReadLine() ) != null ) {
                        if ( line.ToLower().Contains(l.name.ToLower()) ) {
                            if ( AutoLoadChk.Checked == false ) {
                                line = "";
                            }
                            done = true;
                        }
                        oldlines.Add(line);
                    }
                    if ( AutoLoadChk.Checked && !done ) {
                        oldlines.Add(l.name + "=" + l.physics);
                    }
                }
                File.Delete("text/autoload.txt");
                using ( StreamWriter SW = File.CreateText("text/autoload.txt") ) {
                    foreach ( string line in oldlines.Where(line => line.Trim() != "") ) {
                        SW.WriteLine(line);
                    }
                }
            }
            UpdateMapList();
        }

        private void CreateNewMap_Click(object sender, EventArgs e) {
            if ( mapgen ) { MessageBox.Show("Map generator already in use."); return; }

            string name;
            string x;
            string y;
            string z;
            string type;
            string seed;

            try { name = nametxtbox.Text.ToLower(); }
            catch { name = ""; }
            try { x = xtxtbox.SelectedItem.ToString(); }
            catch { x = ""; }
            try { y = ytxtbox.SelectedItem.ToString(); }
            catch { y = ""; }
            try { z = ztxtbox.SelectedItem.ToString(); }
            catch { z = ""; }
            try { type = maptypecombo.SelectedItem.ToString().ToLower(); }
            catch { type = ""; }
            try { seed = seedtxtbox.Text; }
            catch { seed = ""; }

            if ( String.IsNullOrEmpty(name) || String.IsNullOrEmpty(x) || String.IsNullOrEmpty(y) || String.IsNullOrEmpty(z) || String.IsNullOrEmpty(type) ) {
                MessageBox.Show("You left a box blank!");
                return;
            }

            new Thread(() => {
                mapgen = true;
                try {
                    Command.all.Find("newlvl").Use(null, name + " " + x + " " + y + " " + z + " " + type + ( !String.IsNullOrEmpty(seed) ? " " + seed : "" ));
                }
                catch {
                    MessageBox.Show("Level Creation Failed. Are  you sure you didn't leave a box empty?");
                }

                if ( File.Exists("levels/" + nametxtbox.Text + ".mcf") ) {
                    MessageBox.Show("Created Level");
                    try {
                        UnloadedlistUpdate();
                        UpdateMapList();
                    }
                    catch { }
                }
                else {
                    MessageBox.Show("Level may not have been created.");
                }
                mapgen = false;
            }).Start(); ;
        }

        private void btnMapEditorChange_Click(object sender, EventArgs e)
        {
            MapEditorChangeBlock();
        }

        public void MapEditorChangeBlock()
        {
            try
            {
                Level l = Level.Find(txtMapEditorLevelName.Text);
                if (l == null) { l = Level.Load(txtMapEditorLevelName.Text); }
                if (l == null) { MessageBox.Show("Level " + txtMapEditorLevelName.Text + " could not be found.", "Map Editor"); return; }
                ushort x, y, z;
                if (!ushort.TryParse(txtMapEditorX.Text, out x) || (x >= l.width || x < 0)) { txtMapEditorX.Text = "0"; MessageBox.Show("Invalid x value.", "Map Editor"); return; }
                if (!ushort.TryParse(txtMapEditorY.Text, out y) || (y >= l.height || y < 0)) { txtMapEditorY.Text = "0"; MessageBox.Show("Invalid y value.", "Map Editor"); return; }
                if (!ushort.TryParse(txtMapEditorZ.Text, out z) || (z >= l.depth || z < 0)) { txtMapEditorZ.Text = "0"; MessageBox.Show("Invalid z value.", "Map Editor"); return; }
                txtMapEditorCurrentBlock.Text = Block.Name(l.GetTile(x, y, z));

                if (String.IsNullOrEmpty(txtMapEditorChangeBlock.Text)) { MessageBox.Show("Enter a block to change the selected block to."); return; }
                ushort b = Block.Ushort(txtMapEditorChangeBlock.Text);
                if (Block.Name(b).ToLower() == "unknown") { MessageBox.Show("Block could not be found.", "Map Editor"); return; }
                l.SetTile(x, y, z, b); Player.GlobalBlockchange(l, x, y, z, b);
                txtMapEditorCurrentBlock.Text = Block.Name(l.GetTile(x, y, z));
                MapViewerUpdateLevel();
            }
            catch (Exception ex) { Server.ErrorLog(ex); return; }
        }

        public void MapEditorUpdateBlock()
        {
            try
            {
                Level l = Level.Find(txtMapEditorLevelName.Text);
                if (l == null) { l = Level.Load(txtMapEditorLevelName.Text); }
                if (l == null) { MessageBox.Show("Level " + txtMapEditorLevelName.Text + " could not be found.", "Map Editor"); return; }
                ushort x, y, z;
                if (!ushort.TryParse(txtMapEditorX.Text, out x) || (x >= l.width || x < 0)) { txtMapEditorX.Text = "0"; MessageBox.Show("Invalid x value.", "Map Editor"); return; }
                if (!ushort.TryParse(txtMapEditorY.Text, out y) || (y >= l.depth || y < 0)) { txtMapEditorY.Text = "0"; MessageBox.Show("Invalid y value.", "Map Editor"); return; }
                if (!ushort.TryParse(txtMapEditorZ.Text, out z) || (z >= l.height || z < 0)) { txtMapEditorZ.Text = "0"; MessageBox.Show("Invalid z value.", "Map Editor"); return; }
                txtMapEditorCurrentBlock.Text = Block.Name(l.GetTile(x, y, z));
                MapViewerUpdateLevel();
            }
            catch (Exception ex) { Server.ErrorLog(ex); return; }
        }

        private void btnMapEditorUpdate_Click(object sender, EventArgs e)
        {
            MapEditorUpdateBlock();
        }

        private void btnMapViewerUpdate_Click(object sender, EventArgs e)
        {
            MapViewerUpdateLevel();
        }

        private TextBox txtMapViewerZ;
        private TextBox txtMapViewerY;
        private TextBox txtMapViewerX;
        private NumericUpDown txtMapViewerRotation;

        public void MapViewerUpdateLevel()
        {
            try
            {
                Level l = Level.Find(txtMapViewerLevelName.Text);
                if (l == null)
                {
                    Command.all.Find("load").Use(null, txtMapViewerLevelName.Text);
                    l = Level.Find(txtMapViewerLevelName.Text);
                }
                if (l == null) { MessageBox.Show("Level could not be found.", "Map Viewer"); return; }
                txtMapViewerX.Text = l.width.ToString();
                txtMapViewerY.Text = l.depth.ToString();
                txtMapViewerZ.Text = l.height.ToString();
                int rotation = (int)txtMapViewerRotation.Value;
                if (rotation < 0 || rotation > 3) { MessageBox.Show("Invalid rotation (must be from 0-3).", "Map Viewer"); return; }
                IsoCat IsoCat = new IsoCat(l, IsoCatMode.Normal, rotation);
                Rectangle r = new Rectangle(0, 0, picMapViewer.Width, picMapViewer.Height);
                System.ComponentModel.BackgroundWorker bgw = new System.ComponentModel.BackgroundWorker();
                bgw.WorkerReportsProgress = true;
                MapViewerLastDrawn = IsoCat.Draw(out r, bgw);
                picMapViewer.Image = MapViewerLastDrawn;
            }
            catch (Exception ex) { Server.ErrorLog(ex); }
        }
        public Bitmap MapViewerLastDrawn;

        private void btnMapViewerSave_Click(object sender, EventArgs e)
        {
            try
            {
                MapViewerUpdateLevel();
                if (!Directory.Exists("extra/levelimages")) Directory.CreateDirectory("extra/levelimages");
                int lastExisting = 0;
                string saveLocation = "extra/levelimages/" + txtMapViewerLevelName.Text + " (" + DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss") + ")";
                while (File.Exists(saveLocation)) { saveLocation = "extra/levelimages/" + txtMapViewerLevelName.Text + " (" + DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss") + ") (" + lastExisting + ")"; lastExisting++; }
                saveLocation += ".png";
                if (MapViewerLastDrawn != null) MapViewerLastDrawn.Save(saveLocation, ImageFormat.Png);
                if (MessageBox.Show("Saved to " + saveLocation + "." + Environment.NewLine + "Would you like to display the image now?", "Map Viewer", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    Process.Start(Application.StartupPath + "/" + saveLocation);
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        private void ldmapbt_Click(object sender, EventArgs e) {
            try {
                Command.all.Find("load").Use(null, UnloadedList.SelectedItem.ToString());
            }
            catch { }
        }

        private void dgvMapsTab_CellClick(object sender, DataGridViewCellEventArgs e) {
            saveToolStripMenuItem_Click(sender, e);
            editPropertiesToolStripMenuItem_Click(sender, e);
        }

        public void UnloadedlistUpdate() {
            RunOnUiThread(() => {
                UnloadedList.Items.Clear();

                string name;
                FileInfo[] fi = new DirectoryInfo("levels/").GetFiles("*.mcf");
                foreach ( FileInfo file in fi ) {
                    name = file.Name.Replace(".mcf", "");
                    if ( Level.Find(name.ToLower()) == null )
                        UnloadedList.Items.Add(name);
                }
            });
        }
        #endregion
        #region playersTab
        private void LoadPLayerTabDetails(object sender, EventArgs e) {
            Player p = Player.Find(PlyersListBox.Text);
            if ( p != null ) {
                PlayersTextBox.AppendTextAndScroll("==" + p.name + "==");
                { //Top Stuff
                    prpertiesofplyer = p;
                    NameTxtPlayersTab.Text = p.name;
                    MapTxt.Text = p.level.name;
                    RankTxt.Text = p.group.name;
                    StatusTxt.Text = Player.CheckPlayerStatus(p);
                    IPtxt.Text = p.ip;
                    DeathsTxt.Text = p.deathCount.ToString();
                    Blockstxt.Text = p.overallBlocks.ToString();
                    TimesLoggedInTxt.Text = p.totalLogins.ToString();
                    LoggedinForTxt.Text = Convert.ToDateTime(DateTime.Now.Subtract(p.timeLogged).ToString()).ToString("HH:mm:ss");
                    Kickstxt.Text = p.totalKicked.ToString();
                }
                { //Check buttons
                    if ( p.joker ) { JokerBt.Text = "UnJoker"; } else { JokerBt.Text = "Joker"; }
                    if ( p.frozen ) { FreezeBt.Text = "UnFreeze"; } else { FreezeBt.Text = "Freeze"; }
                    if ( p.muted ) { MuteBt.Text = "UnMute"; } else { MuteBt.Text = "Mute"; }
                    if ( p.voice ) { VoiceBt.Text = "UnVoice"; } else { VoiceBt.Text = "Voice"; }
                    if ( p.hidden ) { HideBt.Text = "UnHide"; } else { HideBt.Text = "Hide"; }
                    if ( p.jailed ) { JailBt.Text = "UnJail"; } else { JailBt.Text = "Jail"; }
                }
                { //Text box stuff
                    //Login
                    {
                        if ( File.Exists("text/login/" + p.name + ".txt") ) {
                            LoginTxt.Text = null;
                            LoginTxt.Text = File.ReadAllText("text/login/" + p.name + ".txt");
                        }
                        else {
                            LoginTxt.Text = null;
                        }
                    }
                    //Logout
                    {
                        if ( File.Exists("text/logout/" + p.name + ".txt") ) {
                            LogoutTxt.Text = null;
                            LogoutTxt.Text = File.ReadAllText("text/logout/" + p.name + ".txt");
                        }
                        else {
                            LogoutTxt.Text = null;
                        }
                    }
                    //Title
                    {
                        if ( p.title != null ) {
                            TitleTxt.Text = p.title;
                        }
                        else {
                            TitleTxt.Text = null;
                        }
                    }
                    //Color
                    {
                        ColorCombo.SelectedText = "";
                        switch ( p.color ) {
                            case "&0":
                                ColorCombo.Text = "Black";
                                break;
                            case "&1":
                                ColorCombo.Text = "Navy";
                                break;
                            case "&2":
                                ColorCombo.Text = "Green";
                                break;
                            case "&3":
                                ColorCombo.Text = "Teal";
                                break;
                            case "&4":
                                ColorCombo.Text = "Maroon";
                                break;
                            case "&5":
                                ColorCombo.Text = "Purple";
                                break;
                            case "&6":
                                ColorCombo.Text = "Gold";
                                break;
                            case "&7":
                                ColorCombo.Text = "Silver";
                                break;
                            case "&8":
                                ColorCombo.Text = "Gray";
                                break;
                            case "&9":
                                ColorCombo.Text = "Blue";
                                break;
                            case "&a":
                                ColorCombo.Text = "Lime";
                                break;
                            case "&b":
                                ColorCombo.Text = "Aqua";
                                break;
                            case "&c":
                                ColorCombo.Text = "Red";
                                break;
                            case "&d":
                                ColorCombo.Text = "Pink";
                                break;
                            case "&e":
                                ColorCombo.Text = "Yellow";
                                break;
                            case "&f":
                                ColorCombo.Text = "White";
                                break;
                            default:
                                ColorCombo.Text = "";
                                break;
                        }
                    }
                    //Map
                    {
                        try {
                            try {
                                UpdatePlayerMapCombo();
                            }
                            catch { }
                            foreach ( Object obj in MapCombo.Items ) {
                                if ( Level.Find(obj.ToString()) != null ) {
                                    if ( p.level == Level.Find(obj.ToString()) ) {
                                        MapCombo.SelectedItem = obj;
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }

            }
        }

        public void UpdatePlayerMapCombo() {
            int selected = MapCombo.SelectedIndex;
            MapCombo.Items.Clear();
            foreach ( Level level in Server.levels ) {
                MapCombo.Items.Add(level.name);
            }
            MapCombo.SelectedIndex = selected;
        }

        private void LoginBt_Click(object sender, EventArgs e) {
            if ( prpertiesofplyer == null || !Player.players.Contains(prpertiesofplyer) ) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            File.WriteAllText("text/login/" + prpertiesofplyer.name + ".txt", null);
            File.WriteAllText("text/login/" + prpertiesofplyer.name + ".txt", LoginTxt.Text);
            PlayersTextBox.AppendTextAndScroll("The login message has been saved!");
        }

        private void LogoutBt_Click(object sender, EventArgs e) {
            if ( prpertiesofplyer == null || !Player.players.Contains(prpertiesofplyer) ) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            File.WriteAllText("text/logout/" + prpertiesofplyer.name + ".txt", null);
            File.WriteAllText("text/logout/" + prpertiesofplyer.name + ".txt", LogoutTxt.Text);
            PlayersTextBox.AppendTextAndScroll("The logout message has been saved!");
        }

        private void TitleBt_Click(object sender, EventArgs e) {
            if ( prpertiesofplyer == null || !Player.players.Contains(prpertiesofplyer) ) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            if ( TitleTxt.Text.Length > 17 ) { PlayersTextBox.AppendTextAndScroll("Title must be under 17 letters."); return; }
            prpertiesofplyer.prefix = "[" + TitleTxt.Text + "]";
            PlayersTextBox.AppendTextAndScroll("The title has been saved");
        }

        private void ColorBt_Click(object sender, EventArgs e) {
            if ( prpertiesofplyer == null || !Player.players.Contains(prpertiesofplyer) ) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            switch ( ColorCombo.Text ) {
                case "Black":
                    prpertiesofplyer.color = "&0";
                    break;
                case "Navy":
                    prpertiesofplyer.color = "&1";
                    break;
                case "Green":
                    prpertiesofplyer.color = "&2";
                    break;
                case "Teal":
                    prpertiesofplyer.color = "&3";
                    break;
                case "Maroon":
                    prpertiesofplyer.color = "&4";
                    break;
                case "Purple":
                    prpertiesofplyer.color = "&5";
                    break;
                case "Gold":
                    prpertiesofplyer.color = "&6";
                    break;
                case "Silver":
                    prpertiesofplyer.color = "&7";
                    break;
                case "Gray":
                    prpertiesofplyer.color = "&8";
                    break;
                case "Blue":
                    prpertiesofplyer.color = "&9";
                    break;
                case "Lime":
                    prpertiesofplyer.color = "&a";
                    break;
                case "Aqua":
                    prpertiesofplyer.color = "&b";
                    break;
                case "Red":
                    prpertiesofplyer.color = "&c";
                    break;
                case "Pink":
                    prpertiesofplyer.color = "&d";
                    break;
                case "Yellow":
                    prpertiesofplyer.color = "&e";
                    break;
                case "White":
                    prpertiesofplyer.color = "&f";
                    break;
                default:
                    prpertiesofplyer.color = "";
                    break;
            }
            PlayersTextBox.AppendTextAndScroll("Set color to " + ColorCombo.Text);
        }

        private void MapBt_Click(object sender, EventArgs e) {
            if ( prpertiesofplyer == null || !Player.players.Contains(prpertiesofplyer) ) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            if ( MapCombo.Text.ToLower() == prpertiesofplyer.level.name.ToLower() ) {
                PlayersTextBox.AppendTextAndScroll("The player is already on that map");
                return;
            }
            if ( !Server.levels.Contains(Level.Find(MapCombo.Text)) ) {
                PlayersTextBox.AppendTextAndScroll("That map doesn't exist!!");
                return;
            }
            else {
                try {
                    Command.all.Find("goto").Use(prpertiesofplyer, MapCombo.Text);
                    PlayersTextBox.AppendTextAndScroll("Sent player to " + MapCombo.Text);
                }
                catch {
                    PlayersTextBox.AppendTextAndScroll("Something went wrong!!");
                    return;
                }
            }
        }

        private void UndoBt_Click(object sender, EventArgs e) {
            if ( prpertiesofplyer == null || !Player.players.Contains(prpertiesofplyer) ) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            if ( UndoTxt.Text.Trim() == "" ) {
                PlayersTextBox.AppendTextAndScroll("You didn't specify a time");
                return;
            }
            else {
                try {
                    Command.core.Find("undo").Use(null, prpertiesofplyer.name + " " + UndoTxt.Text);
                    PlayersTextBox.AppendTextAndScroll("Undid player for " + UndoTxt.Text + " Seconds");
                }
                catch {
                    PlayersTextBox.AppendTextAndScroll("Something went wrong!!");
                    return;
                }
            }
        }

        private void MessageBt_Click(object sender, EventArgs e) {
            if ( prpertiesofplyer == null || !Player.players.Contains(prpertiesofplyer) ) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            Player.SendMessage(prpertiesofplyer, "<CONSOLE> " + PLayersMessageTxt.Text);
            PlayersTextBox.AppendTextAndScroll("Sent player message '<CONSOLE> " + PLayersMessageTxt.Text + "'");
            PLayersMessageTxt.Text = "";
        }

        private void ImpersonateORSendCmdBt_Click(object sender, EventArgs e) {
            if ( prpertiesofplyer == null || !Player.players.Contains(prpertiesofplyer) ) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            try {
                if ( ImpersonateORSendCmdTxt.Text.StartsWith("/") ) {
                    string[] array = ImpersonateORSendCmdTxt.Text.Split(' ');
                    Command cmd = Command.all.Find(array[0].Replace("/", ""));
                    if ( cmd == null ) {
                        PlayersTextBox.AppendTextAndScroll("That isn't a command!!");
                        return;
                    }
                    string paramaters = ImpersonateORSendCmdTxt.Text.Replace(array[0], "");
                    cmd.Use(prpertiesofplyer, paramaters.Trim());
                    if ( paramaters != null ) {
                        PlayersTextBox.AppendTextAndScroll("Used command '" + array[0] + "' with parameters '" + paramaters + "' as player");
                    }
                    else {
                        PlayersTextBox.AppendTextAndScroll("Used command '" + array[0] + "' with no parameters as player");
                    }
                }
                else {
                    Command.all.Find("Impersonate").Use(null, prpertiesofplyer.name + " " + ImpersonateORSendCmdTxt.Text);
                    PlayersTextBox.AppendTextAndScroll("Sent Message '" + ImpersonateORSendCmdTxt.Text + "' as player");
                }
                ImpersonateORSendCmdTxt.Text = "";
            }
            catch {
                PlayersTextBox.AppendTextAndScroll("Something went wrong");
            }
        }

        private void PromoteBt_Click(object sender, EventArgs e) {
            if ( prpertiesofplyer == null || !Player.players.Contains(prpertiesofplyer) ) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            Command.all.Find("Promote").Use(null, prpertiesofplyer.name);
            PlayersTextBox.AppendTextAndScroll("Promoted Player");
            return;
        }

        private void DemoteBt_Click(object sender, EventArgs e) {
            if ( prpertiesofplyer == null || !Player.players.Contains(prpertiesofplyer) ) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            Command.all.Find("Demote").Use(null, prpertiesofplyer.name);
            PlayersTextBox.AppendTextAndScroll("Demoted Player");
            return;
        }

        private void HideBt_Click(object sender, EventArgs e) {
            if ( prpertiesofplyer == null || !Player.players.Contains(prpertiesofplyer) ) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            Command.all.Find("Hide").Use(prpertiesofplyer, "");
            if ( prpertiesofplyer.hidden == true ) {
                PlayersTextBox.AppendTextAndScroll("Hid Player");
                HideBt.Text = "UnHide";
                return;
            }
            else {
                PlayersTextBox.AppendTextAndScroll("UnHid Player");
                HideBt.Text = "Hide";
                return;
            }
        }

        private void SlapBt_Click(object sender, EventArgs e) {
            if ( prpertiesofplyer == null || !Player.players.Contains(prpertiesofplyer) ) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            Command.all.Find("Slap").Use(null, prpertiesofplyer.name);
            PlayersTextBox.AppendTextAndScroll("Slapped Player");
        }

        private void JokerBt_Click(object sender, EventArgs e) {
            if ( prpertiesofplyer == null || !Player.players.Contains(prpertiesofplyer) ) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            Command.all.Find("Joker").Use(null, prpertiesofplyer.name);
            if ( prpertiesofplyer.joker == true ) {
                PlayersTextBox.AppendTextAndScroll("Jokered Player");
                JokerBt.Text = "UnJoker";
                return;
            }
            else {
                PlayersTextBox.AppendTextAndScroll("UnJokered Player");
                JokerBt.Text = "Joker";
                return;
            }
        }

        private void FreezeBt_Click(object sender, EventArgs e) {
            if ( prpertiesofplyer == null || !Player.players.Contains(prpertiesofplyer) ) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            Command.all.Find("Freeze").Use(null, prpertiesofplyer.name);
            if ( prpertiesofplyer.frozen == true ) {
                PlayersTextBox.AppendTextAndScroll("Froze Player");
                FreezeBt.Text = "UnFreeze";
                return;
            }
            else {
                PlayersTextBox.AppendTextAndScroll("UnFroze Player");
                FreezeBt.Text = "Freeze";
                return;
            }
        }

        private void MuteBt_Click(object sender, EventArgs e) {
            if ( prpertiesofplyer == null || !Player.players.Contains(prpertiesofplyer) ) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            Command.all.Find("Mute").Use(null, prpertiesofplyer.name);
            if ( prpertiesofplyer.muted == true ) {
                PlayersTextBox.AppendTextAndScroll("Muted Player");
                MuteBt.Text = "UnMute";
                return;
            }
            else {
                PlayersTextBox.AppendTextAndScroll("UnMuted Player");
                MuteBt.Text = "Mute";
                return;
            }
        }

        private void VoiceBt_Click(object sender, EventArgs e) {
            if ( prpertiesofplyer == null || !Player.players.Contains(prpertiesofplyer) ) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            Command.all.Find("Voice").Use(null, prpertiesofplyer.name);
            if ( prpertiesofplyer.voice == true ) {
                PlayersTextBox.AppendTextAndScroll("Voiced Player");
                VoiceBt.Text = "UnVoice";
                return;
            }
            else {
                PlayersTextBox.AppendTextAndScroll("UnVoiced Player");
                VoiceBt.Text = "Voice";
                return;
            }
        }

        private void KillBt_Click(object sender, EventArgs e) {
            if ( prpertiesofplyer == null || !Player.players.Contains(prpertiesofplyer) ) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            Command.all.Find("Kill").Use(null, prpertiesofplyer.name);
            PlayersTextBox.AppendTextAndScroll("Killed Player");
            return;
        }

        private void JailBt_Click(object sender, EventArgs e) {
            if ( prpertiesofplyer == null || !Player.players.Contains(prpertiesofplyer) ) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            Command.all.Find("Jail").Use(null, prpertiesofplyer.name);
            if ( prpertiesofplyer.jailed == true ) {
                PlayersTextBox.AppendTextAndScroll("Jailed Player");
                JailBt.Text = "UnJail";
                return;
            }
            else {
                PlayersTextBox.AppendTextAndScroll("UnJailed Player");
                JailBt.Text = "Jail";
                return;
            }
        }

        private void WarnBt_Click(object sender, EventArgs e) {
            if ( prpertiesofplyer == null || !Player.players.Contains(prpertiesofplyer) ) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            Command.all.Find("Warn").Use(null, prpertiesofplyer.name);
            PlayersTextBox.AppendTextAndScroll("Warned player");
            return;
        }

        private void KickBt_Click(object sender, EventArgs e) {
            if ( prpertiesofplyer == null || !Player.players.Contains(prpertiesofplyer) ) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            Command.all.Find("Kick").Use(null, prpertiesofplyer.name);
            PlayersTextBox.AppendTextAndScroll("Kicked player");
            return;
        }

        private void BanBt_Click(object sender, EventArgs e) {
            if ( prpertiesofplyer == null || !Player.players.Contains(prpertiesofplyer) ) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            Command.all.Find("Ban").Use(null, prpertiesofplyer.name);
            PlayersTextBox.AppendTextAndScroll("Banned player");
            return;
        }

        private void IPBanBt_Click(object sender, EventArgs e) {
            if ( prpertiesofplyer == null || !Player.players.Contains(prpertiesofplyer) ) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            Command.all.Find("IPBan").Use(null, prpertiesofplyer.name);
            PlayersTextBox.AppendTextAndScroll("IpBanned player");
            return;
        }

        private void SendRulesTxt_Click(object sender, EventArgs e) {
            if ( prpertiesofplyer == null || !Player.players.Contains(prpertiesofplyer) ) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            List<string> rules = new List<string>();
            if ( !File.Exists("text/rules.txt") ) {
                File.WriteAllText("text/rules.txt", "No rules entered yet!");
            }
            using ( StreamReader r = File.OpenText("text/rules.txt") ) {
                while ( !r.EndOfStream )
                    rules.Add(r.ReadLine());
            }
            Player who = prpertiesofplyer;
            who.SendMessage("Server Rules:");
            foreach ( string s in rules ) {
                who.SendMessage(s);
            }
            PlayersTextBox.AppendTextAndScroll("Sent rules to player");
        }

        private void SpawnBt_Click(object sender, EventArgs e) {
            if ( prpertiesofplyer == null || !Player.players.Contains(prpertiesofplyer) ) {
                PlayersTextBox.AppendTextAndScroll("No Player Selected");
                return;
            }
            Player p = prpertiesofplyer;
            ushort x = (ushort)( ( 0.5 + p.level.spawnx ) * 32 );
            ushort y = (ushort)( ( 1 + p.level.spawny ) * 32 );
            ushort z = (ushort)( ( 0.5 + p.level.spawnz ) * 32 );
            unchecked {
                p.SendPos((byte)-1, x, y, z,
                            p.level.rotx,
                            p.level.roty);
            }
            PlayersTextBox.AppendTextAndScroll("Sent player to spawn");
        }

        public void UpdatePlyersListBox() {
            RunOnUiThread(
                delegate {

                    PlyersListBox.Items.Clear();
                    foreach ( Player p in Player.players ) {
                        PlyersListBox.Items.Add(p.name);
                    }
                });

        }

        private void PlyersListBox_Click(object sender, EventArgs e) {
            LoadPLayerTabDetails(sender, e);
        }

        private void ImpersonateORSendCmdTxt_KeyDown(object sender, KeyEventArgs e) {
            if ( e.KeyCode == Keys.Enter ) {
                ImpersonateORSendCmdBt_Click(sender, e);
            }
        }

        private void LoginTxt_KeyDown(object sender, KeyEventArgs e) {
            if ( e.KeyCode == Keys.Enter ) {
                LoginBt_Click(sender, e);
            }
        }

        private void LogoutTxt_KeyDown(object sender, KeyEventArgs e) {
            if ( e.KeyCode == Keys.Enter ) {
                LogoutBt_Click(sender, e);
            }
        }

        private void TitleTxt_KeyDown(object sender, KeyEventArgs e) {
            if ( e.KeyCode == Keys.Enter ) {
                TitleBt_Click(sender, e);
            }
        }

        private void UndoTxt_KeyDown(object sender, KeyEventArgs e) {
            if ( e.KeyCode == Keys.Enter ) {
                UndoBt_Click(sender, e);
            }
        }

        private void PLayersMessageTxt_KeyDown(object sender, KeyEventArgs e) {
            if ( e.KeyCode == Keys.Enter ) {
                MessageBt_Click(sender, e);
            }
        }

        private void ColorCombo_KeyDown(object sender, KeyEventArgs e) {
            if ( e.KeyCode == Keys.Enter ) {
                ColorBt_Click(sender, e);
            }
        }

        private void MapCombo_KeyDown(object sender, KeyEventArgs e) {
            if ( e.KeyCode == Keys.Enter ) {
                MapBt_Click(sender, e);
            }
        }
        #endregion

        /*private void txtOpInput_TextChanged(object sender, EventArgs e)
        {

        }*/
        #endregion

        private void txtOpInput_KeyDown(object sender, KeyEventArgs e) {
            if ( e.KeyCode == Keys.Enter ) {
                if ( txtOpInput.Text == null || txtOpInput.Text.Trim() == "" ) { return; }
                string optext = txtOpInput.Text.Trim();
                string opnewtext = optext;
                Player.GlobalMessageOps("To Ops &f-" + Server.DefaultColor + "Console [&a" + Server.ZallState + Server.DefaultColor + "]&f- " + opnewtext);
                Server.s.Log("(OPs): Console: " + opnewtext, false, LogType.Op);
                txtOpInput.Clear();
            }

        }

        private void txtAdminInput_KeyDown(object sender, KeyEventArgs e) {
            if ( e.KeyCode == Keys.Enter ) {
                if ( txtAdminInput.Text == null || txtAdminInput.Text.Trim() == "" ) { return; }
                string admintext = txtAdminInput.Text.Trim();
                string adminnewtext = admintext;
                Player.GlobalMessageAdmins("To Admins &f-" + Server.DefaultColor + "Console [&a" + Server.ZallState + Server.DefaultColor + "]&f- " + adminnewtext);
                Server.s.Log("(Admins): Console: " + adminnewtext, false, LogType.Admin);
                txtAdminInput.Clear();
            }
        }

        private void button_saveall_Click(object sender, EventArgs e) {
            Command.all.Find("save").Use(null, "all");
        }

        private void killphysics_button_Click(object sender, EventArgs e) {
            Command.all.Find("killphysics").Use(null, "");
            try { UpdateMapList(); }
            catch { }
        }

        private void Unloadempty_button_Click(object sender, EventArgs e) {
            Command.all.Find("unload").Use(null, "empty");
            try { UpdateMapList(); }
            catch { }
        }

        private void dgvMaps_CellContentClick(object sender, DataGridViewCellEventArgs e) {

        }

        private void loadOngotoToolStripMenuItem_Click(object sender, EventArgs e) {
            levelcommand("map", " loadongoto");
        }

        private void instantBuildingToolStripMenuItem_Click(object sender, EventArgs e) {
            levelcommand("map", " instant");
        }

        private void autpPhysicsToolStripMenuItem_Click(object sender, EventArgs e) {
            levelcommand("map", " restartphysics");
        }

        private void gunsToolStripMenuItem_Click(object sender, EventArgs e) {
            levelcommand("allowguns");
        }

        private void unloadToolStripMenuItem1_Click(object sender, EventArgs e) {
            levelcommand("map", " unload");
        }

        private void infoToolStripMenuItem_Click(object sender, EventArgs e) {
            levelcommand("map");
            levelcommand("mapinfo");
        }

        private void actiondToolStripMenuItem_Click(object sender, EventArgs e) {

        }

        private void moveAllToolStripMenuItem_Click(object sender, EventArgs e) {
            levelcommand("moveall");
        }

        private void toolStripMenuItem2_Click_1(object sender, EventArgs e) {
            levelcommand("physics", " 0");
        }

        private void toolStripMenuItem3_Click_1(object sender, EventArgs e) {
            levelcommand("physics", " 1");
        }

        private void toolStripMenuItem4_Click_1(object sender, EventArgs e) {
            levelcommand("physics", " 2");
        }

        private void toolStripMenuItem5_Click_1(object sender, EventArgs e) {
            levelcommand("physics", " 3");
        }

        private void toolStripMenuItem6_Click_1(object sender, EventArgs e) {
            levelcommand("physics", " 4");
        }

        private void toolStripMenuItem7_Click_1(object sender, EventArgs e) {
            levelcommand("physics", " 5");
        }

        private void saveToolStripMenuItem_Click_1(object sender, EventArgs e) {
            levelcommand("save");
        }

        private void unloadToolStripMenuItem_Click_1(object sender, EventArgs e) {
            levelcommand("unload");
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e) {
            levelcommand("reload");
        }

        private void leafDecayToolStripMenuItem_Click(object sender, EventArgs e) {
            levelcommand("map", " leafdecay");
        }

        private void randomFlowToolStripMenuItem_Click(object sender, EventArgs e) {
            levelcommand("map", " randomflow");
        }

        private void treeGrowingToolStripMenuItem_Click(object sender, EventArgs e) {
            levelcommand("map", " growtrees");
        }

        private void txtGlobalInput_KeyDown(object sender, KeyEventArgs e) {
            if ( e.KeyCode == Keys.Enter ) {
                if ( txtGlobalInput.Text == null || txtGlobalInput.Text.Trim() == "" ) { return; }
                try { Command.all.Find("global").Use(null, txtGlobalInput.Text.Trim()); }
                catch ( Exception ex ) { Server.ErrorLog(ex); }
                txtGlobalInput.Clear();
            }
        }

        public void LogGlobalChat(string message) {
            if ( this.InvokeRequired ) {
                this.Invoke(new MethodInvoker(() => txtGlobalLog.AppendTextAndScroll(message)));
                return;
            }
            txtGlobalLog.AppendTextAndScroll(message);
        }

        private void button1_Click(object sender, EventArgs e) {
            //Prevent derpy from getting in here..
            if ( !Server.UseTextures ) {
                WoM.Enabled = false;
                return;
            }
            if ( GetSelectedLevelTab() == null ) return;
            var textures = new GUI.Textures { l = GetSelectedLevelTab() };
            textures.Show();
            textures.FormClosing += delegate {
                textures.Dispose();
            };
        }

        #region Colored Reader Context Menu

        private void nightModeToolStripMenuItem_Click_1(object sender, EventArgs e) {
            if ( MessageBox.Show("Changing to and from night mode will clear your logs. Do you still want to change?", "You sure?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No )
                return;

            txtLog.NightMode = nightModeToolStripMenuItem.Checked;
            nightModeToolStripMenuItem.Checked = !nightModeToolStripMenuItem.Checked;
        }

        private void colorsToolStripMenuItem_Click_1(object sender, EventArgs e) {
            txtLog.Colorize = !colorsToolStripMenuItem.Checked;
            colorsToolStripMenuItem.Checked = !colorsToolStripMenuItem.Checked;

        }

        private void dateStampToolStripMenuItem_Click(object sender, EventArgs e) {
            txtLog.DateStamp = !dateStampToolStripMenuItem.Checked;
            dateStampToolStripMenuItem.Checked = !dateStampToolStripMenuItem.Checked;
        }

        private void autoScrollToolStripMenuItem_Click(object sender, EventArgs e) {
            txtLog.AutoScroll = !autoScrollToolStripMenuItem.Checked;
            autoScrollToolStripMenuItem.Checked = !autoScrollToolStripMenuItem.Checked;
        }

        private void copySelectedToolStripMenuItem_Click(object sender, EventArgs e) {
            if ( String.IsNullOrEmpty(txtLog.SelectedText) )
                return;

            Clipboard.SetText(txtLog.SelectedText, TextDataFormat.Text);
        }
        private void copyAllToolStripMenuItem_Click(object sender, EventArgs e) {
            Clipboard.SetText(txtLog.Text, TextDataFormat.Text);
        }
        private void clearToolStripMenuItem_Click(object sender, EventArgs e) {
            if ( MessageBox.Show("Are you sure you want to clear logs?", "You sure?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes ) {
                txtLog.Clear();
            }
        }
        #endregion

        private void button1_Click_1( object sender, EventArgs e ) {
            try {
                System.Diagnostics.Process.Start( txtUrl.Text );
            } catch {
                MessageBox.Show( "Failed to open browser.", "MCForge Redux" );
            }
        }
    }
}
