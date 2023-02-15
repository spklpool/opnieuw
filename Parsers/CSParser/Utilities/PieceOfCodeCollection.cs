#region Copyright (C) Pierre Boudreau
//This file is part of the Opnieuw C# parser.
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
using Opnieuw.Framework;

namespace Opnieuw.Parsers.CSParser
{
	public class PieceOfCodeCollection : IEnumerable 
	{
		ArrayList data;

		//
		// Constructors
		//
		public PieceOfCodeCollection()
		{
			data = new ArrayList ();
		}
		public PieceOfCodeCollection(PieceOfCode val)
		{
			data = new ArrayList();
			Add(val);
		}

		//
		// Properties
		//
		public int Count {
			get {
				return data.Count;
			}
		}

		//
		// Methods
		//
		public void Add (PieceOfCode value)
		{
			data.Add (value);
		}

		public void Add (PieceOfCodeCollection value)
		{
			foreach (PieceOfCode poc in value)
			{
				Add (poc);
			}
		}

		public void Clear ()
		{
			data.Clear ();
		}

		public void Reverse ()
		{
			data.Reverse();
		}

		private class Enumerator : IEnumerator {
			private PieceOfCodeCollection collection;
			private int currentIndex = -1;

			internal Enumerator (PieceOfCodeCollection collection)
			{
				this.collection = collection;
			}

			public object Current {
				get {
					if (currentIndex == collection.Count)
						throw new InvalidOperationException ();
					return collection [currentIndex];
				}
			}

			public bool MoveNext ()
			{
				if (currentIndex > collection.Count)
					throw new InvalidOperationException ();
				return ++currentIndex < collection.Count;
			}

			public void Reset ()
			{
				currentIndex = -1;
			}
		}
		
		public IEnumerator GetEnumerator ()
		{
			return new PieceOfCodeCollection.Enumerator (this);
		}

		public int Add (object value)
		{
			return data.Add (value);
		}

		public bool Contains (Object value)
		{
			return data.Contains (value);
		}

		public bool DeepContains (PieceOfCode poc)
		{
			bool ret = data.Contains (poc);
			if (false == ret)
			{
				foreach (PieceOfCode child in this)
				{
					ret = child.Children.DeepContains(poc);
					if (true == ret)
					{
						break;
					}

				}
			}
			return ret;
		}

		public bool ContainsAllOf (PieceOfCodeCollection col)
		{
			bool ret = true;
			foreach (PieceOfCode val in col)
			{
				if (false == Contains(val))
				{
					ret = false;
					break;
				}
			}
			return ret;
		}

		public PieceOfCode this[int index] {
			get {
				return data [index] as PieceOfCode;
			}
			set {
				data [index] = value;
			}
		}

		/// <summary>
		/// Returns the compilation unit that contains the nodes in the collection.
		/// </summary>
		public CompilationUnit RootCompilationUnit {
			get 
			{
				if (this.Count > 0) {
					return this[0].ParentCompilationUnit;
				} else {
					return null;
				}
			}
		}
		
		/// <summary>
		/// Returns the namespace that contains the nodes in the collection or 
		/// null if the nodes are not siblings of any namespace.
		/// </summary>
		public Namespace RootNamespace {
			get {
				Namespace ret = null;
				FundamentalPieceOfCode lastNamespaceNode = null;
				foreach (PieceOfCode poc in this) {
					FundamentalPieceOfCode node = poc;
					Namespace tempNamespace = null;
					do {
						if (null != node.Parent) {
							if (node is Namespace) {
								tempNamespace = node as Namespace;
							} else {
								node = node.Parent;
							}
						}
					} while((tempNamespace == null) && 
                            (node != null) && (node.Parent != null));
					lastNamespaceNode = node;
					if (null == tempNamespace) {
						ret = null;
						break;
					}
					if ((ret != null) && (ret != tempNamespace)) {
						ret = null;
						break;
					}
					ret = tempNamespace;
				}
				return ret;			
            }
		}

		/// <summary>
		/// Return the parent class member that contains the nodes.  Assumes that all 
		/// nodes are in the same class member.
		/// </summary>
		public PieceOfCode RootClassMember {
			get {
				PieceOfCode node = this[0] as PieceOfCode;
				return (node.ParentClassMember as PieceOfCode);
			}
		}
		
		/// <summary>
		/// Returns the class that contains nodes in the collection or 
		/// null if the nodes are not siblings of any class.
		/// </summary>
		public Class RootClass {
			get {
				Class ret = null;
				FundamentalPieceOfCode lastClassNode = null;
				foreach (PieceOfCode poc in this) {
					FundamentalPieceOfCode node = poc;
					Class tempClass = null;
					do {
						if (null != node.Parent) {
							if (node is Class) {
								tempClass = node as Class;
							} else {
								node = node.Parent;
							}
						}
					} while((tempClass == null) && 
                            (node != null) && (node.Parent != null));
					lastClassNode = node;
					if (null == tempClass) {
						ret = null;
						break;
					}
					if ((ret != null) && (ret != tempClass)) {
						ret = null;
						break;
					}
					ret = tempClass;
				}
				return ret;
			}
		}

