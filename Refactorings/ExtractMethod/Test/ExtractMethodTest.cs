#region Copyright (C) Pierre Boudreau
//This file is part of the Opnieuw project.
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
using System.IO;
using System.Collections;
using Opnieuw.Parsers.CSParser;
using Opnieuw.ExtractMethod;
using Opnieuw.Framework;
using NUnit.Framework;

namespace Opnieuw.Refactorings.ExtractMethod.Test
{
	[TestFixture]
	public class ExtractMethodTest
	{
		string m_FileName = "TestFile.cs";

		[SetUp]
		public void SetUp()
		{
		}

		[TearDown]
		public void TearDown()
		{
		}

		private CompilationUnit ParseTestFile1()
		{
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "\tclass TestClass\r\n";
			fileString += "\t{\r\n";
			fileString += "\t\tprivate void someMethod()\r\n";
			fileString += "\t\t{\r\n";
			fileString += "\t\t\tint a = 3;\r\n";
			fileString += "\t\t\ta++;\r\n";
			fileString += "\t\t}\r\n";
			fileString += "\t}\r\n";
			fileString += "}\r\n";
			return ParseTestFile(fileString);
		}

		[Test]
		public void testOnNodeSelectionChanged1()
		{
			m_IsAvailable = false;
			CompilationUnit unit = ParseTestFile1();
			ArrayList nodeList = new ArrayList();
			nodeList.Add(unit.Namespaces[0].Classes[0].Methods[0].Body.Statements[0]);
			nodeList.Add(unit.Namespaces[0].Classes[0].Methods[0].Body.Statements[1]);
			ExtractMethodRefactoring refactoring = new ExtractMethodRefactoring();
			refactoring.AvailabilityChanged += new AvailabilityChangedHandler(this.OnAvailabilityChanged);
			refactoring.OnNodeSelectionChanged(null, nodeList);
			Assertion.AssertEquals("Expected isAvailable to be true, but it was not!", true, m_IsAvailable);

		}

		private CompilationUnit ParseTestFile2()
		{
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  class TestClass\r\n";
			fileString += "  {\r\n";
			fileString += "    private void someMethod()\r\n";
			fileString += "    {\r\n";
			fileString += "      int a = 3;\r\n";
			fileString += "      a++;\r\n";
			fileString += "      a--;\r\n";
			fileString += "    }\r\n";
			fileString += "  }\r\n";
			fileString += "}\r\n";
			return ParseTestFile(fileString);
		}

		[Test]
		public void testOnNodeSelectionChanged2()
		{
			m_IsAvailable = false;
			CompilationUnit unit = ParseTestFile2();
			ArrayList nodeList = new ArrayList();
			nodeList.Add(unit.Namespaces[0].Classes[0].Methods[0].Body.Statements[0]);
			nodeList.Add(unit.Namespaces[0].Classes[0].Methods[0].Body.Statements[1]);
			ExtractMethodRefactoring refactoring = new ExtractMethodRefactoring();
			refactoring.AvailabilityChanged += new AvailabilityChangedHandler(this.OnAvailabilityChanged);
			refactoring.OnNodeSelectionChanged(null, nodeList);

		}

		[Test]
		public void testOnNodeSelectionChanged3()
		{
			m_IsAvailable = false;
			CompilationUnit unit = ParseTestFile2();
			ArrayList nodeList = new ArrayList();
			nodeList.Add(unit.Namespaces[0].Classes[0].Methods[0].Body.Statements[1]);
			nodeList.Add(unit.Namespaces[0].Classes[0].Methods[0].Body.Statements[2]);
			ExtractMethodRefactoring refactoring = new ExtractMethodRefactoring();
			refactoring.AvailabilityChanged += new AvailabilityChangedHandler(this.OnAvailabilityChanged);
			refactoring.OnNodeSelectionChanged(null, nodeList);

		}

