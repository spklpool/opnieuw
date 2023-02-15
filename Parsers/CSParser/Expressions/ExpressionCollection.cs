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
	public class ExpressionCollection : CommaSeperatedCollection, IEnumerable 
	{
		public static ExpressionCollection parse(TokenProvider tokenizer)
		{
			ExpressionCollection ret = new ExpressionCollection();
			Expression exp = Expression.parse(tokenizer);
			if (false == (exp is MissingExpression))
			{
				if (tokenizer.CurrentToken.Type == Token.COMMA)
				{
					ret.Add(exp, tokenizer.CurrentToken);
				}
				else
				{
					ret.Add(exp);
				}
			}
			while (Token.COMMA == tokenizer.CurrentToken.Type)
			{
				PositionToken commaToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // ,
				exp = Expression.parse(tokenizer);
				if (false == (exp is MissingExpression))
				{
					if (commaToken.Type == Token.COMMA)
					{
						ret.Add(exp, commaToken);
					}
					else
					{
						ret.Add(exp);
					}
				}
			}
			return ret;
		}

		public ExpressionCollection ()
		{
		}

		public void Add (Expression val)
		{
			if (false == (val is MissingExpression))
			{
				data.Add (val);
				AdjustPosition();
			}
		}
		public void Add (Expression val, PositionToken commaToken)
		{
			if (false == (val is MissingExpression))
			{
				data.Add (val);
				commas.Add (commaToken);
				AdjustPosition();
			}
		}

		public void Add (ExpressionCollection col)
		{
			foreach (Expression e in col)
			{
				if (false == (e is MissingExpression))
				{
					Add(e);
				}
			}
			AdjustPosition();
		}

		public void Add (ArgumentCollection col)
		{
			foreach (Argument arg in col)
			{
				if (false == (arg.Expression is MissingExpression))
				{
					Add(arg.Expression);
				}
			}
			AdjustPosition();
		}

		public new Expression this[int index] {
			get {
				return data [index] as Expression;
			}
			set {
				data [index] = value;
			}
		}

		private class Enumerator : PieceOfCodeCollectionBaseEnumerator
		{
			public Enumerator(ExpressionCollection collection) :
			base(collection)
			{
			}

			public new Expression Current {
				get {
					if (currentIndex == collection.Count)
						throw new InvalidOperationException ();
					return collection [currentIndex] as Expression;
				}
			}
		}
		
		public new IEnumerator GetEnumerator ()
		{
			return new ExpressionCollection.Enumerator (this);
		}
	}
}
