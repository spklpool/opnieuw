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
	public class UnaryExpression : Expression
	{
		/// <summary>
		/// primary-expression
		/// + unary-expression
		/// - unary-expression
		/// ! unary-expression
		/// ~ unary-expression
		/// * unary-expression
		/// pre-increment-expression
		/// pre-decrement-expression
		/// cast-expression 
		/// </summary>
		/// <param name="tokenizer"></param>
		/// <returns></returns>
		public static bool parse(TokenProvider tokenizer, ref Expression ret)
		{
			Expression test = PrimaryExpression.parse(tokenizer);
			if (test is MissingExpression)
			{
				if ((Token.BANG == tokenizer.CurrentToken.Type) ||
					(Token.PLUS == tokenizer.CurrentToken.Type) ||
					(Token.MINUS == tokenizer.CurrentToken.Type) ||
					(Token.OP_INC == tokenizer.CurrentToken.Type) ||
					(Token.OP_DEC == tokenizer.CurrentToken.Type))
				{
					tokenizer.setBookmark();
					PositionToken startToken = tokenizer.CurrentToken;
					tokenizer.nextToken();
					Expression exp = Expression.parse(tokenizer);
					if (false == (exp is MissingExpression))
					{
						switch (startToken.Type)
						{
							case Token.PLUS:
								ret = new AdditionExpression(startToken, exp);
								tokenizer.cancelBookmark();
								return true;
							case Token.MINUS:
								ret = new SubtractionExpression(startToken, exp);
								tokenizer.cancelBookmark();
								return true;
							case Token.BANG:
								ret = new NegationExpression(startToken, exp);
								tokenizer.cancelBookmark();
								return true;
							case Token.OP_INC:
								ret = new PreIncrementExpression(startToken, exp);
								tokenizer.cancelBookmark();
								return true;
							case Token.OP_DEC:
								ret = new PreDecrementExpression(startToken, exp);
								tokenizer.cancelBookmark();
								return true;
						}
					}
					tokenizer.returnToBookmark();
				}
				if (ret is MissingExpression)
				{
					if (Token.OPEN_PARENS == tokenizer.CurrentToken.Type)
					{
						tokenizer.setBookmark();
						PositionToken openParensTokenToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // (
						DataType type = DataType.parse(tokenizer);
						if (Token.CLOSE_PARENS == tokenizer.CurrentToken.Type)
						{
							PositionToken closeParensToken = tokenizer.CurrentToken;
							tokenizer.nextToken(); // )
							Expression exp = UnaryExpression.parse(tokenizer);
							if (false == (exp is MissingExpression))
							{
								ret = new CastExpression(openParensTokenToken, type, closeParensToken, exp);
								tokenizer.cancelBookmark();
								return true;
							}
						}
						tokenizer.returnToBookmark();
					}
				}
			}
			else
			{
				ret = test;
				return true;
			}
			return false;
		}

		public UnaryExpression(PositionToken unaryToken, Expression expression) :
		base(unaryToken, expression)
		{
		}

		public PositionToken UnaryToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}

		public Expression Expression {
			get {
				return Pieces[1] as Expression;
			}
		}

		public string Operator {
			get {
				return UnaryToken.Text;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				return new PieceOfCodeCollection(Expression);
			}
		}
	}

	public class MissingUnaryExpression : UnaryExpression
	{
		public MissingUnaryExpression() :
		base(PositionToken.Missing, new MissingExpression())
		{
		}
	}

	public class NegationExpression : UnaryExpression
	{
		public NegationExpression(PositionToken unaryToken, Expression expression) :
		base(unaryToken, expression)
		{
		}
	}

	public class AdditionExpression : UnaryExpression
	{
		public AdditionExpression(PositionToken unaryToken, Expression expression) :
		base(unaryToken, expression)
		{
		}
	}

	public class SubtractionExpression : UnaryExpression
	{
		public SubtractionExpression(PositionToken unaryToken, Expression expression) :
		base(unaryToken, expression)
		{
		}
	}

	public class PreDecrementExpression : UnaryExpression
	{
		public PreDecrementExpression(PositionToken unaryToken, Expression expression) :
		base(unaryToken, expression)
		{
		}

		public override VariableCollection ModifiedVariables {
			get {
				VariableCollection ret = new VariableCollection();
				if (Expression is Identifier)
				{
					Identifier identifier = Expression as Identifier;
					Variable variable = FindParentScopeVariableDeclaration(identifier.Name);
					if (false == variable is MissingVariable)
					{
						ret.Add(variable);
					}
				}
				return ret;
			}
		}
	}

	public class PreIncrementExpression : UnaryExpression
	{
		public PreIncrementExpression(PositionToken unaryToken, Expression expression) :
		base(unaryToken, expression)
		{
		}

		public override VariableCollection ModifiedVariables {
			get {
				VariableCollection ret = new VariableCollection();
				if (Expression is Identifier)
				{
					Identifier identifier = Expression as Identifier;
					Variable variable = FindParentScopeVariableDeclaration(identifier.Name);
					if (false == variable is MissingVariable)
					{
						ret.Add(variable);
					}
				}
				return ret;
			}
		}
	}

	public class CastExpression : Expression
	{
		/// <summary>
		/// ( type ) unary-expression 
		/// </summary>
		public new static Expression parse(TokenProvider tokenizer)
		{
			Expression ret = new MissingExpression();
			tokenizer.setBookmark();
			if (Token.OPEN_PARENS == tokenizer.CurrentToken.Type)
			{
				PositionToken openParensToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // {
				DataType type = DataType.parse(tokenizer);
				if (false == (type is MissingDataType))
				{
					if (Token.CLOSE_PARENS == tokenizer.CurrentToken.Type)
					{
						PositionToken closeParensToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // )
						Expression expression = UnaryExpression.parse(tokenizer);
						if (false == (expression is MissingExpression))
						{
							ret = new CastExpression(openParensToken, type, closeParensToken, expression);
						}
					}
				}
			}
			tokenizer.endBookmark(ret is MissingExpression);
			return ret;
		}

		public CastExpression(PositionToken openParensToken, DataType type, PositionToken closeParensToken, Expression expression) :
		base(openParensToken, type, closeParensToken, expression)
		{
		}
		
		public PositionToken OpenParensToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}

		public DataType Type {
			get {
				return Pieces[1] as DataType;
			}
		}
		
		public PositionToken CloseParensToken {
			get {
				return Pieces[2] as PositionToken;
			}
		}

		public Expression Expression {
			get {
				return Pieces[3] as Expression;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				return new PieceOfCodeCollection(Expression);
			}
		}
	}
}
