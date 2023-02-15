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
	public class IfStatement : Statement
    {
#region static parsing code
        /// <summary>
        /// if ( boolean-expression ) embedded-statement
		/// if ( boolean-expression ) embedded-statement else embedded-statement 
		/// </summary>
		/// <param name="tokenizer"></param>
		/// <returns></returns>
		public new static Statement parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			Statement ret = new MissingStatement();
			if (Token.IF == tokenizer.CurrentToken.Type)
			{
				PositionToken ifToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // if
				if (Token.OPEN_PARENS == tokenizer.CurrentToken.Type)
				{
					PositionToken openParensToken = tokenizer.CurrentToken;
					tokenizer.nextToken(); // (
					Expression exp = Expression.parse(tokenizer);
					if (Token.CLOSE_PARENS == tokenizer.CurrentToken.Type)
					{
						PositionToken closeParensToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // )
						Statement ifStatement = Statement.parse(tokenizer);
						Statement elseStatement = new MissingStatement();
						PositionToken elseToken = PositionToken.Missing;
						if (Token.ELSE == tokenizer.CurrentToken.Type)
						{
							elseToken = tokenizer.CurrentToken;
							tokenizer.nextToken(); // else
							elseStatement = Statement.parse(tokenizer);
						}
						ret = new IfStatement(ifToken, openParensToken, exp, closeParensToken, ifStatement, elseToken, elseStatement);
					}
				}
			}
			tokenizer.endBookmark(ret is MissingStatement);
			return ret;
        }
#endregion

#region constructors
        public IfStatement(PositionToken ifToken, PositionToken openParensToken, Expression expression, PositionToken closeParensToken, Statement ifStatement, PositionToken elseToken, Statement elseStatement) :
		base(ifToken, openParensToken, expression, closeParensToken, ifStatement, elseToken, elseStatement)
		{
        }

        public IfStatement(Expression expression, Statement ifStatement, Statement elseStatement) :
		base(new PositionToken(Position.Missing, "if", Token.IF),
             new PositionToken(Position.Missing, "(", Token.OPEN_PARENS), 
             expression, 
             new PositionToken(Position.Missing, ")", Token.CLOSE_PARENS), 
             ifStatement, 
             new PositionToken(Position.Missing, "else", Token.ELSE), 
             elseStatement)
        {
        }
#endregion

        public override void Format()
        {
            base.Format();
            if ((!(IfPartStatement is BlockStatement)) && (!(IfPartStatement is MissingStatement))) {
                string statementBlock = IfPartStatement.Generate();
                string newStatementBlock = GenericBlockOfCode.Reindent(statementBlock, 4, 4);
                Statement newStatement = Statement.parse(newStatementBlock);
                IfPartStatement.CloneFormat(newStatement);
            } else if (IfPartStatement is BlockStatement) {
                IfPartStatement.LeadingCharacters = " ";
                ElseToken.LeadingCharacters = " ";
            }
            if ((!(ElsePartStatement is BlockStatement)) && (!(ElsePartStatement is MissingStatement))) {
                string statementBlock = ElsePartStatement.Generate();
                string newStatementBlock = GenericBlockOfCode.Reindent(statementBlock, 4, 4);
                Statement newStatement = Statement.parse(newStatementBlock);
                ElsePartStatement.CloneFormat(newStatement);
            } else if ((ElsePartStatement is BlockStatement) && (IfPartStatement is BlockStatement)) {
                ElsePartStatement.LeadingCharacters = " ";
            }
        }

        public PositionToken IfToken {
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

		public Statement IfPartStatement {
			get {
				return Pieces[4] as Statement;
			}
		}

		public PositionToken ElseToken {
			get {
				return Pieces[5] as PositionToken;
			}
		}

		public Statement ElsePartStatement {
			get {
				return Pieces[6] as Statement;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Expression);
				ret.Add(IfPartStatement);
				if (false == (ElsePartStatement is MissingStatement))
				{
					ret.Add(ElsePartStatement);
				}
				return ret;
			}
		}
	}
}
