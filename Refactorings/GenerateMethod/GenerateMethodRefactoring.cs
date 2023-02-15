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
using System.Windows.Forms;
using Opnieuw.Parsers.CSParser;
using Opnieuw.Framework;

namespace Opnieuw.Refactorings.GenerateMethod
{
	public delegate void PreviewTextChangedHandler(string newText);

	/// <summary>
	/// This is the class for the Generate Method refactoring.
	/// </summary>
	public class GenerateMethodRefactoring : Refactoring
	{
		public event PreviewTextChangedHandler PreviewTextChanged;

		/// <summary>
		/// Default constructor
		/// </summary>
		public GenerateMethodRefactoring()
		{
		}

		/// <summary>
		/// Checks if the event has any consumers and fires it.
		/// </summary>
		/// <param name="newText"></param>
		private void FirePreviewTextChanged(string newText)
		{
			if (PreviewTextChanged != null)
			{
				PreviewTextChanged(newText);
			}
		}

		private bool m_PreviousAvailability = false;

		/// <summary>
		/// Determines if the refactoring is available with the currently selected nodes.
		/// </summary>
		public bool IsAvailable {
			get {
				bool ret = false;
				if (m_SelectedNodes.Count == 1)
				{
					if ((m_SelectedNodes[0] is InvocationExpression) ||
						((m_SelectedNodes[0] as PieceOfCode).Parent is InvocationExpression))
					{
						ret = true;
					}
				} 
				return ret;
			}
		}

		/// <summary>
		/// Returns a string that previews the changes to be made by this refactoring with the
		/// selected nodes.
		/// </summary>
		public override string PreviewText {
			get {
				string ret = "";
				if (IsAvailable)
				{
					PieceOfCode currentNode = m_SelectedNodes[0] as PieceOfCode;
					while(currentNode.Parent is MemberAccessExpression)
					{
						currentNode = currentNode.Parent;
					}
					if ((currentNode.Parent is InvocationExpression) &&
						(m_SelectedNodes[0] is Identifier))
					{
						Identifier identifier = m_SelectedNodes[0] as Identifier;
						ret += identifier.Name;
					}
				}
				return ret;
			}
		}

		/// <summary>
		/// Returns the code changes necessary to execute the refactoring with the 
		/// current properties.
		/// </summary>
		public override CodeChangeCommandCollection CodeChanges {
			get 
			{
				CodeChangeCommandCollection ret = new CodeChangeCommandCollection();
				if (IsAvailable) 
				{
					
				}
				return ret;
			}
		}

		/// <summary>
		/// Event handler that gets called when the node selection changes in the user interface.
		/// </summary>
		/// <param name="selectedNodes"></param>
		public override void OnNodeSelectionChanged(object source, ArrayList selectedNodes)
		{
			m_SelectedNodes = selectedNodes;
			if (IsAvailable != m_PreviousAvailability)
			{
				FireAvailabilityChanged(IsAvailable);
				m_PreviousAvailability = IsAvailable;
			}
			FirePreviewTextChanged(PreviewText);
		}

		/// <summary>
		/// Returns the compilation unit that contains the selected nodes.
		/// </summary>
		protected CompilationUnit RootCompilationUnit {
			get 
			{
				PieceOfCode node = m_SelectedNodes[0] as PieceOfCode;
				return node.ParentCompilationUnit;
			}
		}

		/// <summary>
		/// Returns the class that contains the selected nodes or null if the nodes are 
		/// not siblings of any class.
		/// </summary>
		protected Class RootClass {
			get {
				Class ret = null;
				PieceOfCode lastClassNode = null;
				foreach (Object obj in m_SelectedNodes) 
				{
					if (obj is PieceOfCode)
					{
						PieceOfCode tempNode = obj as PieceOfCode;
						PieceOfCode node = tempNode;
						Class tempClass = null;
						do 
						{
							if (null != node.Parent) 
							{
								if (node is Class) 
								{
									tempClass = node as Class;
								} 
								else 
								{
									node = node.Parent;
								}
							}
						} while((tempClass == null) && (node != null) && (node.Parent != null));
						lastClassNode = node;
						if (null == tempClass) 
						{
							ret = null;
							break;
						}
						if ((ret != null) && (ret != tempClass)) 
						{
							ret = null;
							break;
						}
						ret = tempClass;
					}
				}
				return ret;
			}
		}
	}
}
