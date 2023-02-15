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
	public class DestructorDeclaration : PieceOfCodeWithAttributes, ClassMember
	{
		/// <summary>
		/// attributesopt externopt ~ identifier ( ) destructor-body 
		/// </summary>
		public static ClassMember parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			ClassMember ret = new MissingClassMember();
			AttributeSectionCollection attributes = AttributeSectionCollection.parse(tokenizer);
			PositionToken externToken = PositionToken.Missing;
			PositionToken tildeToken = PositionToken.Missing;
			if (Token.EXTERN == tokenizer.CurrentToken.Type)
			{
				externToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // extern
			}
			if (Token.TILDE == tokenizer.CurrentToken.Type)
			{
				tildeToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // ~
				Identifier identifier = Identifier.parse(tokenizer);
				identifier.checkNotMissing();
				PositionToken openParensToken = tokenizer.checkToken(Token.OPEN_PARENS);
				PositionToken closeParensToken = tokenizer.checkToken(Token.CLOSE_PARENS);
				Statement statement = Statement.parse(tokenizer);
				statement.checkNotMissing();
				ret = new DestructorDeclaration(attributes, externToken, tildeToken, identifier, openParensToken, closeParensToken, statement);
			}			
			tokenizer.endBookmark(ret is MissingClassMember);
			return ret;
		}

		public DestructorDeclaration()
		{
		}

		public DestructorDeclaration(AttributeSectionCollection attributes, PositionToken externToken, 
									 PositionToken tildeToken, Identifier identifier, 
									 PositionToken openParensToken, PositionToken closeParensToken, 
									 Statement body) :
		base(attributes, externToken, tildeToken, identifier, openParensToken, closeParensToken, body)
		{
		}
		
		public PositionToken ExternToken {
			get {
				return Pieces[1] as PositionToken;
			}
		}
		
		public PositionToken TildeToken {
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
		
		public PositionToken OpenParensToken {
			get {
				return Pieces[4] as PositionToken;
			}
		}
		
		public PositionToken CloseParensToken {
			get {
				return Pieces[5] as PositionToken;
			}
		}

		public Statement Body {
			get {
				return Pieces[6] as Statement;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Body);
				return ret;
			}
		}

		public override string AsText {
			get {
				return TildeToken.Text + Identifier.Name;
			}
		}
	}

	public class MissingDestructorDeclaration : DestructorDeclaration
	{
	}
}
