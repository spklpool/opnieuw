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
	public class VariableDeclaratorCollection : CommaSeperatedCollection, IEnumerable 
	{
		public static VariableDeclaratorCollection parse(TokenProvider tokenizer)
		{
			VariableDeclaratorCollection ret = new VariableDeclaratorCollection();
			VariableDeclarator declarator = VariableDeclarator.parse(tokenizer);
			while (false == (declarator is MissingVariableDeclarator))
			{
				if (Token.COMMA == tokenizer.CurrentToken.Type)
				{
					ret.Add(declarator, tokenizer.CurrentToken);
					tokenizer.nextToken(); // ,
					declarator = VariableDeclarator.parse(tokenizer);
				}
				else
				{
					ret.Add(declarator);
					break;
				}
			}
			return ret;
		}

		public VariableDeclaratorCollection()
		{
		}
		public VariableDeclaratorCollection(VariableDeclarator val)
		{
			Add(val);
		}

		public override void checkNotMissing()
		{
			if (data.Count == 0)
			{
				throw new ParserException();
			}
		}

		public void Add (VariableDeclarator value)
		{
			data.Add (value);
			AdjustPosition();
		}
		public void Add (VariableDeclarator value, PositionToken commaToken)
		{
			data.Add (value);
			commas.Add (commaToken);
			AdjustPosition();
		}

		public new VariableDeclarator this[int index] {
			get {
				return data [index] as VariableDeclarator;
			}
			set {
				data [index] = value;
			}
		}

		private class Enumerator : PieceOfCodeCollectionBaseEnumerator
		{
			public Enumerator(VariableDeclaratorCollection collection) :
			base(collection)
			{
			}

			public new VariableDeclarator Current {
				get {
					if (currentIndex == collection.Count)
						throw new InvalidOperationException ();
					return collection [currentIndex] as VariableDeclarator;
				}
			}
		}

		public new IEnumerator GetEnumerator ()
		{
			return new VariableDeclaratorCollection.Enumerator (this);
		}
		
		public override string AsText {
			get {
				return "Declarators";
			}
		}
	}
}
