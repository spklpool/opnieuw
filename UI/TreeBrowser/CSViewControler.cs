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
using System.CodeDom.Compiler;
using Opnieuw.Framework;
using Opnieuw.Parsers.CSParser;

namespace Opnieuw.UI.TreeBrowser
{
	public class CSViewControler : ViewControler
	{
		public CSViewControler() :
			base(new CSModel())
		{
		}

		public override CodeDomProvider CodeDomProvider {
			get {
				return new CSCodeDomProvider();
			}
		}

		public override CodeCompileUnit Parse(string fileName)
		{
			CodeCompileUnit ret = null;
			if (fileName.EndsWith(".cs"))
			{
				TextReader reader = new StreamReader(fileName);
				CodeDomProvider provider = new CSCodeDomProvider();
				ICodeParser p = provider.CreateParser();
				ret = p.Parse(reader);
				CompilationUnit csUnit = ret.UserData["SourceCompilationUnit"] as CompilationUnit;
				csUnit.SourceFilePath = fileName;
				reader.Close();
			}
			return ret;
		}

		protected override void AddCodePage(CodeCompileUnit unit)
		{
			CodeViewTabPage newPage = new CSCodeViewTabPage(unit);
			newPage.SelectionChanged += new SelectionChangedWithInfoEventHandler(this.SelectionChanged);
			RemoveTabPage(FileNameFromPath((string)unit.UserData["SourceFilePath"]));
			CodeViewTabs.AddPage(newPage);
		}
		
		protected override void RemoveFileFromCodeTreeView(string fileName)
		{
			CompilationUnit csUnit = null;
			foreach (CodeCompileUnit unit in Model.CodeCompileUnits)
			{
				if ((string)unit.UserData["SourceFilePath"] == fileName)
				{
					csUnit = (CompilationUnit)unit.UserData["SourceCompilationUnit"];
				}
			}
			if (csUnit != null)
			{
				CodeTreeView.Remove(csUnit);
			}
		}
	}
}
