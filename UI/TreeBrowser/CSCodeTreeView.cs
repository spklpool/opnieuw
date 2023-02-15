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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Windows.Forms;
using Opnieuw.Framework;
using Opnieuw.Parsers.CSParser;

namespace Opnieuw.UI.TreeBrowser
{
	public class CSCodeTreeView : CodeTreeView
	{
		public override void Select(ArrayList selectedNodes)
		{
			ClearSelectedNodes();
			foreach (PieceOfCode poc in selectedNodes)
			{
				foreach (TreeNode node in this.Nodes)
				{
					SelectNode(node, poc);
				}
			}
			paintSelectedNodes();
		}

		public void SelectNode(TreeNode node, PieceOfCode poc)
		{
			foreach (TreeNode child in node.Nodes)
			{
				if (TreeNodeMapper[child.Handle] == poc)
				{
					SelectedNodes.Add(child);
					child.EnsureVisible();
				}
				SelectNode(child, poc);
			}
		}

		protected CompilationUnitCollection m_CompilationUnits = new CompilationUnitCollection();

		public override void StartReloading()
		{
			base.BeginUpdate();
		}

		public override void EndReloading()
		{
			LoadTree();
			base.EndUpdate();
		}

		private void LoadTree()
		{
			Nodes.Clear();
			m_TreeNodeMapper.Clear();
			foreach (PieceOfCode poc in m_CompilationUnits.NamespaceMembers)
			{
				TreeNode root = AddTopLevelNodeToTree(poc.AsText, poc, 0);
				foreach (PieceOfCode child in poc.Children) {
					DisplayNodeNonRecursive(child, root);
				}
			}
		}

		public override void Remove(object unit)
		{
			m_CompilationUnits.Remove(unit);
		}

		public override void PopulateTree(CodeCompileUnit unit) 
		{
			base.PopulateTree(unit);
			if (unit != null)
			{
				CompilationUnit csUnit = unit.UserData["SourceCompilationUnit"] as CompilationUnit;
				if (false == (csUnit is MissingCompilationUnit))
				{
					m_CompilationUnits.Add(csUnit);
				}
			}
			ClearSelectedNodes();
		}

		protected TreeNode AddNodeToTree(TreeNode parent, string name, object data, int imageIndex) {
			if (data is NamespaceMember)
			{
				RemoveNodeIfAlreadyInTree(name, parent.Nodes);
			}
			TreeNode ret = parent.Nodes.Add(name);
			m_TreeNodeMapper.Add(ret.Handle, data);
			ret.ImageIndex = imageIndex;
			ret.SelectedImageIndex = imageIndex;
			return ret;
		}

		protected TreeNode AddTopLevelNodeToTree(string name, object data, int imageIndex) {
			if (data is NamespaceMember)
			{
				RemoveNodeIfAlreadyInTree(name, this.Nodes);
			}
			TreeNode ret = Nodes.Add(name);
			m_TreeNodeMapper.Add(ret.Handle, data);
			ret.ImageIndex = imageIndex;
			ret.SelectedImageIndex = imageIndex;
			return ret;
		}

		private void DisplayEvent(EventDeclaration poc, TreeNode node)
		{
			if (poc.Declarators.Count >= 0)
			{
				TreeNode childNode = AddNodeToTree(node, poc.AsText, poc, 25);
				if (poc.Declarators.Count == 1)
				{
					childNode.Text = poc.Declarators[0].AsText;
					if (false == (poc.Declarators[0].Initializer is MissingVariableInitializer))
					{
						AddNodeToTree(childNode, poc.Declarators[0].Initializer.AsText, poc.Declarators[0].Initializer, 10);
					}
				}
				else
				{
					foreach (VariableDeclarator declarator in poc.Declarators)
					{
						TreeNode childNode2 = AddNodeToTree(childNode, declarator.AsText, poc, 25);
						if (false == (declarator.Initializer is MissingVariableInitializer))
						{
							AddNodeToTree(childNode2, declarator.Initializer.AsText, declarator.Initializer, 10);
						}
					}
				}
			}
			else
			{
				AddNodeToTree(node, poc.AsText, poc, 25);
			}
		}

