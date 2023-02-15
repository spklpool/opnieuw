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
	public class DataTypeCollection : PieceOfCodeCollectionBase, IEnumerable 
	{
		public DataTypeCollection()
		{
		}
		public DataTypeCollection(DataType val)
		{
			Add(val);
		}

		public override string Generate()
		{
			string ret = "";
			for (int i=0; i<data.Count; i++)
			{
				ret += this[i].Generate();
				if (i != (data.Count - 1))
				{
					//TODO:  This should be a token with leading characters and stuff.
					ret += ",";
				}
			}
			return ret;
		}

		public void Add (DataType value)
		{
			data.Add (value);
			AdjustPosition();
		}

		public new DataType this[int index] {
			get {
				return data [index] as DataType;
			}
			set {
				data [index] = value;
			}
		}

		private class Enumerator : PieceOfCodeCollectionBaseEnumerator
		{
			public Enumerator(DataTypeCollection collection) :
			base(collection)
			{
			}

			public new DataType Current {
				get {
					if (currentIndex == collection.Count)
						throw new InvalidOperationException ();
					return collection [currentIndex] as DataType;
				}
			}
		}
		
		public new IEnumerator GetEnumerator ()
		{
			return new DataTypeCollection.Enumerator (this);
		}
	}
}
