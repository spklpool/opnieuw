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

namespace Opnieuw.UI.TreeBrowser
		
		public CodeViewTabPage(string fileName, CodeRichTextBox contents)
			if (fileName != "")
			{
				this.Contents = contents;
			}
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

			VirtualLoad();
		public virtual void VirtualLoad()
		{
		}

		{
			System.IO.FileStream fs = null;
			System.IO.StreamReader sr = null;
			fs = System.IO.File.OpenRead(filename);
			sr = new System.IO.StreamReader(fs);
			string ret = sr.ReadToEnd();
			sr.Close();
			return ret;
		}
			this.Contents.SaveRequested += new SaveRequestedEventHandler(this.OnSaveRequested);
			// 
		public void OnStatusChanged(string newStatus)
		{
			FireStatusChanged(newStatus);
		}
		public void OnSaveRequested()
		{
			Save();
		}
		
		public void SelectFromPosition(Position p)
		public void OnTextChanged(object sender, System.EventArgs e)