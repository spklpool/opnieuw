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
using Opnieuw.Refactorings.GenerateMethod;
using Opnieuw.Framework;
using NUnit.Framework;

namespace Opnieuw.Refactorings.GenerateMethod.Test
{
	[TestFixture]
	public class GenerateMethodTest
	{
		[Test]
		public void testOnNodeSelectionChanged1()
		{
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  class TestClass\r\n";
			fileString += "  {\r\n";
			fileString += "    private void TestMethod()\r\n";
			fileString += "    {\r\n";
			fileString += "      TestClass2 class2 = new TestClass2();\r\n";
			fileString += "      class2.NonExistingMethod();\r\n";
			fileString += "    }\r\n";
			fileString += "  }\r\n";
			fileString += "  class TestClass2\r\n";
			fileString += "  {\r\n";
			fileString += "  }\r\n";
			fileString += "}\r\n";
			CompilationUnit unit = ParseTestFile(fileString);

			ArrayList nodeList = new ArrayList();
			nodeList.Add(unit.Namespaces[0].Classes[0].Methods[0].Body.Statements[1]);
			GenerateMethodRefactoring refactoring = new GenerateMethodRefactoring();
			refactoring.AvailabilityChanged += new AvailabilityChangedHandler(this.OnAvailabilityChanged1);
			refactoring.OnNodeSelectionChanged(null, nodeList);
		}
		public void OnAvailabilityChanged1(bool isAvailable)
		{
			Assertion.AssertEquals("Expected isAvailable to be false, but it was not!", false, isAvailable);
		}

		protected void WriteTestFile(string fileString, string fileName) {
			System.IO.FileStream fs = null;
			System.IO.StreamWriter sw = null;
			fs = System.IO.File.OpenWrite(fileName);
			sw = new System.IO.StreamWriter(fs);
			sw.Write(fileString);
			sw.Flush();
			sw.Close();
		}
		
		string m_FileName = "testFile.cs";
		protected CompilationUnit ParseTestFile(string fileString)
		{
			WriteTestFile(fileString, m_FileName);
			FileStream fs = File.OpenRead(m_FileName);
			Tokenizer t = new Tokenizer(fs, "", null);
			CompilationUnit ret = CompilationUnit.Parse(t);
			ret.SourceFilePath = m_FileName;
			fs.Close();
			return ret;
		}
	}
}
