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
using System.Collections;
using Opnieuw.Framework;

namespace Opnieuw.Parsers.CSParser 
{
	public class Namespace : ModifyablePieceOfCodeWithAttributes, NamespaceMember
    {
#region static parsing code
        /// <summary>
        /// namespace qualified-identifier { using-directivesopt namespace-member-declarationsopt } ;opt 
		/// </summary>
		public static NamespaceMember parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			NamespaceMember ret = new MissingNamespaceMember();
			AttributeSectionCollection attributes = AttributeSectionCollection.parse(tokenizer);
			ModifierCollection modifiers = ModifierCollection.parse(tokenizer);
			if (Token.NAMESPACE == tokenizer.CurrentToken.Type)
			{
				PositionToken namespaceToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // skip nemespace token
				Expression exp = QualifiedIdentifier.parse(tokenizer);
				if (exp is QualifiedIdentifier)
				{
					QualifiedIdentifier identifier = exp as QualifiedIdentifier;
					if (Token.OPEN_BRACE == tokenizer.CurrentToken.Type)
					{
						PositionToken openBraceToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // {
						UsingDirectiveCollection usingDirectives = UsingDirectiveCollection.parse(tokenizer);
						NamespaceMemberCollection namespaceMembers = NamespaceMemberCollection.parse(tokenizer);
						if (Token.CLOSE_BRACE == tokenizer.CurrentToken.Type)
						{
							PositionToken closeBraceToken = tokenizer.CurrentToken;
							tokenizer.nextToken();
							ret = new Namespace(attributes, modifiers, namespaceToken, identifier, openBraceToken, usingDirectives, namespaceMembers, closeBraceToken);
						}
					}
				}
			}
			tokenizer.endBookmark(ret is MissingNamespaceMember);
			return ret;
		}

		public static NamespaceMember parseMember(TokenProvider tokenizer)
		{
			NamespaceMember ret = new MissingNamespaceMember();
			//First parse the attributes and modifiers to get to the token
			//that identifies what type of member we are looking at, if any.
			//After that, return to the starting point and let the specific 
			//classes parse the attributes and modifiers again.  This could be
			//optimized later if necessary, but I think it is more consistent
			//this way because we do not need to pass in the pre-parsed sections.
			tokenizer.setBookmark();
			ret = Namespace.parse(tokenizer);
			if (ret is MissingNamespaceMember)
			{
				ret = parseType(tokenizer);
			}
			tokenizer.endBookmark((ret is MissingNamespaceMember) ||
								  (ret is MissingTypeDeclaration));
			return ret;
		}

		public static TypeDeclaration parseType(TokenProvider tokenizer)
		{
			TypeDeclaration ret = new MissingTypeDeclaration();
			BookmarkKeeper bk = new BookmarkKeeper(tokenizer);
			AttributeSectionCollection attributes = AttributeSectionCollection.parse(tokenizer);
			ModifierCollection modifiers = ModifierCollection.parse(tokenizer);
			PositionToken tempToken = tokenizer.CurrentToken;
			bk.returnToBookmark();
			switch (tempToken.Type)
			{
				case Token.CLASS:
					ret = Class.parse(tokenizer);
					break;
				case Token.STRUCT:
					ret = StructDeclaration.parse(tokenizer);
					break;
				case Token.INTERFACE:
					ret = Interface.parse(tokenizer);
					break;
				case Token.ENUM:
					ret = EnumDeclaration.parse(tokenizer);
					break;
				case Token.DELEGATE:
					ret = Delegate.parse(tokenizer);
					break;
			}
			if (ret is MissingTypeDeclaration)
			{
				bk.returnToBookmark();
			}
			bk.cancelBookmark();
			return ret;
        }
#endregion

#region constructors
        public Namespace() :
		base(new AttributeSectionCollection(), new ModifierCollection(), PositionToken.Missing,
			 new MissingQualifiedIdentifier(), PositionToken.Missing, new UsingDirectiveCollection(), 
			 new NamespaceMemberCollection(), PositionToken.Missing)
		{
		}

