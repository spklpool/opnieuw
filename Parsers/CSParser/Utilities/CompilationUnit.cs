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
using System.IO;
using Opnieuw.Framework;

namespace Opnieuw.Parsers.CSParser
{
	/// <summary>
	/// A compilation-unit defines the overall structure of a source file. A compilation unit 
	/// consists of zero or more using-directives followed by zero or more attributes followed 
	/// by zero or more namespace-member-declarations. 
	///   compilation-unit: 
	///     using-directivesopt attributesopt namespace-member-declarationsopt 
	/// </summary>
	public class CompilationUnit : PieceOfCodeWithAttributes
	{
		/// <summary>
		/// Factory to create instances of this class.
		/// </summary>
		/// <param name="tokenizer"></param>
		/// <returns></returns>
		public static CompilationUnit Parse(TokenProvider tokenizer)
		{
			CompilationUnit ret = new MissingCompilationUnit();
			try
			{
				tokenizer.nextToken();
				UsingDirectiveCollection usingDirectives = UsingDirectiveCollection.parse(tokenizer);
				AttributeSectionCollection attributes = AttributeSectionCollection.parse(tokenizer);
				NamespaceMemberCollection namespaceMembers = NamespaceMemberCollection.parse(tokenizer);
				ret = new CompilationUnit(4, usingDirectives, attributes, namespaceMembers);
                ret.PropagateUp();
			} 
			catch (ParserException e) 
			{
				Token tokenLookup = new Token();
				string s = "Parsing error: " + e.Text;
				if (e.Expected != Token.NONE)
				{
					s += "Expected: " + tokenLookup.lookupToken(e.Expected) + " at line: " + 
							   e.Actual.Position.StartLine + " col: " + e.Actual.Position.StartCol;
				}
			} 
			return ret;
		}
		
		public static CompilationUnit parse(string name)
		{
			Tokenizer t = new Tokenizer(new StringReader(name), "", null);
            return CompilationUnit.Parse(t) as CompilationUnit;
		}

		public static ClassMember parseClassMember(TokenProvider tokenizer)
		{
			ClassMember ret = new MissingClassMember();
			BookmarkKeeper bk = new BookmarkKeeper(tokenizer);
			ret = ConstantDeclaration.parse(tokenizer);
			if (ret is MissingClassMember)
			{
				ret = FieldDeclaration.parse(tokenizer);
				if (ret is MissingClassMember)
				{
					ret = DestructorDeclaration.parse(tokenizer);
					if (ret is MissingClassMember)
					{
						ret = ConstructorDeclaration.parse(tokenizer);
						if (ret is MissingClassMember)
						{
							ret = MethodDeclaration.parse(tokenizer);
							if (ret is MissingClassMember)
							{
								ret = PropertyDeclaration.parse(tokenizer);
								if (ret is MissingClassMember)
								{
									ret = OperatorDeclaration.parse(tokenizer);
									if (ret is MissingClassMember)
									{
										ret = EventDeclaration.parse(tokenizer);
										if (ret is MissingClassMember)
										{
											ret = IndexerDeclaration.parse(tokenizer);
											if (ret is MissingClassMember)
											{
												ret = Namespace.parseType(tokenizer);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			if ((ret is MissingClassMember) ||
				(ret is MissingTypeDeclaration))
			{
				ret = new MissingClassMember();
                bk.returnToBookmark();
			}
			bk.cancelBookmark();
			return ret;
		}
		
		public static ClassMember parseClassMember(string name)
		{
			Tokenizer t = new Tokenizer(new StringReader(name), "", null);
            t.nextToken();
            return CompilationUnit.parseClassMember(t) as ClassMember;
		}

		public static StructMember parseStructMember(TokenProvider tokenizer)
		{
			ClassMember ret = new MissingStructMember();
			BookmarkKeeper bk = new BookmarkKeeper(tokenizer);
			ret = ConstantDeclaration.parse(tokenizer);
			if (ret is MissingClassMember)
			{
				ret = FieldDeclaration.parse(tokenizer);
				if (ret is MissingClassMember)
				{
					ret = ConstructorDeclaration.parse(tokenizer);
					if (ret is MissingClassMember)
					{
						ret = MethodDeclaration.parse(tokenizer);
						if (ret is MissingClassMember)
						{
							ret = PropertyDeclaration.parse(tokenizer);
							if (ret is MissingClassMember)
							{
								ret = OperatorDeclaration.parse(tokenizer);
								if (ret is MissingClassMember)
								{
									ret = EventDeclaration.parse(tokenizer);
									if (ret is MissingClassMember)
									{
										ret = IndexerDeclaration.parse(tokenizer);
										if (ret is MissingClassMember)
										{
											ret = Namespace.parseType(tokenizer);
										}
									}
								}
							}
						}
					}
				}
			}
			if ((ret is MissingStructMember) ||
				(ret is MissingTypeDeclaration) ||
				(false == (ret is StructMember)))
			{
				ret = new MissingStructMember();
				bk.returnToBookmark();
			}
			bk.cancelBookmark();
			return ret as StructMember;
		}

		public CompilationUnit()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="tabWidth"></param>
		/// <param name="usingDirectives"></param>
		public CompilationUnit(int tabWidth, 
							   UsingDirectiveCollection usingDirectives, 
							   AttributeSectionCollection attributes, 
							   NamespaceMemberCollection namespaceMembers) :
		base(attributes, usingDirectives, namespaceMembers)
		{
			m_TabWidth = tabWidth;
		}

		/// <summary>
		/// The number of spaces in a tab.  This is used to convert tabs
		/// into a number of characters as it would be seen in a code
		/// editor.
		/// </summary>
		protected int m_TabWidth = 4;
		public int TabWidth {
			get {
				return m_TabWidth;
			}
		}

		/// <summary>
		/// The using directives in this compilation unit.
		/// </summary>
		public UsingDirectiveCollection UsingDirectives {
			get {
				return Pieces[1] as UsingDirectiveCollection;
			}
		}

		/// <summary>
		/// The namespace members in this compilation unit.
		/// </summary>
		public NamespaceMemberCollection Members {
			get {
				return Pieces[2] as NamespaceMemberCollection;
			}
		}

        public FundamentalPieceOfCode FromTrailOfBreadCrumbs(TrailOfBreadCrumbs crumbs)
        {
            FundamentalPieceOfCode ret = this;
            for (int i=(crumbs.Crumbs.Count-1); i>=0; i--) {
                int crumb = (int)crumbs.Crumbs[i];
                ret = ret.Pieces[crumb];
            }
            return ret;
        }

		/// <summary>
		/// Returns only the namespace members that are namespaces.
		/// </summary>
		public NamespaceCollection Namespaces {
			get {
				return Members.Namespaces;
			}
		}

		/// <summary>
		/// Looks for the specified type in this compilation unit
		/// and returns the corresponding NamespaceMember if it is
		/// found.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
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

		/// <summary>
		/// The path to the source file parsed to create this compilation unit.
		/// </summary>
		protected string m_SourceFilePath = "";
		public string SourceFilePath {
			get {
				return m_SourceFilePath;
			}
			set {
				m_SourceFilePath = value;
			}
		}
	}

	public class MissingCompilationUnit : CompilationUnit
	{
	}
}
