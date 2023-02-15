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
	public class StructMemberCollection : PieceOfCodeCollectionBase, IEnumerable 
	{
		public static StructMemberCollection parse(TokenProvider tokenizer)
		{
			StructMemberCollection ret = new StructMemberCollection();
			StructMember member = CompilationUnit.parseStructMember(tokenizer);
			while (false == (member is MissingStructMember))
			{
				ret.Add(member);
				member = CompilationUnit.parseStructMember(tokenizer);
			}
			return ret;
		}

		public StructMemberCollection()
		{
		}
		public StructMemberCollection(StructMember val)
		{
			Add(val);
		}

		public void Add (StructMember value)
		{
			data.Add (value as FundamentalPieceOfCode);
			AdjustPosition();
		}

		public new StructMember this[int index] {
			get {
				return data [index] as StructMember;
			}
		}

		private class Enumerator : PieceOfCodeCollectionBaseEnumerator
		{
			public Enumerator(StructMemberCollection collection) :
			base(collection)
			{
			}

			public new StructMember Current {
				get {
					if (currentIndex == collection.Count)
						throw new InvalidOperationException ();
					return collection [currentIndex] as StructMember;
				}
			}
		}
		
		public new IEnumerator GetEnumerator ()
		{
			return new StructMemberCollection.Enumerator (this);
		}
	}
}
