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
	public class InterfaceGetAccessor : PieceOfCode
	{
		public InterfaceGetAccessor() :
		base(new AttributeSectionCollection(),
			 new PositionToken(Position.Missing, "get", Token.GET), 
			 new PositionToken(Position.Missing, ";", Token.SEMICOLON))
		{
		}
		public InterfaceGetAccessor(AttributeSectionCollection attributeSections) :
		base(attributeSections,
			 new PositionToken(Position.Missing, "get", Token.GET), 
			 new PositionToken(Position.Missing, ";", Token.SEMICOLON))
		{
		}

        public InterfaceGetAccessor(Attribute attribute) :
		base(new AttributeSectionCollection(),
			 new PositionToken(Position.Missing, "get", Token.GET), 
			 new PositionToken(Position.Missing, ";", Token.SEMICOLON))
        {
            AttributeCollection attributes = new AttributeCollection();
            attributes.Add(attribute);
            AttributeSections.Add(new AttributeSection(attributes));
        }

		public InterfaceGetAccessor(AttributeSectionCollection accessors, PositionToken getToken, PositionToken semicolonToken) :
		base(accessors, getToken, semicolonToken)
		{
		}

		public AttributeSectionCollection AttributeSections {
			get {
				return Pieces[0] as AttributeSectionCollection;
			}
		}

		public PositionToken GetToken {
			get {
				return Pieces[1] as PositionToken;
			}
		}

		public PositionToken SemicolonToken {
			get {
				return Pieces[2] as PositionToken;
			}
		}

		public override string AsText {
			get {
				return GetToken.Text;
			}
		}
	}

	public class EmptyInterfaceGetAccessor : InterfaceGetAccessor
	{
		public EmptyInterfaceGetAccessor() :
		base(new AttributeSectionCollection(), PositionToken.Missing, PositionToken.Missing)
		{
		}
	}
}
