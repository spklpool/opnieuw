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
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.CodeDom;
using System.CodeDom.Compiler;
using Opnieuw.Framework;

namespace Opnieuw.UI.TreeBrowser
{
	/// <summary>
	/// This is a tree view control that has the ability to display code elements
	/// using Windows.Forms.
	/// </summary>
	public abstract class CodeTreeView : MultiSelectableTreeView, ICodeTreeView
	{
		public event CodeTreeViewEventHandler NodeSelectionChangedWithInfo;
		private System.Windows.Forms.ImageList Icons;
		private System.ComponentModel.IContainer components;
		protected System.Collections.Hashtable m_TreeNodeMapper = new System.Collections.Hashtable();

		public abstract void StartReloading();
		public abstract void EndReloading();
		public abstract void Select(ArrayList selectedNodes);
		public abstract void Remove(object unit);

		/// <summary>
		/// Constructor.
		/// </summary>
		public CodeTreeView() {
			InitializeComponent();
		}

		/// <summary>
		/// Initializes this component.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(CodeTreeView));
			this.Icons = new System.Windows.Forms.ImageList(this.components);
			// 
			// Icons
			// 
			this.Icons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.Icons.ImageSize = new System.Drawing.Size(16, 16);
			this.Icons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("Icons.ImageStream")));
			this.Icons.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// CodeTreeView
			// 
			this.ImageList = this.Icons;
			this.NodeSelectionChanged += new MultiSelectableTreeViewEventHandler(this.MultiSelectableTreeView_NodeSelectionChanged);
			this.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.OnBeforeExpand);
		}

		public virtual void OnBeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
		}

		protected CodeCompileUnit m_Unit = null;
		public virtual void PopulateTree(CodeCompileUnit unit)
		{
			m_Unit = unit;
		}

		/// <summary>
		/// Event handler that gets called when the selected nodes change in the tree view.
		/// </summary>
		/// <param name="selectedNodes"></param>
		private void MultiSelectableTreeView_NodeSelectionChanged(ArrayList selectedNodes)
		{
			if (null != NodeSelectionChangedWithInfo)
			{
				ArrayList codeNodes = new ArrayList();
				foreach (TreeNode node in selectedNodes)
				{
					codeNodes.Add(m_TreeNodeMapper[node.Handle]);
				}
				NodeSelectionChangedWithInfo(this, codeNodes);
			}
		}

		public Hashtable TreeNodeMapper 
		{
			get {
				return m_TreeNodeMapper;
			}
		}

		private void RemoveNodeFromMapper(TreeNode node) {
			foreach (TreeNode child in node.Nodes) {
				RemoveNodeFromMapper(child);
			}
			m_TreeNodeMapper.Remove(node.Handle);
		}

		public void RemoveNodeIfAlreadyInTree(string name, TreeNodeCollection parentNodes) {
			foreach (TreeNode node in parentNodes) {
				if (node.Text == name) {
					RemoveNodeFromMapper(node);
					parentNodes.Remove(node);
					break;
				}
			}
		}
		
		public void ClearNodes()
		{
			m_TreeNodeMapper.Clear();
			Nodes.Clear();
		}
	}
}
