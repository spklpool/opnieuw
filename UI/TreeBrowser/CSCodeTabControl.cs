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
using Opnieuw.Framework;
using Opnieuw.Parsers.CSParser;

namespace Opnieuw.UI.TreeBrowser
{
	public class CSCodeTabControl : CodeTabControl
	{
		public override void Select(ArrayList selectedNodes)
		{
			string name = "";
			Position newPosition = new InvalidPosition();
			foreach (PieceOfCode poc in selectedNodes)
			{
				string fileName = poc.ParentCompilationUnit.SourceFilePath;
				string currentName = fileName.Substring(fileName.LastIndexOf("\\") + 1, fileName.Length - fileName.LastIndexOf("\\") - 1);
				if (name == "") {
					name = currentName;
				}
				if (name == currentName) {
					if (newPosition is InvalidPosition) {
						newPosition = new Position(poc.Position);
					} else {
						newPosition = new Position(newPosition, poc.Position);
					}
				}
			}
			foreach (TabPage page in this.TabPages)
			{
				if (page.Text == name)
				{
					CSCodeViewTabPage codePage = page as CSCodeViewTabPage;
					codePage.SelectFromPosition(newPosition);
					SelectedTab = page;
					break;
				}
			}
		}
	}
}
