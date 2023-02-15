#region Copyright (C) Pierre Boudreau
//This file is part of the Opnieuw project.
//
//pierre@opnieuw.com
//http://www.opnieuw.com
//
//This program is free software; you can redistribute it and/or modify it 
//under the terms of the GNU General Public License as published by the Free
//Software Foundation; either version 2 of the License, or (at your option)
//any later version.
//
//This program is distributed in the hope that it will be useful, but 
//WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
//or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for 
//more details.
//
//You should have received a copy of the GNU General Public License along 
//with this program; 
//if not, write to the Free Software Foundation, Inc., 
//59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
#endregion

using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.CodeDom;
using System.CodeDom.Compiler;
using Opnieuw.Framework;

namespace Opnieuw.UI.TreeBrowser
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class BrowserForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.ImageList Icons;
		private Browser BrowserControl;
		private System.Windows.Forms.Splitter splitter1;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.StatusBar statusBar;
		private System.Windows.Forms.StatusBarPanel statusPanel;
		private System.Windows.Forms.MenuItem mnuFile;
		private System.Windows.Forms.MenuItem mnuFileOpen;
		private System.Windows.Forms.MenuItem mnuFileSave;
		private System.Windows.Forms.MenuItem mnuEdit;
		private System.Windows.Forms.MenuItem mnuEditUndo;
		private System.Windows.Forms.MenuItem mnuTools;
		private System.Windows.Forms.MenuItem mnuToolsOptions;
		private System.Windows.Forms.MenuItem mnuToolsReload;
		private FileCodeChanger m_CodeChanger = null;

		private void RegisterEventHandlers()
		{
			CodeChangeCommandEventHandler m_ChangerBeforeDo = new CodeChangeCommandEventHandler(Changer_BeforeDo);
			m_CodeChanger.BeforeDo += m_ChangerBeforeDo;
			CodeChangeCommandEventHandler m_ChangerAfterDo = new CodeChangeCommandEventHandler(Changer_AfterDo);
			m_CodeChanger.AfterDo += m_ChangerAfterDo;
			CodeChangerEventHandler m_ChangerDid = new CodeChangerEventHandler(Changer_Did);
			m_CodeChanger.Did += m_ChangerDid;
			CodeChangerEventHandler m_ChangerUndid = new CodeChangerEventHandler(Changer_Undid);
			m_CodeChanger.Undid += m_ChangerUndid;
			m_DoHandler = new ExecuteRefactoringHandler(m_CodeChanger.Do);
			m_UndoHandler = new SimpleEventHandler(m_CodeChanger.Undo);
			BrowserControl.Do += m_DoHandler;
			BrowserControl.Undo += m_UndoHandler;
			BrowserControl.ExecuteClicked += new SimpleEventHandler(this.OnExecuteClicked);
			BrowserControl.StatusChanged += new StatusChangedEventHandler(this.OnStatusChanged);
		}

		public BrowserForm()
		{
			InitializeComponent();
			m_CodeChanger = new FileCodeChanger();
			RegisterEventHandlers();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Form editor sttuff
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BrowserForm));
            this.Icons = new System.Windows.Forms.ImageList(this.components);
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.mnuFile = new System.Windows.Forms.MenuItem();
            this.mnuFileOpen = new System.Windows.Forms.MenuItem();
            this.mnuFileSave = new System.Windows.Forms.MenuItem();
            this.mnuEdit = new System.Windows.Forms.MenuItem();
            this.mnuEditUndo = new System.Windows.Forms.MenuItem();
            this.mnuTools = new System.Windows.Forms.MenuItem();
            this.mnuToolsOptions = new System.Windows.Forms.MenuItem();
            this.mnuToolsReload = new System.Windows.Forms.MenuItem();
            this.BrowserControl = new Opnieuw.UI.TreeBrowser.Browser();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.statusBar = new System.Windows.Forms.StatusBar();
            this.statusPanel = new System.Windows.Forms.StatusBarPanel();
            ((System.ComponentModel.ISupportInitialize)(this.statusPanel)).BeginInit();
            this.SuspendLayout();
// 
// Icons
// 
            this.Icons.ImageSize = new System.Drawing.Size(16, 16);
            this.Icons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("Icons.ImageStream")));
            this.Icons.TransparentColor = System.Drawing.Color.Transparent;
// 
// mainMenu1
// 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuFile,
            this.mnuEdit,
            this.mnuTools});
// 
// mnuFile
// 
            this.mnuFile.Index = 0;
            this.mnuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuFileOpen,
            this.mnuFileSave});
            this.mnuFile.Text = "&File";
// 
// mnuFileOpen
// 
            this.mnuFileOpen.Index = 0;
            this.mnuFileOpen.Text = "&Open";
            this.mnuFileOpen.Click += new System.EventHandler(this.mnuFileOpen_Click);
// 
// mnuFileSave
// 
            this.mnuFileSave.Index = 1;
            this.mnuFileSave.Text = "&Save";
            this.mnuFileSave.Click += new System.EventHandler(this.mnuFileSave_Click);
// 
// mnuEdit
// 
            this.mnuEdit.Index = 1;
            this.mnuEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                this.mnuEditUndo
            });
            this.mnuEdit.Text = "&Edit";
// 
// mnuEditUndo
// 
            this.mnuEditUndo.Enabled = false;
            this.mnuEditUndo.Index = 0;
            this.mnuEditUndo.Text = "&Undo";
            this.mnuEditUndo.Click += new System.EventHandler(this.mnuEditUndo_Click);
