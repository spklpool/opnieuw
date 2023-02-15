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
	public class StructDeclaration : ModifyablePieceOfCodeWithAttributes, NamespaceMember, TypeDeclaration
	{
		/// <summary>
		/// attributesopt struct-modifiersopt struct identifier struct-interfacesopt struct-body ;opt 
		/// </summary>
		/// <param name="tokenizer"></param>
		/// <returns></returns>
		public static TypeDeclaration parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			TypeDeclaration ret = new MissingTypeDeclaration();
			AttributeSectionCollection attributes = AttributeSectionCollection.parse(tokenizer);
			ModifierCollection modifiers = ModifierCollection.parse(tokenizer);
			PositionToken structToken = tokenizer.CurrentToken;
			if (tokenizer.CurrentToken.Type == Token.STRUCT)
			{
				tokenizer.nextToken();
				Identifier name = Identifier.parse(tokenizer);
				name.checkNotMissing();
				PositionToken potentialColonToken = tokenizer.CurrentToken;
				BaseTypeList structBase = BaseTypeList.parse(tokenizer);
				PositionToken openBraceToken = tokenizer.checkToken(Token.OPEN_BRACE);
				StructMemberCollection members = StructMemberCollection.parse(tokenizer);
				Position position = new Position(attributes, modifiers, structToken, tokenizer.CurrentToken);
				PositionToken closeBraceToken = tokenizer.checkToken(Token.CLOSE_BRACE);
				if (tokenizer.CurrentToken.Type == Token.SEMICOLON)
				{
					position = new Position(attributes, modifiers, structToken, tokenizer.CurrentToken);
					tokenizer.nextToken(); // ;
				}
				ret = new StructDeclaration(attributes, modifiers, structToken, name, 
											structBase, openBraceToken, 
											members, closeBraceToken, position);
			}
			tokenizer.endBookmark(ret is MissingTypeDeclaration);
			return ret;
		}

		public StructDeclaration(AttributeSectionCollection attributes, ModifierCollection modifiers, 
					  			 PositionToken structToken, Identifier identifier, BaseTypeList interfaces, 
					  			 PositionToken openBraceToken, StructMemberCollection members, 
					  			 PositionToken closeBraceToken, Position position) :
		base(attributes, modifiers, structToken, identifier, interfaces, openBraceToken, members, closeBraceToken)
		{
		}

		public PositionToken StructToken {
			get {
				return Pieces[2] as PositionToken;
			}
		}

		public Identifier Identifier {
			get {
				return Pieces[3] as Identifier;
			}
		}

		public string Name {
			get {
				return Identifier.Name;
			}
		}

		public BaseTypeList Base {
			get {
				return Pieces[4] as BaseTypeList;
			}
		}

		public PositionToken OpenBraceToken {
			get {
				return Pieces[5] as PositionToken;
			}
		}

		public StructMemberCollection Members {
			get {
				return Pieces[6] as StructMemberCollection;
			}
		}

		public PositionToken CloseBraceToken {
			get {
				return Pieces[7] as PositionToken;
			}
		}

		public NamespaceMember LookupType(string name)
		{
			NamespaceMember ret = null;
			foreach (NamespaceMember member in Members)
			{
				ret = member.LookupType(name);
				if (null != ret)
				{
					break;
				}
			}
			return ret;
		}

		public string FullyQualifiedParentNamespace {
			get {
				return "";
			}
		}
	}
}
