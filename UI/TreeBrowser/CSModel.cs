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
	public class CSModel : Model
	{
		public override Code CreateCode(CodeCompileUnit unit)
		{
			CompilationUnit csUnit = unit.UserData["SourceCompilationUnit"] as CompilationUnit;
			return new CSCode(csUnit.Generate());
		}

		public override Code CreateCode(string fileName)
		{
			TextReader reader = new StreamReader(fileName);
			Code ret = new CSCode(reader.ReadToEnd());
			reader.Close();
			return ret;
		}

		public override CodeCompileUnit Parse(string fileName)
		{
			CodeCompileUnit ret = null;
			try {
				if (fileName.EndsWith(".cs"))
				{
					TextReader reader = new StreamReader(fileName);
					CodeDomProvider provider = new CSCodeDomProvider();
					ICodeParser p = provider.CreateParser();
					ret = p.Parse(reader);
					CompilationUnit csUnit = ret.UserData["SourceCompilationUnit"] as CompilationUnit;
					ret.UserData.Add("SourceFilePath", fileName);
					//One of the followint two lines is needed.  The first one creates
					//the Code object with the text form the file.  The second one 
					//creates the Code object by regenerating the code form the parse tree.
					//ret.UserData["Code"] = CreateCode(fileName);
					ret.UserData["Code"] = CreateCode(ret);
					csUnit.SourceFilePath = fileName;
					reader.Close();
				}
			} catch (Exception e) {
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
			}
			return ret;
		}

		public override void SelectNodesByPositionAndFile(Position position, string fileName)
		{
			m_SelectedNodes.Clear();
			foreach (CodeCompileUnit unit in CodeCompileUnits)
			{
				CompilationUnit csUnit = unit.UserData["SourceCompilationUnit"] as CompilationUnit;
				if (csUnit.SourceFilePath.EndsWith(fileName))
				{
					Position adjustedPosition = ((Code)unit.UserData["Code"]).UnexpandTabs(position);
					foreach (NamespaceMember member in csUnit.Members) 
					{
						PieceOfCode poc = member as PieceOfCode;
						CheckPieceOfCode(poc, adjustedPosition);
					}
				}
			}
			FireNodeSelectionChanged(SelectedNodes);
		}

		public override Position StripTabs(Position position, string fileName)
		{
			Position ret = null;
			foreach (CodeCompileUnit unit in CodeCompileUnits)
			{
				CompilationUnit csUnit = unit.UserData["SourceCompilationUnit"] as CompilationUnit;
				if (csUnit.SourceFilePath.EndsWith(fileName))
				{
					ret = ((Code)unit.UserData["Code"]).UnexpandTabs(position);
				}
			}
			return ret;
		}

		public void CheckPieceOfCode(PieceOfCode poc, Position position)
		{
			if (m_SelectedNodes.Contains(poc) == false)
			{
				if ((position.StartLine == position.EndLine) &&
					(position.StartCol == position.EndCol) &&
					(poc is Identifier) &&
					(poc.Position.ContainsInclusive(position)))
				{
					m_SelectedNodes.Add(poc);
				}
				else
				{
					if ((position.ContainsInclusive(poc.Position)))
					{
						m_SelectedNodes.Add(poc);
					}
					else
					{
						foreach (PieceOfCode child in poc.Children)
						{
							CheckPieceOfCode(child, position);
						}
					}
				}
			}
		}
	}
}
