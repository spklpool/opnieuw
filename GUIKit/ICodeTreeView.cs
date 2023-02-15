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
	/// <summary>
	/// This is an interface to a tree view control that has the ability to display code elements.
	/// </summary>
	public interface ICodeTreeView
	{
		event CodeTreeViewEventHandler NodeSelectionChangedWithInfo;
		Hashtable TreeNodeMapper {
			get ;
		}
		ArrayList SelectedNodes {
			get;
		}
		void StartReloading();
		void EndReloading();		
		void PopulateTree(CodeCompileUnit unit) ;
		void Select(ArrayList selectedNodes);
		void ClearNodes();
		void Refresh();		
		void ClearSelectedNodes();
		void Remove(object unit);
	}
}
