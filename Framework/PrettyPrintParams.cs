#region Copyright (C) Pierre Boudreau
//This file is part of the Opnieuw C# parser.
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

namespace Opnieuw.Framework
{
	/// <summary>
	/// This is container to hold parameters used by the 
	/// pretty printer.
	/// </summary>
	public class PrettyPrintParams
	{
		public PrettyPrintParams(int indentLevel)
		{
			m_IndentLevel = indentLevel;
		}
		
		public PrettyPrintParams(int indentLevel, bool reformat)
		{
			m_IndentLevel = indentLevel;
			m_Reformat = reformat;
		}

		protected int m_IndentLevel = 0;
		public int IndentLevel {
			get {
				return m_IndentLevel;
			}
		}

		protected int m_TabWidth = 4;
		public int TabWidth {
			get {
				return m_TabWidth;
			}
		}

		protected bool m_Reformat = false;
		public bool Reformat {
			get {
				return m_Reformat;
			}
		}

		public string Reindent(string source)
		{
			return GenericBlockOfCode.Reindent(source, TabWidth * IndentLevel, TabWidth);
		}
	}
}
