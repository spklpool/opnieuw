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

using Opnieuw.Framework;

namespace Opnieuw.Parsers.CSParser 
{
	using System;
	using System.Collections;

	public class IdentifierCollection : PieceOfCodeCollectionBase, IEnumerable 
	{
		public IdentifierCollection ()
		{
		}
		
		public IdentifierCollection (Identifier identifier)
		{
			Add(identifier);
		}

		public override Position Position {
			get {
				if (m_Position is InvalidPosition)
				{
					foreach (PieceOfCode poc in data)
					{
						if (m_Position is InvalidPosition)
						{
							m_Position = new Position(poc);
						}
						else
						{
							m_Position = new Position(m_Position, poc.Position);
						}
					}
				}
				return m_Position;
			}
		}

		public void Add (Identifier value)
		{
			data.Add (value);
		}

		public void Add (QualifiedIdentifier value)
		{
			Add (value.Identifiers);
		}

		public void Add (IdentifierCollection value)
		{
			foreach (Identifier id in value)
			{
				Add (id);
			}
		}

		public new Identifier this[int index] {
			get {
				return data [index] as Identifier;
			}
			set {
				data [index] = value;
			}
		}

		private class Enumerator : PieceOfCodeCollectionBaseEnumerator
		{
			public Enumerator(IdentifierCollection collection) :
			base(collection)
			{
			}

			public new Identifier Current {
				get {
					if (currentIndex == collection.Count)
						throw new InvalidOperationException ();
					return collection [currentIndex] as Identifier;
				}
			}
		}
		
		public new IEnumerator GetEnumerator ()
		{
			return new IdentifierCollection.Enumerator (this);
		}
	}
}
