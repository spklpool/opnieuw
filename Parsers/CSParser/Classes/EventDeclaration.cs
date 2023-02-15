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
	public class EventDeclaration : ModifyablePieceOfCodeWithAttributes, ClassMember, StructMember
	{
		/// <summary>
		/// attributesopt event-modifiersopt event type variable-declarators ;
		/// attributesopt event-modifiersopt event type member-name { event-accessor-declarations } 
		/// </summary>
		public static ClassMember parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			ClassMember ret = new MissingClassMember();
			AttributeSectionCollection attributes = AttributeSectionCollection.parse(tokenizer);
			ModifierCollection modifiers = ModifierCollection.parse(tokenizer);
			if (Token.EVENT == tokenizer.CurrentToken.Type)
			{
				PositionToken eventToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // event
				DataType type = DataType.parse(tokenizer);
				type.checkNotMissing();
				tokenizer.setBookmark();
				Expression exp = QualifiedIdentifier.parse(tokenizer);
				if (exp is QualifiedIdentifier)
				{
					QualifiedIdentifier identifier = exp as QualifiedIdentifier;
					if (Token.OPEN_BRACE == tokenizer.CurrentToken.Type)
					{
						PositionToken openBraceToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // {
						EventAccessorDeclarations eventAccessors = EventAccessorDeclarations.parse(tokenizer);
						PositionToken closeBraceToken = tokenizer.checkToken(Token.CLOSE_BRACE);
						ret = new EventDeclaration(attributes, modifiers, eventToken, type, identifier, openBraceToken, eventAccessors, closeBraceToken);
						tokenizer.cancelBookmark();
					}
					else
					{
						tokenizer.returnToBookmark();
					}
				}
				else
				{
					tokenizer.returnToBookmark();
				}
				if (ret is MissingClassMember)
				{
					VariableDeclaratorCollection variableDeclarators = VariableDeclaratorCollection.parse(tokenizer);
					PositionToken semicolonToken = tokenizer.checkToken(Token.SEMICOLON);
					ret = new EventDeclaration(attributes, modifiers, eventToken, type, variableDeclarators, semicolonToken);
				}
			}
			tokenizer.endBookmark(ret is MissingClassMember);
			return ret;
		}

		public EventDeclaration()
		{
			Pieces.Add(new AttributeSectionCollection());
			Pieces.Add(new ModifierCollection());
			Pieces.Add(PositionToken.Missing);
			Pieces.Add(new MissingDataType());
			Pieces.Add(new MissingQualifiedIdentifier());
			Pieces.Add(PositionToken.Missing);
			Pieces.Add(new MissingEventAccessorDeclarations());
			Pieces.Add(PositionToken.Missing);
			Pieces.Add(new VariableDeclaratorCollection());
			Pieces.Add(PositionToken.Missing);
		}

		public EventDeclaration(AttributeSectionCollection attributes, ModifierCollection modifiers, PositionToken eventToken, DataType type, QualifiedIdentifier identifier, PositionToken openBraceToken, EventAccessorDeclarations accessors, PositionToken closeBraceToken) :
		base(attributes, modifiers, eventToken, type, identifier, openBraceToken, accessors, closeBraceToken, new VariableDeclaratorCollection(), PositionToken.Missing)
		{
		}

		public EventDeclaration(AttributeSectionCollection attributes, ModifierCollection modifiers, PositionToken eventToken, DataType type, VariableDeclaratorCollection variableDeclarators, PositionToken semicolonToken) :
		base(attributes, modifiers, eventToken, type, new MissingQualifiedIdentifier(), PositionToken.Missing, new MissingEventAccessorDeclarations(), PositionToken.Missing, variableDeclarators, semicolonToken)
		{
		}
		
		public PositionToken EventToken {
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
		
		public PositionToken OpenBraceToken {
			get {
				return Pieces[5] as PositionToken;
			}
		}

		public EventAccessorDeclarations Accessors {
			get {
				return Pieces[6] as EventAccessorDeclarations;
			}
		}
		
		public PositionToken CloseBraceToken {
			get {
				return Pieces[7] as PositionToken;
			}
		}

		public VariableDeclaratorCollection Declarators {
			get {
				return Pieces[8] as VariableDeclaratorCollection;
			}
		}
		
		public PositionToken SemicolonToken {
			get {
				return Pieces[9] as PositionToken;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				if (false == (Accessors is MissingEventAccessorDeclarations))
				{
					ret.Add(Accessors);
				}
				ret.Add(Declarators.Children);
				return ret;
			}
		}

		public override string AsText {
			get {
				return Identifier.Name;
			}
		}
	}
}