		[Test]
		public void testOnNodeSelectionChanged4()
		{
			m_IsAvailable = false;
			CompilationUnit unit = ParseTestFile2();
			ArrayList nodeList = new ArrayList();
			nodeList.Add(unit.Namespaces[0].Classes[0].Methods[0]);
			ExtractMethodRefactoring refactoring = new ExtractMethodRefactoring();
			refactoring.AvailabilityChanged += new AvailabilityChangedHandler(this.OnAvailabilityChanged);
			refactoring.OnNodeSelectionChanged(null, nodeList);

		}

		[Test]
		public void testOnNodeSelectionChanged5()
		{
			m_IsAvailable = false;
			CompilationUnit unit = ParseTestFile2();
			ArrayList nodeList = new ArrayList();
			nodeList.Add(unit.Namespaces[0].Classes[0].Methods[0].Body.Statements[0]);
			ExtractMethodRefactoring refactoring = new ExtractMethodRefactoring();
			refactoring.AvailabilityChanged += new AvailabilityChangedHandler(this.OnAvailabilityChanged);
			refactoring.OnNodeSelectionChanged(null, nodeList);

		}

		private CompilationUnit ParseTestFile3()
		{
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  class TestClass\r\n";
			fileString += "  {\r\n";
			fileString += "    private void someMethod()\r\n";
			fileString += "    {\r\n";
			fileString += "      int a = 0;\r\n";
			fileString += "      SomeType someObject = new SomeType;\r\n";
			fileString += "      somePrivateMethod(someObject);\r\n";
			fileString += "      a--;\r\n";
			fileString += "    }\r\n";
			fileString += "    private void somePrivateMethod(SomeType someObject)\r\n";
			fileString += "    {\r\n";
			fileString += "      someObject.SomeMethod();\r\n";
			fileString += "      someObject.SomeProperty ++;\r\n";
			fileString += "    }\r\n";
			fileString += "  }\r\n";
			fileString += "}\r\n";
			return ParseTestFile(fileString);
		}

		[Test]
		public void testOnNodeSelectionChanged6()
		{
			m_IsAvailable = false;
			CompilationUnit unit = ParseTestFile3();
			ArrayList nodeList = new ArrayList();
			nodeList.Add(unit.Namespaces[0].Classes[0].Methods[0].Body.Statements[0]);
			nodeList.Add(unit.Namespaces[0].Classes[0].Methods[0].Body.Statements[3]);
			ExtractMethodRefactoring refactoring = new ExtractMethodRefactoring();
			refactoring.AvailabilityChanged += new AvailabilityChangedHandler(this.OnAvailabilityChanged);
			refactoring.OnNodeSelectionChanged(null, nodeList);

		}

		[Test]
		public void testOnNodeSelectionChanged7()
		{
			m_IsAvailable = false;
			CompilationUnit unit = ParseTestFile3();
			ArrayList nodeList = new ArrayList();
			nodeList.Add(unit.Namespaces[0].Classes[0].Methods[0].Body.Statements[0]);
			nodeList.Add(unit.Namespaces[0].Classes[0].Methods[0].Body.Statements[1]);
			ExtractMethodRefactoring refactoring = new ExtractMethodRefactoring();
			refactoring.AvailabilityChanged += new AvailabilityChangedHandler(this.OnAvailabilityChanged);
			refactoring.OnNodeSelectionChanged(null, nodeList);

		}

		[Test]
		public void testOnNodeSelectionChanged8()
		{
			m_IsAvailable = false;
			CompilationUnit unit = ParseTestFile3();
			ArrayList nodeList = new ArrayList();
			nodeList.Add(unit.Namespaces[0].Classes[0].Methods[1].Body.Statements[0]);
			nodeList.Add(unit.Namespaces[0].Classes[0].Methods[1].Body.Statements[1]);
			ExtractMethodRefactoring refactoring = new ExtractMethodRefactoring();
			refactoring.AvailabilityChanged += new AvailabilityChangedHandler(this.OnAvailabilityChanged);
			refactoring.OnNodeSelectionChanged(null, nodeList);

		}

