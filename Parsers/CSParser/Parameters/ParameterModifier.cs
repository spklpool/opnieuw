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
	public class ParameterModifier : PieceOfCode
	{
		public static ParameterModifier parse(TokenProvider tokenizer)
		{
			ParameterModifier ret = new MissingParameterModifier();
			if ((Token.REF == tokenizer.CurrentToken.Type) ||
				(Token.OUT == tokenizer.CurrentToken.Type))
			{
				ret = new ParameterModifier(tokenizer.CurrentToken);
				tokenizer.nextToken(); // out or ref
			}
			return ret;
        }

        public static ParameterModifier parse(string name)
        {
            Tokenizer t = new Tokenizer(new StringReader(name), "", null);
            t.nextToken();
            return ParameterModifier.parse(t) as ParameterModifier;
        }

        public ParameterModifier()
		{
		}

		public ParameterModifier(PositionToken modifierToken) :
		base(modifierToken)
		{
		}
		
		public PositionToken ModifierToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}
	}

	public class MissingParameterModifier : ParameterModifier
	{
	}
}
