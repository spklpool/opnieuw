#region Copyright (C) Pierre Boudreau
//This file is part of the Opnieuw C# parser.
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

namespace Opnieuw.Parsers.CSParser 
{
	using System;
	using System.Collections;

	public class NamespaceCollection : PieceOfCodeCollectionBase, IEnumerable 
	{
		public NamespaceCollection ()
		{
		}

		public void Add (Namespace value)
		{
			data.Add (value);
		}

        public new Namespace this[int index] {
			get {
				return data [index] as Namespace;
			}
			set {
				data [index] = value;
			}
		}

		private class Enumerator : PieceOfCodeCollectionBaseEnumerator
		{
			public Enumerator(NamespaceCollection collection) :
			base(collection)
			{
			}

			public new Namespace Current {
				get {
					if (currentIndex == collection.Count)
						throw new InvalidOperationException ();
					return collection [currentIndex] as Namespace;
				}
			}
		}

		public new IEnumerator GetEnumerator ()
		{
			return new NamespaceCollection.Enumerator (this);
		}
	}
}
