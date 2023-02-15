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
	/// <summary>
	/// This represents a line of code.
	/// </summary>
	public class CodeLine : StringPart
	{
		/// <summary>
		/// Standard constructor.
		/// </summary>
		/// <param name="text"></param>
		public CodeLine(string text) :
		base(text)
		{
			Text = text;
		}

		protected Color m_SelectedForeColor = Color.White;
		public Color SelectedForeColor {
			get {
				return m_SelectedForeColor;
			}
		}
		
		protected Color m_SelectedBackColor = Color.DarkBlue;
		public Color SelectedBackColor {
			get {
				return m_SelectedBackColor;
			}
		}

		protected Color m_ForeColor = Color.Black;
		public Color ForeColor {
			get {
				return m_ForeColor;
			}
		}

		protected Color m_BackColor = Color.White;
		public Color BackColor {
			get {
				return m_BackColor;
			}
		}

		private void InitializeExtendedLength()
		{
			m_ExtendedLength = 0;
			for(int i=0; i<Text.Length; i++)
			{
				if (Text[i] == '\t')
				{
					m_ExtendedLength += (4-(m_ExtendedLength%4));
				}
				else
				{
					m_ExtendedLength ++;
				}
			}
		}
		
		protected int m_ExtendedLength = 0;
		public int ExtendedLength {
			get {
				return m_ExtendedLength;
			}
		}
		
		public override string Text {
			get {
				return m_Text;
			}
			set {
				ResetTextAndParts(value);
			}
		}

		public void ResetTextAndParts(string text)
		{
			char[] CHARS_TO_TRIM = new char[2] {'\r', '\n'};
			m_Text = text.TrimEnd(CHARS_TO_TRIM);
			m_Parts = SplitUpParts();
			InitializeExtendedLength();
		}

		/// <summary>
		/// Returns the closest position to the left that does not fall inside
		/// a tab.
		/// </summary>
		/// <param name="position"></param>
		public int LeftPositionSnap(int position)
		{
			int extendedPosition = 1;
			for(int realPosition=0; realPosition<Text.Length; realPosition++)
			{
				if (Text[realPosition] == '\t') {
					if ((extendedPosition + (4-(extendedPosition%4))) >= position)
					{
						break;
					}
					extendedPosition += (4-(extendedPosition%4));
				} 
				if (extendedPosition >= position)
				{
					break;
				}
				extendedPosition ++;
			}
			return extendedPosition;
		}


		/// <summary>
		/// Returns the width in characters of a tab located at the
		/// specified position.
		/// </summary>
		/// <param name="position"></param>
		public int TabWidthAtPosition(int position)
		{
			int extendedPosition = 1;
			int thisTabWidth = 0;
			for(int realPosition=0; realPosition<Text.Length; realPosition++)
			{
				if (Text[realPosition] == '\t') {
					thisTabWidth = 4-(extendedPosition%4);
					if ((extendedPosition + thisTabWidth) >= position)
					{
						break;
					}
					extendedPosition += thisTabWidth;
				} 
				if (extendedPosition >= position)
				{
					break;
				}
				extendedPosition ++;
			}
			return thisTabWidth;
		}

		public int ClosestPositionSnap(int position)
		{
			if (position > ExtendedLength) return ExtendedLength+1;
			
			int extendedPosition = 1;
			for(int realPosition=0; realPosition<Text.Length; realPosition++)
			{
				if (Text[realPosition] == '\t') {
					if ((extendedPosition + (4-(extendedPosition%4))) >= position)
					{
						int leftPosition = extendedPosition;
						int rightPosition = extendedPosition + (4-(extendedPosition%4));
						if ((rightPosition - position) < (position - leftPosition))
						{
							// We are past the mid point of the tab, so snap to the end
							extendedPosition += (4-(extendedPosition%4));
							extendedPosition += 1;
						}
						break;
					}
					extendedPosition += (4-(extendedPosition%4));
				} 
				if (extendedPosition >= position)
				{
					break;
				}
				extendedPosition ++;
			}
			return extendedPosition;
		}

		public void Insert(int position, string text)
		{
			lock(this)
			{
				string temp = "";
				int extendedPosition = 0;
				for(int realPosition=0; realPosition<Text.Length; realPosition++)
				{
					if (extendedPosition == position)
					{
						temp += text;
					}
					if (Text[realPosition] == '\t') {
						extendedPosition += (4-(extendedPosition%4));
					} else {
						extendedPosition ++;
					}
					temp += Text[realPosition];
				}
				if (extendedPosition == position)
				{
					temp += text;
				}
				Text = temp;
				ResetBitmap();
			}
		}

		public void Remove(int start, int end)
		{
			lock(this)
			{
				string temp = "";
				int extendedPosition = 0;
				for(int realPosition=0; realPosition<Text.Length; realPosition++)
				{
					if ((extendedPosition < start) || (extendedPosition >= end))
					{
						temp += Text[realPosition];
					}
					if (Text[realPosition] == '\t') {
						extendedPosition += (4-(extendedPosition%4));
					} else {
						extendedPosition ++;
					}
				}
				Text = temp;
				ResetBitmap();
			}
		}
		
		/// <summary>
		/// Splits up the line into parts that have assiciated colors.  This
		/// should be overriden for different languages so that the colors
		/// can reflect keywords in the language.
		/// </summary>
		protected ColoredStringPartCollection SplitUpParts()
		{
			return PartsUpTo(m_Text.Length);
		}

		/// <summary>
		/// Splits up the line into parts that have assiciated colors up until
		/// the specified column.  This should be overriden for different languages
		/// so that the colors can reflect keywords in the language.
		/// </summary>
		public virtual ColoredStringPartCollection PartsUpTo(int columnIndex)
		{
			return m_Parts;
		}
		
		/// <summary>
		/// Returns the collection of colored string parts.
		/// </summary>
		protected ColoredStringPartCollection m_Parts = new ColoredStringPartCollection();
		public ColoredStringPartCollection Parts {
			get {
				return m_Parts;
			}
			set {
				m_Parts = value;
			}
		}
		
		/// <summary>
		/// Returns the collection of colored string parts that take the current
		/// selection into account.
		/// </summary>
		protected ColoredStringPartCollection m_SelectionParts = new ColoredStringPartCollection();
		public ColoredStringPartCollection SelectionParts {
			get {
				return m_SelectionParts;
			}
			set {
				m_SelectionParts = value;
			}
		}

		public void SplitForSelection(Position selection, int line)
		{
			lock(this)
			{
				if (((selection.StartLine) > line || (selection.EndLine < line)) ||
					((selection.StartLine == selection.EndLine) && (selection.StartCol == selection.EndCol)))
				{
					m_SelectionParts = m_Parts;
				}
				else
				{
					int offset = 0;
					m_SelectionParts = new ColoredStringPartCollection();
					foreach (ColoredStringPart part in m_Parts)
					{
						m_SelectionParts.Add(part.SplitForSelection(selection, line, offset, SelectedForeColor, SelectedBackColor));
						offset += part.ExtendedLength;
					}
				}
				ResetBitmap();
			}
		}
		
		public void Paint(Graphics g, int X, int Y, float characterWidth, float characterHeight, int characterBoundingBoxWidth, Brush backBrush, Font font) 
		{
			lock(this)
			{
				BackgroundPaintIfNeeded(characterWidth, characterHeight, characterBoundingBoxWidth, backBrush, font);
				if (m_Bitmap != null)
				{
					g.DrawImage(m_Bitmap, X, Y); 
				}
			}
		}
		
		protected Bitmap m_Bitmap = null;
		public void ResetBitmap()
		{
			lock(this)
			{
				if (m_Bitmap != null)
				{
					m_Bitmap.Dispose();
				}
				m_Bitmap = null;
			}
		}
		
		public void BackgroundPaintIfNeeded(float characterWidth, float characterHeight, int characterBoundingBoxWidth, Brush backBrush, Font font)
		{
			lock(this)
			{
				if (m_Bitmap == null)
				{
					Paint(characterWidth, characterHeight, characterBoundingBoxWidth, backBrush, font);
				}
			}
		}
		
		protected void Paint(float characterWidth, float characterHeight, int characterBoundingBoxWidth, Brush backBrush, Font font)
		{
			lock(this)
			{
				if (m_Bitmap != null)
				{
					m_Bitmap.Dispose();
				}
				int width = (int)((ExtendedLength * characterWidth) + (characterWidth*4));
				int height = (int)characterHeight;
				if ((width > 0) && (height > 0))
				{
					m_Bitmap = new Bitmap(width, height);
					Graphics g = Graphics.FromImage(m_Bitmap);
					g.FillRectangle(backBrush, 0, m_Bitmap.Height, m_Bitmap.Width, characterHeight);
					ColoredStringPartCollection selectionSyntaxStringParts = new ColoredStringPartCollection(SelectionParts);
					selectionSyntaxStringParts.Reverse();
					float partStartX = (ExtendedLength * characterWidth);
					foreach (ColoredStringPart part in selectionSyntaxStringParts)
					{
						partStartX -= (part.ExtendedLength * characterWidth);
						int charactersFromStart = (int)partStartX/(int)characterWidth;
						float charStartX = partStartX + (part.ExtendedLength*characterWidth);
						for(int i=part.Text.Length-1; i>=0; i--)
						{
							String sub = new String(part.Text[i], 1);
							Brush partBackBrush = new SolidBrush(part.BackColor);
							if (part.Text[i] == '\t')
							{
								int position = (int)(charStartX/characterWidth);
								int positionBeforeTab = LeftPositionSnap(position);
								int coordinateBeforeTab = (int)((characterWidth*positionBeforeTab));
								int rectangleWidth = (int)(characterWidth*TabWidthAtPosition(position));
								g.FillRectangle(partBackBrush, coordinateBeforeTab + 1, 0, 
												rectangleWidth + characterWidth, characterHeight);
								charStartX -= rectangleWidth;
							}
							else
							{
								g.FillRectangle(partBackBrush, charStartX, 0, 
												(int)characterBoundingBoxWidth, 
												(int)characterHeight);
								Brush drawBrush = new SolidBrush(part.StringColor);
								Rectangle enclosingRectangle = new Rectangle((int)charStartX, 0, 
																			 (int)characterBoundingBoxWidth, 
																			 (int)characterHeight);
								g.DrawString(sub, font, drawBrush, enclosingRectangle, new StringFormat());
							}
							charStartX -= characterWidth;
						}
					}
				}
			}
		}
	}
}
