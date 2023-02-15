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
	public class JumpStatement : Statement
    {
#region static parsing code
        /// <summary>
        /// break-statement
		/// continue-statement
		/// goto-statement
		/// return-statement
		/// throw-statement 
		/// </summary>
		/// <param name="tokenizer"></param>
		/// <returns></returns>
		public new static Statement parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			Statement ret = new MissingStatement();
			switch (tokenizer.CurrentToken.Type)
			{
				case Token.BREAK:
				{
					PositionToken breakToken = tokenizer.CurrentToken;
					tokenizer.nextToken(); // break
					PositionToken semicolonToken = tokenizer.checkToken(Token.SEMICOLON);
					ret = new BreakStatement(breakToken, semicolonToken);
					break;
				}
				case Token.CONTINUE:
				{
					PositionToken continueToken = tokenizer.CurrentToken;
					tokenizer.nextToken(); // continue
					PositionToken semicolonToken = tokenizer.checkToken(Token.SEMICOLON);
					ret = new ContinueStatement(continueToken, semicolonToken);
					break;
				}
				case Token.GOTO:
				{
					PositionToken gotoToken = tokenizer.CurrentToken;
					tokenizer.nextToken(); // goto
					if (Token.CASE == tokenizer.CurrentToken.Type)
					{
						PositionToken caseToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // case
						Expression exp = Expression.parse(tokenizer);
						if (false == (exp is MissingExpression))
						{
							PositionToken semicolonToken = tokenizer.checkToken(Token.SEMICOLON);
							ret = new GotoCaseStatement(gotoToken, caseToken, exp, semicolonToken);
						}
					}
					else if (Token.DEFAULT == tokenizer.CurrentToken.Type)
					{
						PositionToken defaultToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // default
						PositionToken semicolonToken = tokenizer.checkToken(Token.SEMICOLON);
						ret = new GotoDefaultCaseStatement(gotoToken, defaultToken, semicolonToken);
					}
					else
					{
						Identifier id = Identifier.parse(tokenizer);
						if (false == (id is MissingIdentifier))
						{
							PositionToken semicolonToken = tokenizer.checkToken(Token.SEMICOLON);
							ret = new GotoStatement(gotoToken, id, semicolonToken);
						}
					}
					break;
				}
				case Token.RETURN:
				{
					PositionToken returnToken = tokenizer.CurrentToken;
					tokenizer.nextToken(); // return
					Expression exp = Expression.parse(tokenizer);
                    PositionToken semicolonToken = tokenizer.checkToken(Token.SEMICOLON);
    				ret = new ReturnStatement(returnToken, exp, semicolonToken);
					break;
				}
				case Token.THROW:
				{
					PositionToken throwToken = tokenizer.CurrentToken;
					tokenizer.nextToken(); // throw
					Expression exp = Expression.parse(tokenizer); //optional
					PositionToken semicolonToken = tokenizer.checkToken(Token.SEMICOLON);
					ret = new ThrowStatement(throwToken, exp, semicolonToken);
					break;
				}
			}
			tokenizer.endBookmark(ret is MissingStatement);
			return ret;
        }
#endregion

        public JumpStatement(Position position)
		{
			m_Position = position;
		}
		
		public JumpStatement(params FundamentalPieceOfCode[] list) :
		base(list)
		{
		}
	}

	public class BreakStatement : JumpStatement
	{
		public BreakStatement(PositionToken breakToken, PositionToken semicolonToken) :
		base(breakToken, semicolonToken)
		{
		}
		
		public PositionToken BreakToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}
		
		public PositionToken SemicolonToken {
			get {
				return Pieces[1] as PositionToken;
			}
		}
	}

	public class ContinueStatement : JumpStatement
	{
		public ContinueStatement(PositionToken continueToken, PositionToken semicolonToken) :
		base(continueToken, semicolonToken)
		{
		}
	}

	public class ReturnStatement : JumpStatement
	{
        public ReturnStatement(Expression expression) :
        base(new PositionToken(Position.Missing, "return", Token.RETURN),
             expression,
             new PositionToken(Position.Missing, ";", Token.SEMICOLON))
        {
        }

        public ReturnStatement(PositionToken returnToken, Expression expression, PositionToken semicolonToken) :
		base(returnToken, expression, semicolonToken)
		{
		}
		
		public PositionToken ReturnToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}

		public Expression Expression {
			get {
				return Pieces[1] as Expression;
			}
		}
		
		public PositionToken SemicolonToken {
			get {
				return Pieces[2] as PositionToken;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Expression);
				return ret;
			}
		}
	}

	public class GotoStatement : JumpStatement
	{
		public GotoStatement(params FundamentalPieceOfCode[] list) :
		base(list)
		{
		}

		public GotoStatement(PositionToken gotoToken, Identifier identifier, PositionToken semicolonToken) :
		base(gotoToken, PositionToken.Missing, identifier, semicolonToken)
		{
		}
		
		public PositionToken GotoToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}

		public Expression Expression {
			get {
				return Pieces[2] as Expression;
			}
		}
		
		public PositionToken SemicolonToken {
			get {
				return Pieces[3] as PositionToken;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Expression);
				return ret;
			}
		}
	}

	public class GotoCaseStatement : GotoStatement
	{
		public GotoCaseStatement(PositionToken gotoToken, PositionToken caseToken, Expression expression, PositionToken semicolonToken) :
		base(gotoToken, caseToken, expression, semicolonToken)
		{
		}
		
		public PositionToken CaseToken {
			get {
				return Pieces[1] as PositionToken;
			}
		}
	}

	public class GotoDefaultCaseStatement : GotoStatement
	{
		public GotoDefaultCaseStatement(PositionToken gotoToken, PositionToken defaultToken, PositionToken semicolonToken) :
		base(gotoToken, defaultToken, new MissingExpression(), semicolonToken)
		{
		}
		
		public PositionToken DefaultToken {
			get {
				return Pieces[1] as PositionToken;
			}
		}
	}

	public class ThrowStatement : JumpStatement
	{
		public ThrowStatement(PositionToken throwToken, Expression expression, PositionToken semicolonToken) :
		base(throwToken, expression, semicolonToken)
		{
		}
		
		public PositionToken ThrowToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}

		public Expression Expression {
			get {
				return Pieces[1] as Expression;
			}
		}

		public PositionToken SemicolonToken {
			get {
				return Pieces[2] as PositionToken;
			}
		}

		public override ExpressionCollection Expressions {
			get {
				ExpressionCollection ret = new ExpressionCollection();
				ret.Add(Expression);
				ret.Add(Expression.Expressions);
				return ret;
			}
		}
	}
}
