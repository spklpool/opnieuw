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
	public class ForInitializer : PieceOfCode
    {
#region static parsing code
        //local-variable-declaration
        //statement-expression-list
        public static ForInitializer parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			ForInitializer ret = new MissingForInitializer();
			LocalVariableDeclaration lvd = LocalVariableDeclaration.parse(tokenizer);
			//TODO:  This should be statement-expression-list.
			ExpressionCollection expressions = new ExpressionCollection();
			if (lvd is MissingLocalVariableDeclaration)
			{
				expressions = ExpressionCollection.parse(tokenizer);
			}
			if ((false == (lvd is MissingLocalVariableDeclaration)) ||
				(expressions.Count > 0))
			{
				ret = new ForInitializer(lvd, expressions);
			}
			tokenizer.endBookmark(ret is MissingForInitializer);
			return ret;
        }
        #endregion

#region constructors
        public ForInitializer() :
		base(new MissingLocalVariableDeclaration(), new ExpressionCollection())
		{
		}

		public ForInitializer(LocalVariableDeclaration lvd, ExpressionCollection expressions) :
		base(lvd, expressions)
		{
        }
        #endregion

        public LocalVariableDeclaration LocalVariableDeclaration {
			get {
				return Pieces[0] as LocalVariableDeclaration;
			}
		}

		public ExpressionCollection ExpressionList {
			get {
				return Pieces[1] as ExpressionCollection;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(LocalVariableDeclaration);
				ret.Add(ExpressionList.Children);
				return ret;
			}
		}
	}

	public class MissingForInitializer : ForInitializer
	{
	}
}
