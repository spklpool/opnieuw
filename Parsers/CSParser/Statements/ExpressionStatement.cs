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
	public class ExpressionStatement : Statement
	{
		public new static Statement parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			Statement ret = new MissingStatement();
			Expression exp = Expression.parse(tokenizer);
			if ((exp is InvocationExpression) ||
				(exp is ObjectCreationExpression) ||
				(exp is AssignmentExpression) ||
				(exp is PostIncrementExpression) ||
				(exp is PostDecrementExpression) ||
				(exp is PreIncrementExpression) ||
				(exp is PreDecrementExpression))
			{
				if (Token.SEMICOLON == tokenizer.CurrentToken.Type)
				{
					PositionToken semicolonToken = tokenizer.CurrentToken;
					tokenizer.nextToken(); // ;
					ret = new ExpressionStatement(exp, semicolonToken);
				}
			}
			tokenizer.endBookmark(ret is MissingStatement);
			return ret;
		}

		public ExpressionStatement(Expression exp, PositionToken semicolonToken) :
		base(exp, semicolonToken)
		{
		}

		public Expression Expression {
			get {
				return Pieces[0] as Expression;
			}
		}
		
		public PositionToken SemicolonToken {
			get {
				return Pieces[1] as PositionToken;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Expression);
				return ret;
			}
		}
	}
}
