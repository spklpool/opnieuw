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
	public class MethodDeclaration : ModifyablePieceOfCodeWithAttributes, ClassMember, StructMember
    {
#region static parging code
        /// <summary>
        /// attributesopt method-modifiersopt return-type member-name ( formal-parameter-listopt ) block
		/// attributesopt method-modifiersopt return-type member-name ( formal-parameter-listopt ) ;
		/// </summary>
		public static ClassMember parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			ClassMember ret = new MissingClassMember();
			AttributeSectionCollection attributes = AttributeSectionCollection.parse(tokenizer);
			ModifierCollection modifiers = ModifierCollection.parse(tokenizer);
			DataType type = DataType.parse(tokenizer);
			if (false == (type is MissingDataType))
			{
				Expression exp = QualifiedIdentifier.parse(tokenizer);
				if (exp is QualifiedIdentifier)
				{
					QualifiedIdentifier identifier = exp as QualifiedIdentifier;
					if (Token.OPEN_PARENS == tokenizer.CurrentToken.Type)
					{
						PositionToken openParensToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // (
						FormalParameterList parameters = FormalParameterList.parse(tokenizer);
						parameters.checkNotMissing();
						PositionToken closeParensToken = tokenizer.checkToken(Token.CLOSE_PARENS);
						Statement body = Statement.parse(tokenizer);
						body.checkNotMissing();
						ret = new MethodDeclaration(attributes, modifiers, type, identifier, 
													openParensToken, parameters, closeParensToken,
													body);
						MethodDeclaration methodRet = ret as MethodDeclaration;
						if (attributes.Count > 0)
						{
							methodRet.LeadingCharacters = attributes.LeadingCharacters;
						}
						else if (modifiers.Count > 0)
						{
							methodRet.LeadingCharacters = modifiers.LeadingCharacters;
						}
						else
						{
							methodRet.LeadingCharacters = type.LeadingCharacters;
						}
					}
				}
			}
			tokenizer.endBookmark(ret is MissingClassMember);
			return ret;
        }
#endregion

#region constructors
        public MethodDeclaration() :
		base(new AttributeSectionCollection(), new ModifierCollection(), 
			 new MissingDataType(), new MissingQualifiedIdentifier(), 
			 PositionToken.Missing, new FormalParameterList(), 
			 PositionToken.Missing)
		{
		}

		public MethodDeclaration(AttributeSectionCollection attributes, ModifierCollection modifiers, 
								 DataType type, QualifiedIdentifier identifier, 
								 PositionToken openParensToken, FormalParameterList parameters, 
								 PositionToken closeParensToken, Statement body) :
		base(attributes, modifiers, type, identifier, openParensToken, parameters, closeParensToken, body)
		{
		}

		public MethodDeclaration(DataType type, String name, 
								 FormalParameterList parameters, 
								 Statement body) :
		base(new AttributeSectionCollection(), new ModifierCollection(), type,
			 QualifiedIdentifier.parse(name),
			 new PositionToken(Position.Missing, "(", Token.OPEN_PARENS), 
			 parameters, 
			 new PositionToken(Position.Missing, ")", Token.CLOSE_PARENS), 			 
			 body)
		{
            Format();
        }

        public MethodDeclaration(Attribute attribute, DataType type, String name,
                                 FormalParameterList parameters,
                                 Statement body) :
		base(new AttributeSectionCollection(), new ModifierCollection(), type,
			 QualifiedIdentifier.parse(name),
			 new PositionToken(Position.Missing, "(", Token.OPEN_PARENS), 
			 parameters, 
			 new PositionToken(Position.Missing, ")", Token.CLOSE_PARENS), 			 
			 body)
        {
            AttributeCollection attributes = new AttributeCollection();
            attributes.Add(attribute);
            AttributeSection section = new AttributeSection(attributes);
            AttributeSections.Add(section);
            Format();
        }
#endregion

        public override void Reindent(int newIndent)
        {
            Format();
            String generatedString = Generate();
            String reindentedString = GenericBlockOfCode.Reindent(generatedString, newIndent, 4);
			MethodDeclaration clone = CompilationUnit.parseClassMember(reindentedString) as MethodDeclaration;
            CloneFormat(clone);
        }

        public DataType Type {
			get {
				return Pieces[2] as DataType;
			}
		}

		public QualifiedIdentifier Identifier {
			get {
				return Pieces[3] as QualifiedIdentifier;
			}
		}

		public string Name {
			get {
				return Identifier.Name;
			}
		}
		
		public PositionToken OpenParensToken {
			get {
				return Pieces[4] as PositionToken;
			}
		}

		public FormalParameterList Parameters {
			get {
				return Pieces[5] as FormalParameterList;
			}
		}
		
		public PositionToken CloseParensToken {
			get {
				return Pieces[6] as PositionToken;
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
				return Name;
			}
		}
	}

	public class MissingMethodDeclaration : MethodDeclaration
	{
	}
}
