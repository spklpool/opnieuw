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
using System.Threading;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Opnieuw.Framework
{
	public class StaticCodeView : CustomScrollControl
	{
		protected CodeEventHandler m_TextChangedHandler = null;
		protected StatusChangedEventHandler m_StatusChangedHandler = null;
		protected CodeEventHandler m_CurrentLineTextChangedHandler = null;
		protected CodeSelectionChangeEventHandler m_SelectionChangingHandler = null;
		protected CodeSelectionChangeEventHandler m_SelectionChangedHandler = null;
		
		public event CodeSelectionChangeEventHandler SelectionChanged;
		public event StatusChangedEventHandler StatusChanged;
		public event SaveRequestedEventHandler SaveRequested;

		protected StringFormat m_StringFormat = new StringFormat();
		protected float widthOffset = 0;
		protected float characterWidth = 8;
		protected float characterBoundingBoxWidth = 10;
		protected float characterHeight = 15;
		
		protected int m_LeftBorderWidth = 10;

		DebugWindow m_DebugWindow = new DebugWindow();
		
		public StaticCodeView(Code code)
		{
			FontFamily fontFamily = new FontFamily("Courier New");
			Font = new Font(fontFamily, 10, FontStyle.Regular, GraphicsUnit.Point);
			SizeF layoutSize = new SizeF(300.0F, 300.0F);
			StringFormat newStringFormat = new StringFormat();
			m_Code = code;
			BackColor = Code.BackColor;
			InitisalizeEventHandlers();
			InitializeComponent();
			SetCharacterSize();
			SetControlStyle();

//			m_DebugWindow.Show();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				m_DebugWindow.AppendText("*Disposing CodeRichTextBox*\r\n");
				m_CachedBackBrush.Dispose();
				m_CachedLineBrush.Dispose();
			}
			base.Dispose(disposing);
		}

		public override Font Font {
			get {
				return base.Font;
			}
			set {
				base.Font = value;
				SetCharacterSize();
			}
		}

		private void SetCharacterSize()
		{
			Graphics g = CreateGraphics();
			SizeF size1 = g.MeasureString("1", Font);
			SizeF size2 = g.MeasureString("12", Font);
			characterWidth = size2.Width - size1.Width;
			characterBoundingBoxWidth = characterWidth + 2;
			widthOffset = size2.Width - characterWidth;
			characterHeight = Font.Height;
		}

		private void InitisalizeEventHandlers()
		{
			InstanciateDelegates();
			RegisterEventHandlers();
		}

		private void InstanciateDelegates()
		{
			m_TextChangedHandler = new CodeEventHandler(OnCodeTextChanged);
			m_CurrentLineTextChangedHandler = new CodeEventHandler(OnCodeCurrentLineTextChanged);
			m_StatusChangedHandler = new StatusChangedEventHandler(this.Code_StatusChanged);
			m_SelectionChangingHandler = new CodeSelectionChangeEventHandler(this.Code_SelectionChanging);
			m_SelectionChangedHandler = new CodeSelectionChangeEventHandler(this.Code_SelectionChanged);
		}
		
		private void RegisterEventHandlers()
		{
			m_Code.CurrentLineTextChanged += m_CurrentLineTextChangedHandler;
			m_Code.StatusChanged += m_StatusChangedHandler;
			m_Code.TextChanged += m_TextChangedHandler;
			m_Code.SelectionChanging += m_SelectionChangingHandler;
			m_Code.SelectionChanged += m_SelectionChangedHandler;
		}

		private void SetControlStyle()
		{
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.UserMouse, true);
		}

		protected void FireSelectionChanged() {
			if (SelectionChanged != null) {
				SelectionChanged(Selection);
			}
		}

		protected void FireStatusChanged(string newStatus) {
			if (StatusChanged != null) {
				StatusChanged(newStatus);
			}
		}

		protected void FireSaveRequested() {
			if (SaveRequested != null) {
				SaveRequested();
			}
		}

		private void OnCodeCurrentLineTextChanged()
		{
			InvalidateCurrentLine();
			Update();
		}

		private void OnCodeTextChanged()
		{
			Point scrollPoint = CustomScrollPosition;
			Invalidate();
			Update();
		}
		
		public void Code_StatusChanged(string newStatus)
		{
			FireStatusChanged(newStatus);
		}
		
		protected Position m_PreviousSelection = Position.Missing;
		public void Code_SelectionChanging(Position selection)
		{
		}
		
		public virtual void Code_SelectionChanged(Position selection)
		{
			Point scrollPoint = CustomScrollPosition;
			InvalidateSelectionArea(m_PreviousSelection, selection);
			Update();
			m_PreviousSelection = new Position(selection);
		}

		protected Code m_Code = null;
		public Code Code {
			get {
				return m_Code;
			}
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		protected int m_TabCharacters = 4;
		public int TabCharacters {
			get {
				return m_TabCharacters;
			}
			set {
				m_TabCharacters = value;
			}
		}

		public void Select(int row, int col)
		{
		}

		public CodeSelection Selection {
			get {
				return m_Code.Selection;
			}
			set {
				InvalidateSelectionArea(m_Code.Selection, value);
				m_Code.Selection = value;
				Update();
			}
		}

		protected Code CreateCode(string text)
		{
			return new CSCode(text);
		}

		public override string Text {
			get {
				return m_Code.Text;
			}
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			Point scrollPoint = CustomScrollPosition;
		}

		private Color m_CachedLineBrushColor = Color.White;
		private SolidBrush m_CachedLineBrush = new SolidBrush(Color.White);
		private SolidBrush GetLineBrush(Color color)
		{
			if (color != m_CachedLineBrushColor)
			{
				m_CachedLineBrush.Dispose();
				m_CachedLineBrush = new SolidBrush(color);
				m_CachedLineBrushColor = color;
			}
			return m_CachedLineBrush;
		}

		private Color m_CachedBackBrushColor = Color.White;
		private SolidBrush m_CachedBackBrush = new SolidBrush(Color.White);
		private SolidBrush GetBackBrush(Color color)
		{
			if (color != m_CachedBackBrushColor)
			{
				m_CachedBackBrush.Dispose();
				m_CachedBackBrush = new SolidBrush(color);
				m_CachedBackBrushColor = color;
			}
			return m_CachedBackBrush;
		}
		
		public void ResetSelection(CodeSelection selection)
		{
			m_Code.ResetSelectionFromUnexpandedPosition(selection);
		}

		protected Point PointFromCharacterIndex(int x, int y)
		{
			float retX = (x * characterWidth) + m_LeftBorderWidth;
			int retY = (int)(((float)y * characterHeight)-characterHeight);
			return new Point((int)retX + 1, retY);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			RectangleF clipRectangle = e.ClipRectangle;
            Bitmap backBitmap = new Bitmap(CustomClientRectangle.Width, CustomClientRectangle.Height);
            Graphics g = Graphics.FromImage(backBitmap);
            float repaintTop = clipRectangle.Top;
			float repaintBottom = clipRectangle.Bottom;
			Point scrollPoint = CustomScrollPosition;
			int height = 0;
			int width = 0;
			Brush brush = GetBackBrush(Code.BackColor);
			g.FillRectangle(brush, 0, 0, Width, Height);
			foreach (CodeLine line in m_Code.Lines)
			{
				if ((height >= (repaintTop-characterHeight)) && ((height+scrollPoint.Y) <= repaintBottom ))
				{
					line.Paint(g, m_LeftBorderWidth + scrollPoint.X, height+scrollPoint.Y, characterWidth, characterHeight, 
							   (int)characterBoundingBoxWidth, brush, Font);
				}
				if ((line.ExtendedLength * characterWidth) > width)
				{
					width = line.ExtendedLength * (int)characterWidth;
				}
				height += (int)characterHeight;
			}

            //DrawRectangleAroundInvalidatedArea(g, clipRectangle);
			CustomScrollArea = new Size(width,height);

			//Add rectangle around highlighted areas
			Pen highlightPen = new Pen(Color.Red);
			foreach (Position pos in m_HighlightedAreas) {
				ArrayList rectangles = GetRegionFromPosition(pos);
				if (rectangles.Count > 0) {
					int rectX = ((Rectangle)rectangles[0]).X;
					int rectY = ((Rectangle)rectangles[0]).Y;
					int rectWidth = ((Rectangle)rectangles[0]).Width;
					int rectHeight = 0;
					foreach (Rectangle rect in rectangles)
					{
						if (rect.X < rectX) {
							rectX = rect.X;
						}
						if (rect.Y < rectY) {
							rectY = rect.Y;
						}
						if (rect.Width > rectWidth) {
							rectWidth = rect.Width;
						}
						rectHeight += rect.Height;
					}
					Rectangle calculatedRect = new Rectangle(rectX, rectY, rectWidth, rectHeight);
                    g.DrawRectangle(highlightPen, calculatedRect);
                }
            }

            //Draw the screen position marker to the left of the screen
            Brush leftScrollViewBrush = new SolidBrush(Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(255))))));
            Rectangle leftScrollView = new Rectangle(0, 0, 10, CustomClientRectangle.Height);
            float screenRatio = Math.Min(1, (float)CustomClientRectangle.Height / (float)m_CustomScrollArea.Height);
            float screenMarkerHeight = Math.Min(m_CustomScrollArea.Height, screenRatio * (float)CustomClientRectangle.Height);
            float screenMarkerPosition = 0 - (screenRatio * (float)CustomScrollPosition.Y);
            Rectangle leftScrollViewScreenMarker = new Rectangle(1, (int)screenMarkerPosition, 8, (int)screenMarkerHeight - 1);
            g.FillRectangle(leftScrollViewBrush, leftScrollView);

            //Draw the highlighed areas in miniature in the left marker area
            foreach (Position pos in m_HighlightedAreas) {
                Brush highlightBrush = new SolidBrush(Color.Red);
                float highlightY = screenRatio * (float)((pos.StartLine-1) * characterHeight);
                float highlightHeight = Math.Min(m_CustomScrollArea.Height, Math.Max(1, screenRatio * (float)((pos.EndLine-pos.StartLine+1) * characterHeight)));
                Rectangle highlightRect = new Rectangle(2, (int)highlightY, 6, (int)highlightHeight);
                g.FillRectangle(highlightBrush, highlightRect);
            }

            //Draw the screen position indicator in the marker area 
            Pen leftScrollViewScreenMarkerPen = new Pen(Color.Black);
            g.DrawLine(leftScrollViewScreenMarkerPen, (float)1, screenMarkerPosition, (float)1, screenMarkerPosition + screenMarkerHeight - (float)1);
            g.DrawLine(leftScrollViewScreenMarkerPen, (float)1, screenMarkerPosition, (float)4, screenMarkerPosition);
            g.DrawLine(leftScrollViewScreenMarkerPen, (float)1, screenMarkerPosition + screenMarkerHeight - (float)1, (float)4, screenMarkerPosition + screenMarkerHeight - (float)1);

            //Flip the back buffer to the screen
            Graphics screenGraphics = e.Graphics;
            screenGraphics.DrawImageUnscaled(backBitmap, 0, 0);
            g.Dispose();
        }

		private void DrawRectangleAroundInvalidatedArea(Graphics g, RectangleF clipRectangle)
		{
			Pen invalidatingPen = new Pen(Color.Red);
			Rectangle adjusted = new Rectangle((int)clipRectangle.X, (int)clipRectangle.Y, 
											   (int)clipRectangle.Width-1, 
											   (int)clipRectangle.Height-1);
			g.DrawRectangle(invalidatingPen, adjusted);
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			//Leave this here to prevent from painting anything on background.
		}

		protected void InvalidateSelectionArea(Position selection1, Position selection2)
		{
			Region region = new Region();
			region.MakeEmpty();
			PositionCollection col = selection1.ChangingPositionsTo(selection2);
			foreach (Position pos in col)
			{
				ArrayList rectangles = GetRegionFromPosition(pos);
				foreach (Rectangle rect in rectangles)
				{
					region.Union(rect);
				}
			}
			Invalidate(region);
		}

		protected ArrayList GetRegionFromPosition(Position selection)
		{
			ArrayList ret = new ArrayList();
			if (selection.StartLine == selection.EndLine)
			{
				//Everything is on one line, so we can return a simple rectangular region.
				Point startPoint = PointFromCharacterIndex(selection.StartCol, selection.StartLine);
				Point endPoint = PointFromCharacterIndex(selection.EndCol, selection.EndLine);
				ret.Add(new Rectangle(startPoint.X + CustomScrollPosition.X, 
									  startPoint.Y + CustomScrollPosition.Y, 
									  endPoint.X - startPoint.X + 2,
									  endPoint.Y - startPoint.Y + (int)characterHeight));
			}
			else
			{
				//We have multiple lines, so we will need to build up a region made up 
				//of potentially several rectangles.
				Point startStartPoint = PointFromCharacterIndex(selection.StartCol, selection.StartLine);
				Point startEndPoint = PointFromCharacterIndex(m_Code.Lines[selection.StartLine-1].ExtendedLength, selection.StartLine);
				ret.Add(new Rectangle(startStartPoint.X + CustomScrollPosition.X, 
									  startStartPoint.Y + CustomScrollPosition.Y, 
									  startEndPoint.X - startStartPoint.X + (int)(characterWidth), 
									  startEndPoint.Y - startStartPoint.Y + (int)characterHeight));
				for (int i=selection.StartLine; i<selection.EndLine-1; i++)
				{
					Point startPoint = PointFromCharacterIndex(1, i+1);
					Point endPoint = PointFromCharacterIndex(m_Code.Lines[i].ExtendedLength, i+1);
					ret.Add(new Rectangle(startPoint.X + CustomScrollPosition.X, 
										  startPoint.Y + CustomScrollPosition.Y, 
										  endPoint.X - startPoint.X + (int)(characterWidth*3), 
										  endPoint.Y - startPoint.Y + (int)characterHeight));
				}
				Point endStartPoint = PointFromCharacterIndex(1, selection.EndLine);
				Point endEndPoint = PointFromCharacterIndex(selection.EndCol, selection.EndLine);
				ret.Add(new Rectangle(endStartPoint.X + CustomScrollPosition.X, 
									  endStartPoint.Y + CustomScrollPosition.Y, 
									  endEndPoint.X - endStartPoint.X + (int)(characterWidth),  
									  endEndPoint.Y - endStartPoint.Y + (int)characterHeight));
			}
			return ret;
		}

		protected void InvalidateSelectionArea(Position selection)
		{
			Point startPoint = PointFromCharacterIndex(0, selection.StartLine);
			Point endPoint = PointFromCharacterIndex(0, selection.EndLine);
			Rectangle rect = new Rectangle(0, startPoint.Y + CustomScrollPosition.Y, Width, endPoint.Y - startPoint.Y + (int)characterHeight);
			Invalidate(rect);
		}

		protected void InvalidateSelectionArea()
		{
			InvalidateSelectionArea(Selection);
		}

		protected void InvalidateCurrentLine()
		{
			Point p = PointFromCharacterIndex(Selection.EndCol, Selection.EndLine);
			Rectangle rect = new Rectangle(0, p.Y + CustomScrollPosition.Y, this.Width, (int)characterHeight);
			Invalidate(rect);
		}

		protected void InvalidateCurrentLineAndUpdate()
		{
			InvalidateCurrentLine();
			Update();
		}

		protected int m_CaretWidth = 2;
		public int CaretWidth {
			get {
				return m_CaretWidth;
			}
			set {
				m_CaretWidth = value;
			}
		}

		protected CodeLine CurrentLine {
			get {
				return m_Code.Lines[Selection.EndLine];
			}
			set {
				m_Code.Lines[Selection.EndLine] = value;
			}
		}

		private void ScrollUpOneLine()
		{
			Point t = CustomScrollPosition;
			int newY = t.Y + (int) characterHeight;
			if(newY > 0)
			{
				newY = 0;
			}
			CustomScrollPosition = new Point(t.X, newY);
		}

		protected void ScrollCursorIntoView()
		{
			Point p = PointFromCharacterIndex(0, Selection.EndLine);
			if ((p.Y + CustomScrollPosition.Y + characterHeight) > Height)
			{
				Point t = CustomScrollPosition;
				int newY = t.Y - (int)characterHeight;
				if (newY < (-base.MaximumVScroll + this.Height - (int)characterHeight) )
				{
					newY = (-base.MaximumVScroll + this.Height - (int)characterHeight);
				}
				CustomScrollPosition = new Point(t.X,newY);
			}
			if ((p.Y + CustomScrollPosition.Y) < 0)
			{
				ScrollUpOneLine();
			}
		}

		protected void ScrollDownForMouse()
		{
			Point t = CustomScrollPosition;
			int newY = t.Y - (SystemInformation.MouseWheelScrollLines * (int) characterHeight);
			if(newY < (-base.MaximumVScroll + this.Height - (int) characterHeight))
				newY = (-base.MaximumVScroll + this.Height - (int) characterHeight);
			CustomScrollPosition = new Point(t.X, newY);
		}

		protected void ScrollUpForMouse()
		{
			Point t = CustomScrollPosition;
			int newY = t.Y + (SystemInformation.MouseWheelScrollLines * (int) characterHeight);
			if(newY > 0)
				newY = 0;
			CustomScrollPosition = new Point(t.X, newY);
		}

        public void ScrollToLine(int line)
        {
            if ((line > 0) && (line < Code.Lines.Count))
            {
                Point p = CustomScrollPosition;
                
                if (CustomScrollArea.Height > 0) {
                    p.Y = Math.Max(0, (line * ((int)characterHeight) - (CustomScrollArea.Height/2)));
                } else {
                    p.Y = Math.Max(0, (line * ((int)characterHeight) - ((int)characterHeight * 10)));
                }
                CustomScrollPosition = p;
            }
        }

        //Returns a Point indicating the character indexes closest to the
		//mouse coordinates specified.
		public Point MoveCursorToCoordinates(Point CustomScrollPosition, int X, int Y, float characterHeight, CodeLineCollection lines)
		{
			Point ret = new Point(1, 1);
			ret.Y = (int)((Y - CustomScrollPosition.Y)/characterHeight)+1;
			if (ret.Y > lines.Count) {
				ret.Y = lines.Count;
			}
			if (ret.Y < 1) {
				ret.Y = 1;
			} 
			if (X < 5) {
				ret.X = 1;
			} else {
				ret.X = ((int)((X - (int)CustomScrollPosition.X)-((int)characterWidth/2))/(int)characterWidth);
			}
			return ret;
		}
		
		protected PositionCollection m_HighlightedAreas = new PositionCollection();
		public void AddHighlightedArea(Position position)
		{
			m_HighlightedAreas.Add(position);
		}
		
		public void ClearHighlightedAreas()
		{
			m_HighlightedAreas.Clear();
		}
	}
}
