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
using System.CodeDom;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Opnieuw.Parsers.CSParser;

namespace Opnieuw.Framework
{
    public class MultiUnitView : UserControl
    {
        private System.Windows.Forms.TabControl UnitTabs;

        public MultiUnitView(CodeCompileUnitCollection units)
        {
            InitializeComponent();
			BuildFromUnits(units);
        }

        delegate void InvokeBuildFromUnitsDelegate(CodeCompileUnitCollection units);

		public void BuildFromUnits(CodeCompileUnitCollection units)
		{
            if(InvokeRequired)
            {
               InvokeBuildFromUnitsDelegate buildFromUnitsDel = new InvokeBuildFromUnitsDelegate(InvokeBuildFromUnits); 
               try
               {
                  Invoke(buildFromUnitsDel,new object[]{units});
               }
               catch{}
            }
            else
            {
               InvokeBuildFromUnits(units);
            }
		}

        public void InvokeBuildFromUnits(CodeCompileUnitCollection units)
        {
            UnitTabs.TabPages.Clear();
            foreach (CodeCompileUnit unit in units)
            {
                System.Windows.Forms.TabPage tabPage = new System.Windows.Forms.TabPage();
                CompilationUnit csUnit = unit.UserData["SourceCompilationUnit"] as CompilationUnit;
                Code code = new CSCode(csUnit.Generate());
                StaticCodeView codeView = new StaticCodeView(code);
                codeView.Location = new System.Drawing.Point(10, 10);
                codeView.Size = new System.Drawing.Size(100, 50);
                codeView.TabStop = false;
                codeView.Dock = System.Windows.Forms.DockStyle.Fill;
                codeView.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
                String filePath = unit.UserData["SourceFilePath"] as String;
                if (filePath != "")
                {
                    tabPage.Text = FileNameFromPath(filePath);
                    tabPage.ToolTipText = filePath;
                }
				PositionCollection highlights = unit.UserData["HighlightPositions"] as PositionCollection;
				if (highlights != null) {
					codeView.ClearHighlightedAreas();
					foreach (Position position in highlights) {
						Position adjustedPosition = code.AdjustPositionForExpandedTabs(position);
						adjustedPosition.EndCol ++;
						codeView.AddHighlightedArea(adjustedPosition);
                        codeView.RecalcCustomScrollBars();
                        codeView.ScrollToLine(adjustedPosition.StartLine);
                    }
				}
                tabPage.Controls.Add(codeView);
                UnitTabs.TabPages.Add(tabPage);
            }
        }

        public static string FileNameFromPath(string path)
        {
            return path.Substring(path.LastIndexOf("\\") + 1, path.Length - path.LastIndexOf("\\") - 1);
        }

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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.UnitTabs = new System.Windows.Forms.TabControl();
            this.UnitTabs.SuspendLayout();
            this.SuspendLayout();
// 
// UnitTabs
// 
            this.UnitTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UnitTabs.HotTrack = true;
            this.UnitTabs.Location = new System.Drawing.Point(0, 0);
            this.UnitTabs.Multiline = true;
            this.UnitTabs.Name = "UnitTabs";
            this.UnitTabs.SelectedIndex = 0;
            this.UnitTabs.TabIndex = 0;
            this.UnitTabs.BackColor = Color.White;
            this.UnitTabs.ForeColor = Color.Black;
// 
// MultiUnitView
// 
            this.Controls.Add(this.UnitTabs);
            this.Name = "MultiUnitView";
            this.Size = new System.Drawing.Size(392, 294);
            this.UnitTabs.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
