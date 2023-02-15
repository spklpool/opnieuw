#region Copyright (C) Pierre Boudreau
//This file is part of the Opnieuw project.
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
using System.IO;
using Opnieuw.Framework;
using Opnieuw.Parsers.CSParser;

namespace Opnieuw.Parsers.CSParser.Test
{
	[TestFixture]
	public class StructTest : ParserTest
	{
		[Test]
		public void testSingleLineComment()
		{
			//Preparation
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  //Some comment.\r\n";
			fileString += "  struct TestStruct\r\n";
			fileString += "  {\r\n";
			fileString += "    public TestStruct()\r\n";
			fileString += "    {\r\n";
			fileString += "    }\r\n";
			fileString += "    public void SomeMethod()\r\n";
			fileString += "    {\r\n";
			fileString += "    }\r\n";
			fileString += "  }\r\n";
			fileString += "}";

			//Execution
			CompilationUnit unit = ParseTestFile(fileString);

			//General tests
			Assertion.AssertEquals("Namespace count is wrong! ", 1, unit.Members.Count);
			Namespace ns = unit.Members[0] as Namespace;
			Assertion.AssertEquals("Namespace name is wrong! ", "TestNamespace", ns.Name);
			Assertion.AssertEquals("Member count is wrong! ", 1, ns.Members.Count);
			Assertion.Assert("Expected StructDeclaration but got something else!", ns.Members[0] is StructDeclaration);
			StructDeclaration strct = ns.Members[0] as StructDeclaration;

			//PrettyPrint test
			AssertEquals("Pretty print string was wrong!", fileString, ns.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 3, strct.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, strct.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 11, strct.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 3, strct.Position.EndCol);
		}

		[Test]
		public void testMultiLineComment()
		{
			//Preparation
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  //Some comment.\r\n";
			fileString += "  //Line 2 of comment.\r\n";
			fileString += "  struct TestStruct{\r\n";
			fileString += "  }\r\n";
			fileString += "}";

			//Execution
			CompilationUnit unit = ParseTestFile(fileString);

			//General tests
			Assertion.AssertEquals("Namespace count is wrong! ", 1, unit.Members.Count);
			Namespace ns = unit.Members[0] as Namespace;
			Assertion.AssertEquals("Namespace name is wrong! ", "TestNamespace", ns.Name);
			Assertion.AssertEquals("Member count is wrong! ", 1, ns.Members.Count);
			Assertion.Assert("Expected StructDeclaration but got something else!", ns.Members[0] is StructDeclaration);
			StructDeclaration strct = ns.Members[0] as StructDeclaration;

			//PrettyPrint test
			AssertEquals("Pretty print string was wrong!", fileString, ns.Generate());
		}

		[Test]
		public void testWithInterfaces()
		{
			//Preparation
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  //Some comment.\r\n";
			fileString += "  //Line 2 of comment.\r\n";
			fileString += "  struct TestStruct : SomeInterface, SomeOtherInterface{\r\n";
			fileString += "  }\r\n";
			fileString += "}";

			//Execution
			CompilationUnit unit = ParseTestFile(fileString);

			//General tests
			Assertion.AssertEquals("Namespace count is wrong! ", 1, unit.Members.Count);
			Namespace ns = unit.Members[0] as Namespace;
			Assertion.AssertEquals("Namespace name is wrong! ", "TestNamespace", ns.Name);
			Assertion.AssertEquals("Member count is wrong! ", 1, ns.Members.Count);
			Assertion.Assert("Expected StructDeclaration but got something else!", ns.Members[0] is StructDeclaration);
			StructDeclaration strct = ns.Members[0] as StructDeclaration;
			Assertion.AssertEquals("Interface count is wrong!", 2, strct.Base.TypeCollection.Count);
			Assertion.AssertEquals("First interface name is wrong!", "SomeInterface", strct.Base.TypeCollection[0].Name);
			Assertion.AssertEquals("First interface name is wrong!", "SomeOtherInterface", strct.Base.TypeCollection[1].Name);

			//PrettyPrint test
			AssertEquals("Pretty print string was wrong!", fileString, ns.Generate());
		}

