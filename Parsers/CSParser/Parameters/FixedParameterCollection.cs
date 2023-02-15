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
	/// <summary>
	/// attributesopt parameter-modifieropt type identifier 
	/// </summary>
	public class FixedParameterCollection : CommaSeperatedCollection, IEnumerable 
	{
		public static FixedParameterCollection parse(TokenProvider tokenizer)
		{
			FixedParameterCollection ret = new FixedParameterCollection();
			FixedParameter fixedParameter = FixedParameter.parse(tokenizer);
			if (false == (fixedParameter is MissingFixedParameter))
			{
				if (Token.COMMA == tokenizer.CurrentToken.Type) {
					ret.Add(fixedParameter, tokenizer.CurrentToken);
				} else {
					ret.Add(fixedParameter);
				}
			}
			while (Token.COMMA == tokenizer.CurrentToken.Type)
			{
				tokenizer.nextToken();
				fixedParameter = FixedParameter.parse(tokenizer);
				if (false == (fixedParameter is MissingFixedParameter))
				{
					if (Token.COMMA == tokenizer.CurrentToken.Type) {
						ret.Add(fixedParameter, tokenizer.CurrentToken);
					} else {
						ret.Add(fixedParameter);
					}
				}
			}
			return ret;
		}

		public FixedParameterCollection()
		{
		}
		public FixedParameterCollection(FixedParameter val)
		{
			Add(val);
		}
		
		public override string Generate()
		{
			System.Text.StringBuilder ret = new System.Text.StringBuilder();
			for (int i=0; i<data.Count; i++)
			{
				ret.Append(this[i].Generate());
				if (i < commas.Count)
				{
					PositionToken commaToken = commas[i] as PositionToken;
					ret.Append(commaToken.Generate());
				}
			}
			return ret.ToString();
		}

		public void Add (FixedParameter value)
		{
			data.Add (value);
			AdjustPosition();
		}
		public void Add (FixedParameter value, PositionToken commaToken)
		{
			data.Add (value);
			commas.Add (commaToken);
			AdjustPosition();
		}

        public void RemoveAt(int index)
        {
            data.RemoveAt(index);
        }

        public new FixedParameter this[int index] {
			get {
				return data [index] as FixedParameter;
			}
			set {
				data [index] = value;
			}
		}

		private class Enumerator : PieceOfCodeCollectionBaseEnumerator
		{
			public Enumerator(FixedParameterCollection collection) :
			base(collection)
			{
			}

			public new FixedParameter Current {
				get {
					if (currentIndex == collection.Count)
						throw new InvalidOperationException ();
					return collection [currentIndex] as FixedParameter;
				}
			}
		}

		public new IEnumerator GetEnumerator ()
		{
			return new FixedParameterCollection.Enumerator (this);
		}
	}
}
