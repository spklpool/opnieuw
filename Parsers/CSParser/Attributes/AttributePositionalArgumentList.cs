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
	public class AttributePositionalArgumentList : CommaSeperatedCollection, IEnumerable 
	{
		public static AttributePositionalArgumentList parse(TokenProvider tokenizer)
		{
			AttributePositionalArgumentList ret = new AttributePositionalArgumentList();
			if (nextIsNotNamedArgument(tokenizer))
			{
				AttributePositionalArgument	positionalArgument = AttributePositionalArgument.parse(tokenizer);
				while (false == (positionalArgument is MissingAttributePositionalArgument))
				{
					if (Token.COMMA == tokenizer.CurrentToken.Type) {
						ret.Add(positionalArgument, tokenizer.CurrentToken);
					} else {
						ret.Add(positionalArgument);
					}
					if ((Token.COMMA == tokenizer.CurrentToken.Type) &&
						(nextIsNotNamedArgument(tokenizer))) {
						tokenizer.nextToken(); // ,
						positionalArgument = AttributePositionalArgument.parse(tokenizer);
					} else {
						break;
					}
				}
			}
			return ret;
		}

		private static bool nextIsNotNamedArgument(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			if (Token.COMMA == tokenizer.CurrentToken.Type)
			{
				tokenizer.nextToken(); // ,
			}
			AttributeNamedArgument check = AttributeNamedArgument.parse(tokenizer);
			tokenizer.returnToBookmark();
			return (check is MissingAttributeNamedArgument);
		}

		public AttributePositionalArgumentList()
		{
		}
		public AttributePositionalArgumentList(AttributePositionalArgument val)
		{
			Add(val);
		}

		public void Add (AttributePositionalArgument value)
		{
			data.Add (value);
			AdjustPosition();
		}
		public void Add (AttributePositionalArgument value, PositionToken commaToken)
		{
			data.Add (value);
			commas.Add (commaToken);
			AdjustPosition();
		}
		
		public new AttributePositionalArgument this[int index] {
			get {
				return data [index] as AttributePositionalArgument;
			}

			set {
				data [index] = value;
			}
		}

		private class Enumerator : PieceOfCodeCollectionBaseEnumerator
		{
			public Enumerator(AttributePositionalArgumentList collection) :
			base(collection)
			{
			}

			public new AttributePositionalArgument Current {
				get {
					if (currentIndex == collection.Count)
						throw new InvalidOperationException ();
					return collection [currentIndex] as AttributePositionalArgument;
				}
			}
		}

		public new IEnumerator GetEnumerator ()
		{
			return new AttributePositionalArgumentList.Enumerator (this);
		}

	}
}
