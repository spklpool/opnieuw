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
	public class ClassTest : TokenizerTestBase
	{
		[Test]
		public void testSingleLineComment()
		{
			//Preparation
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  //Some comment.\r\n";
			fileString += "  class TestClass {\r\n";
			fileString += "    public TestClass()  {\r\n";
			fileString += "    }\r\n";
			fileString += "    public void SomeMethod() {\r\n";
			fileString += "    }\r\n";
			fileString += "  }\r\n";
			fileString += "}";

			//Execution
			CompilationUnit unit = ParseTestFile(fileString);

			//General tests
			AssertEquals("Namespace count is wrong! ", 1, unit.Members.Count);
			Namespace ns = unit.Members[0] as Namespace;
			AssertEquals("Namespace name is wrong! ", "TestNamespace", ns.Name);
			AssertEquals("Member count is wrong! ", 1, ns.Members.Count);
			Assert("Expected Class but got something else!", ns.Members[0] is Class);
			Class cls = ns.Members[0] as Class;
			AssertEquals("Interface count is wrong!", 0, cls.Interfaces.Count);
			Assert("Interface count is wrong!", cls.Base.Base is MissingDataType);
			AssertEquals("Method count is wrong!", 1, cls.Methods.Count);
			AssertEquals("Field count is wrong!", 0, cls.Fields.Count);
			AssertEquals("Constant count is wrong!", 0, cls.Constants.Count);
			AssertEquals("Event count is wrong!", 0, cls.Events.Count);
			AssertEquals("Operators count is wrong!", 0, cls.Operators.Count);
			AssertEquals("Property count is wrong!", 0, cls.Properties.Count);
			AssertEquals("PrettyPrint is wrong!", fileString, unit.Generate());

			//Position tests
			AssertEquals("StartLine is wrong!", 3, cls.Position.StartLine);
			AssertEquals("StartCol is wrong!", 3, cls.Position.StartCol);
			AssertEquals("EndLine is wrong!", 8, cls.Position.EndLine);
			AssertEquals("EndCol is wrong!", 3, cls.Position.EndCol);
		}

		[Test]
		public void testDelimitedComment()
		{
			//Preparation
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  /*Some comment.\r\n";
			fileString += "   *Line 2 of comment.\r\n";
			fileString += "   */\r\n";
			fileString += "  class TestClass{\r\n";
			fileString += "  }\r\n";
			fileString += "}";

			//Execution
			CompilationUnit unit = ParseTestFile(fileString);

			//General tests
			AssertEquals("Namespace count is wrong! ", 1, unit.Members.Count);
			Namespace ns = unit.Members[0] as Namespace;
			AssertEquals("Namespace name is wrong! ", "TestNamespace", ns.Name);
			AssertEquals("Member count is wrong! ", 1, ns.Members.Count);
			Assert("Expected Class but got something else!", ns.Members[0] is Class);
			Class cls = ns.Members[0] as Class;
			AssertEquals("PrettyPrint is wrong!", fileString, unit.Generate());
		}

		[Test]
		public void testDelimitedCommentOnSameLine()
		{
			//Preparation
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  /*Some comment.*/class TestClass{\r\n";
			fileString += "  }\r\n";
			fileString += "}";

			//Execution
			CompilationUnit unit = ParseTestFile(fileString);

			//General tests
			AssertEquals("Namespace count is wrong! ", 1, unit.Members.Count);
			Namespace ns = unit.Members[0] as Namespace;
			AssertEquals("Namespace name is wrong! ", "TestNamespace", ns.Name);
			AssertEquals("Member count is wrong! ", 1, ns.Members.Count);
			Assert("Expected Class but got something else!", ns.Members[0] is Class);
			Class cls = ns.Members[0] as Class;
			AssertEquals("PrettyPrint is wrong!", fileString, unit.Generate());
		}

		[Test]
		public void testWithBaseClass()
		{
			//Preparation
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  //Some comment.\r\n";
			fileString += "  //Line 2 of comment.\r\n";
			fileString += "  class TestClass : TheBase{\r\n";
			fileString += "  }\r\n";
			fileString += "}";

			//Execution
			CompilationUnit unit = ParseTestFile(fileString);

			//General tests
			AssertEquals("Namespace count is wrong! ", 1, unit.Members.Count);
			Namespace ns = unit.Members[0] as Namespace;
			
			//PrettyPrint tests
			AssertEquals("PrettyPrint is wrong!", fileString, ns.Generate());
			
			AssertEquals("Namespace name is wrong! ", "TestNamespace", ns.Name);
			AssertEquals("Member count is wrong! ", 1, ns.Members.Count);
			Assert("Expected Class but got something else!", ns.Members[0] is Class);
			Class cls = ns.Members[0] as Class;
			AssertEquals("Base is wrong!", "TheBase", cls.Base.Base.Name);
			AssertEquals("Interface count is wrong!", 0, cls.Interfaces.Count);
		}

		[Test]
		public void testWithBaseAndInterfaces()
		{
			//Preparation
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  //Some comment.\r\n";
			fileString += "  //Line 2 of comment.\r\n";
			fileString += "  class TestClass : TheBase, SomeNamespace.SomeInterface, SomeOtherInterface{\r\n";
			fileString += "  }\r\n";
			fileString += "}\r\n";

			//Execution
			CompilationUnit unit = ParseTestFile(fileString);

			//General tests
			AssertEquals("Namespace count is wrong! ", 1, unit.Members.Count);
			Namespace ns = unit.Members[0] as Namespace;
			AssertEquals("Namespace name is wrong! ", "TestNamespace", ns.Name);
			AssertEquals("Member count is wrong! ", 1, ns.Members.Count);
			Assert("Expected Class but got something else!", ns.Members[0] is Class);
			Class cls = ns.Members[0] as Class;
			AssertEquals("Base is wrong!", "TheBase", cls.Base.Base.Name);
			DataType classBase = cls.Base.Base;
			AssertEquals("Interface count is wrong!", 2, cls.Base.BaseInterfaces.Count);
			AssertEquals("First interface name is wrong!", "SomeNamespace.SomeInterface",cls.Base.BaseInterfaces[0].Name);
			AssertEquals("First interface name is wrong!", "SomeOtherInterface",cls.Base.BaseInterfaces[1].Name);

			//Position tests
			AssertEquals("StartLine is wrong!", 4, classBase.Position.StartLine);
			AssertEquals("StartCol is wrong!", 21, classBase.Position.StartCol);
			AssertEquals("EndLine is wrong!", 4, classBase.Position.EndLine);
			AssertEquals("EndCol is wrong!", 27, classBase.Position.EndCol);

			AssertEquals("StartLine is wrong!", 4, cls.Base.BaseInterfaces[0].Position.StartLine);
			AssertEquals("StartCol is wrong!", 30, cls.Base.BaseInterfaces[0].Position.StartCol);
			AssertEquals("EndLine is wrong!", 4, cls.Base.BaseInterfaces[0].Position.EndLine);
			AssertEquals("EndCol is wrong!", 56, cls.Base.BaseInterfaces[0].Position.EndCol);

			AssertEquals("StartLine is wrong!", 4, cls.Base.BaseInterfaces[1].Position.StartLine);
			AssertEquals("StartCol is wrong!", 59, cls.Base.BaseInterfaces[1].Position.StartCol);
			AssertEquals("EndLine is wrong!", 4, cls.Base.BaseInterfaces[1].Position.EndLine);
			AssertEquals("EndCol is wrong!", 76, cls.Base.BaseInterfaces[1].Position.EndCol);
		}

		[Test]
		public void testWithOneModifier()
		{
			//Preparation
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  //Some comment.\r\n";
			fileString += "  //Line 2 of comment.\r\n";
			fileString += "  public class TestClass : TheBase, SomeInterface, SomeOtherInterface{\r\n";
			fileString += "  }\r\n";
			fileString += "}\r\n";

			//Execution
			CompilationUnit unit = ParseTestFile(fileString);

			//General tests
			AssertEquals("Namespace count is wrong! ", 1, unit.Members.Count);
			Namespace ns = unit.Members[0] as Namespace;
			AssertEquals("Namespace name is wrong! ", "TestNamespace", ns.Name);
			AssertEquals("Member count is wrong! ", 1, ns.Members.Count);
			Assert("Expected Class but got something else!", ns.Members[0] is Class);
			Class cls = ns.Members[0] as Class;
			AssertEquals("Modifier count is wrong!", 1, cls.Modifiers.Count);
			Modifier mod1 = cls.Modifiers[0];
			AssertEquals("Modifier name is wrong!", "public", mod1.Name);
			AssertEquals("Base is wrong!", "TheBase", cls.Base.Base.Name);
			AssertEquals("Interface count is wrong!", 2, cls.Base.BaseInterfaces.Count);
			AssertEquals("First interface name is wrong!", "SomeInterface", cls.Base.BaseInterfaces[0].Name);
			AssertEquals("First interface 1name is wrong!", "SomeOtherInterface", cls.Base.BaseInterfaces[1].Name);

			//Position tests
			AssertEquals("StartLine is wrong!", 4, mod1.Position.StartLine);
			AssertEquals("StartCol is wrong!", 3, mod1.Position.StartCol);
			AssertEquals("EndLine is wrong!", 4, mod1.Position.EndLine);
			AssertEquals("EndCol is wrong!", 8, mod1.Position.EndCol);

			AssertEquals("StartLine is wrong!", 4, cls.Position.StartLine);
			AssertEquals("StartCol is wrong!", 3, cls.Position.StartCol);
			AssertEquals("EndLine is wrong!", 5, cls.Position.EndLine);
			AssertEquals("EndCol is wrong!", 3, cls.Position.EndCol);
		}

		[Test]
		public void testWithAllModifiers()
		{
			//Preparation
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  //Some comment.\r\n";
			fileString += "  //Line 2 of comment.\r\n";
			fileString += "  new public protected internal private abstract sealed class TestClass : TheBase, SomeInterface, SomeOtherInterface{\r\n";
			fileString += "  }\r\n";
			fileString += "}\r\n";

			//Execution
			CompilationUnit unit = ParseTestFile(fileString);

			//General tests
			AssertEquals("Namespace count is wrong! ", 1, unit.Members.Count);
			Namespace ns = unit.Members[0] as Namespace;
			AssertEquals("Namespace name is wrong! ", "TestNamespace", ns.Name);
			AssertEquals("Member count is wrong! ", 1, ns.Members.Count);
			Assert("Expected Class but got something else!", ns.Members[0] is Class);
			Class cls = ns.Members[0] as Class;
			AssertEquals("Modifier count is wrong!", 7, cls.Modifiers.Count);
			AssertEquals("Modifier name is wrong!", "new", cls.Modifiers[0].Name);
			AssertEquals("Modifier name is wrong!", "public", cls.Modifiers[1].Name);
			AssertEquals("Modifier name is wrong!", "protected", cls.Modifiers[2].Name);
			AssertEquals("Modifier name is wrong!", "internal", cls.Modifiers[3].Name);
			AssertEquals("Modifier name is wrong!", "private", cls.Modifiers[4].Name);
			AssertEquals("Modifier name is wrong!", "abstract", cls.Modifiers[5].Name);
			AssertEquals("Modifier name is wrong!", "sealed", cls.Modifiers[6].Name);

			//Position tests
			AssertEquals("Base is wrong!", "TheBase", cls.Base.Base.Name);
			AssertEquals("Interface count is wrong!", 2, cls.Base.BaseInterfaces.Count);
			AssertEquals("First interface name is wrong!", "SomeInterface", cls.Base.BaseInterfaces[0].Name);
			AssertEquals("First interface name is wrong!", "SomeOtherInterface", cls.Base.BaseInterfaces[1].Name);
		}

		[Test]
		public void testConstantDeclaration1()
		{
			//Preparation
			string fileString = "const int a = 3;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			ClassMember m = CompilationUnit.parseClassMember(t);

			//General tests
			Assert("Expected ConstantDeclaration, but got something else!", m is ConstantDeclaration);
			ConstantDeclaration cd = m as ConstantDeclaration;
			
			//PrettyPrint tests
			AssertEquals("PrettyPrint is wrong!", fileString, cd.Generate());

			//Position tests
			AssertEquals("StartLine is wrong!", 1, cd.Position.StartLine);
			AssertEquals("StartCol is wrong!", 1, cd.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, cd.Position.EndLine);
			AssertEquals("EndCol is wrong!", 16, cd.Position.EndCol);
		}

		[Test]
		public void testConstantDeclaration2()
		{
			//Preparation
			string fileString = "class someClass { int m_Value = 3; const int a = m_Value;}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Class cls = Class.parse(t) as Class;
			ClassMember m1 = cls.Members[0];
			ClassMember m2 = cls.Members[1];

			//General tests
			Assert("Expected ConstantDeclaration, but got something else!", m2 is ConstantDeclaration);
			ConstantDeclaration cd = m2 as ConstantDeclaration;
			AssertEquals("Method count is wrong!", 0, cls.Methods.Count);
			AssertEquals("Field count is wrong!", 1, cls.Fields.Count);
			AssertEquals("Constant count is wrong!", 1, cls.Constants.Count);
			AssertEquals("Event count is wrong!", 0, cls.Events.Count);
			AssertEquals("Operators count is wrong!", 0, cls.Operators.Count);
			AssertEquals("Property count is wrong!", 0, cls.Properties.Count);

			//Variable and expression tests
			AssertEquals("Number of Expressions is wrong!", 2, cd.Expressions.Count);
			AssertEquals("Number of declared variables is wrong!", 1, cd.DeclaredVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 0, cd.ReferencedVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 2, cd.DeepReferencedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 1, cd.ModifiedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 1, cd.DeepModifiedVariables.Count);

			//Position tests
			AssertEquals("StartLine is wrong!", 1, cd.Position.StartLine);
			AssertEquals("StartCol is wrong!", 36, cd.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, cd.Position.EndLine);
			AssertEquals("EndCol is wrong!", 57, cd.Position.EndCol);
		}

		[Test]
		public void testFieldDeclaration1()
		{
			//Preparation
			string fileString = "int a = 3;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			ClassMember m = CompilationUnit.parseClassMember(t);

			//General tests
			Assert("Expected FieldDeclaration, but got something else!", m is FieldDeclaration);
			FieldDeclaration fd = m as FieldDeclaration;

			//Position tests
			AssertEquals("StartLine is wrong!", 1, fd.Position.StartLine);
			AssertEquals("StartCol is wrong!", 1, fd.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, fd.Position.EndLine);
			AssertEquals("EndCol is wrong!", 10, fd.Position.EndCol);
		}

		[Test]
		public void testFieldDeclaration2()
		{
			//Preparation
			string fileString = "class someClass { Tokenizer tokenizer = new Tokenizer(); DataType type = DataType.parse(tokenizer);}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Class cls = Class.parse(t) as Class;
			ClassMember m1 = cls.Members[0];
			ClassMember m2 = cls.Members[1];

			//General tests
			Assert("Expected FieldDeclaration, but got something else!", m1 is FieldDeclaration);
			Assert("Expected FieldDeclaration, but got something else!", m2 is FieldDeclaration);
			FieldDeclaration fd1 = m1 as FieldDeclaration;
			FieldDeclaration fd2 = m2 as FieldDeclaration;
			AssertEquals("Method count is wrong!", 0, cls.Methods.Count);
			AssertEquals("Field count is wrong!", 2, cls.Fields.Count);
			AssertEquals("Constant count is wrong!", 0, cls.Constants.Count);
			AssertEquals("Event count is wrong!", 0, cls.Events.Count);
			AssertEquals("Operators count is wrong!", 0, cls.Operators.Count);
			AssertEquals("Property count is wrong!", 0, cls.Properties.Count);

			//Variable and expression tests
			AssertEquals("Number of Expressions is wrong!", 2, fd1.Expressions.Count);
			AssertEquals("Number of declared variables is wrong!", 1, fd1.DeclaredVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 0, fd1.ReferencedVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 1, fd1.DeepReferencedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 1, fd1.ModifiedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 1, fd1.DeepModifiedVariables.Count);

			AssertEquals("Number of Expressions is wrong!", 5, fd2.Expressions.Count);
			AssertEquals("Number of declared variables is wrong!", 1, fd2.DeclaredVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 0, fd2.ReferencedVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 2, fd2.DeepReferencedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 1, fd2.ModifiedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 1, fd2.DeepModifiedVariables.Count);

			//Position tests
			AssertEquals("StartLine is wrong!", 1, fd1.Position.StartLine);
			AssertEquals("StartCol is wrong!", 19, fd1.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, fd1.Position.EndLine);
			AssertEquals("EndCol is wrong!", 56, fd1.Position.EndCol);

			AssertEquals("StartLine is wrong!", 1, fd2.Position.StartLine);
			AssertEquals("StartCol is wrong!", 58, fd2.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, fd2.Position.EndLine);
			AssertEquals("EndCol is wrong!", 99, fd2.Position.EndCol);
		}

		[Test]
		public void testMethodDeclaration1()
		{
			//Preparation
			string fileString = "class someClass { int m_SomeMember = 0; int someMethod(){}}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Class cls = Class.parse(t) as Class;
			ClassMember m = cls.Members[1];

			//General tests
			Assert("Expected MethodDeclaration, but got something else!", m is MethodDeclaration);
			MethodDeclaration md = m as MethodDeclaration;
			AssertEquals("Name is wrong!", "someMethod", md.Name);
			AssertEquals("Method count is wrong!", 1, cls.Methods.Count);
			AssertEquals("Field count is wrong!", 1, cls.Fields.Count);
			AssertEquals("Constant count is wrong!", 0, cls.Constants.Count);
			AssertEquals("Event count is wrong!", 0, cls.Events.Count);
			AssertEquals("Operators count is wrong!", 0, cls.Operators.Count);
			AssertEquals("Property count is wrong!", 0, cls.Properties.Count);
			AssertEquals("Pretty Print string is wrong!", " int someMethod(){}", md.Generate());

			//Variable and expression tests
			AssertEquals("Number of Expressions is wrong!", 0, md.Expressions.Count);
			AssertEquals("Number of declared variables is wrong!", 0, md.DeclaredVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 0, md.ReferencedVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 0, md.DeepReferencedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 0, md.ModifiedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 0, md.DeepModifiedVariables.Count);

			//Position tests
			AssertEquals("StartLine is wrong!", 1, md.Position.StartLine);
			AssertEquals("StartCol is wrong!", 41, md.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, md.Position.EndLine);
			AssertEquals("EndCol is wrong!", 58, md.Position.EndCol);
		}

		[Test]
		public void testMethodDeclaration2()
		{
			//Preparation
			string fileString = "class someClass { int m_SomeMember = 0; int someMethod(string c, int b, SomeClass d) ;}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Class cls = Class.parse(t) as Class;
			ClassMember m = cls.Members[1];

			//General tests
			Assert("Expected MethodDeclaration, but got something else!", m is MethodDeclaration);
			MethodDeclaration md = m as MethodDeclaration;
			AssertEquals("Name is wrong!", "someMethod", md.Name);
			AssertEquals("Method count is wrong!", 1, cls.Methods.Count);
			AssertEquals("Field count is wrong!", 1, cls.Fields.Count);
			AssertEquals("Constant count is wrong!", 0, cls.Constants.Count);
			AssertEquals("Event count is wrong!", 0, cls.Events.Count);
			AssertEquals("Operators count is wrong!", 0, cls.Operators.Count);
			AssertEquals("Property count is wrong!", 0, cls.Properties.Count);
			
			//PrettyPrint tests
			AssertEquals("Pretty Print string is wrong!", fileString, cls.Generate());

			//Variable and expression tests
			AssertEquals("Number of Expressions is wrong!", 0, md.Expressions.Count);
			AssertEquals("Number of declared variables is wrong!", 3, md.DeclaredVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 0, md.ReferencedVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 0, md.DeepReferencedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 0, md.ModifiedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 0, md.DeepModifiedVariables.Count);

			//Position tests
			AssertEquals("StartLine is wrong!", 1, md.Position.StartLine);
			AssertEquals("StartCol is wrong!", 41, md.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, md.Position.EndLine);
			AssertEquals("EndCol is wrong!", 86, md.Position.EndCol);
		}

		[Test]
		public void testMethodDeclaration3()
		{
			//Preparation
			string fileString = "class someClass { int m_SomeMember = 0; public virtual bool Do(){return m_SomeMember;}}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Class cls = Class.parse(t) as Class;
			ClassMember m = cls.Members[1];

			//General tests
			Assert("Expected MethodDeclaration, but got something else!", m is MethodDeclaration);
			MethodDeclaration md = m as MethodDeclaration;
			AssertEquals("Name is wrong!", "Do", md.Name);
			AssertEquals("Method count is wrong!", 1, cls.Methods.Count);
			AssertEquals("Field count is wrong!", 1, cls.Fields.Count);
			AssertEquals("Constant count is wrong!", 0, cls.Constants.Count);
			AssertEquals("Event count is wrong!", 0, cls.Events.Count);
			AssertEquals("Operators count is wrong!", 0, cls.Operators.Count);
			AssertEquals("Property count is wrong!", 0, cls.Properties.Count);

			//Variable and expression tests
			AssertEquals("Number of Expressions is wrong!", 1, md.Expressions.Count);
			AssertEquals("Number of declared variables is wrong!", 0, md.DeclaredVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 0, md.ReferencedVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 1, md.DeepReferencedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 0, md.ModifiedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 0, md.DeepModifiedVariables.Count);

			//Position tests
			AssertEquals("StartLine is wrong!", 1, md.Position.StartLine);
			AssertEquals("StartCol is wrong!", 41, md.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, md.Position.EndLine);
			AssertEquals("EndCol is wrong!", 86, md.Position.EndCol);
		}

		[Test]
		public void testPropertyDeclaration1()
		{
			//Preparation
			string fileString = "class someClass { int m_SomeMember = 0; int someProperty{get{return m_SomeMember;}set{m_SomeMember = value;}}}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Class cls = Class.parse(t) as Class;
			ClassMember m = cls.Members[1];

			//General tests
			Assertion.Assert("Expected PropertyDeclaration, but got something else!", m is PropertyDeclaration);
			PropertyDeclaration pd = m as PropertyDeclaration;
			AssertEquals("Name is wrong!", "someProperty", pd.Name);
			AssertEquals("Method count is wrong!", 0, cls.Methods.Count);
			AssertEquals("Field count is wrong!", 1, cls.Fields.Count);
			AssertEquals("Constant count is wrong!", 0, cls.Constants.Count);
			AssertEquals("Event count is wrong!", 0, cls.Events.Count);
			AssertEquals("Operators count is wrong!", 0, cls.Operators.Count);
			AssertEquals("Indexers count is wrong!", 0, cls.Indexers.Count);
			AssertEquals("Property count is wrong!", 1, cls.Properties.Count);
			AssertEquals("PrettyPrint string is wrong!", fileString, cls.Generate());

			//Variable and expression tests
			AssertEquals("Number of Expressions is wrong!", 4, pd.Expressions.Count);
			AssertEquals("Number of declared variables is wrong!", 0, pd.DeclaredVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 0, pd.ReferencedVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 1, pd.DeepReferencedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 0, pd.ModifiedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 1, pd.DeepModifiedVariables.Count);

			//Position tests
			AssertEquals("StartLine is wrong!", 1, pd.Position.StartLine);
			AssertEquals("StartCol is wrong!", 41, pd.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, pd.Position.EndLine);
			AssertEquals("EndCol is wrong!", 109, pd.Position.EndCol);
		}

		[Test]
		public void testPropertyDeclaration2()
		{
			//Preparation
			string fileString = "int someProperty{get{;}}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			ClassMember m = CompilationUnit.parseClassMember(t);

			//General tests
			Assertion.Assert("Expected PropertyDeclaration, but got something else!", m is PropertyDeclaration);
			PropertyDeclaration pd = m as PropertyDeclaration;
			Assertion.AssertEquals("Name is wrong!", "someProperty", pd.Name);

			//Variable and expression tests
			AssertEquals("Number of Expressions is wrong!", 0, pd.Expressions.Count);
			AssertEquals("Number of declared variables is wrong!", 0, pd.DeclaredVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 0, pd.ReferencedVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 0, pd.DeepReferencedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 0, pd.ModifiedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 0, pd.DeepModifiedVariables.Count);

			//Position tests
			AssertEquals("StartLine is wrong!", 1, pd.Position.StartLine);
			AssertEquals("StartCol is wrong!", 1, pd.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, pd.Position.EndLine);
			AssertEquals("EndCol is wrong!", 24, pd.Position.EndCol);
		}

		[Test]
		public void testConstructorDeclaration1()
		{
			//Preparation
			string fileString = "public ConstructorDeclaration(){}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			ClassMember m = CompilationUnit.parseClassMember(t);

			//General tests
			Assert("Expected ConstructorDeclaration, but got something else!", m is ConstructorDeclaration);
			AssertEquals("Name is wrong!", "ConstructorDeclaration", m.Name);
			ConstructorDeclaration cd = m as ConstructorDeclaration;
			AssertEquals("PrettyPrint is wrong!", fileString, cd.Generate());

			//Variable and expression tests
			AssertEquals("Number of Expressions is wrong!", 0, cd.Expressions.Count);
			AssertEquals("Number of declared variables is wrong!", 0, cd.DeclaredVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 0, cd.ReferencedVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 0, cd.DeepReferencedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 0, cd.ModifiedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 0, cd.DeepModifiedVariables.Count);

			//Position tests
			AssertEquals("StartLine is wrong!", 1, cd.Position.StartLine);
			AssertEquals("StartCol is wrong!", 1, cd.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, cd.Position.EndLine);
			AssertEquals("EndCol is wrong!", 33, cd.Position.EndCol);
		}

		[Test]
		public void testConstructorDeclaration2()
		{
			//Preparation
			string fileString = "public ConstructorDeclaration(SomeType something) : base(something){}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			ClassMember m = CompilationUnit.parseClassMember(t);

			//General tests
			Assert("Expected ConstructorDeclaration, but got something else!", m is ConstructorDeclaration);
			AssertEquals("Name is wrong!", "ConstructorDeclaration", m.Name);
			ConstructorDeclaration cd = m as ConstructorDeclaration;
            cd.PropagateUp();
			AssertEquals("PrettyPrint is wrong!", fileString, cd.Generate());

			//Variable and expression tests
			AssertEquals("Number of Expressions is wrong!", 1, cd.Expressions.Count);
			AssertEquals("Number of declared variables is wrong!", 1, cd.DeclaredVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 0, cd.ReferencedVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 1, cd.DeepReferencedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 0, cd.ModifiedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 0, cd.DeepModifiedVariables.Count);

			//Position tests
			AssertEquals("StartLine is wrong!", 1, cd.Position.StartLine);
			AssertEquals("StartCol is wrong!", 1, cd.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, cd.Position.EndLine);
			AssertEquals("EndCol is wrong!", 69, cd.Position.EndCol);
		}

		[Test]
		public void testConstructorInitializer1()
		{
			//Preparation
			string fileString = ": base(something)";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			ConstructorInitializer ci = ConstructorInitializer.parse(t);

			//General tests
			AssertEquals("Got MissingConstructorInitializer!", false, ci is MissingConstructorInitializer);

			//Position tests
			AssertEquals("StartLine is wrong!", 1, ci.Position.StartLine);
			AssertEquals("StartCol is wrong!", 1, ci.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, ci.Position.EndLine);
			AssertEquals("EndCol is wrong!", 17, ci.Position.EndCol);
		}

		[Test]
		public void testConstructorInitializer2()
		{
			//Preparation
			string fileString = ": this(something)";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			ConstructorInitializer ci = ConstructorInitializer.parse(t);

			//General tests
			AssertEquals("Got MissingConstructorInitializer!", false, ci is MissingConstructorInitializer);

			//Position tests
			AssertEquals("StartLine is wrong!", 1, ci.Position.StartLine);
			AssertEquals("StartCol is wrong!", 1, ci.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, ci.Position.EndLine);
			AssertEquals("EndCol is wrong!", 17, ci.Position.EndCol);
		}

		[Test]
		public void testDestructorDeclaration1()
		{
			//Preparation
			string fileString = "class someClass {int a, b; ~SomeClass(){a = b;}}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Class cls = Class.parse(t) as Class;
			ClassMember m = cls.Members[1];

			//General tests
			AssertEquals("Expected DestructorDeclaration, but it is missing!", false, m is MissingDestructorDeclaration);
			AssertEquals("Name is wrong!", "SomeClass", m.Name);
			DestructorDeclaration dd = m as DestructorDeclaration;
			AssertEquals("PrettyPrint is wrong!", fileString, cls.Generate());

			//Variable and expression tests
			AssertEquals("Number of Expressions is wrong!", 3, dd.Expressions.Count);
			AssertEquals("Number of declared variables is wrong!", 0, dd.DeclaredVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 0, dd.ReferencedVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 2, dd.DeepReferencedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 0, dd.ModifiedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 1, dd.DeepModifiedVariables.Count);

			//Position tests
			AssertEquals("StartLine is wrong!", 1, dd.Position.StartLine);
			AssertEquals("StartCol is wrong!", 28, dd.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, dd.Position.EndLine);
			AssertEquals("EndCol is wrong!", 47, dd.Position.EndCol);
		}

		[Test]
		public void testDestructorDeclaration2()
		{
			//Preparation
			string fileString = "[attribute] extern ~SomeClass(){}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			ClassMember m = CompilationUnit.parseClassMember(t);

			//General tests
			AssertEquals("Expected DestructorDeclaration, but it is missing!", false, m is MissingDestructorDeclaration);
			AssertEquals("Name is wrong!", "SomeClass", m.Name);
			DestructorDeclaration dd = m as DestructorDeclaration;
			AssertEquals("PrettyPrint is wrong!", fileString, dd.Generate());

			//Position tests
			AssertEquals("StartLine is wrong!", 1, dd.Position.StartLine);
			AssertEquals("StartCol is wrong!", 1, dd.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, dd.Position.EndLine);
			AssertEquals("EndCol is wrong!", 33, dd.Position.EndCol);
		}

		[Test]
		public void testIndexerDeclaration1()
		{
			//Preparation
			string fileString = "public object this[int index]{get{;}set{;}}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			ClassMember cm = CompilationUnit.parseClassMember(t);

			//General tests
			AssertEquals("Got MissingClassMember!", false, cm is MissingClassMember);
			IndexerDeclaration id = cm as IndexerDeclaration;

			//Position tests
			AssertEquals("StartLine is wrong!", 1, id.Position.StartLine);
			AssertEquals("StartCol is wrong!", 1, id.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, id.Position.EndLine);
			AssertEquals("EndCol is wrong!", 43, id.Position.EndCol);
		}

		[Test]
		public void testIndexerDeclaration2()
		{
			//Preparation
			string fileString = "class someClass{int a,b; public object someType.this[int index]{get{a++;}set{b--;}}}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Class cls = Class.parse(t) as Class;
			
			//General tests
			ClassMember cm = cls.Members[1];
			AssertEquals("Got MissingClassMember!", false, cm is MissingClassMember);
			IndexerDeclaration id = cm as IndexerDeclaration;
			AssertEquals("Method count is wrong!", 0, cls.Methods.Count);
			AssertEquals("Field count is wrong!", 1, cls.Fields.Count);
			AssertEquals("Constant count is wrong!", 0, cls.Constants.Count);
			AssertEquals("Event count is wrong!", 0, cls.Events.Count);
			AssertEquals("Operators count is wrong!", 0, cls.Operators.Count);
			AssertEquals("Indexers count is wrong!", 1, cls.Indexers.Count);
			AssertEquals("Property count is wrong!", 0, cls.Properties.Count);

			//Variable and expression tests
			AssertEquals("Number of Expressions is wrong!", 4, id.Expressions.Count);
			AssertEquals("Number of declared variables is wrong!", 0, id.DeclaredVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 0, id.ReferencedVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 2, id.DeepReferencedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 0, id.ModifiedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 2, id.DeepModifiedVariables.Count);

			//Position tests
			AssertEquals("StartLine is wrong!", 1, id.Position.StartLine);
			AssertEquals("StartCol is wrong!", 26, id.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, id.Position.EndLine);
			AssertEquals("EndCol is wrong!", 83, id.Position.EndCol);
		}

		[Test]
		public void testOperatorDeclaration1()
		{
			//Preparation
			string fileString = "public static SymbolSet operator+ (SymbolSet s,SymbolSet t) {}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			ClassMember cm = CompilationUnit.parseClassMember(t);

			//General tests
			Assert("Expected OperatorDeclaration, but got something else!", cm is OperatorDeclaration);
			OperatorDeclaration od = cm as OperatorDeclaration;
			AssertEquals("PrettyPrint is wrong!", fileString, od.Generate());

			//Variable and expression tests
			AssertEquals("Number of Expressions is wrong!", 0, od.Expressions.Count);
			AssertEquals("Number of declared variables is wrong!", 2, od.DeclaredVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 0, od.ReferencedVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 0, od.DeepReferencedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 0, od.ModifiedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 0, od.DeepModifiedVariables.Count);

			//Position tests
			AssertEquals("StartLine is wrong!", 1, od.Position.StartLine);
			AssertEquals("StartCol is wrong!", 1, od.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, od.Position.EndLine);
			AssertEquals("EndCol is wrong!", 62, od.Position.EndCol);
		}

		[Test]
		public void testOperatorDeclaration2()
		{
			//Preparation
			string fileString = "class someClass {int a; public static SymbolSet operator+ (SymbolSet s,SymbolSet t) {--a;}}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Class cls = Class.parse(t) as Class;

			//General tests
			Assert("Expected OperatorDeclaration, but got something else!", cls.Members[1] is OperatorDeclaration);
			ClassMember cm = cls.Members[1];
			OperatorDeclaration od = cm as OperatorDeclaration;
			AssertEquals("Method count is wrong!", 0, cls.Methods.Count);
			AssertEquals("Field count is wrong!", 1, cls.Fields.Count);
			AssertEquals("Constant count is wrong!", 0, cls.Constants.Count);
			AssertEquals("Event count is wrong!", 0, cls.Events.Count);
			AssertEquals("Operators count is wrong!", 1, cls.Operators.Count);
			AssertEquals("Indexers count is wrong!", 0, cls.Indexers.Count);
			AssertEquals("Property count is wrong!", 0, cls.Properties.Count);
			AssertEquals("PrettyPrint is wrong!", fileString, cls.Generate());
			
			//Variable and expression tests
			AssertEquals("Number of Expressions is wrong!", 2, od.Expressions.Count);
			AssertEquals("Number of declared variables is wrong!", 2, od.DeclaredVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 0, od.ReferencedVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 1, od.DeepReferencedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 0, od.ModifiedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 1, od.DeepModifiedVariables.Count);

			//Position tests
			AssertEquals("StartLine is wrong!", 1, od.Position.StartLine);
			AssertEquals("StartCol is wrong!", 25, od.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, od.Position.EndLine);
			AssertEquals("EndCol is wrong!", 90, od.Position.EndCol);
		}


		[Test]
		public void testClassInClass()
		{
			//Preparation
			string fileString = "class parent{class child{}}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			TypeDeclaration td = Class.parse(t);

			//General tests
			AssertEquals("Got MissingTypeDeclaration!", false, td is MissingTypeDeclaration);
			Class cls = td as Class;
			Assertion.AssertEquals("Number of inner classes is wrong!", 1, cls.Classes.Count);

			//Position tests
			AssertEquals("StartLine is wrong!", 1, cls.Position.StartLine);
			AssertEquals("StartCol is wrong!", 1, cls.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, cls.Position.EndLine);
			AssertEquals("EndCol is wrong!", 27, cls.Position.EndCol);
		}

		[Test]
		public void testInterfaceInClass()
		{
			//Preparation
			string fileString = "class parent{interface child{}}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			TypeDeclaration td = Class.parse(t);

			//General tests
			AssertEquals("Got MissingTypeDeclaration!", false, td is MissingTypeDeclaration);
			Class cls = td as Class;
			AssertEquals("BaseInterface count is wrong!", 1, cls.Interfaces.Count);

			//Position tests
			AssertEquals("StartLine is wrong!", 1, cls.Position.StartLine);
			AssertEquals("StartCol is wrong!", 1, cls.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, cls.Position.EndLine);
			AssertEquals("EndCol is wrong!", 31, cls.Position.EndCol);
		}

		[Test]
		public void testEventDeclaration1()
		{
			//Preparation
			string fileString = "class someClass {int a,b; public event MouseEventHandler MouseDown {add{a++;} remove {b--;}}}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Class cls = Class.parse(t) as Class;
			ClassMember cm = cls.Members[1];

			//General tests
			Assert("Expected EventDeclaration, but got something else!", cm is EventDeclaration);
			EventDeclaration ed = cm as EventDeclaration;
			AssertEquals("Method count is wrong!", 0, cls.Methods.Count);
			AssertEquals("Field count is wrong!", 1, cls.Fields.Count);
			AssertEquals("Constant count is wrong!", 0, cls.Constants.Count);
			AssertEquals("Event count is wrong!", 1, cls.Events.Count);
			AssertEquals("Operators count is wrong!", 0, cls.Operators.Count);
			AssertEquals("Property count is wrong!", 0, cls.Properties.Count);
			AssertEquals("PrettyPrint is wrong!", fileString, cls.Generate());

			//Variable and expression tests
			AssertEquals("Number of Expressions is wrong!", 4, ed.Expressions.Count);
			AssertEquals("Number of declared variables is wrong!", 0, ed.DeclaredVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 0, ed.ReferencedVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 2, ed.DeepReferencedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 0, ed.ModifiedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 2, ed.DeepModifiedVariables.Count);

			//Position tests
			AssertEquals("StartLine is wrong!", 1, ed.Position.StartLine);
			AssertEquals("StartCol is wrong!", 27, ed.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, ed.Position.EndLine);
			AssertEquals("EndCol is wrong!", 92, ed.Position.EndCol);
		}

		[Test]
		public void testEventDeclaration2()
		{
			//Preparation
			string fileString = "class someClass {public event MouseEventHandler MouseDown;}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Class cls = Class.parse(t) as Class;

			//General tests
			AssertEquals("Class member count is wrong!", 1, cls.Members.Count);
			ClassMember cm = cls.Members[0];
			Assert("Expected EventDeclaration, but got something else!", cm is EventDeclaration);
			EventDeclaration ed = cm as EventDeclaration;
			AssertEquals("Method count is wrong!", 0, cls.Methods.Count);
			AssertEquals("Field count is wrong!", 0, cls.Fields.Count);
			AssertEquals("Constant count is wrong!", 0, cls.Constants.Count);
			AssertEquals("Event count is wrong!", 1, cls.Events.Count);
			AssertEquals("Operators count is wrong!", 0, cls.Operators.Count);
			AssertEquals("Property count is wrong!", 0, cls.Properties.Count);
			AssertEquals("PrettyPrint is wrong!", fileString, cls.Generate());

			//Position tests
			AssertEquals("StartLine is wrong!", 1, ed.Position.StartLine);
			AssertEquals("StartCol is wrong!", 18, ed.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, ed.Position.EndLine);
			AssertEquals("EndCol is wrong!", 58, ed.Position.EndCol);
		}

		[Test]
		public void testEventAccessorDeclarations1()
		{
			//Preparation
			string fileString = "[anAttribute]add{} [anotherAttribute]remove{}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			EventAccessorDeclarations ead = EventAccessorDeclarations.parse(t);

			//General tests
			AssertEquals("EventAccessorDeclarations is missing!", false, ead is MissingEventAccessorDeclarations);

			//Position tests
			AssertEquals("StartLine is wrong!", 1, ead.Position.StartLine);
			AssertEquals("StartCol is wrong!", 1, ead.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, ead.Position.EndLine);
			AssertEquals("EndCol is wrong!", 45, ead.Position.EndCol);
		}
		
		[Test]
		public void testReprint1()
		{
			//Preparation
			string fileString = "";
			fileString += "public class TestClass : TheBase{\r\n";
			fileString += "}";
			
			//Execution
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();
			Class cls = Class.parse(t) as Class;
			
			//PrettyPrintTest
			Assertion.AssertEquals("PrettyPrint does not match original!", fileString, cls.Generate());
		}
		
		[Test]
		public void testReprint2()
		{
			//Preparation
			string fileString = "";
			fileString += "public class TestClass : \r\n";
			fileString += "TheBase\r\n";
			fileString += "{";
			fileString += "}";
			
			//Execution
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();
			Class cls = Class.parse(t) as Class;
			
			//PrettyPrintTest
			Assertion.AssertEquals("PrettyPrint does not match original!", fileString, cls.Generate());
		}
	}
}

