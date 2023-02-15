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
using Opnieuw.Framework;

namespace Opnieuw.Parsers.CSParser
{
	public class InvocationExpression : Expression
	{
		/// <summary>
		/// primary-expression ( argument-listopt ) 
		/// </summary>
		public new static Expression parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			Expression ret = new MissingExpression();
			Expression expression = PrimaryExpression.parse(tokenizer);
			if (false == (expression is MissingExpression))
			{
				if (Token.OPEN_PARENS == tokenizer.CurrentToken.Type)
				{
					PositionToken openParensToken = tokenizer.CurrentToken;
					tokenizer.nextToken(); // (
					ArgumentCollection arguments = ArgumentCollection.parse(tokenizer);
					if (Token.CLOSE_PARENS == tokenizer.CurrentToken.Type)
					{
						PositionToken closeParensToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // )
						ret = new InvocationExpression(expression as PrimaryExpression, openParensToken, arguments, closeParensToken);
					}
				}
			}
			tokenizer.endBookmark(ret is MissingExpression);
			return ret;
		}

		public InvocationExpression(PrimaryExpression expression, PositionToken openParensToken, ArgumentCollection arguments, PositionToken closeParensToken) :
		base(expression, openParensToken, arguments, closeParensToken)
		{
		}

		public Expression Expression {
			get {
				return Pieces[0] as Expression;
			}
		}
		
		public PositionToken OpenParensToken {
			get {
				return Pieces[1] as PositionToken;
			}
		}

		public ArgumentCollection Arguments {
			get {
				return Pieces[2] as ArgumentCollection;
			}
		}
		
		public PositionToken CloseParensToken {
			get {
				return Pieces[3] as PositionToken;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Expression);
				ret.Add(Arguments.Children);
				return ret;
			}
		}
	}
}
