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
	public class GenericBlockOfCode
	{
		public GenericBlockOfCode()
		{
		}

        public static int GetIndent(string block, int tabSize)
        {
            string lastLine = "";
            int ret = block.Length;
            for (int i = 0; i < block.Length; i++)
            {
                lastLine += block[i];
                if ((block[i] == '\r') || (block[i] == '\n') || (i == (block.Length - 1)))
                {
                    while ((block.Length > (i + 1)) &&
                           ((block[i + 1] == '\r') || (block[i + 1] == '\n')))
                    {
                        i++;
                        lastLine += block[i];
                    }
                    string indentString = GenericLineOfCode.GetIndent(lastLine, tabSize);
                    if (!GenericLineOfCode.IsBlank(lastLine)) {
                        int indent = GenericLineOfCode.CountCharacters(indentString, tabSize);
                        if (indent < ret) {
                            ret = indent;
                        }
                    }
                    lastLine = "";
                }
            }
            return ret;
        }

        public static string RemoveIndent(string block, int tabSize)
        {
            int indentToRemove = GetIndent(block, tabSize);
            string ret = "";
            string lastLine = "";
            for (int i = 0; i < block.Length; i++)
            {
                lastLine += block[i];
                if ((block[i] == '\r') || (block[i] == '\n') || (i == (block.Length - 1)))
                {
                    while ((block.Length > (i + 1)) &&
                           ((block[i + 1] == '\r') || (block[i + 1] == '\n')))
                    {
                        i++;
                        lastLine += block[i];
                    }
                    ret += GenericLineOfCode.Reindent(lastLine, -indentToRemove, tabSize);
                    lastLine = "";
                }
            }
            return ret;
        }

		public static string Reindent(string block, int reindentSpaces, int tabSize)
		{
			string ret = "";
			string lastLine = "";
			for(int i=0; i<block.Length; i++)
			{
				lastLine += block[i];
				if ((block[i] == '\r') || (block[i] == '\n') || (i == (block.Length - 1)))
				{
					while ((block.Length > (i+1)) &&
						   ((block[i+1] == '\r') || (block[i+1] == '\n')))
					{
						i++;
						lastLine += block[i];
					}
					ret += GenericLineOfCode.Reindent(lastLine, reindentSpaces, tabSize);
					lastLine = "";
				}
			}
			return ret;
		}
	}
}
