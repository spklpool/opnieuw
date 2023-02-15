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
	public class InterfaceMethod : InterfaceMember
	{
		public InterfaceMethod(AttributeSectionCollection attributes, 
							   ModifierCollection modifiers, DataType type, 
							   Identifier identifier, PositionToken openBraceToken, 
							   FormalParameterList parameters, PositionToken closeBraceToken, 
							   PositionToken semicolonToken) :
		base(attributes, modifiers, type, identifier, openBraceToken, parameters, 
			 closeBraceToken, semicolonToken)
		{
			m_Name = identifier.Name;
		}
		
		public InterfaceMethod(DataType type, String name, FormalParameterList parameters) :
		base(new AttributeSectionCollection(), new ModifierCollection(), type, 
			 Identifier.parse(" " + name), 
			 new PositionToken(Position.Missing, "(", Token.OPEN_PARENS), 
			 parameters, 
			 new PositionToken(Position.Missing, ")", Token.CLOSE_PARENS), 			 
			 new PositionToken(Position.Missing, ";", Token.SEMICOLON))
		{
			m_Name = name;
		}
		
		public DataType Type {
			get {
				return Pieces[2] as DataType;
			}
		}
		
		public Identifier Identifier {
			get {
				return Pieces[3] as Identifier;
			}
		}
		
		public PositionToken OpenBraceToken {
			get {
				return Pieces[4] as PositionToken;
			}
		}

		public FormalParameterList Parameters {
			get {
				return Pieces[5] as FormalParameterList;
			}
		}
		
		public PositionToken CloseBraceToken {
			get {
				return Pieces[6] as PositionToken;
			}
		}
		
		public PositionToken SemicolonToken {
			get {
				return Pieces[7] as PositionToken;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Parameters.Children);
				return ret;
			}
		}

		public override string AsText {
			get {
				return Name;
			}
		}
	}
}
