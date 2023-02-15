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
using System.Collections;
using System.CodeDom;
using System.CodeDom.Compiler;

namespace Opnieuw.Framework
{
	public delegate void ParsingStartedEventHandler(int fileCount);
	public delegate void ParsingDoneEventHandler();
	public delegate void ParsingFileEventHandler(string filename);
	public delegate void ParsingFileDoneEventHandler(string filename, CodeCompileUnit unit, bool success);
	public delegate void CodeTreeViewEventHandler(object source, ArrayList nodes);
	public delegate void StatusChangedEventHandler(string newStatus);
	public delegate void CodeSelectionChangeEventHandler(Position selection);
	public delegate void SelectionChangedWithInfoEventHandler(Position selection, string filename);
	public delegate void SaveRequestedEventHandler();
	public delegate void RefactoringChangedHandler();
}
