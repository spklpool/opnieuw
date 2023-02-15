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
	public class PrimaryExpression : Expression
	{
		public new static Expression parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			Expression ret = new MissingExpression();
			ret = PrimaryNoArrayCreationExpression.parse(tokenizer);
			if (ret is MissingExpression)
			{
				ret = ArrayCreationExpression.parse(tokenizer);
			}
			tokenizer.endBookmark(ret is MissingExpression);
			return ret;
		}

		public PrimaryExpression(params FundamentalPieceOfCode[] list) :
		base(list)
		{
		}

		public PrimaryExpression(Position position)
		{
			m_Position = position;
		}
	}
}
