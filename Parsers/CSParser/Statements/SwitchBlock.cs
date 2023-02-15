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
	public class SwitchBlock : PieceOfCode
	{
		public static SwitchBlock parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			SwitchBlock ret = new MissingSwitchBlock();
			if (Token.OPEN_BRACE == tokenizer.CurrentToken.Type)
			{
				PositionToken openBraceToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // {
				SwitchSectionCollection sections = SwitchSectionCollection.parse(tokenizer);
				if (Token.CLOSE_BRACE == tokenizer.CurrentToken.Type)
				{
					PositionToken closeBraceToken = tokenizer.CurrentToken;
					tokenizer.nextToken(); // }
					ret = new SwitchBlock(openBraceToken, sections, closeBraceToken);
				}
			}
			tokenizer.endBookmark(ret is MissingSwitchBlock);
			return ret;
		}

		public SwitchBlock()
		{
		}

		public SwitchBlock(PositionToken openBraceToken, SwitchSectionCollection sections, PositionToken closeBraceToken) :
		base(openBraceToken, sections, closeBraceToken)
		{
		}
		
		public PositionToken OpenBraceToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}

		public SwitchSectionCollection Sections {
			get {
				return Pieces[1] as SwitchSectionCollection;
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
				ret.Add(Sections);
				return ret;
			}
		}
	}

	public class MissingSwitchBlock : SwitchBlock
	{
		public override void checkNotMissing()
		{
			throw new ParserException("Missing switch block.");
		}
	}
}
