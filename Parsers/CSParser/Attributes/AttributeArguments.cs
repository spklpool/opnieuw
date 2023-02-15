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
	/// ( positional-argument-listopt )
	/// ( positional-argument-list , named-argument-list )
	///	( named-argument-list ) 
	/// </summary>
	public class AttributeArguments : PieceOfCode
	{
		public static AttributeArguments parse(TokenProvider tokenizer)
		{
			AttributeArguments ret = new MissingAttributeArguments();
			if (Token.OPEN_PARENS == tokenizer.CurrentToken.Type)
			{
				PositionToken openParensToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // (
				AttributePositionalArgumentList positionalArguments = AttributePositionalArgumentList.parse(tokenizer);
				AttributeNamedArgumentList namedArguments = new AttributeNamedArgumentList();
				PositionToken commaToken = PositionToken.Missing;
				if (Token.COMMA == tokenizer.CurrentToken.Type)
				{
					commaToken = tokenizer.CurrentToken;
					tokenizer.nextToken(); // ,
					namedArguments = AttributeNamedArgumentList.parse(tokenizer);
				}
				PositionToken closeParensToken = tokenizer.checkToken(Token.CLOSE_PARENS);
				ret = new AttributeArguments(openParensToken, positionalArguments, commaToken, namedArguments, closeParensToken);
			}
			return ret;
		}

		public AttributeArguments()
		{
			Pieces.Add(PositionToken.Missing);
			Pieces.Add(new AttributePositionalArgumentList());
			Pieces.Add(PositionToken.Missing);
			Pieces.Add(new AttributeNamedArgumentList());
			Pieces.Add(PositionToken.Missing);
		}

		public AttributeArguments(PositionToken openParensToken, AttributePositionalArgumentList positionalArguments, PositionToken commaToken, AttributeNamedArgumentList namedArguments, PositionToken closeParensToken) :
		base(openParensToken, positionalArguments, commaToken, namedArguments, closeParensToken)
		{
		}
		
		public PositionToken OpenParensToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}

		public AttributePositionalArgumentList PositionalArguments {
			get {
				return Pieces[1] as AttributePositionalArgumentList;
			}
		}

		public PositionToken CommaToken {
			get {
				return Pieces[2] as PositionToken;
			}
		}

		public AttributeNamedArgumentList NamedArguments {
			get {
				return Pieces[3] as AttributeNamedArgumentList;
			}
		}
		
		public PositionToken CloseParensToken {
			get {
				return Pieces[4] as PositionToken;
			}
		}
	}

	public class MissingAttributeArguments : AttributeArguments
	{
	}
}
