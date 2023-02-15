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
    public class BlockStatement : Statement, StatementContainer
    {
#region static parting code
        public new static Statement parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			Statement ret = new MissingStatement();
			PositionToken openBraceToken = tokenizer.CurrentToken;
			if (Token.OPEN_BRACE == tokenizer.CurrentToken.Type)
			{
				tokenizer.nextToken(); // {
				StatementCollection statements = StatementCollection.parse(tokenizer);
				PositionToken closeBraceToken = tokenizer.checkToken(Token.CLOSE_BRACE);
				ret = new BlockStatement(openBraceToken, statements, closeBraceToken);
                ret.PropagateUp();
			}
			tokenizer.endBookmark(ret is MissingStatement);
			return ret;
        }
#endregion

#region constructors
        public BlockStatement() :
		base(new PositionToken(Position.Missing, "{", Token.OPEN_BRACE), 
			 new StatementCollection(), 
			 new PositionToken(Position.Missing, "}", Token.CLOSE_BRACE))
        {
        }

        public BlockStatement(StatementCollection statements) :
		base(new PositionToken(Position.Missing, "{", Token.OPEN_BRACE), 
			 statements, 
			 new PositionToken(Position.Missing, "}", Token.CLOSE_BRACE))
		{
        }

		public BlockStatement(PositionToken openBraceToken, StatementCollection statements, PositionToken closeBraceToken) :
		base(openBraceToken, statements, closeBraceToken)
		{
        }
#endregion

        public override void Format()
        {
            base.Format();
            for (int i = 0; i < Statements.Count; i++)
            {
                string statementBlock = Statements[i].Generate();
                statementBlock = GenericBlockOfCode.RemoveIndent(statementBlock, 4);
                string newStatementBlock = GenericBlockOfCode.Reindent(statementBlock, Indent+4, 4);
                Statement newStatement = Statement.parse(newStatementBlock);
                Statements[i] = newStatement;
            }
        }

        public PositionToken OpenBraceToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}

		public override StatementCollection Statements {
			get {
				return Pieces[1] as StatementCollection;
			}
		}
		
		public PositionToken CloseBraceToken {
			get {
				return Pieces[2] as PositionToken;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Statements.Children);
				return ret;
			}
		}

		public override VariableCollection DeclaredVariables {
			get {
				VariableCollection ret = new VariableCollection();
				foreach (Statement statement in Statements)
				{
					ret.Add(statement.DeclaredVariables);
				}
				return ret;
			}
		}
	}
}
