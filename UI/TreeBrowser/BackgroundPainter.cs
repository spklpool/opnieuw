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
using System.Drawing;
using Opnieuw.Framework;

namespace Opnieuw.UI.TreeBrowser
{
	public class BackgroundPainter : IDisposable
	{
		Thread m_Thread = null;
		private bool m_Active = true;
		private Code m_Code = null;
		private int m_Height = 0;
		private int m_Width = 0;
		private float m_CharacterWidth = 0;
		private float m_CharacterHeight = 0;
		private Bitmap m_BackgroundImage = null;
		private float characterBoundingBoxWidth = 10;
		private Font m_Font = null;
		private StringFormat m_StringFormat = new StringFormat();
		private DebugWindow m_DebugWindow = null;

		public BackgroundPainter(Code code, float characterWidth, float characterHeight, DebugWindow debugWindow)
		{
			m_Code = code;
			m_CharacterWidth = characterWidth;
			m_CharacterHeight = characterHeight;
			m_DebugWindow = debugWindow;

			FontFamily m_FontFamily = new FontFamily("Courier New");
			m_Font = new Font(m_FontFamily, 10, FontStyle.Regular, GraphicsUnit.Point);

			//Figure out the width and height from the code.
			m_Height = m_Code.Lines.Count * (int)m_CharacterHeight;
			foreach (CodeLine line in m_Code.Lines)
			{
				if (line.ExtendedLength > m_Width)
				{
					m_Width = line.ExtendedLength;
				}
			}
			m_Width *= (int)m_CharacterWidth;
			m_BackgroundImage = new Bitmap(m_Width, m_Height);

			//Start background painting thread.			
			ThreadStart threadDelegate = new ThreadStart(PainterThread);
			Thread m_Thread = new Thread(threadDelegate);
			m_Thread.IsBackground = true;
			m_Thread.Priority = ThreadPriority.BelowNormal;
			m_Thread.Start();
		}

		public void Dispose()
		{
			m_Active = false;
			m_Thread.Join();
		}

		public void SaveAllCodeToFile(string filename)
		{
			Graphics g = Graphics.FromImage(m_BackgroundImage);
			g.Clear(m_Code.BackColor);
			Brush brush = new SolidBrush(m_Code.BackColor);
			int height = 0;
			foreach (CodeLine line in m_Code.Lines)
			{
				line.Paint(g, 0, height, m_CharacterWidth, m_CharacterHeight, (int)characterBoundingBoxWidth, brush, m_Font);
				height += (int)m_CharacterHeight;
			}
			g.Dispose();
			m_BackgroundImage.Save(filename);
		}

		public void PainterThread()
		{
			while (m_Active)
			{
				Brush brush = new SolidBrush(m_Code.BackColor);
				foreach (CodeLine line in m_Code.Lines)
				{
					line.BackgroundPaintIfNeeded(m_CharacterWidth, m_CharacterHeight, (int)characterBoundingBoxWidth, brush, m_Font);
				}
				Thread.Sleep(0);
			}
		}
	}
}