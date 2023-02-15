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
	/// attribute-target : 
	/// </summary>
	public class AttributeTargetSpecifier : PieceOfCode
	{
		public static AttributeTargetSpecifier parse(TokenProvider tokenizer)
		{
			AttributeTargetSpecifier ret = new MissingAttributeTargetSpecifier();
			Position position = Position.Missing;
			AttributeTarget attributeTarget = AttributeTarget.parse(tokenizer);
			if (false == (attributeTarget is MissingAttributeTarget))
			{
				PositionToken colonToken = tokenizer.checkToken(Token.COLON);
				ret = new AttributeTargetSpecifier(attributeTarget, colonToken);
			}
			return ret;
		}

		public AttributeTargetSpecifier()
		{
			Pieces.Add(new MissingAttributeTarget());
			Pieces.Add(PositionToken.Missing);
		}

		public AttributeTargetSpecifier(AttributeTarget attributeTarget, PositionToken colonToken) :
		base(attributeTarget, colonToken)
		{
		}

		public AttributeTarget AttributeTarget {
			get {
				return Pieces[0] as AttributeTarget;
			}
		}
		
		public PositionToken ColonToken {
			get {
				return Pieces[1] as PositionToken;
			}
		}
	}

	public class MissingAttributeTargetSpecifier : AttributeTargetSpecifier
	{
	}
}
