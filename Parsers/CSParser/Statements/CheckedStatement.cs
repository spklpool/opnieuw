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
	public class CheckedStatement : Statement
    {
#region static parsing code
        //checked block
        public new static Statement parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			Statement ret = new MissingStatement();
			if (Token.CHECKED == tokenizer.CurrentToken.Type)
			{
				PositionToken checkedToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // checked
				Statement block = BlockStatement.parse(tokenizer);
				if (block is BlockStatement)
				{
					ret = new CheckedStatement(checkedToken, block as BlockStatement);
				}
			}
			tokenizer.endBookmark(ret is MissingStatement);
			return ret;
        }
#endregion

#region constructors
        public CheckedStatement(PositionToken checkedToken, BlockStatement block) :
		base(checkedToken, block)
		{
        }

        public CheckedStatement(BlockStatement block) :
		base(new PositionToken(Position.Missing, "checked", Token.CHECKED), 
             block)
        {
        }
#endregion

        public PositionToken CheckedToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}

		public BlockStatement Block {
			get {
				return Pieces[1] as BlockStatement;
			}
		}
		
		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Block);
				return ret;
			}
		}
	}
}
