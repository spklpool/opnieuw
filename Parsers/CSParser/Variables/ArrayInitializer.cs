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
	/// { variable-initializer-listopt }
	/// { variable-initializer-list , } 
	/// </summary>
	public class ArrayInitializer : VariableInitializer
	{
		public new static ArrayInitializer parse(TokenProvider tokenizer)
		{
			ArrayInitializer ret = new MissingArrayInitializer();
			if (Token.OPEN_BRACE == tokenizer.CurrentToken.Type)
			{
				PositionToken openBraceToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // {
				VariableInitializerCollection initializers = VariableInitializerCollection.parse(tokenizer);
				PositionToken closeBraceToken = tokenizer.checkToken(Token.CLOSE_BRACE);
				ret = new ArrayInitializer(openBraceToken, initializers, closeBraceToken);
			}
			return ret;
		}

		public ArrayInitializer()
		{
		}

		public ArrayInitializer(PositionToken openBraceToken, VariableInitializerCollection initializers, PositionToken closeBraceToken) :
		base(openBraceToken, initializers, closeBraceToken)
		{
		}
		
		public PositionToken OpenBraceToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}

		public VariableInitializerCollection Initializers {
			get {
				return Pieces[1] as VariableInitializerCollection;
			}
		}
		
		public PositionToken CloseBraceToken {
			get {
				return Pieces[2] as PositionToken;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Initializers);
				return ret;
			}
		}
	}

	public class MissingArrayInitializer : ArrayInitializer
	{
		public MissingArrayInitializer()
		{
			Pieces.Add(PositionToken.Missing);
			Pieces.Add(new VariableInitializerCollection());
			Pieces.Add(PositionToken.Missing);
			//System.Console.WriteLine("Creating MissingArrayInitializer with position: " + m_Position.ToString());
			//System.Console.WriteLine("OpenBraceToken: " + OpenBraceToken.Position);
			//System.Console.WriteLine("Initializers: " + Initializers.Position is InvalidPosition);
			//System.Console.WriteLine("CloseBraceToken: " + CloseBraceToken.Position is InvalidPosition);
		}
	}
}
