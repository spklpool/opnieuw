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
	public class LabeledStatement : Statement
    {
#region static parsing code
        public new static Statement parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			Statement ret = new MissingStatement();
			if (Token.IDENTIFIER == tokenizer.CurrentToken.Type)
			{
				Identifier identifier = Identifier.parse(tokenizer);
				if (Token.COLON == tokenizer.CurrentToken.Type)
				{
					PositionToken colonToken = tokenizer.CurrentToken;
					tokenizer.nextToken(); // :
					Statement statement = Statement.parse(tokenizer);
					if (false == (statement is MissingStatement))
					{
						ret = new LabeledStatement(identifier, colonToken, statement);
					}
				}
			}
			tokenizer.endBookmark(ret is MissingStatement);
			return ret;
        }
#endregion

#region constructors
        public LabeledStatement(Identifier identifier, PositionToken colonToken, Statement statement) :
		base(identifier, colonToken, statement)
		{
        }

        public LabeledStatement(Identifier identifier, Statement statement) :
		base(identifier, 
             new PositionToken(Position.Missing, ":", Token.COLON), 
             statement)
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
            }
        }

        public Identifier Identifier {
			get {
				return Pieces[0] as Identifier;
			}
		}

		public PositionToken ColonToken {
			get {
				return Pieces[1] as PositionToken;
			}
		}

		public Statement Statement {
			get {
				return Pieces[2] as Statement;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Statement);
				return ret;
			}
		}
	}
}
