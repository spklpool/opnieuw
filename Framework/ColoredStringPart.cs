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
using System;using System.Drawing;
namespace Opnieuw.Framework{	public class ColoredStringPart : StringPart	{		private int m_Offset = 0;

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
		protected Color m_StringColor = Color.Black;		public Color StringColor {			get {				return m_StringColor;			}			set {				m_StringColor = value;			}		}		protected Color m_BackColor = Color.White;		public Color BackColor {			get {				return m_BackColor;			}			set {				m_BackColor = value;			}		}				protected int m_ExtendedLength = 0;		public int ExtendedLength {			get {				return m_ExtendedLength;			}		}		public string ExpandedString {			get {				string ret = "";				for (int i=0; i<Text.Length; i++)				{					if (Text[i] == '\t')					{						ret += "    ";					}					else					{						ret += Text[i];					}				}				return ret;			}		}		public ColoredStringPartCollection Split(int offset, int index1, int index2)		{			//Console.WriteLine("Splitting " + Text + " (" + index1 + "," + index2 + ")");			if (index2 < index1)			{				int temp = index1;				index1 = index2;				index2 = temp;			}			if (index2 > (ExtendedLength+1))			{				index2 = ExtendedLength+1;			}			ColoredStringPartCollection ret = new ColoredStringPartCollection();			string part1 = Substring(0, index1-1);			if (part1 != "")			{				//Console.WriteLine("part1: " + part1);				ret.Add(new ColoredStringPart(offset, part1, StringColor, BackColor));			}			string part2 = Substring(index1-1, index2 - index1);			if (part2 != "")			{				//Console.WriteLine("part2: " + part2);				ret.Add(new ColoredStringPart(offset, part2, StringColor, BackColor));			}			string part3 = Substring(index2-1, ExtendedLength-(index2-1));			if (part3 != "")			{				//Console.WriteLine("part3: " + part3);				ret.Add(new ColoredStringPart(offset, part3, StringColor, BackColor));			}			return ret;		}		public ColoredStringPartCollection SplitForSelection(Position selection, int line, int offset, Color selectedStringColor, Color selectedBackColor)		{			ColoredStringPartCollection ret = new ColoredStringPartCollection();			if ((selection.StartLine < line) && (selection.EndLine > line))			{				ret.Add(new ColoredStringPart(offset, Text, selectedStringColor, selectedBackColor));			}			else if ((selection.StartLine == line) && (selection.EndLine == line))			{				int start = Math.Max(0, selection.StartCol - offset);				int end = Math.Max(0, selection.EndCol - offset);				ret = Split(offset, start, end);				if (ret.Count == 3)				{					ret[1].StringColor = selectedStringColor;					ret[1].BackColor = selectedBackColor;				}				else if (ret.Count == 2)				{					if ((selection.StartCol-1) <= offset)					{						ret[0].StringColor = selectedStringColor;						ret[0].BackColor = selectedBackColor;					}					else					{						ret[1].StringColor = selectedStringColor;						ret[1].BackColor = selectedBackColor;					}				}				else				{					if (((selection.StartCol-1) <= offset) && 						((selection.EndCol-1) >= (offset + Text.Length)))					{						ret[0].StringColor = selectedStringColor;						ret[0].BackColor = selectedBackColor;					}				}			}			else if (selection.StartLine == line)			{				if ((selection.StartCol-1) <= offset)				{					ret.Add(new ColoredStringPart(offset, Text, StringColor, BackColor));					ret[0].StringColor = selectedStringColor;					ret[0].BackColor = selectedBackColor;				}				else				{					int start = Math.Max(0, selection.StartCol - offset);					ret = Split(offset, start, start);					if (ret.Count == 2)					{						ret[1].StringColor = selectedStringColor;						ret[1].BackColor = selectedBackColor;					}				}			}			else if (selection.EndLine == line)			{				int end = Math.Max(0, selection.EndCol - offset);				ret = Split(offset, end, end);				if (ret.Count == 2)				{					ret[0].StringColor = selectedStringColor;					ret[0].BackColor = selectedBackColor;				}				else				{					if ((selection.EndCol-1) >= (offset + Text.Length))					{						ret[0].StringColor = selectedStringColor;						ret[0].BackColor = selectedBackColor;					}				}			}			return ret;		}
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
	}}