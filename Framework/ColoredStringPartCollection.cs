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
	public class ColoredStringPartCollection : ICollection, IEnumerable 
	{
		ArrayList data;

		//
		// Constructors
		//
		public ColoredStringPartCollection()
		{
			data = new ArrayList ();
		}
		public ColoredStringPartCollection(ColoredStringPart val)
		{
			data = new ArrayList();
			Add(val);
		}
		public ColoredStringPartCollection(ColoredStringPartCollection val)
		{
			data = new ArrayList();
			foreach(ColoredStringPart part in val)
			{
				Add(part);
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

		//
		// Methods
		//
		public void Add (ColoredStringPart value)
		{
			data.Add (value);
		}
		public void Add (ColoredStringPartCollection value)
		{
			foreach(ColoredStringPart part in value)
			{
				Add(part);
			}
		}

		public void Clear ()
		{
			data.Clear ();
		}
		
		public void Reverse ()
		{
			data.Reverse(0, data.Count);
		}
		
		private class Enumerator : IEnumerator {
			private ColoredStringPartCollection collection;
			private int currentIndex = -1;

			internal Enumerator (ColoredStringPartCollection collection)
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
			return new ColoredStringPartCollection.Enumerator (this);
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

		public ColoredStringPart this[int index] {
			get {
				return data [index] as ColoredStringPart;
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
