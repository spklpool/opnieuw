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
	public class PrimaryNoArrayCreationExpression : PrimaryExpression
	{
		public static bool canStillBeAnExpression(TokenProvider tokenizer)
		{
			return ((Token.OPEN_PARENS == tokenizer.CurrentToken.Type) ||
					(Token.OPEN_BRACKET == tokenizer.CurrentToken.Type) ||
					(Token.ASSIGN == tokenizer.CurrentToken.Type) ||
					(Token.OP_ADD_ASSIGN == tokenizer.CurrentToken.Type) ||
					(Token.OP_SUB_ASSIGN == tokenizer.CurrentToken.Type) ||
					(Token.OP_MULT_ASSIGN == tokenizer.CurrentToken.Type) ||
					(Token.OP_DIV_ASSIGN == tokenizer.CurrentToken.Type) ||
					(Token.OP_MOD_ASSIGN == tokenizer.CurrentToken.Type) ||
					(Token.OP_AND_ASSIGN == tokenizer.CurrentToken.Type) ||
					(Token.OP_OR_ASSIGN == tokenizer.CurrentToken.Type) ||
					(Token.OP_XOR_ASSIGN == tokenizer.CurrentToken.Type) ||
					(Token.OP_SHIFT_LEFT_ASSIGN == tokenizer.CurrentToken.Type) ||
					(Token.OP_SHIFT_RIGHT_ASSIGN == tokenizer.CurrentToken.Type) ||
					(Token.OP_INC == tokenizer.CurrentToken.Type) ||
					(Token.OP_DEC == tokenizer.CurrentToken.Type) ||
					(Token.DOT == tokenizer.CurrentToken.Type));
		}

		public new static Expression parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			Expression ret = new MissingExpression();
			Literal.parse(tokenizer, ref ret);
			if (ret is MissingExpression)
			{
				ret = MemberAccessExpression.parseWithPredefinedType(tokenizer);
				if (ret is MissingExpression)
				{
					switch (tokenizer.CurrentToken.Type)
					{
						case Token.IDENTIFIER:
							ret = Identifier.parse(tokenizer);
							break;
						case Token.CHECKED:
							CheckedExpression.parse(tokenizer, ref ret);
							break;
						case Token.UNCHECKED:
							UncheckedExpression.parse(tokenizer, ref ret);
							break;
						case Token.TYPEOF:
							TypeofExpression.parse(tokenizer, ref ret);
							break;
						case Token.NEW:
							ret = ObjectCreationExpression.parse(tokenizer);
							break;
						case Token.BASE:
							ret = BaseAccessExpression.parse(tokenizer);
							break;
						case Token.THIS:
							ret = ThisAccessExpression.parse(tokenizer);
							break;
						case Token.OPEN_PARENS:
							ret = CastExpression.parse(tokenizer);
							if (ret is MissingExpression)
							{
								ret = ParenthesizedExpression.parse(tokenizer);
							}
							break;
					}
				}
			}
			if (false == (ret is MissingExpression))
			{
				while (canStillBeAnExpression(tokenizer))
				{
					if (Token.DOT == tokenizer.CurrentToken.Type)
					{
						//primary-expression . identifier
						//predefined-type . identifier 
						PositionToken dotToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // .
						Identifier identifier = Identifier.parse(tokenizer);
						if (false == (identifier is MissingIdentifier))
						{
							MemberAccessExpression temp1 = new MemberAccessExpression(ret, dotToken, identifier);
							ret = temp1;
							if (Token.OPEN_PARENS == tokenizer.CurrentToken.Type)
							{
								PositionToken openParensToken = tokenizer.CurrentToken;
								tokenizer.nextToken(); // (
								ArgumentCollection arguments = ArgumentCollection.parse(tokenizer);
								Position position = new Position(ret, tokenizer.CurrentToken);
								PositionToken closeParensToken = tokenizer.checkToken(Token.CLOSE_PARENS);
								InvocationExpression temp2 = new InvocationExpression(ret as PrimaryExpression, openParensToken, arguments, closeParensToken);
								ret = temp2;
							}
						}
					}
					else if (Token.OPEN_PARENS == tokenizer.CurrentToken.Type)
					{
						//primary-expression ( argument-listopt ) 
						PositionToken openParensToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // (
						ArgumentCollection args = ArgumentCollection.parse(tokenizer);
						if (Token.CLOSE_PARENS == tokenizer.CurrentToken.Type)
						{
							PositionToken closeParensToken = tokenizer.CurrentToken;
							tokenizer.nextToken(); // )
							InvocationExpression temp = new InvocationExpression(ret as PrimaryExpression, openParensToken, args, closeParensToken);
							ret = temp;
						}
					}
					else if (Token.OPEN_BRACKET == tokenizer.CurrentToken.Type)
					{
						//primary-no-array-creation-expression [ expression-list ] 
						PositionToken openBracketToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // [
						ExpressionCollection expressions = ExpressionCollection.parse(tokenizer);
						if (Token.CLOSE_BRACKET == tokenizer.CurrentToken.Type)
						{
							PositionToken closeBracketToken = tokenizer.CurrentToken;
							tokenizer.nextToken(); // ]
							ElementAccessExpression temp = new ElementAccessExpression(ret as PrimaryExpression, openBracketToken, expressions, closeBracketToken);
							ret = temp;
						}
					}
					else if (Token.OP_INC == tokenizer.CurrentToken.Type)
					{
						PositionToken incrementToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // ++
						PostIncrementExpression temp = new PostIncrementExpression(ret, incrementToken);
						ret = temp;
					}
					else if (Token.OP_DEC == tokenizer.CurrentToken.Type)
					{
						PositionToken decrementToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // --
						PostDecrementExpression temp = new PostDecrementExpression(ret, decrementToken);
						ret = temp;
					}
					else
					{
						//It must be an assignment expression.
						BookmarkKeeper bk2 = new BookmarkKeeper(tokenizer);
						PositionToken op = tokenizer.CurrentToken;
						tokenizer.nextToken(); // assuming this is the operator
						Expression exp2 = Expression.parse(tokenizer);
						Position position = new Position(ret, exp2);
						Expression temp = null;
						switch (op.Type)
						{
							case Token.ASSIGN:
								temp = new AssignmentExpression(ret, op, exp2);
								ret = temp;
								break;
							case Token.OP_ADD_ASSIGN:
								temp = new AdditionAssignmentExpression(ret, op, exp2);
								ret = temp;
								break;
							case Token.OP_SUB_ASSIGN:
								temp = new SubtractionAssignmentExpression(ret, op, exp2);
								ret = temp;
								break;
							case Token.OP_MULT_ASSIGN:
								temp = new MultiplicationAssignmentExpression(ret, op, exp2);
								ret = temp;
								break;
							case Token.OP_DIV_ASSIGN:
								temp = new DivisionAssignmentExpression(ret, op, exp2);
								ret = temp;
								break;
							case Token.OP_MOD_ASSIGN:
								temp = new ModuloAssignmentExpression(ret, op, exp2);
								ret = temp;
								break;
							case Token.OP_AND_ASSIGN:
								temp = new AndAssignmentExpression(ret, op, exp2);
								ret = temp;
								break;
							case Token.OP_OR_ASSIGN:
								temp = new OrAssignmentExpression(ret, op, exp2);
								ret = temp;
								break;
							case Token.OP_XOR_ASSIGN:
								temp = new ExclusiveOrAssignmentExpression(ret, op, exp2);
								ret = temp;
								break;
							case Token.OP_SHIFT_LEFT_ASSIGN:
								temp = new ShiftLeftAssignmentExpression(ret, op, exp2);
								ret = temp;
								break;
							case Token.OP_SHIFT_RIGHT_ASSIGN:
								temp = new ShiftRightAssignmentExpression(ret, op, exp2);
								ret = temp;
								break;
							default:
								bk2.returnToBookmark();
								break;
						}
					}
				}
			}
			tokenizer.endBookmark(ret is MissingExpression);
			return ret;
		}

		public PrimaryNoArrayCreationExpression()
		{
		}
		
		public PrimaryNoArrayCreationExpression(params FundamentalPieceOfCode[] list) :
		base(list)
		{
		}
		
		public PrimaryNoArrayCreationExpression(Position position)
		{
			m_Position = position;
		}
	}

	public class MissingPrimaryNoArrayCreationExpression : PrimaryNoArrayCreationExpression
	{
	}

	public class ParenthesizedExpression : PrimaryNoArrayCreationExpression
	{
		public new static Expression parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			Expression ret = new MissingExpression();
			if (Token.OPEN_PARENS == tokenizer.CurrentToken.Type)
			{
				PositionToken openParensToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // (
				Expression exp = Expression.parse(tokenizer);
				if (false == (exp is MissingExpression))
				{
					if (Token.CLOSE_PARENS == tokenizer.CurrentToken.Type)
					{
						PositionToken closeParensToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // )
						ret = new ParenthesizedExpression(openParensToken, exp, closeParensToken);
					}
				}
			}
			tokenizer.endBookmark(ret is MissingExpression);
			return ret;
		}

		public ParenthesizedExpression(PositionToken openParensToken, Expression expression, PositionToken closeParensToken) :
		base(openParensToken, expression, closeParensToken)
		{
		}

		public PositionToken OpenParensToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}

		public Expression InnerExpression {
			get {
				return Pieces[1] as Expression;
			}
		}

		public PositionToken CloseParensToken {
			get {
				return Pieces[2] as PositionToken;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				return new PieceOfCodeCollection(InnerExpression);
			}
		}
	}

	public class ParenthesizedExpressionCommand : PrimaryNoArrayCreationExpression
	{
		public ParenthesizedExpressionCommand(PositionToken commandToken, PositionToken openParensToken, Expression expression, PositionToken closeParensToken) :
		base(commandToken, openParensToken, expression, closeParensToken)
		{
		}
		
		public PositionToken CommandToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}
		
		public PositionToken OpenParensToken {
			get {
				return Pieces[1] as PositionToken;
			}
		}

		public Expression Expression {
			get {
				return Pieces[2] as Expression;
			}
		}
		
		public PositionToken CloseParensToken {
			get {
				return Pieces[3] as PositionToken;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				return new PieceOfCodeCollection(Expression);
			}
		}
	}

	public class ThisAccessExpression : PrimaryNoArrayCreationExpression
	{
		public new static Expression parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			Expression ret = new MissingExpression();
			if (Token.THIS == tokenizer.CurrentToken.Type)
			{
				ret = new ThisAccessExpression(tokenizer.CurrentToken);
				tokenizer.nextToken(); // this
			}
			tokenizer.endBookmark(ret is MissingExpression);
			return ret;
		}

		public ThisAccessExpression(PositionToken thisToken) :
		base(thisToken)
		{
		}
		
		public PositionToken ThisToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}
	}

	public class CheckedExpression : ParenthesizedExpressionCommand
	{
		/// <summary>
		/// checked ( expression ) 
		/// </summary>
		public static bool parse(TokenProvider tokenizer, ref Expression ret)
		{
			if (Token.CHECKED == tokenizer.CurrentToken.Type)
			{
				PositionToken checkedToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // checked
				PositionToken openParensToken = tokenizer.checkToken(Token.OPEN_PARENS);
				Expression exp = Expression.parse(tokenizer);
				exp.checkNotMissing();
				PositionToken closeParensToken = tokenizer.checkToken(Token.CLOSE_PARENS);
				ret = new CheckedExpression(checkedToken, openParensToken, exp, closeParensToken);
				return true;
			}
			return false;
		}

		public CheckedExpression(PositionToken checkedToken, PositionToken openParensToken, Expression expression, PositionToken closeParensToken) :
		base(checkedToken, openParensToken, expression, closeParensToken)
		{
		}
	}

	public class UncheckedExpression : ParenthesizedExpressionCommand
	{
		/// <summary>
		/// unchecked ( expression ) 
		/// </summary>
		public static bool parse(TokenProvider tokenizer, ref Expression ret)
		{
			if (Token.UNCHECKED == tokenizer.CurrentToken.Type)
			{
				PositionToken uncheckedToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // unchecked
				PositionToken openParensToken = tokenizer.checkToken(Token.OPEN_PARENS);
				Expression exp = Expression.parse(tokenizer);
				exp.checkNotMissing();
				PositionToken closeParensToken = tokenizer.checkToken(Token.CLOSE_PARENS);
				ret = new UncheckedExpression(uncheckedToken, openParensToken, exp, closeParensToken);
				return true;
			}
			return false;
		}

		public UncheckedExpression(PositionToken uncheckedToken, PositionToken openParensToken, Expression expression, PositionToken closeParensToken) :
		base(uncheckedToken, openParensToken, expression, closeParensToken)
		{
		}
	}

	public class BaseAccessExpression : PrimaryNoArrayCreationExpression
	{
		/// <summary>
		/// base . identifier
		/// base [ expression-list ] 
		/// </summary>
		public new static Expression parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			Expression ret = new MissingExpression();
			if (Token.BASE == tokenizer.CurrentToken.Type)
			{
				PositionToken baseToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // base
				if (Token.DOT == tokenizer.CurrentToken.Type)
				{
					PositionToken dotToken = tokenizer.CurrentToken;
					tokenizer.nextToken(); // .
					Identifier identifier = Identifier.parse(tokenizer);
					ret = new BaseAccessExpression(baseToken, dotToken, identifier);
				}
				else if (Token.OPEN_BRACKET == tokenizer.CurrentToken.Type)
				{
					PositionToken openBracketToken = tokenizer.CurrentToken;
					tokenizer.nextToken(); // [
					ExpressionCollection expressions = ExpressionCollection.parse(tokenizer);
					if (Token.CLOSE_BRACKET == tokenizer.CurrentToken.Type)
					{
						PositionToken closeBracketToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // ]
						ret = new BaseAccessExpression(baseToken, openBracketToken, expressions, closeBracketToken);
					}
				}
			}
			tokenizer.endBookmark(ret is MissingExpression);
			return ret;
		}
		
		public BaseAccessExpression(PositionToken baseToken, PositionToken dotToken, Identifier identifier) :
		base(baseToken, dotToken, PositionToken.Missing, new ExpressionCollection(), PositionToken.Missing, identifier)
		{
		}

		public BaseAccessExpression(PositionToken baseToken, PositionToken openBracketToken, ExpressionCollection expressions, PositionToken closeBracketToken) :
		base(baseToken, PositionToken.Missing, openBracketToken, expressions, closeBracketToken)
		{
		}
		
		public PositionToken BaseToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}
		
		public PositionToken DotToken {
			get {
				return Pieces[1] as PositionToken;
			}
		}
		
		public PositionToken OpenBracketToken {
			get {
				return Pieces[2] as PositionToken;
			}
		}

		public ExpressionCollection ExpressionList {
			get {
				return Pieces[3] as ExpressionCollection;
			}
		}
		
		public PositionToken CloseBracketToken {
			get {
				return Pieces[4] as PositionToken;
			}
		}

		public Identifier Identifier {
			get {
				return Pieces[5] as Identifier;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				return new PieceOfCodeCollection(ExpressionList);
			}
		}
	}

	/// <summary>
	/// primary-no-array-creation-expression [ expression-list ] 
	/// </summary>
	public class ElementAccessExpression : PrimaryNoArrayCreationExpression
	{
		public ElementAccessExpression(Expression expression, PositionToken openBracketToken, ExpressionCollection expressions, PositionToken closeBracketToken) :
		base(expression, openBracketToken, expressions, closeBracketToken)
		{
		}

		public Expression Expression {
			get {
				return Pieces[0] as Expression;
			}
		}
		
		public PositionToken OpenBracketToken {
			get {
				return Pieces[1] as PositionToken;
			}
		}

		public ExpressionCollection ExpressionList {
			get {
				return Pieces[2] as ExpressionCollection;
			}
		}
		
		public PositionToken CloseBracketToken {
			get {
				return Pieces[3] as PositionToken;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Expression);
				ret.Add(ExpressionList);
				return ret;
			}
		}
	}

	public class MemberAccessExpression : PrimaryNoArrayCreationExpression
	{
		/// <summary>
		/// primary-expression . identifier
		/// </summary>
		public static Expression parseWithPrimaryExpression(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			Expression ret = new MissingExpression();
			Expression expression = PrimaryExpression.parse(tokenizer);
			if (false == (expression is MissingExpression))
			{
				if (Token.DOT == tokenizer.CurrentToken.Type)
				{
					PositionToken dotToken = tokenizer.CurrentToken;
					tokenizer.nextToken(); // .
					Identifier identifier = Identifier.parse(tokenizer);
					if (false == (identifier is MissingIdentifier))
					{
						ret = new MemberAccessExpression(expression, dotToken, identifier);
					}
				}
			}
			tokenizer.endBookmark(ret is MissingExpression);
			return ret;
		}

		/// <summary>
		/// predefined-type . identifier 
		/// </summary>
		public static Expression parseWithPredefinedType(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			Expression ret = new MissingExpression();
			//TODO:  This should only be PreDefinedType instead of all data types.
			DataType type = DataType.parse(tokenizer);
			if (Token.DOT == tokenizer.CurrentToken.Type)
			{
				PositionToken dotToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // .
				Identifier identifier = Identifier.parse(tokenizer);
				if (false == (identifier is MissingIdentifier))
				{
					ret = new MemberAccessExpression(type, dotToken, identifier);
				}
			}
			tokenizer.endBookmark(ret is MissingExpression);
			return ret;
		}

		public MemberAccessExpression(DataType type, PositionToken dotToken, Identifier identifier) :
		base(type, new MissingExpression(), dotToken, identifier)
		{
		}

		public MemberAccessExpression(Expression expression, PositionToken dotToken, Identifier identifier) :
		base(new MissingDataType(), expression, dotToken, identifier)
		{
		}

		public DataType Type {
			get {
				return Pieces[0] as DataType;
			}
		}
		
		public Expression Expression {
			get {
				return Pieces[1] as Expression;
			}
		}
		
		public PositionToken DotToken {
			get {
				return Pieces[2] as PositionToken;
			}
		}

		public Identifier Identifier {
			get {
				return Pieces[3] as Identifier;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				return new PieceOfCodeCollection(Expression);
			}
		}
	}

	public class ObjectCreationExpression : PrimaryNoArrayCreationExpression
	{
		/// <summary>
		/// new type ( argument-listopt ) 
		/// </summary>
		public new static Expression parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			Expression ret = new MissingExpression();
			PositionToken newToken = tokenizer.CurrentToken;
			if (Token.NEW == newToken.Type)
			{
				tokenizer.nextToken(); // new
				DataType type = DataType.parse(tokenizer);
				if (Token.OPEN_PARENS == tokenizer.CurrentToken.Type)
				{
					PositionToken openParensToken = tokenizer.CurrentToken;
					tokenizer.nextToken(); // (
					ArgumentCollection arguments = ArgumentCollection.parse(tokenizer);
					if (Token.CLOSE_PARENS == tokenizer.CurrentToken.Type)
					{
						PositionToken closeParensToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // )
						ret = new ObjectCreationExpression(newToken, type, openParensToken, arguments, closeParensToken);
					}
				}
			}
			tokenizer.endBookmark(ret is MissingExpression);
			return ret;
		}

		public ObjectCreationExpression(PositionToken newToken, DataType type, PositionToken openParensToken, ArgumentCollection arguments, PositionToken closeParensToken) :
		base(newToken, type, openParensToken, arguments, closeParensToken)
		{
		}

		public PositionToken NewToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}

		public DataType Type {
			get {
				return Pieces[1] as DataType;
			}
		}
		
		public PositionToken OpenParensToken {
			get {
				return Pieces[2] as PositionToken;
			}
		}

		public ArgumentCollection Arguments {
			get {
				return Pieces[3] as ArgumentCollection;
			}
		}
		
		public PositionToken CloseParensToken {
			get {
				return Pieces[4] as PositionToken;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Arguments.Children);
				return ret;
			}
		}
		
		/// <summary>
		/// Overrides the default ModifiedVariables to return ref and out parameters which
		/// can be modified.
		/// </summary>
		public override VariableCollection ModifiedVariables {
			get {
				VariableCollection ret = new VariableCollection();
				foreach (Argument arg in Arguments)
				{
					if(arg.IsOut || arg.IsRef)
					{
						if (arg.Expression is Identifier)
						{
							Identifier identifier = arg.Expression as Identifier;
							Variable variable = FindParentScopeVariableDeclaration(identifier.Name);
							if (false == variable is MissingVariable)
							{
								ret.Add(variable);
							}
						}
					}
				}
				return ret;
			}
		}
	}

	public class TypeofExpression : PrimaryNoArrayCreationExpression
	{
		public static bool parse(TokenProvider tokenizer, ref Expression ret)
		{
			if (Token.TYPEOF == tokenizer.CurrentToken.Type)
			{
				PositionToken typeofToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // typeof
				PositionToken openParensToken = tokenizer.checkToken(Token.OPEN_PARENS);
				DataType type = DataType.parse(tokenizer);
				type.checkNotMissing();
				PositionToken closeParensToken = tokenizer.checkToken(Token.CLOSE_PARENS);
				ret = new TypeofExpression(typeofToken, openParensToken, type, closeParensToken);
				return true;
			}
			return false;
		}

		public TypeofExpression(PositionToken typeofToken, PositionToken openParensToken, DataType type, PositionToken closeParensToken) :
		base(typeofToken, openParensToken, type, closeParensToken)
		{
		}
		
		public PositionToken TypeofToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}
		
		public PositionToken OpenParensToken {
			get {
				return Pieces[1] as PositionToken;
			}
		}

		public DataType Type {
			get {
				return Pieces[2] as DataType;
			}
		}
		
		public PositionToken CloseParensToken {
			get {
				return Pieces[3] as PositionToken;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				return new PieceOfCodeCollection(Type);
			}
		}
	}

	public class PostIncrementExpression : PrimaryNoArrayCreationExpression
    {
        public PostIncrementExpression(Expression expression) :
		base(expression, new PositionToken(Position.Missing, "++", Token.OP_INC))
        {
        }
        
        public PostIncrementExpression(Expression expression, PositionToken incrementToken) :
		base(expression, incrementToken)
		{
		}

		public Expression Expression {
			get {
				return Pieces[0] as Expression;
			}
		}
		
		public PositionToken IncrementToken {
			get {
				return Pieces[1] as PositionToken;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				return new PieceOfCodeCollection(Expression);
			}
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

	public class PostDecrementExpression : PrimaryNoArrayCreationExpression
	{
		public PostDecrementExpression(Expression expression, PositionToken decrementToken) :
		base(expression, decrementToken)
		{
		}

		public Expression Expression {
			get {
				return Pieces[0] as Expression;
			}
		}
		
		public PositionToken DecrementToken {
			get {
				return Pieces[1] as PositionToken;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				return new PieceOfCodeCollection(Expression);
			}
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
}
