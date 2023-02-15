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
	/// attributesopt operator-modifiers operator-declarator operator-body 
	/// </summary>
	public class OperatorDeclaration : ModifyablePieceOfCodeWithAttributes, ClassMember, StructMember
	{
		public static ClassMember parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			ClassMember ret = new MissingClassMember();
			AttributeSectionCollection attributes = AttributeSectionCollection.parse(tokenizer);
			ModifierCollection modifiers = ModifierCollection.parse(tokenizer);
			OperatorDeclarator declarator = OperatorDeclarator.parse(tokenizer);
			if (false == (declarator is MissingOperatorDeclarator))
			{
				Statement statement = Statement.parse(tokenizer);
				ret = new OperatorDeclaration(attributes, modifiers, declarator, statement);
			}
			tokenizer.endBookmark(ret is MissingClassMember);
			return ret;
		}

		public OperatorDeclaration() :
		base(new AttributeSectionCollection(), new ModifierCollection(), 
			 new MissingOperatorDeclarator(), new MissingStatement())
		{
		}

		public OperatorDeclaration(AttributeSectionCollection attributes, ModifierCollection modifiers, 
								   OperatorDeclarator declarator, Statement body) :
		base(attributes, modifiers, declarator, body)
		{
		}

		public string Name {
			get {
				return "";
			}
		}

		public OperatorDeclarator Declarator {
			get {
				return Pieces[2] as OperatorDeclarator;
			}
		}

		public Statement Body {
			get {
				return Pieces[3] as Statement;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Body);
				ret.Add(Declarator);
				return ret;
			}
		}

		public override ExpressionCollection Expressions {
			get {
				return Body.Expressions;	
			}
		}

		public override VariableCollection DeclaredVariables {
			get {
				VariableCollection ret = new VariableCollection();
				if (false == (Declarator.Type1 is MissingDataType))
				{
					ret.Add(new Variable(Declarator.Identifier1.Name, Declarator.Type1, new MissingVariableInitializer(), this));
				}
				if (false == (Declarator.Type2 is MissingDataType))
				{
					ret.Add(new Variable(Declarator.Identifier2.Name, Declarator.Type2, new MissingVariableInitializer(), this));
				}
				return ret;
			}
		}
		
		public override VariableCollection ReferencedVariables {
			get {
				return Body.ReferencedVariables;
			}
		}

		public override VariableCollection ModifiedVariables {
			get {
				return Body.ModifiedVariables;
			}
		}
	}

	public class MissingOperatorDeclaration : OperatorDeclaration
	{
	}
}
