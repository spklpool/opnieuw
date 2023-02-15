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
	public class ForStatement : Statement
    {
#region static parsing code
        //for ( for-initializeropt ; for-conditionopt ; for-iteratoropt ) embedded-statement
        public new static Statement parse(TokenProvider tokenizer)
		{
			if (Token.FOR == tokenizer.CurrentToken.Type)
			{
				PositionToken forToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // for
				PositionToken openParensToken = tokenizer.checkToken(Token.OPEN_PARENS);
				ForInitializer initializer = ForInitializer.parse(tokenizer);
				PositionToken semicolonToken1 = tokenizer.checkToken(Token.SEMICOLON);
				Expression condition = Expression.parse(tokenizer);
				PositionToken semicolonToken2 = tokenizer.checkToken(Token.SEMICOLON);
				ExpressionCollection iterator = ExpressionCollection.parse(tokenizer);
				PositionToken closeParensToken = tokenizer.checkToken(Token.CLOSE_PARENS);
				Statement statement = Statement.parse(tokenizer);
				Statement ret = new ForStatement(forToken, openParensToken, initializer, semicolonToken1, condition, semicolonToken2, iterator, closeParensToken, statement);
				return ret;
			}
			else
			{
				return new MissingStatement();
			}
        }
#endregion

#region constructors
        public ForStatement() :
		base(PositionToken.Missing, PositionToken.Missing, new MissingForInitializer(), 
             PositionToken.Missing, new MissingExpression(), PositionToken.Missing, 
             new ExpressionCollection(), PositionToken.Missing, new MissingStatement())
		{
        }

        public ForStatement(ForInitializer initializer, Expression condition, 
                            ExpressionCollection iterator, Statement statement) :
		base(new PositionToken(Position.Missing, "for", Token.FOR), 
             new PositionToken(Position.Missing, "(", Token.OPEN_PARENS), 
             initializer, 
             new PositionToken(Position.Missing, ";", Token.SEMICOLON), 
             condition, 
             new PositionToken(Position.Missing, ";", Token.SEMICOLON), 
             iterator, 
             new PositionToken(Position.Missing, ")", Token.CLOSE_PARENS), 
             statement)
        {
        }

        public ForStatement(PositionToken forToken, PositionToken openParensToken, 
                            ForInitializer initializer, PositionToken semicolonToken1, 
                            Expression condition, PositionToken semicolonToken2, 
                            ExpressionCollection iterator, PositionToken closeParensToken, 
                            Statement statement) :
		base(forToken, openParensToken, initializer, semicolonToken1, condition, semicolonToken2, 
             iterator, closeParensToken, statement)
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
                Pieces[8] = newStatement;
            }
        }

        public PositionToken ForToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}

		public PositionToken OpenParensToken {
			get {
				return Pieces[1] as PositionToken;
			}
		}

		public ForInitializer Initializer {
			get {
				return Pieces[2] as ForInitializer;
			}
		}

		public PositionToken SemicolonToken1 {
			get {
				return Pieces[3] as PositionToken;
			}
		}

		public Expression Condition {
			get {
				return Pieces[4] as Expression;
			}
		}

		public PositionToken SemicolonToken2 {
			get {
				return Pieces[5] as PositionToken;
			}
		}

		public ExpressionCollection Iterator {
			get {
				return Pieces[6] as ExpressionCollection;
			}
		}

		public PositionToken CloseParensToken {
			get {
				return Pieces[7] as PositionToken;
			}
		}

		public Statement Statement {
			get {
				return Pieces[8] as Statement;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Initializer.Children);
				ret.Add(Condition);
				ret.Add(Iterator);
				ret.Add(Statement);
				return ret;
			}
		}

		public override VariableCollection DeclaredVariables {
			get {
				VariableCollection ret = new VariableCollection();
				if (false == (Initializer.LocalVariableDeclaration is MissingLocalVariableDeclaration))
				{
					foreach (VariableDeclarator vd in Initializer.LocalVariableDeclaration.VariableDeclarators)
					{
						ret.Add(new Variable(vd.Name, Initializer.LocalVariableDeclaration.Type, vd.Initializer, this));
					}
				}
				return ret;
			}
		}
	}

	public class MissingForStatement : ForStatement
	{
	}
}
