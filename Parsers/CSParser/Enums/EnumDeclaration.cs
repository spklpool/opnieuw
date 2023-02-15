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
	/// attributesopt enum-modifiersopt enum identifier enum-baseopt enum-body ;opt 
	/// </summary>
	public class EnumDeclaration : ModifyablePieceOfCodeWithAttributes, NamespaceMember, TypeDeclaration
	{
		public static TypeDeclaration parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			TypeDeclaration ret = new MissingTypeDeclaration();
			AttributeSectionCollection attributes = AttributeSectionCollection.parse(tokenizer);
			ModifierCollection modifiers = ModifierCollection.parse(tokenizer);
			if (Token.ENUM == tokenizer.CurrentToken.Type)
			{
				PositionToken enumToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // enum
				Expression exp = QualifiedIdentifier.parse(tokenizer);
				if (exp is QualifiedIdentifier)
				{
					QualifiedIdentifier identifier = exp as QualifiedIdentifier;
					EnumBase enumBase = EnumBase.parse(tokenizer);
					PositionToken openBraceToken = tokenizer.checkToken(Token.OPEN_BRACE);
					EnumMemberCollection members = EnumMemberCollection.parse(tokenizer);
					PositionToken closeBraceToken = tokenizer.checkToken(Token.CLOSE_BRACE);
					PositionToken semicolonToken = PositionToken.Missing;
					if (Token.SEMICOLON ==  tokenizer.CurrentToken.Type)
					{
						semicolonToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // ;
					}
					ret = new EnumDeclaration(attributes, modifiers, enumToken, identifier, enumBase, openBraceToken, members, closeBraceToken, semicolonToken);
				}
			}
			tokenizer.endBookmark(ret is MissingTypeDeclaration);
			return ret;
		}

		public EnumDeclaration()
		{
			Pieces.Add(new AttributeSectionCollection());
			Pieces.Add(new ModifierCollection());
			Pieces.Add(PositionToken.Missing);
			Pieces.Add(new MissingIdentifier());
			Pieces.Add(new MissingEnumBase());
			Pieces.Add(PositionToken.Missing);
			Pieces.Add(new EnumMemberCollection());
			Pieces.Add(PositionToken.Missing);
			Pieces.Add(PositionToken.Missing);
		}

		public EnumDeclaration(AttributeSectionCollection attributes, ModifierCollection modifiers, PositionToken enumToken, 
					QualifiedIdentifier identifier, EnumBase enumBase, PositionToken openBraceToken, 
					EnumMemberCollection members, PositionToken closeBraceToken, PositionToken semicolonToken) :
		base(attributes, modifiers, enumToken, identifier, enumBase, openBraceToken, members, closeBraceToken, semicolonToken)
		{
		}
		
		public PositionToken EnumToken {
			get {
				return Pieces[2] as PositionToken;
			}
		}

		public QualifiedIdentifier Identifier {
			get {
				return Pieces[3] as QualifiedIdentifier;
			}
		}
		public string Name {
			get {
				return Identifier.Name;
			}
		}

		public EnumBase Base {
			get {
				return Pieces[4] as EnumBase;
			}
		}
		
		public PositionToken OpenBraceToken {
			get {
				return Pieces[5] as PositionToken;
			}
		}

		public EnumMemberCollection Members {
			get {
				return Pieces[6] as EnumMemberCollection;
			}
		}
		
		public PositionToken CloseBraceToken {
			get {
				return Pieces[7] as PositionToken;
			}
		}
		
		public PositionToken SemicolonToken {
			get {
				return Pieces[8] as PositionToken;
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

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Base);
				ret.Add(Members);
				return ret;
			}
		}
	}
}
