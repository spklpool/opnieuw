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
	public class OperatorDeclarator : PieceOfCode
	{
		/// <summary>
		/// + - * / % & | ^ << >> == != > < >= <= 
		/// </summary>
		public static bool IsOverloadableUnaryOperator(int token)
		{
			return ((Token.PLUS == token) ||
					(Token.MINUS == token) ||
					(Token.STAR == token) ||
					(Token.DIV == token) ||
					(Token.PERCENT == token) ||
					(Token.BITWISE_AND == token) ||
					(Token.BITWISE_OR == token) ||
					(Token.CARRET == token) ||
					(Token.OP_SHIFT_LEFT == token) ||
					(Token.OP_SHIFT_RIGHT == token) ||
					(Token.OP_EQ == token) ||
					(Token.OP_NE == token) ||
					(Token.OP_GT == token) ||
					(Token.OP_LT == token) ||
					(Token.OP_GE == token) ||
					(Token.OP_LE == token));
		}

		/// <summary>
		/// + - ! ~ ++ -- true false 
		/// </summary>
		public static bool IsOverloadableBinaryOperator(int token)
		{
			return ((Token.PLUS == token) ||
					(Token.MINUS == token) ||
					(Token.BANG == token) ||
					(Token.TILDE == token) ||
					(Token.OP_INC == token) ||
					(Token.OP_DEC == token) ||
					(Token.TRUE == token) ||
					(Token.FALSE == token));
		}

		/// <summary>
		/// implicit operator type ( type identifier )
		/// explicit operator type ( type identifier ) 
		/// type operator overloadable-unary-operator ( type identifier )
		/// type operator overloadable-binary-operator ( type identifier , type identifier )
		/// </summary>
		public static OperatorDeclarator parse(TokenProvider tokenizer)
		{
			OperatorDeclarator ret = new MissingOperatorDeclarator();
			tokenizer.setBookmark();
			if ((Token.IMPLICIT == tokenizer.CurrentToken.Type) ||
				(Token.EXPLICIT == tokenizer.CurrentToken.Type))
			{
				PositionToken modifierToken = tokenizer.CurrentToken;
				tokenizer.nextToken();
				PositionToken operatorToken = tokenizer.checkToken(Token.OPERATOR);
				DataType returnType = DataType.parse(tokenizer);
				PositionToken openParensToken = tokenizer.checkToken(Token.OPEN_PARENS);
				DataType type1 = DataType.parse(tokenizer);
				type1.checkNotMissing();
				Identifier identifier1 = Identifier.parse(tokenizer);
				identifier1.checkNotMissing();
				PositionToken closeParensToken = tokenizer.checkToken(Token.CLOSE_PARENS);
				ret = new OperatorDeclarator(modifierToken, operatorToken, returnType, openParensToken, type1, identifier1, closeParensToken);
			}
			else
			{
				DataType returnType = DataType.parse(tokenizer);
				if (false == (returnType is MissingDataType))
				{
					if (Token.OPERATOR == tokenizer.CurrentToken.Type)
					{
						PositionToken operatorToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // operator
						if ((IsOverloadableUnaryOperator(tokenizer.CurrentToken.Type)) ||
							(IsOverloadableBinaryOperator(tokenizer.CurrentToken.Type)))
						{
							PositionToken overloadedOpertorToken = tokenizer.CurrentToken;
							tokenizer.nextToken();
							PositionToken openParensToken = tokenizer.checkToken(Token.OPEN_PARENS);
							DataType type1 = DataType.parse(tokenizer);
							type1.checkNotMissing();
							Identifier identifier1 = Identifier.parse(tokenizer);
							identifier1.checkNotMissing();
							DataType type2 = new MissingDataType();
							Identifier identifier2 = new MissingIdentifier();
							PositionToken commaToken = PositionToken.Missing;
							if (Token.COMMA == tokenizer.CurrentToken.Type)
							{
								commaToken = tokenizer.CurrentToken;
								tokenizer.nextToken(); // ,
								type2 = DataType.parse(tokenizer);
								type2.checkNotMissing();
								identifier2 = Identifier.parse(tokenizer);
								identifier2.checkNotMissing();
							}
							PositionToken closeParensToken = tokenizer.checkToken(Token.CLOSE_PARENS);
							ret = new OperatorDeclarator(returnType, operatorToken, overloadedOpertorToken, openParensToken, type1, identifier1, commaToken, type2, identifier2, closeParensToken);
						}
					}
				}
			}
			tokenizer.endBookmark(ret is MissingOperatorDeclarator);
			return ret;
		}

		public OperatorDeclarator() :
		base(new MissingDataType(), PositionToken.Missing, PositionToken.Missing, 
			 PositionToken.Missing, new MissingDataType(), PositionToken.Missing, 
			 new MissingDataType(), new MissingIdentifier(), PositionToken.Missing, 
			 new MissingDataType(), new MissingIdentifier(), PositionToken.Missing)
		{
		}

		public OperatorDeclarator(PositionToken modifierToken, PositionToken operatorToken, 
								  DataType returnType, PositionToken openParensToken, 
								  DataType type, Identifier identifier, PositionToken closeParensToken) :
		base(new MissingDataType(), modifierToken, operatorToken, PositionToken.Missing, 
			 returnType, openParensToken, type, identifier, PositionToken.Missing, new MissingDataType(), 
			 new MissingIdentifier(), closeParensToken)
		{
		}

		public OperatorDeclarator(DataType returnType, PositionToken operatorToken, 
								  PositionToken overloadedOperatorToken, PositionToken openParensToken, 
								  DataType type1, Identifier identifier1, PositionToken commaToken, 
								  DataType type2, Identifier identifier2, PositionToken closeParensToken) :
		base(returnType, PositionToken.Missing, operatorToken, overloadedOperatorToken, new MissingDataType(), 
			 openParensToken, type1, identifier1, commaToken, type2, identifier2, closeParensToken)
		{
		}
		
		public DataType ReturnType1 {
			get {
				return Pieces[0] as DataType;
			}
		}
		
		public PositionToken ModifierToken {
			get {
				return Pieces[1] as PositionToken;
			}
		}
		
		public PositionToken OperatorToken {
			get {
				return Pieces[2] as PositionToken;
			}
		}
		
		public PositionToken OverloadedOperatorToken {
			get {
				return Pieces[3] as PositionToken;
			}
		}
		
		public DataType ReturnType2 {
			get {
				return Pieces[4] as DataType;
			}
		}
		
		public PositionToken OpenParensToken {
			get {
				return Pieces[5] as PositionToken;
			}
		}

		public DataType Type1 {
			get {
				return Pieces[6] as DataType;
			}
		}

		public Identifier Identifier1 {
			get {
				return Pieces[7] as Identifier;
			}
		}
		
		public PositionToken CommaToken {
			get {
				return Pieces[8] as PositionToken;
			}
		}

		public DataType Type2 {
			get {
				return Pieces[9] as DataType;
			}
		}

		public Identifier Identifier2 {
			get {
				return Pieces[10] as Identifier;
			}
		}
		
		public PositionToken CloseParensToken {
			get {
				return Pieces[11] as PositionToken;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Type1);
				ret.Add(Type2);
				ret.Add(Identifier1);
				ret.Add(Identifier2);
				return ret;
			}
		}

		public override ExpressionCollection Expressions {
			get {
				return new ExpressionCollection();	
			}
		}

		public override VariableCollection DeclaredVariables {
			get {
				VariableCollection ret = new VariableCollection();
				if (false == (Type1 is MissingDataType))
				{
					ret.Add(new Variable(Identifier1.Name, Type1, new MissingVariableInitializer(), this));
				}
				if (false == (Type2 is MissingDataType))
				{
					ret.Add(new Variable(Identifier2.Name, Type2, new MissingVariableInitializer(), this));
				}
				return ret;
			}
		}

		public override VariableCollection ReferencedVariables {
			get {
				return new VariableCollection();
			}
		}

		public override VariableCollection ModifiedVariables {
			get {
				return new VariableCollection();
			}
		}
	}

	public class MissingOperatorDeclarator : OperatorDeclarator
	{
	}
}
