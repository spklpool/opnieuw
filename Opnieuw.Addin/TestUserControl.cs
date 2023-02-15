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
using System.Resources;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Reflection;
using System.Windows.Forms;
using System.CodeDom;
using System.CodeDom.Compiler;
using Opnieuw.Framework;
using Opnieuw.Parsers.CSParser;
using Opnieuw.UI.TreeBrowser;

namespace Opnieuw.Addin
{
    public delegate bool SimpleEventHandler();

    /// <summary>
    /// This is the refactoring browser control.  It can be hosted in any container 
    /// that can accept .NET user controls.  It contains a tree view of the parced
    /// C# source files and a tab view that dynamically adjusts itself depending on 
    /// the selected nodes in the tree view so only the refactorings that make sense 
    /// with the current selection are shown.
    /// </summary>
    public class TestUserControl : System.Windows.Forms.UserControl
    {
        public event StatusChangedEventHandler StatusChanged;
        public event ExecuteRefactoringHandler Do;
        public event SimpleEventHandler Undo;
        public event SimpleEventHandler ExecuteClicked;

        private System.Windows.Forms.TabControl RefactoringTabs;
        private System.Windows.Forms.Panel ButtonPannel;
        private System.Windows.Forms.Button Execute;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.Panel RefactoringTabsPannel;
        private ImageList RefactoringIcons;
        private IContainer components;
        private ParserProgress parserProgress;
        private System.Windows.Forms.Splitter splitter1;

        /// <summary>
        /// Constructor
        /// </summary>
        public TestUserControl()
        {
            InitializeComponent();
            LoadLanguages();
            LoadRefactorings();
            RegisterEventListeners();
        }

        /// <summary>
        /// Register any listeners needed at startup.
        /// </summary>
        private void RegisterEventListeners()
        {
            m_ViewControler.ParsingStarted += new ParsingStartedEventHandler(this.OnParsingStarted);
            m_ViewControler.ParsingDone += new ParsingDoneEventHandler(this.OnParsingDone);
            m_ViewControler.ParsingFile += new ParsingFileEventHandler(this.OnParsingFile);
            m_ViewControler.ParsingFileDone += new ParsingFileDoneEventHandler(this.OnParsingFileDone);
            m_ViewControler.StatusChanged += new StatusChangedEventHandler(this.OnStatusChanged);
        }

        /// <summary>
        /// This is a helper to fire the StatusChanged event to all listeners.
        /// </summary>
        public void FireStatusChanged(string newStatus)
        {
            if (null != StatusChanged) {
                StatusChanged(newStatus);
            }
        }

        /// <summary>
        /// This is a helper to fire the Undo event to all listeners.
        /// </summary>
        public void FireUndo()
        {
            if (null != Undo) {
                startProgressMode(1);
                m_Executing = true;
                Undo();
                foreach (RefactoringTabPage page in refactoringTabPages) {
                    RefactoringTabs.Controls.Remove(page);
                }
                m_Executing = false;
                endProgressMode();
            }
        }

        /// <summary>
        /// This is a helper to fire the ExecuteClicked event to all listeners.
        /// </summary>
        private void FireExecuteClicked()
        {
            if (null != ExecuteClicked) {
                ExecuteClicked();
            }
        }

        /// <summary>
        /// This gets called when parsing starts on one or more files.
        /// </summary>
        /// <param name="fileCount"></param>
        private void OnParsingStarted(int fileCount)
        {
            if (m_Executing == false) {
                startProgressMode(fileCount);
            }
        }

        /// <summary>
        /// This gets called when parsing the specified file.
        /// </summary>
        /// <param name="fileName"></param>
        private void OnParsingFile(string fileName)
        {
            if (m_Executing == false) {
                parserProgress.Text = fileName;
                parserProgress.PerformProgressStep();
                parserProgress.Refresh();
            }
        }

        /// <summary>
        /// This gets called when parsing of the specified file
        /// is finished.
        /// </summary>
        /// <param name="fileName"></param>
        private void OnParsingFileDone(string fileName, CodeCompileUnit unit, bool success)
        {
        }

        /// <summary>
        /// This gets called when paraing is done.
        /// </summary>
        private void OnParsingDone()
        {
            if (m_Executing == false) {
                endProgressMode();
            }
        }

        private void OnStatusChanged(string newStatus)
        {
            FireStatusChanged(newStatus);
        }

        private bool m_Executing = false;

