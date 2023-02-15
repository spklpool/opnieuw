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
	public class AttributeNamedArgumentList : CommaSeperatedCollection, IEnumerable 
	{
		public static AttributeNamedArgumentList parse(TokenProvider tokenizer)
		{
			AttributeNamedArgumentList ret = new AttributeNamedArgumentList();
			AttributeNamedArgument arg = AttributeNamedArgument.parse(tokenizer);
			while (false == (arg is MissingAttributeNamedArgument))
			{
				if (Token.COMMA == tokenizer.CurrentToken.Type)
				{
					ret.Add(arg, tokenizer.CurrentToken);
					tokenizer.nextToken(); // ,
					arg = AttributeNamedArgument.parse(tokenizer);
				}
				else 
				{
					ret.Add(arg);
					break;
				}
			}
			return ret;
		}

		public AttributeNamedArgumentList()
		{
		}
		public AttributeNamedArgumentList(AttributeNamedArgument val)
		{
			Add(val);
		}

		public void Add (AttributeNamedArgument value)
		{
			data.Add (value);
			AdjustPosition();
		}
		public void Add (AttributeNamedArgument value, PositionToken commaToken)
		{
			data.Add (value);
			commas.Add (commaToken);
			AdjustPosition();
		}

		public new AttributeNamedArgument this[int index] {
			get {
				return data [index] as AttributeNamedArgument;
			}

			set {
				data [index] = value;
			}
		}
		
		private class Enumerator : PieceOfCodeCollectionBaseEnumerator
		{
			public Enumerator(AttributeNamedArgumentList collection) :
			base(collection)
			{
			}

			public new AttributeNamedArgument Current {
				get {
					if (currentIndex == collection.Count)
						throw new InvalidOperationException ();
					return collection [currentIndex] as AttributeNamedArgument;
				}
			}
		}
		
		public new IEnumerator GetEnumerator ()
		{
			return new AttributeNamedArgumentList.Enumerator (this);
		}
	}
}
