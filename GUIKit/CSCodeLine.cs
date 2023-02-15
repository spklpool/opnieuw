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
using System.Drawing;
using Opnieuw.Parsers.CSParser;

namespace Opnieuw.Framework
{
	public class CSCodeLine : CodeLine
	{
		/// <summary>
		/// Standard constructor.
		/// </summary>
		/// <param name="text"></param>
		public CSCodeLine(string text) :
			base(text)
		{
		}

		/// <summary>
		/// Splits up the line into parts that have assiciated colors up until
		/// the specified column.  This should be overriden for different languages
		/// so that the colors can reflect keywords in the language.
		/// </summary>
		public override ColoredStringPartCollection PartsUpTo(int columnIndex)
		{
			string whatWasIDoing = "Parts up to index " + columnIndex + " of " + Text;
			ColoredStringPartCollection ret = new ColoredStringPartCollection();
			try
			{
				int lastStartingPoint = 0;
				Tokenizer t = new Tokenizer(new StringReader(m_Text), "", null);
				PositionTokenCollection tokens = t.ScanAllTokens();
				foreach (PositionToken token in tokens)
				{
					//exit criteria
					if (token.Position.StartCol > columnIndex) break;

					//keywords in blue
					if (t.is_keyword(token.Text))
					{
						string stringToAdd = m_Text.Substring(lastStartingPoint, token.Position.StartCol - 1 - lastStartingPoint);
						if(stringToAdd != "")
						{
							int substringLength = Math.Min(stringToAdd.Length, columnIndex - lastStartingPoint - 1);
							if (substringLength > 0)
							{
								stringToAdd = stringToAdd.Substring(0, substringLength);
								ret.Add(new ColoredStringPart(0, stringToAdd, Color.Black));
								lastStartingPoint += stringToAdd.Length;
							}
						}
						if ((columnIndex - lastStartingPoint) > 0)
						{
							int substringLength = Math.Min(token.Text.Length, columnIndex - lastStartingPoint);
							if (substringLength > 0)
							{
                                ret.Add(new ColoredStringPart(token.Position.StartCol - 1, m_Text.Substring(token.Position.StartCol - 1, substringLength), Color.Blue));
                                lastStartingPoint += substringLength;
							}
						}
					} 

					//comments in green
					if (token.Value is Comment) 
					{
						string stringToAdd = m_Text.Substring(lastStartingPoint, token.Position.StartCol - 1 - lastStartingPoint);
						if(stringToAdd != "")
						{
							int substringLength = Math.Min(stringToAdd.Length, columnIndex - lastStartingPoint - 1);
							if (substringLength > 0)
							{
								stringToAdd = stringToAdd.Substring(0, substringLength);
								ret.Add(new ColoredStringPart(0, stringToAdd, Color.Black));
								lastStartingPoint += stringToAdd.Length;
							}
						}
						if ((columnIndex - lastStartingPoint) > 0)
						{
							Comment comment = token.Value as Comment;
							int substringLength = Math.Min(comment.Text.Length, columnIndex - lastStartingPoint);
							if (substringLength > 0)
							{
                                ret.Add(new ColoredStringPart(token.Position.StartCol - 1, m_Text.Substring(token.Position.StartCol - 1, substringLength), Color.Green));
                                lastStartingPoint += substringLength;
							}
						}
					}
				}

				//add anything remaining up to specified index
				if (columnIndex >= lastStartingPoint)
				{
					string rest = m_Text.Substring(lastStartingPoint, columnIndex - lastStartingPoint);
					if (rest != "")
					{
                        ret.Add(new ColoredStringPart(lastStartingPoint, rest, Color.Black));
                    }
				}
			}
			catch (Exception e)
			{
				System.Windows.Forms.MessageBox.Show(whatWasIDoing + "\r\n" + e.Message + "\r\n" + e.StackTrace);
			}
			return ret;
		}
	}
}
