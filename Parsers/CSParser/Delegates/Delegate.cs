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
	/// attributesopt delegate-modifiersopt delegate return-type identifier ( formal-parameter-listopt ) ; 
	/// </summary>
	public class Delegate : ModifyablePieceOfCodeWithAttributes, NamespaceMember, TypeDeclaration
	{
		public static TypeDeclaration parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			TypeDeclaration ret = new MissingTypeDeclaration();
			AttributeSectionCollection attributes = AttributeSectionCollection.parse(tokenizer);
			ModifierCollection modifiers = ModifierCollection.parse(tokenizer);
			if (Token.DELEGATE == tokenizer.CurrentToken.Type)
			{
				PositionToken delegateToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // delegate
				DataType type = DataType.parse(tokenizer);
				type.checkNotMissing();
				Expression exp = QualifiedIdentifier.parse(tokenizer);
				if (exp is QualifiedIdentifier)
				{
					QualifiedIdentifier identifier = exp as QualifiedIdentifier;
					PositionToken openParensToken = tokenizer.checkToken(Token.OPEN_PARENS);
					FormalParameterList parameters = FormalParameterList.parse(tokenizer);
					parameters.checkNotMissing();
					PositionToken closeParensToken = tokenizer.checkToken(Token.CLOSE_PARENS);
					PositionToken semicolonToken = tokenizer.checkToken(Token.SEMICOLON);
					ret = new Delegate(attributes, modifiers, delegateToken, type, identifier, openParensToken, parameters, closeParensToken, semicolonToken);
				}
			}
			tokenizer.endBookmark(ret is MissingTypeDeclaration);
			return ret;
		}

		public Delegate(AttributeSectionCollection attributes, ModifierCollection modifiers, 
						PositionToken delegateToken, DataType type, 
						QualifiedIdentifier identifier, PositionToken openParensToken, 
						FormalParameterList parameters, PositionToken closeParensToken,
						PositionToken semicolonToken) :
		base(attributes, modifiers, delegateToken, type, identifier, openParensToken, parameters, closeParensToken, semicolonToken)
		{
		}
		
		public PositionToken DelegateToken {
			get {
				return Pieces[2] as PositionToken;
			}
		}

		public DataType Type {
			get {
				return Pieces[3] as DataType;
			}
		}

		public QualifiedIdentifier Identifier {
			get {
				return Pieces[4] as QualifiedIdentifier;
			}
		}
		public string Name {
			get {
				return Identifier.Name;
			}
		}
		
		public PositionToken OpenParensToken {
			get {
				return Pieces[5] as PositionToken;
			}
		}

		public FormalParameterList Parameters {
			get {
				return Pieces[6] as FormalParameterList;
			}
		}
		
		public PositionToken CloseParensToken {
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
			return new MissingNamespaceMember();
		}

		public string FullyQualifiedParentNamespace {
			get {
				return "";
			}
		}

		public override string AsText {
			get {
				return Identifier.Name;
			}
		}
	}
}
