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
using System.Collections;
using Opnieuw.Framework;

namespace Opnieuw.Parsers.CSParser
{
	public class FormattingPropertiesCollection
    {
        private static FormattingPropertiesCollection instance = null;
        private Hashtable data = new Hashtable();
		
		private FormattingPropertiesCollection()
		{
            //identifier = variable-initializer
            Add(Type.GetType("Opnieuw.Parsers.CSParser.VariableDeclarator"), new String[3] { "", " ", " " });
            //unary-expression assignment-operator expression
            Add(Type.GetType("Opnieuw.Parsers.CSParser.AssignmentExpression"), new String[3] { "", " ", " " });
            Add(Type.GetType("Opnieuw.Parsers.CSParser.AdditionAssignmentExpression"), new String[3] { "", " ", " " });
            //type local-variable-declarators
            Add(Type.GetType("Opnieuw.Parsers.CSParser.LocalVariableDeclaration"), new String[2] { "", " " });
            //using namespace-name ;
            Add(Type.GetType("Opnieuw.Parsers.CSParser.UsingNamespaceDirective"), new String[5] { "\r\n", "", "", " ", "" });
            //attributesopt parameter-modifieropt type identifier
            Add(Type.GetType("Opnieuw.Parsers.CSParser.FixedParameter"), new String[4] { " ", " ", " ", " " });
            //;
            Add(Type.GetType("Opnieuw.Parsers.CSParser.EmptyStatement"), new String[1] { "\r\n" });
            //identifier : statement
            Add(Type.GetType("Opnieuw.Parsers.CSParser.LabeledStatement"), new String[3] { "\r\n", "", "\r\n" });
            //using ( local-variable-declaration ) embedded-statement
            //using ( expression ) embedded-statement
            Add(Type.GetType("Opnieuw.Parsers.CSParser.UsingStatement"), new String[6] { "\r\n", " ", "", "", "", " " });
            //local-variable-declaration ;
            //local-constant-declaration ; 
            Add(Type.GetType("Opnieuw.Parsers.CSParser.DeclarationStatement"), new String[3] { "\r\n", "\r\n", "" });
            //checked block
            Add(Type.GetType("Opnieuw.Parsers.CSParser.CheckedStatement"), new String[2] { "\r\n", "\r\n" });
            //while ( boolean-expression ) embedded-statement
            Add(Type.GetType("Opnieuw.Parsers.CSParser.WhileStatement"), new String[5] { "\r\n", " ", "", "", "\r\n" });
            //do embedded-statement while ( boolean-expression ) ;
            Add(Type.GetType("Opnieuw.Parsers.CSParser.DoStatement"), new String[7] { "\r\n", "\r\n", "\r\n", " ", "", "", "" });
            //if ( boolean-expression ) embedded-statement else embedded-statement 
            Add(Type.GetType("Opnieuw.Parsers.CSParser.IfStatement"), new String[7] { "\r\n", " ", "", "", "\r\n", "\r\n", "\r\n" });
            //foreach ( type identifier in expression ) embedded-statement
            Add(Type.GetType("Opnieuw.Parsers.CSParser.ForeachStatement"), new String[8] { "\r\n", " ", "", " ", " ", " ", "", "\r\n" });
            //for ( for-initializer ; for-condition ; for-iterator ) embedded-statement
            Add(Type.GetType("Opnieuw.Parsers.CSParser.ForStatement"), new String[9] { "\r\n", " ", "", "", " ", "", " ", "", "\r\n" });
            //{ statement-listopt }
            Add(Type.GetType("Opnieuw.Parsers.CSParser.BlockStatement"), new String[3] { "\r\n", "", "\r\n" });
            //return expression;
            Add(Type.GetType("Opnieuw.Parsers.CSParser.ReturnStatement"), new String[3] { "\r\n", " ", "" });
            //try block catch-clauses finally-clause 
            Add(Type.GetType("Opnieuw.Parsers.CSParser.TryStatement"), new String[4] { "\r\n", "", "", "" });
            //catch ( class-type identifieropt ) block 
            Add(Type.GetType("Opnieuw.Parsers.CSParser.CatchClause"), new String[6] { " ", "", "", " ", "", " " });
            //finally block 
            Add(Type.GetType("Opnieuw.Parsers.CSParser.FinallyClause"), new String[2] { " ", " " });
            //attributesopt get ;
			Add(Type.GetType("Opnieuw.Parsers.CSParser.InterfaceGetAccessor"), new String[3] { "\r\n", "\r\n", "" });
			//attributesopt set ;
			Add(Type.GetType("Opnieuw.Parsers.CSParser.InterfaceSetAccessor"), new String[3] { "\r\n", "\r\n", "" });
            //namespace qualified-identifier { using-directivesopt namespace-member-declarationsopt } ;opt
            Add(Type.GetType("Opnieuw.Parsers.CSParser.Namespace"), new String[10] {"\r\n", "\n\r", "\r\n", " ", "\r\n", "\r\n", "\r\n", "\r\n", "\r\n", "" });
            //attributesopt class-modifiersopt class identifier class-baseopt { class-member-declarationsopt } ;opt 
            Add(Type.GetType("Opnieuw.Parsers.CSParser.Class"), new String[9] {"\r\n", "\r\n", "\r\n", " ", "", "\r\n", "", "\r\n", ""});
            //attributesopt method-modifiersopt return-type member-name ( formal-parameter-listopt ) block
            Add(Type.GetType("Opnieuw.Parsers.CSParser.MethodDeclaration"), new String[8] { "\r\n", "\r\n", "\r\n", " ", "", "", "", "\r\n" });
            //attributesopt property-modifiersopt type member-name { accessor-declarations }
            Add(Type.GetType("Opnieuw.Parsers.CSParser.PropertyDeclaration"), new String[7] { "\r\n", "\r\n", "\r\n", " ", " ", "\r\n", "\r\n" });
            //get-accessor-declaration set-accessor-declarationopt
            //set-accessor-declaration get-accessor-declarationopt
            Add(Type.GetType("Opnieuw.Parsers.CSParser.AccessorDeclarations"), new String[2] { "\r\n", "\r\n" });
            //attributesopt get accessor-body
            Add(Type.GetType("Opnieuw.Parsers.CSParser.GetAccessorDeclaration"), new String[3] { "\r\n", "\r\n", " " });
            //attributesopt set accessor-body
            Add(Type.GetType("Opnieuw.Parsers.CSParser.SetAccessorDeclaration"), new String[3] { "\r\n", "\r\n", " " });
            /// attributesopt indexer-modifiersopt type this [ formal-parameter-list ] { accessor-declarations } 
            /// attributesopt indexer-modifiersopt type interface-type . this [ formal-parameter-list ]  { accessor-declarations } 
            Add(Type.GetType("Opnieuw.Parsers.CSParser.IndexerDeclaration"), new String[9] { "\r\n", "\r\n", "\r\n", " ", "", "\r\n", "", "\r\n", "" });
            //attributesopt interface-modifiersopt interface identifier interface-baseopt { interface-member-declarationsopt } ;opt 
            Add(Type.GetType("Opnieuw.Parsers.CSParser.Interface"), new String[9] { "\r\n", "\r\n", "\r\n", " ", "", "\r\n", "", "\r\n", "" });
            //attributesopt newopt return-type identifier ( formal-parameter-listopt ) ;
            Add(Type.GetType("Opnieuw.Parsers.CSParser.InterfaceMethod"), new String[8] { "\r\n", "\r\n", "\r\n", " ", "", "", "", "" });
            //attributesopt newopt type identifier { interface-accessors }
            Add(Type.GetType("Opnieuw.Parsers.CSParser.InterfaceProperty"), new String[7] { "\r\n", "\r\n", "\r\n", " ", " ", "", "\r\n" });
        }

        public static FormattingPropertiesCollection Instance {
            get {
                if (instance == null) {
                    instance = new FormattingPropertiesCollection();
                }
                return instance;
            }
        }

        private void Add(Type type, String[] tokenLeadingCharacters)
        {
            data.Add(type, new FormattingProperties(type, tokenLeadingCharacters));
        }

		public FormattingProperties this[Type type] {
			get {
				return data[type] as FormattingProperties;
			}
			set {
				data[type] = value;
			}
		}
	}
}