        private void startProgressMode(int progressCount)
        {
            Execute.Height = 0;
            ButtonPannel.Height = 50;
            parserProgress.Height = 50;
            parserProgress.MaxProgress = progressCount;
            parserProgress.Text = "Executing";
            this.Refresh();
            parserProgress.Refresh();
        }

        private void endProgressMode()
        {
            parserProgress.Text = "";
            ButtonPannel.Height = 35;
            parserProgress.Height = 0;
            Execute.Height = 35;
        }

        /// <summary>
        /// This is the event handler for the OnChangeCode event of RefacrotingTabPage
        /// objects.
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        private bool OnChangeCode(CodeChangeCommandCollection col)
        {
            bool ret = true;
            if (null != Do) {
                startProgressMode(1);
                m_Executing = true;
                ret = Do(col);
                foreach (RefactoringTabPage page in refactoringTabPages) {
                    RefactoringTabs.Controls.Remove(page);
                }
                endProgressMode();
                m_Executing = false;
            }
            return ret;
        }

        private void OnTabPageAvailabilityChanged(RefactoringTabPage page, bool isAvailable)
        {
            if (isAvailable == true) {
                RefactoringTabs.Controls.Remove(page);
                RefactoringTabs.Controls.Add(page);
                page.Preview();
            } else {
                RefactoringTabs.Controls.Remove(page);
            }
            ButtonPannel.Enabled = (RefactoringTabs.TabPages.Count > 0);
        }

        /// <summary>
        /// We want the component to behave exactly the same way if the 
        /// execution is started internally or externally, so we just fire 
        /// an event indicating that the Execute button was clicked and
        /// let the container tell us to execute by calling Execute().
        /// The container can also call Execute() at other times if it 
        /// has a menu or toolbar with an Execute command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Execute_Click(object sender, System.EventArgs e)
        {
            FireExecuteClicked();
        }

        /// <summary>
        /// Executes the refactoring defined by the currently selected
        /// tab page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public bool ExecuteCurrentRefactoring()
        {
            bool ret = false;
            if (RefactoringTabs.TabPages[RefactoringTabs.SelectedIndex] is RefactoringTabPage) {
                RefactoringTabPage rtp = RefactoringTabs.TabPages[RefactoringTabs.SelectedIndex] as RefactoringTabPage;
                ret = rtp.Execute();
                m_ViewControler.CodeTreeView.ClearSelectedNodes();
            }
            return ret;
        }

        Hashtable m_TabPages = new Hashtable();

        public CodeDomProvider CodeDomProvider
        {
            get
            {
                return m_ViewControler.CodeDomProvider;
            }
        }

        protected ViewControler m_ViewControler = null;
        private ArrayList refactoringTabPages = new ArrayList();

        /// <summary>
        /// Helper to return a subset of the RefactoringTabPage objects that
        /// are currently displayed in the tab view.
        /// </summary>
        private ArrayList VisibleRefactoringTabPages
        {
            get
            {
                ArrayList ret = new ArrayList();
                foreach (RefactoringTabPage page in refactoringTabPages) {
                    if (RefactoringTabs.Controls.Contains(page)) {
                        ret.Add(page);
                    }
                }
                return ret;
            }
        }

        /// <summary>
        /// Loads the specified refactoring extension at runtime.  Once it is loaded, the
        /// framework will ask it if it should be displayed every time the selected nodes 
        /// change in the tree view and display its tab page if necessary.  Once it is 
        /// visible, it will then be available for execution.
        /// </summary>
        /// <param name="refactoring"></param>
        private void LoadRefactoring(OpnieuwConfiguration.RefactoringRow refactoringRow)
        {
            try {
                Assembly assem = Assembly.Load(refactoringRow.Name);
                Type refactoringGUIType = assem.GetType(refactoringRow.GUIClassName);
                Type refactoringType = assem.GetType(refactoringRow.ClassName);
                Object objGUI = Activator.CreateInstance(refactoringGUIType);
                Object objRefactoring = Activator.CreateInstance(refactoringType);
                if (objGUI is RefactoringGUI) {
                    RefactoringGUI refactoringGUI = objGUI as RefactoringGUI;
                    RefactoringTabPage refactoringTabPage = new RefactoringTabPage(
                        (CodeTreeView)m_ViewControler.CodeTreeView,
                        refactoringGUI, refactoringRow.FriendlyName);
                    refactoringTabPage.ExecuteRefactoring += new ExecuteRefactoringHandler(this.OnChangeCode);
                    try {
                        ResourceManager rm = new System.Resources.ResourceManager("images", assem);
                        RefactoringIcons.Images.Add((Image)rm.GetObject("refactoring.gif"));
                        refactoringTabPage.ImageIndex = RefactoringIcons.Images.Count - 1;
                    } catch (Exception ex) {/*Simply dont use image if it can not be loaded.*/}
                    refactoringTabPage.ToolTipText = refactoringRow.FriendlyName;
                    refactoringTabPage.AvailabilityChanged += new TabPageAvailabilityChangedHandler(this.OnTabPageAvailabilityChanged);
                    refactoringTabPages.Add(refactoringTabPage);
                    if (objRefactoring is Refactoring) {
                        Refactoring refactoring = objRefactoring as Refactoring;
                        refactoringGUI.Connect(refactoring);
                        m_ViewControler.NodeSelectionChanged += new CodeTreeViewEventHandler(refactoring.OnNodeSelectionChanged);
                    }
                }
            } catch (Exception e) {
                System.Windows.Forms.MessageBox.Show(e.Message + e.StackTrace);
            }
        }

