#region Copyright (C) Pierre Boudreau
//This file is part of the Opnieuw project.
//
//pierre.opnieuw.com
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
	/// <summary>
	/// This is the base class for command objects that can execute 
	/// a change to code.
	/// </summary>
	public class CodeChangeCommand
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fileName"></param>
		public CodeChangeCommand(string fileName)
		{
			m_FileName = fileName;
		}

		/// <summary>
		/// The name of the file that the change is executed on.
		/// </summary>
		protected string m_FileName = "";
		public string FileName {
			get {
				return m_FileName;
			}
		}

		/// <summary>
		/// Gets or sets the worker that executes this command.  The command
		/// keeps its worker because it might keep some state that will be used
		/// to undo the command.
		/// </summary>
		protected CodeChangeCommandWorker m_Worker = null;
		public CodeChangeCommandWorker Worker {
			get {
				return m_Worker;
			}
			set {
				m_Worker = value;
			}
		}
	}

	public class MissingCodeChangeCommand : CodeChangeCommand
	{
		public MissingCodeChangeCommand() :
		base("")
		{
		}
	}
}
