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
	public class ConditionalExpression : Expression
	{
		public static bool parse(TokenProvider tokenizer, ref Expression expression)
		{
			tokenizer.setBookmark();
			bool ret = ConditionalOrExpression.parse(tokenizer, ref expression);
			if (false == ret)
			{
				if (Token.INTERR == tokenizer.CurrentToken.Type)
				{
					tokenizer.nextToken(); // ?
					Expression expression2 = Expression.parse(tokenizer);
					tokenizer.nextToken(); // :
					Expression expression3 = Expression.parse(tokenizer);
					expression = new ConditionalExpression(expression, expression2, expression3);
					ret = true;
				}
			}
			tokenizer.endBookmark(expression is MissingExpression);
			return ret;
		}

		public ConditionalExpression()
		{
			Pieces.Add(new MissingExpression());
			Pieces.Add(PositionToken.Missing);
			Pieces.Add(new MissingExpression());
			Pieces.Add(new MissingExpression());
		}

		public ConditionalExpression(Expression expression1, Expression expression2) :
		base(expression1, PositionToken.Missing, expression2, new MissingExpression())
		{
		}

		public ConditionalExpression(Expression expression1, PositionToken token, Expression expression2) :
		base(expression1, token, expression2, new MissingExpression())
		{
		}

		public ConditionalExpression(Expression expression1, Expression expression2, Expression expression3) :
		base(expression1, PositionToken.Missing, expression2, expression3)
		{
		}

		public Expression Expression1 {
			get {
				return Pieces[0] as Expression;
			}
		}

		public PositionToken ConditionalToken {
			get {
				return Pieces[1] as PositionToken;
			}
		}

		public Expression Expression2 {
			get {
				return Pieces[2] as Expression;
			}
		}
		
		public Expression Expression3 {
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
				if (false == (Expression3 is MissingExpression))
				{
					ret.Add(Expression3);
				}
				return ret;
			}
		}
	}

	public class MissingConditionalExpression : ConditionalExpression
	{
	}

	public class ConditionalOrExpression : ConditionalExpression
	{
		public new static bool parse(TokenProvider tokenizer, ref Expression expression)
		{
			bool ret = false;
			ConditionalAndExpression.parse(tokenizer, ref expression);
			if ((false == (expression is MissingExpression)) &&
				(Token.OP_OR == tokenizer.CurrentToken.Type))
			{
				PositionToken token = tokenizer.CurrentToken;
				tokenizer.nextToken(); // ||
				Expression after = new MissingExpression();
				ConditionalOrExpression.parse(tokenizer, ref after);
				if (false == (after is MissingExpression))
				{
					ConditionalOrExpression temp = new ConditionalOrExpression(expression, token, after);
					expression = temp;
					ret = true;
				}
			}
			return ret;
		}

		public ConditionalOrExpression(Expression exp1, PositionToken token, Expression exp2) :
			base(exp1, token, exp2)
		{
		}
	}

	public class ConditionalAndExpression : ConditionalExpression
	{
		public new static bool parse(TokenProvider tokenizer, ref Expression expression)
		{
			bool ret = false;
			InclusiveOrExpression.parse(tokenizer, ref expression);
			if ((false == (expression is MissingExpression)) &&
				(Token.OP_AND == tokenizer.CurrentToken.Type))
			{
				PositionToken token = tokenizer.CurrentToken;
				tokenizer.nextToken(); // &&
				Expression after = new MissingExpression();
				ConditionalAndExpression.parse(tokenizer, ref after);
				if (false == (after is MissingExpression))
				{
					ConditionalAndExpression temp = new ConditionalAndExpression(expression, token, after);
					expression = temp;
					ret = true;
				}
			}
			return ret;
		}

		public ConditionalAndExpression(Expression exp1, PositionToken token, Expression exp2) :
		base(exp1, token, exp2)
		{
		}
	}

	public class InclusiveOrExpression : ConditionalExpression
	{
		public new static bool parse(TokenProvider tokenizer, ref Expression expression)
		{
			bool ret = false;
			ExclusiveOrExpression.parse(tokenizer, ref expression);
			if ((false == (expression is MissingExpression)) &&
				(Token.BITWISE_OR == tokenizer.CurrentToken.Type))
			{
				PositionToken token = tokenizer.CurrentToken;
				tokenizer.nextToken(); // |
				Expression after = new MissingExpression();
				InclusiveOrExpression.parse(tokenizer, ref after);
				if (false == (after is MissingExpression))
				{
					InclusiveOrExpression temp = new InclusiveOrExpression(expression, token, after);
					expression = temp;
					ret = true;
				}
			}
			return ret;
		}

		public InclusiveOrExpression(Expression exp1, PositionToken token, Expression exp2) :
		base(exp1, token, exp2)
		{
		}
	}

	public class ExclusiveOrExpression : ConditionalExpression
	{
		public new static bool parse(TokenProvider tokenizer, ref Expression expression)
		{
			bool ret = false;
			AndExpression.parse(tokenizer, ref expression);
			if ((false == (expression is MissingExpression)) &&
				(Token.CARRET == tokenizer.CurrentToken.Type))
			{
				PositionToken token = tokenizer.CurrentToken;
				tokenizer.nextToken(); // ^
				Expression after = new MissingExpression();
				ExclusiveOrExpression.parse(tokenizer, ref after);
				if (false == (after is MissingExpression))
				{
					ExclusiveOrExpression temp = new ExclusiveOrExpression(expression, token, after);
					expression = temp;
					ret = true;
				}
			}
			return ret;
		}

		public ExclusiveOrExpression(Expression exp1, PositionToken token, Expression exp2) :
		base(exp1, token, exp2)
		{
		}
	}

	public class AndExpression : ConditionalExpression
	{
		public new static bool parse(TokenProvider tokenizer, ref Expression expression)
		{
			bool ret = false;
			EqualityExpression.parse(tokenizer, ref expression);
			if ((false == (expression is MissingExpression)) &&
				(Token.BITWISE_AND == tokenizer.CurrentToken.Type))
			{
				PositionToken token = tokenizer.CurrentToken;
				tokenizer.nextToken(); // &
				Expression after = new MissingExpression();
				AndExpression.parse(tokenizer, ref after);
				if (false == (after is MissingExpression))
				{
					AndExpression temp = new AndExpression(expression, token, after);
					expression = temp;
					ret = true;
				}
			}
			return ret;
		}

		public AndExpression(Expression exp1, PositionToken token, Expression exp2) :
		base(exp1, token, exp2)
		{
		}
	}

	public class EqualityExpression : ConditionalExpression
	{
		public new static bool parse(TokenProvider tokenizer, ref Expression expression)
		{
			bool ret = false;
			RelationalExpression.parse(tokenizer, ref expression);
			if ((false == (expression is MissingExpression)) &&
				(IsEqualityToken(tokenizer.CurrentToken)))
			{
				PositionToken token = tokenizer.CurrentToken;
				tokenizer.nextToken(); // one of != ==
				Expression after = new MissingExpression();
				AndExpression.parse(tokenizer, ref after);
				if (false == (after is MissingExpression))
				{
					EqualityExpression temp = new EqualityExpression(expression, token, after);
					expression = temp;
					ret = true;
				}
			}
			return ret;
		}

		private static bool IsEqualityToken(PositionToken token)
		{
			return ((Token.OP_NE == token.Type) || 
					(Token.OP_EQ == token.Type));
		}

		public EqualityExpression(Expression exp1, PositionToken token, Expression exp2) :
		base(exp1, token, exp2)
		{
		}
	}

	public class ShiftExpression : ConditionalExpression
	{
		public new static bool parse(TokenProvider tokenizer, ref Expression expression)
		{
			bool ret = false;
			AdditiveExpression.parse(tokenizer, ref expression);
			if ((false == (expression is MissingExpression)) &&
				(IsShiftToken(tokenizer.CurrentToken)))
			{
				PositionToken token = tokenizer.CurrentToken;
				tokenizer.nextToken(); // one of << >>
				Expression after = new MissingExpression();
				ShiftExpression.parse(tokenizer, ref after);
				if (false == (after is MissingExpression))
				{
					ShiftExpression temp = new ShiftExpression(expression, token, after);
					expression = temp;
					ret = true;
				}
			}
			return ret;
		}

		private static bool IsShiftToken(PositionToken token)
		{
			return ((Token.OP_SHIFT_LEFT == token.Type) || 
					(Token.OP_SHIFT_RIGHT == token.Type));
		}

		public ShiftExpression(Expression exp1, PositionToken token, Expression exp2) :
		base(exp1, token, exp2)
		{
		}
	}

	public class AdditiveExpression : ConditionalExpression
	{
		public new static bool parse(TokenProvider tokenizer, ref Expression expression)
		{
			bool ret = false;
			MultiplicativeExpression.parse(tokenizer, ref expression);
			if ((false == (expression is MissingExpression)) &&
				(IsAdditiveToken(tokenizer.CurrentToken)))
			{
				PositionToken token = tokenizer.CurrentToken;
				tokenizer.nextToken(); // one of + -
				Expression after = new MissingExpression();
				AdditiveExpression.parse(tokenizer, ref after);
				if (false == (after is MissingExpression))
				{
					AdditiveExpression temp = new AdditiveExpression(expression, after, token);
					expression = temp;
					ret = true;
				}
			}
			return ret;
		}

		private static bool IsAdditiveToken(PositionToken token)
		{
			return ((Token.PLUS == token.Type) || 
					(Token.MINUS == token.Type));
		}

		public AdditiveExpression(Expression exp1, Expression exp2, PositionToken token) :
			base(exp1, token, exp2)
		{
		}
	}

	public class MultiplicativeExpression : ConditionalExpression
	{
		public new static bool parse(TokenProvider tokenizer, ref Expression expression)
		{
			bool ret = false;
			UnaryExpression.parse(tokenizer, ref expression);
			if ((false == (expression is MissingExpression)) &&
				(IsMultiplicativeToken(tokenizer.CurrentToken)))
			{
				PositionToken token = tokenizer.CurrentToken;
				tokenizer.nextToken(); // one of * / %
				Expression after = UnaryExpression.parse(tokenizer);
				if (false == (after is MissingExpression))
				{
					MultiplicativeExpression temp = new MultiplicativeExpression(expression, token, after);
					expression = temp;
					ret = true;
				}
			}
			return ret;
		}

		public static bool IsMultiplicativeToken(PositionToken token)
		{
			return ((Token.STAR == token.Type) || 
					(Token.DIV == token.Type) || 
					(Token.PERCENT == token.Type));
		}

		public MultiplicativeExpression(Expression exp1, PositionToken token, Expression exp2) :
		base(exp1, token, exp2)
		{
		}
	}
}