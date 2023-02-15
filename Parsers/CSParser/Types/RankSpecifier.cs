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
	public class RankSpecifier : PieceOfCode
	{
		public static RankSpecifier parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			RankSpecifier ret = new MissingRankSpecifier();
			if (Token.OPEN_BRACKET == tokenizer.CurrentToken.Type)
			{
				PositionToken openBracketToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // [
				DimSeperatorCollection dimSeperators = DimSeperatorCollection.parse(tokenizer);
				if (Token.CLOSE_BRACKET == tokenizer.CurrentToken.Type)
				{
					PositionToken closeBracketToken = tokenizer.CurrentToken;
					tokenizer.nextToken(); // ]
					ret = new RankSpecifier(openBracketToken, dimSeperators, closeBracketToken);
				}
			}
			tokenizer.endBookmark(ret is MissingRankSpecifier);
			return ret;
		}

		public RankSpecifier() :
		base(PositionToken.Missing, new DimSeperatorCollection(), PositionToken.Missing)
		{
		}

		public RankSpecifier(PositionToken openBracketToken, DimSeperatorCollection dimSeperators, PositionToken closeBracketToken) :
		base(openBracketToken, dimSeperators, closeBracketToken)
		{
		}
		
		public PositionToken OpenBracketToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}

		public DimSeperatorCollection DimSeperators {
			get {
				return Pieces[1] as DimSeperatorCollection;
			}
		}
		
		public PositionToken CloseBracketToken {
			get {
				return Pieces[2] as PositionToken;
			}
		}
	}

	public class MissingRankSpecifier : RankSpecifier
	{
	}
}
