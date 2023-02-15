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
using Opnieuw.Framework;

namespace Opnieuw.Framework.Test
{
	[TestFixture]
	public class FileCodeChangerTest : Assertion
	{
		[Test]
		public void testAddToEmptyFile()
		{
			string fileName = "AddToEmptyFile.cs";
			TextReader tr = null;
			string result = "";
			string expectedResult = "";

			//Prepare environment
			FileStream fs = File.Create(fileName);
			fs.Close();

			//Prepare command(s)
			CodeChangeCommandCollection col = new CodeChangeCommandCollection();
			col.Add(new ReplaceCodeChangeCommand(fileName, new CodeReplacementCollection(new CodeReplacement(new Position(1, 1, 1, 1), "namespace ns {}"))));

			//Execute the change
			FileCodeChanger fcc = new FileCodeChanger();
			Assert("Was able to undo before doing!", false == fcc.CanUndo);
			fcc.Do(col);

			//Verify the results
			tr = File.OpenText(fileName);
			result = tr.ReadToEnd();
			tr.Close();
			expectedResult = "namespace ns {}";
			AssertEquals("The text written to the file is incorrect!", expectedResult, result);

			//Undo and check results again.
			Assert("Was not able to undo after doing!", true == fcc.CanUndo);
			fcc.Undo();
			tr = File.OpenText(fileName);
			result = tr.ReadToEnd();
			tr.Close();
			expectedResult = "";
			AssertEquals("The text written to the file is incorrect!", expectedResult, result);

			//Clean up
			File.Delete(fileName);
		}

		[Test]
		public void testAddToStartOfExistingFile()
		{
			string fileName = "AddToStartOfExistingFile.cs";
			TextReader tr = null;
			string result = "";
			string expectedResult = "";

			//Prepare environment
			FileStream fs = File.Create(fileName);
			StreamWriter writerStream = new StreamWriter(fs);
			TextWriter writer = writerStream as TextWriter;
			writer.Write("namespace ns {}");
			writer.Close();
			fs.Close();

			//Prepare command(s)
			CodeChangeCommandCollection col = new CodeChangeCommandCollection();
			col.Add(new ReplaceCodeChangeCommand(fileName, new CodeReplacementCollection(new CodeReplacement(new Position(1, 1, 1, 1), "namespace anotherns {}"))));

			//Execute the change
			FileCodeChanger fcc = new FileCodeChanger();
			Assert("Was able to undo before doing!", false == fcc.CanUndo);
			fcc.Do(col);

			//Verify the results
			tr = File.OpenText(fileName);
			result = tr.ReadToEnd();
			tr.Close();
			expectedResult = "namespace anotherns {}namespace ns {}";
			AssertEquals("The text written to the file is incorrect!", expectedResult, result);

			//Undo and check results again.
			Assert("Was not able to undo after doing!", true == fcc.CanUndo);
			fcc.Undo();
			tr = File.OpenText(fileName);
			result = tr.ReadToEnd();
			tr.Close();
			expectedResult = "namespace ns {}";
			AssertEquals("The text written to the file is incorrect!", expectedResult, result);

			//Clean up
			File.Delete(fileName);
		}

		[Test]
		public void testAddToStartOfExistingFileWithMultipleLines()
		{
			string fileName = "AddToStartOfExistingFile.cs";
			TextReader tr = null;
			string result = "";
			string expectedResult = "";

			//Prepare environment
			FileStream fs = File.Create(fileName);
			StreamWriter writerStream = new StreamWriter(fs);
			TextWriter writer = writerStream as TextWriter;
			writer.Write("namespace ns\r\n{\r\n}");
			writer.Close();
			fs.Close();

			//Prepare command(s)
			CodeChangeCommandCollection col = new CodeChangeCommandCollection();
			col.Add(new ReplaceCodeChangeCommand(fileName, new CodeReplacementCollection(new CodeReplacement(new Position(1, 1, 1, 1), "namespace anotherns\r\n{\r\n}"))));

			//Execute the change
			FileCodeChanger fcc = new FileCodeChanger();
			Assert("Was able to undo before doing!", false == fcc.CanUndo);
			fcc.Do(col);

			//Verify the results
			tr = File.OpenText(fileName);
			result = tr.ReadToEnd();
			tr.Close();
			expectedResult = "namespace anotherns\r\n{\r\n}namespace ns\r\n{\r\n}";
			AssertEquals("The text written to the file is incorrect!", expectedResult, result);

			//Undo and check results again.
			Assert("Was not able to undo after doing!", true == fcc.CanUndo);
			fcc.Undo();
			tr = File.OpenText(fileName);
			result = tr.ReadToEnd();
			tr.Close();
			expectedResult = "namespace ns\r\n{\r\n}";
			AssertEquals("The text written to the file is incorrect!", expectedResult, result);

			//Clean up
			File.Delete(fileName);
		}

