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
	public class TryStatement : Statement
    {
#region static parsing code
        /// <summary>
        /// try block catch-clauses
		/// try block finally-clause
		/// try block catch-clauses finally-clause 
		/// </summary>
		public new static Statement parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			Statement ret = new MissingStatement();
			PositionToken tryToken = tokenizer.checkToken(Token.TRY);
			Statement tryStatement = BlockStatement.parse(tokenizer);
			if (false == (tryStatement is MissingStatement))
			{
				CatchClauseCollection catchClauses = CatchClauseCollection.parse(tokenizer);
				FinallyClause finallyClause = FinallyClause.parse(tokenizer);
				ret = new TryStatement(tryToken, tryStatement, catchClauses, finallyClause);
			}
			tokenizer.endBookmark(ret is MissingStatement);
			return ret;
        }
#endregion

#region constructors
        public TryStatement() :
		base(PositionToken.Missing, new MissingStatement(), new CatchClauseCollection(), new MissingFinallyClause())
		{
		}

		public TryStatement(PositionToken tryToken, Statement body, CatchClauseCollection catchClauses, FinallyClause finallyClause) :
		base(tryToken, body, catchClauses, finallyClause)
		{
        }

		public TryStatement(Statement body, CatchClause catchClause) :
		base(new PositionToken(Position.Missing, "try", Token.TRY), 
             body, 
             new CatchClauseCollection(catchClause), 
             new MissingFinallyClause())
		{
        }

		public TryStatement(Statement body, CatchClause catchClause, FinallyClause finallyClause) :
		base(new PositionToken(Position.Missing, "try", Token.TRY), 
             body, 
             new CatchClauseCollection(catchClause), 
             finallyClause)
		{
        }

		public TryStatement(Statement body, CatchClauseCollection catchClauses) :
		base(new PositionToken(Position.Missing, "try", Token.TRY), 
             body, 
             catchClauses, 
             new MissingFinallyClause())
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

        public PositionToken TryToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}

		public Statement Body {
			get {
				return Pieces[1] as Statement;
			}
		}

		public CatchClauseCollection CatchClauses {
			get {
				return Pieces[2] as CatchClauseCollection;
			}
		}

		public FinallyClause FinallyClause {
			get {
				return Pieces[3] as FinallyClause;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Body);
				ret.Add(CatchClauses);
				ret.Add(FinallyClause);
				return ret;
			}
		}
	}

	public class MissingTryStatement : TryStatement
	{
	}
}
