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
using System.CodeDom;
using System.Threading;
using System.Drawing;
using Opnieuw.Framework;

namespace Opnieuw.UI.TreeBrowser
{
	public class CSCodeViewTabPage : CodeViewTabPage
	{
		public CSCodeViewTabPage(CodeCompileUnit unit) :
			base((string)unit.UserData["SourceFilePath"], new CodeRichTextBox((CSCode)unit.UserData["Code"]))
		{
		}

		public override void VirtualLoad()
		{
			//Contents.Text = LoadFile(m_FileName);
		}
	}
}
