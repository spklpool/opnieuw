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
using Opnieuw.Parsers.CSParser;

namespace Opnieuw.Framework
{
	public class CSCode : Code
	{
		/// <summary>
		/// Standarc constructor.
		/// </summary>
		/// <param name="text"></param>
		public CSCode(string text) :
			base(text)
		{
		}

		/// <summary>
		/// Sets this object's text from the compile unit
		/// that was given in the constructor.
		/// </summary>
		protected override void SetTextFromCompileUnit()
		{
			CompilationUnit csUnit = m_CompileUnit.UserData["SourceCompilationUnit"] as CompilationUnit;
			m_Text = csUnit.Generate();
		}

		/// <summary>
		/// Override form Code to create CSCodeLine objects.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		protected override CodeLine CreateLine(string text)
		{
			char[] CHARS_TO_TRIM = new char[2] {'\r', '\n'};
			return new CSCodeLine(text.Trim(CHARS_TO_TRIM));
		}
	}
}
