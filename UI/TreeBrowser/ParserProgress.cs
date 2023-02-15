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
	/// Summary description for ParserProgress.
	/// </summary>
	public class ParserProgress : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Label ProgressText;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Splitter splitter1;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ParserProgress()
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
			this.ProgressText = new System.Windows.Forms.Label();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.SuspendLayout();
			// 
			// ProgressText
			// 
			this.ProgressText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ProgressText.Name = "ProgressText";
			this.ProgressText.Size = new System.Drawing.Size(440, 136);
			this.ProgressText.TabIndex = 3;
			this.ProgressText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// progressBar
			// 
			this.progressBar.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.progressBar.Location = new System.Drawing.Point(0, 136);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(440, 24);
			this.progressBar.Step = 1;
			this.progressBar.TabIndex = 2;
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter1.Enabled = false;
			this.splitter1.Location = new System.Drawing.Point(0, 133);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(440, 3);
			this.splitter1.TabIndex = 4;
			this.splitter1.TabStop = false;
			// 
			// ParserProgress
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.splitter1,
																		  this.ProgressText,
																		  this.progressBar});
			this.Name = "ParserProgress";
			this.Size = new System.Drawing.Size(440, 160);
			this.ResumeLayout(false);

		}
		#endregion

		public override string Text {
			set {
				ProgressText.Text = value;
			}
		}

		public int MaxProgress {
			set {
				progressBar.Maximum = value;
				progressBar.Value = 0;
			}
		}

		public void PerformProgressStep()
		{
			progressBar.PerformStep();
		}
	}
}
