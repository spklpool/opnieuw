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

using System.IO;
using System.Collections;

using Opnieuw.Framework;

namespace Opnieuw.Parsers.CSParser
{
	public class Expression : VariableInitializer, FundamentalPieceOfCode
	{
		public new static Expression parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			Expression expression = new MissingExpression();
			bool ret = ConditionalExpression.parse(tokenizer, ref expression);
			tokenizer.endBookmark(expression is MissingExpression);
			return expression;
		}

        public static Expression parse(string code)
        {
            Tokenizer t = new Tokenizer(new StringReader(code), "", null);
            t.nextToken();
            return Expression.parse(t) as Expression;
        }

		public Expression()
		{
		}

		public Expression(params FundamentalPieceOfCode[] list) :
		this(new Position(list))
		{
			for (int i=0; i<list.Length; i++)
			{
				Pieces.Add(list[i]);
			}
		}
		
		public Expression(Position position)
		{
			m_Position = position;
		}
	}

	public class MissingExpression : Expression
	{
		public override void checkNotMissing()
		{
			throw new ParserException();
		}
	}
}
