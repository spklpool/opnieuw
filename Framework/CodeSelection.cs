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

namespace Opnieuw.Framework
{
	public class CodeSelection : Position
	{
		public CodeSelection(CodeSelection selection) :
		base(selection.StartLine, selection.StartCol, selection.EndLine, selection.EndCol)
		{
			m_IsCursorAtStart = selection.IsCursorAtStart;
		}
		
		public CodeSelection(int startLine, int startCol, int endLine, int endCol) :
			base(startLine, startCol, endLine, endCol)
		{
		}

		protected bool m_IsCursorAtStart = true;
		public bool IsCursorAtEnd {
			get {
				return m_IsCursorAtStart;
			}
			set {
				m_IsCursorAtStart = value;
			}
		}
		public bool IsCursorAtStart {
			get {
				return !m_IsCursorAtStart;
			}
			set {
				m_IsCursorAtStart = !value;
			}
		}
	}
}
