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
	/// attributesopt newopt return-type identifier ( formal-parameter-listopt ) ; 
	/// attributesopt newopt type identifier { interface-accessors } 
	/// attributesopt newopt event type identifier ; 
	/// attributesopt newopt type this [ formal-parameter-list ] { interface-accessors } 
	/// </summary>
	public class InterfaceMember : ModifyablePieceOfCodeWithAttributes, NamespaceMember
	{
		public static InterfaceMember parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			InterfaceMember ret = new MissingInterfaceMember();
			AttributeSectionCollection attributes = AttributeSectionCollection.parse(tokenizer);
			ModifierCollection modifiers = ModifierCollection.parse(tokenizer); // newopt
			DataType type = new MissingDataType();
			Identifier identifier = new MissingIdentifier();
			if (Token.EVENT == tokenizer.CurrentToken.Type)
			{
				PositionToken eventToken = tokenizer.CurrentToken;
				tokenizer.nextToken();
				type = DataType.parse(tokenizer);
				identifier = Identifier.parse(tokenizer);
				PositionToken semicolonToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // ;
				ret = new InterfaceEvent(attributes, modifiers, eventToken, type, identifier, semicolonToken);
			}
			else
			{
				type = DataType.parse(tokenizer);
				if (Token.THIS == tokenizer.CurrentToken.Type)
				{
					PositionToken thisToken = tokenizer.CurrentToken;
					tokenizer.nextToken(); // this
					if (Token.OPEN_BRACKET == tokenizer.CurrentToken.Type)
					{
						PositionToken openBracketToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // [
						FormalParameterList parameters = FormalParameterList.parse(tokenizer);
						if (Token.CLOSE_BRACKET == tokenizer.CurrentToken.Type)
						{
							PositionToken closeBracketToken = tokenizer.CurrentToken;
							tokenizer.nextToken(); // ]
							if (Token.OPEN_BRACE == tokenizer.CurrentToken.Type)
							{
								PositionToken openBraceToken = tokenizer.CurrentToken;
								tokenizer.nextToken(); // {
								InterfaceAccessors accessors = InterfaceAccessors.parse(tokenizer);
								PositionToken closeBraceToken = tokenizer.CurrentToken;
								tokenizer.nextToken(); // }
								ret = new InterfaceIndexer(attributes, modifiers, type, thisToken, 
														   openBracketToken, parameters, closeBracketToken,
														   openBraceToken, accessors, closeBraceToken);
							}
						}
					}
				}
				else if (Token.IDENTIFIER == tokenizer.CurrentToken.Type)
				{
					identifier = Identifier.parse(tokenizer);
					if(Token.OPEN_PARENS == tokenizer.CurrentToken.Type)
					{
						PositionToken openBraceToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // (
						FormalParameterList parameters = FormalParameterList.parse(tokenizer);
						PositionToken closeBraceToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // )
						PositionToken semicolonToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // ;
						ret = new InterfaceMethod(attributes, modifiers, type, identifier, openBraceToken, parameters, closeBraceToken, semicolonToken);
					}
					else if (Token.OPEN_BRACE == tokenizer.CurrentToken.Type)
					{
						PositionToken openBraceToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // {
						InterfaceAccessors accessors = InterfaceAccessors.parse(tokenizer);
						PositionToken closeBraceToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // }
						ret = new InterfaceProperty(attributes, modifiers, type, identifier, openBraceToken, accessors, closeBraceToken);
					}
				}
			}
			ret.m_Modifiers = modifiers;
			tokenizer.endBookmark(ret is MissingInterfaceMember);
			return ret;
		}

		public InterfaceMember()
		{
		}

		public InterfaceMember(params FundamentalPieceOfCode[] list) :
		base(list)
		{
		}

		public InterfaceMember(string name)
		{
			m_Name = name;
		}

		protected string m_Name;
		public string Name {
			get {
				return m_Name;
			}
		}

		public string FullyQualifiedParentNamespace {
			get {
				return "";
			}
		}

		public NamespaceMember LookupType(string name)
		{
			return new MissingNamespaceMember();
		}
	}

	public class MissingInterfaceMember : InterfaceMember
	{
	}
}
