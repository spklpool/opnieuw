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
	/// attributesopt newopt event type identifier ; 
	/// </summary>
	public class InterfaceEvent : InterfaceMember
	{
		public InterfaceEvent(AttributeSectionCollection attributes,
							  ModifierCollection modifiers,
							  PositionToken eventToken, DataType type, 
							  Identifier identifier, PositionToken semicolonToken) :
		base(attributes, modifiers, eventToken, type, identifier, semicolonToken)
		{
			m_Name = identifier.Name;
		}

		public PositionToken EventToken {
			get {
				return Pieces[2] as PositionToken;
			}
		}

		public DataType Type {
			get {
				return Pieces[3] as DataType;
			}
		}
		
		public Identifier Identifier {
			get {
				return Pieces[4] as Identifier;
			}
		}

		public PositionToken SemicolonToken {
			get {
				return Pieces[5] as PositionToken;
			}
		}

		public override string AsText {
			get {
				return Name;
			}
		}
	}
}
