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
	public class AttributeCollection : CommaSeperatedCollection, IEnumerable 
	{
		public static AttributeCollection parse(TokenProvider tokenizer)
		{
			AttributeCollection ret = new AttributeCollection();
			Attribute attribute = Attribute.parse(tokenizer);
			if (false == (attribute is MissingAttribute))
			{
				if (Token.COMMA == tokenizer.CurrentToken.Type) {
					ret.Add(attribute, tokenizer.CurrentToken);
				} else {
					ret.Add(attribute);
				}
			}
			while (Token.COMMA == tokenizer.CurrentToken.Type)
			{
				tokenizer.nextToken(); // ,
				attribute = Attribute.parse(tokenizer);
				if (false == (attribute is MissingAttribute))
				{
					if (Token.COMMA == tokenizer.CurrentToken.Type) {
						ret.Add(attribute, tokenizer.CurrentToken);
					} else {
						ret.Add(attribute);
					}
				}
			}
			return ret;
		}

		public AttributeCollection()
		{
		}
		public AttributeCollection(Attribute val)
		{
			Add(val);
		}

		public void Add (Attribute value)
		{
			data.Add (value);
			AdjustPosition();
		}
		public void Add (Attribute value, PositionToken commaToken)
		{
			data.Add (value);
			commas.Add (commaToken);
			AdjustPosition();
		}

		public new Attribute this[int index] {
			get {
				return data [index] as Attribute;
			}
			set {
				data [index] = value;
			}
		}
		
		private class Enumerator : PieceOfCodeCollectionBaseEnumerator
		{
			public Enumerator(AttributeCollection collection) :
			base(collection)
			{
			}

			public new Attribute Current {
				get {
					if (currentIndex == collection.Count)
						throw new InvalidOperationException ();
					return collection [currentIndex] as Attribute;
				}
			}
		}
		
		public new IEnumerator GetEnumerator ()
		{
			return new AttributeCollection.Enumerator (this);
		}
	}
}
