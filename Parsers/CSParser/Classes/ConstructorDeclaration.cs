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
	public class ConstructorDeclaration : ModifyablePieceOfCodeWithAttributes, ClassMember, StructMember
	{
		/// <summary>
		/// attributesopt constructor-modifiersopt identifier ( formal-parameter-listopt ) constructor-initializeropt constructor-body 
		/// </summary>
		public static ClassMember parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			ClassMember ret = new MissingClassMember();
			AttributeSectionCollection attributes = AttributeSectionCollection.parse(tokenizer);
			ModifierCollection modifiers = ModifierCollection.parse(tokenizer);
			Expression exp = QualifiedIdentifier.parse(tokenizer);
			if (exp is QualifiedIdentifier)
			{
				QualifiedIdentifier identifier = exp as QualifiedIdentifier;
				PositionToken openParensToken = tokenizer.CurrentToken;
				if (Token.OPEN_PARENS == openParensToken.Type)
				{
					tokenizer.nextToken(); // (
					FormalParameterList parameters = FormalParameterList.parse(tokenizer);
					parameters.checkNotMissing();
					PositionToken closeParensToken = tokenizer.checkToken(Token.CLOSE_PARENS);
					ConstructorInitializer initializer = ConstructorInitializer.parse(tokenizer);
					Statement body = Statement.parse(tokenizer);
					body.checkNotMissing();
					ret = new ConstructorDeclaration(attributes, modifiers, identifier, 
													 openParensToken, parameters, closeParensToken,
													 initializer, body);
				}
			}
			tokenizer.endBookmark(ret is MissingClassMember);
			return ret;
		}

		public ConstructorDeclaration()
		{
		}

		public ConstructorDeclaration(AttributeSectionCollection attributes, ModifierCollection modifiers, 
									  QualifiedIdentifier identifier, PositionToken openParensToken,
									  FormalParameterList parameters, PositionToken closeParensToken,
									  ConstructorInitializer initializer, Statement body) :
		base(attributes, modifiers, identifier, openParensToken, parameters, closeParensToken, initializer, body)
		{
		}

		public QualifiedIdentifier Identifier {
			get {
				return Pieces[2] as QualifiedIdentifier;
			}
		}

		public string Name {
			get {
				return Identifier.Name;
			}
		}

		public PositionToken OpenParensToken {
			get {
				return Pieces[3] as PositionToken;
			}
		}

		public FormalParameterList Parameters {
			get {
				return Pieces[4] as FormalParameterList;
			}
		}

		public PositionToken CloseParensToken {
			get {
				return Pieces[5] as PositionToken;
			}
		}

		public ConstructorInitializer Initializer {
			get {
				return Pieces[6] as ConstructorInitializer;
			}
		}

		public Statement Body {
			get {
				return Pieces[7] as Statement;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Body.Statements.Children);
				ret.Add(Parameters.Children);
				if (false == (Initializer is MissingConstructorInitializer))
				{
					ret.Add(Initializer);
				}
				return ret;
			}
		}

		public override VariableCollection DeclaredVariables {
			get {
				VariableCollection ret = new VariableCollection();
				foreach (FixedParameter param in Parameters.FixedParameters)
				{
					ret.Add(new Variable(param.Name, param.Type, new MissingVariableInitializer(), this));
				}
				if (false == (Parameters.ParameterArray is MissingParameterArray))
				{
					ret.Add(new Variable(Parameters.ParameterArray.Name, Parameters.ParameterArray.Type, new MissingVariableInitializer(), this));
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
	public class MissingConstructorDeclaration : ConstructorDeclaration
	{
	}
}
