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
	/// Using directives facilitate the use of namespaces and types defined in other 
	/// namespaces. Using directives impact the name resolution process of 
	/// namespace-or-type-names and simple-names , but unlike declarations, using 
	/// directives do not contribute new members to the underlying declaration spaces
	/// of the compilation units or namespaces within which they are used. 
	/// 
	/// A using directive can be one of the following:
	///	using identifier = namespace-or-type-name ;
	/// using namespace-name ;
	/// </summary>
	public class UsingDirective : PieceOfCode
	{
		public static UsingDirective Parse(TokenProvider tokenizer)
		{
			UsingDirective ret = new MissingUsingDirective();
			if (Token.USING == tokenizer.CurrentToken.Type)
			{
				PositionToken usingToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // using
				Expression identifier1 = QualifiedIdentifier.parse(tokenizer);
				if (identifier1 is QualifiedIdentifier)
				{
					if (Token.SEMICOLON == tokenizer.CurrentToken.Type)
					{
						PositionToken semicolonToken = tokenizer.CurrentToken;
						Position position = new Position(usingToken, tokenizer.CurrentToken);
						ret = new UsingNamespaceDirective(usingToken, identifier1 as QualifiedIdentifier, semicolonToken);
					}
					if (Token.ASSIGN == tokenizer.CurrentToken.Type)
					{
						//It must be a UsingAliasDirective
						PositionToken assignToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // =
						QualifiedIdentifier identifier = identifier1 as QualifiedIdentifier;
						Expression namespaceOrTypeName = QualifiedIdentifier.parse(tokenizer);
						PositionToken semicolonToken = tokenizer.CurrentToken;
						ret = new UsingAliasDirective(usingToken, identifier, assignToken, namespaceOrTypeName as QualifiedIdentifier, semicolonToken);
					}
					tokenizer.nextToken();
				}
			}
			tokenizer.endBookmark(ret is MissingUsingDirective);
			return ret;
		}

		public UsingDirective() :
		base(PositionToken.Missing, new MissingQualifiedIdentifier(), 
			 PositionToken.Missing, new MissingQualifiedIdentifier(), 
			 PositionToken.Missing)
		{
		}

		public UsingDirective(PositionToken usingToken, QualifiedIdentifier identifier, 
							  PositionToken assignToken, QualifiedIdentifier namespaceOrTypeName, 
							  PositionToken semicolonToken) :
		base(usingToken, identifier, assignToken, namespaceOrTypeName, semicolonToken)
		{
		}

		public PositionToken UsingToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}

		public QualifiedIdentifier NamespaceOrTypeName {
			get {
				return Pieces[3] as QualifiedIdentifier;
			}
		}

		public PositionToken SemicolonToken {
			get {
				return Pieces[4] as PositionToken;
			}
		}
	}

	public class MissingUsingDirective : UsingDirective
	{
	}
}