// 
// mnuTools
// 
            this.mnuTools.Index = 2;
            this.mnuTools.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuToolsOptions,
            this.mnuToolsReload});
            this.mnuTools.Text = "&Tools";
// 
// mnuToolsOptions
// 
            this.mnuToolsOptions.Index = 0;
            this.mnuToolsOptions.Text = "&Options";
            this.mnuToolsOptions.Click += new System.EventHandler(this.mnuToolsOptions_Click);
// 
// mnuToolsReload
// 
            this.mnuToolsReload.Index = 1;
            this.mnuToolsReload.Text = "&Reload";
            this.mnuToolsReload.Click += new System.EventHandler(this.mnuToolsReload_Click);
// 
// BrowserControl
// 
            this.BrowserControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BrowserControl.Location = new System.Drawing.Point(0, 0);
            this.BrowserControl.Size = new System.Drawing.Size(696, 400);
            this.BrowserControl.TabIndex = 0;
// 
// splitter1
// 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Enabled = false;
            this.splitter1.Location = new System.Drawing.Point(0, 400);
            this.splitter1.Size = new System.Drawing.Size(696, 3);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
// 
// statusBar
// 
            this.statusBar.Location = new System.Drawing.Point(0, 403);
            this.statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
                this.statusPanel
            });
            this.statusBar.ShowPanels = true;
            this.statusBar.Size = new System.Drawing.Size(696, 22);
            this.statusBar.TabIndex = 2;
// 
// statusPanel
// 
            this.statusPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            this.statusPanel.Text = "Ready...";
            this.statusPanel.Width = 680;
// 
// BrowserForm
// 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(696, 425);
            this.Controls.Add(this.BrowserControl);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.statusBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMenu1;
            this.Name = "BrowserForm";
            this.Text = "Opnieuw Browser";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.statusPanel)).EndInit();
            this.ResumeLayout(false);

        }
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new BrowserForm());
		}

		ExecuteRefactoringHandler m_DoHandler = null;
		SimpleEventHandler m_UndoHandler = null;

		private void mnuFileOpen_Click(object sender, System.EventArgs e) {
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Multiselect = true;
			ofd.ShowDialog();
			BrowserControl.PopulateTree(ofd.FileNames);
		}

		private void mnuFileSave_Click(object sender, System.EventArgs e) {
			BrowserControl.SaveCurrentFile();
		}

		private bool ParseFile(string fileName)
		{
			TextReader reader = new StreamReader(fileName);
			ICodeParser parser = BrowserControl.CodeDomProvider.CreateParser();
			CodeCompileUnit unit = parser.Parse(reader);
			reader.Close();
			return (null != unit);
		}

		private void PopulateBrowserTree(string fileName)
		{
			string[] param = new string[1];
			param[0] = fileName;
			BrowserControl.PopulateTree(param);
		}

		private void PopulateBrowserTree(CodeChangeCommandCollection codeChanges)
		{
			CodeChangeCommandCollection uniqueFileCommands = new CodeChangeCommandCollection();
			foreach (CodeChangeCommand command in codeChanges) {
				bool exists = false;
				foreach (CodeChangeCommand testCommand in uniqueFileCommands)
				{
					if (testCommand.FileName == command.FileName)
					{
						exists = true;
						break;
					}
				}
				if (exists == false)
				{
					uniqueFileCommands.Add(command);
				}
			}
			string[] fileNames = new string[uniqueFileCommands.Count];
			for (int i=0; i<uniqueFileCommands.Count; i++)
			{
				fileNames[i] = uniqueFileCommands[i].FileName;
			}
			BrowserControl.Clear();
			BrowserControl.PopulateTree(fileNames);
		}

		private bool Changer_BeforeDo(CodeChangeCommand command) {
			bool ret = true;
			if (ConfigurationManager.ParseBeforeExecute == true) {
                if (System.IO.File.Exists(command.FileName)) {
				    ret = ParseFile(command.FileName);
                }
			}
			return ret;
		}

		private bool Changer_AfterDo(CodeChangeCommand command) {
			bool ret = true;
			if (ConfigurationManager.ParseAfterExecute == true)
			{
				ret = ParseFile(command.FileName);
			}
			return ret;
		}

		private void Changer_Did(CodeChangeCommandCollection col) {
			BrowserControl.ReloadAllFiles();
            foreach (CodeChangeCommand command in col) {
                if (command is CreateFileCodeChangeCommand) {
                    String[] fileNames = {command.FileName};
                    BrowserControl.PopulateTree(fileNames);
                }
            }
			mnuEditUndo.Enabled = m_CodeChanger.CanUndo;
		}

		private void Changer_Undid(CodeChangeCommandCollection col) {
			BrowserControl.ReloadAllFiles();
			mnuEditUndo.Enabled = m_CodeChanger.CanUndo;
		}

		private void mnuEditUndo_Click(object sender, System.EventArgs e) {
			BrowserControl.FireUndo();
		}

		private void mnuToolsOptions_Click(object sender, System.EventArgs e) {
			Form options = new OptionsDialog();
			options.ShowDialog(this);
		}

		private void mnuToolsReload_Click(object sender, System.EventArgs e) {
			BrowserControl.ReloadAllFiles();
		}

		private bool OnExecuteClicked()	{
			return BrowserControl.ExecuteCurrentRefactoring();;
		}
		
		private void OnStatusChanged(string newStatus)
		{
			statusPanel.Text = newStatus;
		}
	}
}
