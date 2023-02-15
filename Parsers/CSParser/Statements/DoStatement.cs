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
	public class DoStatement : Statement
    {
#region static parsing code
        /// <summary>
        /// do embedded-statement while ( boolean-expression ) ; 
        /// </summary>
        public new static Statement parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			Statement ret = new MissingStatement();
			if (Token.DO == tokenizer.CurrentToken.Type)
			{
				PositionToken doToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // do
				Statement statement = Statement.parse(tokenizer);
				statement.checkNotMissing();
				PositionToken whileToken = tokenizer.checkToken(Token.WHILE);
				PositionToken openParensToken = tokenizer.checkToken(Token.OPEN_PARENS);
				Expression exp = Expression.parse(tokenizer);
				exp.checkNotMissing();
				PositionToken closeParensToken = tokenizer.checkToken(Token.CLOSE_PARENS);
				PositionToken semicolonToken = tokenizer.checkToken(Token.SEMICOLON);
				ret = new DoStatement(doToken, statement, whileToken, openParensToken, exp, closeParensToken, semicolonToken);
			}
			tokenizer.endBookmark(ret is MissingStatement);
			return ret;
        }
#endregion

#region constructors
        public DoStatement(PositionToken doToken, Statement statement, 
						   PositionToken whileToken, PositionToken openParensToken, 
						   Expression expression, PositionToken closeParensToken, 
						   PositionToken semicolonToken) :
		base(doToken, statement, whileToken, openParensToken, expression, 
             closeParensToken, semicolonToken)
		{
        }

        public DoStatement(Statement statement, Expression expression) :
		base(new PositionToken(Position.Missing, "do", Token.DO), 
             statement, 
             new PositionToken(Position.Missing, "while", Token.WHILE), 
             new PositionToken(Position.Missing, "(", Token.OPEN_PARENS), 
             expression, 
             new PositionToken(Position.Missing, ")", Token.CLOSE_PARENS), 
             new PositionToken(Position.Missing, ";", Token.SEMICOLON))
        {
        }
#endregion

        public override void Format()
        {
            base.Format();
            if ((!(Statement is BlockStatement)) && (!(Statement is MissingStatement)))
            {
                string statementBlock = Statement.Generate();
                string newStatementBlock = GenericBlockOfCode.Reindent(statementBlock, 4, 4);
                Statement newStatement = Statement.parse(newStatementBlock);
                Pieces[1] = newStatement;
            }
        }

        public PositionToken DoToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}

		public Statement Statement {
			get {
				return Pieces[1] as Statement;
			}
		}
		
		public PositionToken WhileToken {
			get {
				return Pieces[2] as PositionToken;
			}
		}
		
		public PositionToken OpenParensToken {
			get {
				return Pieces[3] as PositionToken;
			}
		}

		public Expression Expression {
			get {
				return Pieces[4] as Expression;
			}
		}
		
		public PositionToken CloseParensToken {
			get {
				return Pieces[5] as PositionToken;
			}
		}
		
		public PositionToken SemicolonToken {
			get {
				return Pieces[6] as PositionToken;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Expression);
				ret.Add(Statement);
				return ret;
			}
		}
	}
}
