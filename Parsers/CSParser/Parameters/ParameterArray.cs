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
	/// attributesopt params array-type identifier 
	/// </summary>
	public class ParameterArray : Parameter
	{
		public static ParameterArray parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			ParameterArray ret = new MissingParameterArray();
			AttributeSectionCollection attributes = AttributeSectionCollection.parse(tokenizer);
			if (Token.PARAMS == tokenizer.CurrentToken.Type)
			{
				PositionToken paramsToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // params
				ArrayType arrayType = ArrayType.parse(tokenizer);
				Identifier identifier = Identifier.parse(tokenizer);
				ret = new ParameterArray(attributes, paramsToken, arrayType, identifier);
			}
			tokenizer.endBookmark(ret is MissingParameterArray);
			return ret;
		}

		public ParameterArray() :
		base(new AttributeSectionCollection(), PositionToken.Missing, new MissingArrayType(), new MissingIdentifier())
		{
		}

		public ParameterArray(AttributeSectionCollection attributes, PositionToken paramsToken, ArrayType arrayType, Identifier identifier) :
		base(attributes, paramsToken, arrayType, identifier)
		{
		}
		
		public PositionToken ParamsToken {
			get {
				return Pieces[1] as PositionToken;
			}
		}

		public ArrayType Type {
			get {
				return Pieces[2] as ArrayType;
			}
		}

		public Identifier Identifier {
			get {
				return Pieces[3] as Identifier;
			}
		}

		public override string Name {
			get {
				return Identifier.Name;
			}
		}
	}

	public class MissingParameterArray : ParameterArray
	{
	}
}