		[Test]
		public void testAddToStartOfLineWithTabs()
		{
			string fileName = "AddToStartOfLineWithTabs.cs";
			TextReader tr = null;
			string result = "";
			string expectedResult = "";

			//Prepare environment
			FileStream fs = File.Create(fileName);
			StreamWriter writerStream = new StreamWriter(fs);
			TextWriter writer = writerStream as TextWriter;
			writer.Write("namespace ns\r\n{\r\n\tclass someClass\r\n\t{\r\n\t}\r\n}");
			writer.Close();
			fs.Close();

			//Prepare command(s)
			CodeChangeCommandCollection col = new CodeChangeCommandCollection();
			col.Add(new ReplaceCodeChangeCommand(fileName, new CodeReplacementCollection(new CodeReplacement(new Position(3, 1, 3, 1), "\tclass someOtherClass\r\n\t{\r\n\t}\r\n"))));

			//Execute the change
			FileCodeChanger fcc = new FileCodeChanger();
			Assert("Was able to undo before doing!", false == fcc.CanUndo);
			fcc.Do(col);

			//Verify the results
			tr = File.OpenText(fileName);
			result = tr.ReadToEnd();
			tr.Close();
			expectedResult = "namespace ns\r\n{\r\n\tclass someOtherClass\r\n\t{\r\n\t}\r\n\tclass someClass\r\n\t{\r\n\t}\r\n}";
			AssertEquals("The text written to the file is incorrect!", expectedResult, result);

			//Undo and check results again.
			Assert("Was not able to undo after doing!", true == fcc.CanUndo);
			fcc.Undo();
			tr = File.OpenText(fileName);
			result = tr.ReadToEnd();
			tr.Close();
			expectedResult = "namespace ns\r\n{\r\n\tclass someClass\r\n\t{\r\n\t}\r\n}";
			AssertEquals("The text written to the file is incorrect!", expectedResult, result);

			//Clean up
			File.Delete(fileName);
		}

		[Test]
		public void testReplaceTabInStartOfLine()
		{
			string fileName = "AddToStartOfLineWithTabs.cs";
			TextReader tr = null;
			string result = "";
			string expectedResult = "";

			//Prepare environment
			FileStream fs = File.Create(fileName);
			StreamWriter writerStream = new StreamWriter(fs);
			TextWriter writer = writerStream as TextWriter;
			writer.Write("namespace ns\r\n{\r\n\tclass someClass\r\n\t{\r\n\t}\r\n}");
			writer.Close();
			fs.Close();

			//Prepare command(s)
			CodeChangeCommandCollection col = new CodeChangeCommandCollection();
			col.Add(new ReplaceCodeChangeCommand(fileName, new CodeReplacementCollection(new CodeReplacement(new Position(3, 1, 3, 2), "\tclass someOtherClass\r\n\t{\r\n\t}\r\n"))));

			//Execute the change
			FileCodeChanger fcc = new FileCodeChanger();
			Assert("Was able to undo before doing!", false == fcc.CanUndo);
			fcc.Do(col);

			//Verify the results
			tr = File.OpenText(fileName);
			result = tr.ReadToEnd();
			tr.Close();
			expectedResult = "namespace ns\r\n{\r\n\tclass someOtherClass\r\n\t{\r\n\t}\r\nclass someClass\r\n\t{\r\n\t}\r\n}";
			AssertEquals("The text written to the file is incorrect!", expectedResult, result);

			//Undo and check results again.
			Assert("Was not able to undo after doing!", true == fcc.CanUndo);
			fcc.Undo();
			tr = File.OpenText(fileName);
			result = tr.ReadToEnd();
			tr.Close();
			expectedResult = "namespace ns\r\n{\r\n\tclass someClass\r\n\t{\r\n\t}\r\n}";
			AssertEquals("The text written to the file is incorrect!", expectedResult, result);

			//Clean up
			File.Delete(fileName);
		}

