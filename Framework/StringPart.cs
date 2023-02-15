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
using System.Drawing;

namespace Opnieuw.Framework
{
	public class StringPart
	{
		public StringPart(string text)
		{
			m_Text = text;
		}
		
		/// <summary>
		/// Returns the text of this line.
		/// </summary>
		protected string m_Text = "";
		public virtual string Text {
			get {
				return m_Text;
			}
			set {
				char[] CHARS_TO_TRIM = new char[2] {'\r', '\n'};
				m_Text = value.TrimEnd(CHARS_TO_TRIM);
			}
		}
		
		public string Substring(int start, int length)
		{
			string ret = "";
			int realPosition = 0;
			int expandedPosition = 0;
			int end = start+length;
			while (realPosition<Text.Length)
			{
				if (Text[realPosition] == '\t') 
				{
					int positionBeforeExpansion = expandedPosition;
					//Console.WriteLine("Tab adjustment: " + (4-(expandedPosition%4)));
					expandedPosition += (4-(expandedPosition%4));
					if ((positionBeforeExpansion>=start) && (positionBeforeExpansion<end))
					{ 
						ret += Text[realPosition];
					}
					else if ((start>=positionBeforeExpansion) && 
							 (start<expandedPosition) && 
							 (length!=0))
					{
						ret += Text[realPosition];
					}
				} 
				else 
				{
					if ((expandedPosition>=start) && (expandedPosition<end)) 
					{
						ret += Text[realPosition];
					}
					expandedPosition ++;
				}
				realPosition ++;
			}
			//Console.WriteLine("Looking for substring (" + start + "," + length + ") of [" + Text + "] and got [" + ret + "]");
			return ret;
		}
	}
}