		[Test]
		public void testWithOneModifier()
		{
			//Preparation
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  //Some comment.\r\n";
			fileString += "  //Line 2 of comment.\r\n";
			fileString += "  public struct TestStruct : SomeInterface, SomeOtherInterface{\r\n";
			fileString += "  }\r\n";
			fileString += "}\r\n";

			//Execution
			CompilationUnit unit = ParseTestFile(fileString);

			//General tests
			Assertion.AssertEquals("Namespace count is wrong! ", 1, unit.Members.Count);
			Namespace ns = unit.Members[0] as Namespace;
			Assertion.AssertEquals("Namespace name is wrong! ", "TestNamespace", ns.Name);
			Assertion.AssertEquals("Member count is wrong! ", 1, ns.Members.Count);
			Assertion.Assert("Expected StructDeclaration but got something else!", ns.Members[0] is StructDeclaration);
			StructDeclaration strct = ns.Members[0] as StructDeclaration;
			Assertion.AssertEquals("Modifier count is wrong!", 1, strct.Modifiers.Count);
			Modifier mod1 = strct.Modifiers[0];
			Assertion.AssertEquals("Modifier name is wrong!", "public", mod1.Name);
			Assertion.AssertEquals("Interface count is wrong!", 2, strct.Base.TypeCollection.Count);
			Assertion.AssertEquals("First interface name is wrong!", "SomeInterface", strct.Base.TypeCollection[0].Name);
			Assertion.AssertEquals("First interface name is wrong!", "SomeOtherInterface", strct.Base.TypeCollection[1].Name);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 4, mod1.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, mod1.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 4, mod1.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 8, mod1.Position.EndCol);
		}

		[Test]
		public void testWithAllModifiers()
		{
			//Preparation
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  //Some comment.\r\n";
			fileString += "  //Line 2 of comment.\r\n";
			fileString += "  new public protected internal private struct TestStruct : SomeInterface, SomeOtherInterface{\r\n";
			fileString += "  }\r\n";
			fileString += "}";

			//Execution
			CompilationUnit unit = ParseTestFile(fileString);

			//General tests
			Assertion.AssertEquals("Namespace count is wrong! ", 1, unit.Members.Count);
			Namespace ns = unit.Members[0] as Namespace;
			Assertion.AssertEquals("Namespace name is wrong! ", "TestNamespace", ns.Name);
			Assertion.AssertEquals("Member count is wrong! ", 1, ns.Members.Count);
			Assertion.Assert("Expected StructDeclaration but got something else!", ns.Members[0] is StructDeclaration);
			StructDeclaration strct = ns.Members[0] as StructDeclaration;
			Assertion.AssertEquals("Interface count is wrong!", 2, strct.Base.TypeCollection.Count);
			Assertion.AssertEquals("First interface name is wrong!", "SomeInterface", strct.Base.TypeCollection[0].Name);
			Assertion.AssertEquals("First interface name is wrong!", "SomeOtherInterface", strct.Base.TypeCollection[1].Name);

			//Modifier tests
			Assertion.AssertEquals("Modifier count is wrong!", 5, strct.Modifiers.Count);
			Assertion.AssertEquals("Modifier name is wrong!", "new", strct.Modifiers[0].Name);
			Assertion.AssertEquals("Modifier name is wrong!", "public", strct.Modifiers[1].Name);
			Assertion.AssertEquals("Modifier name is wrong!", "protected", strct.Modifiers[2].Name);
			Assertion.AssertEquals("Modifier name is wrong!", "internal", strct.Modifiers[3].Name);
			Assertion.AssertEquals("Modifier name is wrong!", "private", strct.Modifiers[4].Name);

			//PrettyPrint test
			AssertEquals("Pretty print string was wrong!", fileString, ns.Generate());
		}
	}
}
