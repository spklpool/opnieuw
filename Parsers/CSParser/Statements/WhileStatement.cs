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
	public class WhileStatement : Statement
    {
#region static parsing code
        /// <summary>
        /// while ( boolean-expression ) embedded-statement 
        /// </summary>
        public new static Statement parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			Statement ret = new MissingStatement();
			if (Token.WHILE == tokenizer.CurrentToken.Type)
			{
				PositionToken whileToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // while
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
							Statement statement = Statement.parse(tokenizer);
							if (false == (statement is MissingStatement))
							{
								ret = new WhileStatement(whileToken, openParensToken, 
														 exp, closeParensToken, statement);
							}

						}
					}
				}
			}
			tokenizer.endBookmark(ret is MissingStatement);
			return ret;
        }
#endregion

#region constructors
        public WhileStatement(PositionToken whileToken, PositionToken openParensToken, 
							  Expression expression, PositionToken closeParensToken, 
							  Statement statement) :
		base(whileToken, openParensToken, expression, closeParensToken, statement)
		{
        }

        public WhileStatement(Expression expression, Statement statement) :
		base(new PositionToken(Position.Missing, "while", Token.WHILE), 
             new PositionToken(Position.Missing, "(", Token.OPEN_PARENS), 
             expression, 
             new PositionToken(Position.Missing, ")", Token.CLOSE_PARENS), 
             statement)
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
                Statement.CloneFormat(newStatement);
            }
        }

        public PositionToken WhileToken {
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

		public Statement Statement {
			get {
				return Pieces[4] as Statement;
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
