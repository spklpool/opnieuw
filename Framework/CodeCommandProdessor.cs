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

namespace Opnieuw.Framework
{
	public class CodeCommandProcessor
	{
		private CodeCommandStack m_UndoStack = new CodeCommandStack();
		private CodeCommandStack m_RedoStack = new CodeCommandStack();

		public void Do(CodeCommand command)
		{
			command.Do();
			m_UndoStack.Push(command);
		}

		public void Undo()
		{
			if (m_UndoStack.Count > 0)
			{
				CodeCommand command = m_UndoStack.Pop();
				command.Undo();
				m_RedoStack.Push(command);
			}
		}

		public void Redo()
		{
			if (m_RedoStack.Count > 0)
			{
				CodeCommand command = m_RedoStack.Pop();
				command.Redo();
				m_UndoStack.Push(command);
			}
		}

		public int UndoableCommandCount {
			get {
				return m_UndoStack.Count;
			}
		}

		public CodeCommand NextUndoCommand {
			get {
				return m_UndoStack.Peek();
			}
		}

		public int RedoableCommandCount {
			get {
				return m_RedoStack.Count;
			}
		}

		public CodeCommand NextRedoCommand {
			get {
				return m_RedoStack.Peek();
			}
		}
	}
}