		[Test]
		public void testReplaceTextSpanningMultipleLines()
		{
			string fileName = "ReplaceTextSpanningMultipleLines.cs";
			TextReader tr = null;
			string result = "";
			string expectedResult = "";

			//Prepare environment
			FileStream fs = File.Create(fileName);
			StreamWriter writerStream = new StreamWriter(fs);
			TextWriter writer = writerStream as TextWriter;
			writer.Write("namespace ns\r\n{\r\n\tclass someClass\r\n\t{\r\n\t}\r\n}");
			writer.Close();
			fs.Close();

			//Prepare command(s)
			CodeChangeCommandCollection col = new CodeChangeCommandCollection();
			col.Add(new ReplaceCodeChangeCommand(fileName, new CodeReplacementCollection(new CodeReplacement(new Position(3, 2, 6, 1), "class someOtherClass\r\n\t{\r\n\t}\r\n"))));

			//Execute the change
			FileCodeChanger fcc = new FileCodeChanger();
			Assert("Was able to undo before doing!", false == fcc.CanUndo);
			fcc.Do(col);

			//Verify the results
			tr = File.OpenText(fileName);
			result = tr.ReadToEnd();
			tr.Close();
			expectedResult = "namespace ns\r\n{\r\n\tclass someOtherClass\r\n\t{\r\n\t}\r\n}";
			AssertEquals("The text written to the file is incorrect!", expectedResult, result);

			//Undo and check results again.
			Assert("Was not able to undo after doing!", true == fcc.CanUndo);
			fcc.Undo();
			tr = File.OpenText(fileName);
			result = tr.ReadToEnd();
			tr.Close();
			expectedResult = "namespace ns\r\n{\r\n\tclass someClass\r\n\t{\r\n\t}\r\n}";
			AssertEquals("The text written to the file is incorrect!", expectedResult, result);

			//Clean up
			File.Delete(fileName);
		}

		[Test]
		public void testMultipleReplaceInSameFile()
		{
			string fileName = "ReplaceTextSpanningMultipleLines.cs";
			TextReader tr = null;
			string result = "";
			string expectedResult = "";

			//Prepare environment
			FileStream fs = File.Create(fileName);
			StreamWriter writerStream = new StreamWriter(fs);
			TextWriter writer = writerStream as TextWriter;
			writer.Write("namespace ns\r\n{\r\n\tclass someClass\r\n\t{\r\n\t\tint a, b, c;\r\n\t}\r\n}");
			writer.Close();
			fs.Close();

			//Prepare command(s)
			CodeChangeCommandCollection col = new CodeChangeCommandCollection();
			CodeReplacementCollection codeReplacements = new CodeReplacementCollection();
			codeReplacements.Add(new CodeReplacement(new Position(5, 7, 5, 8), "A1"));
			codeReplacements.Add(new CodeReplacement(new Position(5, 10, 5, 11), "B2"));
			codeReplacements.Add(new CodeReplacement(new Position(5, 13, 5, 14), "C3"));
			col.Add(new ReplaceCodeChangeCommand(fileName, codeReplacements));

			//Execute the change
			FileCodeChanger fcc = new FileCodeChanger();
			Assert("Was able to undo before doing!", false == fcc.CanUndo);
			fcc.Do(col);

			//Verify the results
			tr = File.OpenText(fileName);
			result = tr.ReadToEnd();
			tr.Close();
			expectedResult = "namespace ns\r\n{\r\n\tclass someClass\r\n\t{\r\n\t\tint A1, B2, C3;\r\n\t}\r\n}";
			AssertEquals("The text written to the file is incorrect!", expectedResult, result);

			//Undo and check results again.
			Assert("Was not able to undo after doing!", true == fcc.CanUndo);
			fcc.Undo();
			tr = File.OpenText(fileName);
			result = tr.ReadToEnd();
			tr.Close();
			expectedResult = "namespace ns\r\n{\r\n\tclass someClass\r\n\t{\r\n\t\tint a, b, c;\r\n\t}\r\n}";
			AssertEquals("The text written to the file is incorrect!", expectedResult, result);

			//Clean up
			File.Delete(fileName);
		}
	}
}