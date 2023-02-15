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

namespace Opnieuw.Refactorings.GenerateMethod
{
	/// <summary>
	/// This is the user interface component for the Generate Method 
	/// refactoring.
	/// </summary>
	public class GenerateMethodGUI : RefactoringGUI
	{
		private System.Windows.Forms.TextBox GenerateMethodPreview;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Constructor.
		/// </summary>
		public GenerateMethodGUI()
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
			this.GenerateMethodPreview = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// GenerateMethodPreview
			// 
			this.GenerateMethodPreview.Dock = System.Windows.Forms.DockStyle.Fill;
			this.GenerateMethodPreview.Multiline = true;
			this.GenerateMethodPreview.Name = "GenerateMethodPreview";
			this.GenerateMethodPreview.ReadOnly = true;
			this.GenerateMethodPreview.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.GenerateMethodPreview.Size = new System.Drawing.Size(488, 316);
			this.GenerateMethodPreview.TabIndex = 1;
			this.GenerateMethodPreview.Text = "";
			this.GenerateMethodPreview.WordWrap = false;
			// 
			// GenerateMethodGUI
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {this.GenerateMethodPreview});
			this.Name = "GenerateMethodGUI";
			this.Size = new System.Drawing.Size(488, 360);
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
			GenerateMethodRefactoring GenerateMethodRefactoring = refactoring as GenerateMethodRefactoring;
			GenerateMethodRefactoring.PreviewTextChanged += new PreviewTextChangedHandler(this.OnPreviewTextChanged);
		}

		/// <summary>
		/// Event handler that gets called when the preview text of the interface to extract changes
		/// because of a change in the selected nodes in the user interface.
		/// </summary>
		/// <param name="newText"></param>
		private void OnPreviewTextChanged(string newText)
		{
			GenerateMethodPreview.Text = newText;
		}

		/// <summary>
		/// Evnent handler that is called when the text changes in the GenerateMethodName text box.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param> 
		private void GenerateMethodName_TextChanged(object sender, System.EventArgs e) 
		{
			GenerateMethodPreview.Text = m_Refactoring.PreviewText;
		}
	}
}