		private CompilationUnit ParseTestFile4()
		{
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  class TestClass\r\n";
			fileString += "  {\r\n";
			fileString += "    private void someMethod()\r\n";
			fileString += "    {\r\n";
			fileString += "      SomeType someObject = new SomeType();\r\n";
			fileString += "      a--;\r\n";
			fileString += "    }\r\n";
			fileString += "  }\r\n";
			fileString += "}\r\n";
			return ParseTestFile(fileString);
		}

		[Test]
		public void testExtractAnExpression()
		{
			m_IsAvailable = false;
			CompilationUnit unit = ParseTestFile4();
			ArrayList nodeList = new ArrayList();
			DeclarationStatement statement = unit.Namespaces[0].Classes[0].Methods[0].Body.Statements[0] as DeclarationStatement;
			ObjectCreationExpression expression = statement.LocalVariableDeclaration.VariableDeclarators[0].Initializer as ObjectCreationExpression;
			nodeList.Add(expression);
			ExtractMethodRefactoring refactoring = new ExtractMethodRefactoring();
			refactoring.AvailabilityChanged += new AvailabilityChangedHandler(this.OnAvailabilityChanged);
			refactoring.OnNodeSelectionChanged(null, nodeList);

		}

		private CompilationUnit ParseTestFile5()
		{
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "\tclass TestClass\r\n";
			fileString += "\t{\r\n";
			fileString += "\t\tprivate void someMethod()\r\n";
			fileString += "\t\t{\r\n";
			fileString += "\t\t\tSomeType someObject = new SomeType();\r\n";
			fileString += "\t\t\tint a = 0;\r\n";
			fileString += "\t\t\tif (true)\r\n";
			fileString += "\t\t\t{\r\n";
			fileString += "\t\t\ta --;\r\n";
			fileString += "\t\t\t}\r\n";
			fileString += "\t\ta ++;\r\n";
			fileString += "\t\t}\r\n";
			fileString += "\t}\r\n";
			fileString += "}\r\n";
			return ParseTestFile(fileString);
		}

		[Test]
		public void testExtractFromNestedStatement1()
		{
			m_IsAvailable = false;
			CompilationUnit unit = ParseTestFile5();
			ArrayList nodeList = new ArrayList();
			IfStatement ifStatement = unit.Namespaces[0].Classes[0].Methods[0].Body.Statements[2] as IfStatement;
			ExpressionStatement postDecrementStatement = ifStatement.IfPartStatement.Statements[0] as ExpressionStatement;
			nodeList.Add(postDecrementStatement);
			ExtractMethodRefactoring refactoring = new ExtractMethodRefactoring();
			refactoring.AvailabilityChanged += new AvailabilityChangedHandler(this.OnAvailabilityChanged);
			refactoring.OnNodeSelectionChanged(null, nodeList);

		}

		protected bool m_IsAvailable = false;
		public void OnAvailabilityChanged(bool isAvailable)
		{
			m_IsAvailable = isAvailable;
		}

		protected string ReadTestFile() {
			System.IO.FileStream fs = null;
			System.IO.StreamReader sr = null;
			fs = System.IO.File.OpenRead(m_FileName);
			sr = new System.IO.StreamReader(fs);
			string ret = sr.ReadToEnd();
			sr.Close();
			return ret;
		}

		protected void WriteTestFile(string fileString, string fileName) {
			if (System.IO.File.Exists(m_FileName))
			{
				System.IO.File.Delete(m_FileName);
			}
			System.IO.FileStream fs = null;
			System.IO.StreamWriter sw = null;
			fs = System.IO.File.OpenWrite(fileName);
			sw = new System.IO.StreamWriter(fs);
			sw.Write(fileString);
			sw.Flush();
			sw.Close();
		}

		protected CompilationUnit ParseWrittenFile()
		{
			FileStream fs = File.OpenRead(m_FileName);
			Tokenizer t = new Tokenizer(fs, "", null);
			CompilationUnit ret = CompilationUnit.Parse(t);
			ret.SourceFilePath = m_FileName;
			fs.Close();
			return ret;
		}

		protected CompilationUnit ParseTestFile(string fileString)
		{
			WriteTestFile(fileString, m_FileName);
			return ParseWrittenFile();
		}
	}
}
