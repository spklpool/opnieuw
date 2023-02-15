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
	public class CodeReplacement
	{
		public CodeReplacement(Position position, string newCode)
		{
			m_Position = position;
			m_NewCode = newCode;
		}

		protected Position m_Position = Position.Missing;
		/// <summary>
		/// The position of the code to be replaced.
		/// </summary>
		public Position Position {
			get {
				return m_Position;
			}
		}

		/// <summary>
		/// The new code to be inserted at the specified position.
		/// </summary>
		protected string m_NewCode = "";
		public string NewCode {
			get {
				return m_NewCode;
			}
		}
	}

	class MissingCodeReplacement : CodeReplacement
	{
		public MissingCodeReplacement() :
			base(Position.Missing, "")
		{
		}
	}
}
