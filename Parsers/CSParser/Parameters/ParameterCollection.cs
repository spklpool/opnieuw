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
	public class ParameterCollection : PieceOfCode, IEnumerable {

		ArrayList data;
		
		//
		// Constructors
		//
		public ParameterCollection()
		{
			data = new ArrayList ();
		}
		public ParameterCollection(Parameter val)
		{
			data = new ArrayList();
			Add(val);
		}

		private void AdjustPosition()
		{
			foreach (object obj in data)
			{
				PieceOfCode poc = (PieceOfCode)obj;
				if (poc.StartsBefore(Position))
				{
					Position oldPosition = new Position(Position);
					m_Position = new Position(poc.Position, oldPosition);
				}
				if (poc.EndsAfter(Position))
				{
					Position oldPosition = new Position(Position);
					m_Position = new Position(oldPosition, poc.Position);
				}
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
		public void Add (Parameter value)
		{
			data.Add (value);
			AdjustPosition();
		}

		private class Enumerator : IEnumerator {
			private ParameterCollection collection;
			private int currentIndex = -1;

			internal Enumerator (ParameterCollection collection)
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
			return new ParameterCollection.Enumerator (this);
		}

		public Parameter this[int index] {
			get {
				return data [index] as Parameter;
			}

			set {
				data [index] = value;
			}
		}
	}
}