		/// <summary>
		/// Returns a collection of variables declared in the scope that immediately 
		/// contains all the selected statements.
		/// </summary>
		public VariableCollection LocalScopeVariables {
			get {
				return RootClassMember.DeepDeclaredVariables;
			}
		}

		/// <summary>
		/// Returns a collection of all the declared variables in the statements
		/// to extract.
		/// </summary>
		public VariableCollection DeclaredVariables {
			get {
				VariableCollection ret = new VariableCollection();
				foreach (PieceOfCode poc in this)
				{
					ret.Add(poc.DeepDeclaredVariables);
				}
				return ret;
			}
		}

        public VariableCollection ExternallyVisibleDeclaredVariables {
            get {
                VariableCollection ret = new VariableCollection();
                foreach (Variable v in DeclaredVariables) {
                    if (!this.Contains(v.ContainingScope)) {
                        ret.Add(v);
                    }
                }
                return ret;
            }
        }

		/// <summary>
		/// Returns a collection of local scope variables that get referenced by 
		/// the nodes in the collection.
		/// </summary>
		public VariableCollection ReferencedLocalScopeVariables {
			get {
				VariableCollection ret = new VariableCollection();
				foreach (Variable variable in LocalScopeVariables)
				{
					foreach (PieceOfCode poc in this)
					{
						if (poc.DeepReferencedVariables.Contains(variable) == true)
						{
							if (DeclaredVariables.Contains(variable) == false)
							{
								ret.Add(variable);
								break;
							}
						}
					}
				}
				return ret;
			}
		}

		/// <summary>
		/// Returns a collection of local scope variables that get modified by 
		/// the selected nodes.
		/// </summary>
		public VariableCollection ModifiedLocalScopeVariables {
			get {
				VariableCollection ret = new VariableCollection();
				foreach (Variable variable in LocalScopeVariables)
				{
					foreach (PieceOfCode poc in this)
					{
						if (poc.DeepModifiedVariables.Contains(variable) == true)
						{
							if (DeclaredVariables.Contains(variable) == false)
							{
								ret.Add(variable);
								break;
							}
						}
					}
				}
				return ret;
			}
		}

		/// <summary>
		/// Returns a collection of the referenced variables in the selected code that
		/// are not modified within the selected code.
		/// </summary>
		public VariableCollection ReferencedUnmodifiedLocalScopeVariables {
			get {
				VariableCollection ret = new VariableCollection();
				foreach (Variable v in ReferencedLocalScopeVariables)
				{
					if (ModifiedLocalScopeVariables.Contains(v) == false)
					{
						ret.Add(v);
					}
				}
				return ret;
			}
		}

		/// <summary>
		/// Returns the first common parent of all the nodes.
		/// </summary>
		public PieceOfCode ParentOfSelectedNodes {
			get	{
				PieceOfCode ret = new MissingPieceOfCode();
				if (Count > 0) {
					if (false == ((this[0] as FundamentalPieceOfCode).Parent is MissingPieceOfCode)) {
                        if ((this[0] as FundamentalPieceOfCode).Parent is PieceOfCode) {
                            ret = (this[0] as FundamentalPieceOfCode).Parent as PieceOfCode;
						    foreach (PieceOfCode poc in this) {
                                PieceOfCode test = (poc as FundamentalPieceOfCode).Parent as PieceOfCode;
							    if (test != ret) {
								    ret = new MissingPieceOfCode();
								    break;
							    }
						    }
                        }
					}
				}
				return ret;
			}
		}

		/// <summary>
		/// Returns variables that are declared in the extracted nodes and referenced 
		/// outside of them.
		/// </summary>
		public VariableCollection NeededVariableDeclarations {
			get {
				VariableCollection ret = new VariableCollection();
				if (false == (ParentOfSelectedNodes is MissingPieceOfCode))
				{
					VariableCollection declaredVariablesInSelectedNodes = ExternallyVisibleDeclaredVariables;
					foreach (PieceOfCode poc in ParentOfSelectedNodes.DeepChildren)
					{
						if ((!DeepContains(poc)) && // poc is not extracted
							(!poc.DeepChildren.ContainsAllOf(this))) // poc is not parent of all extracted nodes
						{
							foreach (Variable v in declaredVariablesInSelectedNodes)
							{
								if (true == poc.DeepReferencedVariables.Contains(v))
								{
									ret.Add(v);;
								}
							}
						}
					}
				}
				return ret;
			}
		}
	}
}
