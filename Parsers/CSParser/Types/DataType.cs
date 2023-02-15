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
	public class DataType : PieceOfCode
	{
		public static DataType parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			DataType ret = new MissingDataType();
			Expression identifier = new MissingExpression();
			Position position = new Position(tokenizer.CurrentToken);
			BasicType basicType = BasicType.parse(tokenizer);
			if (basicType is MissingBasicType) {
				identifier = QualifiedIdentifier.parse(tokenizer);
				if (identifier is QualifiedIdentifier) {
					ret = new DataType(identifier as QualifiedIdentifier);
					if (Token.OPEN_BRACKET == tokenizer.CurrentToken.Type) {
						RankSpecifierCollection rankSpecifiers = RankSpecifierCollection.parse(tokenizer);
						ret = new ArrayType(ret, rankSpecifiers);
					}
				}
			} else {
				ret = new DataType(basicType);
				if (Token.OPEN_BRACKET == tokenizer.CurrentToken.Type) {
					RankSpecifierCollection rankSpecifiers = RankSpecifierCollection.parse(tokenizer);
					ret = new ArrayType(ret, rankSpecifiers);
				}
			}
			tokenizer.endBookmark(ret is MissingDataType);
			return ret;
        }

        public static DataType parse(string name)
        {
            Tokenizer t = new Tokenizer(new StringReader(name), "", null);
            t.nextToken();
            return DataType.parse(t) as DataType;
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
		
		public DataType()
		{
		}

		public DataType(params FundamentalPieceOfCode[] list) :
		base(list)
		{
		}
		
		public DataType(BasicType basicType)
		{
			foreach (FundamentalPieceOfCode piece in basicType.Pieces)
			{
				Pieces.Add(piece);
			}
			m_Position = new Position(basicType.Position);
			m_Name = basicType.Name;
		}

		public DataType(QualifiedIdentifier identifier) :
		base(identifier)
		{
			m_Name = identifier.Name;
		}

		public DataType(string name) :
		base(QualifiedIdentifier.parse(name))
		{
			m_Name = name;
		}

		protected string m_Name = "";
		public string Name {
			get {
				return m_Name;
			}
		}
	}

	public class MissingDataType : DataType
	{
		public override void checkNotMissing()
		{
			throw new ParserException();
		}
	}
}
