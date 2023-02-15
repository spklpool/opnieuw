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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Opnieuw.Framework
{
	/// <summary>
	/// This is a base User Control for the GUI portions fo all
	/// refactoring GUI implementations.
	/// </summary>
	public class RefactoringGUI : System.Windows.Forms.UserControl
	{
		public event AvailabilityChangedHandler AvailabilityChanged;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public RefactoringGUI()
		{
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		public event ExecuteRefactoringHandler ExecuteRefactoring;

		protected Refactoring m_Refactoring = null;

		public virtual void Connect(Refactoring refactoring)
		{
			m_Refactoring = refactoring;
			refactoring.AvailabilityChanged += new AvailabilityChangedHandler(this.OnRefactoringAvailabilityChanged);
		}

		public virtual void OnRefactoringAvailabilityChanged(bool isAvailable)
		{
			FireAvailabilityChanged(isAvailable);
		}

		protected void FireAvailabilityChanged(bool isAvailable)
		{
			if (AvailabilityChanged != null)
			{
				AvailabilityChanged(isAvailable);
			}
		}

		public virtual string Preview()
		{
			string ret = "";
			if (null != m_Refactoring)
			{
				ret = m_Refactoring.PreviewText;
			}
			return ret;
		}

		public virtual bool Execute()
		{
			bool ret = false;
			if (null != m_Refactoring)
			{
				try {
					FireExecuteRefactoring(m_Refactoring.CodeChanges);
					return true;
				} catch (Exception e) {
					Console.WriteLine(e.Message);
					Console.WriteLine(e.StackTrace);
					ret = false;
				}
			}
			return ret;
		}

		public bool FireExecuteRefactoring(CodeChangeCommandCollection col)
		{
			return ExecuteRefactoring(col);
		}
	}
}
