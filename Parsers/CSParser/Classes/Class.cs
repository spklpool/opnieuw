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
	public class Class : ModifyablePieceOfCodeWithAttributes, TypeDeclaration
    {
#region static parsing code
        /// <summary>
        /// attributesopt class-modifiersopt class identifier class-baseopt class-body ;opt 
		/// </summary>
		/// <param name="tokenizer"></param>
		/// <returns></returns>
		public static TypeDeclaration parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			TypeDeclaration ret = new MissingTypeDeclaration();
			AttributeSectionCollection attributes = AttributeSectionCollection.parse(tokenizer);
			ModifierCollection modifiers = ModifierCollection.parse(tokenizer);
			if (Token.CLASS == tokenizer.CurrentToken.Type)
			{
				PositionToken classToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // class
				Identifier name = Identifier.parse(tokenizer);
				name.checkNotMissing();
				BaseTypeList classBase = BaseTypeList.parse(tokenizer);
				PositionToken openBraceToken = tokenizer.checkToken(Token.OPEN_BRACE);
				ClassMemberCollection members = ClassMemberCollection.parse(tokenizer);
				PositionToken closeBraceToken = tokenizer.checkToken(Token.CLOSE_BRACE);
                PositionToken semicolonToken = PositionToken.Missing;
                if (tokenizer.CurrentToken.Type == Token.SEMICOLON) {
                    semicolonToken = tokenizer.CurrentToken;
                }
                ret = new Class(attributes, modifiers, classToken, name, classBase, 
                                openBraceToken, members, closeBraceToken, semicolonToken);
				Class classRet = ret as Class;
                classRet.PropagateUp();
			}
			tokenizer.endBookmark(ret is MissingTypeDeclaration);
			return ret;
        }
#endregion

#region constructors
        public Class(String name, ClassMemberCollection members) :
		base(new AttributeSectionCollection(), new ModifierCollection(), 
             new PositionToken(new InvalidPosition(), "class", Token.CLASS), 
             Identifier.parse(name), 
             new MissingBaseTypeList(), 
             new PositionToken(new InvalidPosition(), "{", Token.OPEN_BRACE), 
             members, 
             new PositionToken(new InvalidPosition(), "}", Token.CLOSE_BRACE),
             PositionToken.Missing)
        {
        }

        public Class(String name) :
		base(new AttributeSectionCollection(), new ModifierCollection(), 
             new PositionToken(new InvalidPosition(), "class", Token.CLASS), 
             Identifier.parse(name), 
             new MissingBaseTypeList(), 
             new PositionToken(new InvalidPosition(), "{", Token.OPEN_BRACE), 
             new ClassMemberCollection(), 
             new PositionToken(new InvalidPosition(), "}", Token.CLOSE_BRACE),
             PositionToken.Missing)
        {
        }

        public Class(AttributeSectionCollection attributes, ModifierCollection modifiers, 
					 PositionToken classToken, Identifier identifier, BaseTypeList classBase, 
					 PositionToken openBraceToken, ClassMemberCollection members, 
					 PositionToken closeBraceToken, PositionToken semicolonToken) :
		base(attributes, modifiers, classToken, identifier, classBase, openBraceToken, members, 
             closeBraceToken, semicolonToken)
		{
        }
#endregion

        public override void Format()
        {
            base.Format();
            string membersBlock = Members.Generate();
            string newMembersBlock = GenericBlockOfCode.Reindent(membersBlock, 4, 4);
            ClassMemberCollection newMembers = ClassMemberCollection.parse(newMembersBlock);
            Pieces[6] = newMembers;
        }

        public PositionToken ClassToken {
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
			set {
				Pieces[4] = value;
			}
		}

		public PositionToken OpenBraceToken {
			get {
				return Pieces[5] as PositionToken;
			}
		}

		public ClassMemberCollection Members {
			get {
				return Pieces[6] as ClassMemberCollection;
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

		public ConstructorDeclaration Constructor {
			get {
				ConstructorDeclaration ret = new MissingConstructorDeclaration();
				foreach (ClassMember member in Members)
				{
					if (member is ConstructorDeclaration)
					{
						ret = member as ConstructorDeclaration;
						break;
					}
				}
				return ret;
			}
		}

		public DestructorDeclaration Destructor {
			get {
				DestructorDeclaration ret = new MissingDestructorDeclaration();
				foreach (ClassMember member in Members)
				{
					if (member is DestructorDeclaration)
					{
						ret = member as DestructorDeclaration;
						break;
					}
				}
				return ret;
			}
		}

		public MethodDeclarationCollection Methods {
			get {
				MethodDeclarationCollection ret = new MethodDeclarationCollection();
				foreach (ClassMember member in Members)
				{
					if (member is MethodDeclaration)
					{
						ret.Add(member as MethodDeclaration);
					}
				}
				return ret;
			}
		}

		public EventDeclarationCollection Events {
			get {
				EventDeclarationCollection ret = new EventDeclarationCollection();
				foreach (ClassMember member in Members)
				{
					if (member is EventDeclaration)
					{
						ret.Add(member as EventDeclaration);
					}
				}
				return ret;
			}
		}

		public OperatorDeclarationCollection Operators {
			get {
				OperatorDeclarationCollection ret = new OperatorDeclarationCollection();
				foreach (ClassMember member in Members)
				{
					if (member is OperatorDeclaration)
					{
						ret.Add(member as OperatorDeclaration);
					}
				}
				return ret;
			}
		}

		public PropertyDeclarationCollection Properties {
			get {
				PropertyDeclarationCollection ret = new PropertyDeclarationCollection();
				foreach (ClassMember member in Members)
				{
					if (member is PropertyDeclaration)
					{
						ret.Add(member as PropertyDeclaration);
					}
				}
				return ret;
			}
		}

		public IndexerDeclarationCollection Indexers {
			get {
				IndexerDeclarationCollection ret = new IndexerDeclarationCollection();
				foreach (ClassMember member in Members)
				{
					if (member is IndexerDeclaration)
					{
						ret.Add(member as IndexerDeclaration);
					}
				}
				return ret;
			}
		}

		public ConstantDeclarationCollection Constants {
			get {
				ConstantDeclarationCollection ret = new ConstantDeclarationCollection();
				foreach (ClassMember member in Members)
				{
					if (member is ConstantDeclaration)
					{
						ret.Add(member as ConstantDeclaration);
					}
				}
				return ret;
			}
		}

		public FieldDeclarationCollection Fields {
			get {
				FieldDeclarationCollection ret = new FieldDeclarationCollection();
				foreach (ClassMember member in Members)
				{
					if (member is FieldDeclaration)
					{
						ret.Add(member as FieldDeclaration);
					}
				}
				return ret;
			}
		}

		public ClassCollection Classes {
			get {
				ClassCollection ret = new ClassCollection();
				foreach (ClassMember member in Members)
				{
					if (member is Class)
					{
						ret.Add(member as Class);
					}
				}
				return ret;
			}
		}

		public InterfaceCollection Interfaces {
			get {
				InterfaceCollection ret = new InterfaceCollection();
				foreach (ClassMember member in Members)
				{
					if (member is Interface)
					{
						ret.Add(member as Interface);
					}
				}
				return ret;
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
				return Identifier.Name;
			}
		}
	}
}
