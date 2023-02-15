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
	public class RemoveAccessorDeclaration : EventAccessorDeclaration
	{
		public static RemoveAccessorDeclaration parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			RemoveAccessorDeclaration ret = new MissingRemoveAccessorDeclaration();
			AttributeSectionCollection removeAttributes = AttributeSectionCollection.parse(tokenizer);
			if (Token.REMOVE == tokenizer.CurrentToken.Type)
			{
				PositionToken removeToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // remove
				Statement body = BlockStatement.parse(tokenizer);
				body.checkNotMissing();
				ret = new RemoveAccessorDeclaration(removeAttributes, removeToken, body);
			}
			tokenizer.endBookmark(ret is MissingRemoveAccessorDeclaration);
			return ret;
		}

		public RemoveAccessorDeclaration()
		{
		}

		public RemoveAccessorDeclaration(AttributeSectionCollection attributes, PositionToken removeToken, Statement body) :
		base(attributes, removeToken, body)
		{
		}
		
		private static RemoveAccessorDeclaration m_Missing = new MissingRemoveAccessorDeclaration();
		public static RemoveAccessorDeclaration Missing {
			get {
				return m_Missing;
			}
		}
	}

	public class MissingRemoveAccessorDeclaration : RemoveAccessorDeclaration
	{
	}
}
