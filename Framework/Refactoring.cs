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
using System.CodeDom;
using System.Collections;

namespace Opnieuw.Framework
{
	public delegate void AvailabilityChangedHandler(bool isAvailable);

	/// <summary>
	/// This is the base class for all refactoring implementations.
	/// </summary>
	public class Refactoring
	{
		public event AvailabilityChangedHandler AvailabilityChanged;

		protected FundamentalPieceOfCodeCollection m_SelectedNodes = new FundamentalPieceOfCodeCollection();

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Refactoring()
		{
		}

		/// <summary>
		/// Checks if the event has any consumers and fires it.
		/// </summary>
		protected void FireAvailabilityChanged(bool isAvailable)
		{
			if (AvailabilityChanged != null) {
				AvailabilityChanged(isAvailable);
			}
		}

		public virtual CodeCompileUnitCollection ChangingCodeCompileUnits {
			get {
				CodeCompileUnitCollection ret = new CodeCompileUnitCollection();
				return ret;
			}
		}

		/// <summary>
		/// Returns a string that previews the changes made by this refactoring with the
		/// selected nodes.
		/// </summary>
		public virtual string PreviewText {
			get {
				return "";
			}
		}

		/// <summary>
		/// Returns the code changes necessary to execute the refactoring with the 
		/// current properties.
		/// </summary>
		public virtual CodeChangeCommandCollection CodeChanges {
			get {
				return new CodeChangeCommandCollection();
			}
		}

		/// <summary>
		/// Event handler that gets called when the node selection changes in the user interface.
		/// </summary>
		/// <param name="selectedNodes"></param>
		public virtual void OnNodeSelectionChanged(object source, ArrayList selectedNodes)
		{
			m_SelectedNodes = new FundamentalPieceOfCodeCollection();
			foreach (object obj in selectedNodes) {
				if (obj is FundamentalPieceOfCode) {
					m_SelectedNodes.Add(obj as FundamentalPieceOfCode);
				}
			}
		}
	}
}
