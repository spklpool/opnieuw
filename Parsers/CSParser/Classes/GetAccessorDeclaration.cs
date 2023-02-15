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
	public class GetAccessorDeclaration : AccessorDeclaration
	{
		public static AccessorDeclaration parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			AccessorDeclaration ret = AccessorDeclaration.Missing;
			AttributeSectionCollection attributes = AttributeSectionCollection.parse(tokenizer);
			if (Token.GET == tokenizer.CurrentToken.Type)
			{
				PositionToken getToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // get
				Statement statement = Statement.parse(tokenizer);
				statement.checkNotMissing();
				ret = new GetAccessorDeclaration(attributes, getToken, statement);
			}
			tokenizer.endBookmark(ret is MissingAccessorDeclaration);
			return ret;
		}

		public GetAccessorDeclaration(AttributeSectionCollection attributes, PositionToken getToken, Statement statement) :
		base(attributes, getToken, statement)
		{
        }

        public GetAccessorDeclaration(AttributeSectionCollection attributes, Statement statement) :
		base(attributes, new PositionToken(Position.Missing, "get", Token.GET), statement)
        {
        }

        public override string AsText {
			get {
				return "get";
			}
		}
	}
}
