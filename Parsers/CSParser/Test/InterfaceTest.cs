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
	public class InterfaceTest : ParserTest
	{
		[Test]
		public void testWithMethods()
		{
			//Preparation
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  //Some comment.\r\n";
			fileString += "  interface TestInterface\r\n";
			fileString += "  {\r\n";
			fileString += "    void SomeMethod1();\r\n";
			fileString += "    void SomeMethod2(int parameter1, string parameter2);\r\n";
			fileString += "    void SomeMethod3(int parameter1, string parameter2, params int[] paramArray);\r\n";
			fileString += "  }\r\n";
			fileString += "}";

			//Execution
			CompilationUnit unit = ParseTestFile(fileString);

			//General tests
			Assertion.AssertEquals("Namespace count is wrong! ", 1, unit.Members.Count);
			Namespace ns = unit.Members[0] as Namespace;
			Assertion.AssertEquals("Namespace name is wrong! ", "TestNamespace", ns.Name);
			Assertion.AssertEquals("Member count is wrong! ", 1, ns.Members.Count);
			Assertion.Assert("Expected Interface but got something else!", ns.Members[0] is Interface);
			Interface interf = ns.Members[0] as Interface;
			Assertion.AssertEquals("Base interface count is wrong!", 0, interf.Base.TypeCollection.Count);
			Assertion.AssertEquals("Interface member count is wrong!", 3, interf.Members.Count);
			Assertion.Assert("Expected InterfaceMethodMember, but got something else!", interf.Members[0] is InterfaceMethod);
			InterfaceMethod method1 = interf.Members[0] as InterfaceMethod;
			Assertion.Assert("Expected InterfaceMethodMember, but got something else!", interf.Members[1] is InterfaceMethod);
			InterfaceMethod method2 = interf.Members[1] as InterfaceMethod;
			Assertion.Assert("Expected InterfaceMethodMember, but got something else!", interf.Members[2] is InterfaceMethod);
			InterfaceMethod method3 = interf.Members[2] as InterfaceMethod;

			//PrettyPrint test
			AssertEquals("Pretty print string was wrong!", fileString, unit.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 7, method3.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 5, method3.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 7, method3.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 81, method3.Position.EndCol);
			Assertion.AssertEquals("StartLine is wrong!", 6, method2.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 5, method2.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 6, method2.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 56, method2.Position.EndCol);
			Assertion.AssertEquals("StartLine is wrong!", 5, method1.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 5, method1.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 5, method1.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 23, method1.Position.EndCol);
			Assertion.AssertEquals("StartLine is wrong!", 3, interf.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, interf.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 8, interf.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 3, interf.Position.EndCol);
		}

		[Test]
		public void testWithProperties()
		{
			//Preparation
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  //Some comment.\r\n";
			fileString += "  interface TestInterface\r\n";
			fileString += "  {\r\n";
			fileString += "    void SomeProperty\r\n";
			fileString += "    {\r\n";
			fileString += "      get;\r\n";
			fileString += "    }\r\n";
			fileString += "    void SomeProperty2\r\n";
			fileString += "    {\r\n";
			fileString += "      set;\r\n";
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
			Assertion.Assert("Expected Interface but got something else!", ns.Members[0] is Interface);
			Interface interf = ns.Members[0] as Interface;
			Assertion.AssertEquals("Base interface count is wrong!", 0, interf.Base.TypeCollection.Count);
			Assertion.AssertEquals("Interface member count is wrong!", 2, interf.Members.Count);
			Assertion.Assert("Expected InterfaceProperty, but got something else!", interf.Members[0] is InterfaceProperty);
			InterfaceProperty prop1 = interf.Members[0] as InterfaceProperty;
			Assertion.AssertEquals("ContainsGet property of InterfaceAccessors is wrong!", true, prop1.Accessors.ContainsGet);
			Assertion.AssertEquals("ContainsSet property of InterfaceAccessors is wrong!", false, prop1.Accessors.ContainsSet);
			Assertion.Assert("Expected InterfaceProperty, but got something else!", interf.Members[1] is InterfaceProperty);
			InterfaceProperty prop2 = interf.Members[1] as InterfaceProperty;
			Assertion.AssertEquals("ContainsGet property of InterfaceAccessors is wrong!", false, prop2.Accessors.ContainsGet);
			Assertion.AssertEquals("ContainsSet property of InterfaceAccessors is wrong!", true, prop2.Accessors.ContainsSet);

			//PrettyPrint test
			AssertEquals("Pretty print string was wrong!", fileString, unit.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 3, interf.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, interf.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 13, interf.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 3, interf.Position.EndCol);
		}

		[Test]
		public void testWithEvents()
		{
			//Preparation
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  interface TestInterface\r\n";
			fileString += "  {\r\n";
			fileString += "    [SomeAttribute]\r\n";
			fileString += "    public event SomeEventHandler Changed1;\r\n";
			fileString += "    public new event SomeOtherEventHandler Changed2;\r\n";
			fileString += "  }\r\n";
			fileString += "}";

			//Execution
			CompilationUnit unit = ParseTestFile(fileString);

			//General tests
			Assertion.AssertEquals("Namespace count is wrong! ", 1, unit.Members.Count);
			Namespace ns = unit.Members[0] as Namespace;
			Assertion.AssertEquals("Namespace name is wrong! ", "TestNamespace", ns.Name);
			Assertion.AssertEquals("Member count is wrong! ", 1, ns.Members.Count);
			Assertion.Assert("Expected Interface but got something else!", ns.Members[0] is Interface);
			Interface interf = ns.Members[0] as Interface;
			Assertion.AssertEquals("Base interface count is wrong!", 0, interf.Base.TypeCollection.Count);
			Assertion.AssertEquals("Interface member count is wrong!", 2, interf.Members.Count);
			Assertion.Assert("Expected InterfaceEvent, but got something else!", interf.Members[0] is InterfaceEvent);
			InterfaceEvent event1 = interf.Members[0] as InterfaceEvent;
			Assertion.AssertEquals("Name of event 1 is wrong!", "Changed1", event1.Name);
			Assertion.AssertEquals("Type of event 1 is wrong!", "SomeEventHandler", event1.Type.Name);
			Assertion.Assert("Expected InterfaceEvent, but got something else!", interf.Members[1] is InterfaceEvent);
			InterfaceEvent event2 = interf.Members[1] as InterfaceEvent;
			Assertion.AssertEquals("Name of event 1 is wrong!", "Changed2", event2.Name);
			Assertion.AssertEquals("Type of event 1 is wrong!", "SomeOtherEventHandler", event2.Type.Name);

			//PrettyPrint test
			AssertEquals("Pretty print string was wrong!", fileString, unit.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 2, interf.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, interf.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 7, interf.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 3, interf.Position.EndCol);
			Assertion.AssertEquals("StartLine is wrong!", 4, event1.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 5, event1.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 5, event1.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 43, event1.Position.EndCol);
			Assertion.AssertEquals("StartLine is wrong!", 6, event2.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 5, event2.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 6, event2.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 52, event2.Position.EndCol);
		}

		[Test]
		public void testWithIndexers()
		{
			//Preparation
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  //Some comment.\r\n";
			fileString += "  interface TestInterface\r\n";
			fileString += "  {\r\n";
			fileString += "    Interface this[int index]{\r\n";
			fileString += "      get;\r\n";
			fileString += "      set;\r\n";
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
			Assertion.Assert("Expected Interface but got something else!", ns.Members[0] is Interface);
			Interface interf = ns.Members[0] as Interface;
			Assertion.AssertEquals("Base interface count is wrong!", 0, interf.Base.TypeCollection.Count);
			Assertion.AssertEquals("Interface member count is wrong!", 1, interf.Members.Count);
			Assertion.Assert("Expected InterfaceIndexer, but got something else!", interf.Members[0] is InterfaceIndexer);
			InterfaceIndexer index1 = interf.Members[0] as InterfaceIndexer;

			//PrettyPrint test
			AssertEquals("Pretty print string was wrong!", fileString, unit.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 3, interf.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, interf.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 9, interf.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 3, interf.Position.EndCol);
		}

		[Test]
		public void testWithIndexers2()
		{
			//Preparation
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  interface TestInterface\r\n";
			fileString += "  {\r\n";
			fileString += "    new Interface this[int index]{\r\n";
			fileString += "      get;\r\n";
			fileString += "      set;\r\n";
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
			Assertion.Assert("Expected Interface but got something else!", ns.Members[0] is Interface);
			Interface interf = ns.Members[0] as Interface;
			Assertion.AssertEquals("Base interface count is wrong!", 0, interf.Base.TypeCollection.Count);
			Assertion.AssertEquals("Interface member count is wrong!", 1, interf.Members.Count);
			Assertion.Assert("Expected InterfaceIndexer, but got something else!", interf.Members[0] is InterfaceIndexer);
			InterfaceIndexer index1 = interf.Members[0] as InterfaceIndexer;

			//PrettyPrint test
			AssertEquals("Pretty print string was wrong!", fileString, unit.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 2, interf.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, interf.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 8, interf.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 3, interf.Position.EndCol);
		}

		[Test]
		public void testSingleLineComment()
		{
			//Preparation
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  //Some comment.\r\n";
			fileString += "  interface TestInterface\r\n";
			fileString += "  {\r\n";
			fileString += "    void SomeMethod();\r\n";
			fileString += "  }\r\n";
			fileString += "}";

			//Execution
			CompilationUnit unit = ParseTestFile(fileString);

			//General tests
			Assertion.AssertEquals("Namespace count is wrong! ", 1, unit.Members.Count);
			Namespace ns = unit.Members[0] as Namespace;
			Assertion.AssertEquals("Namespace name is wrong! ", "TestNamespace", ns.Name);
			Assertion.AssertEquals("Member count is wrong! ", 1, ns.Members.Count);
			Assertion.Assert("Expected Interface but got something else!", ns.Members[0] is Interface);
			Interface interf = ns.Members[0] as Interface;
			Assertion.AssertEquals("Base interface count is wrong!", 0, interf.Base.TypeCollection.Count);
			Assertion.AssertEquals("Interface member count is wrong!", 1, interf.Members.Count);

			//PrettyPrint test
			AssertEquals("Pretty print string was wrong!", fileString, unit.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 3, interf.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, interf.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 6, interf.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 3, interf.Position.EndCol);
		}

		[Test]
		public void testMultiLineComment()
		{
			//Preparation
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  //Some comment.\r\n";
			fileString += "  //Line 2 of comment.\r\n";
			fileString += "  interface TestInterface{\r\n";
			fileString += "  }\r\n";
			fileString += "}";

			//Execution
			CompilationUnit unit = ParseTestFile(fileString);

			//General tests
			Assertion.AssertEquals("Namespace count is wrong! ", 1, unit.Members.Count);
			Namespace ns = unit.Members[0] as Namespace;
			Assertion.AssertEquals("Namespace name is wrong! ", "TestNamespace", ns.Name);
			Assertion.AssertEquals("Member count is wrong! ", 1, ns.Members.Count);
			Assertion.Assert("Expected Interface but got something else!", ns.Members[0] is Interface);
			Interface interf = ns.Members[0] as Interface;

			//PrettyPrint test
			AssertEquals("Pretty print string was wrong!", fileString, unit.Generate());
		}

		[Test]
		public void testWithBaseInterfaces()
		{
			//Preparation
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  //Some comment.\r\n";
			fileString += "  //Line 2 of comment.\r\n";
			fileString += "  interface TestClass : SomeInterface, SomeOtherInterface{\r\n";
			fileString += "  }\r\n";
			fileString += "}";

			//Execution
			CompilationUnit unit = ParseTestFile(fileString);

			//General tests
			Assertion.AssertEquals("Namespace count is wrong! ", 1, unit.Members.Count);
			Namespace ns = unit.Members[0] as Namespace;
			Assertion.AssertEquals("Namespace name is wrong! ", "TestNamespace", ns.Name);
			Assertion.AssertEquals("Member count is wrong! ", 1, ns.Members.Count);
			Assertion.Assert("Expected Interface but got something else!", ns.Members[0] is Interface);
			Interface interf = ns.Members[0] as Interface;
			Assertion.AssertEquals("Interface count is wrong!", 2, interf.Base.TypeCollection.Count);
			Assertion.AssertEquals("First interface name is wrong!", "SomeInterface", interf.Base.TypeCollection[0].Name);
			Assertion.AssertEquals("First interface name is wrong!", "SomeOtherInterface", interf.Base.TypeCollection[1].Name);

			//PrettyPrint test
			AssertEquals("Pretty print string was wrong!", fileString, unit.Generate());
		}

		[Test]
		public void testWithOneModifier()
		{
			//Preparation
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  /*Delimited comment.*/public interface TestInterface : SomeInterface, SomeOtherInterface{\r\n";
			fileString += "  }\r\n";
			fileString += "}";

			//Execution
			CompilationUnit unit = ParseTestFile(fileString);

			//General tests
			Assertion.AssertEquals("Namespace count is wrong! ", 1, unit.Members.Count);
			Namespace ns = unit.Members[0] as Namespace;
			Assertion.AssertEquals("Namespace name is wrong! ", "TestNamespace", ns.Name);
			Assertion.AssertEquals("Member count is wrong! ", 1, ns.Members.Count);
			Assertion.Assert("Expected Interface but got something else!", ns.Members[0] is Interface);
			Interface interf = ns.Members[0] as Interface;
			Assertion.AssertEquals("Modifier count is wrong!", 1, interf.Modifiers.Count);
			Assertion.AssertEquals("Modifier name is wrong!", "public", interf.Modifiers[0].Name);
			Assertion.AssertEquals("Interface count is wrong!", 2, interf.Base.TypeCollection.Count);
			Assertion.AssertEquals("First interface name is wrong!", "SomeInterface", interf.Base.TypeCollection[0].Name);
			Assertion.AssertEquals("First interface name is wrong!", "SomeOtherInterface",interf.Base.TypeCollection[1].Name);

			//PrettyPrint test
			AssertEquals("Pretty print string was wrong!", fileString, unit.Generate());
		}

		[Test]
		public void testWithAllModifiers()
		{
			//Preparation
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  //Some comment.\r\n";
			fileString += "  //Line 2 of comment.\r\n";
			fileString += "  new public protected internal private interface TestClass : SomeInterface, SomeOtherInterface{\r\n";
			fileString += "  }\r\n";
			fileString += "}";

			//Execution
			CompilationUnit unit = ParseTestFile(fileString);

			//General tests
			Assertion.AssertEquals("Namespace count is wrong! ", 1, unit.Members.Count);
			Namespace ns = unit.Members[0] as Namespace;
			Assertion.AssertEquals("Namespace name is wrong! ", "TestNamespace", ns.Name);
			Assertion.AssertEquals("Member count is wrong! ", 1, ns.Members.Count);
			Assertion.Assert("Expected Interface but got something else!", ns.Members[0] is Interface);
			Interface interf = ns.Members[0] as Interface;
			Assertion.AssertEquals("Interface count is wrong!", 2, interf.Base.TypeCollection.Count);
			Assertion.AssertEquals("First interface name is wrong!", "SomeInterface", interf.Base.TypeCollection[0].Name);
			Assertion.AssertEquals("First interface name is wrong!", "SomeOtherInterface", interf.Base.TypeCollection[1].Name);

			//Modifier tests
			Assertion.AssertEquals("Modifier name is wrong!", "private", interf.Modifiers[4].Name);
			Assertion.AssertEquals("Modifier count is wrong!", 5, interf.Modifiers.Count);
			Assertion.AssertEquals("Modifier name is wrong!", "new", interf.Modifiers[0].Name);
			Assertion.AssertEquals("Modifier name is wrong!", "public", interf.Modifiers[1].Name);
			Assertion.AssertEquals("Modifier name is wrong!", "protected", interf.Modifiers[2].Name);
			Assertion.AssertEquals("Modifier name is wrong!", "internal", interf.Modifiers[3].Name);
			Assertion.AssertEquals("Modifier name is wrong!", "private", interf.Modifiers[4].Name);

			//PrettyPrint test
			AssertEquals("Pretty print string was wrong!", fileString, unit.Generate());
		}
	}
}