		public Namespace(AttributeSectionCollection attributes, ModifierCollection modifiers, 
						 PositionToken namespaceToken, QualifiedIdentifier identifier, 
						 PositionToken openBraceToken, UsingDirectiveCollection usingDirectives, 
						 NamespaceMemberCollection namespaceMembers, PositionToken closeBraceToken) :
		base(attributes, modifiers, namespaceToken, identifier, openBraceToken, usingDirectives, 
			 namespaceMembers, closeBraceToken)
		{
		}

		public Namespace(QualifiedIdentifier identifier) :
		base(new AttributeSectionCollection(), new ModifierCollection(), PositionToken.Missing,
			 identifier, PositionToken.Missing, new UsingDirectiveCollection(), 
			 new NamespaceMemberCollection(), PositionToken.Missing)
		{
        }

		public Namespace(QualifiedIdentifier identifier, NamespaceMemberCollection members) :
		base(new AttributeSectionCollection(), 
			 new ModifierCollection(), 
			 new PositionToken(Position.Missing, "namespace", Token.NAMESPACE),
			 identifier, 
			 new PositionToken(Position.Missing, "{", Token.OPEN_BRACE), 
			 new UsingDirectiveCollection(), 
			 members, 
			 new PositionToken(Position.Missing, "}", Token.CLOSE_BRACE))
		{
        }
#endregion

        public override void Format()
        {
            base.Format();
            string membersBlock = Members.Generate();
            string newMembersBlock = GenericBlockOfCode.Reindent(membersBlock, 4, 4);
            NamespaceMemberCollection newMembers = NamespaceMemberCollection.parse(newMembersBlock);
            Pieces[6] = newMembers;
        }

        public PositionToken NamespaceToken {
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

		public PositionToken OpenBraceToken {
			get {
				return Pieces[4] as PositionToken;
			}
		}

		public UsingDirectiveCollection UsingDirectives {
			get {
				return Pieces[5] as UsingDirectiveCollection;
			}
		}

		public NamespaceMemberCollection Members {
			get {
				return Pieces[6] as NamespaceMemberCollection;
			}
		}
		
		public PositionToken CloseBraceToken {
			get {
				return Pieces[7] as PositionToken;
			}
		}

		public IdentifierCollection FullyQualifiedNameIdentifiers {
			get {
				IdentifierCollection ret = new IdentifierCollection();
				if (!(this.ParentNamespace is MissingNamespace))
				{
					Namespace parent = this.ParentNamespace;
					ret.Add(parent.Identifier);
				}
				ret.Add(Identifier);
				return ret;
			}
		}

		public string FullyQualifiedName {
			get {
				string ret = Name;
				if (!(this.ParentNamespace is MissingNamespace))
				{
					Namespace parent = this.ParentNamespace;
					ret = ret.Insert(0, parent.FullyQualifiedName + '.');
				}
				return ret;
			}
		}

		public string FullyQualifiedParentNamespace {
			get {
				string ret = "";
				if (FullyQualifiedName.LastIndexOf('.') != -1)
				{
					ret = FullyQualifiedName.Substring(0, FullyQualifiedName.LastIndexOf('.'));
				}
				return ret;
			}
		}

		protected NamespaceCollection m_Contributors = new NamespaceCollection();
		public NamespaceCollection Contributors {
			get {
				return m_Contributors;
			}
			set {
				m_Contributors = value;
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

		public NamespaceCollection Namespaces {
			get {
				return Members.Namespaces;
			}
		}

 		public ClassCollection Classes {
			get {
				return Members.Classes;
			}
		}

 		public InterfaceCollection Interfaces {
			get {
				return Members.Interfaces;
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

	public class MissingNamespace : Namespace
	{
	}
}

