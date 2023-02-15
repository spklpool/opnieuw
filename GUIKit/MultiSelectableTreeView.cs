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
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

namespace Opnieuw.Framework
{
	public delegate void MultiSelectableTreeViewEventHandler(ArrayList nodes);

	public class MultiSelectableTreeView : TreeView
	{
		public event MultiSelectableTreeViewEventHandler NodeSelectionChanged;

		public ArrayList m_coll = new ArrayList();
		public TreeNode m_firstNodeForShift = null;

		public MultiSelectableTreeView()
		{
			InitializeComponent();
		}

		private void InitializeComponent() 
		{
			this.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.CodeTreeView_BeforeSelect);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CodeTreeView_MouseDown);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CodeTreeView_MouseUp);
		}

		public ArrayList SelectedNodes {
			get	{
				return m_coll;
			}
			set	{
				removePaintFromNodes();
				m_coll.Clear();
				m_coll = value;
				paintSelectedNodes();
			}
		}

		public void ClearSelectedNodes() 
		{
			removePaintFromNodes();
			m_coll.Clear();
			paintSelectedNodes(); // is this needed?
		}

		public void paintSelectedNodes() 
		{
			BeginUpdate();
			foreach ( TreeNode n in m_coll ) {
				n.BackColor = SystemColors.Highlight;
				n.ForeColor = SystemColors.HighlightText;
			}
			EndUpdate();
		}

		protected void removePaintFromNodes() 
		{
			if (m_coll.Count==0) return;

			//TreeView property will be null if node is no longer
			//in tree.
			BeginUpdate();
			TreeNode n0 = m_coll[0] as TreeNode;
			if (null != n0.TreeView) {
				Color back = n0.TreeView.BackColor;
				Color fore = n0.TreeView.ForeColor;
				foreach ( TreeNode n in m_coll ) {
					if (n.BackColor != back) {
						n.BackColor = back;
					}
					if (n.ForeColor != fore) {
						n.ForeColor = fore;
					}
				}
			}
			EndUpdate();
		}

		private void CodeTreeView_BeforeSelect(object sender, System.Windows.Forms.TreeViewCancelEventArgs e) 
		{
			//Disable normal selection mechanism.  We will implement our own.
			e.Cancel = true;
		}

		private void CodeTreeView_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) 
		{
			FireNodeSelectionChanged();
		}

		private void MouseDownWithControlPressed(TreeNode node)
		{
			if(m_coll.Contains(node)) {
				m_coll.Remove(node);
			} else {
				m_coll.Add(node);
			}
			m_firstNodeForShift = node;
		}

		private void SelectVisibleNodesBetween(TreeNode uppernode, TreeNode bottomnode)
		{
			m_coll.Add(uppernode);
			TreeNode tempNode = uppernode;
			do {
				tempNode = tempNode.NextVisibleNode;
				if(null != tempNode) {
					m_coll.Add(tempNode);
				}
			}while((tempNode != null) && (tempNode != bottomnode));
		}

		private TreeNode InvertNodesIfNecessary(ref TreeNode uppernode, ref TreeNode bottomnode)
		{
			if(!(IsAbove(uppernode, bottomnode))) {
				TreeNode t = uppernode;
				uppernode = bottomnode;
				bottomnode = t;
			}
			return uppernode;
		}

		private void MouseDownWithShiftPressed(TreeNode node)
		{
			m_coll.Clear();
			TreeNode uppernode = m_firstNodeForShift;
			TreeNode bottomnode = node;
			uppernode = InvertNodesIfNecessary(ref uppernode, ref bottomnode);
			SelectVisibleNodesBetween(uppernode, bottomnode);
		}

		private void CodeTreeView_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) 
		{
			TreeNode node = GetNodeAt(e.X, e.Y);
			if (null != node) {
				removePaintFromNodes();
				if (ModifierKeys==Keys.Control) {
					MouseDownWithControlPressed(node);
				} else if (ModifierKeys==Keys.Shift) {
					MouseDownWithShiftPressed(node);
				} else {
					m_coll.Clear();
					m_coll.Add(node);
					m_firstNodeForShift = node;
				}
				paintSelectedNodes();
			}
		}

		public void FireNodeSelectionChanged()
		{
			if (null != NodeSelectionChanged) {
				NodeSelectionChanged(SelectedNodes);
			}
		}

		private bool IsAbove(TreeNode top, TreeNode bottom)
		{
			TreeNode tempNode = top;
			do {
				tempNode = tempNode.NextVisibleNode;
			}while ((tempNode != null) && (tempNode != bottom));
			return (tempNode == bottom);
		}
	}
}
