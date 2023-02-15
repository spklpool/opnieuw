using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Opnieuw.Framework;

namespace Opnieuw.UI.TreeBrowser
{
	/// <summary>
	/// Summary description for Options.
	/// </summary>
	public class OptionsDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button OK;
		private System.Windows.Forms.Button Cancel;
		private System.Windows.Forms.TabControl OptionTabs;
		private System.Windows.Forms.TabPage Refactorings;
		private System.Windows.Forms.TabPage General;
		private System.Windows.Forms.CheckBox ParseAfterExecution;
		private System.Windows.Forms.CheckBox ParseBeforeExecution;
		private System.Windows.Forms.ListBox RefactoringList;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public OptionsDialog()
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(OptionsDialog));
			this.OK = new System.Windows.Forms.Button();
			this.Cancel = new System.Windows.Forms.Button();
			this.OptionTabs = new System.Windows.Forms.TabControl();
			this.Refactorings = new System.Windows.Forms.TabPage();
			this.RefactoringList = new System.Windows.Forms.ListBox();
			this.General = new System.Windows.Forms.TabPage();
			this.ParseAfterExecution = new System.Windows.Forms.CheckBox();
			this.ParseBeforeExecution = new System.Windows.Forms.CheckBox();
			this.OptionTabs.SuspendLayout();
			this.Refactorings.SuspendLayout();
			this.General.SuspendLayout();
			this.SuspendLayout();
			// 
			// OK
			// 
			this.OK.Location = new System.Drawing.Point(360, 248);
			this.OK.Name = "OK";
			this.OK.Size = new System.Drawing.Size(72, 24);
			this.OK.TabIndex = 2;
			this.OK.Text = "&OK";
			this.OK.Click += new System.EventHandler(this.OK_Click);
			// 
			// Cancel
			// 
			this.Cancel.Location = new System.Drawing.Point(280, 248);
			this.Cancel.Name = "Cancel";
			this.Cancel.Size = new System.Drawing.Size(72, 24);
			this.Cancel.TabIndex = 3;
			this.Cancel.Text = "&Cancel";
			this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
			// 
			// OptionTabs
			// 
			this.OptionTabs.Controls.AddRange(new System.Windows.Forms.Control[] {
																					 this.Refactorings,
																					 this.General});
			this.OptionTabs.Name = "OptionTabs";
			this.OptionTabs.SelectedIndex = 0;
			this.OptionTabs.Size = new System.Drawing.Size(432, 240);
			this.OptionTabs.TabIndex = 4;
			// 
			// Refactorings
			// 
			this.Refactorings.Controls.AddRange(new System.Windows.Forms.Control[] {
																					   this.RefactoringList});
			this.Refactorings.Location = new System.Drawing.Point(4, 22);
			this.Refactorings.Name = "Refactorings";
			this.Refactorings.Size = new System.Drawing.Size(424, 214);
			this.Refactorings.TabIndex = 0;
			this.Refactorings.Text = "Refactorings";
			// 
			// RefactoringList
			// 
			this.RefactoringList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.RefactoringList.Name = "RefactoringList";
			this.RefactoringList.Size = new System.Drawing.Size(424, 212);
			this.RefactoringList.TabIndex = 0;
			// 
			// General
			// 
			this.General.Controls.AddRange(new System.Windows.Forms.Control[] {
																				  this.ParseAfterExecution,
																				  this.ParseBeforeExecution});
			this.General.Location = new System.Drawing.Point(4, 22);
			this.General.Name = "General";
			this.General.Size = new System.Drawing.Size(424, 214);
			this.General.TabIndex = 1;
			this.General.Text = "General";
			// 
			// ParseAfterExecution
			// 
			this.ParseAfterExecution.Location = new System.Drawing.Point(16, 48);
			this.ParseAfterExecution.Name = "ParseAfterExecution";
			this.ParseAfterExecution.Size = new System.Drawing.Size(144, 24);
			this.ParseAfterExecution.TabIndex = 3;
			this.ParseAfterExecution.Text = "Parse after execution";
			// 
			// ParseBeforeExecution
			// 
			this.ParseBeforeExecution.Location = new System.Drawing.Point(16, 16);
			this.ParseBeforeExecution.Name = "ParseBeforeExecution";
			this.ParseBeforeExecution.Size = new System.Drawing.Size(144, 24);
			this.ParseBeforeExecution.TabIndex = 2;
			this.ParseBeforeExecution.Text = "Parse before execution";
			// 
			// OptionsDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(434, 274);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.OptionTabs,
																		  this.Cancel,
																		  this.OK});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "OptionsDialog";
			this.ShowInTaskbar = false;
			this.Text = "Options";
			this.Load += new System.EventHandler(this.OptionsDialog_Load);
			this.OptionTabs.ResumeLayout(false);
			this.Refactorings.ResumeLayout(false);
			this.General.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void OK_Click(object sender, System.EventArgs e)
		{
			try
			{
				if (ConfigurationManager.Configuration.PropertyList.Count == 1)
				{
					//Reset the properties from the GUI
					OpnieuwConfiguration.PropertyRow[] properties = ConfigurationManager.Configuration.PropertyList[0].GetPropertyRows();
					foreach (OpnieuwConfiguration.PropertyRow property in properties)
					{
						if (property.Name == "ParseBeforeExecute")
						{
							property.Value = ParseBeforeExecution.Checked.ToString();
						}
						if (property.Name == "ParseAfterExecute")
						{
							property.Value = ParseAfterExecution.Checked.ToString();
						}
					}
				}
				ConfigurationManager.Save();
			}
			catch (Exception except)
			{
				Console.WriteLine(except.Message);
				Console.WriteLine(except.StackTrace);
			}
			this.Hide();
		}

		private void Cancel_Click(object sender, System.EventArgs e)
		{
			this.Hide();
		}

		private void OptionsDialog_Load(object sender, System.EventArgs e)
		{
			try
			{
				ConfigurationManager.Load();

				//Display the list of refactorings
				if (ConfigurationManager.Configuration.RefactoringList.Count == 1)
				{
					OpnieuwConfiguration.RefactoringRow[] refactorings = ConfigurationManager.Configuration.RefactoringList[0].GetRefactoringRows();
					foreach (OpnieuwConfiguration.RefactoringRow refactoring in refactorings)
					{
						RefactoringList.Items.Add(refactoring.FriendlyName);
					}
				}

				ParseBeforeExecution.Checked = ConfigurationManager.ParseBeforeExecute;
				ParseAfterExecution.Checked = ConfigurationManager.ParseAfterExecute;
			}
			catch (Exception except)
			{
				Console.WriteLine(except.Message);
				Console.WriteLine(except.StackTrace);
			}
		}

		private void OnBeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
		
		}

		private void ddd(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
		
		}

	}
}
