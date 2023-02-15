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
	public class ConstantDeclaration : ModifyablePieceOfCodeWithAttributes, ClassMember, StructMember
	{
		public static ClassMember parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			ClassMember ret = new MissingClassMember();
			AttributeSectionCollection attributes = AttributeSectionCollection.parse(tokenizer);
			ModifierCollection modifiers = ModifierCollection.parse(tokenizer);
			if (Token.CONST == tokenizer.CurrentToken.Type)
			{
				PositionToken constToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // const
				DataType type = DataType.parse(tokenizer);
				type.checkNotMissing();
				//TODO:  This should be ConstantDeclaratorCollecion.
				VariableDeclaratorCollection declarators = VariableDeclaratorCollection.parse(tokenizer);
				declarators.checkNotMissing();
				PositionToken semicolonToken = tokenizer.checkToken(Token.SEMICOLON);
                ret = new ConstantDeclaration(attributes, modifiers, constToken, type, declarators, semicolonToken);
			}
			tokenizer.endBookmark(ret is MissingClassMember);
			return ret;
		}

		public ConstantDeclaration()
		{
			Pieces.Add(new AttributeSectionCollection());
			Pieces.Add(new ModifierCollection());
			Pieces.Add(PositionToken.Missing);
			Pieces.Add(new MissingDataType());
			Pieces.Add(new VariableDeclaratorCollection());
			Pieces.Add(PositionToken.Missing);
		}

		public ConstantDeclaration(AttributeSectionCollection attributes, 
								   ModifierCollection modifiers, 
								   PositionToken constToken, DataType type, 
								   VariableDeclaratorCollection declarators,
								   PositionToken semicolonToken) :
		base(attributes, modifiers, constToken, type, declarators, semicolonToken)
		{
		}
		
		public PositionToken ConstToken {
			get {
				return Pieces[2] as PositionToken;
			}
		}

		public DataType Type {
			get {
				return Pieces[3] as DataType;
			}
		}

		public VariableDeclaratorCollection Declarators {
			get {
				return Pieces[4] as VariableDeclaratorCollection;
			}
		}
		
		public PositionToken SemicolonToken {
			get {
				return Pieces[5] as PositionToken;
			}
		}
		
		public string Name {
			get {
				return "";
			}
		}
		
		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Declarators);
				return ret;
			}
		}

		public override VariableCollection DeclaredVariables {
			get {
				VariableCollection ret = new VariableCollection();
				foreach (VariableDeclarator vd in Declarators)
				{
					ret.Add(new Variable(vd.Name, Type, vd.Initializer, this.Parent as PieceOfCode));
				}
				return ret;
			}
		}

		public override VariableCollection ModifiedVariables {
			get {
				VariableCollection ret = new VariableCollection();
				foreach (VariableDeclarator vd in Declarators)
				{
					if (false == (vd.Initializer is MissingVariableInitializer))
					{
						ret.Add(new Variable(vd.Name, Type, vd.Initializer, this as PieceOfCode));
					}
				}
				return ret;
			}
		}
	}
}
