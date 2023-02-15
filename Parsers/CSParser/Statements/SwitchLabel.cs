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
	public class SwitchLabel : PieceOfCode
	{
		public static SwitchLabel parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			SwitchLabel ret = new MissingSwitchLabel();
			if (Token.CASE == tokenizer.CurrentToken.Type)
			{
				PositionToken caseToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // case
				Expression expression = Expression.parse(tokenizer);
				expression.checkNotMissing();
				PositionToken colonToken = tokenizer.checkToken(Token.COLON);
				ret = new SwitchLabel(caseToken, expression, colonToken);
			}
			else if (Token.DEFAULT == tokenizer.CurrentToken.Type)
			{
				PositionToken defaultToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // default
				PositionToken colonToken = tokenizer.checkToken(Token.COLON);
                ret = new SwitchLabel(defaultToken, new MissingExpression(), colonToken);
			}
			tokenizer.endBookmark(ret is MissingSwitchLabel);
			return ret;
		}

		public SwitchLabel()
		{
		}

		public SwitchLabel(PositionToken labelToken, Expression expression, PositionToken colonToken) :
		base(labelToken, expression, colonToken)
		{
		}
		
		public PositionToken LabelToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}

		public Expression Expression {
			get {
				return Pieces[1] as Expression;
			}
		}
		
		public PositionToken ColonToken {
			get {
				return Pieces[2] as PositionToken;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Expression);
				return ret;
			}
		}
	}

	public class MissingSwitchLabel : SwitchLabel
	{
	}
}
