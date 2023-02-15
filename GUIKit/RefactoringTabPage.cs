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
using System.Windows.Forms;
using Opnieuw.Framework;

namespace Opnieuw.Framework
{
	public delegate bool ExecuteRefactoringHandler(CodeChangeCommandCollection col);
	public delegate void TabPageAvailabilityChangedHandler(RefactoringTabPage page, bool isAvailable);

	/// <summary>
	/// A refactoring tab page is the base user interface element for all refactorings.  It acts
	/// as a container for RefactoringGUI derived controls for specific refactorings.
	/// </summary>
	public class RefactoringTabPage : TabPage
	{
		public event TabPageAvailabilityChangedHandler AvailabilityChanged;
		public event ExecuteRefactoringHandler ExecuteRefactoring;

		protected ICodeTreeView m_CodeTreeView = null;
		private RefactoringGUI m_HostedControl;
		protected string m_RefactoringName;

		/// <summary>
		/// Constructor that passes in all the needed objects.
		/// </summary>
		/// <param name="codeTreeView"></param>
		/// <param name="hostedControl"></param>
		/// <param name="refactoringName"></param>
		public RefactoringTabPage(ICodeTreeView codeTreeView, RefactoringGUI hostedControl, string refactoringName)
		{
			m_CodeTreeView = codeTreeView;
			hostedControl.AvailabilityChanged += new AvailabilityChangedHandler(this.OnAvailabilityChanged);
			m_HostedControl = hostedControl;
			m_RefactoringName = refactoringName;

			InitializeComponent();
		}

		public ICodeTreeView CodeTreeView {
			get {
				return m_CodeTreeView;
			}
		}

		protected ArrayList SelectedNodes {
			get {
				return m_CodeTreeView.SelectedNodes;
			}
		}

		protected Hashtable TreeNodeMapper {
			get {
				return m_CodeTreeView.TreeNodeMapper;
			}
		}

		public virtual void OnAvailabilityChanged(bool isAvailable)
		{
			FireAvailabilityChanged(isAvailable);
		}

		protected void FireAvailabilityChanged(bool isAvailable)
		{
			if (AvailabilityChanged != null)
			{
				AvailabilityChanged(this, isAvailable);
			}
		}

		public virtual string Preview()
		{
			string ret = "";
			if (null != m_HostedControl)
			{
				ret = m_HostedControl.Preview();
			}
			return ret;
		}

		public virtual bool Execute()
		{
			bool ret = false;
			if (null != m_HostedControl)
			{
				ret = m_HostedControl.Execute();
			}
			return ret;
		}

		private void InitializeComponent()
		{
			if (null != this.m_HostedControl)
			{
				this.m_HostedControl.Dock = System.Windows.Forms.DockStyle.Fill;
				this.m_HostedControl.Name = "hostedControl";

				this.m_HostedControl.ExecuteRefactoring += new Opnieuw.Framework.ExecuteRefactoringHandler(this.FireExecuteRefactoring);

				this.Controls.AddRange(new System.Windows.Forms.Control[] {this.m_HostedControl});
			}
			this.Text = m_RefactoringName;
		}

		public bool FireExecuteRefactoring(CodeChangeCommandCollection col)
		{
			return ExecuteRefactoring(col);
		}
	}
}
