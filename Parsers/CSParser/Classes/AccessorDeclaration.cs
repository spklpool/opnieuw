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
	public class AccessorDeclaration : PieceOfCodeWithAttributes
	{
		public AccessorDeclaration(AttributeSectionCollection attributes, PositionToken accessorToken, Statement statement) :
		base(attributes, accessorToken, statement)
		{
			m_AttributeSections = attributes;
        }

        public override void Format()
        {
            base.Format();
            Body.LeadingCharacters = " ";
        }

        public PositionToken AccessorToken {
			get {
				return Pieces[1] as PositionToken;
			}
		}

		public Statement Body {
			get {
				return Pieces[2] as Statement;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Body.Statements.Children);
				return ret;
			}
		}
		
		protected static AccessorDeclaration m_Missing = new MissingAccessorDeclaration();
		public static AccessorDeclaration Missing {
			get {
				return m_Missing;
			}
		}
	}

	public class MissingAccessorDeclaration : AccessorDeclaration
	{
		public MissingAccessorDeclaration() :
		base(new AttributeSectionCollection(), PositionToken.Missing, new MissingStatement())
		{
		}
	}
}
