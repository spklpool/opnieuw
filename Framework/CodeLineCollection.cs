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

namespace Opnieuw.Framework
{
	public class CodeLineCollection : ICollection, IEnumerable 
	{
		ArrayList data = new ArrayList();

		// Constructors
		public CodeLineCollection()
		{
		}

		public CodeLineCollection(CodeLine val)
		{
			Add(val);
		}

		// Properties
		public int Count {
			get {
				return data.Count;
			}
		}

		// Methods
		public void Add (CodeLine value)
		{
			data.Add (value);
		}

		public void AddRange (CodeLine [] values)
		{
			foreach (CodeLine ca in values) 
				data.Add (ca);
		}

		public void Clear ()
		{
			data.Clear ();
		}

		private class Enumerator : IEnumerator {
			private CodeLineCollection collection;
			private int currentIndex = -1;

			internal Enumerator (CodeLineCollection collection)
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
			return new CodeLineCollection.Enumerator (this);
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

		/// <summary>
		/// Inserts an element after the one at the specified index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		public void InsertAfter (int index, Object value)
		{
			ArrayList temp = new ArrayList();
			int i = 0;
			while (i<=index)
			{
				temp.Add(data[i]);
				i++;
			}
			temp.Add(value);
			while (i<data.Count)
			{
				temp.Add(data[i]);
				i++;
			}
			data = temp;
		}

		public void Insert (int index, Object value)
		{
			data.Insert(index, value);
		}

		public CodeLine this[int index] {
			get {
				return data [index] as CodeLine;
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