        /// <summary>
        /// Loads the refactorings at runtime by parsing the .xml configuration file.
        /// </summary>
        private void LoadRefactorings()
        {
            try {
                ConfigurationManager.Load();
                if (ConfigurationManager.Configuration.RefactoringList.Count == 1) {
                    OpnieuwConfiguration.RefactoringRow[] refactorings = ConfigurationManager.Configuration.RefactoringList[0].GetRefactoringRows();
                    foreach (OpnieuwConfiguration.RefactoringRow refactoring in refactorings) {
                        LoadRefactoring(refactoring);
                    }
                }
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        /// <summary>
        /// Loads all the languages in the configuration file.
        /// </summary>
        private void LoadLanguages()
        {
            TabPage CodeSelectorTabPage = new TabPage();
            CodeSelectorTabPage.ImageIndex = 0;
            CodeSelectorTabPage.ToolTipText = "Code View Selection";

            TabPage TreeSelectorTabPage = new TabPage();
            TreeSelectorTabPage.ImageIndex = 1;
            TreeSelectorTabPage.ToolTipText = "Tree View Selection";

            m_ViewControler = new CSViewControler();
            CSCodeTreeView CodeTreeViewControl = new CSCodeTreeView();
            CodeTreeViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            CodeTreeViewControl.Name = "BrowserTree";
            CodeTreeViewControl.Size = new System.Drawing.Size(320, 608);
            CodeTreeViewControl.TabIndex = 1;
            m_ViewControler.CodeTreeView = CodeTreeViewControl;
            TreeSelectorTabPage.Controls.Add(CodeTreeViewControl);

            CSCodeTabControl CodeTabs = new CSCodeTabControl();
            CodeTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            CodeTabs.Name = "CodeViewTabs";
            CodeTabs.SelectedIndex = 0;
            CodeTabs.Size = new System.Drawing.Size(893, 375);
            CodeTabs.TabIndex = 7;
            m_ViewControler.CodeViewTabs = CodeTabs;
            CodeTabs.StatusChanged += new StatusChangedEventHandler(this.OnStatusChanged);
            CodeSelectorTabPage.Controls.Add(CodeTabs);

            RefactoringTabs.ImageList = this.RefactoringIcons;
            this.RefactoringTabs.TabPages.Add(TreeSelectorTabPage);
            this.RefactoringTabs.TabPages.Add(CodeSelectorTabPage);
            this.RefactoringTabsPannel.Controls.Add(this.RefactoringTabs);
        }

        public void Clear()
        {
            m_ViewControler.Clear();
        }

        public void SaveCurrentFile()
        {
            int index = m_ViewControler.CodeViewTabs.SelectedIndex;
            if (index >= 0) {
                m_ViewControler.CodeViewTabs.SaveTabPage(index);
            }
        }

        #region Form editor stuff
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.Execute = new System.Windows.Forms.Button();
            this.ButtonPannel = new System.Windows.Forms.Panel();
            this.parserProgress = new Opnieuw.Framework.ParserProgress();
            this.RefactoringTabs = new System.Windows.Forms.TabControl();
            this.panel2 = new System.Windows.Forms.Panel();
            this.RefactoringTabsPannel = new System.Windows.Forms.Panel();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.RefactoringIcons = new System.Windows.Forms.ImageList(this.components);
            this.ButtonPannel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(0, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 600);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // Execute
            // 
            this.Execute.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Execute.Location = new System.Drawing.Point(0, 0);
            this.Execute.Name = "Execute";
            this.Execute.Size = new System.Drawing.Size(497, 35);
            this.Execute.TabIndex = 6;
            this.Execute.Text = "Execute";
            this.Execute.Click += new System.EventHandler(this.Execute_Click);
            // 
            // ButtonPannel
            // 
            this.ButtonPannel.Controls.Add(this.Execute);
            this.ButtonPannel.Controls.Add(this.parserProgress);
            this.ButtonPannel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ButtonPannel.Enabled = false;
            this.ButtonPannel.Location = new System.Drawing.Point(0, 565);
            this.ButtonPannel.Name = "ButtonPannel";
            this.ButtonPannel.Size = new System.Drawing.Size(497, 35);
            this.ButtonPannel.TabIndex = 6;
            // 
            // parserProgress
            // 
            this.parserProgress.BackColor = System.Drawing.SystemColors.Control;
            this.parserProgress.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.parserProgress.Location = new System.Drawing.Point(0, 35);
            this.parserProgress.Name = "parserProgress";
            this.parserProgress.Size = new System.Drawing.Size(497, 0);
            this.parserProgress.TabIndex = 7;
            // 
            // RefactoringTabs
            // 
            this.RefactoringTabs.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.RefactoringTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RefactoringTabs.HotTrack = true;
            this.RefactoringTabs.Location = new System.Drawing.Point(0, 378);
            this.RefactoringTabs.Multiline = true;
            this.RefactoringTabs.Name = "RefactoringTabs";
            this.RefactoringTabs.SelectedIndex = 0;
            this.RefactoringTabs.ShowToolTips = true;
            this.RefactoringTabs.Size = new System.Drawing.Size(500, 184);
            this.RefactoringTabs.TabIndex = 4;
            this.RefactoringTabs.SelectedIndexChanged += new System.EventHandler(this.RefactoringTabs_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.RefactoringTabsPannel);
            this.panel2.Controls.Add(this.splitter2);
            this.panel2.Controls.Add(this.ButtonPannel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(497, 600);
            this.panel2.TabIndex = 8;
            // 
            // RefactoringTabsPannel
            // 
            this.RefactoringTabsPannel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RefactoringTabsPannel.Location = new System.Drawing.Point(0, 0);
            this.RefactoringTabsPannel.Name = "RefactoringTabsPannel";
            this.RefactoringTabsPannel.Size = new System.Drawing.Size(497, 562);
            this.RefactoringTabsPannel.TabIndex = 8;
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter2.Enabled = false;
            this.splitter2.Location = new System.Drawing.Point(0, 562);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(497, 3);
            this.splitter2.TabIndex = 7;
            this.splitter2.TabStop = false;
            // 
            // RefactoringIcons
            // 
            this.RefactoringIcons.ImageSize = new System.Drawing.Size(16, 16);
            this.RefactoringIcons.TransparentColor = System.Drawing.Color.White;
            ResourceManager rm = new System.Resources.ResourceManager("images", Assembly.GetExecutingAssembly());
            try {
                this.RefactoringIcons.Images.Add((Image)rm.GetObject("code.gif"));
                this.RefactoringIcons.Images.Add((Image)rm.GetObject("tree.gif"));
            } catch (Exception) { }
            // 
            // Browser
            // 
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.splitter1);
            this.Name = "Browser";
            this.Size = new System.Drawing.Size(500, 600);
            this.ButtonPannel.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        public Button ExecuteButtonControl
        {
            get
            {
                return Execute;
            }
        }

        #region Things that go in ViewControler
        public ArrayList SelectedNodes
        {
            get
            {
                return new ArrayList();
            }
        }

        public Hashtable TreeNodeMapper
        {
            get
            {
                return m_ViewControler.CodeTreeView.TreeNodeMapper;
            }
        }

        /// <summary>
        /// Parses the specified files and fills the tree view with the parsed
        /// nodes.
        /// </summary>
        public void PopulateTree(string[] filenames)
        {
            m_ViewControler.Parse(filenames);
        }

        public void ReloadAllFiles()
        {
            m_ViewControler.ReloadAllFiles();
        }

        #endregion

        /// <summary>
        /// Event handler that gets called when the selected tab page changes.
        /// </summary>
        private void RefactoringTabs_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            ButtonPannel.Enabled = (RefactoringTabs.TabPages.Count > 0);
        }
    }
}
