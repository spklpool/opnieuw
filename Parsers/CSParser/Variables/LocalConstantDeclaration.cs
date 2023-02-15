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
	/// TODO:  This is currently implemented to be less restrictive than the grammar specifies.  It
	/// will allow array initializers for constant declarators where only expressions should be allowed.
	/// We should create a ConstantDeclaratorCollection instead of using VariableDeclaratorCollection to
	/// implement this.
	/// </summary>
	public class LocalConstantDeclaration : PieceOfCode
	{
		public static LocalConstantDeclaration parse(TokenProvider tokenizer)
		{
			LocalConstantDeclaration ret = new MissingLocalConstantDeclaration();
			if (Token.CONST == tokenizer.CurrentToken.Type)
			{
				PositionToken constToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // const
				DataType type = DataType.parse(tokenizer);
				if (false == (type is MissingDataType))
				{
					VariableDeclaratorCollection variableDeclarators = VariableDeclaratorCollection.parse(tokenizer);
					ret = new LocalConstantDeclaration(constToken, type, variableDeclarators);
				}
			}
			return ret;
		}

		public LocalConstantDeclaration() :
		base(PositionToken.Missing, new MissingDataType(), new VariableDeclaratorCollection())
		{
		}

		public LocalConstantDeclaration(PositionToken constToken, DataType type, VariableDeclaratorCollection variableDeclarators) :
		base(constToken, type, variableDeclarators)
		{
		}

		public PositionToken ConstToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}

		public DataType Type {
			get {
				return Pieces[1] as DataType;
			}
		}

		public VariableDeclaratorCollection VariableDeclarators {
			get {
				return Pieces[2] as VariableDeclaratorCollection;
			}
		}

		public override ExpressionCollection Expressions {
			get {
				return VariableDeclarators.Expressions;
			}
		}
	}

	public class MissingLocalConstantDeclaration : LocalConstantDeclaration
	{
	}
}
