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
using NUnit.Framework;
using Opnieuw.Refactorings.RenamePrivateMethod;
using Opnieuw.Framework;
using Opnieuw.Parsers.CSParser;

namespace Opnieuw.Refactorings.RenamePrivateMethod.Test
{
	[TestFixture]
	public class RenamePrivateMethodTest
	{
/*
		[Test]
		public void testOnNodeSelectionChanged1()
		{
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  class TestClass\r\n";
			fileString += "  {\r\n";
			fileString += "    private void MethodToBeRenamed()\r\n";
			fileString += "    {\r\n";
			fileString += "    }\r\n";
			fileString += "  }\r\n";
			fileString += "}\r\n";
			CompilationUnit unit = ParseTestFile(fileString);

			ArrayList nodeList = new ArrayList();
			nodeList.Add(unit.Namespaces[0].Classes[0].Members[0]);
			RenamePrivateMethodRefactoring refactoring = new RenamePrivateMethodRefactoring();
			refactoring.AvailabilityChanged += new AvailabilityChangedHandler(this.OnAvailabilityChanged1);
			refactoring.OnNodeSelectionChanged(null, nodeList);
		}
		public void OnAvailabilityChanged1(bool isAvailable)
		{
			Assertion.AssertEquals("Expected isAvailable to be true, but it was not!", true, isAvailable);
		}

		[Test]
		public void testOnNodeSelectionChanged2()
		{
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  class TestClass\r\n";
			fileString += "  {\r\n";
			fileString += "    public void MethodToBeRenamed()\r\n";
			fileString += "    {\r\n";
			fileString += "    }\r\n";
			fileString += "  }\r\n";
			fileString += "}\r\n";
			CompilationUnit unit = ParseTestFile(fileString);

			ArrayList nodeList = new ArrayList();
			MethodDeclaration method = new MethodDeclaration();
			nodeList.Add(unit.Namespaces[0].Classes[0].Members[0]);
			RenamePrivateMethodRefactoring refactoring = new RenamePrivateMethodRefactoring();
			refactoring.AvailabilityChanged += new AvailabilityChangedHandler(this.OnAvailabilityChanged2);
			refactoring.OnNodeSelectionChanged(null, nodeList);
		}
		public void OnAvailabilityChanged2(bool isAvailable)
		{
			Assertion.AssertEquals("Expected isAvailable to be false, but it was not!", false, isAvailable);
		}

		[Test]
		public void testCodeChanges1()
		{
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  class TestClass\r\n";
			fileString += "  {\r\n";
			fileString += "    private void MethodToBeRenamed()\r\n";
			fileString += "    {\r\n";
			fileString += "    }\r\n";
			fileString += "  }\r\n";
			fileString += "}\r\n";
			CompilationUnit unit = ParseTestFile(fileString);

			RenamePrivateMethodRefactoring refactoring = new RenamePrivateMethodRefactoring();
			refactoring.NewMethodName = "RenamedMethod";
			ArrayList selectedNodes = new ArrayList();
			selectedNodes.Add(unit.Namespaces[0].Classes[0].Members[0]);
			refactoring.OnNodeSelectionChanged(null, selectedNodes);

			Assertion.AssertEquals("Expected one code change but didn't get it!", 1, refactoring.CodeChanges.Count);
			CodeChangeCommand command = refactoring.CodeChanges[0];
			Assertion.Assert("Command is the wrong type!", command is ReplaceCodeChangeCommand);
			ReplaceCodeChangeCommand replaceCommand = command as ReplaceCodeChangeCommand;
			Assertion.AssertEquals("File name of command is wrong!", m_FileName, replaceCommand.FileName);
			Assertion.AssertEquals("New method name in command is wrong!", "RenamedMethod", replaceCommand.CodeReplacements[0].NewCode);
			Assertion.AssertEquals("StartLine is wrong!", 4, replaceCommand.CodeReplacements[0].Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 18, replaceCommand.CodeReplacements[0].Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 4, replaceCommand.CodeReplacements[0].Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 35, replaceCommand.CodeReplacements[0].Position.EndCol);
		}

		[Test]
		public void testCodeChanges2()
		{
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  class TestClass\r\n";
			fileString += "  {\r\n";
			fileString += "    private void MethodToBeRenamed()\r\n";
			fileString += "    {\r\n";
			fileString += "    }\r\n";
			fileString += "    private void SomeOtherMethod()\r\n";
			fileString += "    {\r\n";
			fileString += "      MethodToBeRenamed();\r\n";
			fileString += "    }\r\n";
			fileString += "  }\r\n";
			fileString += "}\r\n";
			CompilationUnit unit = ParseTestFile(fileString);

			RenamePrivateMethodRefactoring refactoring = new RenamePrivateMethodRefactoring();
			refactoring.NewMethodName = "RenamedMethod";
			ArrayList selectedNodes = new ArrayList();
			selectedNodes.Add(unit.Namespaces[0].Classes[0].Members[0]);
			refactoring.OnNodeSelectionChanged(null, selectedNodes);

			Assertion.AssertEquals("Expected two code change but didn't get them all!", 2, refactoring.CodeChanges.Count);

			CodeChangeCommand command1 = refactoring.CodeChanges[0];
			Assertion.Assert("Command1 is the wrong type!", command1 is ReplaceCodeChangeCommand);
			ReplaceCodeChangeCommand replaceCommand1 = command1 as ReplaceCodeChangeCommand;
			Assertion.AssertEquals("File name of command1 is wrong!", m_FileName, replaceCommand1.FileName);
			Assertion.AssertEquals("New method name in command1 is wrong!", "RenamedMethod", replaceCommand1.CodeReplacements[0].NewCode);
			Assertion.AssertEquals("StartLine is wrong!", 9, replaceCommand1.CodeReplacements[0].Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 7, replaceCommand1.CodeReplacements[0].Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 9, replaceCommand1.CodeReplacements[0].Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 24, replaceCommand1.CodeReplacements[0].Position.EndCol);

			CodeChangeCommand command2 = refactoring.CodeChanges[1];
			Assertion.Assert("Command2 is the wrong type!", command2 is ReplaceCodeChangeCommand);
			ReplaceCodeChangeCommand replaceCommand2 = command2 as ReplaceCodeChangeCommand;
			Assertion.AssertEquals("File name of command2 is wrong!", m_FileName, replaceCommand2.FileName);
			Assertion.AssertEquals("New method name in command2 is wrong!", "RenamedMethod", replaceCommand2.CodeReplacements[0].NewCode);
			Assertion.AssertEquals("StartLine is wrong!", 4, replaceCommand2.CodeReplacements[0].Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 18, replaceCommand2.CodeReplacements[0].Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 4, replaceCommand2.CodeReplacements[0].Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 35, replaceCommand2.CodeReplacements[0].Position.EndCol);

		}

		[Test]
		public void testCodeChanges3()
		{
			string fileString = "";
			fileString += "namespace TestNamespace {\r\n";
			fileString += "  class TestClass\r\n";
			fileString += "  {\r\n";
			fileString += "    private bool MethodToBeRenamed()\r\n";
			fileString += "    {\r\n";
			fileString += "    }\r\n";
			fileString += "    private bool SomeOtherMethod()\r\n";
			fileString += "    {\r\n";
			fileString += "      return !MethodToBeRenamed() && MethodToBeRenamed();\r\n";
			fileString += "    }\r\n";
			fileString += "  }\r\n";
			fileString += "}\r\n";
			CompilationUnit unit = ParseTestFile(fileString);

			RenamePrivateMethodRefactoring refactoring = new RenamePrivateMethodRefactoring();
			refactoring.NewMethodName = "RenamedMethod";
			ArrayList selectedNodes = new ArrayList();
			selectedNodes.Add(unit.Namespaces[0].Classes[0].Members[0]);
			refactoring.OnNodeSelectionChanged(null, selectedNodes);

			Assertion.AssertEquals("Expected two code change but didn't get them all!", 2, refactoring.CodeChanges.Count);

			CodeChangeCommand command1 = refactoring.CodeChanges[0];
			Assertion.Assert("Command1 is the wrong type!", command1 is ReplaceCodeChangeCommand);
			ReplaceCodeChangeCommand replaceCommand1 = command1 as ReplaceCodeChangeCommand;
			Assertion.AssertEquals("Code replacement count is wrong!", 2, replaceCommand1.CodeReplacements.Count);
			Assertion.AssertEquals("File name of command1 is wrong!", m_FileName, replaceCommand1.FileName);
			
			Assertion.AssertEquals("New method name in code replacement 1 is wrong!", "RenamedMethod", replaceCommand1.CodeReplacements[0].NewCode);
			Assertion.AssertEquals("StartLine is wrong!", 9, replaceCommand1.CodeReplacements[0].Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 38, replaceCommand1.CodeReplacements[0].Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 9, replaceCommand1.CodeReplacements[0].Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 55, replaceCommand1.CodeReplacements[0].Position.EndCol);

			Assertion.AssertEquals("New method name in code replacement 1 is wrong!", "RenamedMethod", replaceCommand1.CodeReplacements[1].NewCode);
			Assertion.AssertEquals("StartLine is wrong!", 9, replaceCommand1.CodeReplacements[1].Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 15, replaceCommand1.CodeReplacements[1].Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 9, replaceCommand1.CodeReplacements[1].Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 32, replaceCommand1.CodeReplacements[1].Position.EndCol);

			CodeChangeCommand command2 = refactoring.CodeChanges[1];
			Assertion.Assert("Command2 is the wrong type!", command2 is ReplaceCodeChangeCommand);
			ReplaceCodeChangeCommand replaceCommand2 = command2 as ReplaceCodeChangeCommand;
			Assertion.AssertEquals("Code replacement count is wrong!", 1, replaceCommand2.CodeReplacements.Count);
			Assertion.AssertEquals("File name of command2 is wrong!", m_FileName, replaceCommand2.FileName);
			Assertion.AssertEquals("New method name in code replacement 2 is wrong!", "RenamedMethod", replaceCommand2.CodeReplacements[0].NewCode);
			Assertion.AssertEquals("StartLine is wrong!", 4, replaceCommand2.CodeReplacements[0].Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 18, replaceCommand2.CodeReplacements[0].Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 4, replaceCommand2.CodeReplacements[0].Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 35, replaceCommand2.CodeReplacements[0].Position.EndCol);
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
*/
    }
}
