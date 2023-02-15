#region Copyright (C) Pierre Boudreau
//This file is part of the Opnieuw C# parser.
//
//pierre.boudreau@alphacentauri.biz
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
using NUnit.Framework;

using Opnieuw.Framework;
using Opnieuw.Parsers.CSParser;

namespace Opnieuw.Parsers.CSParser.Test
{
	[TestFixture]
	public class PieceOfCodeTest : TokenizerTestBase
	{
		[Test]
		public void testBreadCrumbs1()
		{
            CompilationUnit unit = new CompilationUnit(4, new UsingDirectiveCollection(), new AttributeSectionCollection(), new NamespaceMemberCollection());
            NamespaceMemberCollection members = new NamespaceMemberCollection();
            ClassMemberCollection classMembers = new ClassMemberCollection();
            BlockStatement statement = new BlockStatement();
            statement.Statements.Add(new EmptyStatement());
            statement.Statements.Add(new IfStatement(new MissingExpression(), new BlockStatement(new StatementCollection(new EmptyStatement())), new EmptyStatement()));
            statement.Statements.Add(new EmptyStatement());
            MethodDeclaration method = new MethodDeclaration(DataType.parse("void"), "SomeMethod", new MissingFormalParameterList(), statement);
            classMembers.Add(method);
            members.Add(new Class("SomeClass", classMembers));
            unit.Members.Add(new Namespace(QualifiedIdentifier.parse("SomeNamespace"), members));
            unit.PropagateUp();
            TrailOfBreadCrumbs crumbs = method.Crumbs;
            unit.Format();

            CompilationUnit unitClone = CompilationUnit.parse(unit.Generate());
            FundamentalPieceOfCode piece = unitClone.FromTrailOfBreadCrumbs(crumbs);
            MethodDeclaration foundMethod = piece as MethodDeclaration;
            String result1 = foundMethod.Name;
            AssertEquals(method.Name, foundMethod.Name);

            CompilationUnit unitClone2 = CompilationUnit.parse(unitClone.Generate());
            FundamentalPieceOfCode piece2 = unitClone2.FromTrailOfBreadCrumbs((unitClone.Namespaces[0].Classes[0].Methods[0].Body.Statements[1] as IfStatement).IfPartStatement.Statements[0].Crumbs);
            Statement foundStatement2 = piece2 as Statement;
            Assert(foundStatement2 is EmptyStatement);
		}

        [Test]
        public void testCloneFormat()
        {
            String codeString1 = "";
            codeString1 += "namespace ns\r\n";
            codeString1 += "{\r\n";
            codeString1 += "  class cls\r\n";
            codeString1 += "  {\r\n";
            codeString1 += "    public method1()\r\n";
            codeString1 += "    {\r\n";
            codeString1 += "    }\r\n";
            codeString1 += "  }\r\n";
            codeString1 += "}\r\n";

            String codeString2 = "";
            codeString2 += "namespace ns";
            codeString2 += "{";
            codeString2 += "class cls";
            codeString2 += "{";
            codeString2 += "public method1()";
            codeString2 += "{";
            codeString2 += "}";
            codeString2 += "}";
            codeString2 += "}";

            CompilationUnit unit1 = CompilationUnit.parse(codeString1);
            CompilationUnit unit2 = CompilationUnit.parse(codeString2);

            unit2.CloneFormat(unit1);

            AssertEquals("\r\n    ", (((unit2.Members[0] as Namespace).Members[0] as Class).Members[0] as PieceOfCode).LeadingCharacters);
        }
	}
}