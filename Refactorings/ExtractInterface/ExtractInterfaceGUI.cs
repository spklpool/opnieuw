#region Copyright (C) Pierre Boudreau
//This file is part of the Opnieuw project.
//
//pierre.boudreau@alphacentauri.biz
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
using Opnieuw.Framework;
using Opnieuw.Parsers.CSParser;

namespace Opnieuw.ExtractInterface
{
	/// <summary>
	/// This is the user interface component for the Extract interface 
	/// refactoring.
	/// </summary>
	public class ExtractInterfaceGUI : RefactoringGUI
	{
		private System.Windows.Forms.Panel PreviewsPanel;
        private CodeCompileUnitCollection m_ChangingUnits;
        private MultiUnitView ChangeViewer;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ExtractInterfaceGUI()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.PreviewsPanel = new System.Windows.Forms.Panel();
            this.PreviewsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PreviewsPanel.Location = new System.Drawing.Point(10, 100);
            this.PreviewsPanel.Name = "PreviewsPanel";
            
            this.PreviewsPanel.SuspendLayout();
            this.SuspendLayout();

            m_ChangingUnits = new CodeCompileUnitCollection();
            ChangeViewer = new MultiUnitView(m_ChangingUnits);
            ChangeViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PreviewsPanel.Controls.Add(ChangeViewer);
			// 
			// ExtractInterfaceGUI
			// 
			this.Controls.Add(this.PreviewsPanel);
			this.Name = "ExtractInterfaceGUI";
			this.Size = new System.Drawing.Size(488, 360);
			this.PreviewsPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Connects this RefactoringGUI to its Refactoring controler class.  The base implementation
		/// should always be called because it sets the m_Refactoring member that can be used to access
		/// the Refactoring controler later.
		/// </summary>
		/// <param name="refactoring"></param>
		public override void Connect(Refactoring refactoring)
		{
			base.Connect(refactoring);
			ExtractInterfaceRefactoring extractInterfaceRefactoring = refactoring as ExtractInterfaceRefactoring;
			extractInterfaceRefactoring.Changed += new RefactoringChangedHandler(this.OnRefactoringChanged);
		}

		/// <summary>
		/// Event handler that gets called when the preview text of the interface to extract changes
		/// because of a change in the selected nodes in the user interface.
		/// </summary>
		/// <param name="newText"></param>
		private void OnRefactoringChanged()
		{
			CodeCompileUnitCollection compileUnits = m_Refactoring.ChangingCodeCompileUnits;
			ChangeViewer.BuildFromUnits(compileUnits);
		}
	}
}
