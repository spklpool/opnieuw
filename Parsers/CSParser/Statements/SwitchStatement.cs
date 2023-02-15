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
	public class SwitchStatement : Statement
	{
		public new static Statement parse(TokenProvider tokenizer)
		{
			Statement ret = new MissingStatement();
			if (Token.SWITCH == tokenizer.CurrentToken.Type)
			{
				tokenizer.setBookmark();
				PositionToken switchToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // switch
				PositionToken openParensToken = tokenizer.checkToken(Token.OPEN_PARENS);
				Expression expression = Expression.parse(tokenizer);
				expression.checkNotMissing();
				PositionToken closeParensToken = tokenizer.checkToken(Token.CLOSE_PARENS);
				SwitchBlock switchBlock = SwitchBlock.parse(tokenizer);
				switchBlock.checkNotMissing();
				ret = new SwitchStatement(switchToken, openParensToken, expression, closeParensToken, switchBlock);
				tokenizer.endBookmark(ret is MissingStatement);
			}
			return ret;
		}

		public SwitchStatement(PositionToken switchToken, PositionToken openParensToken, Expression expression, PositionToken closeParensToken, SwitchBlock switchBlock) :
		base(switchToken, openParensToken, expression, closeParensToken, switchBlock)
		{
		}
		
		public PositionToken SwitchToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}
		
		public PositionToken OpenParensToken {
			get {
				return Pieces[1] as PositionToken;
			}
		}

		public Expression Expression {
			get {
				return Pieces[2] as Expression;
			}
		}
		
		public PositionToken CloseParensToken {
			get {
				return Pieces[3] as PositionToken;
			}
		}

		public SwitchBlock SwitchBlock {
			get {
				return Pieces[4] as SwitchBlock;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Expression);
				ret.Add(SwitchBlock);
				return ret;
			}
		}
	}
}
