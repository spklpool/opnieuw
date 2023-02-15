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
	/// attributesopt newopt type this [ formal-parameter-list ] { interface-accessors } 
	/// </summary>
	public class InterfaceIndexer : InterfaceMember
	{
		public InterfaceIndexer(AttributeSectionCollection attributes, ModifierCollection modifiers, 
								DataType type, PositionToken thisToken, 
								PositionToken openBracketToken, FormalParameterList parameters, 
								PositionToken closeBracketToken, PositionToken openBraceToken, 
								InterfaceAccessors accessors, PositionToken closeBraceToken) :
		base(attributes, modifiers, type, thisToken, openBracketToken, parameters, 
			 closeBracketToken,  openBraceToken, accessors, closeBraceToken)
		{
		}

		public DataType Type {
			get {
				return Pieces[2] as DataType;
			}
		}
		
		public PositionToken OpenBracketToken {
			get {
				return Pieces[3] as PositionToken;
			}
		}

		public FormalParameterList Parameters {
			get {
				return Pieces[4] as FormalParameterList;
			}
		}
		
		public PositionToken CloseBracketToken {
			get {
				return Pieces[5] as PositionToken;
			}
		}
		
		public PositionToken OpenBraceToken {
			get {
				return Pieces[6] as PositionToken;
			}
		}

		public InterfaceAccessors Accessors {
			get {
				return Pieces[7] as InterfaceAccessors;
			}
		}
		
		public PositionToken CloseBraceToken {
			get {
				return Pieces[8] as PositionToken;
			}
		}
	}
}
