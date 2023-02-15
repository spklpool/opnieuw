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
	public class InterfaceAccessors : PieceOfCode
	{
		public static InterfaceAccessors parse(TokenProvider tokenizer)
		{
			AttributeSectionCollection attributes1 = AttributeSectionCollection.parse(tokenizer);
			InterfaceGetAccessor getAccessor = new EmptyInterfaceGetAccessor();
			InterfaceSetAccessor setAccessor = new EmptyInterfaceSetAccessor();
			if (Token.GET == tokenizer.CurrentToken.Type)
			{
				PositionToken getToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // get
				PositionToken semicolonToken = tokenizer.checkToken(Token.SEMICOLON);
				getAccessor = new InterfaceGetAccessor(attributes1, getToken, semicolonToken);
			}
			else if (Token.SET == tokenizer.CurrentToken.Type)
			{
				PositionToken setToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // set
				PositionToken semicolonToken = tokenizer.checkToken(Token.SEMICOLON);
				setAccessor = new InterfaceSetAccessor(attributes1, setToken, semicolonToken);
			}
			AttributeSectionCollection attributes2 = AttributeSectionCollection.parse(tokenizer);
			if (Token.GET == tokenizer.CurrentToken.Type)
			{
				PositionToken getToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // get
				PositionToken semicolonToken = tokenizer.checkToken(Token.SEMICOLON);
				getAccessor = new InterfaceGetAccessor(attributes2, getToken, semicolonToken);
			}
			else if (Token.SET == tokenizer.CurrentToken.Type)
			{
				PositionToken setToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // set
				PositionToken semicolonToken = tokenizer.checkToken(Token.SEMICOLON);
				setAccessor = new InterfaceSetAccessor(attributes2, setToken, semicolonToken);
			}
			InterfaceAccessors ret = new InterfaceAccessors(getAccessor, setAccessor);
			return ret;
		}
		
		public static InterfaceAccessors parse(string input)
		{
			Tokenizer t = new Tokenizer(new StringReader(input), "", null);
            t.nextToken();
            return InterfaceAccessors.parse(t) as InterfaceAccessors;
		}

		public InterfaceAccessors()
		{
			Pieces.Add(new EmptyInterfaceGetAccessor());
			Pieces.Add(new EmptyInterfaceSetAccessor());
		}

		public InterfaceAccessors(InterfaceGetAccessor getAccessor, InterfaceSetAccessor setAccessor) :
		base(getAccessor, setAccessor)
		{
		}

		public InterfaceGetAccessor GetAccessor {
			get {
				return Pieces[0] as InterfaceGetAccessor;
			}
		}
		public bool ContainsGet {
			get {
				return !(GetAccessor is EmptyInterfaceGetAccessor);
			}
		}

		public InterfaceSetAccessor SetAccessor {
			get {
				return Pieces[1] as InterfaceSetAccessor;
			}
		}
		public bool ContainsSet {
			get {
				return !(SetAccessor is EmptyInterfaceSetAccessor);
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				if (ContainsGet)
				{
					ret.Add(GetAccessor);
				}
				if (ContainsSet)
				{
					ret.Add(SetAccessor);
				}
				return ret;
			}
		}
	}
}
