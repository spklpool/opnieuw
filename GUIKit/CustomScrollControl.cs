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
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

        private Point m_CustomScrollPosition = new Point(0,0);
        [Browsable(false)]
                return new Point(-m_CustomScrollPosition.X, -m_CustomScrollPosition.Y);
            }
                m_CustomScrollPosition = value;
                if (m_HScroll!=null)
                    m_HScroll.Value = -m_CustomScrollPosition.X;

                if (m_VScroll!=null)
                    m_VScroll.Value = -m_CustomScrollPosition.Y;
            }
                m_HScroll.Value = m_CustomScrollPosition.X;
                m_HScroll.BringToFront();
                m_VScroll.Value = m_CustomScrollPosition.Y;
                m_VScroll.BringToFront();
                m_RealClientWidth = ClientRectangle.Width-l_HeightHScroll;
            m_CustomScrollPosition.Y = m_VScroll.Value;
            OnVScrollPositionChanged(new ScrollPositionChangedEventArgs(-m_VScroll.Value,-m_OldVScrollValue));
            m_CustomScrollPosition.X = m_HScroll.Value;
            OnHScrollPositionChanged(new ScrollPositionChangedEventArgs(-m_HScroll.Value,-m_OldHScrollValue));