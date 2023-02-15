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
	/// attributesopt identifier
	/// attributesopt identifier = constant-expression 
	/// </summary>
	public class EnumMember : PieceOfCodeWithAttributes, ClassMember
	{
		public static EnumMember parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			EnumMember ret = new MissingEnumMember();
			Position position = Position.Missing;
			AttributeSectionCollection attributes = AttributeSectionCollection.parse(tokenizer);
			Identifier identifier = Identifier.parse(tokenizer);
			if (false == (identifier is MissingIdentifier))
			{
				if (Token.ASSIGN == tokenizer.CurrentToken.Type)
				{
					PositionToken assignToken = tokenizer.CurrentToken;
					tokenizer.nextToken(); // =
					Expression expression = Expression.parse(tokenizer);
					ret = new EnumMemberWithExpression(attributes, identifier, assignToken, expression);
				}
				else
				{
					ret = new EnumMember(attributes, identifier);
				}
			}
			tokenizer.endBookmark(ret is MissingEnumMember);
			return ret;
		}

		public EnumMember()
		{
			Pieces.Add(new AttributeSectionCollection());
			Pieces.Add(new MissingIdentifier());
		}

		public EnumMember(AttributeSectionCollection attributes, Identifier identifier) :
		base(attributes, identifier)
		{
		}

		public Identifier Identifier {
			get {
				return Pieces[1] as Identifier;
			}
		}
		public string Name {
			get {
				return Identifier.Name;
			}
		}
	}

	public class EnumMemberWithExpression : EnumMember
	{
		public EnumMemberWithExpression(AttributeSectionCollection attributes, Identifier identifier, PositionToken assignToken, Expression expression) :
		base(attributes, identifier)
		{
			Pieces.Add(assignToken);
			Pieces.Add(expression);
			m_Position = new Position(attributes, identifier, expression);
		}
		
		public PositionToken AssignToken {
			get {
				return Pieces[2] as PositionToken;
			}
		}

		public Expression Expression {
			get {
				return Pieces[3] as Expression;
			}
		}
	}

	public class MissingEnumMember : EnumMember
	{
	}
}
