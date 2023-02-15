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
	public class CompilationUnitCollection : IEnumerable 
	{
		ArrayList data;

		//
		// Constructors
		//
		public CompilationUnitCollection()
		{
			data = new ArrayList ();
		}
		public CompilationUnitCollection(CompilationUnit val)
		{
			data = new ArrayList();
			Add(val);
		}
		public CompilationUnitCollection(object obj)
		{
			data = new ArrayList();
			if (obj is CompilationUnit)
			{
				Add(obj);
			}
		}

		//
		// Properties
		//
		public int Count {
			get {
				return data.Count;
			}
		}

		/// <summary>
		/// Returns a collection of namespace members found in all the 
		/// compilation units.  The classes and other members from
		/// different compilation units are grouped together in the
		/// appropriate namespaces.
		/// </summary>
		public NamespaceMemberCollection NamespaceMembers {
			get {
				NamespaceMemberCollection ret = new NamespaceMemberCollection();
				foreach (CompilationUnit unit in this)
				{
					foreach (NamespaceMember member in unit.Members)
					{
						ret.AddFullyQualifiedNamespaceMember(member);
					}
				}
				return ret;
			}
		}

		//
		// Methods
		//
		public void Add (CompilationUnit value)
		{
			bool alreadyContains = false;
			foreach (CompilationUnit unit in this)
			{
				if (unit.Generate() == value.Generate())
				{
					alreadyContains = true;
					break;
				}
			}
			if (alreadyContains == false)
			{
				data.Add (value);
			}
		}
		
		public void Remove (object value)
		{
			data.Remove (value);
		}

		public void Clear ()
		{
			data.Clear ();
		}

		public CompilationUnit this[int index] {
			get {
				return data [index] as CompilationUnit;
			}
			set {
				data [index] = value;
			}
		}

		private class Enumerator : IEnumerator {
			private CompilationUnitCollection collection;
			private int currentIndex = -1;

			internal Enumerator (CompilationUnitCollection collection)
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
			return new CompilationUnitCollection.Enumerator (this);
		}

		public int Add (object value)
		{
			return data.Add (value);
		}
	}
}
