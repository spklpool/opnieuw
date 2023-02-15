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
	public class ArgumentCollection : CommaSeperatedCollection, IEnumerable
    {
#region static parsing code
        /// <summary>
        /// argument
		/// argument-list , argument 
		/// </summary>
		public static ArgumentCollection parse(TokenProvider tokenizer)
		{
			ArgumentCollection ret = new ArgumentCollection();
			Argument arg = Argument.parse(tokenizer);
			if (false == (arg is MissingArgument))
			{
				if (Token.COMMA == tokenizer.CurrentToken.Type) {
					ret.Add(arg, tokenizer.CurrentToken);
				} else {
					ret.Add(arg);
				}
			}
			while (Token.COMMA == tokenizer.CurrentToken.Type)
			{
				tokenizer.nextToken(); // ,
				arg = Argument.parse(tokenizer);
				if (false == (arg is MissingArgument))
				{
					if (Token.COMMA == tokenizer.CurrentToken.Type) {
						ret.Add(arg, tokenizer.CurrentToken);
					} else {
						ret.Add(arg);
					}
				}
			}
			return ret;
        }

        public static ArgumentCollection parse(string name)
        {
            Tokenizer t = new Tokenizer(new StringReader(name), "", null);
            t.nextToken();
            return ArgumentCollection.parse(t) as ArgumentCollection;
        }
#endregion

		public ArgumentCollection()
		{
		}
		public ArgumentCollection(Argument val)
		{
			Add(val);
			AdjustPosition();
		}

		public void Add (Argument value)
		{
			data.Add (value);
			AdjustPosition();
		}
		public void Add (Argument value, PositionToken commaToken)
		{
			data.Add (value);
			commas.Add (commaToken);
			AdjustPosition();
		}

		public new Argument this[int index] {
			get {
				return data [index] as Argument;
			}
			set {
				data [index] = value;
			}
		}
		
		private class Enumerator : PieceOfCodeCollectionBaseEnumerator
		{
			public Enumerator(ArgumentCollection collection) :
			base(collection)
			{
			}

			public new Argument Current {
				get {
					if (currentIndex == collection.Count)
						throw new InvalidOperationException ();
					return collection [currentIndex] as Argument;
				}
			}
		}
		
		public new IEnumerator GetEnumerator ()
		{
			return new ArgumentCollection.Enumerator (this);
		}
		
		private static ArgumentCollection m_Missing = new ArgumentCollection();
		public static ArgumentCollection Missing {
			get {
				return m_Missing;
			}
		}
	}
}
