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
	public class ConstructorInitializer : PieceOfCode
	{
		/// <summary>
		/// : base ( argument-listopt )
		/// : this ( argument-listopt ) 
		/// </summary>
		public static ConstructorInitializer parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			ConstructorInitializer ret = new MissingConstructorInitializer();
			if (Token.COLON == tokenizer.CurrentToken.Type)
			{
				PositionToken colonToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // :
				if ((Token.BASE == tokenizer.CurrentToken.Type) ||
					(Token.THIS == tokenizer.CurrentToken.Type))
				{
					PositionToken initializerTypeToken = tokenizer.CurrentToken;
					tokenizer.nextToken(); // base or this
					if (Token.OPEN_PARENS == tokenizer.CurrentToken.Type)
					{
						PositionToken openParensToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // (
						ArgumentCollection arguments = ArgumentCollection.parse(tokenizer);
						if (Token.CLOSE_PARENS == tokenizer.CurrentToken.Type)
						{
							PositionToken closeParensToken = tokenizer.CurrentToken;
							tokenizer.nextToken(); // )
							ret = new ConstructorInitializer(colonToken, initializerTypeToken, openParensToken, arguments, closeParensToken);
						}
					}
				}
			}
			tokenizer.endBookmark(ret is MissingConstructorInitializer);
			return ret;
		}

		public ConstructorInitializer(PositionToken colonToken, PositionToken initializerTypeToken, PositionToken openParensToken, ArgumentCollection arguments, PositionToken closeParensToken) :
		base(colonToken, initializerTypeToken, openParensToken, arguments, closeParensToken)
		{
		}
		
		public PositionToken ColonToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}
		
		public PositionToken InitializerTypeToken {
			get {
				return Pieces[1] as PositionToken;
			}
		}
		
		public PositionToken OpenParensToken {
			get {
				return Pieces[2] as PositionToken;
			}
		}

		public ArgumentCollection Arguments {
			get {
				return Pieces[3] as ArgumentCollection;
			}
		}
		
		public PositionToken CloseParensToken {
			get {
				return Pieces[4] as PositionToken;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Arguments);
				return ret;
			}
		}
	}

	public class MissingConstructorInitializer : ConstructorInitializer
	{
		public MissingConstructorInitializer() :
		base(PositionToken.Missing, PositionToken.Missing, 
			 PositionToken.Missing, ArgumentCollection.Missing, 
			 PositionToken.Missing)
		{
		}
	}
}
