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
using Opnieuw.Framework;

namespace Opnieuw.Framework
{
	public class CodeReplacementCollection : ICollection, IEnumerable 
	{
		ArrayList data;

		//
		// Constructors
		//
		public CodeReplacementCollection()
		{
			data = new ArrayList ();
		}
		public CodeReplacementCollection(CodeReplacement val)
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
		public void Add (CodeReplacement value)
		{
			int insertPosition = 0;
			for (int i=data.Count; insertPosition>0; insertPosition--)
			{
				Position tempPosition = (data[i] as CodeReplacement).Position;
				if (value.Position.StartsBefore(tempPosition))
				{
					insertPosition = 1;
					break;
				}
			}
			data.Insert(insertPosition, value);
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
			private CodeReplacementCollection collection;
			private int currentIndex = -1;

			internal Enumerator (CodeReplacementCollection collection)
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
			return new CodeReplacementCollection.Enumerator (this);
		}

		public int Add (object value)
		{
			return data.Add (value);
		}

		public bool Contains (Object value)
		{
			return data.Contains (value);
		}

		public int IndexOf (Object value)
		{
			return data.IndexOf (value);
		}

		public void Insert (int index, Object value)
		{
			data [index] = value;
		}

		public CodeReplacement this[int index] {
			get {
				return data [index] as CodeReplacement;
			}

			set {
				data [index] = value;
			}
		}

		public void Remove (object value)
		{
			data.Remove (value);
		}

		public void RemoveAt (int index)
		{
			data.RemoveAt (index);
		}

		//
		// ICollection method implementations
		//
		public void CopyTo (Array array, int index)
		{
			data.CopyTo (array, index);
		}

		public object SyncRoot {
			get {
				return data.SyncRoot;
			}
		}

		public bool IsReadOnly {
			get {
				return false;
			}
		}

		public bool IsSynchronized {
			get {
				return data.IsSynchronized;
			}
		}

		public bool IsFixedSize {
			get {
				return false;
			}
		}
	}
}
