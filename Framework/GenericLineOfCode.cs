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
	public class GenericLineOfCode
	{
		public GenericLineOfCode()
		{
		}

        public static bool IsBlank(string line)
        {
            for (int i = (line.Length - 1); i >= 0; i--)
            {
                if ((line[i] != ' ') && 
                    (line[i] != '\t') &&
                    (line[i] != '\r') &&
                    (line[i] != '\n'))
                {
                    return false;
                }
            }
            return true;
        }

        public static string TrimIndent(string line, int spaceCount, int tabSize)
		{
			string ret = "";
			int trimCount = 0;

			for (int i=0; i<line.Length; i++)
			{
				if (trimCount < spaceCount)
				{
					if ((line[i] != '\t') || (line[i] != ' '))
					{
						if (line[i] == '\t')
						{
							trimCount += tabSize;
						}
						if (line[i] == ' ')
						{
							trimCount ++;
						}
					}
					else
					{
						break;
					}

					//If we went overboard by trimming a tab, replace
					//spaces to make up the difference.
					while (trimCount > spaceCount)
					{
						ret += ' ';
						trimCount --;
					}
				}
				else
				{
					ret += line[i];
				}
			}

			return ret;
		}

		public static int CountCharacters(string text, int tabSize)
		{
			int ret = 0;
			for (int i=0; i<text.Length; i++) {
				if (text[i] == '\t') {
					ret += tabSize;
				} else {
					ret ++;
				}
			}
			return ret;
		}

		public static string GetIndent(string codeLine, int tabSize)
		{
			string ret = "";
			int spaceCount = 0;

			//Find out how many leading spaces we have
			for (int i=0; i<codeLine.Length; i++)
			{
				if ((codeLine[i] == ' ') || (codeLine[i] == '\t'))
				{
					if (codeLine[i] == ' ')
					{
						spaceCount ++;
					}
					if (codeLine[i] == '\t')
					{
						spaceCount += tabSize;
					}
				}
				else
				{
					break;
				}
			}

			int filledCount = 0;

			//Fill the needed space with as many tabs as possible
			while ((spaceCount - filledCount) >= tabSize)
			{
				ret += '\t';
				filledCount += tabSize;
			}

			//Fill the rest with spaces
			while (spaceCount > filledCount)
			{
				ret += ' ';
				filledCount ++;
			}

			return ret;
		}

		public static string AddLeadingSpaces(string line, int spacesToAdd, int tabSize)
		{
			string padding = "";
			int padCount = 0;
			while (padCount < spacesToAdd)
			{
				if ((spacesToAdd - padCount) >= tabSize)
				{
					padding += '\t';
					padCount += tabSize;
				}
				else
				{
					padding += ' ';
					padCount ++;
				}
			}
			return padding + line;
		}

		public static string RemoveLeadingSpaces(string line, int spacesToRemove, int tabSize)
		{
			string ret = line;
			int spacesRemoved = 0;
			int i = 0;
			while (((line[i] == ' ') || (line[i] == '\t')) && (spacesRemoved <= spacesToRemove))
			{
				ret = ret.Substring(1, ret.Length - 1);
				if (line[i] == '\t')
				{
					spacesRemoved += tabSize;

					//Add some spaces back if we went overboard by removing a tab
					if (spacesToRemove < spacesRemoved)
					{
						ret = AddLeadingSpaces(ret, (spacesRemoved - spacesToRemove), tabSize);
					}
				}
				else if (line[i] == ' ')
				{
					spacesRemoved ++;
				}
				i++;
			}
			return ret;
		}

		public static string Reindent(string line, int reindentSpaces, int tabSize)
		{
			string ret = line;
			if (line != "\r\n")
			{
				if (reindentSpaces == 0)
				{
					ret = line;
				}
				else if (reindentSpaces < 0)
				{
					ret = RemoveLeadingSpaces(line, System.Math.Abs(reindentSpaces), tabSize);
				}
				else if (reindentSpaces > 0)
				{
					ret = AddLeadingSpaces(line, reindentSpaces, tabSize);
				}
			}
			return ret;
		}
	}
}
