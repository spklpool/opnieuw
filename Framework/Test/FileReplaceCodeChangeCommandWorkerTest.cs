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
using NUnit.Framework;

namespace Opnieuw.Framework.Test
{
	[TestFixture]
	public class FileReplaceCodeChangeCommandWorkerTest
	{
		[SetUp]
		public void SetUp()
		{
		}

		[TearDown]
		public void TearDown()
		{
 			System.IO.File.Delete(m_FileName);
		}

		private void WriteTestFile1()
		{
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  class TestClass\r\n";
			fileString += "  {\r\n";
			fileString += "    private void someMethod()\r\n";
			fileString += "    {\r\n";
			fileString += "      int a = 0;\r\n";
			fileString += "    }\r\n";
			fileString += "  }\r\n";
			fileString += "}\r\n";
			WriteTestFile(fileString);
		}

		[Test]
		public void testSingleLine()
		{
			WriteTestFile1();
			CodeReplacementCollection replacements = new CodeReplacementCollection();
			replacements.Add(new CodeReplacement(new Position(6, 7, 6, 16), "aaa"));
			ReplaceCodeChangeCommand command = new ReplaceCodeChangeCommand(m_FileName, replacements);
			FileReplaceCodeChangeCommandWorker worker = new FileReplaceCodeChangeCommandWorker(command);
			Assertion.AssertEquals("Command execution failed!", true, worker.Do());
			string expected = "namespace TestNamespace {\r\n  class TestClass\r\n  {\r\n    private void someMethod()\r\n    {\r\n      aaa;\r\n    }\r\n  }\r\n}\r\n";
			Assertion.AssertEquals("Resulting file is not as expected!", expected, ReadTestFile());
		}

		[Test]
		public void testEntireMethod()
		{
			WriteTestFile1();
			CodeReplacementCollection replacements = new CodeReplacementCollection();
			replacements.Add(new CodeReplacement(new Position(4, 5, 7, 6), "aaa"));
			ReplaceCodeChangeCommand command = new ReplaceCodeChangeCommand(m_FileName, replacements);
			FileReplaceCodeChangeCommandWorker worker = new FileReplaceCodeChangeCommandWorker(command);
			Assertion.AssertEquals("Command execution failed!", true, worker.Do());
			string expected = "namespace TestNamespace {\r\n  class TestClass\r\n  {\r\n    aaa\r\n  }\r\n}\r\n";
			Assertion.AssertEquals("Resulting file is not as expected!", expected, ReadTestFile());
		}

		[Test]
		public void testPartOfMethod1()
		{
			WriteTestFile1();
			CodeReplacementCollection replacements = new CodeReplacementCollection();
			replacements.Add(new CodeReplacement(new Position(4, 5, 6, 16), "aaa"));
			ReplaceCodeChangeCommand command = new ReplaceCodeChangeCommand(m_FileName, replacements);
			FileReplaceCodeChangeCommandWorker worker = new FileReplaceCodeChangeCommandWorker(command);
			Assertion.AssertEquals("Command execution failed!", true, worker.Do());
			string expected = "namespace TestNamespace {\r\n  class TestClass\r\n  {\r\n    aaa;\r\n    }\r\n  }\r\n}\r\n";
			Assertion.AssertEquals("Resulting file is not as expected!", expected, ReadTestFile());
		}

		[Test]
		public void testPartOfMethod2()
		{
			WriteTestFile1();
			CodeReplacementCollection replacements = new CodeReplacementCollection();
			replacements.Add(new CodeReplacement(new Position(3, 3, 4, 30), "aaa"));
			ReplaceCodeChangeCommand command = new ReplaceCodeChangeCommand(m_FileName, replacements);
			FileReplaceCodeChangeCommandWorker worker = new FileReplaceCodeChangeCommandWorker(command);
			Assertion.AssertEquals("Command execution failed!", true, worker.Do());
			string expected = "namespace TestNamespace {\r\n  class TestClass\r\n  aaa\r\n    {\r\n      int a = 0;\r\n    }\r\n  }\r\n}\r\n";
			Assertion.AssertEquals("Resulting file is not as expected!", expected, ReadTestFile());
		}

		[Test]
		public void testEntireFile()
		{
			WriteTestFile1();
			String replacement = "namespace ns {}";
			CodeReplacementCollection replacements = new CodeReplacementCollection();
			replacements.Add(new CodeReplacement(new InvalidPosition(), replacement));
			ReplaceCodeChangeCommand command = new ReplaceCodeChangeCommand(m_FileName, replacements);
			FileReplaceCodeChangeCommandWorker worker = new FileReplaceCodeChangeCommandWorker(command);
			Assertion.AssertEquals("Command execution failed!", true, worker.Do());
			string expected = "namespace ns {}";
			Assertion.AssertEquals("Resulting file is not as expected!", expected, ReadTestFile());
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

		string m_FileName = "TestFile.cs";
		protected void WriteTestFile(string fileString) {
			if (System.IO.File.Exists(m_FileName))
			{
				System.IO.File.Delete(m_FileName);
			}
			System.IO.FileStream fs = null;
			System.IO.StreamWriter sw = null;
			fs = System.IO.File.OpenWrite(m_FileName);
			sw = new System.IO.StreamWriter(fs);
			sw.Write(fileString);
			sw.Flush();
			sw.Close();
		}
	}
}
