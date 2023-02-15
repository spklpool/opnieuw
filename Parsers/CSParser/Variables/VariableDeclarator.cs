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
	public class VariableDeclarator : PieceOfCode
    {
#region static parsing code
        /// <summary>
        /// identifier
		/// identifier = variable-initializer 
		/// </summary>
		/// <param name="tokenizer"></param>
		/// <returns></returns>
		public static VariableDeclarator parse(TokenProvider tokenizer)
		{
			VariableDeclarator ret = new MissingVariableDeclarator();
			if (Token.IDENTIFIER == tokenizer.CurrentToken.Type)
			{
				Identifier identifier = Identifier.parse(tokenizer);
				VariableInitializer initializer = new MissingVariableInitializer();
				PositionToken assignToken = PositionToken.Missing;
				if (Token.ASSIGN == tokenizer.CurrentToken.Type)
				{
					assignToken = tokenizer.CurrentToken;
					tokenizer.nextToken(); // =
					initializer = VariableInitializer.parse(tokenizer);
				}
				ret = new VariableDeclarator(identifier, assignToken, initializer);
			}
            return ret;
        }
#endregion

#region constructors
        public VariableDeclarator()
		{
		}

		public VariableDeclarator(Identifier identifier, PositionToken assignToken, VariableInitializer initializer) :
		base(identifier, assignToken, initializer)
		{
        }

        public VariableDeclarator(Identifier identifier, VariableInitializer initializer) :
		base(identifier, 
             new PositionToken(Position.Missing, "=", Token.ASSIGN), 
             initializer)
        {
        }
#endregion

        public string Name {
			get {
				return Identifier.Name;
			}
		}

		public Identifier Identifier {
			get {
				return Pieces[0] as Identifier;
			}
		}

		public PositionToken AssignToken {
			get {
				return Pieces[1] as PositionToken;
			}
		}

		public VariableInitializer Initializer {
			get {
				return Pieces[2] as VariableInitializer;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Identifier);
				if (false == (Initializer is MissingVariableInitializer))
				{
					ret.Add(Initializer);
				}
				return ret;
			}
		}

		public override string AsText {
			get {
				return Identifier.Name;
			}
		}
	}

	public class MissingVariableDeclarator : VariableDeclarator
	{
	}
}