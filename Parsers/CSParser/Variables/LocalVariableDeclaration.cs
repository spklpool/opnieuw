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
	public class LocalVariableDeclaration : PieceOfCode
    {
#region static parsing code
        //type local-variable-declarators
        public static LocalVariableDeclaration parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			LocalVariableDeclaration ret = new MissingLocalVariableDeclaration();
			DataType type = DataType.parse(tokenizer);
			if (false == (type is MissingDataType))
			{
				VariableDeclaratorCollection variableDeclarators = VariableDeclaratorCollection.parse(tokenizer);
				if (variableDeclarators.Count > 0)
				{
					ret = new LocalVariableDeclaration(type, variableDeclarators);
				}
			}
			tokenizer.endBookmark(ret is MissingLocalVariableDeclaration);
			return ret;
        }
#endregion

#region constructors
        public LocalVariableDeclaration()
		{
			Pieces.Add(new MissingDataType());
			Pieces.Add(new VariableDeclaratorCollection());
		}

		public LocalVariableDeclaration(DataType type, VariableDeclaratorCollection variableDeclarators) :
		base(type, variableDeclarators)
		{
        }
#endregion
        
        public DataType Type {
			get {
				return Pieces[0] as DataType;
			}
		}

		public VariableDeclaratorCollection VariableDeclarators {
			get {
				return Pieces[1] as VariableDeclaratorCollection;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(VariableDeclarators);
				return ret;
			}
		}
	}

	public class MissingLocalVariableDeclaration : LocalVariableDeclaration
	{
	}
}
