/*
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
using System.Windows.Forms;

namespace MCForge.Gui
{
    public partial class Window
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void WndProc(ref Message msg)
        {
            /*const int WM_SIZE = 0x0005;
            const int SIZE_MINIMIZED = 1;

            if ((msg.Msg == WM_SIZE) && ((int)msg.WParam == SIZE_MINIMIZED) && (Window.Minimize != null))
            {
                this.Window_Minimize(this, EventArgs.Empty);
            }*/

            base.WndProc(ref msg);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Window));
            this.grpMapEditor = new System.Windows.Forms.GroupBox();
            this.label44 = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.btnMapEditorUpdate = new System.Windows.Forms.Button();
            this.btnMapEditorChange = new System.Windows.Forms.Button();
            this.txtMapEditorChangeBlock = new System.Windows.Forms.TextBox();
            this.txtMapEditorCurrentBlock = new System.Windows.Forms.TextBox();
            this.txtMapEditorZ = new System.Windows.Forms.TextBox();
            this.txtMapEditorY = new System.Windows.Forms.TextBox();
            this.txtMapEditorX = new System.Windows.Forms.TextBox();
            this.txtMapEditorLevelName = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.mapsStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.physicsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.physicsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.finiteModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.randomFlowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.edgeWaterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.growingGrassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeGrowingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leafDecayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autpPhysicsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unloadToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.loadOngotoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miscToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.animalAIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.survivalDeathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.killerBlocksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.instantBuildingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rPChatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gunsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actiondToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playerStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.whoisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kickToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.banToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.voiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clonesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.promoteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.demoteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iconContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openConsole = new System.Windows.Forms.ToolStripMenuItem();
            this.shutdownServer = new System.Windows.Forms.ToolStripMenuItem();
            this.restartServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnProperties = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.Restart = new System.Windows.Forms.Button();
            this.txtLogMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.nightModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dateStampToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoScrollToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.copySelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grpMapViewer = new System.Windows.Forms.GroupBox();
            this.label47 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.txtMapViewerRotation = new System.Windows.Forms.NumericUpDown();
            this.txtMapViewerZ = new System.Windows.Forms.TextBox();
            this.txtMapViewerY = new System.Windows.Forms.TextBox();
            this.txtMapViewerX = new System.Windows.Forms.TextBox();
            this.btnMapViewerSave = new System.Windows.Forms.Button();
            this.btnMapViewerUpdate = new System.Windows.Forms.Button();
            this.txtMapViewerLevelName = new System.Windows.Forms.TextBox();
            this.picMapViewer = new System.Windows.Forms.PictureBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.tpChat = new System.Windows.Forms.TabPage();
            this.tcChat = new System.Windows.Forms.TabControl();
            this.tpGlobalChat = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label40 = new System.Windows.Forms.Label();
            this.txtGlobalInput = new System.Windows.Forms.TextBox();
            this.tpOpChat = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label33 = new System.Windows.Forms.Label();
            this.txtOpInput = new System.Windows.Forms.TextBox();
            this.tpAdminChat = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label32 = new System.Windows.Forms.Label();
            this.txtAdminInput = new System.Windows.Forms.TextBox();
            this.tpPlayers = new System.Windows.Forms.TabPage();
            this.StatusTxt = new System.Windows.Forms.TextBox();
            this.LoggedinForTxt = new System.Windows.Forms.TextBox();
            this.Kickstxt = new System.Windows.Forms.TextBox();
            this.TimesLoggedInTxt = new System.Windows.Forms.TextBox();
            this.Blockstxt = new System.Windows.Forms.TextBox();
            this.DeathsTxt = new System.Windows.Forms.TextBox();
            this.IPtxt = new System.Windows.Forms.TextBox();
            this.RankTxt = new System.Windows.Forms.TextBox();
            this.MapTxt = new System.Windows.Forms.TextBox();
            this.NameTxtPlayersTab = new System.Windows.Forms.TextBox();
            this.PlyersListBox = new System.Windows.Forms.ListBox();
            this.label25 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.SpawnBt = new System.Windows.Forms.Button();
            this.UndoTxt = new System.Windows.Forms.TextBox();
            this.UndoBt = new System.Windows.Forms.Button();
            this.SlapBt = new System.Windows.Forms.Button();
            this.SendRulesTxt = new System.Windows.Forms.Button();
            this.ImpersonateORSendCmdTxt = new System.Windows.Forms.TextBox();
            this.ImpersonateORSendCmdBt = new System.Windows.Forms.Button();
            this.KillBt = new System.Windows.Forms.Button();
            this.JailBt = new System.Windows.Forms.Button();
            this.DemoteBt = new System.Windows.Forms.Button();
            this.PromoteBt = new System.Windows.Forms.Button();
            this.LoginTxt = new System.Windows.Forms.TextBox();
            this.LogoutTxt = new System.Windows.Forms.TextBox();
            this.TitleTxt = new System.Windows.Forms.TextBox();
            this.ColorCombo = new System.Windows.Forms.ComboBox();
            this.ColorBt = new System.Windows.Forms.Button();
            this.TitleBt = new System.Windows.Forms.Button();
            this.LogoutBt = new System.Windows.Forms.Button();
            this.LoginBt = new System.Windows.Forms.Button();
            this.FreezeBt = new System.Windows.Forms.Button();
            this.VoiceBt = new System.Windows.Forms.Button();
            this.JokerBt = new System.Windows.Forms.Button();
            this.WarnBt = new System.Windows.Forms.Button();
            this.MessageBt = new System.Windows.Forms.Button();
            this.PLayersMessageTxt = new System.Windows.Forms.TextBox();
            this.HideBt = new System.Windows.Forms.Button();
            this.IPBanBt = new System.Windows.Forms.Button();
            this.BanBt = new System.Windows.Forms.Button();
            this.KickBt = new System.Windows.Forms.Button();
            this.MapCombo = new System.Windows.Forms.ComboBox();
            this.MapBt = new System.Windows.Forms.Button();
            this.MuteBt = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.tpMaps = new System.Windows.Forms.TabPage();
            this.tcMaps = new System.Windows.Forms.TabControl();
            this.tpMapSettings = new System.Windows.Forms.TabPage();
            this.panel3 = new System.Windows.Forms.Panel();
            this.UnloadedList = new System.Windows.Forms.ListBox();
            this.ldmapbt = new System.Windows.Forms.Button();
            this.dgvMapsTab = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.WoM = new System.Windows.Forms.Button();
            this.TreeGrowChk = new System.Windows.Forms.CheckBox();
            this.leafDecayChk = new System.Windows.Forms.CheckBox();
            this.chkRndFlow = new System.Windows.Forms.CheckBox();
            this.UnloadChk = new System.Windows.Forms.CheckBox();
            this.LoadOnGotoChk = new System.Windows.Forms.CheckBox();
            this.AutoLoadChk = new System.Windows.Forms.CheckBox();
            this.drownNumeric = new System.Windows.Forms.NumericUpDown();
            this.Fallnumeric = new System.Windows.Forms.NumericUpDown();
            this.Gunschk = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.Aicombo = new System.Windows.Forms.ComboBox();
            this.edgewaterchk = new System.Windows.Forms.CheckBox();
            this.grasschk = new System.Windows.Forms.CheckBox();
            this.finitechk = new System.Windows.Forms.CheckBox();
            this.Killerbloxchk = new System.Windows.Forms.CheckBox();
            this.SurvivalStyleDeathchk = new System.Windows.Forms.CheckBox();
            this.chatlvlchk = new System.Windows.Forms.CheckBox();
            this.physlvlnumeric = new System.Windows.Forms.NumericUpDown();
            this.MOTDtxt = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.SaveMap = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.seedtxtbox = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.xtxtbox = new System.Windows.Forms.ComboBox();
            this.ytxtbox = new System.Windows.Forms.ComboBox();
            this.ztxtbox = new System.Windows.Forms.ComboBox();
            this.nametxtbox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.maptypecombo = new System.Windows.Forms.ComboBox();
            this.CreateNewMap = new System.Windows.Forms.Button();
            this.tpMapViewer = new System.Windows.Forms.TabPage();
            this.tpLogs = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.tpErrors = new System.Windows.Forms.TabPage();
            this.txtErrors = new System.Windows.Forms.TextBox();
            this.tpChangelog = new System.Windows.Forms.TabPage();
            this.txtChangelog = new System.Windows.Forms.TextBox();
            this.tpSystem = new System.Windows.Forms.TabPage();
            this.txtSystem = new System.Windows.Forms.TextBox();
            this.tpMain = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.label41 = new System.Windows.Forms.Label();
            this.Unloadempty_button = new System.Windows.Forms.Button();
            this.killphysics_button = new System.Windows.Forms.Button();
            this.button_saveall = new System.Windows.Forms.Button();
            this.gBCommands = new System.Windows.Forms.GroupBox();
            this.dgvMaps = new System.Windows.Forms.DataGridView();
            this.gBChat = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCommands = new System.Windows.Forms.TextBox();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.dgvPlayers = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tabPage10 = new System.Windows.Forms.TabPage();
            this.grpRCUsers = new System.Windows.Forms.GroupBox();
            this.liRCUsers = new System.Windows.Forms.ListBox();
            this.grpRCSettings = new System.Windows.Forms.GroupBox();
            this.grpConnectedRCs = new System.Windows.Forms.GroupBox();
            this.txtServerrName = new System.Windows.Forms.TextBox();
            this.txtCommandsUsed = new MCForge.Gui.AutoScrollTextBox();
            this.txtLog = new MCForge.Gui.Components.ColoredTextBox();
            this.LogsTxtBox = new MCForge.Gui.Components.ColoredTextBox();
            this.PlayersTextBox = new MCForge.Gui.AutoScrollTextBox();
            this.txtGlobalLog = new MCForge.Gui.AutoScrollTextBox();
            this.txtOpLog = new MCForge.Gui.AutoScrollTextBox();
            this.txtAdminLog = new MCForge.Gui.AutoScrollTextBox();
            this.grpMapEditor.SuspendLayout();
            this.mapsStrip.SuspendLayout();
            this.playerStrip.SuspendLayout();
            this.iconContext.SuspendLayout();
            this.txtLogMenuStrip.SuspendLayout();
            this.grpMapViewer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMapViewerRotation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMapViewer)).BeginInit();
            this.tpChat.SuspendLayout();
            this.tcChat.SuspendLayout();
            this.tpGlobalChat.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tpOpChat.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tpAdminChat.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tpPlayers.SuspendLayout();
            this.panel4.SuspendLayout();
            this.tpMaps.SuspendLayout();
            this.tcMaps.SuspendLayout();
            this.tpMapSettings.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMapsTab)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.drownNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Fallnumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.physlvlnumeric)).BeginInit();
            this.panel1.SuspendLayout();
            this.tpMapViewer.SuspendLayout();
            this.tpLogs.SuspendLayout();
            this.tpErrors.SuspendLayout();
            this.tpChangelog.SuspendLayout();
            this.tpSystem.SuspendLayout();
            this.tpMain.SuspendLayout();
            this.gBCommands.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaps)).BeginInit();
            this.gBChat.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPlayers)).BeginInit();
            this.tcMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpMapEditor
            // 
            this.grpMapEditor.Controls.Add(this.label44);
            this.grpMapEditor.Controls.Add(this.label43);
            this.grpMapEditor.Controls.Add(this.label42);
            this.grpMapEditor.Controls.Add(this.btnMapEditorUpdate);
            this.grpMapEditor.Controls.Add(this.btnMapEditorChange);
            this.grpMapEditor.Controls.Add(this.txtMapEditorChangeBlock);
            this.grpMapEditor.Controls.Add(this.txtMapEditorCurrentBlock);
            this.grpMapEditor.Controls.Add(this.txtMapEditorZ);
            this.grpMapEditor.Controls.Add(this.txtMapEditorY);
            this.grpMapEditor.Controls.Add(this.txtMapEditorX);
            this.grpMapEditor.Controls.Add(this.txtMapEditorLevelName);
            this.grpMapEditor.Location = new System.Drawing.Point(6, 6);
            this.grpMapEditor.Name = "grpMapEditor";
            this.grpMapEditor.Size = new System.Drawing.Size(340, 137);
            this.grpMapEditor.TabIndex = 0;
            this.grpMapEditor.TabStop = false;
            this.grpMapEditor.Text = "Map Editor";
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Location = new System.Drawing.Point(8, 77);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(42, 13);
            this.label44.TabIndex = 14;
            this.label44.Text = "... with:";
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Location = new System.Drawing.Point(8, 50);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(41, 13);
            this.label43.TabIndex = 13;
            this.label43.Text = "Switch:";
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(8, 23);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(61, 13);
            this.label42.TabIndex = 12;
            this.label42.Text = "Map Name:";
            // 
            // btnMapEditorUpdate
            // 
            this.btnMapEditorUpdate.Location = new System.Drawing.Point(11, 102);
            this.btnMapEditorUpdate.Name = "btnMapEditorUpdate";
            this.btnMapEditorUpdate.Size = new System.Drawing.Size(318, 23);
            this.btnMapEditorUpdate.TabIndex = 11;
            this.btnMapEditorUpdate.Text = "Refresh Map Editor Block";
            this.btnMapEditorUpdate.UseVisualStyleBackColor = true;
            this.btnMapEditorUpdate.Click += new System.EventHandler(this.btnMapEditorUpdate_Click);
            // 
            // btnMapEditorChange
            // 
            this.btnMapEditorChange.Location = new System.Drawing.Point(254, 73);
            this.btnMapEditorChange.Name = "btnMapEditorChange";
            this.btnMapEditorChange.Size = new System.Drawing.Size(75, 23);
            this.btnMapEditorChange.TabIndex = 10;
            this.btnMapEditorChange.Text = "Change";
            this.btnMapEditorChange.UseVisualStyleBackColor = true;
            this.btnMapEditorChange.Click += new System.EventHandler(this.btnMapEditorChange_Click);
            // 
            // txtMapEditorChangeBlock
            // 
            this.txtMapEditorChangeBlock.Location = new System.Drawing.Point(78, 74);
            this.txtMapEditorChangeBlock.Name = "txtMapEditorChangeBlock";
            this.txtMapEditorChangeBlock.Size = new System.Drawing.Size(170, 21);
            this.txtMapEditorChangeBlock.TabIndex = 9;
            // 
            // txtMapEditorCurrentBlock
            // 
            this.txtMapEditorCurrentBlock.Location = new System.Drawing.Point(198, 47);
            this.txtMapEditorCurrentBlock.Name = "txtMapEditorCurrentBlock";
            this.txtMapEditorCurrentBlock.ReadOnly = true;
            this.txtMapEditorCurrentBlock.Size = new System.Drawing.Size(131, 21);
            this.txtMapEditorCurrentBlock.TabIndex = 7;
            this.txtMapEditorCurrentBlock.Text = "none";
            // 
            // txtMapEditorZ
            // 
            this.txtMapEditorZ.Location = new System.Drawing.Point(158, 47);
            this.txtMapEditorZ.Name = "txtMapEditorZ";
            this.txtMapEditorZ.Size = new System.Drawing.Size(34, 21);
            this.txtMapEditorZ.TabIndex = 5;
            this.txtMapEditorZ.Text = "0";
            // 
            // txtMapEditorY
            // 
            this.txtMapEditorY.Location = new System.Drawing.Point(118, 47);
            this.txtMapEditorY.Name = "txtMapEditorY";
            this.txtMapEditorY.Size = new System.Drawing.Size(34, 21);
            this.txtMapEditorY.TabIndex = 4;
            this.txtMapEditorY.Text = "0";
            // 
            // txtMapEditorX
            // 
            this.txtMapEditorX.Location = new System.Drawing.Point(78, 47);
            this.txtMapEditorX.Name = "txtMapEditorX";
            this.txtMapEditorX.Size = new System.Drawing.Size(34, 21);
            this.txtMapEditorX.TabIndex = 3;
            this.txtMapEditorX.Text = "0";
            // 
            // txtMapEditorLevelName
            // 
            this.txtMapEditorLevelName.Location = new System.Drawing.Point(78, 20);
            this.txtMapEditorLevelName.Name = "txtMapEditorLevelName";
            this.txtMapEditorLevelName.Size = new System.Drawing.Size(251, 21);
            this.txtMapEditorLevelName.TabIndex = 1;
            this.txtMapEditorLevelName.Text = "main";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(108, 62);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(19, 13);
            this.label21.TabIndex = 20;
            this.label21.Text = "AI:";
            // 
            // mapsStrip
            // 
            this.mapsStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.physicsToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.actiondToolStripMenuItem,
            this.toolStripSeparator1,
            this.infoToolStripMenuItem});
            this.mapsStrip.Name = "mapsStrip";
            this.mapsStrip.Size = new System.Drawing.Size(144, 98);
            // 
            // physicsToolStripMenuItem
            // 
            this.physicsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5,
            this.toolStripMenuItem6,
            this.toolStripMenuItem7});
            this.physicsToolStripMenuItem.Name = "physicsToolStripMenuItem";
            this.physicsToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.physicsToolStripMenuItem.Text = "Physics Level";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(135, 22);
            this.toolStripMenuItem2.Text = "Off";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click_1);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(135, 22);
            this.toolStripMenuItem3.Text = "Normal";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click_1);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(135, 22);
            this.toolStripMenuItem4.Text = "Advanced";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.toolStripMenuItem4_Click_1);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(135, 22);
            this.toolStripMenuItem5.Text = "Hardcore";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.toolStripMenuItem5_Click_1);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(135, 22);
            this.toolStripMenuItem6.Text = "Instant";
            this.toolStripMenuItem6.Click += new System.EventHandler(this.toolStripMenuItem6_Click_1);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(135, 22);
            this.toolStripMenuItem7.Text = "Doors-Only";
            this.toolStripMenuItem7.Click += new System.EventHandler(this.toolStripMenuItem7_Click_1);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.physicsToolStripMenuItem1,
            this.loadingToolStripMenuItem,
            this.miscToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // physicsToolStripMenuItem1
            // 
            this.physicsToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.finiteModeToolStripMenuItem,
            this.randomFlowToolStripMenuItem,
            this.edgeWaterToolStripMenuItem,
            this.growingGrassToolStripMenuItem,
            this.treeGrowingToolStripMenuItem,
            this.leafDecayToolStripMenuItem,
            this.autpPhysicsToolStripMenuItem});
            this.physicsToolStripMenuItem1.Name = "physicsToolStripMenuItem1";
            this.physicsToolStripMenuItem1.Size = new System.Drawing.Size(117, 22);
            this.physicsToolStripMenuItem1.Text = "Physics";
            // 
            // finiteModeToolStripMenuItem
            // 
            this.finiteModeToolStripMenuItem.Name = "finiteModeToolStripMenuItem";
            this.finiteModeToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.finiteModeToolStripMenuItem.Text = "Finite Mode";
            this.finiteModeToolStripMenuItem.Click += new System.EventHandler(this.finiteModeToolStripMenuItem_Click);
            // 
            // randomFlowToolStripMenuItem
            // 
            this.randomFlowToolStripMenuItem.Name = "randomFlowToolStripMenuItem";
            this.randomFlowToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.randomFlowToolStripMenuItem.Text = "Random Flow";
            this.randomFlowToolStripMenuItem.Click += new System.EventHandler(this.randomFlowToolStripMenuItem_Click);
            // 
            // edgeWaterToolStripMenuItem
            // 
            this.edgeWaterToolStripMenuItem.Name = "edgeWaterToolStripMenuItem";
            this.edgeWaterToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.edgeWaterToolStripMenuItem.Text = "Edge Water";
            this.edgeWaterToolStripMenuItem.Click += new System.EventHandler(this.edgeWaterToolStripMenuItem_Click);
            // 
            // growingGrassToolStripMenuItem
            // 
            this.growingGrassToolStripMenuItem.Name = "growingGrassToolStripMenuItem";
            this.growingGrassToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.growingGrassToolStripMenuItem.Text = "Grass Growing";
            this.growingGrassToolStripMenuItem.Click += new System.EventHandler(this.growingGrassToolStripMenuItem_Click);
            // 
            // treeGrowingToolStripMenuItem
            // 
            this.treeGrowingToolStripMenuItem.Name = "treeGrowingToolStripMenuItem";
            this.treeGrowingToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.treeGrowingToolStripMenuItem.Text = "Tree Growing";
            this.treeGrowingToolStripMenuItem.Click += new System.EventHandler(this.treeGrowingToolStripMenuItem_Click);
            // 
            // leafDecayToolStripMenuItem
            // 
            this.leafDecayToolStripMenuItem.Name = "leafDecayToolStripMenuItem";
            this.leafDecayToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.leafDecayToolStripMenuItem.Text = "Leaf Decay";
            this.leafDecayToolStripMenuItem.Click += new System.EventHandler(this.leafDecayToolStripMenuItem_Click);
            // 
            // autpPhysicsToolStripMenuItem
            // 
            this.autpPhysicsToolStripMenuItem.Name = "autpPhysicsToolStripMenuItem";
            this.autpPhysicsToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.autpPhysicsToolStripMenuItem.Text = "Auto Physics";
            this.autpPhysicsToolStripMenuItem.Click += new System.EventHandler(this.autpPhysicsToolStripMenuItem_Click);
            // 
            // loadingToolStripMenuItem
            // 
            this.loadingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.unloadToolStripMenuItem1,
            this.loadOngotoToolStripMenuItem});
            this.loadingToolStripMenuItem.Name = "loadingToolStripMenuItem";
            this.loadingToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.loadingToolStripMenuItem.Text = "Loading";
            // 
            // unloadToolStripMenuItem1
            // 
            this.unloadToolStripMenuItem1.Name = "unloadToolStripMenuItem1";
            this.unloadToolStripMenuItem1.Size = new System.Drawing.Size(150, 22);
            this.unloadToolStripMenuItem1.Text = "Auto Unload";
            this.unloadToolStripMenuItem1.Click += new System.EventHandler(this.unloadToolStripMenuItem1_Click);
            // 
            // loadOngotoToolStripMenuItem
            // 
            this.loadOngotoToolStripMenuItem.Name = "loadOngotoToolStripMenuItem";
            this.loadOngotoToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.loadOngotoToolStripMenuItem.Text = "Load on /goto";
            this.loadOngotoToolStripMenuItem.Click += new System.EventHandler(this.loadOngotoToolStripMenuItem_Click);
            // 
            // miscToolStripMenuItem
            // 
            this.miscToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.animalAIToolStripMenuItem,
            this.survivalDeathToolStripMenuItem,
            this.killerBlocksToolStripMenuItem,
            this.instantBuildingToolStripMenuItem,
            this.rPChatToolStripMenuItem,
            this.gunsToolStripMenuItem});
            this.miscToolStripMenuItem.Name = "miscToolStripMenuItem";
            this.miscToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.miscToolStripMenuItem.Text = "Misc";
            // 
            // animalAIToolStripMenuItem
            // 
            this.animalAIToolStripMenuItem.Name = "animalAIToolStripMenuItem";
            this.animalAIToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.animalAIToolStripMenuItem.Text = "Animal AI";
            this.animalAIToolStripMenuItem.Click += new System.EventHandler(this.animalAIToolStripMenuItem_Click);
            // 
            // survivalDeathToolStripMenuItem
            // 
            this.survivalDeathToolStripMenuItem.Name = "survivalDeathToolStripMenuItem";
            this.survivalDeathToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.survivalDeathToolStripMenuItem.Text = "Survival Death";
            this.survivalDeathToolStripMenuItem.Click += new System.EventHandler(this.survivalDeathToolStripMenuItem_Click);
            // 
            // killerBlocksToolStripMenuItem
            // 
            this.killerBlocksToolStripMenuItem.Name = "killerBlocksToolStripMenuItem";
            this.killerBlocksToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.killerBlocksToolStripMenuItem.Text = "Killer Blocks";
            this.killerBlocksToolStripMenuItem.Click += new System.EventHandler(this.killerBlocksToolStripMenuItem_Click);
            // 
            // instantBuildingToolStripMenuItem
            // 
            this.instantBuildingToolStripMenuItem.Name = "instantBuildingToolStripMenuItem";
            this.instantBuildingToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.instantBuildingToolStripMenuItem.Text = "Instant Building";
            this.instantBuildingToolStripMenuItem.Click += new System.EventHandler(this.instantBuildingToolStripMenuItem_Click);
            // 
            // rPChatToolStripMenuItem
            // 
            this.rPChatToolStripMenuItem.Name = "rPChatToolStripMenuItem";
            this.rPChatToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.rPChatToolStripMenuItem.Text = "RP Chat";
            this.rPChatToolStripMenuItem.Click += new System.EventHandler(this.rPChatToolStripMenuItem_Click);
            // 
            // gunsToolStripMenuItem
            // 
            this.gunsToolStripMenuItem.Name = "gunsToolStripMenuItem";
            this.gunsToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.gunsToolStripMenuItem.Text = "Guns";
            this.gunsToolStripMenuItem.Click += new System.EventHandler(this.gunsToolStripMenuItem_Click);
            // 
            // actiondToolStripMenuItem
            // 
            this.actiondToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.reloadToolStripMenuItem,
            this.unloadToolStripMenuItem,
            this.moveAllToolStripMenuItem});
            this.actiondToolStripMenuItem.Name = "actiondToolStripMenuItem";
            this.actiondToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.actiondToolStripMenuItem.Text = "Actions";
            this.actiondToolStripMenuItem.Click += new System.EventHandler(this.actiondToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click_1);
            // 
            // reloadToolStripMenuItem
            // 
            this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
            this.reloadToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.reloadToolStripMenuItem.Text = "Reload";
            this.reloadToolStripMenuItem.Click += new System.EventHandler(this.reloadToolStripMenuItem_Click);
            // 
            // unloadToolStripMenuItem
            // 
            this.unloadToolStripMenuItem.Name = "unloadToolStripMenuItem";
            this.unloadToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.unloadToolStripMenuItem.Text = "Unload";
            this.unloadToolStripMenuItem.Click += new System.EventHandler(this.unloadToolStripMenuItem_Click_1);
            // 
            // moveAllToolStripMenuItem
            // 
            this.moveAllToolStripMenuItem.Name = "moveAllToolStripMenuItem";
            this.moveAllToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.moveAllToolStripMenuItem.Text = "Move All";
            this.moveAllToolStripMenuItem.Click += new System.EventHandler(this.moveAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(140, 6);
            // 
            // infoToolStripMenuItem
            // 
            this.infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            this.infoToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.infoToolStripMenuItem.Text = "Info";
            this.infoToolStripMenuItem.Click += new System.EventHandler(this.infoToolStripMenuItem_Click);
            // 
            // playerStrip
            // 
            this.playerStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.whoisToolStripMenuItem,
            this.kickToolStripMenuItem,
            this.banToolStripMenuItem,
            this.voiceToolStripMenuItem,
            this.clonesToolStripMenuItem,
            this.promoteToolStripMenuItem,
            this.demoteToolStripMenuItem});
            this.playerStrip.Name = "playerStrip";
            this.playerStrip.Size = new System.Drawing.Size(121, 158);
            // 
            // whoisToolStripMenuItem
            // 
            this.whoisToolStripMenuItem.Name = "whoisToolStripMenuItem";
            this.whoisToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.whoisToolStripMenuItem.Text = "Whois";
            this.whoisToolStripMenuItem.Click += new System.EventHandler(this.whoisToolStripMenuItem_Click);
            // 
            // kickToolStripMenuItem
            // 
            this.kickToolStripMenuItem.Name = "kickToolStripMenuItem";
            this.kickToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.kickToolStripMenuItem.Text = "Kick";
            this.kickToolStripMenuItem.Click += new System.EventHandler(this.kickToolStripMenuItem_Click);
            // 
            // banToolStripMenuItem
            // 
            this.banToolStripMenuItem.Name = "banToolStripMenuItem";
            this.banToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.banToolStripMenuItem.Text = "Ban";
            this.banToolStripMenuItem.Click += new System.EventHandler(this.banToolStripMenuItem_Click);
            // 
            // voiceToolStripMenuItem
            // 
            this.voiceToolStripMenuItem.Name = "voiceToolStripMenuItem";
            this.voiceToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.voiceToolStripMenuItem.Text = "Voice";
            this.voiceToolStripMenuItem.Click += new System.EventHandler(this.voiceToolStripMenuItem_Click);
            // 
            // clonesToolStripMenuItem
            // 
            this.clonesToolStripMenuItem.Name = "clonesToolStripMenuItem";
            this.clonesToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.clonesToolStripMenuItem.Text = "Clones";
            this.clonesToolStripMenuItem.Click += new System.EventHandler(this.clonesToolStripMenuItem_Click);
            // 
            // promoteToolStripMenuItem
            // 
            this.promoteToolStripMenuItem.Name = "promoteToolStripMenuItem";
            this.promoteToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.promoteToolStripMenuItem.Text = "Promote";
            this.promoteToolStripMenuItem.Click += new System.EventHandler(this.promoteToolStripMenuItem_Click);
            // 
            // demoteToolStripMenuItem
            // 
            this.demoteToolStripMenuItem.Name = "demoteToolStripMenuItem";
            this.demoteToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.demoteToolStripMenuItem.Text = "Demote";
            this.demoteToolStripMenuItem.Click += new System.EventHandler(this.demoteToolStripMenuItem_Click);
            // 
            // iconContext
            // 
            this.iconContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openConsole,
            this.shutdownServer,
            this.restartServerToolStripMenuItem});
            this.iconContext.Name = "iconContext";
            this.iconContext.Size = new System.Drawing.Size(164, 70);
            // 
            // openConsole
            // 
            this.openConsole.Name = "openConsole";
            this.openConsole.Size = new System.Drawing.Size(163, 22);
            this.openConsole.Text = "Open Console";
            this.openConsole.Click += new System.EventHandler(this.openConsole_Click);
            // 
            // shutdownServer
            // 
            this.shutdownServer.Name = "shutdownServer";
            this.shutdownServer.Size = new System.Drawing.Size(163, 22);
            this.shutdownServer.Text = "Shutdown Server";
            this.shutdownServer.Click += new System.EventHandler(this.shutdownServer_Click);
            // 
            // restartServerToolStripMenuItem
            // 
            this.restartServerToolStripMenuItem.Name = "restartServerToolStripMenuItem";
            this.restartServerToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.restartServerToolStripMenuItem.Text = "Restart Server";
            this.restartServerToolStripMenuItem.Click += new System.EventHandler(this.restartServerToolStripMenuItem_Click);
            // 
            // btnProperties
            // 
            this.btnProperties.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.btnProperties.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnProperties.Location = new System.Drawing.Point(546, 5);
            this.btnProperties.Name = "btnProperties";
            this.btnProperties.Size = new System.Drawing.Size(84, 23);
            this.btnProperties.TabIndex = 34;
            this.btnProperties.Text = "Properties";
            this.btnProperties.UseVisualStyleBackColor = true;
            this.btnProperties.Click += new System.EventHandler(this.btnProperties_Click_1);
            // 
            // btnClose
            // 
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.btnClose.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(726, 5);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(84, 23);
            this.btnClose.TabIndex = 35;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click_1);
            // 
            // Restart
            // 
            this.Restart.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.Restart.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Restart.Location = new System.Drawing.Point(636, 5);
            this.Restart.Name = "Restart";
            this.Restart.Size = new System.Drawing.Size(84, 23);
            this.Restart.TabIndex = 36;
            this.Restart.Text = "Restart";
            this.Restart.UseVisualStyleBackColor = true;
            this.Restart.Click += new System.EventHandler(this.Restart_Click);
            // 
            // txtLogMenuStrip
            // 
            this.txtLogMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nightModeToolStripMenuItem,
            this.colorsToolStripMenuItem,
            this.dateStampToolStripMenuItem,
            this.autoScrollToolStripMenuItem,
            this.toolStripSeparator2,
            this.copySelectedToolStripMenuItem,
            this.copyAllToolStripMenuItem,
            this.toolStripSeparator3,
            this.clearToolStripMenuItem});
            this.txtLogMenuStrip.Name = "txtLogMenuStrip";
            this.txtLogMenuStrip.Size = new System.Drawing.Size(150, 170);
            // 
            // nightModeToolStripMenuItem
            // 
            this.nightModeToolStripMenuItem.Name = "nightModeToolStripMenuItem";
            this.nightModeToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.nightModeToolStripMenuItem.Text = "Night Theme";
            this.nightModeToolStripMenuItem.Click += new System.EventHandler(this.nightModeToolStripMenuItem_Click_1);
            // 
            // colorsToolStripMenuItem
            // 
            this.colorsToolStripMenuItem.Checked = true;
            this.colorsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.colorsToolStripMenuItem.Name = "colorsToolStripMenuItem";
            this.colorsToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.colorsToolStripMenuItem.Text = "Colors";
            this.colorsToolStripMenuItem.Click += new System.EventHandler(this.colorsToolStripMenuItem_Click_1);
            // 
            // dateStampToolStripMenuItem
            // 
            this.dateStampToolStripMenuItem.Checked = true;
            this.dateStampToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.dateStampToolStripMenuItem.Name = "dateStampToolStripMenuItem";
            this.dateStampToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.dateStampToolStripMenuItem.Text = "Date Stamp";
            this.dateStampToolStripMenuItem.Click += new System.EventHandler(this.dateStampToolStripMenuItem_Click);
            // 
            // autoScrollToolStripMenuItem
            // 
            this.autoScrollToolStripMenuItem.Checked = true;
            this.autoScrollToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoScrollToolStripMenuItem.Name = "autoScrollToolStripMenuItem";
            this.autoScrollToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.autoScrollToolStripMenuItem.Text = "Auto Scroll";
            this.autoScrollToolStripMenuItem.Click += new System.EventHandler(this.autoScrollToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(146, 6);
            // 
            // copySelectedToolStripMenuItem
            // 
            this.copySelectedToolStripMenuItem.Name = "copySelectedToolStripMenuItem";
            this.copySelectedToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.copySelectedToolStripMenuItem.Text = "Copy Selected";
            this.copySelectedToolStripMenuItem.Click += new System.EventHandler(this.copySelectedToolStripMenuItem_Click);
            // 
            // copyAllToolStripMenuItem
            // 
            this.copyAllToolStripMenuItem.Name = "copyAllToolStripMenuItem";
            this.copyAllToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.copyAllToolStripMenuItem.Text = "Copy All";
            this.copyAllToolStripMenuItem.Click += new System.EventHandler(this.copyAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(146, 6);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // grpMapViewer
            // 
            this.grpMapViewer.Controls.Add(this.label47);
            this.grpMapViewer.Controls.Add(this.label46);
            this.grpMapViewer.Controls.Add(this.label45);
            this.grpMapViewer.Controls.Add(this.txtMapViewerRotation);
            this.grpMapViewer.Controls.Add(this.txtMapViewerZ);
            this.grpMapViewer.Controls.Add(this.txtMapViewerY);
            this.grpMapViewer.Controls.Add(this.txtMapViewerX);
            this.grpMapViewer.Controls.Add(this.btnMapViewerSave);
            this.grpMapViewer.Controls.Add(this.btnMapViewerUpdate);
            this.grpMapViewer.Controls.Add(this.txtMapViewerLevelName);
            this.grpMapViewer.Location = new System.Drawing.Point(6, 149);
            this.grpMapViewer.Name = "grpMapViewer";
            this.grpMapViewer.Size = new System.Drawing.Size(340, 109);
            this.grpMapViewer.TabIndex = 1;
            this.grpMapViewer.TabStop = false;
            this.grpMapViewer.Text = "Map Viewer";
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Location = new System.Drawing.Point(135, 49);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(66, 13);
            this.label47.TabIndex = 23;
            this.label47.Text = "Dimensions:";
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Location = new System.Drawing.Point(8, 49);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(50, 13);
            this.label46.TabIndex = 22;
            this.label46.Text = "Rotation:";
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Location = new System.Drawing.Point(8, 23);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(61, 13);
            this.label45.TabIndex = 21;
            this.label45.Text = "Map Name:";
            // 
            // txtMapViewerRotation
            // 
            this.txtMapViewerRotation.Location = new System.Drawing.Point(78, 47);
            this.txtMapViewerRotation.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.txtMapViewerRotation.Name = "txtMapViewerRotation";
            this.txtMapViewerRotation.Size = new System.Drawing.Size(44, 21);
            this.txtMapViewerRotation.TabIndex = 20;
            // 
            // txtMapViewerZ
            // 
            this.txtMapViewerZ.Location = new System.Drawing.Point(293, 46);
            this.txtMapViewerZ.Name = "txtMapViewerZ";
            this.txtMapViewerZ.ReadOnly = true;
            this.txtMapViewerZ.Size = new System.Drawing.Size(36, 21);
            this.txtMapViewerZ.TabIndex = 19;
            this.txtMapViewerZ.Text = "0";
            // 
            // txtMapViewerY
            // 
            this.txtMapViewerY.Location = new System.Drawing.Point(250, 46);
            this.txtMapViewerY.Name = "txtMapViewerY";
            this.txtMapViewerY.ReadOnly = true;
            this.txtMapViewerY.Size = new System.Drawing.Size(36, 21);
            this.txtMapViewerY.TabIndex = 18;
            this.txtMapViewerY.Text = "0";
            // 
            // txtMapViewerX
            // 
            this.txtMapViewerX.Location = new System.Drawing.Point(207, 46);
            this.txtMapViewerX.Name = "txtMapViewerX";
            this.txtMapViewerX.ReadOnly = true;
            this.txtMapViewerX.Size = new System.Drawing.Size(36, 21);
            this.txtMapViewerX.TabIndex = 17;
            this.txtMapViewerX.Text = "0";
            // 
            // btnMapViewerSave
            // 
            this.btnMapViewerSave.Location = new System.Drawing.Point(173, 74);
            this.btnMapViewerSave.Name = "btnMapViewerSave";
            this.btnMapViewerSave.Size = new System.Drawing.Size(156, 23);
            this.btnMapViewerSave.TabIndex = 15;
            this.btnMapViewerSave.Text = "Save Map Viewer Image";
            this.btnMapViewerSave.UseVisualStyleBackColor = true;
            this.btnMapViewerSave.Click += new System.EventHandler(this.btnMapViewerSave_Click);
            // 
            // btnMapViewerUpdate
            // 
            this.btnMapViewerUpdate.Location = new System.Drawing.Point(11, 74);
            this.btnMapViewerUpdate.Name = "btnMapViewerUpdate";
            this.btnMapViewerUpdate.Size = new System.Drawing.Size(156, 23);
            this.btnMapViewerUpdate.TabIndex = 12;
            this.btnMapViewerUpdate.Text = "Refresh Map Viewer Image";
            this.btnMapViewerUpdate.UseVisualStyleBackColor = true;
            this.btnMapViewerUpdate.Click += new System.EventHandler(this.btnMapViewerUpdate_Click);
            // 
            // txtMapViewerLevelName
            // 
            this.txtMapViewerLevelName.Location = new System.Drawing.Point(78, 20);
            this.txtMapViewerLevelName.Name = "txtMapViewerLevelName";
            this.txtMapViewerLevelName.Size = new System.Drawing.Size(251, 21);
            this.txtMapViewerLevelName.TabIndex = 2;
            this.txtMapViewerLevelName.Text = "main";
            // 
            // picMapViewer
            // 
            this.picMapViewer.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.picMapViewer.Location = new System.Drawing.Point(352, 6);
            this.picMapViewer.Name = "picMapViewer";
            this.picMapViewer.Size = new System.Drawing.Size(430, 430);
            this.picMapViewer.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picMapViewer.TabIndex = 0;
            this.picMapViewer.TabStop = false;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(387, 7);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(33, 13);
            this.label24.TabIndex = 43;
            this.label24.Text = "Rank:";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(86, 115);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(41, 13);
            this.label22.TabIndex = 34;
            this.label22.Text = "Drown:";
            // 
            // tpChat
            // 
            this.tpChat.Controls.Add(this.tcChat);
            this.tpChat.Location = new System.Drawing.Point(4, 22);
            this.tpChat.Name = "tpChat";
            this.tpChat.Padding = new System.Windows.Forms.Padding(3);
            this.tpChat.Size = new System.Drawing.Size(810, 507);
            this.tpChat.TabIndex = 8;
            this.tpChat.Text = "Chat";
            // 
            // tcChat
            // 
            this.tcChat.Controls.Add(this.tpGlobalChat);
            this.tcChat.Controls.Add(this.tpOpChat);
            this.tcChat.Controls.Add(this.tpAdminChat);
            this.tcChat.Location = new System.Drawing.Point(8, 6);
            this.tcChat.Name = "tcChat";
            this.tcChat.SelectedIndex = 0;
            this.tcChat.Size = new System.Drawing.Size(792, 467);
            this.tcChat.TabIndex = 38;
            // 
            // tpGlobalChat
            // 
            this.tpGlobalChat.Controls.Add(this.groupBox3);
            this.tpGlobalChat.Location = new System.Drawing.Point(4, 22);
            this.tpGlobalChat.Name = "tpGlobalChat";
            this.tpGlobalChat.Padding = new System.Windows.Forms.Padding(3);
            this.tpGlobalChat.Size = new System.Drawing.Size(784, 441);
            this.tpGlobalChat.TabIndex = 0;
            this.tpGlobalChat.Text = "Global Chat";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label40);
            this.groupBox3.Controls.Add(this.txtGlobalLog);
            this.groupBox3.Controls.Add(this.txtGlobalInput);
            this.groupBox3.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(3, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(778, 432);
            this.groupBox3.TabIndex = 37;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Global Chat";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(6, 408);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(61, 13);
            this.label40.TabIndex = 32;
            this.label40.Text = "GlobalChat:";
            // 
            // txtGlobalInput
            // 
            this.txtGlobalInput.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGlobalInput.Location = new System.Drawing.Point(73, 405);
            this.txtGlobalInput.Name = "txtGlobalInput";
            this.txtGlobalInput.Size = new System.Drawing.Size(699, 21);
            this.txtGlobalInput.TabIndex = 28;
            this.txtGlobalInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtGlobalInput_KeyDown);
            // 
            // tpOpChat
            // 
            this.tpOpChat.Controls.Add(this.groupBox1);
            this.tpOpChat.Location = new System.Drawing.Point(4, 22);
            this.tpOpChat.Name = "tpOpChat";
            this.tpOpChat.Padding = new System.Windows.Forms.Padding(3);
            this.tpOpChat.Size = new System.Drawing.Size(784, 441);
            this.tpOpChat.TabIndex = 1;
            this.tpOpChat.Text = "Op Chat";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label33);
            this.groupBox1.Controls.Add(this.txtOpInput);
            this.groupBox1.Controls.Add(this.txtOpLog);
            this.groupBox1.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(3, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(778, 432);
            this.groupBox1.TabIndex = 35;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Op Chat";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(6, 408);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(44, 13);
            this.label33.TabIndex = 31;
            this.label33.Text = "OpChat:";
            // 
            // txtOpInput
            // 
            this.txtOpInput.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOpInput.Location = new System.Drawing.Point(56, 405);
            this.txtOpInput.Name = "txtOpInput";
            this.txtOpInput.Size = new System.Drawing.Size(716, 21);
            this.txtOpInput.TabIndex = 30;
            this.txtOpInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtOpInput_KeyDown);
            // 
            // tpAdminChat
            // 
            this.tpAdminChat.Controls.Add(this.groupBox2);
            this.tpAdminChat.Location = new System.Drawing.Point(4, 22);
            this.tpAdminChat.Name = "tpAdminChat";
            this.tpAdminChat.Size = new System.Drawing.Size(784, 441);
            this.tpAdminChat.TabIndex = 2;
            this.tpAdminChat.Text = "Admin Chat";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label32);
            this.groupBox2.Controls.Add(this.txtAdminLog);
            this.groupBox2.Controls.Add(this.txtAdminInput);
            this.groupBox2.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(3, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(778, 432);
            this.groupBox2.TabIndex = 36;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = " Admin Chat";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(6, 408);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(62, 13);
            this.label32.TabIndex = 32;
            this.label32.Text = "AdminChat:";
            // 
            // txtAdminInput
            // 
            this.txtAdminInput.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAdminInput.Location = new System.Drawing.Point(74, 405);
            this.txtAdminInput.Name = "txtAdminInput";
            this.txtAdminInput.Size = new System.Drawing.Size(698, 21);
            this.txtAdminInput.TabIndex = 28;
            this.txtAdminInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtAdminInput_KeyDown);
            // 
            // tpPlayers
            // 
            this.tpPlayers.Controls.Add(this.PlayersTextBox);
            this.tpPlayers.Controls.Add(this.StatusTxt);
            this.tpPlayers.Controls.Add(this.LoggedinForTxt);
            this.tpPlayers.Controls.Add(this.Kickstxt);
            this.tpPlayers.Controls.Add(this.TimesLoggedInTxt);
            this.tpPlayers.Controls.Add(this.Blockstxt);
            this.tpPlayers.Controls.Add(this.DeathsTxt);
            this.tpPlayers.Controls.Add(this.IPtxt);
            this.tpPlayers.Controls.Add(this.RankTxt);
            this.tpPlayers.Controls.Add(this.MapTxt);
            this.tpPlayers.Controls.Add(this.NameTxtPlayersTab);
            this.tpPlayers.Controls.Add(this.PlyersListBox);
            this.tpPlayers.Controls.Add(this.label25);
            this.tpPlayers.Controls.Add(this.label31);
            this.tpPlayers.Controls.Add(this.label30);
            this.tpPlayers.Controls.Add(this.label29);
            this.tpPlayers.Controls.Add(this.label28);
            this.tpPlayers.Controls.Add(this.label27);
            this.tpPlayers.Controls.Add(this.label26);
            this.tpPlayers.Controls.Add(this.panel4);
            this.tpPlayers.Controls.Add(this.label24);
            this.tpPlayers.Controls.Add(this.label14);
            this.tpPlayers.Controls.Add(this.label12);
            this.tpPlayers.Location = new System.Drawing.Point(4, 22);
            this.tpPlayers.Name = "tpPlayers";
            this.tpPlayers.Padding = new System.Windows.Forms.Padding(3);
            this.tpPlayers.Size = new System.Drawing.Size(810, 507);
            this.tpPlayers.TabIndex = 7;
            this.tpPlayers.Text = "Players";
            // 
            // StatusTxt
            // 
            this.StatusTxt.Location = new System.Drawing.Point(612, 4);
            this.StatusTxt.Name = "StatusTxt";
            this.StatusTxt.ReadOnly = true;
            this.StatusTxt.Size = new System.Drawing.Size(188, 21);
            this.StatusTxt.TabIndex = 61;
            // 
            // LoggedinForTxt
            // 
            this.LoggedinForTxt.Location = new System.Drawing.Point(537, 31);
            this.LoggedinForTxt.Name = "LoggedinForTxt";
            this.LoggedinForTxt.ReadOnly = true;
            this.LoggedinForTxt.Size = new System.Drawing.Size(76, 21);
            this.LoggedinForTxt.TabIndex = 59;
            // 
            // Kickstxt
            // 
            this.Kickstxt.Location = new System.Drawing.Point(658, 31);
            this.Kickstxt.Name = "Kickstxt";
            this.Kickstxt.ReadOnly = true;
            this.Kickstxt.Size = new System.Drawing.Size(142, 21);
            this.Kickstxt.TabIndex = 57;
            // 
            // TimesLoggedInTxt
            // 
            this.TimesLoggedInTxt.Location = new System.Drawing.Point(412, 31);
            this.TimesLoggedInTxt.Name = "TimesLoggedInTxt";
            this.TimesLoggedInTxt.ReadOnly = true;
            this.TimesLoggedInTxt.Size = new System.Drawing.Size(92, 21);
            this.TimesLoggedInTxt.TabIndex = 55;
            // 
            // Blockstxt
            // 
            this.Blockstxt.Location = new System.Drawing.Point(281, 31);
            this.Blockstxt.Name = "Blockstxt";
            this.Blockstxt.ReadOnly = true;
            this.Blockstxt.Size = new System.Drawing.Size(65, 21);
            this.Blockstxt.TabIndex = 53;
            // 
            // DeathsTxt
            // 
            this.DeathsTxt.Location = new System.Drawing.Point(188, 31);
            this.DeathsTxt.Name = "DeathsTxt";
            this.DeathsTxt.ReadOnly = true;
            this.DeathsTxt.Size = new System.Drawing.Size(34, 21);
            this.DeathsTxt.TabIndex = 51;
            // 
            // IPtxt
            // 
            this.IPtxt.Location = new System.Drawing.Point(42, 31);
            this.IPtxt.Name = "IPtxt";
            this.IPtxt.ReadOnly = true;
            this.IPtxt.Size = new System.Drawing.Size(89, 21);
            this.IPtxt.TabIndex = 49;
            // 
            // RankTxt
            // 
            this.RankTxt.Location = new System.Drawing.Point(426, 4);
            this.RankTxt.Name = "RankTxt";
            this.RankTxt.ReadOnly = true;
            this.RankTxt.Size = new System.Drawing.Size(134, 21);
            this.RankTxt.TabIndex = 44;
            // 
            // MapTxt
            // 
            this.MapTxt.Location = new System.Drawing.Point(238, 4);
            this.MapTxt.Name = "MapTxt";
            this.MapTxt.ReadOnly = true;
            this.MapTxt.Size = new System.Drawing.Size(143, 21);
            this.MapTxt.TabIndex = 42;
            // 
            // NameTxtPlayersTab
            // 
            this.NameTxtPlayersTab.Location = new System.Drawing.Point(45, 4);
            this.NameTxtPlayersTab.Name = "NameTxtPlayersTab";
            this.NameTxtPlayersTab.ReadOnly = true;
            this.NameTxtPlayersTab.Size = new System.Drawing.Size(150, 21);
            this.NameTxtPlayersTab.TabIndex = 40;
            // 
            // PlyersListBox
            // 
            this.PlyersListBox.FormattingEnabled = true;
            this.PlyersListBox.Location = new System.Drawing.Point(8, 300);
            this.PlyersListBox.Name = "PlyersListBox";
            this.PlyersListBox.Size = new System.Drawing.Size(291, 173);
            this.PlyersListBox.TabIndex = 62;
            this.PlyersListBox.Click += new System.EventHandler(this.PlyersListBox_Click);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(566, 7);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(40, 13);
            this.label25.TabIndex = 60;
            this.label25.Text = "Status:";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(505, 34);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(25, 13);
            this.label31.TabIndex = 58;
            this.label31.Text = "For:";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(619, 34);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(34, 13);
            this.label30.TabIndex = 56;
            this.label30.Text = "Kicks:";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(352, 34);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(54, 13);
            this.label29.TabIndex = 54;
            this.label29.Text = "Logged in:";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(228, 34);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(52, 13);
            this.label28.TabIndex = 52;
            this.label28.Text = "Modified:";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(137, 34);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(44, 13);
            this.label27.TabIndex = 50;
            this.label27.Text = "Deaths:";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(5, 34);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(19, 13);
            this.label26.TabIndex = 48;
            this.label26.Text = "IP:";
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.SpawnBt);
            this.panel4.Controls.Add(this.UndoTxt);
            this.panel4.Controls.Add(this.UndoBt);
            this.panel4.Controls.Add(this.SlapBt);
            this.panel4.Controls.Add(this.SendRulesTxt);
            this.panel4.Controls.Add(this.ImpersonateORSendCmdTxt);
            this.panel4.Controls.Add(this.ImpersonateORSendCmdBt);
            this.panel4.Controls.Add(this.KillBt);
            this.panel4.Controls.Add(this.JailBt);
            this.panel4.Controls.Add(this.DemoteBt);
            this.panel4.Controls.Add(this.PromoteBt);
            this.panel4.Controls.Add(this.LoginTxt);
            this.panel4.Controls.Add(this.LogoutTxt);
            this.panel4.Controls.Add(this.TitleTxt);
            this.panel4.Controls.Add(this.ColorCombo);
            this.panel4.Controls.Add(this.ColorBt);
            this.panel4.Controls.Add(this.TitleBt);
            this.panel4.Controls.Add(this.LogoutBt);
            this.panel4.Controls.Add(this.LoginBt);
            this.panel4.Controls.Add(this.FreezeBt);
            this.panel4.Controls.Add(this.VoiceBt);
            this.panel4.Controls.Add(this.JokerBt);
            this.panel4.Controls.Add(this.WarnBt);
            this.panel4.Controls.Add(this.MessageBt);
            this.panel4.Controls.Add(this.PLayersMessageTxt);
            this.panel4.Controls.Add(this.HideBt);
            this.panel4.Controls.Add(this.IPBanBt);
            this.panel4.Controls.Add(this.BanBt);
            this.panel4.Controls.Add(this.KickBt);
            this.panel4.Controls.Add(this.MapCombo);
            this.panel4.Controls.Add(this.MapBt);
            this.panel4.Controls.Add(this.MuteBt);
            this.panel4.Location = new System.Drawing.Point(8, 59);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(792, 235);
            this.panel4.TabIndex = 47;
            // 
            // SpawnBt
            // 
            this.SpawnBt.Location = new System.Drawing.Point(655, 150);
            this.SpawnBt.Name = "SpawnBt";
            this.SpawnBt.Size = new System.Drawing.Size(131, 23);
            this.SpawnBt.TabIndex = 43;
            this.SpawnBt.Text = "Spawn";
            this.SpawnBt.UseVisualStyleBackColor = true;
            this.SpawnBt.Click += new System.EventHandler(this.SpawnBt_Click);
            // 
            // UndoTxt
            // 
            this.UndoTxt.Location = new System.Drawing.Point(131, 150);
            this.UndoTxt.Name = "UndoTxt";
            this.UndoTxt.Size = new System.Drawing.Size(234, 21);
            this.UndoTxt.TabIndex = 42;
            this.UndoTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UndoTxt_KeyDown);
            // 
            // UndoBt
            // 
            this.UndoBt.Location = new System.Drawing.Point(4, 148);
            this.UndoBt.Name = "UndoBt";
            this.UndoBt.Size = new System.Drawing.Size(121, 23);
            this.UndoBt.TabIndex = 41;
            this.UndoBt.Text = "Undo:";
            this.UndoBt.UseVisualStyleBackColor = true;
            this.UndoBt.Click += new System.EventHandler(this.UndoBt_Click);
            // 
            // SlapBt
            // 
            this.SlapBt.Location = new System.Drawing.Point(369, 150);
            this.SlapBt.Name = "SlapBt";
            this.SlapBt.Size = new System.Drawing.Size(137, 23);
            this.SlapBt.TabIndex = 40;
            this.SlapBt.Text = "Slap";
            this.SlapBt.UseVisualStyleBackColor = true;
            this.SlapBt.Click += new System.EventHandler(this.SlapBt_Click);
            // 
            // SendRulesTxt
            // 
            this.SendRulesTxt.Location = new System.Drawing.Point(655, 121);
            this.SendRulesTxt.Name = "SendRulesTxt";
            this.SendRulesTxt.Size = new System.Drawing.Size(131, 23);
            this.SendRulesTxt.TabIndex = 39;
            this.SendRulesTxt.Text = "Send Rules";
            this.SendRulesTxt.UseVisualStyleBackColor = true;
            this.SendRulesTxt.Click += new System.EventHandler(this.SendRulesTxt_Click);
            // 
            // ImpersonateORSendCmdTxt
            // 
            this.ImpersonateORSendCmdTxt.Location = new System.Drawing.Point(132, 208);
            this.ImpersonateORSendCmdTxt.Name = "ImpersonateORSendCmdTxt";
            this.ImpersonateORSendCmdTxt.Size = new System.Drawing.Size(654, 21);
            this.ImpersonateORSendCmdTxt.TabIndex = 38;
            this.ImpersonateORSendCmdTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ImpersonateORSendCmdTxt_KeyDown);
            // 
            // ImpersonateORSendCmdBt
            // 
            this.ImpersonateORSendCmdBt.Location = new System.Drawing.Point(4, 206);
            this.ImpersonateORSendCmdBt.Name = "ImpersonateORSendCmdBt";
            this.ImpersonateORSendCmdBt.Size = new System.Drawing.Size(122, 23);
            this.ImpersonateORSendCmdBt.TabIndex = 37;
            this.ImpersonateORSendCmdBt.Text = "Impersonate/Cmd:";
            this.ImpersonateORSendCmdBt.UseVisualStyleBackColor = true;
            this.ImpersonateORSendCmdBt.Click += new System.EventHandler(this.ImpersonateORSendCmdBt_Click);
            // 
            // KillBt
            // 
            this.KillBt.Location = new System.Drawing.Point(512, 121);
            this.KillBt.Name = "KillBt";
            this.KillBt.Size = new System.Drawing.Size(137, 23);
            this.KillBt.TabIndex = 36;
            this.KillBt.Text = "Kill";
            this.KillBt.UseVisualStyleBackColor = true;
            this.KillBt.Click += new System.EventHandler(this.KillBt_Click);
            // 
            // JailBt
            // 
            this.JailBt.Location = new System.Drawing.Point(512, 150);
            this.JailBt.Name = "JailBt";
            this.JailBt.Size = new System.Drawing.Size(137, 23);
            this.JailBt.TabIndex = 34;
            this.JailBt.Text = "Jail";
            this.JailBt.UseVisualStyleBackColor = true;
            this.JailBt.Click += new System.EventHandler(this.JailBt_Click);
            // 
            // DemoteBt
            // 
            this.DemoteBt.Location = new System.Drawing.Point(369, 91);
            this.DemoteBt.Name = "DemoteBt";
            this.DemoteBt.Size = new System.Drawing.Size(137, 23);
            this.DemoteBt.TabIndex = 33;
            this.DemoteBt.Text = "Demote";
            this.DemoteBt.UseVisualStyleBackColor = true;
            this.DemoteBt.Click += new System.EventHandler(this.DemoteBt_Click);
            // 
            // PromoteBt
            // 
            this.PromoteBt.Location = new System.Drawing.Point(369, 62);
            this.PromoteBt.Name = "PromoteBt";
            this.PromoteBt.Size = new System.Drawing.Size(137, 23);
            this.PromoteBt.TabIndex = 32;
            this.PromoteBt.Text = "Promote";
            this.PromoteBt.UseVisualStyleBackColor = true;
            this.PromoteBt.Click += new System.EventHandler(this.PromoteBt_Click);
            // 
            // LoginTxt
            // 
            this.LoginTxt.Location = new System.Drawing.Point(131, 5);
            this.LoginTxt.Name = "LoginTxt";
            this.LoginTxt.Size = new System.Drawing.Size(375, 21);
            this.LoginTxt.TabIndex = 31;
            this.LoginTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LoginTxt_KeyDown);
            // 
            // LogoutTxt
            // 
            this.LogoutTxt.Location = new System.Drawing.Point(131, 32);
            this.LogoutTxt.Name = "LogoutTxt";
            this.LogoutTxt.Size = new System.Drawing.Size(375, 21);
            this.LogoutTxt.TabIndex = 30;
            this.LogoutTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LogoutTxt_KeyDown);
            // 
            // TitleTxt
            // 
            this.TitleTxt.Location = new System.Drawing.Point(131, 60);
            this.TitleTxt.Name = "TitleTxt";
            this.TitleTxt.Size = new System.Drawing.Size(234, 21);
            this.TitleTxt.TabIndex = 29;
            this.TitleTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TitleTxt_KeyDown);
            // 
            // ColorCombo
            // 
            this.ColorCombo.FormattingEnabled = true;
            this.ColorCombo.Items.AddRange(new object[] {
            "",
            "Black",
            "Navy",
            "Green",
            "Teal",
            "Maroon",
            "Purple",
            "Gold",
            "Silver",
            "Gray",
            "Blue",
            "Lime",
            "Aqua",
            "Red",
            "Pink",
            "Yellow",
            "White"});
            this.ColorCombo.Location = new System.Drawing.Point(131, 90);
            this.ColorCombo.Name = "ColorCombo";
            this.ColorCombo.Size = new System.Drawing.Size(234, 21);
            this.ColorCombo.TabIndex = 28;
            this.ColorCombo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ColorCombo_KeyDown);
            // 
            // ColorBt
            // 
            this.ColorBt.Location = new System.Drawing.Point(4, 89);
            this.ColorBt.Name = "ColorBt";
            this.ColorBt.Size = new System.Drawing.Size(122, 23);
            this.ColorBt.TabIndex = 27;
            this.ColorBt.Text = "Color:";
            this.ColorBt.UseVisualStyleBackColor = true;
            this.ColorBt.Click += new System.EventHandler(this.ColorBt_Click);
            // 
            // TitleBt
            // 
            this.TitleBt.Location = new System.Drawing.Point(4, 60);
            this.TitleBt.Name = "TitleBt";
            this.TitleBt.Size = new System.Drawing.Size(122, 23);
            this.TitleBt.TabIndex = 26;
            this.TitleBt.Text = "Title:";
            this.TitleBt.UseVisualStyleBackColor = true;
            this.TitleBt.Click += new System.EventHandler(this.TitleBt_Click);
            // 
            // LogoutBt
            // 
            this.LogoutBt.Location = new System.Drawing.Point(4, 31);
            this.LogoutBt.Name = "LogoutBt";
            this.LogoutBt.Size = new System.Drawing.Size(122, 23);
            this.LogoutBt.TabIndex = 25;
            this.LogoutBt.Text = "Logout:";
            this.LogoutBt.UseVisualStyleBackColor = true;
            this.LogoutBt.Click += new System.EventHandler(this.LogoutBt_Click);
            // 
            // LoginBt
            // 
            this.LoginBt.Location = new System.Drawing.Point(4, 4);
            this.LoginBt.Name = "LoginBt";
            this.LoginBt.Size = new System.Drawing.Size(122, 23);
            this.LoginBt.TabIndex = 24;
            this.LoginBt.Text = "Login:";
            this.LoginBt.UseVisualStyleBackColor = true;
            this.LoginBt.Click += new System.EventHandler(this.LoginBt_Click);
            // 
            // FreezeBt
            // 
            this.FreezeBt.Location = new System.Drawing.Point(512, 33);
            this.FreezeBt.Name = "FreezeBt";
            this.FreezeBt.Size = new System.Drawing.Size(137, 23);
            this.FreezeBt.TabIndex = 14;
            this.FreezeBt.Text = "Freeze";
            this.FreezeBt.UseVisualStyleBackColor = true;
            this.FreezeBt.Click += new System.EventHandler(this.FreezeBt_Click);
            // 
            // VoiceBt
            // 
            this.VoiceBt.Location = new System.Drawing.Point(512, 91);
            this.VoiceBt.Name = "VoiceBt";
            this.VoiceBt.Size = new System.Drawing.Size(137, 23);
            this.VoiceBt.TabIndex = 12;
            this.VoiceBt.Text = "Voice";
            this.VoiceBt.UseVisualStyleBackColor = true;
            this.VoiceBt.Click += new System.EventHandler(this.VoiceBt_Click);
            // 
            // JokerBt
            // 
            this.JokerBt.Location = new System.Drawing.Point(512, 4);
            this.JokerBt.Name = "JokerBt";
            this.JokerBt.Size = new System.Drawing.Size(137, 23);
            this.JokerBt.TabIndex = 11;
            this.JokerBt.Text = "Joker";
            this.JokerBt.UseVisualStyleBackColor = true;
            this.JokerBt.Click += new System.EventHandler(this.JokerBt_Click);
            // 
            // WarnBt
            // 
            this.WarnBt.Location = new System.Drawing.Point(655, 4);
            this.WarnBt.Name = "WarnBt";
            this.WarnBt.Size = new System.Drawing.Size(131, 23);
            this.WarnBt.TabIndex = 10;
            this.WarnBt.Text = "Warn";
            this.WarnBt.UseVisualStyleBackColor = true;
            this.WarnBt.Click += new System.EventHandler(this.WarnBt_Click);
            // 
            // MessageBt
            // 
            this.MessageBt.Location = new System.Drawing.Point(4, 177);
            this.MessageBt.Name = "MessageBt";
            this.MessageBt.Size = new System.Drawing.Size(122, 23);
            this.MessageBt.TabIndex = 9;
            this.MessageBt.Text = "Message:";
            this.MessageBt.UseVisualStyleBackColor = true;
            this.MessageBt.Click += new System.EventHandler(this.MessageBt_Click);
            // 
            // PLayersMessageTxt
            // 
            this.PLayersMessageTxt.Location = new System.Drawing.Point(131, 179);
            this.PLayersMessageTxt.Name = "PLayersMessageTxt";
            this.PLayersMessageTxt.Size = new System.Drawing.Size(655, 21);
            this.PLayersMessageTxt.TabIndex = 8;
            this.PLayersMessageTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PLayersMessageTxt_KeyDown);
            // 
            // HideBt
            // 
            this.HideBt.Location = new System.Drawing.Point(369, 121);
            this.HideBt.Name = "HideBt";
            this.HideBt.Size = new System.Drawing.Size(137, 23);
            this.HideBt.TabIndex = 7;
            this.HideBt.Text = "Hide";
            this.HideBt.UseVisualStyleBackColor = true;
            this.HideBt.Click += new System.EventHandler(this.HideBt_Click);
            // 
            // IPBanBt
            // 
            this.IPBanBt.Location = new System.Drawing.Point(655, 91);
            this.IPBanBt.Name = "IPBanBt";
            this.IPBanBt.Size = new System.Drawing.Size(131, 23);
            this.IPBanBt.TabIndex = 6;
            this.IPBanBt.Text = "IP Ban";
            this.IPBanBt.UseVisualStyleBackColor = true;
            this.IPBanBt.Click += new System.EventHandler(this.IPBanBt_Click);
            // 
            // BanBt
            // 
            this.BanBt.Location = new System.Drawing.Point(655, 62);
            this.BanBt.Name = "BanBt";
            this.BanBt.Size = new System.Drawing.Size(131, 23);
            this.BanBt.TabIndex = 5;
            this.BanBt.Text = "Ban";
            this.BanBt.UseVisualStyleBackColor = true;
            this.BanBt.Click += new System.EventHandler(this.BanBt_Click);
            // 
            // KickBt
            // 
            this.KickBt.Location = new System.Drawing.Point(655, 33);
            this.KickBt.Name = "KickBt";
            this.KickBt.Size = new System.Drawing.Size(131, 23);
            this.KickBt.TabIndex = 4;
            this.KickBt.Text = "Kick";
            this.KickBt.UseVisualStyleBackColor = true;
            this.KickBt.Click += new System.EventHandler(this.KickBt_Click);
            // 
            // MapCombo
            // 
            this.MapCombo.FormattingEnabled = true;
            this.MapCombo.Location = new System.Drawing.Point(131, 121);
            this.MapCombo.Name = "MapCombo";
            this.MapCombo.Size = new System.Drawing.Size(234, 21);
            this.MapCombo.TabIndex = 3;
            this.MapCombo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MapCombo_KeyDown);
            // 
            // MapBt
            // 
            this.MapBt.Location = new System.Drawing.Point(4, 119);
            this.MapBt.Name = "MapBt";
            this.MapBt.Size = new System.Drawing.Size(122, 23);
            this.MapBt.TabIndex = 2;
            this.MapBt.Text = "Map:";
            this.MapBt.UseVisualStyleBackColor = true;
            this.MapBt.Click += new System.EventHandler(this.MapBt_Click);
            // 
            // MuteBt
            // 
            this.MuteBt.Location = new System.Drawing.Point(512, 62);
            this.MuteBt.Name = "MuteBt";
            this.MuteBt.Size = new System.Drawing.Size(137, 23);
            this.MuteBt.TabIndex = 13;
            this.MuteBt.Text = "Mute";
            this.MuteBt.UseVisualStyleBackColor = true;
            this.MuteBt.Click += new System.EventHandler(this.MuteBt_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(201, 7);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(31, 13);
            this.label14.TabIndex = 41;
            this.label14.Text = "Map:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(5, 7);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(38, 13);
            this.label12.TabIndex = 39;
            this.label12.Text = "Name:";
            // 
            // tpMaps
            // 
            this.tpMaps.Controls.Add(this.tcMaps);
            this.tpMaps.Location = new System.Drawing.Point(4, 22);
            this.tpMaps.Name = "tpMaps";
            this.tpMaps.Padding = new System.Windows.Forms.Padding(3);
            this.tpMaps.Size = new System.Drawing.Size(810, 507);
            this.tpMaps.TabIndex = 6;
            this.tpMaps.Text = "Maps";
            // 
            // tcMaps
            // 
            this.tcMaps.Controls.Add(this.tpMapSettings);
            this.tcMaps.Controls.Add(this.tpMapViewer);
            this.tcMaps.Location = new System.Drawing.Point(6, 7);
            this.tcMaps.Name = "tcMaps";
            this.tcMaps.SelectedIndex = 0;
            this.tcMaps.Size = new System.Drawing.Size(796, 468);
            this.tcMaps.TabIndex = 50;
            // 
            // tpMapSettings
            // 
            this.tpMapSettings.Controls.Add(this.panel3);
            this.tpMapSettings.Controls.Add(this.dgvMapsTab);
            this.tpMapSettings.Controls.Add(this.panel2);
            this.tpMapSettings.Controls.Add(this.panel1);
            this.tpMapSettings.Location = new System.Drawing.Point(4, 22);
            this.tpMapSettings.Name = "tpMapSettings";
            this.tpMapSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tpMapSettings.Size = new System.Drawing.Size(788, 442);
            this.tpMapSettings.TabIndex = 0;
            this.tpMapSettings.Text = "Map Settings";
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.UnloadedList);
            this.panel3.Controls.Add(this.ldmapbt);
            this.panel3.Location = new System.Drawing.Point(6, 6);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(236, 207);
            this.panel3.TabIndex = 49;
            // 
            // UnloadedList
            // 
            this.UnloadedList.FormattingEnabled = true;
            this.UnloadedList.Location = new System.Drawing.Point(4, 4);
            this.UnloadedList.Name = "UnloadedList";
            this.UnloadedList.Size = new System.Drawing.Size(226, 160);
            this.UnloadedList.TabIndex = 1;
            // 
            // ldmapbt
            // 
            this.ldmapbt.Location = new System.Drawing.Point(4, 168);
            this.ldmapbt.Name = "ldmapbt";
            this.ldmapbt.Size = new System.Drawing.Size(226, 33);
            this.ldmapbt.TabIndex = 0;
            this.ldmapbt.Text = "Load Map";
            this.ldmapbt.UseVisualStyleBackColor = true;
            this.ldmapbt.Click += new System.EventHandler(this.ldmapbt_Click);
            // 
            // dgvMapsTab
            // 
            this.dgvMapsTab.AllowUserToAddRows = false;
            this.dgvMapsTab.AllowUserToDeleteRows = false;
            this.dgvMapsTab.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvMapsTab.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvMapsTab.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMapsTab.Location = new System.Drawing.Point(6, 219);
            this.dgvMapsTab.MultiSelect = false;
            this.dgvMapsTab.Name = "dgvMapsTab";
            this.dgvMapsTab.ReadOnly = true;
            this.dgvMapsTab.RowHeadersVisible = false;
            this.dgvMapsTab.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMapsTab.Size = new System.Drawing.Size(776, 218);
            this.dgvMapsTab.TabIndex = 39;
            this.dgvMapsTab.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMapsTab_CellClick);
            this.dgvMapsTab.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMapsTab_CellClick);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.WoM);
            this.panel2.Controls.Add(this.TreeGrowChk);
            this.panel2.Controls.Add(this.leafDecayChk);
            this.panel2.Controls.Add(this.chkRndFlow);
            this.panel2.Controls.Add(this.UnloadChk);
            this.panel2.Controls.Add(this.LoadOnGotoChk);
            this.panel2.Controls.Add(this.AutoLoadChk);
            this.panel2.Controls.Add(this.drownNumeric);
            this.panel2.Controls.Add(this.Fallnumeric);
            this.panel2.Controls.Add(this.label22);
            this.panel2.Controls.Add(this.Gunschk);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.Aicombo);
            this.panel2.Controls.Add(this.edgewaterchk);
            this.panel2.Controls.Add(this.grasschk);
            this.panel2.Controls.Add(this.finitechk);
            this.panel2.Controls.Add(this.Killerbloxchk);
            this.panel2.Controls.Add(this.SurvivalStyleDeathchk);
            this.panel2.Controls.Add(this.chatlvlchk);
            this.panel2.Controls.Add(this.physlvlnumeric);
            this.panel2.Controls.Add(this.MOTDtxt);
            this.panel2.Controls.Add(this.label21);
            this.panel2.Controls.Add(this.label15);
            this.panel2.Controls.Add(this.label11);
            this.panel2.Controls.Add(this.SaveMap);
            this.panel2.Location = new System.Drawing.Point(427, 6);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(355, 207);
            this.panel2.TabIndex = 48;
            // 
            // WoM
            // 
            this.WoM.Enabled = false;
            this.WoM.Location = new System.Drawing.Point(111, 138);
            this.WoM.Name = "WoM";
            this.WoM.Size = new System.Drawing.Size(108, 24);
            this.WoM.TabIndex = 49;
            this.WoM.Text = "Wom Textures";
            this.WoM.UseVisualStyleBackColor = true;
            this.WoM.Click += new System.EventHandler(this.button1_Click);
            // 
            // TreeGrowChk
            // 
            this.TreeGrowChk.AutoSize = true;
            this.TreeGrowChk.Location = new System.Drawing.Point(7, 142);
            this.TreeGrowChk.Name = "TreeGrowChk";
            this.TreeGrowChk.Size = new System.Drawing.Size(90, 17);
            this.TreeGrowChk.TabIndex = 48;
            this.TreeGrowChk.Text = "Tree growing?";
            this.TreeGrowChk.UseVisualStyleBackColor = true;
            // 
            // leafDecayChk
            // 
            this.leafDecayChk.AutoSize = true;
            this.leafDecayChk.Location = new System.Drawing.Point(236, 142);
            this.leafDecayChk.Name = "leafDecayChk";
            this.leafDecayChk.Size = new System.Drawing.Size(81, 17);
            this.leafDecayChk.TabIndex = 46;
            this.leafDecayChk.Text = "Leaf decay?";
            this.leafDecayChk.UseVisualStyleBackColor = true;
            // 
            // chkRndFlow
            // 
            this.chkRndFlow.AutoSize = true;
            this.chkRndFlow.Location = new System.Drawing.Point(236, 124);
            this.chkRndFlow.Name = "chkRndFlow";
            this.chkRndFlow.Size = new System.Drawing.Size(92, 17);
            this.chkRndFlow.TabIndex = 44;
            this.chkRndFlow.Text = "Random flow?";
            this.chkRndFlow.UseVisualStyleBackColor = true;
            // 
            // UnloadChk
            // 
            this.UnloadChk.AutoSize = true;
            this.UnloadChk.Location = new System.Drawing.Point(236, 105);
            this.UnloadChk.Name = "UnloadChk";
            this.UnloadChk.Size = new System.Drawing.Size(89, 17);
            this.UnloadChk.TabIndex = 42;
            this.UnloadChk.Text = "Auto-unload?";
            this.UnloadChk.UseVisualStyleBackColor = true;
            // 
            // LoadOnGotoChk
            // 
            this.LoadOnGotoChk.AutoSize = true;
            this.LoadOnGotoChk.Location = new System.Drawing.Point(236, 86);
            this.LoadOnGotoChk.Name = "LoadOnGotoChk";
            this.LoadOnGotoChk.Size = new System.Drawing.Size(95, 17);
            this.LoadOnGotoChk.TabIndex = 40;
            this.LoadOnGotoChk.Text = "Load on /goto?";
            this.LoadOnGotoChk.UseVisualStyleBackColor = true;
            // 
            // AutoLoadChk
            // 
            this.AutoLoadChk.AutoSize = true;
            this.AutoLoadChk.Location = new System.Drawing.Point(7, 122);
            this.AutoLoadChk.Name = "AutoLoadChk";
            this.AutoLoadChk.Size = new System.Drawing.Size(77, 17);
            this.AutoLoadChk.TabIndex = 38;
            this.AutoLoadChk.Text = "Auto-load?";
            this.AutoLoadChk.UseVisualStyleBackColor = true;
            // 
            // drownNumeric
            // 
            this.drownNumeric.Location = new System.Drawing.Point(133, 111);
            this.drownNumeric.Name = "drownNumeric";
            this.drownNumeric.Size = new System.Drawing.Size(86, 21);
            this.drownNumeric.TabIndex = 36;
            // 
            // Fallnumeric
            // 
            this.Fallnumeric.Location = new System.Drawing.Point(133, 85);
            this.Fallnumeric.Name = "Fallnumeric";
            this.Fallnumeric.Size = new System.Drawing.Size(86, 21);
            this.Fallnumeric.TabIndex = 35;
            // 
            // Gunschk
            // 
            this.Gunschk.AutoSize = true;
            this.Gunschk.Location = new System.Drawing.Point(7, 103);
            this.Gunschk.Name = "Gunschk";
            this.Gunschk.Size = new System.Drawing.Size(55, 17);
            this.Gunschk.TabIndex = 33;
            this.Gunschk.Text = "Guns?";
            this.Gunschk.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(100, 89);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(27, 13);
            this.label6.TabIndex = 32;
            this.label6.Text = "Fall:";
            // 
            // Aicombo
            // 
            this.Aicombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Aicombo.FormattingEnabled = true;
            this.Aicombo.Items.AddRange(new object[] {
            "Hunt",
            "Flee"});
            this.Aicombo.Location = new System.Drawing.Point(133, 59);
            this.Aicombo.Name = "Aicombo";
            this.Aicombo.Size = new System.Drawing.Size(86, 21);
            this.Aicombo.TabIndex = 30;
            // 
            // edgewaterchk
            // 
            this.edgewaterchk.AutoSize = true;
            this.edgewaterchk.Location = new System.Drawing.Point(236, 67);
            this.edgewaterchk.Name = "edgewaterchk";
            this.edgewaterchk.Size = new System.Drawing.Size(120, 17);
            this.edgewaterchk.TabIndex = 29;
            this.edgewaterchk.Text = "Flowing edgewater?";
            this.edgewaterchk.UseVisualStyleBackColor = true;
            // 
            // grasschk
            // 
            this.grasschk.AutoSize = true;
            this.grasschk.Location = new System.Drawing.Point(7, 64);
            this.grasschk.Name = "grasschk";
            this.grasschk.Size = new System.Drawing.Size(58, 17);
            this.grasschk.TabIndex = 28;
            this.grasschk.Text = "Grass?";
            this.grasschk.UseVisualStyleBackColor = true;
            // 
            // finitechk
            // 
            this.finitechk.AutoSize = true;
            this.finitechk.Location = new System.Drawing.Point(236, 48);
            this.finitechk.Name = "finitechk";
            this.finitechk.Size = new System.Drawing.Size(87, 17);
            this.finitechk.TabIndex = 27;
            this.finitechk.Text = "Finite liquid?";
            this.finitechk.UseVisualStyleBackColor = true;
            // 
            // Killerbloxchk
            // 
            this.Killerbloxchk.AutoSize = true;
            this.Killerbloxchk.Location = new System.Drawing.Point(236, 10);
            this.Killerbloxchk.Name = "Killerbloxchk";
            this.Killerbloxchk.Size = new System.Drawing.Size(88, 17);
            this.Killerbloxchk.TabIndex = 26;
            this.Killerbloxchk.Text = "Killer blocks?";
            this.Killerbloxchk.UseVisualStyleBackColor = true;
            // 
            // SurvivalStyleDeathchk
            // 
            this.SurvivalStyleDeathchk.AutoSize = true;
            this.SurvivalStyleDeathchk.Location = new System.Drawing.Point(236, 29);
            this.SurvivalStyleDeathchk.Name = "SurvivalStyleDeathchk";
            this.SurvivalStyleDeathchk.Size = new System.Drawing.Size(103, 17);
            this.SurvivalStyleDeathchk.TabIndex = 25;
            this.SurvivalStyleDeathchk.Text = "Survival deaths?";
            this.SurvivalStyleDeathchk.UseVisualStyleBackColor = true;
            // 
            // chatlvlchk
            // 
            this.chatlvlchk.AutoSize = true;
            this.chatlvlchk.Location = new System.Drawing.Point(7, 84);
            this.chatlvlchk.Name = "chatlvlchk";
            this.chatlvlchk.Size = new System.Drawing.Size(84, 17);
            this.chatlvlchk.TabIndex = 24;
            this.chatlvlchk.Text = "World-chat?";
            this.chatlvlchk.UseVisualStyleBackColor = true;
            // 
            // physlvlnumeric
            // 
            this.physlvlnumeric.Location = new System.Drawing.Point(81, 33);
            this.physlvlnumeric.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.physlvlnumeric.Name = "physlvlnumeric";
            this.physlvlnumeric.Size = new System.Drawing.Size(138, 21);
            this.physlvlnumeric.TabIndex = 22;
            // 
            // MOTDtxt
            // 
            this.MOTDtxt.Location = new System.Drawing.Point(48, 7);
            this.MOTDtxt.Name = "MOTDtxt";
            this.MOTDtxt.Size = new System.Drawing.Size(171, 21);
            this.MOTDtxt.TabIndex = 21;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(4, 11);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(38, 13);
            this.label15.TabIndex = 14;
            this.label15.Text = "MOTD:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(4, 36);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(72, 13);
            this.label11.TabIndex = 10;
            this.label11.Text = "Physics Level:";
            // 
            // SaveMap
            // 
            this.SaveMap.Location = new System.Drawing.Point(4, 168);
            this.SaveMap.Name = "SaveMap";
            this.SaveMap.Size = new System.Drawing.Size(345, 33);
            this.SaveMap.TabIndex = 9;
            this.SaveMap.Text = "Save Map Properties";
            this.SaveMap.UseVisualStyleBackColor = true;
            this.SaveMap.Click += new System.EventHandler(this.SaveMap_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.seedtxtbox);
            this.panel1.Controls.Add(this.label34);
            this.panel1.Controls.Add(this.xtxtbox);
            this.panel1.Controls.Add(this.ytxtbox);
            this.panel1.Controls.Add(this.ztxtbox);
            this.panel1.Controls.Add(this.nametxtbox);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.maptypecombo);
            this.panel1.Controls.Add(this.CreateNewMap);
            this.panel1.Location = new System.Drawing.Point(248, 6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(173, 207);
            this.panel1.TabIndex = 45;
            // 
            // seedtxtbox
            // 
            this.seedtxtbox.Location = new System.Drawing.Point(45, 141);
            this.seedtxtbox.Name = "seedtxtbox";
            this.seedtxtbox.Size = new System.Drawing.Size(121, 21);
            this.seedtxtbox.TabIndex = 16;
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(4, 146);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(33, 13);
            this.label34.TabIndex = 15;
            this.label34.Text = "Seed:";
            // 
            // xtxtbox
            // 
            this.xtxtbox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.xtxtbox.FormattingEnabled = true;
            this.xtxtbox.Items.AddRange(new object[] {
            "16",
            "32",
            "64",
            "128",
            "256",
            "512",
            "1024"});
            this.xtxtbox.Location = new System.Drawing.Point(45, 34);
            this.xtxtbox.Name = "xtxtbox";
            this.xtxtbox.Size = new System.Drawing.Size(121, 21);
            this.xtxtbox.TabIndex = 14;
            // 
            // ytxtbox
            // 
            this.ytxtbox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ytxtbox.FormattingEnabled = true;
            this.ytxtbox.Items.AddRange(new object[] {
            "16",
            "32",
            "64",
            "128",
            "256",
            "512",
            "1024"});
            this.ytxtbox.Location = new System.Drawing.Point(45, 61);
            this.ytxtbox.Name = "ytxtbox";
            this.ytxtbox.Size = new System.Drawing.Size(121, 21);
            this.ytxtbox.TabIndex = 13;
            // 
            // ztxtbox
            // 
            this.ztxtbox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ztxtbox.FormattingEnabled = true;
            this.ztxtbox.Items.AddRange(new object[] {
            "16",
            "32",
            "64",
            "128",
            "256",
            "512",
            "1024"});
            this.ztxtbox.Location = new System.Drawing.Point(45, 88);
            this.ztxtbox.Name = "ztxtbox";
            this.ztxtbox.Size = new System.Drawing.Size(121, 21);
            this.ztxtbox.TabIndex = 12;
            // 
            // nametxtbox
            // 
            this.nametxtbox.Location = new System.Drawing.Point(45, 7);
            this.nametxtbox.Name = "nametxtbox";
            this.nametxtbox.Size = new System.Drawing.Size(121, 21);
            this.nametxtbox.TabIndex = 9;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(4, 64);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(34, 13);
            this.label10.TabIndex = 8;
            this.label10.Text = "Size Y:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(4, 91);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(35, 13);
            this.label9.TabIndex = 7;
            this.label9.Text = "Size Z:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(4, 37);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(36, 13);
            this.label8.TabIndex = 6;
            this.label8.Text = "Size X:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 10);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "Name:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 119);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Type:";
            // 
            // maptypecombo
            // 
            this.maptypecombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.maptypecombo.FormattingEnabled = true;
            this.maptypecombo.Items.AddRange(new object[] {
            "Island",
            "Mountains",
            "Forest",
            "Ocean",
            "Flat",
            "Pixel",
            "Desert",
            "Space",
            "Rainbow",
            "Hell"});
            this.maptypecombo.Location = new System.Drawing.Point(45, 115);
            this.maptypecombo.Name = "maptypecombo";
            this.maptypecombo.Size = new System.Drawing.Size(121, 21);
            this.maptypecombo.TabIndex = 1;
            // 
            // CreateNewMap
            // 
            this.CreateNewMap.Location = new System.Drawing.Point(4, 168);
            this.CreateNewMap.Name = "CreateNewMap";
            this.CreateNewMap.Size = new System.Drawing.Size(163, 33);
            this.CreateNewMap.TabIndex = 0;
            this.CreateNewMap.Text = "Create New Map";
            this.CreateNewMap.UseVisualStyleBackColor = true;
            this.CreateNewMap.Click += new System.EventHandler(this.CreateNewMap_Click);
            // 
            // tpMapViewer
            // 
            this.tpMapViewer.Controls.Add(this.grpMapViewer);
            this.tpMapViewer.Controls.Add(this.grpMapEditor);
            this.tpMapViewer.Controls.Add(this.picMapViewer);
            this.tpMapViewer.Location = new System.Drawing.Point(4, 22);
            this.tpMapViewer.Name = "tpMapViewer";
            this.tpMapViewer.Padding = new System.Windows.Forms.Padding(3);
            this.tpMapViewer.Size = new System.Drawing.Size(788, 442);
            this.tpMapViewer.TabIndex = 1;
            this.tpMapViewer.Text = "Map Viewer";
            // 
            // tpLogs
            // 
            this.tpLogs.BackColor = System.Drawing.SystemColors.Control;
            this.tpLogs.Controls.Add(this.LogsTxtBox);
            this.tpLogs.Controls.Add(this.label3);
            this.tpLogs.Controls.Add(this.dateTimePicker1);
            this.tpLogs.Location = new System.Drawing.Point(4, 22);
            this.tpLogs.Name = "tpLogs";
            this.tpLogs.Padding = new System.Windows.Forms.Padding(3);
            this.tpLogs.Size = new System.Drawing.Size(810, 507);
            this.tpLogs.TabIndex = 4;
            this.tpLogs.Text = "Logs";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "View logs from:";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(92, 5);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 21);
            this.dateTimePicker1.TabIndex = 2;
            this.dateTimePicker1.Value = new System.DateTime(2011, 7, 20, 18, 31, 50, 0);
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.DatePicker1_ValueChanged);
            // 
            // tpErrors
            // 
            this.tpErrors.BackColor = System.Drawing.Color.Transparent;
            this.tpErrors.Controls.Add(this.txtErrors);
            this.tpErrors.Location = new System.Drawing.Point(4, 22);
            this.tpErrors.Name = "tpErrors";
            this.tpErrors.Padding = new System.Windows.Forms.Padding(3);
            this.tpErrors.Size = new System.Drawing.Size(810, 507);
            this.tpErrors.TabIndex = 2;
            this.tpErrors.Text = "Errors";
            // 
            // txtErrors
            // 
            this.txtErrors.BackColor = System.Drawing.Color.White;
            this.txtErrors.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.txtErrors.Location = new System.Drawing.Point(8, 10);
            this.txtErrors.Multiline = true;
            this.txtErrors.Name = "txtErrors";
            this.txtErrors.ReadOnly = true;
            this.txtErrors.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtErrors.Size = new System.Drawing.Size(792, 463);
            this.txtErrors.TabIndex = 1;
            // 
            // tpChangelog
            // 
            this.tpChangelog.BackColor = System.Drawing.Color.Transparent;
            this.tpChangelog.Controls.Add(this.txtChangelog);
            this.tpChangelog.Location = new System.Drawing.Point(4, 22);
            this.tpChangelog.Name = "tpChangelog";
            this.tpChangelog.Padding = new System.Windows.Forms.Padding(3);
            this.tpChangelog.Size = new System.Drawing.Size(810, 507);
            this.tpChangelog.TabIndex = 1;
            this.tpChangelog.Text = "Changelog";
            // 
            // txtChangelog
            // 
            this.txtChangelog.BackColor = System.Drawing.Color.White;
            this.txtChangelog.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.txtChangelog.Location = new System.Drawing.Point(8, 10);
            this.txtChangelog.Multiline = true;
            this.txtChangelog.Name = "txtChangelog";
            this.txtChangelog.ReadOnly = true;
            this.txtChangelog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtChangelog.Size = new System.Drawing.Size(792, 463);
            this.txtChangelog.TabIndex = 0;
            // 
            // tpSystem
            // 
            this.tpSystem.BackColor = System.Drawing.Color.Transparent;
            this.tpSystem.Controls.Add(this.txtSystem);
            this.tpSystem.Location = new System.Drawing.Point(4, 22);
            this.tpSystem.Name = "tpSystem";
            this.tpSystem.Padding = new System.Windows.Forms.Padding(3);
            this.tpSystem.Size = new System.Drawing.Size(810, 507);
            this.tpSystem.TabIndex = 3;
            this.tpSystem.Text = "System";
            // 
            // txtSystem
            // 
            this.txtSystem.BackColor = System.Drawing.Color.White;
            this.txtSystem.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.txtSystem.Location = new System.Drawing.Point(8, 10);
            this.txtSystem.Multiline = true;
            this.txtSystem.Name = "txtSystem";
            this.txtSystem.ReadOnly = true;
            this.txtSystem.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSystem.Size = new System.Drawing.Size(792, 463);
            this.txtSystem.TabIndex = 1;
            // 
            // tpMain
            // 
            this.tpMain.BackColor = System.Drawing.Color.Transparent;
            this.tpMain.Controls.Add(this.txtServerrName);
            this.tpMain.Controls.Add(this.button1);
            this.tpMain.Controls.Add(this.label41);
            this.tpMain.Controls.Add(this.Unloadempty_button);
            this.tpMain.Controls.Add(this.killphysics_button);
            this.tpMain.Controls.Add(this.button_saveall);
            this.tpMain.Controls.Add(this.gBCommands);
            this.tpMain.Controls.Add(this.dgvMaps);
            this.tpMain.Controls.Add(this.gBChat);
            this.tpMain.Controls.Add(this.label2);
            this.tpMain.Controls.Add(this.txtCommands);
            this.tpMain.Controls.Add(this.txtInput);
            this.tpMain.Controls.Add(this.txtUrl);
            this.tpMain.Controls.Add(this.dgvPlayers);
            this.tpMain.Controls.Add(this.label1);
            this.tpMain.Location = new System.Drawing.Point(4, 22);
            this.tpMain.Name = "tpMain";
            this.tpMain.Padding = new System.Windows.Forms.Padding(3);
            this.tpMain.Size = new System.Drawing.Size(810, 507);
            this.tpMain.TabIndex = 0;
            this.tpMain.Text = "Main";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(423, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 22);
            this.button1.TabIndex = 43;
            this.button1.Text = "Join Server";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(9, 6);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(28, 13);
            this.label41.TabIndex = 42;
            this.label41.Text = "URL:";
            // 
            // Unloadempty_button
            // 
            this.Unloadempty_button.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.Unloadempty_button.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Unloadempty_button.Location = new System.Drawing.Point(707, 267);
            this.Unloadempty_button.Name = "Unloadempty_button";
            this.Unloadempty_button.Size = new System.Drawing.Size(95, 23);
            this.Unloadempty_button.TabIndex = 41;
            this.Unloadempty_button.Text = "Unload Empty";
            this.Unloadempty_button.UseVisualStyleBackColor = true;
            this.Unloadempty_button.Click += new System.EventHandler(this.Unloadempty_button_Click);
            // 
            // killphysics_button
            // 
            this.killphysics_button.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.killphysics_button.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.killphysics_button.Location = new System.Drawing.Point(606, 267);
            this.killphysics_button.Name = "killphysics_button";
            this.killphysics_button.Size = new System.Drawing.Size(96, 23);
            this.killphysics_button.TabIndex = 40;
            this.killphysics_button.Text = "Kill All Physics";
            this.killphysics_button.UseVisualStyleBackColor = true;
            this.killphysics_button.Click += new System.EventHandler(this.killphysics_button_Click);
            // 
            // button_saveall
            // 
            this.button_saveall.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.button_saveall.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_saveall.Location = new System.Drawing.Point(505, 267);
            this.button_saveall.Name = "button_saveall";
            this.button_saveall.Size = new System.Drawing.Size(95, 23);
            this.button_saveall.TabIndex = 39;
            this.button_saveall.Text = "Save All";
            this.button_saveall.UseVisualStyleBackColor = true;
            this.button_saveall.Click += new System.EventHandler(this.button_saveall_Click);
            // 
            // gBCommands
            // 
            this.gBCommands.Controls.Add(this.txtCommandsUsed);
            this.gBCommands.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gBCommands.Location = new System.Drawing.Point(6, 350);
            this.gBCommands.Name = "gBCommands";
            this.gBCommands.Size = new System.Drawing.Size(493, 123);
            this.gBCommands.TabIndex = 34;
            this.gBCommands.TabStop = false;
            this.gBCommands.Text = "Commands";
            // 
            // dgvMaps
            // 
            this.dgvMaps.AllowUserToAddRows = false;
            this.dgvMaps.AllowUserToDeleteRows = false;
            this.dgvMaps.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvMaps.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvMaps.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMaps.ContextMenuStrip = this.mapsStrip;
            this.dgvMaps.Location = new System.Drawing.Point(505, 296);
            this.dgvMaps.MultiSelect = false;
            this.dgvMaps.Name = "dgvMaps";
            this.dgvMaps.ReadOnly = true;
            this.dgvMaps.RowHeadersVisible = false;
            this.dgvMaps.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMaps.Size = new System.Drawing.Size(297, 177);
            this.dgvMaps.TabIndex = 38;
            this.dgvMaps.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMaps_CellContentClick);
            // 
            // gBChat
            // 
            this.gBChat.Controls.Add(this.txtLog);
            this.gBChat.Location = new System.Drawing.Point(7, 30);
            this.gBChat.Name = "gBChat";
            this.gBChat.Size = new System.Drawing.Size(493, 314);
            this.gBChat.TabIndex = 32;
            this.gBChat.TabStop = false;
            this.gBChat.Text = "Chat";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(503, 482);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 29;
            this.label2.Text = "Command:";
            // 
            // txtCommands
            // 
            this.txtCommands.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCommands.Location = new System.Drawing.Point(567, 479);
            this.txtCommands.Name = "txtCommands";
            this.txtCommands.Size = new System.Drawing.Size(235, 21);
            this.txtCommands.TabIndex = 28;
            this.txtCommands.Text = "/";
            this.txtCommands.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCommands_KeyDown);
            // 
            // txtInput
            // 
            this.txtInput.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtInput.Location = new System.Drawing.Point(38, 479);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(456, 21);
            this.txtInput.TabIndex = 27;
            this.txtInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInput_KeyDown);
            // 
            // txtUrl
            // 
            this.txtUrl.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtUrl.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUrl.Location = new System.Drawing.Point(38, 3);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.ReadOnly = true;
            this.txtUrl.Size = new System.Drawing.Size(379, 21);
            this.txtUrl.TabIndex = 25;
            this.txtUrl.DoubleClick += new System.EventHandler(this.txtUrl_DoubleClick);
            // 
            // dgvPlayers
            // 
            this.dgvPlayers.AllowUserToAddRows = false;
            this.dgvPlayers.AllowUserToDeleteRows = false;
            this.dgvPlayers.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvPlayers.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvPlayers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPlayers.ContextMenuStrip = this.playerStrip;
            this.dgvPlayers.Location = new System.Drawing.Point(506, 30);
            this.dgvPlayers.MultiSelect = false;
            this.dgvPlayers.Name = "dgvPlayers";
            this.dgvPlayers.ReadOnly = true;
            this.dgvPlayers.RowHeadersVisible = false;
            this.dgvPlayers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPlayers.Size = new System.Drawing.Size(297, 231);
            this.dgvPlayers.TabIndex = 37;
            this.dgvPlayers.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dgvPlayers_RowPrePaint);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 482);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 26;
            this.label1.Text = "Chat:";
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tpMain);
            this.tcMain.Controls.Add(this.tpSystem);
            this.tcMain.Controls.Add(this.tpChangelog);
            this.tcMain.Controls.Add(this.tpErrors);
            this.tcMain.Controls.Add(this.tpLogs);
            this.tcMain.Controls.Add(this.tpMaps);
            this.tcMain.Controls.Add(this.tpPlayers);
            this.tcMain.Controls.Add(this.tpChat);
            this.tcMain.Cursor = System.Windows.Forms.Cursors.Default;
            this.tcMain.Font = new System.Drawing.Font("Calibri", 8.25F);
            this.tcMain.Location = new System.Drawing.Point(2, 12);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(818, 533);
            this.tcMain.TabIndex = 2;
            this.tcMain.Click += new System.EventHandler(this.tabControl1_Click);
            // 
            // tabPage10
            // 
            this.tabPage10.Location = new System.Drawing.Point(0, 0);
            this.tabPage10.Name = "tabPage10";
            this.tabPage10.Size = new System.Drawing.Size(200, 100);
            this.tabPage10.TabIndex = 0;
            // 
            // grpRCUsers
            // 
            this.grpRCUsers.Location = new System.Drawing.Point(0, 0);
            this.grpRCUsers.Name = "grpRCUsers";
            this.grpRCUsers.Size = new System.Drawing.Size(200, 100);
            this.grpRCUsers.TabIndex = 0;
            this.grpRCUsers.TabStop = false;
            // 
            // liRCUsers
            // 
            this.liRCUsers.Location = new System.Drawing.Point(0, 0);
            this.liRCUsers.Name = "liRCUsers";
            this.liRCUsers.Size = new System.Drawing.Size(120, 95);
            this.liRCUsers.TabIndex = 0;
            // 
            // grpRCSettings
            // 
            this.grpRCSettings.Location = new System.Drawing.Point(0, 0);
            this.grpRCSettings.Name = "grpRCSettings";
            this.grpRCSettings.Size = new System.Drawing.Size(200, 100);
            this.grpRCSettings.TabIndex = 0;
            this.grpRCSettings.TabStop = false;
            // 
            // grpConnectedRCs
            // 
            this.grpConnectedRCs.Location = new System.Drawing.Point(0, 0);
            this.grpConnectedRCs.Name = "grpConnectedRCs";
            this.grpConnectedRCs.Size = new System.Drawing.Size(200, 100);
            this.grpConnectedRCs.TabIndex = 0;
            this.grpConnectedRCs.TabStop = false;
            // 
            // txtServerrName
            // 
            this.txtServerrName.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtServerrName.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtServerrName.Location = new System.Drawing.Point(506, 3);
            this.txtServerrName.Name = "txtServerrName";
            this.txtServerrName.ReadOnly = true;
            this.txtServerrName.Size = new System.Drawing.Size(296, 21);
            this.txtServerrName.TabIndex = 44;
            // 
            // txtCommandsUsed
            // 
            this.txtCommandsUsed.BackColor = System.Drawing.Color.White;
            this.txtCommandsUsed.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtCommandsUsed.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCommandsUsed.Location = new System.Drawing.Point(6, 15);
            this.txtCommandsUsed.Multiline = true;
            this.txtCommandsUsed.Name = "txtCommandsUsed";
            this.txtCommandsUsed.ReadOnly = true;
            this.txtCommandsUsed.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCommandsUsed.Size = new System.Drawing.Size(481, 102);
            this.txtCommandsUsed.TabIndex = 0;
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.SystemColors.Window;
            this.txtLog.ContextMenuStrip = this.txtLogMenuStrip;
            this.txtLog.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLog.Location = new System.Drawing.Point(6, 20);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.txtLog.Size = new System.Drawing.Size(481, 287);
            this.txtLog.TabIndex = 0;
            this.txtLog.Text = "";
            // 
            // LogsTxtBox
            // 
            this.LogsTxtBox.BackColor = System.Drawing.SystemColors.Window;
            this.LogsTxtBox.ContextMenuStrip = this.txtLogMenuStrip;
            this.LogsTxtBox.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LogsTxtBox.Location = new System.Drawing.Point(8, 32);
            this.LogsTxtBox.Name = "LogsTxtBox";
            this.LogsTxtBox.ReadOnly = true;
            this.LogsTxtBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.LogsTxtBox.Size = new System.Drawing.Size(792, 441);
            this.LogsTxtBox.TabIndex = 4;
            this.LogsTxtBox.Text = "";
            // 
            // PlayersTextBox
            // 
            this.PlayersTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.PlayersTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.PlayersTextBox.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PlayersTextBox.Location = new System.Drawing.Point(309, 300);
            this.PlayersTextBox.Multiline = true;
            this.PlayersTextBox.Name = "PlayersTextBox";
            this.PlayersTextBox.ReadOnly = true;
            this.PlayersTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.PlayersTextBox.Size = new System.Drawing.Size(491, 173);
            this.PlayersTextBox.TabIndex = 63;
            // 
            // txtGlobalLog
            // 
            this.txtGlobalLog.BackColor = System.Drawing.SystemColors.Window;
            this.txtGlobalLog.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtGlobalLog.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGlobalLog.Location = new System.Drawing.Point(6, 20);
            this.txtGlobalLog.Multiline = true;
            this.txtGlobalLog.Name = "txtGlobalLog";
            this.txtGlobalLog.ReadOnly = true;
            this.txtGlobalLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtGlobalLog.Size = new System.Drawing.Size(766, 379);
            this.txtGlobalLog.TabIndex = 2;
            // 
            // txtOpLog
            // 
            this.txtOpLog.BackColor = System.Drawing.SystemColors.Window;
            this.txtOpLog.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtOpLog.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOpLog.Location = new System.Drawing.Point(6, 20);
            this.txtOpLog.Multiline = true;
            this.txtOpLog.Name = "txtOpLog";
            this.txtOpLog.ReadOnly = true;
            this.txtOpLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtOpLog.Size = new System.Drawing.Size(766, 379);
            this.txtOpLog.TabIndex = 29;
            // 
            // txtAdminLog
            // 
            this.txtAdminLog.BackColor = System.Drawing.SystemColors.Window;
            this.txtAdminLog.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtAdminLog.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAdminLog.Location = new System.Drawing.Point(6, 20);
            this.txtAdminLog.Multiline = true;
            this.txtAdminLog.Name = "txtAdminLog";
            this.txtAdminLog.ReadOnly = true;
            this.txtAdminLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtAdminLog.Size = new System.Drawing.Size(766, 379);
            this.txtAdminLog.TabIndex = 2;
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 546);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.Restart);
            this.Controls.Add(this.btnProperties);
            this.Controls.Add(this.tcMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Window";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Window_FormClosing);
            this.Load += new System.EventHandler(this.Window_Load);
            this.Resize += new System.EventHandler(this.Window_Resize);
            this.grpMapEditor.ResumeLayout(false);
            this.grpMapEditor.PerformLayout();
            this.mapsStrip.ResumeLayout(false);
            this.playerStrip.ResumeLayout(false);
            this.iconContext.ResumeLayout(false);
            this.txtLogMenuStrip.ResumeLayout(false);
            this.grpMapViewer.ResumeLayout(false);
            this.grpMapViewer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMapViewerRotation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMapViewer)).EndInit();
            this.tpChat.ResumeLayout(false);
            this.tcChat.ResumeLayout(false);
            this.tpGlobalChat.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tpOpChat.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tpAdminChat.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tpPlayers.ResumeLayout(false);
            this.tpPlayers.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.tpMaps.ResumeLayout(false);
            this.tcMaps.ResumeLayout(false);
            this.tpMapSettings.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMapsTab)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.drownNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Fallnumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.physlvlnumeric)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tpMapViewer.ResumeLayout(false);
            this.tpLogs.ResumeLayout(false);
            this.tpLogs.PerformLayout();
            this.tpErrors.ResumeLayout(false);
            this.tpErrors.PerformLayout();
            this.tpChangelog.ResumeLayout(false);
            this.tpChangelog.PerformLayout();
            this.tpSystem.ResumeLayout(false);
            this.tpSystem.PerformLayout();
            this.tpMain.ResumeLayout(false);
            this.tpMain.PerformLayout();
            this.gBCommands.ResumeLayout(false);
            this.gBCommands.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaps)).EndInit();
            this.gBChat.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPlayers)).EndInit();
            this.tcMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion


        private TabPage tabPage10;
        private Button btnClose;
        private ContextMenuStrip iconContext;
        private ToolStripMenuItem openConsole;
        private ToolStripMenuItem shutdownServer;
        private ContextMenuStrip playerStrip;
        private ToolStripMenuItem whoisToolStripMenuItem;
        private ToolStripMenuItem kickToolStripMenuItem;
        private ToolStripMenuItem banToolStripMenuItem;
        private ToolStripMenuItem voiceToolStripMenuItem;
        private ContextMenuStrip mapsStrip;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem finiteModeToolStripMenuItem;
        private ToolStripMenuItem animalAIToolStripMenuItem;
        private ToolStripMenuItem edgeWaterToolStripMenuItem;
        private ToolStripMenuItem growingGrassToolStripMenuItem;
        private ToolStripMenuItem survivalDeathToolStripMenuItem;
        private ToolStripMenuItem killerBlocksToolStripMenuItem;
        private ToolStripMenuItem rPChatToolStripMenuItem;
        private ToolStripMenuItem clonesToolStripMenuItem;
        private Button Restart;
        private ToolStripMenuItem restartServerToolStripMenuItem;
        private ToolStripMenuItem promoteToolStripMenuItem;
        private ToolStripMenuItem demoteToolStripMenuItem;
        private ToolStripMenuItem unloadToolStripMenuItem1;
        private ToolStripMenuItem loadOngotoToolStripMenuItem;
        private ToolStripMenuItem autpPhysicsToolStripMenuItem;
        private ToolStripMenuItem instantBuildingToolStripMenuItem;
        private ToolStripMenuItem gunsToolStripMenuItem;
        private ToolStripMenuItem infoToolStripMenuItem;
        private ToolStripMenuItem actiondToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem unloadToolStripMenuItem;
        private ToolStripMenuItem moveAllToolStripMenuItem;
        private ToolStripMenuItem reloadToolStripMenuItem;
        private ToolStripMenuItem randomFlowToolStripMenuItem;
        private ToolStripMenuItem leafDecayToolStripMenuItem;
        private ToolStripMenuItem treeGrowingToolStripMenuItem;
        private ToolStripMenuItem physicsToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem toolStripMenuItem4;
        private ToolStripMenuItem toolStripMenuItem5;
        private ToolStripMenuItem toolStripMenuItem6;
        private ToolStripMenuItem toolStripMenuItem7;
        private ToolStripMenuItem physicsToolStripMenuItem1;
        private ToolStripMenuItem loadingToolStripMenuItem;
        private ToolStripMenuItem miscToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ContextMenuStrip txtLogMenuStrip;
        private ToolStripMenuItem nightModeToolStripMenuItem;
        private ToolStripMenuItem colorsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem copySelectedToolStripMenuItem;
        private ToolStripMenuItem copyAllToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem clearToolStripMenuItem;
        private ToolStripMenuItem dateStampToolStripMenuItem;
        private ToolStripMenuItem autoScrollToolStripMenuItem;
        private Button btnProperties;
        private TabPage tpChat;
        private GroupBox groupBox3;
        private Label label40;
        private AutoScrollTextBox txtGlobalLog;
        private TextBox txtGlobalInput;
        private GroupBox groupBox2;
        private Label label32;
        private AutoScrollTextBox txtAdminLog;
        private TextBox txtAdminInput;
        private GroupBox groupBox1;
        private Label label33;
        private TextBox txtOpInput;
        private AutoScrollTextBox txtOpLog;
        private TabPage tpPlayers;
        private GroupBox grpRCUsers;
        private GroupBox grpRCSettings;
        private GroupBox grpConnectedRCs;
        public ListBox liRCUsers;
        private AutoScrollTextBox PlayersTextBox;
        private TextBox StatusTxt;
        private TextBox LoggedinForTxt;
        private TextBox Kickstxt;
        private TextBox TimesLoggedInTxt;
        private TextBox Blockstxt;
        private TextBox DeathsTxt;
        private TextBox IPtxt;
        private TextBox RankTxt;
        private TextBox MapTxt;
        private TextBox NameTxtPlayersTab;
        private ListBox PlyersListBox;
        private Label label25;
        private Label label31;
        private Label label30;
        private Label label29;
        private Label label28;
        private Label label27;
        private Label label26;
        private Panel panel4;
        private Button SpawnBt;
        private TextBox UndoTxt;
        private Button UndoBt;
        private Button SlapBt;
        private Button SendRulesTxt;
        private TextBox ImpersonateORSendCmdTxt;
        private Button ImpersonateORSendCmdBt;
        private Button KillBt;
        private Button JailBt;
        private Button DemoteBt;
        private Button PromoteBt;
        private TextBox LoginTxt;
        private TextBox LogoutTxt;
        private TextBox TitleTxt;
        private ComboBox ColorCombo;
        private Button ColorBt;
        private Button TitleBt;
        private Button LogoutBt;
        private Button LoginBt;
        private Button FreezeBt;
        private Button VoiceBt;
        private Button JokerBt;
        private Button WarnBt;
        private Button MessageBt;
        private TextBox PLayersMessageTxt;
        private Button HideBt;
        private Button IPBanBt;
        private Button BanBt;
        private Button KickBt;
        private ComboBox MapCombo;
        private Button MapBt;
        private Button MuteBt;
        private Label label24;
        private Button btnMapViewerUpdate;
        private Button btnMapViewerSave;
        private Label label14;
        private Label label12;
        private TabPage tpMaps;
        private Panel panel3;
        public ListBox UnloadedList;
        private Button ldmapbt;
        private Panel panel2;
        private Button WoM;
        private CheckBox TreeGrowChk;
        private CheckBox leafDecayChk;
        private CheckBox chkRndFlow;
        private CheckBox UnloadChk;
        private CheckBox LoadOnGotoChk;
        private CheckBox AutoLoadChk;
        private NumericUpDown drownNumeric;
        private NumericUpDown Fallnumeric;
        private Label label22;
        private CheckBox Gunschk;
        private Label label6;
        private ComboBox Aicombo;
        private CheckBox edgewaterchk;
        private CheckBox grasschk;
        private CheckBox finitechk;
        private CheckBox Killerbloxchk;
        private CheckBox SurvivalStyleDeathchk;
        private CheckBox chatlvlchk;
        private NumericUpDown physlvlnumeric;
        private TextBox MOTDtxt;
        private Label label21;
        private Label label15;
        private Label label11;
        private Button SaveMap;
        private Panel panel1;
        private TextBox seedtxtbox;
        private Label label34;
        private ComboBox xtxtbox;
        private ComboBox ytxtbox;
        private ComboBox ztxtbox;
        private TextBox nametxtbox;
        private Label label10;
        private Label label9;
        private Label label8;
        private Label label7;
        private Label label4;
        private ComboBox maptypecombo;
        private Button CreateNewMap;
        private DataGridView dgvMapsTab;
        private TabPage tpLogs;
        private Label label3;
        private DateTimePicker dateTimePicker1;
        private TabPage tpErrors;
        private TextBox txtErrors;
        private TabPage tpChangelog;
        private TextBox txtChangelog;
        private TabPage tpSystem;
        private TextBox txtSystem;
        private TabPage tpMain;
        private Button Unloadempty_button;
        private Button killphysics_button;
        private Button button_saveall;
        private GroupBox gBCommands;
        private AutoScrollTextBox txtCommandsUsed;
        private DataGridView dgvMaps;
        private GroupBox gBChat;
        private Components.ColoredTextBox txtLog;
        private Label label2;
        private TextBox txtCommands;
        private TextBox txtInput;
        private TextBox txtUrl;
        private DataGridView dgvPlayers;
        private Label label1;
        private TabControl tcMain;
        private TabControl tcChat;
        private TabPage tpGlobalChat;
        private TabPage tpOpChat;
        private TabPage tpAdminChat;
        private Label label41;
        private TabControl tcMaps;
        private TabPage tpMapSettings;
        private TabPage tpMapViewer;
        private Label label44;
        private Label label43;
        private Label label42;
        private Label label46;
        private Label label45;
        private Label label47;
        private Button button1;
        private Components.ColoredTextBox LogsTxtBox;
        private TextBox txtServerrName;
    }
}