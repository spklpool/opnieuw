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
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Opnieuw.Framework
{
	class Caret
	{
		[DllImport("user32.dll")]
		public static extern int CreateCaret(IntPtr hwnd, IntPtr hbm, int cx, int cy);

		[DllImport("user32.dll")]
		public static extern int DestroyCaret();

		[DllImport("user32.dll")]
		public static extern int SetCaretPos(int x, int y);

		[DllImport("user32.dll")]
		public static extern int ShowCaret(IntPtr hwnd);

		[DllImport("user32.dll")]
		public static extern int HideCaret(IntPtr hwnd);

		Control ctrl;
		Size    size;
		Point   ptPos;
		bool    bVisible;

		//Default konstruktor soll nicht verwendet werden können (deshalb private)
		private Caret()
		{
		}

		public Caret(Control ctrl)
		{
			this.ctrl = ctrl;
			Position  = Point.Empty;
			Size      = new Size(1, ctrl.Font.Height);

			Control.GotFocus  += new EventHandler(ControlOnGotFocus);
			Control.LostFocus += new EventHandler(ControlOnLostFocus);

			//wenn das control schon den focus hat, erstell mar das caret
			if(ctrl.Focused)
				ControlOnGotFocus(ctrl, new EventArgs());
		}

		//property´s
		public Control Control
		{
			get { return ctrl; }
		}

		public Size Size
		{
			get { return size;  }
			set { size = value; }
		}

		public Point Position
		{
			get { return ptPos; }
			set { ptPos = value; SetCaretPos(ptPos.X, ptPos.Y); }
		}

		public bool Visibility
		{
			get { return bVisible; }
			set
			{
				if(bVisible = value)
					ShowCaret(Control.Handle);
				else
					HideCaret(Control.Handle);
			}
		}

		//my functions

		public void Show()
		{
			Visibility = true;
		}

		public void Hide()
		{
			Visibility = false;
		}

		public void Dispose()
		{
			if(ctrl.Focused)
				ControlOnLostFocus(ctrl, new EventArgs());

			Control.GotFocus  -= new EventHandler(ControlOnGotFocus);
			Control.LostFocus -= new EventHandler(ControlOnLostFocus);
		}

		void ControlOnGotFocus(object obj, EventArgs ea)
		{
			CreateCaret(Control.Handle, IntPtr.Zero, Size.Width, Size.Height);
			SetCaretPos(Position.X, Position.Y);
			Show();
		}

		void ControlOnLostFocus(object obj, EventArgs ea)
		{
			Hide();
			DestroyCaret();
		}
	}
}