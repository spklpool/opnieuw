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
using System.IO;
using System.Collections;
using Opnieuw.Framework;

namespace Opnieuw.Parsers.CSParser
{
	public class InterfaceMemberCollection : PieceOfCodeCollectionBase, IEnumerable 
	{
		public static InterfaceMemberCollection parse(TokenProvider tokenizer)
		{
			InterfaceMemberCollection ret = new InterfaceMemberCollection();
			InterfaceMember member = InterfaceMember.parse(tokenizer);
			while (false == (member is MissingInterfaceMember))
			{
				ret.Add(member);
				member = InterfaceMember.parse(tokenizer);
			}
			return ret;
		}
		
		public static InterfaceMemberCollection parse(string input)
		{
			Tokenizer t = new Tokenizer(new StringReader(input), "", null);
            t.nextToken();
            return InterfaceMemberCollection.parse(t) as InterfaceMemberCollection;
		}
		
		public InterfaceMemberCollection()
		{
		}
		public InterfaceMemberCollection(InterfaceMember val)
		{
			Add(val);
		}

		public void Add (InterfaceMember value)
		{
			data.Add (value);
			AdjustPosition();
		}

		public new InterfaceMember this[int index] {
			get {
				return data [index] as InterfaceMember;
			}
			set {
				data [index] = value;
			}
		}

		private class Enumerator : PieceOfCodeCollectionBaseEnumerator
		{
			public Enumerator(InterfaceMemberCollection collection) :
			base(collection)
			{
			}

			public new InterfaceMember Current {
				get {
					if (currentIndex == collection.Count)
						throw new InvalidOperationException ();
					return collection [currentIndex] as InterfaceMember;
				}
			}
		}
		
		public new IEnumerator GetEnumerator ()
		{
			return new InterfaceMemberCollection.Enumerator (this);
		}
	}
}