		private void DisplayField(FieldDeclaration poc, TreeNode node)
		{
			if (poc.Declarators.Count == 1)
			{
				TreeNode childNode = AddNodeToTree(node, poc.Declarators[0].AsText, poc, GetImageIndex(poc));
				if (false == (poc.Declarators[0].Initializer is MissingVariableInitializer))
				{
					AddNodeToTree(childNode, poc.Declarators[0].Initializer.AsText, poc.Declarators[0].Initializer, 10);
				}
			}
			else
			{
				TreeNode childNode = AddNodeToTree(node, poc.AsText, poc, 6);
				foreach (VariableDeclarator declarator in poc.Declarators)
				{
					TreeNode childNode2 = AddNodeToTree(childNode, declarator.AsText, poc, 6);
					if (false == (declarator.Initializer is MissingVariableInitializer))
					{
						AddNodeToTree(childNode2, declarator.Initializer.AsText, declarator.Initializer, 10);
					}
				}
			}
		}

		public override void OnBeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			PieceOfCode poc = TreeNodeMapper[e.Node.Handle] as PieceOfCode;
			if (poc.Children.Count > 0)
			{
				for (int i=1; i<poc.Children.Count; i++)
				{
					DisplayNodeNonRecursive(poc.Children[i], e.Node);
				}
			}
		}

		private void DisplayNodeNonRecursive(PieceOfCode poc, TreeNode node)
		{
			if(poc is EventDeclaration) {
				DisplayEvent(poc as EventDeclaration, node);
			} else if (poc is FieldDeclaration) {
				DisplayField(poc as FieldDeclaration, node);
			}
			else
			{
				TreeNode childNode = null;
				if (childNode == null)
				{
					string name = poc.AsText;
					childNode = AddNodeToTree(node, name, poc, GetImageIndex(poc));
				}
				if (poc.Children.Count > 0)
				{
					DisplayNodeNonRecursive(poc.Children[0], childNode);
				}
			}
		}

		private int GetImageIndex(object obj) {
			int ret = 0;
			if (obj is Class) {
				ret = 1;
			} 
			else if (obj is Opnieuw.Parsers.CSParser.Delegate) {
				ret = 3;
			} 
			else if (obj is Interface) {
				ret = 7;
			} 
			else if (obj is EnumDeclaration) {
				ret = 26;
			} else if (obj is AssignmentExpression) {
				ret = 20;
			} else if (obj is AdditiveExpression) {
				ret = 21;
			} else if (obj is Variable) {
				Variable v = obj as Variable;
				if (v.IsPrivate) {
					ret = 13;
				} else if (v.IsProtected) {
					ret = 14;
				} else {
					ret = 6;
				}
			} else if (obj is FieldDeclaration) {
				FieldDeclaration fd = obj as FieldDeclaration;
				if (fd.IsPrivate) {
					ret = 13;
				} else if (fd.IsProtected) {
					ret = 14;
				} else {
					ret = 6;
				}
			} 
			else if (obj is InterfaceMethod){
				ret = 8;
			} 
			else if (obj is InterfaceProperty){
				ret = 11;
			} 
			else if (obj is ConstructorDeclaration){
				ret = 8;
			} 
			else if (obj is DestructorDeclaration){
				ret = 8;
			} 
			else if (obj is MethodDeclaration){
				ret = 8;
			}
			else if (obj is EventDeclaration){
				ret = 25;
			} 
			else if (obj is PropertyDeclaration){
				ret = 11;
			} 
			else if (obj is Expression){
				ret = 10;
			} 
			else if (obj is ExpressionStatement){
				ret = 10;
			}
			return ret;
		}
	}
}
