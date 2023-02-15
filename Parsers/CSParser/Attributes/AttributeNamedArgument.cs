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
	public class AttributeNamedArgument : PieceOfCode
	{
		/// <summary>
		/// identifier = attribute-argument-expression 
		/// </summary>
		public static AttributeNamedArgument parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			AttributeNamedArgument ret = new MissingAttributeNamedArgument();
			Identifier identifier = Identifier.parse(tokenizer);
			if (false == (identifier is MissingIdentifier))
			{
				if (Token.ASSIGN == tokenizer.CurrentToken.Type)
				{
					PositionToken assignToken = tokenizer.CurrentToken;
					tokenizer.nextToken(); // =
					Expression expression = Expression.parse(tokenizer);
					if (false == (expression is MissingExpression))
					{
						ret = new AttributeNamedArgument(identifier, assignToken, expression);
					}
				}
			}
			tokenizer.endBookmark(ret is MissingAttributeNamedArgument);
			return ret;
		}

		public AttributeNamedArgument()
		{
			Pieces.Add(new MissingIdentifier());
			Pieces.Add(PositionToken.Missing);
			Pieces.Add(new MissingExpression());
		}

		public AttributeNamedArgument(Identifier identifier, PositionToken assignToken, Expression expression) :
		base(identifier, assignToken, expression)
		{
		}

		public Identifier Identifier {
			get {
				return Pieces[0] as Identifier;
			}
		}
		public string Name {
			get {
				return Identifier.Name;
			}
		}
		
		public PositionToken AssignToken {
			get {
				return Pieces[1] as PositionToken;
			}
		}

		public Expression Expression {
			get {
				return Pieces[2] as Expression;
			}
		}
	}

	public class MissingAttributeNamedArgument : AttributeNamedArgument
	{
	}
}
