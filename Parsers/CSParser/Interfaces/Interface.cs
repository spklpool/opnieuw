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
	public class Interface : ModifyablePieceOfCodeWithAttributes, NamespaceMember, TypeDeclaration
	{
		/// <summary>
		/// attributesopt interface-modifiersopt interface identifier interface-baseopt interface-body ;opt 
		/// </summary>
		/// <param name="tokenizer"></param>
		/// <returns></returns>
		public static TypeDeclaration parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			TypeDeclaration ret = new MissingTypeDeclaration();
			AttributeSectionCollection attributes = AttributeSectionCollection.parse(tokenizer);
			ModifierCollection modifiers = ModifierCollection.parse(tokenizer);
			PositionToken interfaceToken = tokenizer.CurrentToken;
			tokenizer.nextToken();
			Expression name = QualifiedIdentifier.parse(tokenizer);
			if (name is QualifiedIdentifier)
			{
				BaseTypeList interfaceBase = BaseTypeList.parse(tokenizer);
				PositionToken openBraceToken = tokenizer.checkToken(Token.OPEN_BRACE);
				InterfaceMemberCollection members = InterfaceMemberCollection.parse(tokenizer);
				PositionToken closeBraceToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // }
				ret = new Interface(attributes, modifiers, interfaceToken, 
									name as QualifiedIdentifier, interfaceBase, 
									openBraceToken, members, closeBraceToken);
			}
			tokenizer.endBookmark(ret is MissingTypeDeclaration);
			return ret;
		}

		public Interface(AttributeSectionCollection attributes, ModifierCollection modifiers, 
						 PositionToken interfaceToken, QualifiedIdentifier identifier, 
						 BaseTypeList interfaceBase, PositionToken openBraceToken, 
						 InterfaceMemberCollection members, PositionToken closeBraceToken) :
		base(attributes, modifiers, interfaceToken, identifier, interfaceBase, openBraceToken, members, closeBraceToken)
		{
		}

		public Interface(String name, BaseTypeList interfaceBase,
						 InterfaceMemberCollection members) :
		base(new AttributeSectionCollection(),
			 new ModifierCollection(),
			 new PositionToken(Position.Missing, "interface", Token.INTERFACE),
			 QualifiedIdentifier.parse(" " + name),
			 interfaceBase,
			 new PositionToken(Position.Missing, "{", Token.OPEN_BRACE),
			 members,
			 new PositionToken(Position.Missing, "}", Token.CLOSE_BRACE))
		{
        }

        public Interface(Attribute attribute, String name, BaseTypeList interfaceBase,
                         InterfaceMemberCollection members) :
		base(new AttributeSectionCollection(),
			 new ModifierCollection(),
			 new PositionToken(Position.Missing, "interface", Token.INTERFACE),
			 QualifiedIdentifier.parse(name),
			 interfaceBase,
			 new PositionToken(Position.Missing, "{", Token.OPEN_BRACE),
			 members,
			 new PositionToken(Position.Missing, "}", Token.CLOSE_BRACE))
        {
            AttributeCollection attributes = new AttributeCollection();
            attributes.Add(attribute);
            AttributeSection section = new AttributeSection(attributes);
            AttributeSections.Add(section);
        }

        public override void Format()
        {
            base.Format();
            string membersBlock = Members.Generate();
            string newMembersBlock = GenericBlockOfCode.Reindent(membersBlock, 4, 4);
            InterfaceMemberCollection newMembers = InterfaceMemberCollection.parse(newMembersBlock);
            Pieces[6] = newMembers;
        }

        public PositionToken InterfaceToken {
			get {
				return Pieces[2] as PositionToken;
			}
		}

		public QualifiedIdentifier Identifier {
			get {
				return Pieces[3] as QualifiedIdentifier;
			}
		}

		protected string m_Name;
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

		public InterfaceMemberCollection Members {
			get {
				return Pieces[6] as InterfaceMemberCollection;
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
		
		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Members.Children);
				return ret;
			}
		}

		public override string AsText {
			get {
				return Name;
			}
		}
	}
}
