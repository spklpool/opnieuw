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
	[Serializable]
	public class Identifier : PrimaryExpression
	{
		public new static Identifier parse(TokenProvider tokenizer)
		{
			Identifier ret = new MissingIdentifier();
			if (Token.IDENTIFIER == tokenizer.CurrentToken.Type)
			{
				PositionToken identifier = tokenizer.CurrentToken;
				tokenizer.nextToken();
				ret = new Identifier(identifier);
			}
			return ret;
		}
		
		public new static Identifier parse(string name)
		{
			Tokenizer t = new Tokenizer(new StringReader(name), "", null);
            t.nextToken();
            return Identifier.parse(t) as Identifier;
		}

		public Identifier()
		{
			Pieces.Add(new PositionToken(Position.Missing, "", Token.IDENTIFIER));
		}

		public Identifier(PositionToken identifier) :
		base(identifier)
		{
		}

		public PositionToken IdentifierToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}

		public string Name {
			get {
				return IdentifierToken.Text;
			}
		}

		public override string AsText {
			get {
				return Name;
			}
		}
	}

	public class MissingIdentifier : Identifier
	{
		public override void checkNotMissing()
		{
			throw new ParserException();
		}
	}
}
