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
	public class RelationalExpression : Expression
	{
		public static bool parse(TokenProvider tokenizer, ref Expression expression)
		{
			bool ret = false;
			ShiftExpression.parse(tokenizer, ref expression);
			if ((false ==(expression is MissingExpression)) && (IsRelationalToken(tokenizer.CurrentToken)))
			{
				tokenizer.setBookmark();
				PositionToken relationalToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // one of > >= < <=
				Expression expression2 = new MissingExpression();
				RelationalExpression.parse(tokenizer, ref expression2);
				if (false == (expression2 is MissingExpression))
				{
					Expression expression1 = expression;
					expression = new RelationalExpression(relationalToken, expression1, expression2);
					ret = true;
				}
				tokenizer.endBookmark(false == ret);
			}
			else if ((false ==(expression is MissingExpression)) && (Token.IS == tokenizer.CurrentToken.Type))
			{
				tokenizer.setBookmark();
				PositionToken relationalToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // is
				DataType type = DataType.parse(tokenizer);
				if (false == (type is MissingDataType))
				{
					Expression expression1 = expression;
					expression = new RelationalExpression(relationalToken, type, expression1);
					ret = true;
				}
				tokenizer.endBookmark(false == ret);
			}
			else if ((false ==(expression is MissingExpression)) && (Token.AS == tokenizer.CurrentToken.Type))
			{
				tokenizer.setBookmark();
				PositionToken relationalToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // as
				DataType type = DataType.parse(tokenizer);
				if (false == (type is MissingDataType))
				{
					Expression expression1 = expression;
					expression = new RelationalExpression(relationalToken, type, expression1);
					ret = true;
				}
				tokenizer.endBookmark(false == ret);
			}
			return ret;
		}

		private static bool IsRelationalToken(PositionToken token)
		{
			return ((Token.OP_GT == token.Type) || 
					(Token.OP_GE == token.Type) || 
					(Token.OP_LT == token.Type) || 
					(Token.OP_LE == token.Type));
		}

		public RelationalExpression(PositionToken relationalToken, DataType type, Expression expression1) :
		base(expression1, relationalToken, type, new MissingExpression())
		{
		}

		public RelationalExpression(PositionToken relationalToken, Expression expression1, Expression expression2) :
		base(expression1, relationalToken, new MissingDataType(), expression2)
		{
		}

		public Expression Expression1 {
			get {
				return Pieces[0] as Expression;
			}
		}

		public PositionToken RelationalToken {
			get {
				return Pieces[1] as PositionToken;
			}
		}

		public DataType Type {
			get {
				return Pieces[2] as DataType;
			}
		}

		public Expression Expression2 {
			get {
				return Pieces[3] as Expression;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				if (false == (Expression1 is MissingExpression))
				{
					ret.Add(Expression1);
				}
				if (false == (Expression2 is MissingExpression))
				{
					ret.Add(Expression2);
				}
				return ret;
			}
		}

		public override ExpressionCollection Expressions {
			get {
				ExpressionCollection ret = new ExpressionCollection();
				ret.Add(Expression1);
				ret.Add(Expression1.Expressions);
				ret.Add(Expression2);
				ret.Add(Expression2.Expressions);
				return ret;
			}
		}
	}
}
