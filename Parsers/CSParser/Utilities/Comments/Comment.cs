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
using Opnieuw.Framework;

namespace Opnieuw.Parsers.CSParser
{
	/// <summary>
	/// This class represents a comment in code.
	/// </summary>
	public class Comment : PieceOfCode
	{
		/// <summary>
		/// Internal member to keep the text.
		/// </summary>
		protected string m_Text = "";

		/// <summary>
		/// Constructor that initializes all fields.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="position"></param>
		public Comment(string text, Position position)
		{
			char[] CHARS_TO_TRIM = new char[2] {'\r', '\n'};
			m_Text = text.TrimEnd(CHARS_TO_TRIM);
		}

		/// <summary>
		/// Constructor that initializes the field that
		/// is really necessary. 
		/// </summary>
		/// <param name="text"></param>
		/// <param name="position"></param>
		public Comment(string text)
		{
			m_Text = text;
		}

		/// <summary>
		/// Returns the text contained in the comment.
		/// </summary>
		public virtual string Text {
			get {
				return m_Text;
			}
		}

        public virtual string ContainedText {
            get {
                return m_Text;
            }
        }

		public override string Generate()
		{
			System.Text.StringBuilder ret = new System.Text.StringBuilder();
			ret.Append(Text);
			return ret.ToString();
		}
	}
}
