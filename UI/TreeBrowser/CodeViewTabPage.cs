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
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using Opnieuw.Framework;

namespace Opnieuw.UI.TreeBrowser{	/// <summary>	/// Summary description for CodeViewTabPage.	/// </summary>	public class CodeViewTabPage : TabPage, ICodeViewTabPage	{		public event StatusChangedEventHandler StatusChanged;
		
		public CodeViewTabPage(string fileName, CodeRichTextBox contents)		{
			if (fileName != "")
			{				this.Text = FileNameFromPath(fileName);
				this.Contents = contents;				InitializeComponent();				m_FileName = fileName;				Load();
			}		}
		public event SelectionChangedWithInfoEventHandler SelectionChanged;
		public void FireSelectionChanged(Position selection, string filename) {
			if (SelectionChanged != null) {
				SelectionChanged(selection, filename);
			}
		}

		//TODO: This is copied from ViewControler.  The duplication should eventually
		//be removed.
		public static string FileNameFromPath(string fileName)
		{
			return fileName.Substring(fileName.LastIndexOf("\\") + 1, fileName.Length - fileName.LastIndexOf("\\") - 1);
		}

		public void FireStatusChanged(string newStatus) {
			if (StatusChanged != null) {
				StatusChanged(newStatus);
			}
		}
		public void Load()		{
			VirtualLoad();		}
		public virtual void VirtualLoad()
		{
		}
		public void RTFEncode(string code)		{		}		/// <summary>		/// Colors the keywords and things like that in different colours like		/// most IDE's do.		/// </summary>		public void ColorCode(string code)		{		}		/// <summary>		/// Reads the contents of the file and returns it as a string.		/// </summary>		/// <returns></returns>		public string LoadFile(string filename) 
		{
			System.IO.FileStream fs = null;
			System.IO.StreamReader sr = null;
			fs = System.IO.File.OpenRead(filename);
			sr = new System.IO.StreamReader(fs);
			string ret = sr.ReadToEnd();
			sr.Close();
			return ret;
		}		protected CodeRichTextBox Contents;		protected string m_FileName = "";		public string FileName {			get {				return m_FileName;			}		}		public void Save() 		{			try			{				System.IO.StreamWriter writer = new System.IO.StreamWriter(m_FileName);				writer.Write(Contents.Text.ToCharArray(), 0, Contents.Text.Length);				writer.Close();			}			catch			{				System.Diagnostics.Debug.WriteLine("Save failed!!!!  File might be open???");			}		}		#region Component Designer generated code		/// <summary>		/// Required method for Designer support - do not modify		/// the contents of this method with the code editor.		/// </summary>		private void InitializeComponent()		{			this.SuspendLayout();			// 			// Contents			// 			this.Contents.Dock = System.Windows.Forms.DockStyle.Fill;			this.Contents.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));			this.Contents.Location = new System.Drawing.Point(17, 17);			this.Contents.Name = "Contents";			this.Contents.Size = new System.Drawing.Size(200, 100);			this.Contents.TabIndex = 0;			this.Contents.Text = "";			this.Contents.TextChanged += new System.EventHandler(this.OnTextChanged);			this.Contents.SelectionChanged += new CodeSelectionChangeEventHandler(this.OnSelectionChanged);			this.Contents.StatusChanged += new StatusChangedEventHandler(this.OnStatusChanged);
			this.Contents.SaveRequested += new SaveRequestedEventHandler(this.OnSaveRequested);
			// 			// CodeViewTabPage			// 			this.Controls.AddRange(new System.Windows.Forms.Control[] {																		  this.Contents});			this.ResumeLayout(false);		}		#endregion		public bool m_Selecting = false;		public void OnSelectionChanged(Position position)		{			FireSelectionChanged(position, FileName);		}		
		public void OnStatusChanged(string newStatus)
		{
			FireStatusChanged(newStatus);
		}		
		public void OnSaveRequested()
		{
			Save();
		}
		
		public void SelectFromPosition(Position p)		{			Contents.ResetSelection(new CodeSelection(p.StartLine, p.StartCol, p.EndLine, p.EndCol));		}
		public void OnTextChanged(object sender, System.EventArgs e)		{		}	}}
