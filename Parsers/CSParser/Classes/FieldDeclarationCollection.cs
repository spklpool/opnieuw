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
	public class FieldDeclarationCollection : PieceOfCodeCollectionBase, IEnumerable 
	{
		public FieldDeclarationCollection()
		{
		}
		public FieldDeclarationCollection(FieldDeclaration val)
		{
			Add(val);
		}

		public void Add (FieldDeclaration value)
		{
			data.Add (value);
			AdjustPosition();
		}

		public new FieldDeclaration this[int index] {
			get {
				return data [index] as FieldDeclaration;
			}
			set {
				data [index] = value;
			}
		}

		private class Enumerator : PieceOfCodeCollectionBaseEnumerator
		{
			public Enumerator(FieldDeclarationCollection collection) :
			base(collection)
			{
			}

			public new FieldDeclaration Current {
				get {
					if (currentIndex == collection.Count)
						throw new InvalidOperationException ();
					return collection [currentIndex] as FieldDeclaration;
				}
			}
		}
		
		public new IEnumerator GetEnumerator ()
		{
			return new FieldDeclarationCollection.Enumerator (this);
		}
	}
}