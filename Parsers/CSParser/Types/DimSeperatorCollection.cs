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
	public class DimSeperatorCollection : PieceOfCodeCollectionBase, IEnumerable 
	{
		public static DimSeperatorCollection parse(TokenProvider tokenizer)
		{
			DimSeperatorCollection ret = new DimSeperatorCollection();
			while (Token.COMMA == tokenizer.CurrentToken.Type)
			{
				ret.Add(new DimSeperator(tokenizer.CurrentToken));
				tokenizer.nextToken();
			}
			return ret;
		}

		public DimSeperatorCollection()
		{
		}
		public DimSeperatorCollection(DimSeperator ds)
		{
			Add(ds);
		}

		public void Add (DimSeperator value)
		{
			data.Add (value);
		}

		public new DimSeperator this[int index] {
			get {
				return data [index] as DimSeperator;
			}
			set {
				data [index] = value;
			}
		}

		private class Enumerator : PieceOfCodeCollectionBaseEnumerator
		{
			public Enumerator(DimSeperatorCollection collection) :
			base(collection)
			{
			}

			public new DimSeperator Current {
				get {
					if (currentIndex == collection.Count)
						throw new InvalidOperationException ();
					return collection [currentIndex] as DimSeperator;
				}
			}
		}

		public new IEnumerator GetEnumerator ()
		{
			return new DimSeperatorCollection.Enumerator (this);
		}
	}
}
