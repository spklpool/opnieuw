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
	public class EnumMemberCollection : CommaSeperatedCollection, IEnumerable 
	{
		/// <summary>
		/// enum-member-declaration
		/// enum-member-declarations , enum-member-declaration 
		/// </summary>
		/// <param name="tokenizer"></param>
		/// <returns></returns>
		public static EnumMemberCollection parse(TokenProvider tokenizer)
		{
			EnumMemberCollection ret = new EnumMemberCollection();
			EnumMember member = EnumMember.parse(tokenizer);
			while (false == (member is MissingEnumMember))
			{
				if (tokenizer.CurrentToken.Type == Token.COMMA)
				{
					ret.Add(member, tokenizer.CurrentToken);
				}
				else
				{
					ret.Add(member);
				}
				if (Token.COMMA == tokenizer.CurrentToken.Type)
				{
					tokenizer.nextToken();
					member = EnumMember.parse(tokenizer);
				}
				else
				{
					break;
				}
			}
			return ret;
		}

		public EnumMemberCollection()
		{
		}

		public void Add (EnumMember value)
		{
			data.Add (value);
			AdjustPosition();
		}
		public void Add (EnumMember value, PositionToken commaToken)
		{
			data.Add (value);
			commas.Add (commaToken);
			AdjustPosition();
		}

		public new EnumMember this[int index] {
			get {
				return data [index] as EnumMember;
			}
			set {
				data [index] = value;
			}
		}

		private class Enumerator : PieceOfCodeCollectionBaseEnumerator
		{
			public Enumerator(EnumMemberCollection collection) :
			base(collection)
			{
			}

			public new EnumMember Current {
				get {
					if (currentIndex == collection.Count)
						throw new InvalidOperationException ();
					return collection [currentIndex] as EnumMember;
				}
			}
		}

		public new IEnumerator GetEnumerator ()
		{
			return new EnumMemberCollection.Enumerator (this);
		}
	}
}
