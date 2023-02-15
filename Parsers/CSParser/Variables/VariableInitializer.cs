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
	public class VariableInitializer : PieceOfCode
	{
		public static VariableInitializer parse(TokenProvider tokenizer)
		{
			VariableInitializer ret = new MissingVariableInitializer();
/*
			//This is an optimization that takes advantage of the 
			//fact that variable initializers are often literals.
			//If we don't do this, long array initializer lists
			//take an insane amount of time to parse.
			//If we find a literal followed by a comma, we can 
			//return the literal immediately as our initializer.
			if ((tokenizer.CurrentToken.Type == Token.LITERAL_CHARACTER) ||
				(tokenizer.CurrentToken.Type == Token.LITERAL_DECIMAL) ||
				(tokenizer.CurrentToken.Type == Token.LITERAL_DOUBLE) ||
				(tokenizer.CurrentToken.Type == Token.LITERAL_FLOAT) ||
				(tokenizer.CurrentToken.Type == Token.LITERAL_INTEGER) ||
				(tokenizer.CurrentToken.Type == Token.LITERAL_STRING))
			{
				tokenizer.setBookmark();
				tokenizer.nextToken();
				PositionToken testToken = tokenizer.CurrentToken;
				tokenizer.cancelBookmark();
				if (testToken.Type == Token.COMMA)
				{
					ret = Literal.parse(tokenizer);
				}
			}
*/
			if (ret is MissingVariableInitializer)
			{
				ret = ArrayInitializer.parse(tokenizer);
				if (ret is MissingArrayInitializer)
				{
					ret = Expression.parse(tokenizer);
					if (ret is MissingExpression)
					{
						ret = new MissingVariableInitializer();
					}
				}
			}
			return ret;
		}
		
		public VariableInitializer()
		{
		}
		
		public VariableInitializer(params FundamentalPieceOfCode[] list) :
		base(list)
		{
		}
		
		public VariableInitializer(Position position) :
		base(position)
		{
		}
	}
}