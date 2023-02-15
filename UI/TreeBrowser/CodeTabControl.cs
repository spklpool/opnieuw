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
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;
using Opnieuw.Framework;
using Opnieuw.Parsers.CSParser;

namespace Opnieuw.UI.TreeBrowser
{
	public class CodeTabControl : TabControl, ICodeTabControl
	{
		public event StatusChangedEventHandler StatusChanged;
				
		public CodeTabControl()
		{
			InitializeComponent();
		}

		private void FireStatusChanged(string newStatus) {
			if (StatusChanged != null) {
				StatusChanged(newStatus);
			}
		}

		public void OnStatusChanged(string newStatus)
		{
			FireStatusChanged(newStatus);
		}

		public virtual void Select(ArrayList selectedNodes)
		{
		}

		public void RemovePage(string name)
		{
		}

		public void AddPage(ICodeViewTabPage page)
		{
			CodeViewTabPage codePage = page as CodeViewTabPage;
			codePage.StatusChanged += new StatusChangedEventHandler(this.OnStatusChanged);
			TabPages.Add(codePage);
			codePage.Select();
		}

		public void ClearPages()
		{
			TabPages.Clear();
		}

		public void SaveTabPage(int index)
		{
			TabPage page = TabPages[index];
			if (page != null)
			{
				CodeViewTabPage codePage = page as CodeViewTabPage;
				codePage.Save();
			}
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.HotTrack = true;
		}
		#endregion
	}
}
