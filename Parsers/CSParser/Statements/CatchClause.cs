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
	public class CatchClause : PieceOfCode
    {
#region static parsing code
        /// <summary>
        /// catch ( class-type identifieropt ) block 
		/// catch block 
		/// </summary>
		public static CatchClause parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			CatchClause ret = new MissingCatchClause();
			if (Token.CATCH == tokenizer.CurrentToken.Type)
			{
				PositionToken catchToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // catch
				DataType type = new MissingDataType();
				Identifier identifier = new MissingIdentifier();
				PositionToken openParensToken = PositionToken.Missing;
				PositionToken closeParensToken = PositionToken.Missing;
				if (Token.OPEN_PARENS == tokenizer.CurrentToken.Type)
				{
					openParensToken = tokenizer.CurrentToken;
					tokenizer.nextToken(); // (
					type = DataType.parse(tokenizer);
					type.checkNotMissing();
					identifier = Identifier.parse(tokenizer);
					closeParensToken = tokenizer.checkToken(Token.CLOSE_PARENS);
				}
				Statement statement = BlockStatement.parse(tokenizer);
				statement.checkNotMissing();
				ret = new CatchClause(catchToken, openParensToken, type, identifier, closeParensToken, statement);
			}
			tokenizer.endBookmark(ret is MissingCatchClause);
			return ret;
        }
#endregion

#region constructors
        public CatchClause()
		{
			Pieces.Add(PositionToken.Missing);
			Pieces.Add(PositionToken.Missing);
			Pieces.Add(new MissingDataType());
			Pieces.Add(new MissingIdentifier());
			Pieces.Add(PositionToken.Missing);
			Pieces.Add(new MissingStatement());
		}

		public CatchClause(PositionToken catchToken, PositionToken openParensToken, DataType type, Identifier identifier, PositionToken closeParensToken, Statement body) :
		base (catchToken, openParensToken, type, identifier, closeParensToken, body)
		{
        }

		public CatchClause(DataType type, Identifier identifier, Statement body) :
		base (new PositionToken(Position.Missing, "catch", Token.CATCH), 
              new PositionToken(Position.Missing, "(", Token.OPEN_PARENS), 
              type, 
              identifier, 
              new PositionToken(Position.Missing, ")", Token.CLOSE_PARENS), 
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

        public PositionToken CatchToken {
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
		
		public PositionToken CloseParensToken {
			get {
				return Pieces[4] as PositionToken;
			}
		}		

		public Statement Body {
			get {
				return Pieces[5] as Statement;
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

	public class MissingCatchClause : CatchClause
	{
	}
}
