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
	public class UsingStatement : Statement
    {
#region static parsing code
        //using ( local-variable-declaration ) embedded-statement
        //using ( expression ) embedded-statement
        public new static Statement parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			Statement ret = new MissingStatement();
			if (Token.USING == tokenizer.CurrentToken.Type)
			{
				PositionToken usingToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // using
				if (Token.OPEN_PARENS == tokenizer.CurrentToken.Type)
				{
					PositionToken openParensToken = tokenizer.CurrentToken;
					tokenizer.nextToken(); // (
					Expression exp = new MissingExpression();
					LocalVariableDeclaration lvd = LocalVariableDeclaration.parse(tokenizer);
					if (lvd is MissingLocalVariableDeclaration)
					{
						exp = Expression.parse(tokenizer);
					}
					if ((false == (lvd is MissingLocalVariableDeclaration)) ||
						(false == (exp is MissingExpression)))
					{
						if (Token.CLOSE_PARENS == tokenizer.CurrentToken.Type)
						{
							PositionToken closeParensToken = tokenizer.CurrentToken;
							tokenizer.nextToken(); // )
							Statement stmt = Statement.parse(tokenizer);
							if (false == (stmt is MissingStatement))
							{
								Position position = new Position(usingToken, stmt);
								ret = new UsingStatement(usingToken, openParensToken, lvd, exp, closeParensToken, stmt, position);
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
        public UsingStatement(PositionToken usingToken, PositionToken openParensToken, LocalVariableDeclaration lvd, Expression exp, PositionToken closeParensToken, Statement stmt, Position position) :
		base(usingToken, openParensToken, lvd, exp, closeParensToken, stmt)
		{
		}

        public UsingStatement(LocalVariableDeclaration lvd, Statement stmt) :
		base(new PositionToken(Position.Missing, "using", Token.USING), 
             new PositionToken(Position.Missing, "(", Token.OPEN_PARENS), 
             lvd, new MissingExpression(), 
             new PositionToken(Position.Missing, ")", Token.CLOSE_PARENS), 
             stmt)
		{
        }

        public UsingStatement(Expression exp, Statement stmt) :
		base(new PositionToken(Position.Missing, "using", Token.USING), 
             new PositionToken(Position.Missing, "(", Token.OPEN_PARENS), 
             new MissingLocalVariableDeclaration(), exp, 
             new PositionToken(Position.Missing, ")", Token.CLOSE_PARENS), 
             stmt)
		{
        }
#endregion

        public override void Format()
        {
            base.Format();
            if (!(Statement is BlockStatement)) {
                string statementBlock = Statement.Generate();
                string newStatementBlock = GenericBlockOfCode.Reindent(statementBlock, 4, 4);
                Statement newStatement = Statement.parse(newStatementBlock);
                Statement.CloneFormat(newStatement);
            } else {
                Statement.LeadingCharacters = " ";
            }
        }

		public PositionToken UsingToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}

		public PositionToken OpenParensToken {
			get {
				return Pieces[1] as PositionToken;
			}
		}

		public LocalVariableDeclaration LocalVariableDeclaration {
			get {
				return Pieces[2] as LocalVariableDeclaration;
			}
		}

		public Expression Expression {
			get {
				return Pieces[3] as Expression;
			}
		}


		public PositionToken CloseParensToken {
			get {
				return Pieces[4] as PositionToken;
			}
		}

		public Statement Statement {
			get {
				return Pieces[5] as Statement;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(LocalVariableDeclaration);
				ret.Add(Expression);
				ret.Add(Statement);
				return ret;
			}
		}

		public override VariableCollection DeclaredVariables {
			get {
				VariableCollection ret = new VariableCollection();
				if (false == (LocalVariableDeclaration is MissingLocalVariableDeclaration))
				{
					foreach (VariableDeclarator vd in LocalVariableDeclaration.VariableDeclarators)
					{
						ret.Add(new Variable(vd.Name, LocalVariableDeclaration.Type, vd.Initializer, this.Parent as PieceOfCode));
					}
				}
				return ret;
			}
		}
	}
}
