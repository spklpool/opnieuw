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
	/// attribute-target: 
	///	assembly
	///	field
	///	event
	///	method
	///	module
	///	param
	///	property
	///	return
	///	type 
	/// </summary>
	public class AttributeTarget : PieceOfCode
	{
		public static AttributeTarget parse(TokenProvider tokenizer)
		{
			AttributeTarget ret = new MissingAttributeTarget();
			PositionToken checkToken = tokenizer.CurrentToken;
			if ((Token.ASSEMBLY == checkToken.Type) ||
				(Token.FIELD == checkToken.Type) ||
				(Token.EVENT == checkToken.Type) ||
				(Token.METHOD == checkToken.Type) ||
				(Token.MODULE == checkToken.Type) ||
				(Token.PARAM == checkToken.Type) ||
				(Token.PROPERTY == checkToken.Type) ||
				(Token.RETURN == checkToken.Type) ||
				(Token.TYPE == checkToken.Type))
			{
				ret = new AttributeTarget(checkToken);
				tokenizer.nextToken();
			}
			return ret;
		}

		public AttributeTarget(PositionToken targetToken) :
		base(targetToken)
		{
		}
		
		public PositionToken TargetToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}
	}

	public class MissingAttributeTarget : AttributeTarget
	{
		public MissingAttributeTarget() :
		base(PositionToken.Missing)
		{
		}
	}
}
