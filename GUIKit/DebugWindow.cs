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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Opnieuw.Framework
{
    public class DebugWindow : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.DebugText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
			// 
			// DebugText
			// 
            this.DebugText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DebugText.Location = new System.Drawing.Point(0, 0);
            this.DebugText.Multiline = true;
            this.DebugText.Name = "DebugText";
            this.DebugText.Size = new System.Drawing.Size(292, 273);
            this.DebugText.TabIndex = 0;
			// 
			// Debug
			// 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.DebugText);
            this.Name = "Debug";
            this.Text = "Opnieuw Debug";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox DebugText;
        
        public DebugWindow()
        {
            InitializeComponent();
        }

        public void AppendText(string text)
        {
            DebugText.AppendText(text);
        }
    }
}