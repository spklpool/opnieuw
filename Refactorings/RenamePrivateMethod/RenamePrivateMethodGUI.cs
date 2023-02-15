using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Opnieuw.Framework;

namespace Opnieuw.Refactorings.RenamePrivateMethod
{
	/// <summary>
	/// Summary description for RenamePrivateMethodGUI.
	/// </summary>
	public class RenamePrivateMethodGUI : RefactoringGUI
	{
		private System.Windows.Forms.TextBox NewMethodName;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public RenamePrivateMethodGUI()
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
			this.NewMethodName = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// NewMethodName
			// 
			this.NewMethodName.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.NewMethodName.Location = new System.Drawing.Point(0, 220);
			this.NewMethodName.Name = "NewMethodName";
			this.NewMethodName.Size = new System.Drawing.Size(320, 20);
			this.NewMethodName.TabIndex = 0;
			this.NewMethodName.Text = "";
			this.NewMethodName.TextChanged += new System.EventHandler(this.NewMethodName_TextChanged);
			// 
			// RenamePrivateMethodGUI
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.NewMethodName});
			this.Name = "RenamePrivateMethodGUI";
			this.Size = new System.Drawing.Size(320, 240);
			this.ResumeLayout(false);

		}
		#endregion

		private void NewMethodName_TextChanged(object sender, System.EventArgs e)
		{
			(m_Refactoring as RenamePrivateMethodRefactoring).NewMethodName = this.NewMethodName.Text;
		}
	}
}
