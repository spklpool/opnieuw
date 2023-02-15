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
	public class FinallyClause : PieceOfCode
    {
#region static parsing code
        //finally-token body
        public static FinallyClause parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			FinallyClause ret = new MissingFinallyClause();
			if (Token.FINALLY == tokenizer.CurrentToken.Type)
			{
				PositionToken finallyToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // finally
				Statement statement = BlockStatement.parse(tokenizer);
				if (false == (statement is MissingStatement))
				{
					ret = new FinallyClause(finallyToken, statement);
				}
			}
			tokenizer.endBookmark(ret is MissingFinallyClause);
			return ret;
        }
#endregion

#region constructors
        public FinallyClause()
		{
			Pieces.Add(PositionToken.Missing);
			Pieces.Add(new MissingStatement());
		}

		public FinallyClause(PositionToken finallyToken, Statement body) :
		base(finallyToken, body)
		{
        }

		public FinallyClause(Statement body) :
		base(new PositionToken(Position.Missing, "finally", Token.FINALLY), 
             body)
		{
        }
#endregion

        public override void Format()
        {
            base.Format();
            if (Body is BlockStatement) {
                Body.LeadingCharacters = " ";
            }
        }

        public PositionToken FinallyToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}

		public Statement Body {
			get {
				return Pieces[1] as Statement;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Body);
				return ret;
			}
		}
	}

	public class MissingFinallyClause : FinallyClause
	{
	}
}
