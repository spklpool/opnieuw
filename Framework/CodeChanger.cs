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
	public delegate bool CodeChangeCommandEventHandler(CodeChangeCommand command);
	public delegate void CodeChangerEventHandler(CodeChangeCommandCollection col);

	/// <summary>
	/// This is the base class for objects that can execute code changes.
	/// </summary>
	public abstract class CodeChanger
	{
		public event CodeChangeCommandEventHandler BeforeDo;
		public event CodeChangeCommandEventHandler AfterDo;
		public event CodeChangerEventHandler Did;
		public event CodeChangerEventHandler Undid;

		protected bool m_IsDone = false;
		protected Stack m_UndoStack = new Stack();

		/// <summary>
		/// Constructor.
		/// </summary>
		public CodeChanger()
		{
		}

		private bool fireBeforeDo(CodeChangeCommand command)
		{
			if (BeforeDo != null)
			{
				return BeforeDo(command);
			}
			return true;
		}

		private bool fireAfterDo(CodeChangeCommand command)
		{
			if (AfterDo != null)
			{
				return AfterDo(command);
			}
			return true;
		}

		/// <summary>
		/// Real code changers need to implement this to return their workers.
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		protected abstract CodeChangeCommandWorker workerForCommand(CodeChangeCommand command);

		/// <summary>
		/// Executes the code change identified in the specified 
		/// collection of CodeChange objects.
		/// </summary>
		/// <param name="col"></param>
		/// <returns></returns>
		public virtual bool Do(CodeChangeCommandCollection col)
		{
			bool ret = true;
			try
			{
				//Execute commands and push them onto local undo stack in
				//case something goes wrong.
				Stack localUndoStack = new Stack();
				foreach (CodeChangeCommand command in col)
				{
					CodeChangeCommandWorker worker = workerForCommand(command);
					command.Worker = worker;
					bool commandSuccess = false;
					if (fireBeforeDo(command) == true)
					{
						if (true == worker.Do())
						{
							if (fireAfterDo(command) == true)
							{
								localUndoStack.Push(worker);
								commandSuccess = true;
							}
							else
							{
								worker.Undo();
							}
						}
					}
					if (false == commandSuccess)
					{
						ret = false;
						break;
					}
				}

				if (false == ret)
				{
					//There was an error, undo every command on the local stack.
					while (localUndoStack.Count > 0)
					{
						CodeChangeCommandWorker undoWorker = localUndoStack.Pop() as CodeChangeCommandWorker;
						undoWorker.Undo();
					}
				}
				else
				{
					//Everything went OK.  Add the command collection to the undo stack
					//and notify listeners.
					m_UndoStack.Push(col);
					if (null != Did)
					{
						Did(col);
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
				ret = false;
			}
			return ret;
		}

		/// <summary>
		/// Indicates if it is possible to undo the last code change execution.
		/// </summary>
		public virtual bool CanUndo {
			get {
				return (m_UndoStack.Count > 0);
			}
		}

		/// <summary>
		/// Reverses the execution of the last code change execution.
		/// </summary>
		/// <returns></returns>
		public virtual bool Undo()
		{
			bool ret = true;
			try
			{
				if (true == CanUndo)
				{
					CodeChangeCommandCollection col = m_UndoStack.Pop() as CodeChangeCommandCollection;
					col.Reverse();
					foreach (CodeChangeCommand command in col)
					{
						command.Worker.Undo();
					}
					if (null != Undid)
					{
						Undid(col);
					}
				}
			}
			catch
			{
				ret = false;
			}
			return ret;
		}
	}
}
