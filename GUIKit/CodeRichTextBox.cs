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
	public class CodeRichTextBox : StaticCodeView
	{
		private Caret m_Caret = null;
		
		public CodeRichTextBox(Code code) :
		base(code)
		{
			Size size = new Size(m_CaretWidth, (int)characterHeight);
			m_Caret = new Caret(this);
			m_Caret.Size = size;
		}
		
		bool m_LastKeyHandled = false;
		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			if (m_LastKeyHandled == false)
			{
				m_CodeCommandProcessor.Do(new CodeCommandKeyPress(m_Code, e.KeyChar));
			}
			base.OnKeyPress(e);
		}

		CodeCommandProcessor m_CodeCommandProcessor = new CodeCommandProcessor();
		protected override bool ProcessDialogKey(Keys keyData)
		{
			m_LastKeyHandled = true;
			switch (keyData) 
			{
				case Keys.PageUp:
					m_Code.PageUp();
					break;
				case Keys.PageDown:
					m_Code.PageDown();
					break;
				case Keys.Down:
					m_Code.MoveCursorDown();
					break;
				case Keys.Up:
					m_Code.MoveCursorUp();
					break;
				case Keys.Left:
					m_Code.MoveCursorLeft();
					break;
				case Keys.Right:
					m_Code.MoveCursorRight();
					break;
				case Keys.Shift | Keys.Right:
					m_Code.ExtendSelectionRight();
					break;
				case Keys.Shift | Keys.Left:
					m_Code.ExtendSelectionLeft();
					break;
				case Keys.Shift | Keys.Up:
					m_Code.ExtendSelectionUp();
					break;
				case Keys.Shift | Keys.Down:
					m_Code.ExtendSelectionDown();
					break;
				case Keys.Shift | Keys.Home:
					m_Code.ExtendSelectionToHome();
					break;
				case Keys.Shift | Keys.End:
					m_Code.ExtendSelectionToEnd();
					break;
				case Keys.Home:
					m_Code.Home();
					break;
				case Keys.End:
					Code.End();
					break;
				case Keys.Back:
					m_CodeCommandProcessor.Do(new CodeCommandBackspace(m_Code));
					break;
				case Keys.Delete:
					m_CodeCommandProcessor.Do(new CodeCommandDelete(m_Code));
					break;
				case Keys.Enter:
					m_Code.Enter();
					break;
				case Keys.Tab:
					m_Code.KeyPress('\t');
					//Don't let base class do anything in this case 
					//cause it will give focus away.
					return true;
				case Keys.Alt | Keys.F4:
					Application.Exit();
					break;
				case Keys.Control | Keys.S:
					FireSaveRequested();
					break;
				case Keys.Control | Keys.X:
					if (!Code.Selection.IsZeroLength)
					{
						Clipboard.SetDataObject(Code.SelectedText, true);
						Code.DeleteSelectedText();
					}
					break;
				case Keys.Control | Keys.C:
					Clipboard.SetDataObject(Code.SelectedText, true);
					break;
				case Keys.Control | Keys.V:
					string selectionText = (string)Clipboard.GetDataObject().GetData("System.String", true);
					Code.DeleteSelectedText();
					Code.InsertTextInSelection(selectionText);
					break;
				case Keys.Control | Keys.Z:
					m_CodeCommandProcessor.Undo();
					break;
				case Keys.Control | Keys.Y:
					m_CodeCommandProcessor.Redo();
					break;
				default:
					m_LastKeyHandled = false;
					break;
			}
			ScrollCursorIntoView();
			Select();
			return base.ProcessDialogKey(keyData);
		}

		[DllImport("user32")] public static extern ushort GetKeyState(int nVirtKey);

		enum VirtualKeys
		{
			VK_LSHIFT   = 0xA0,
			VK_RSHIFT   = 0xA1,
			VK_LCONTROL = 0xA2,
			VK_RCONTROL = 0xA3,
		};

		bool GetKeyState( VirtualKeys k)
		{
			return ( GetKeyState( (int)k )&0x100) != 0  ? true : false;
		}

		protected bool IsShiftKeyPressed {
			get {
				return (GetKeyState(VirtualKeys.VK_LSHIFT) || GetKeyState(VirtualKeys.VK_LSHIFT));
			}
		}

		private Position m_PositionOnMouseDown = Position.Missing;
		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				if (IsShiftKeyPressed)
				{
					Point p = MoveCursorToCoordinates(CustomScrollPosition, e.X, e.Y, characterHeight, m_Code.Lines);
					m_Code.ExtendSelectionToPoint(p.Y, p.X);
				}
				else
				{
					Capture = true;
					Point p = MoveCursorToCoordinates(CustomScrollPosition, e.X, e.Y, characterHeight, m_Code.Lines);
					int x = m_Code.Lines[p.Y-1].ClosestPositionSnap(p.X);
					m_Code.ResetSelection(p.Y, x, p.Y, x);
					m_PositionOnMouseDown = new Position(p.Y, x, p.Y, x);
				}
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (Capture && (e.Button == MouseButtons.Left))
			{
				if (e.Y > Height)
				{
					ScrollDownForMouse();
				}
				if (e.Y < 0)
				{
					ScrollUpForMouse();
				}
				Point p = MoveCursorToCoordinates(CustomScrollPosition, e.X, e.Y, characterHeight, m_Code.Lines);
				p.X = m_Code.Lines[p.Y-1].ClosestPositionSnap(p.X);
				Position checkPosition = new Position(p.Y, p.X, p.Y, p.X);
				if (checkPosition.StartsBefore(m_PositionOnMouseDown))
				{
					m_Code.ResetSelection(p.Y, p.X, m_PositionOnMouseDown.EndLine, m_PositionOnMouseDown.EndCol);
				}
				else
				{
					m_Code.ResetSelection(m_PositionOnMouseDown.StartLine, m_PositionOnMouseDown.StartCol, p.Y, p.X);
				}
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (Capture && (e.Button == MouseButtons.Left))
			{
				Capture = false;
				FireSelectionChanged();
				Select();
			}
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			if (e.Delta >= 120 || e.Delta <= -120)
			{
				Point t = CustomScrollPosition;
				int l_NewY = t.Y + (SystemInformation.MouseWheelScrollLines*(int)characterHeight) * Math.Sign(e.Delta) ;

				//check that the value is between max and min
				if (l_NewY>0)
					l_NewY = 0;
				if (l_NewY < (-base.MaximumVScroll + this.Height - (int)characterHeight) )
					l_NewY = (-base.MaximumVScroll + this.Height - (int)characterHeight);

				CustomScrollPosition = new Point(t.X,l_NewY);
				ResetCaretToSelection();
			}
		}

		private void ResetCaretToSelection()
		{
			Point scrollPoint = CustomScrollPosition;
			Point caretPoint = PointFromCharacterIndex(Selection.StartCol, Selection.StartLine);
			Point adjustedCaretPoint = new Point(caretPoint.X + scrollPoint.X, caretPoint.Y + scrollPoint.Y);
			if (Selection.IsZeroLength)
			{
				m_Caret.Position = adjustedCaretPoint;
				m_Caret.Show();
			}
			else
			{
				m_Caret.Hide();
			}
		}
		
		public override void Code_SelectionChanged(Position selection)
		{
			InvalidateSelectionArea(m_PreviousSelection, selection);
			Update();
			m_PreviousSelection = new Position(selection);
			ResetCaretToSelection();
		}
		
		protected override void OnVScrollPositionChanged(ScrollPositionChangedEventArgs e)
		{
            base.OnVScrollPositionChanged(e);
            ResetCaretToSelection();
		}
		
		protected override void OnHScrollPositionChanged(ScrollPositionChangedEventArgs e)
		{
            base.OnHScrollPositionChanged(e);
            ResetCaretToSelection();
		}
		
		protected override void RecalcHScrollBar()
		{
			if (m_HScroll != null)
			{
				int l_WidthVScroll = 0;
				if (m_VScroll != null)
					l_WidthVScroll = m_VScroll.Width;

				m_HScroll.Location = new Point(0,ClientRectangle.Height-m_HScroll.Height);
				m_HScroll.Width = ClientRectangle.Width-l_WidthVScroll;
				m_HScroll.Minimum = 0;
				m_HScroll.Maximum = Math.Max(0,m_CustomScrollArea.Width); //Math.Max(0,m_CustomScrollArea.Width - ClientRectangle.Width) + m_VScroll.Width;
				m_HScroll.LargeChange = Math.Max(5,ClientRectangle.Width - l_WidthVScroll);
				m_HScroll.SmallChange = m_HScroll.LargeChange / 5;
				m_HScroll.BringToFront();
				m_RealClientHeight = ClientRectangle.Height-l_WidthVScroll;
			}
		}
		
		protected override void RecalcVScrollBar()
		{
			if (m_VScroll != null)
			{
				int l_HeightHScroll = 0;
				if (m_HScroll != null)
					l_HeightHScroll = m_HScroll.Height;

				m_VScroll.Location = new Point(ClientRectangle.Width-m_VScroll.Width,0);
				m_VScroll.Height = ClientRectangle.Height-l_HeightHScroll;
				m_VScroll.Minimum = 0;
				m_VScroll.Maximum = Math.Max(0,m_CustomScrollArea.Height); //Math.Max(0,m_CustomScrollArea.Height - ClientRectangle.Height) + m_HScroll.Height;
				m_VScroll.LargeChange = Math.Max(5,ClientRectangle.Height - l_HeightHScroll);
				m_VScroll.SmallChange = m_VScroll.LargeChange / 5;
				m_VScroll.BringToFront();
				m_RealClientWidth = ClientRectangle.Width-l_HeightHScroll;
			}
		}
	}
}
