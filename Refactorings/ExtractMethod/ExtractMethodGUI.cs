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
using System.Threading;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

using Opnieuw.Framework;

namespace Opnieuw.ExtractMethod
{
	/// <summary>
	/// Summary description for ExtractMethodGUI.
	/// </summary>
	public class ExtractMethodGUI : Opnieuw.Framework.RefactoringGUI
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
        private CodeCompileUnitCollection m_ChangingUnits;
        private Opnieuw.Framework.BackgroundWorker BackgroundPreviewWorker;
        private MultiUnitView ChangeViewer;

		public ExtractMethodGUI()
		{
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
            m_ChangingUnits = new CodeCompileUnitCollection();
            this.ChangeViewer = new MultiUnitView(m_ChangingUnits);
            this.BackgroundPreviewWorker = new Opnieuw.Framework.BackgroundWorker();
            this.SuspendLayout();
// 
// ChangeViewer
// 
            this.ChangeViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChangeViewer.Location = new System.Drawing.Point(0, 0);
            this.ChangeViewer.Name = "ChangeViewer";
            this.ChangeViewer.Size = new System.Drawing.Size(408, 268);
            this.ChangeViewer.TabIndex = 0;
// 
// BackgroundPreviewWorker
// 
            this.BackgroundPreviewWorker.WorkerReportsProgress = false;
            this.BackgroundPreviewWorker.WorkerSupportsCancellation = false;
            this.BackgroundPreviewWorker.DoWork += new Opnieuw.Framework.DoWorkEventHandler(this.OnDoWork);
            this.BackgroundPreviewWorker.RunWorkerCompleted += new Opnieuw.Framework.RunWorkerCompletedEventHandler(this.OnCompleted);
            this.BackgroundPreviewWorker.ProgressChanged += new Opnieuw.Framework.ProgressChangedEventHandler(this.OnProgressChanged);

// 
// ExtractMethodGUI
// 
            this.Controls.Add(this.ChangeViewer);
            this.Name = "ExtractMethodGUI";
            this.Size = new System.Drawing.Size(408, 312);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
		#endregion

        void OnProgressChanged(object sender,Opnieuw.Framework.ProgressChangedEventArgs progressArgs)
        {
        }

        void OnCompleted(object sender,Opnieuw.Framework.RunWorkerCompletedEventArgs completedArgs)
        {
        }

        void OnDoWork(object sender, Opnieuw.Framework.DoWorkEventArgs doWorkArgs)
        {
            Opnieuw.Framework.BackgroundWorker backgroundWorker = sender as Opnieuw.Framework.BackgroundWorker;
            doWorkArgs.Result = null;
            CodeCompileUnitCollection compileUnits = m_Refactoring.ChangingCodeCompileUnits;
			ChangeViewer.BuildFromUnits(compileUnits);
        }

		/// <summary>
		/// Connects this RefactoringGUI to its Refactoring controler class.  The base implementation
		/// should always be called because it sets the m_Refactoring member that can be used to access
		/// the Refactoring controler later.
		/// </summary>
		/// <param name="refactoring"></param>
		public override void Connect(Refactoring refactoring)
		{
			base.Connect(refactoring);
			ExtractMethodRefactoring extractMethodRefactoring = refactoring as ExtractMethodRefactoring;
			extractMethodRefactoring.Changed += new RefactoringChangedHandler(this.OnRefactoringChanged);
		}

		/// <summary>
		/// Event handler that gets called when the preview text of the method to extract changes
		/// because of a change in the selected nodes in the user interface.
		/// </summary>
		/// <param name="newText"></param>
		private void OnRefactoringChanged()
		{
//            BackgroundPreviewWorker.RunWorkerAsync();
            CodeCompileUnitCollection compileUnits = m_Refactoring.ChangingCodeCompileUnits;
			ChangeViewer.BuildFromUnits(compileUnits);
        }
	}
}
