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
	public class ParserException : Exception
	{
		public ParserException()
		{
		}

		public ParserException(string text)
		{
			m_Text = text;
		}

		public ParserException(int expected, PositionToken actual)
		{
			m_Expected = expected;
			m_Actual = actual;
		}

		protected string m_Text = "";
		public string Text {
			get {
				return m_Text;
			}
		}

		protected int m_Expected = Token.NONE;
		public int Expected {
			get {
				return m_Expected;
			}
		}

		protected PositionToken m_Actual = new PositionToken(Position.Missing, null, Token.NONE);
		public PositionToken Actual {
			get {
				return m_Actual;
			}
		}
	}
}
