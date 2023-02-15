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
	public class InterfaceProperty : InterfaceMember
	{
		public InterfaceProperty(AttributeSectionCollection attributes, 
								 ModifierCollection modifiers, 
								 DataType type, Identifier identifier, 
								 PositionToken openBraceToken, 
								 InterfaceAccessors accessors, 
								 PositionToken closeBraceToken) :
		base(attributes, modifiers, type, identifier, openBraceToken, 
			 accessors, closeBraceToken)
		{
			m_Name = identifier.Name;
		}

		public InterfaceProperty(DataType type, string name, 
								 InterfaceAccessors accessors) :
		base(new AttributeSectionCollection(), new ModifierCollection(), 
			 type, 
			 Identifier.parse(" " + name), 
			 new PositionToken(Position.Missing, "{", Token.OPEN_BRACE), 
			 accessors, 
			 new PositionToken(Position.Missing, "}", Token.CLOSE_BRACE))
		{
			m_Name = Identifier.Name;
		}

        public override void Format()
        {
            base.Format();
			string accessorsBlock = Accessors.Generate();
			string newAccessorsBlock = GenericBlockOfCode.Reindent(accessorsBlock, 4, 4);
			InterfaceAccessors newAccessors = InterfaceAccessors.parse(newAccessorsBlock);
			Pieces[5] = newAccessors;
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

		public InterfaceAccessors Accessors {
			get {
				return Pieces[5] as InterfaceAccessors;
			}
		}

		public PositionToken CloseBraceToken {
			get {
				return Pieces[6] as PositionToken;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Accessors.Children);
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
