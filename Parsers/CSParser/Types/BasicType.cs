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
	public class BasicType : PieceOfCode
	{
		public static BasicType parse(TokenProvider tokenizer)
		{
			BasicType ret = new MissingBasicType();
			if (IsTypeToken(tokenizer.CurrentToken))
			{
				ret = new BasicType(tokenizer.CurrentToken);
				tokenizer.nextToken();
			}
			return ret;
		}

		public static bool IsTypeToken(PositionToken token) 
		{
			return ((token.Type == Token.BOOL) ||
					(token.Type == Token.DECIMAL) ||
					(token.Type == Token.SBYTE) ||
					(token.Type == Token.BYTE) ||
					(token.Type == Token.SHORT) ||
					(token.Type == Token.USHORT) ||
					(token.Type == Token.UINT) ||
					(token.Type == Token.INT) ||
					(token.Type == Token.LONG) ||
					(token.Type == Token.ULONG) ||
					(token.Type == Token.CHAR) ||
					(token.Type == Token.FLOAT) ||
					(token.Type == Token.DOUBLE) ||
					(token.Type == Token.OBJECT) ||
					(token.Type == Token.STRING) ||
					(token.Type == Token.VOID));
		}
		
		public static BasicType parse(string typeString)
		{
			Tokenizer t = new Tokenizer(new StringReader(typeString), "", null);
            t.nextToken();
            return BasicType.parse(t) as BasicType;
		}
		
		public BasicType() :
		base(PositionToken.Missing)
		{
		}
		
		public BasicType(PositionToken typeToken) :
		base(typeToken)
		{
		}
		
		public PositionToken TypeToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}

		public string Name {
			get {
				return TypeToken.Text;
			}
		}
	}
	
	public class MissingBasicType : BasicType
	{
	}
}