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
using NUnit.Framework;
using Opnieuw.Framework;

namespace Opnieuw.Framework.Test
{
	[TestFixture]
	public class CodeCommandBackspaceTest : Assertion
	{
		[Test]
		public void TestDoUndo1()
		{
			//li[n]e1
			TestCode c = new TestCode("line1\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 3;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 4;

			CodeCommandProcessor processor = new CodeCommandProcessor();
			processor.Do(new CodeCommandBackspace(c));

			Assertion.AssertEquals("After Do : StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("After Do : StartCol is wrong!", 3, c.Selection.StartCol);
			Assertion.AssertEquals("After Do : EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("After Do : EndCol is wrong!", 3, c.Selection.EndCol);
			string expected = "lie1\r\n";
			Assertion.AssertEquals("Text after do is wrong", expected, c.Text);
			
			processor.Undo();
			
			Assertion.AssertEquals("After Undo : StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("After Undo : StartCol is wrong!", 4, c.Selection.StartCol);
			Assertion.AssertEquals("After Undo : EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("After Undo : EndCol is wrong!", 4, c.Selection.EndCol);
			expected = "line1\r\n";
			Assertion.AssertEquals("Text after undo is wrong", expected, c.Text);
		}
		
		[Test]
		public void TestDoUndo2()
		{
			//li[]ne1
			TestCode c = new TestCode("line1\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 3;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 3;

			CodeCommandProcessor processor = new CodeCommandProcessor();
			processor.Do(new CodeCommandBackspace(c));

			Assertion.AssertEquals("After Do : StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("After Do : StartCol is wrong!", 2, c.Selection.StartCol);
			Assertion.AssertEquals("After Do : EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("After Do : EndCol is wrong!", 2, c.Selection.EndCol);
			string expected = "lne1\r\n";
			Assertion.AssertEquals("Text after do is wrong", expected, c.Text);
			
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 1;
			
			processor.Undo();
			
			Assertion.AssertEquals("After Undo : StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("After Undo : StartCol is wrong!", 3, c.Selection.StartCol);
			Assertion.AssertEquals("After Undo : EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("After Undo : EndCol is wrong!", 3, c.Selection.EndCol);
			expected = "line1\r\n";
			Assertion.AssertEquals("Text after undo is wrong", expected, c.Text);

			c.Selection.StartLine = 1;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 1;

			processor.Redo();
			
			Assertion.AssertEquals("After Redo : StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("After Redo : StartCol is wrong!", 2, c.Selection.StartCol);
			Assertion.AssertEquals("After Redo : EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("After Redo : EndCol is wrong!", 2, c.Selection.EndCol);
			expected = "lne1\r\n";
			Assertion.AssertEquals("Text after undo is wrong", expected, c.Text);
		}
	}
}