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
	public class BaseTypeList : PieceOfCode
	{
		public static BaseTypeList parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			BaseTypeList ret = new MissingBaseTypeList();
			if (Token.COLON == tokenizer.CurrentToken.Type)
			{
				PositionToken colonToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // :
				DataTypeCollection types = new DataTypeCollection();
				DataType type = DataType.parse(tokenizer);
				while (false == (type is MissingDataType))
				{
					types.Add(type);
					if (Token.COMMA == tokenizer.CurrentToken.Type)
					{
						tokenizer.nextToken(); // ,
						type = DataType.parse(tokenizer);
					}
					else
					{
						break;
					}
				}
				ret = new BaseTypeList(colonToken, types);
			}
			tokenizer.endBookmark(ret is MissingBaseTypeList);
			return ret;
		}

		public BaseTypeList(PositionToken colonToken, DataTypeCollection types) :
		base(colonToken, types)
		{
		}

		public BaseTypeList(DataTypeCollection types) :
		base(new PositionToken(Position.Missing, ":", Token.COLON), types)
		{
			ColonToken.LeadingCharacters = " ";
			TypeCollection.LeadingCharacters = " ";
		}
		
		public PositionToken ColonToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}
		
		public DataTypeCollection TypeCollection {
			get {
				return Pieces[1] as DataTypeCollection;
			}
		}

		/// <summary>
		/// Returns the base class which is the first element in the 
		/// TypeList collection.
		/// </summary>
		public DataType Base {
			get {
				DataType ret = new MissingDataType();
				if (0 != TypeCollection.Count)
				{
					ret = TypeCollection[0];
				}
				return ret;
			}
		}

		/// <summary>
		/// Returns the TypeList collection except the first item which is
		/// the base class.
		/// class someClass : base, interface, interface, ...
		/// </summary>
		public DataTypeCollection BaseInterfaces {
			get {
				DataTypeCollection ret = new DataTypeCollection();
				for(int i=1; i<TypeCollection.Count; i++)
				{
					ret.Add(TypeCollection[i]);
				}
				return ret;
			}
		}
	}
	
	public class MissingBaseTypeList : BaseTypeList
	{
		public MissingBaseTypeList() :
		base(PositionToken.Missing, new DataTypeCollection())
		{
		}
	}
}