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
	public class AddAccessorDeclaration : EventAccessorDeclaration
	{
		public static AddAccessorDeclaration parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			AddAccessorDeclaration ret = new MissingAddAccessorDeclaration();
			AttributeSectionCollection attributes = AttributeSectionCollection.parse(tokenizer);
			if (Token.ADD == tokenizer.CurrentToken.Type)
			{
				PositionToken addToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // add
				Statement body = BlockStatement.parse(tokenizer);
				body.checkNotMissing();
				ret = new AddAccessorDeclaration(attributes, addToken, body);
			}
			tokenizer.endBookmark(ret is MissingAddAccessorDeclaration);
			return ret;
		}

		public AddAccessorDeclaration(AttributeSectionCollection attributes, PositionToken addToken, Statement body) :
		base(attributes, addToken, body)
		{
		}
		
		protected static AddAccessorDeclaration m_Missing = new MissingAddAccessorDeclaration();
		public static AddAccessorDeclaration Missing {
			get {
				return m_Missing;
			}
		}
	}

	public class MissingAddAccessorDeclaration : AddAccessorDeclaration
	{
		public MissingAddAccessorDeclaration() :
		base(new AttributeSectionCollection(), PositionToken.Missing, new MissingStatement())
		{
		}
	}
}
