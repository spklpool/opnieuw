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

namespace Opnieuw.Framework
{
	class CodeCommandStack
	{
		private Stack m_Stack = new Stack();
		
		public void Push(CodeCommand command)
		{
			m_Stack.Push(command);
		}
		
		public CodeCommand Peek() 
		{
			return m_Stack.Peek() as CodeCommand;
		}
		
		public CodeCommand Pop() 
		{
			return m_Stack.Pop() as CodeCommand;
		}
		
		public int Count {
			get {
				return m_Stack.Count;
			}
		}
	}
}
