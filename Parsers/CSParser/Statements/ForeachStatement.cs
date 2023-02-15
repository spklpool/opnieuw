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
	public class ForeachStatement : Statement
    {
#region static parsing code
        //foreach ( type identifier in expression ) embedded-statement
        public new static Statement parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			Statement ret = new MissingStatement();
			if (Token.FOREACH == tokenizer.CurrentToken.Type)
			{
				PositionToken foreachToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // foreach
				PositionToken openParensToken = tokenizer.CurrentToken;
				if (Token.OPEN_PARENS == openParensToken.Type)
				{
					tokenizer.nextToken(); // (
					DataType type = DataType.parse(tokenizer);
					if (false == (type is MissingDataType))
					{
						Identifier identifier = Identifier.parse(tokenizer);
						if (false == (identifier is MissingIdentifier))
						{
							PositionToken inToken = tokenizer.CurrentToken;
							if (Token.IN == inToken.Type)
							{
								tokenizer.nextToken(); // in
								Expression expression = Expression.parse(tokenizer);
								if (false == (expression is MissingExpression))
								{
									PositionToken closeParensToken = tokenizer.CurrentToken;
									if (Token.CLOSE_PARENS == closeParensToken.Type)
									{
										tokenizer.nextToken(); // )
										Statement statement = Statement.parse(tokenizer);
										if (false == (statement is MissingStatement))
										{
											ret = new ForeachStatement(foreachToken, openParensToken, 
																	   type, identifier, inToken, 
																	   expression, closeParensToken, 
																	   statement);
										}
									}
								}
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
        public ForeachStatement(PositionToken foreachToken, PositionToken openParensToken, 
								DataType type, Identifier identifier, PositionToken inToken, 
								Expression expression, PositionToken closeParensToken, 
								Statement statement) :
		base(foreachToken, openParensToken, type, identifier, inToken, expression, closeParensToken, statement)
		{
        }

        public ForeachStatement(DataType type, Identifier identifier, 
                                Expression expression, Statement statement) :
		base(new PositionToken(Position.Missing, "foreach", Token.FOREACH), 
             new PositionToken(Position.Missing, "(", Token.OPEN_PARENS), 
             type, identifier, 
             new PositionToken(Position.Missing, "in", Token.IN), 
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
                Pieces[7] = newStatement;
            }
        }

        public PositionToken ForeachToken {
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

		public Identifier Identifier {
			get {
				return Pieces[3] as Identifier;
			}
		}

		public PositionToken InToken {
			get {
				return Pieces[4] as PositionToken;
			}
		}

		public Expression Expression {
			get {
				return Pieces[5] as Expression;
			}
		}

		public PositionToken CloseParensToken {
			get {
				return Pieces[6] as PositionToken;
			}
		}

		public Statement Statement {
			get {
				return Pieces[7] as Statement;
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

		public override VariableCollection DeclaredVariables {
			get {
				VariableCollection ret = new VariableCollection();
				ret.Add(new Variable(Identifier.Name, Type.Name, new MissingVariableInitializer(), this));
				return ret;
			}
		}
	}
}
