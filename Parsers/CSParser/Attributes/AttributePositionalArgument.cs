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
	/// expression
	/// </summary>
	public class AttributePositionalArgument : PieceOfCode
	{
		public static AttributePositionalArgument parse(TokenProvider tokenizer)
		{
			AttributePositionalArgument ret = new MissingAttributePositionalArgument();
			Expression expression = Expression.parse(tokenizer);
			if (false == (expression is MissingExpression))
			{
				//If it is followed by an assignment operator, it must be
				//a named argument and not a positional argument.
				if (Token.ASSIGN != tokenizer.CurrentToken.Type)
				{
					ret = new AttributePositionalArgument(expression);
				}
			}
			return ret;
		}

		public AttributePositionalArgument()
		{
			Pieces.Add(new MissingExpression());
		}

		public AttributePositionalArgument(Expression expression) :
		base(expression)
		{
		}

		public Expression Expression {
			get {
				return Pieces[0] as Expression;
			}
		}
	}

	public class MissingAttributePositionalArgument : AttributePositionalArgument
	{
	}
}
