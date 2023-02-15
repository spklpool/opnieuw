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

		public ColoredStringPart(int offset, string text) :
		base(text)
		{
			m_Offset = offset;
			InitializeExtendedLength();
        }

        public ColoredStringPart(int offset, string text, Color stringColor) :
		base(text)
        {
            m_Offset = offset;
            m_StringColor = stringColor;
            InitializeExtendedLength();
        }

        public ColoredStringPart(int offset, string text, Color stringColor, Color backColor) :
		base(text)
		{
			m_Offset = offset;
			m_StringColor = stringColor;
			BackColor = backColor;
			InitializeExtendedLength();
		}

		private void InitializeExtendedLength()
		{
			int tempExtendedLength = m_Offset;
			for(int i=0; i<Text.Length; i++)
			{
				if (Text[i] == '\t')
				{
					tempExtendedLength += (4-(tempExtendedLength%4));
				}
				else
				{
					tempExtendedLength ++;
				}
			}
			m_ExtendedLength = tempExtendedLength - m_Offset;
		}
	}