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
	/// <summary>
	/// attribute-section: 
	/// [ attribute-target-specifieropt attribute-list ]
	/// [ attribute-target-specifieropt attribute-list , ] 
	/// </summary>
	public class AttributeSection : PieceOfCode
	{
		public static bool isThere(TokenProvider tokenizer)
		{
			return (Token.OPEN_BRACKET == tokenizer.CurrentToken.Type);
		}

		public static AttributeSection Parse(TokenProvider tokenizer)
		{
			AttributeSection ret = new MissingAttributeSection();
			if (Token.OPEN_BRACKET == tokenizer.CurrentToken.Type)
			{
				PositionToken openBracketToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // [
				AttributeTargetSpecifier targetSpecifier = AttributeTargetSpecifier.parse(tokenizer);
				AttributeCollection attributes = AttributeCollection.parse(tokenizer);
				PositionToken closeBracketToken = tokenizer.checkToken(Token.CLOSE_BRACKET);
				ret = new AttributeSection(openBracketToken, targetSpecifier, attributes, closeBracketToken);
			}
			return ret;
		}

		public AttributeSection()
		{
			Pieces.Add(PositionToken.Missing);
			Pieces.Add(new MissingAttributeTargetSpecifier());
			Pieces.Add(new AttributeCollection());
			Pieces.Add(PositionToken.Missing);
		}

		public AttributeSection(AttributeTargetSpecifier target, AttributeCollection attributes) :
		base(new PositionToken(new InvalidPosition(), "[", Token.OPEN_BRACKET),
			 target,
			 attributes,
			 new PositionToken(new InvalidPosition(), "]", Token.CLOSE_BRACKET))
		{
		}

		public AttributeSection(AttributeCollection attributes) :
		base(new PositionToken(new InvalidPosition(), "[", Token.OPEN_BRACKET),
			 new MissingAttributeTargetSpecifier(),
			 attributes,
			 new PositionToken(new InvalidPosition(), "]", Token.CLOSE_BRACKET))
		{
		}

		public AttributeSection(PositionToken openBracketToken, AttributeTargetSpecifier targetSpecifier, AttributeCollection attributes, PositionToken closeBracketToken) :
		base(openBracketToken, targetSpecifier, attributes, closeBracketToken)
		{
		}
		
		public PositionToken OpenBracketToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}

		public AttributeTargetSpecifier AttributeTargetSpecifier {
			get {
				return Pieces[1] as AttributeTargetSpecifier;
			}
		}

		public AttributeCollection Attributes {
			get {
				return Pieces[2] as AttributeCollection;
			}
		}
		
		public PositionToken CloseBracketToken {
			get {
				return Pieces[3] as PositionToken;
			}
		}
	}

	public class MissingAttributeSection : AttributeSection
	{
	}
}
