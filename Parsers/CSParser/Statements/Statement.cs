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
using System.IO;
using Opnieuw.Framework;

namespace Opnieuw.Parsers.CSParser
{
	public class Statement : PieceOfCode
	{
		public static Statement parse(TokenProvider tokenizer)
		{
			Statement ret = new MissingStatement();
			if (Token.IF == tokenizer.CurrentToken.Type)
			{
				ret = IfStatement.parse(tokenizer);
			}
			else if (Token.SWITCH == tokenizer.CurrentToken.Type)
			{
				ret = SwitchStatement.parse(tokenizer);
			}
			else if (Token.WHILE == tokenizer.CurrentToken.Type)
			{
				ret = WhileStatement.parse(tokenizer);
			}
			else if (Token.FOREACH == tokenizer.CurrentToken.Type)
			{
				ret = ForeachStatement.parse(tokenizer);
			}
			else if (Token.FOR == tokenizer.CurrentToken.Type)
			{
				ret = ForStatement.parse(tokenizer);
			}
			else if (Token.DO == tokenizer.CurrentToken.Type)
			{
				ret = DoStatement.parse(tokenizer);
			}
			else if (Token.TRY == tokenizer.CurrentToken.Type)
			{
				ret = TryStatement.parse(tokenizer);
			}
			else if (Token.OPEN_BRACE == tokenizer.CurrentToken.Type)
			{
				ret = BlockStatement.parse(tokenizer);
			}
			else if (Token.SEMICOLON == tokenizer.CurrentToken.Type)
			{
				ret = EmptyStatement.parse(tokenizer);
			}
			else if (Token.CHECKED == tokenizer.CurrentToken.Type)
			{
				ret = CheckedStatement.parse(tokenizer);
			}
			else if (Token.UNCHECKED == tokenizer.CurrentToken.Type)
			{
				ret = UncheckedStatement.parse(tokenizer);
			}
			else if (Token.LOCK == tokenizer.CurrentToken.Type)
			{
				ret = LockStatement.parse(tokenizer);
			}
			else if (Token.USING == tokenizer.CurrentToken.Type)
			{
				ret = UsingStatement.parse(tokenizer);
			}
			else if (Token.IDENTIFIER == tokenizer.CurrentToken.Type)
			{
				ret = LabeledStatement.parse(tokenizer);
			}
			if (ret is MissingStatement)
			{
				ret = JumpStatement.parse(tokenizer);
				if (ret is MissingStatement)
				{
					ret = DeclarationStatement.parse(tokenizer);
					if (ret is MissingStatement)
					{
						ret = ExpressionStatement.parse(tokenizer);
					}
				}
			}
			return ret;
        }

        public static Statement parse(string name)
        {
            Tokenizer t = new Tokenizer(new StringReader(name), "", null);
            t.nextToken();
            return Statement.parse(t) as Statement;
        }

        public Statement()
		{
		}

		public Statement(params FundamentalPieceOfCode[] list) :
		base(list)
		{
		}

		public Statement(Position position)
		{
			m_Position = position;
		}

        public PieceOfCode Clone {
            get {
                return Statement.parse(Generate());
            }
        }

        public override void Reindent(int newIndent)
        {
            Format();
            String statementString = Generate();
            String reindentedStatementString = GenericBlockOfCode.Reindent(statementString, newIndent, 4);
			Statement statementClone = Statement.parse(reindentedStatementString);
            CloneFormat(statementClone);
        }

		public virtual StatementCollection Statements {
			get {
				return new StatementCollection();
			}
		}
	}

	public class MissingStatement : Statement
	{
		public override void checkNotMissing()
		{
			throw new ParserException();
		}
	}
}
