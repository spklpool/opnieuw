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
	public class EnumBase : PieceOfCode
	{
		public static EnumBase parse(TokenProvider tokenizer)
		{
			EnumBase ret = new MissingEnumBase();
			if (Token.COLON == tokenizer.CurrentToken.Type)
			{
				PositionToken colonToken = tokenizer.CurrentToken;
				tokenizer.nextToken();
				DataType type = DataType.parse(tokenizer);
				type.checkNotMissing();
				ret = new EnumBase(colonToken, type);
			}
			return ret;
		}

		public EnumBase()
		{
			Pieces.Add(new MissingDataType());
			Pieces.Add(PositionToken.Missing);
		}

		public EnumBase(PositionToken colonToken, DataType type) :
		base(colonToken, type)
		{
		}

		public DataType Type {
			get {
				return Pieces[0] as DataType;
			}
		}

		public PositionToken ColonToken {
			get {
				return Pieces[1] as PositionToken;
			}
		}
	}

	public class MissingEnumBase : EnumBase
	{
	}
}
