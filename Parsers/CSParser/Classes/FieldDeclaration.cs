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
	public class FieldDeclaration : ModifyablePieceOfCodeWithAttributes, ClassMember, StructMember
	{
		public static ClassMember parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			ClassMember ret = new MissingClassMember();
			AttributeSectionCollection attributes = AttributeSectionCollection.parse(tokenizer);
			ModifierCollection modifiers = ModifierCollection.parse(tokenizer);
			DataType type = DataType.parse(tokenizer);
			if (false == (type is MissingDataType))
			{
				VariableDeclaratorCollection declarators = VariableDeclaratorCollection.parse(tokenizer);
				if (declarators.Count > 0)
				{
					if (Token.SEMICOLON == tokenizer.CurrentToken.Type)
					{
						PositionToken semicolonToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // ;
                        ret = new FieldDeclaration(attributes, modifiers, type, declarators, semicolonToken);
					}
				}
			}
			tokenizer.endBookmark(ret is MissingClassMember);
			return ret;
		}

		public FieldDeclaration()
		{
			Pieces.Add(new AttributeSectionCollection());
			Pieces.Add(new ModifierCollection());
			Pieces.Add(new MissingDataType());
			Pieces.Add(new VariableDeclaratorCollection());
			Pieces.Add(PositionToken.Missing);
		}

		public FieldDeclaration(AttributeSectionCollection attributes, ModifierCollection modifiers, 
								DataType type, VariableDeclaratorCollection declarators, PositionToken semicolonToken) :
		base(attributes, modifiers, type, declarators, semicolonToken)
		{
		}

		protected string m_Name = "";
		public string Name {
			get {
				return m_Name;
			}
		}

		public DataType Type {
			get {
				return Pieces[2] as DataType;
			}
		}

		public VariableDeclaratorCollection Declarators {
			get {
				return Pieces[3] as VariableDeclaratorCollection;
			}
		}
		
		public PositionToken SemicolonToken {
			get {
				return Pieces[4] as PositionToken;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Declarators.Children);
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
						ret.Add(new Variable(vd.Name, Type, vd.Initializer, this.Parent as PieceOfCode));
					}
				}
				return ret;
			}
		}
	}

	public class MissingFieldDeclaration : FieldDeclaration
	{
	}
}
