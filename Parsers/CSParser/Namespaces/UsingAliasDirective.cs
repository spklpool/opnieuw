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
	/// A using-alias-directive introduces an identifier that serves as an alias 
	/// for a namespace or type within the immediately enclosing compilation unit
	/// or namespace body. 
	///   using-alias-directive: 
	///	    using identifier = namespace-or-type-name ; 
	/// </summary>
	public class UsingAliasDirective : UsingDirective
	{
		public UsingAliasDirective(PositionToken usingToken, QualifiedIdentifier identifier, PositionToken assignToken, QualifiedIdentifier namespaceOrTypeName, PositionToken semicolonToken) :
		base(usingToken, identifier, assignToken, namespaceOrTypeName, semicolonToken)
		{
		}
		
		public QualifiedIdentifier Identifier {
			get {
				return Pieces[1] as QualifiedIdentifier;
			}
		}

		public PositionToken AssignToken {
			get {
				return Pieces[2] as PositionToken;
			}
		}
	}
}
