#region Copyright (C) Pierre Boudreau
//This file is part of the Opnieuw project.
//
//pierreopnieuw.com
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

using Gtk;
using Gdk;
using System;
using System.Drawing;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using Opnieuw.Parsers.CSParser;

namespace Opnieuw.Framework
{
	public class RefactoringBrowser
	{
		private Gtk.Window m_Window = null;
		private ViewControler m_ViewControler = null;
		private TreeStore store = null;
		private FileSelection filew = null;
		private HPaned m_Splitter = null;
		private Notebook m_Notebook = null;
		private VBox m_RightBox = null;

		public RefactoringBrowser()
		{
			m_ViewControler = new CSViewControler();
			initWindow();
			RegisterEventListeners();
		}

		private void initWindow()
		{
			m_Window = new Gtk.Window ("Opnieuw");
			m_Window.DeleteEvent += new DeleteEventHandler (Window_Delete);
			m_Window.DefaultSize = new Size(200, 150);
			VBox box = new VBox (false, 2);

			//Add a menu
			MenuBar mb = new MenuBar ();
			Menu file_menu = new Menu ();
			MenuItem exit_item = new MenuItem("Exit");
			exit_item.Activated += new EventHandler (File_Exit);
			file_menu.Append (exit_item);
			MenuItem open_item = new MenuItem("Open");
			open_item.Activated += new EventHandler (File_Open);
			file_menu.Append (open_item);
			MenuItem file_item = new MenuItem("File");
			file_item.Submenu = file_menu;
			mb.Append (file_item);
			box.PackStart(mb, false, false, 0);
			m_Window.Add (box);

			//Add a 2x2 table for the tree view and code view
			m_Splitter = new HPaned ();

			//Add a scrolling enabled TreeView
			ScrolledWindow sw = new ScrolledWindow ();
			store = new TreeStore (typeof (string), typeof (string));
			TreeView tv = new TreeView (store);
			tv.HeadersVisible = false;
			tv.AppendColumn ("", new CellRendererText (), "text", 0);
			sw.Add (tv);
			m_Splitter.Add1(sw);
			box.Add (m_Splitter);

			//Add a tab view with text boxes
			m_Notebook = new Notebook();
			m_Notebook.TabPos = PositionType.Top;
			m_Notebook.Scrollable = true;
			m_RightBox = new VBox(false, 0);
			m_RightBox.PackStart (m_Notebook);
			Button executeButton = new Button("Execute");
			m_RightBox.PackStart(executeButton, false, false, 0);
			m_Splitter.Add2(m_RightBox);

			//Display the whole thing
			m_Window.ShowAll ();
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
		}

		/// <summary>
		/// This gets called when parsing starts on one or more files.
		/// </summary>
		/// <param name="fileCount"></param>
		private void OnParsingStarted(int fileCount)
		{
		}

		/// <summary>
		/// This gets called when parsing the specified file.
		/// </summary>
		/// <param name="fileName"></param>
		private void OnParsingFile(string fileName)
		{
		}

		/// <summary>
		/// This gets called when parsing of the specified file
		/// is finished.
		/// </summary>
		/// <param name="fileName"></param>
		private void OnParsingFileDone(string fileName, CodeCompileUnit unit, bool success)
		{
			PopulateTree(unit);
			AddCodePage(unit);
		}

		/// <summary>
		/// This gets called when paraing is done.
		/// </summary>
		private void OnParsingDone()
		{
			LoadTree();
		}

		protected CompilationUnitCollection m_CompilationUnits = new CompilationUnitCollection();
		public void PopulateTree(CodeCompileUnit unit)
		{
			if (unit != null)
			{
				CompilationUnit csUnit = unit.UserData["SourceCompilationUnit"] as CompilationUnit;
				if (false == (csUnit is MissingCompilationUnit))
				{
					m_CompilationUnits.Add(csUnit);
				}
			}
		}

		private void LoadTree()
		{
			store.Clear();
			foreach (PieceOfCode poc in m_CompilationUnits.NamespaceMembers)
			{
				TreeIter root = store.AppendValues (poc.AsText);
				foreach (PieceOfCode child in poc.Children) {
					DisplayNode(child, root);
				}
			}
		}

		private void DisplayNode(PieceOfCode poc, TreeIter parent)
		{
			TreeIter childNode = store.AppendValues (parent, poc.AsText);
			foreach (PieceOfCode child in poc.Children)
			{
				DisplayNode(child, childNode);
			}
		}

		private string FileNameFromPath(string fileName)
		{
			return fileName.Substring(fileName.LastIndexOf("/") + 1, fileName.Length - fileName.LastIndexOf("/") - 1);
		}

		private Gtk.TextBuffer textBuffer;
		private TextView textView;
		private Label label;
		private void AddCodePage(CodeCompileUnit unit)
		{
			if (unit != null)
			{
				CompilationUnit csUnit = unit.UserData["SourceCompilationUnit"] as CompilationUnit;
				if (false == (csUnit is MissingCompilationUnit))
				{
					string filePath = (string) unit.UserData["SourceFilePath"];
					label = new Label (FileNameFromPath(filePath));
					ScrolledWindow sw = new ScrolledWindow ();
					textView = new TextView();
					textBuffer = textView.Buffer;
					textBuffer.Text = LoadFile(filePath);
					sw.Add (textView);
					m_Notebook.AppendPage (sw, label);
					m_Notebook.ShowAll ();
				}
			}
		}

		/// <summary>
		/// Reads the contents of the file and returns it as a string.
		/// </summary>
		/// <returns></returns>
		protected string LoadFile(string filename) 
		{
			System.IO.FileStream fs = null;
			System.IO.StreamReader sr = null;
			fs = System.IO.File.OpenRead(filename);
			sr = new System.IO.StreamReader(fs);
			string ret = sr.ReadToEnd();
			sr.Close();
			return ret;
		}

		public static int Main (string[] args)
		{
			Application.Init ();
			RefactoringBrowser me = new RefactoringBrowser();
			Application.Run ();
			return 0;
		}

		void Window_Delete (object obj, DeleteEventArgs args)
		{
			Application.Quit ();
			args.RetVal = true;
		}

		void File_Exit (object o, EventArgs args)
		{
			Application.Quit ();
		}

		void file_ok_sel_event( object obj,EventArgs args )
		{
			m_ViewControler.Parse(filew.Selections);
			filew.Hide();
			filew = null;
		}

		void file_cancel_sel_event( object obj,EventArgs args )
		{
			filew.Hide();
			filew = null;
		}

		void File_Open (object o, EventArgs args)
		{
			filew = new FileSelection("File selection");
			filew.Filename = "somefile.cs";
			filew.SelectMultiple = true;
			filew.OkButton.Clicked +=new EventHandler(file_ok_sel_event);
			filew.CancelButton.Clicked +=new EventHandler(file_cancel_sel_event);
			filew.Show();
		}
	}
}
