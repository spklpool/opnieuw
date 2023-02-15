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
using System.Drawing;
using System.Collections;
using NUnit.Framework;
using Opnieuw.Framework;

namespace Opnieuw.Framework.Test
{
	public class TestCode1 : Code
	{
		public TestCode1(string text) :
			base(text)
		{
		}

		protected override CodeLine CreateLine(string text)
		{
			char[] CHARS_TO_TRIM = new char[2] {'\r', '\n'};
			return new TestCodeLine1(text.Trim(CHARS_TO_TRIM));
		}
	}
	
	public class TestCodeLine1 : CodeLine
	{
		public TestCodeLine1(string text) :
			base(text)
		{
		}

		public override ColoredStringPartCollection PartsUpTo(int columnIndex)
		{
			ColoredStringPartCollection ret = new ColoredStringPartCollection();
			ret.Add(new ColoredStringPart(0, m_Text.Substring(0, m_Text.Length/2)));
			ret.Add(new ColoredStringPart(m_Text.Length/2, m_Text.Substring(m_Text.Length/2, (m_Text.Length/2)+1)));
			return ret;
		}
	}
	
	[TestFixture]
	public class CodeTest : Assertion
	{
		[Test]
		public void testLines1()
		{
			TestCode c = new TestCode("line1\r\nline2\r\n");
			Assertion.AssertEquals("Wrong number of lines!", 2, c.Lines.Count);
		}

		[Test]
		public void testEnter1()
		{
			//[]line1
			TestCode c = new TestCode("line1");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 1;
			
			Assertion.AssertEquals("Line count before enter is wrong!", 1, c.Lines.Count);
			
			c.Enter();
			
			Assertion.AssertEquals("Line count after enter is wrong!", 2, c.Lines.Count);
			Assertion.AssertEquals("Line 1 text is wrong!", "", c.Lines[0].Text);
			Assertion.AssertEquals("Line 2 text is wrong!", "line1", c.Lines[1].Text);
		}

		[Test]
		public void testEnter2()
		{
			//l[]ine1
			TestCode c = new TestCode("line1");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 2;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 2;
			
			Assertion.AssertEquals("Line count before enter is wrong!", 1, c.Lines.Count);
			
			c.Enter();
			
			Assertion.AssertEquals("Line count after enter is wrong!", 2, c.Lines.Count);
			Assertion.AssertEquals("Line 1 text is wrong!", "l", c.Lines[0].Text);
			Assertion.AssertEquals("Line 2 text is wrong!", "ine1", c.Lines[1].Text);
		}

		[Test]
		public void testEnter3()
		{
			//line1[]
			TestCode c = new TestCode("line1");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 6;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 6;
			
			Assertion.AssertEquals("Line count before enter is wrong!", 1, c.Lines.Count);
			
			c.Enter();
			
			Assertion.AssertEquals("Line count after enter is wrong!", 2, c.Lines.Count);
			Assertion.AssertEquals("Line 1 text is wrong!", "line1", c.Lines[0].Text);
			Assertion.AssertEquals("Line 2 text is wrong!", "", c.Lines[1].Text);
		}

		[Test]
		public void testEnter4()
		{
			//\tline1[]
			TestCode c = new TestCode("\tline1");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 10;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 10;
			
			Assertion.AssertEquals("Line count before enter is wrong!", 1, c.Lines.Count);
			
			c.Enter();
			
			Assertion.AssertEquals("Line count after enter is wrong!", 2, c.Lines.Count);
			Assertion.AssertEquals("Line 1 text is wrong!", "\tline1", c.Lines[0].Text);
			Assertion.AssertEquals("Line 2 text is wrong!", "", c.Lines[1].Text);
		}

		[Test]
		public void testEnter5()
		{
			//\tli[n]e1
			//\tline2
			TestCode c = new TestCode("\tline1\r\n\tline2");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 7;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 8;
			
			Assertion.AssertEquals("Line count before enter is wrong!", 2, c.Lines.Count);
			
			c.Enter();
			
			Assertion.AssertEquals("Line count after enter is wrong!", 3, c.Lines.Count);
			Assertion.AssertEquals("Line 1 text is wrong!", "\tli", c.Lines[0].Text);
			Assertion.AssertEquals("Line 2 text is wrong!", "e1", c.Lines[1].Text);
			Assertion.AssertEquals("Line 3 text is wrong!", "\tline2", c.Lines[2].Text);
		}
		
		[Test]
		public void testKeyPress1()
		{
			//l[]ine1
			TestCode c = new TestCode("line1");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 2;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 2;
			
			c.KeyPress('a');
			
			Assertion.AssertEquals("Line text is wrong!", "laine1", c.Lines[0].Text);
		}

		[Test]
		public void testSplitForSelection1()
		{
			//p[a]rt1
			TestCodeLine cl = new TestCodeLine("part1");
			CodeSelection selection = new CodeSelection(1, 2, 1, 3);
			
			cl.SplitForSelection(selection, 1);
			ColoredStringPartCollection parts = cl.SelectionParts;
			
			Assertion.AssertEquals("Wrong number of parts!", 3, parts.Count);
			Assertion.AssertEquals("part 1 is wrong!", "p", parts[0].Text);
			Assertion.AssertEquals("part 2 is wrong!", "a", parts[1].Text);
			Assertion.AssertEquals("part 3 is wrong!", "rt1", parts[2].Text);
		}

		[Test]
		public void testSplitForSelection2()
		{
			//[par]t1
			TestCodeLine cl = new TestCodeLine("part1");
			CodeSelection selection = new CodeSelection(1, 1, 1, 4);
			
			cl.SplitForSelection(selection, 1);
			ColoredStringPartCollection parts = cl.SelectionParts;

			Assertion.AssertEquals("Wrong number of parts!", 2, parts.Count);
			Assertion.AssertEquals("part 1 is wrong!", "par", parts[0].Text);
			Assertion.AssertEquals("part 2 is wrong!", "t1", parts[1].Text);
		}

		[Test]
		public void testSplitForSelection3()
		{
			//[part]1
			TestCodeLine cl = new TestCodeLine("part1");
			CodeSelection selection = new CodeSelection(1, 1, 1, 5);
			
			cl.SplitForSelection(selection, 1);
			ColoredStringPartCollection parts = cl.SelectionParts;

			Assertion.AssertEquals("Wrong number of parts!", 2, parts.Count);
			Assertion.AssertEquals("part 1 is wrong!", "part", parts[0].Text);
			Assertion.AssertEquals("part 2 is wrong!", "1", parts[1].Text);
		}

		[Test]
		public void testSplitForSelection4()
		{
			//pa[rt1
			TestCodeLine cl1 = new TestCodeLine("part1");
			//pa]rt2
			TestCodeLine cl2 = new TestCodeLine("part2");
			CodeSelection selection = new CodeSelection(1, 3, 2, 3);
			
			cl1.SplitForSelection(selection, 1);
			ColoredStringPartCollection parts1 = cl1.SelectionParts;

			Assertion.AssertEquals("Wrong number of parts!", 2, parts1.Count);
			Assertion.AssertEquals("part 1 is wrong!", "pa", parts1[0].Text);
			Assertion.AssertEquals("part 2 is wrong!", "rt1", parts1[1].Text);

			cl2.SplitForSelection(selection, 2);
			ColoredStringPartCollection parts2 = cl2.SelectionParts;

			Assertion.AssertEquals("Wrong number of parts!", 2, parts2.Count);
			Assertion.AssertEquals("part 1 is wrong!", "pa", parts2[0].Text);
			Assertion.AssertEquals("part 2 is wrong!", "rt2", parts2[1].Text);
		}

		[Test]
		public void testSplitForSelection5()
		{
			//pa[rt1]
			TestCode c = new TestCode("part1");
			TestCodeLine cl = new TestCodeLine("part1");
			CodeSelection selection = new CodeSelection(1, 3, 1, 6);
			
			cl.SplitForSelection(selection, 1);
			ColoredStringPartCollection parts = cl.SelectionParts;

			Assertion.AssertEquals("Wrong number of parts!", 2, parts.Count);
			Assertion.AssertEquals("part 1 is wrong!", "pa", parts[0].Text);
			Assertion.AssertEquals("part 2 is wrong!", "rt1", parts[1].Text);
		}

		[Test]
		public void testSplitForSelection6()
		{
			//[/t1]234567890
			TestCodeLine cl = new TestCodeLine("\t1234567890");
			CodeSelection selection = new CodeSelection(1, 1, 1, 6);
			
			cl.SplitForSelection(selection, 1);
			ColoredStringPartCollection parts = cl.SelectionParts;

			Assertion.AssertEquals("Wrong number of parts!", 2, parts.Count);
			Assertion.AssertEquals("part 1 is wrong!", "\t1", parts[0].Text);
			Assertion.AssertEquals("part 2 is wrong!", "234567890", parts[1].Text);
		}

		[Test]
		public void testSplitForSelection7()
		{
			///t12[345
			TestCodeLine cl1 = new TestCodeLine("\t12345");
			//\t\t67]890
			TestCodeLine cl2 = new TestCodeLine("\t\t67890");
			CodeSelection selection = new CodeSelection(1, 7, 2, 11);
			
			cl1.SplitForSelection(selection, 1);
			ColoredStringPartCollection parts = cl1.SelectionParts;

			Assertion.AssertEquals("Wrong number of parts!", 2, parts.Count);
			Assertion.AssertEquals("part 1 is wrong!", "\t12", parts[0].Text);
			Assertion.AssertEquals("part 2 is wrong!", "345", parts[1].Text);
		}

		[Test]
		public void testSplitForSelection8()
		{
			//12[3 456
			TestCodeLine1 cl1 = new TestCodeLine1("123 456");
			//78]9 0
			TestCodeLine1 cl2 = new TestCodeLine1("789 0");
			CodeSelection selection = new CodeSelection(1, 3, 2, 3);
			
			cl1.SplitForSelection(selection, 1);
			ColoredStringPartCollection parts = cl1.SelectionParts;

			Assertion.AssertEquals("Wrong number of parts!", 3, parts.Count);
			Assertion.AssertEquals("part 1 is wrong!", "12", parts[0].Text);
			Assertion.AssertEquals("parts[0] BackColor is wrong", cl1.BackColor, parts[0].BackColor);
			Assertion.AssertEquals("part 2 is wrong!", "3", parts[1].Text);
			Assertion.AssertEquals("parts[1] BackColor is wrong", cl1.SelectedBackColor, parts[1].BackColor);
			Assertion.AssertEquals("part 2 is wrong!", " 456", parts[2].Text);
			Assertion.AssertEquals("parts[2] BackColor is wrong", cl1.SelectedBackColor, parts[2].BackColor);
		}

		[Test]
		public void testSplitForSelection9()
		{

			//123[456789
			TestCodeLine cl1 = new TestCodeLine("123456789");
			//\t123
			TestCodeLine cl2 = new TestCodeLine("\t123");
			//\t12]3456789
			TestCodeLine cl3 = new TestCodeLine("\t123456789");

			CodeSelection selection = new CodeSelection(1, 4, 3, 7);
			
			cl1.SplitForSelection(selection, 1);
			ColoredStringPartCollection parts1 = cl1.SelectionParts;
			Assertion.AssertEquals("Wrong number of parts!", 2, parts1.Count);
			Assertion.AssertEquals("part 1 is wrong!", "123", parts1[0].Text);
			Assertion.AssertEquals("parts1[0] BackColor is wrong", cl1.BackColor, parts1[0].BackColor);
			Assertion.AssertEquals("part 2 is wrong!", "456789", parts1[1].Text);
			Assertion.AssertEquals("parts1[1] BackColor is wrong", cl1.SelectedBackColor, parts1[1].BackColor);

			cl2.SplitForSelection(selection, 2);
			ColoredStringPartCollection parts2 = cl2.SelectionParts;
			Assertion.AssertEquals("Wrong number of parts!", 1, parts2.Count);
			Assertion.AssertEquals("part 1 is wrong!", "\t123", parts2[0].Text);
			Assertion.AssertEquals("parts2[0] BackColor is wrong", cl2.SelectedBackColor, parts2[0].BackColor);

			cl3.SplitForSelection(selection, 3);
			ColoredStringPartCollection parts3 = cl3.SelectionParts;
			Assertion.AssertEquals("Wrong number of parts!", 2, parts3.Count);
			Assertion.AssertEquals("part 1 is wrong!", "\t12", parts3[0].Text);
			Assertion.AssertEquals("parts3[0] BackColor is wrong", cl2.SelectedBackColor, parts3[0].BackColor);
			Assertion.AssertEquals("part 2 is wrong!", "3456789", parts3[1].Text);
			Assertion.AssertEquals("parts3[1] BackColor is wrong", cl2.BackColor, parts3[1].BackColor);
		}

		[Test]
		public void testSplitForSelection10()
		{
			//[123456]78
			TestCodeLine1 cl = new TestCodeLine1("123456789");
			CodeSelection selection = new CodeSelection(1, 1, 1, 7);
			
			cl.SplitForSelection(selection, 1);
			ColoredStringPartCollection parts = cl.SelectionParts;
			
			Assertion.AssertEquals("Wrong number of parts!", 3, parts.Count);
			Assertion.AssertEquals("part 1 is wrong!", "1234", parts[0].Text);
			Assertion.AssertEquals("parts1[0] BackColor is wrong", cl.SelectedBackColor, parts[0].BackColor);
			Assertion.AssertEquals("part 2 is wrong!", "56", parts[1].Text);
			Assertion.AssertEquals("parts1[1] BackColor is wrong", cl.SelectedBackColor, parts[1].BackColor);
			Assertion.AssertEquals("part 3 is wrong!", "789", parts[2].Text);
			Assertion.AssertEquals("parts1[2] BackColor is wrong", cl.BackColor, parts[2].BackColor);
		}

		[Test]
		public void testMoveCursorLeft1()
		{
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 3;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 3;

			c.MoveCursorLeft();

			Assertion.AssertEquals("StartLine is wrong!", 2, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 2, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 2, c.Selection.EndCol);
		}

		[Test]
		public void testMoveCursorLeft2()
		{
			//line1
			//line1
			//[]
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 3;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 3;
			c.Selection.EndCol = 1;

			c.MoveCursorLeft();

			Assertion.AssertEquals("StartLine is wrong!", 2, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 6, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 6, c.Selection.EndCol);
		}

		[Test]
		public void testMoveCursorLeft3()
		{
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 1;

			c.MoveCursorLeft();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 1, c.Selection.EndCol);
		}

		[Test]
		public void testMoveCursorLeft4()
		{
			//    []line1
			//    line2
			TestCode c = new TestCode("\tline1\r\n\tline2");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 5;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 5;

			c.MoveCursorLeft();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 1, c.Selection.EndCol);
		}

		[Test]
		public void testMoveCursorLeft5()
		{
			TestCode c = new TestCode("\tline1\r\n\tline2");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 1;

			c.MoveCursorLeft();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 10, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 10, c.Selection.EndCol);
		}

		[Test]
		public void testMoveCursorLeft6()
		{
			TestCode c = new TestCode("line1");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 2;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 4;

			c.MoveCursorLeft();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 2, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 2, c.Selection.EndCol);
		}

		[Test]
		public void testMoveCursorLeft7()
		{
			TestCode c = new TestCode(" \tline1");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 5;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 5;

			c.MoveCursorLeft();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 2, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 2, c.Selection.EndCol);
		}

		[Test]
		public void testMoveCursorRight1()
		{
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 1;

			c.MoveCursorRight();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 2, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 2, c.Selection.EndCol);
		}

		[Test]
		public void testMoveCursorRight2()
		{
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 6;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 6;

			c.MoveCursorRight();

			Assertion.AssertEquals("StartLine is wrong!", 2, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 1, c.Selection.EndCol);
		}

		[Test]
		public void testMoveCursorRight3()
		{
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 6;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 6;

			c.MoveCursorRight();

			Assertion.AssertEquals("StartLine is wrong!", 2, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 6, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 6, c.Selection.EndCol);
		}

		[Test]
		public void testMoveCursorRight4()
		{
			TestCode c = new TestCode("\tline1\r\n\tline2");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 1;

			c.MoveCursorRight();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 5, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 5, c.Selection.EndCol);
		}

		[Test]
		public void testMoveCursorRight5()
		{
			TestCode c = new TestCode("\t\tline2");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 1;

			c.MoveCursorRight();
			c.MoveCursorRight();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 9, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 9, c.Selection.EndCol);
		}

		[Test]
		public void testMoveCursorRight6()
		{
			TestCode c = new TestCode("\t\tline2");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 13;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 13;

			c.MoveCursorRight();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 14, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 14, c.Selection.EndCol);
		}

		[Test]
		public void testMoveCursorRight7()
		{
			TestCode c = new TestCode("\t \tline2");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 5;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 5;

			c.MoveCursorRight();
			c.MoveCursorRight();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 9, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 9, c.Selection.EndCol);
		}

		[Test]
		public void testMoveCursorRight8()
		{
			TestCode c = new TestCode("  \t \tline2");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 5;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 5;

			c.MoveCursorRight();
			c.MoveCursorRight();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 9, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 9, c.Selection.EndCol);
		}

		[Test]
		public void testMoveCursorRight9()
		{
			TestCode c = new TestCode("   \t \tline2");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 5;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 5;

			c.MoveCursorRight();
			c.MoveCursorRight();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 9, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 9, c.Selection.EndCol);
		}

		[Test]
		public void testMoveCursorRight10()
		{
			TestCode c = new TestCode("1\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 1;

			c.MoveCursorRight();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 2, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 2, c.Selection.EndCol);
		}

		[Test]
		public void testMoveCursorUp1()
		{
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 1;

			c.MoveCursorUp();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 1, c.Selection.EndCol);
		}

		[Test]
		public void testMoveCursorUp2()
		{
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 1;

			c.MoveCursorUp();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 1, c.Selection.EndCol);
		}

		[Test]
		public void testMoveCursorUp3()
		{
			TestCode c = new TestCode("1\r\nline2\r\n");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 3;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 3;

			c.MoveCursorUp();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 2, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 2, c.Selection.EndCol);
		}

		[Test]
		public void testMoveCursorUp4()
		{
			TestCode c = new TestCode("\t1\r\n\tline2\r\n");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 8;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 8;

			c.MoveCursorUp();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 6, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 6, c.Selection.EndCol);
		}

		[Test]
		public void testMoveCursorUp5()
		{
			TestCode c = new TestCode("\t\tline1\r\n\tline2\r\n");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 6;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 6;

			c.MoveCursorUp();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 5, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 5, c.Selection.EndCol);
		}

		[Test]
		public void testMoveCursorDown1()
		{
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 1;

			c.MoveCursorDown();

			Assertion.AssertEquals("StartLine is wrong!", 2, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 1, c.Selection.EndCol);
		}

		[Test]
		public void testMoveCursorDown2()
		{
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 1;

			c.MoveCursorDown();

			Assertion.AssertEquals("StartLine is wrong!", 2, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 1, c.Selection.EndCol);
		}

		[Test]
		public void testMoveCursorDown3()
		{
			//li[]ne1
			//2
			//
			TestCode c = new TestCode("line1\r\n2\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 3;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 3;

			c.MoveCursorDown();

			Assertion.AssertEquals("StartLine is wrong!", 2, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 2, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 2, c.Selection.EndCol);
		}

		[Test]
		public void testMoveCursorDown4()
		{
			TestCode c = new TestCode("\tline1\r\n\t2\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 8;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 8;

			c.MoveCursorDown();

			Assertion.AssertEquals("StartLine is wrong!", 2, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 6, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 6, c.Selection.EndCol);
		}

		[Test]
		public void testMoveCursorDown5()
		{
			TestCode c = new TestCode("\tline1\r\n\t\tline2\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 6;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 6;

			c.MoveCursorDown();

			Assertion.AssertEquals("StartLine is wrong!", 2, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 5, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 5, c.Selection.EndCol);
		}

		[Test]
		public void testBackspace1()
		{
			//li[]ne1
			//line2
			//
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 3;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 3;

			c.Backspace();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 2, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 2, c.Selection.EndCol);
			Assertion.AssertEquals("Text after backapace is wrong!", "lne1\r\nline2\r\n", c.Text);
		}

		[Test]
		public void testBackspace2()
		{
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 1;

			c.Backspace();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 1, c.Selection.EndCol);
			Assertion.AssertEquals("Text after backapace is wrong!", "line1\r\nline2\r\n", c.Text);
		}

		[Test]
		public void testBackspace3()
		{
			//line1
			//[]line2
			//
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 1;

			c.Backspace();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 6, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 6, c.Selection.EndCol);
			Assertion.AssertEquals("Text after backapace is wrong!", "line1line2\r\n", c.Text);
		}

		[Test]
		public void testBackspace4()
		{
			TestCode c = new TestCode("line1\r\nline2\r\nline3\r\n");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 2;

			c.Backspace();

			Assertion.AssertEquals("StartLine is wrong!", 2, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 1, c.Selection.EndCol);
			Assertion.AssertEquals("Text after backapace is wrong!", "line1\r\nine2\r\nline3\r\n", c.Text);
		}

		[Test]
		public void testBackspace5()
		{
			TestCode c = new TestCode("\tline1\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 7;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 7;

			c.Backspace();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 6, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 6, c.Selection.EndCol);
			Assertion.AssertEquals("Text after backapace is wrong!", "\tlne1\r\n", c.Text);
		}

		[Test]
		public void testBackspace6()
		{
			//    line1
			//[]line2
			//
			TestCode c = new TestCode("\tline1\r\nline2\r\n");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 1;

			c.Backspace();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 10, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 10, c.Selection.EndCol);
			Assertion.AssertEquals("Text after backapace is wrong!", "\tline1line2\r\n", c.Text);
		}

		[Test]
		public void testDelete1()
		{
			//    li[]ne1
			//
			TestCode c = new TestCode("\tline1\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 7;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 7;

			c.Delete();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 7, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 7, c.Selection.EndCol);
			Assertion.AssertEquals("Text after delete is wrong!", "\tlie1\r\n", c.Text);
		}
		
		[Test]
		public void testHome1()
		{
			//line1
			//li[]ne2
			//line3
			TestCode c = new TestCode("line1\r\nline2\r\nline3");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 3;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 3;

			c.Home();

			Assertion.AssertEquals("StartLine is wrong!", 2, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 1, c.Selection.EndCol);
		}

		[Test]
		public void testEnd1()
		{
			//line1
			//[]line2
			//line3
			//
			TestCode c = new TestCode("line1\r\nline2\r\nline3\r\n");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 1;

			c.End();

			Assertion.AssertEquals("StartLine is wrong!", 2, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 6, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 6, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionRight1()
		{
			//[]line1
			//line2
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 1;

			c.ExtendSelectionRight();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 2, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionRight2()
		{
			//line1[]
			//line2
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 6;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 6;

			c.ExtendSelectionRight();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 6, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 1, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionRight3()
		{
			//line1[]
			//    line2
			TestCode c = new TestCode("line1\r\n\tline2\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 6;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 6;

			c.ExtendSelectionRight();
			c.ExtendSelectionRight();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 6, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 5, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionRight4()
		{
			//    line1[]
			//    line2
			TestCode c = new TestCode("\tline1\r\n\tline2\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 10;
			c.Selection.IsCursorAtStart = true;

			c.ExtendSelectionRight();
			c.ExtendSelectionRight();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 6, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 10, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionRight5()
		{
			//[]    line1
			TestCode c = new TestCode("\tline1\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 1;
			c.Selection.IsCursorAtStart = true;

			c.ExtendSelectionRight();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 5, c.Selection.EndCol);

			CodeLine line = c.Lines[0];
			ColoredStringPartCollection parts = line.SelectionParts;
			Assertion.AssertEquals("Number of parts is wrong!", 2, parts.Count);
			Assertion.AssertEquals("Part1 text is wrong!", "\t", parts[0].Text);
			Assertion.AssertEquals("Part1 back color is wrong!", Color.DarkBlue, parts[0].BackColor);
			Assertion.AssertEquals("Part2 text is wrong!", "line1", parts[1].Text);
			Assertion.AssertEquals("Part2 back color is wrong!", Color.White, parts[1].BackColor);
		}

		[Test]
		public void testExtendSelectionRight6()
		{
			//[]1
			TestCode c = new TestCode("1\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 1;

			c.ExtendSelectionRight();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 2, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionRight7()
		{
			//line1[
			//li]ne2
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 6;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 3;
			c.Selection.IsCursorAtStart = true;

			c.ExtendSelectionRight();

			Assertion.AssertEquals("StartLine is wrong!", 2, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 3, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionRight8()
		{
			//li[]  ne1
			TestCode c = new TestCode("li\tne1");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 3;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 3;
			c.Selection.IsCursorAtStart = true;

			c.ExtendSelectionRight();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 5, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionToHome1()
		{
			TestCode c = new TestCode("1\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 2;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 2;

			c.ExtendSelectionToHome();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 2, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionToHome2()
		{
			//li[ne1
			//li]ne2
			TestCode c = new TestCode("line1\r\nline2");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 2;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 2;
			c.Selection.IsCursorAtStart = true;

			c.ExtendSelectionToHome();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 2, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionToHome3()
		{
			//li[ne1
			//li]ne2
			TestCode c = new TestCode("line1\r\nline2");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 2;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 2;
			c.Selection.IsCursorAtEnd = true;

			c.ExtendSelectionToHome();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 2, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 1, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionToEnd1()
		{
			TestCode c = new TestCode("1\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 1;
			c.Selection.IsCursorAtStart = true;

			c.ExtendSelectionToEnd();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 2, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionToEnd2()
		{
			TestCode c = new TestCode("12\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 1;
			c.Selection.IsCursorAtStart = true;

			c.ExtendSelectionToEnd();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 3, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionToEnd3()
		{
			TestCode c = new TestCode("12\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 3;
			c.Selection.IsCursorAtEnd = true;

			c.ExtendSelectionToEnd();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 3, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionToEnd4()
		{
			TestCode c = new TestCode("\t12\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 5;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 6;
			c.Selection.IsCursorAtEnd = true;

			c.ExtendSelectionToEnd();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 5, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 7, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionToEnd5()
		{
			//12
			//34
			TestCode c = new TestCode("12\r\n34");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 1;
			c.Selection.IsCursorAtStart = true;

			c.ExtendSelectionToEnd();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 1, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionToEnd6()
		{
			//12
			//34
			TestCode c = new TestCode("12\r\n34");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 1;
			c.Selection.IsCursorAtEnd = true;

			c.ExtendSelectionToEnd();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 3, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionToEnd7()
		{
			//1234
			//56
			TestCode c = new TestCode("1234\r\n56");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 1;
			c.Selection.IsCursorAtStart = true;

			c.ExtendSelectionToEnd();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 5, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 1, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionToPoint1()
		{
			TestCode c = new TestCode("1234\r\n56");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 2;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 2;

			c.ExtendSelectionToPoint(2, 2);

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 2, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 2, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionToPoint2()
		{
			TestCode c = new TestCode("1234\r\n56");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 2;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 2;

			c.ExtendSelectionToPoint(1, 2);

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 2, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 2, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionLeft1()
		{
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 6;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 6;
			c.Selection.IsCursorAtStart = true;

			c.ExtendSelectionLeft();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 5, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 6, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionLeft2()
		{
			//line1
			//line2
			//
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 1;
			c.Selection.IsCursorAtStart = true;

			c.ExtendSelectionLeft();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 1, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionLeft3()
		{
			//line1
			//line2
			//
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 1;
			c.Selection.IsCursorAtStart = true;

			c.ExtendSelectionLeft();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 6, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 1, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionLeft4()
		{
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 1;
			c.Selection.IsCursorAtStart = true;

			c.ExtendSelectionLeft();
			c.ExtendSelectionLeft();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 5, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 1, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionLeft5()
		{
			//[    l]ine1
			TestCode c = new TestCode("\tline1\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 6;
			c.Selection.IsCursorAtStart = false;

			c.ExtendSelectionLeft();
			c.ExtendSelectionLeft();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 1, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionLeft6()
		{
			TestCode c = new TestCode(" \tline1\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 5;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 5;
			c.Selection.IsCursorAtStart = false;

			c.ExtendSelectionLeft();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 2, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 5, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionLeft7()
		{
			TestCode c = new TestCode("  \tline1\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 5;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 6;
			c.Selection.IsCursorAtStart = true;

			c.ExtendSelectionLeft();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 6, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionLeft8()
		{
			TestCode c = new TestCode("   \tline1\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 5;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 7;
			c.Selection.IsCursorAtStart = true;

			c.ExtendSelectionLeft();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 4, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 7, c.Selection.EndCol);
		}
		
		[Test]
		public void testExtendSelectionLeft9()
		{
			TestCode c = new TestCode("  \tline1\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 5;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 7;
			c.Selection.IsCursorAtStart = false;

			c.ExtendSelectionLeft();
			c.ExtendSelectionLeft();
			c.ExtendSelectionLeft();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 5, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionLeft10()
		{
			TestCode c = new TestCode("line1\t\r\nline2\r\n");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 1;
			c.Selection.IsCursorAtStart = false;

			c.ExtendSelectionLeft();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 9, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 1, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionLeft11()
		{
			TestCode c = new TestCode("line1\t\r\nline2\r\n");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 1;
			c.Selection.IsCursorAtStart = false;

			c.ExtendSelectionLeft();
			c.ExtendSelectionLeft();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 6, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 1, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionLeft12()
		{
			TestCode c = new TestCode("line123\r\n\t\r\n");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 5;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 5;

			c.ExtendSelectionLeft();
			c.ExtendSelectionLeft();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 8, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 5, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionLeft13()
		{
			TestCode c = new TestCode("line123\r\n\t\r\n");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 5;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 5;

			c.ExtendSelectionLeft();
			c.ExtendSelectionLeft();
			c.ExtendSelectionLeft();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 7, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 5, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionLeft14()
		{
			//line1
			//line2
			TestCode c = new TestCode("line1\r\nline2");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 3;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 1;

			c.ExtendSelectionLeft();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 6, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionLeft15()
		{
			TestCode c = new TestCode("line1234\r\nline2\r\n");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 1;

			c.ExtendSelectionLeft();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 9, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 1, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionLeft16()
		{
			//1
			//2
			//1234567
			//123
			//1234567890
			TestCode c = new TestCode("1\r\n2\r\n1234567\r\n123\r\n123456789");
			c.Selection.StartLine = 5;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 5;
			c.Selection.EndCol = 1;

			c.ExtendSelectionLeft();
			c.ExtendSelectionLeft();
			c.ExtendSelectionLeft();
			c.ExtendSelectionLeft();
			c.ExtendSelectionLeft();
			c.ExtendSelectionLeft();

			Assertion.AssertEquals("StartLine is wrong!", 3, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 7, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 5, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 1, c.Selection.EndCol);
		}
		
		[Test]
		public void testExtendSelectionSequence1()
		{
			TestCode c = new TestCode("1\r\nline2\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 1;
			c.Selection.IsCursorAtStart = true;

			c.ExtendSelectionRight();
			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 2, c.Selection.EndCol);
			Assertion.AssertEquals("Cursor should not be at back!", false, c.Selection.IsCursorAtStart);

			c.ExtendSelectionLeft();
			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 1, c.Selection.EndCol);
		}

		[Test]
		public void testReduceSelectionRight1()
		{
			TestCode c = new TestCode(" \t ");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 5;
			c.Selection.IsCursorAtStart = false;

			c.ExtendSelectionLeft();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 2, c.Selection.EndCol);
		}

		[Test]
		public void testReduceSelectionRight2()
		{
			TestCode c = new TestCode(" \t \t ");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 5;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 10;
			c.Selection.IsCursorAtStart = false;

			c.ExtendSelectionLeft();
			c.ExtendSelectionLeft();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 5, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 6, c.Selection.EndCol);
		}

		[Test]
		public void testReduceSelectionLeft1()
		{
			//a[   ]
			TestCode c = new TestCode("a\t ");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 2;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 5;
			c.Selection.IsCursorAtStart = true;

			c.ExtendSelectionRight();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 5, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 5, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionUp()
		{
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 4;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 4;
			c.Selection.IsCursorAtStart = true;

			c.ExtendSelectionUp();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 4, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 4, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionUp2()
		{
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 4;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 4;
			c.Selection.IsCursorAtStart = true;

			c.ExtendSelectionUp();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 4, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionUp3()
		{
			//line1
			//l[ine2
			//line]3
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 2;
			c.Selection.EndLine = 3;
			c.Selection.EndCol = 5;
			c.Selection.IsCursorAtStart = false;

			c.ExtendSelectionUp();

			Assertion.AssertEquals("StartLine is wrong!", 2, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 2, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 5, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionUp4()
		{
			//line1
			//l[ine2
			//line]3
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 2;
			c.Selection.EndLine = 3;
			c.Selection.EndCol = 5;
			c.Selection.IsCursorAtStart = false;

			c.ExtendSelectionUp();
			c.ExtendSelectionUp();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 5, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 2, c.Selection.EndCol);
		}

		[Test]
		public void testExtendSelectionUp5()
		{
			//line1
			//line2_long[]er
			TestCode c = new TestCode("line1\r\nline2_longer");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 12;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 12;
			c.Selection.IsCursorAtStart = false;

			c.ExtendSelectionUp();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 6, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 12, c.Selection.EndCol);
		}
		
		[Test]
		public void testExtendSelectionDown1()
		{
			//lin[]e1
			//line2
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 4;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 4;

			c.ExtendSelectionDown();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 4, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 4, c.Selection.EndCol);
		}
		
		[Test]
		public void testExtendSelectionDown2()
		{
			//lin[]e1
			//line2
			TestCode c = new TestCode("line1\r\nline2");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 4;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 4;

			c.ExtendSelectionDown();
			c.ExtendSelectionDown();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 4, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 6, c.Selection.EndCol);
		}
		
		[Test]
		public void testExtendSelectionDown3()
		{
			//line1_long[]er
			//line2
			TestCode c = new TestCode("line1_longer\r\nline2");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 11;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 11;

			c.ExtendSelectionDown();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 11, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 6, c.Selection.EndCol);
		}
		
		[Test]
		public void testExtendSelectionDown4()
		{
			//li[]ne1
			//line2_longer
			TestCode c = new TestCode("line1\r\nline2_longer");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 3;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 3;

			c.ExtendSelectionDown();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 3, c.Selection.EndCol);
		}
		
		[Test]
		public void testExtendSelectionDown5()
		{
			//li[ne1
			//line2]_longer
			TestCode c = new TestCode("line1\r\nline2_longer");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 3;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 6;
			c.Selection.IsCursorAtStart = true;

			c.ExtendSelectionDown();

			Assertion.AssertEquals("StartLine is wrong!", 2, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 6, c.Selection.EndCol);
		}
		
		[Test]
		public void testExtendSelectionDown6()
		{
			//line1
			//li[ne2]_longer
			TestCode c = new TestCode("line1\r\nline2_longer");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 3;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 6;
			c.Selection.IsCursorAtStart = true;

			c.ExtendSelectionDown();

			Assertion.AssertEquals("StartLine is wrong!", 2, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 6, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 13, c.Selection.EndCol);
		}
		
		[Test]
		public void testExtendSelectionDown7()
		{
			//line1
			//li[ne2]_longer
			TestCode c = new TestCode("line1\r\nline2_longer");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 3;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 6;
			c.Selection.IsCursorAtEnd = true;

			c.ExtendSelectionDown();

			Assertion.AssertEquals("StartLine is wrong!", 2, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 13, c.Selection.EndCol);
		}
		
		[Test]
		public void testExtendSelectionDown8()
		{
			//l[]ine1
			//	line2
			TestCode c = new TestCode("line1\r\n\tline2");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 2;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 2;
			c.Selection.IsCursorAtEnd = true;

			c.ExtendSelectionDown();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 2, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 1, c.Selection.EndCol);
		}
		
		[Test]
		public void testExtendSelectionUpAndDown()
		{
			//line1
			//line2
			//line3
			TestCode c = new TestCode("line1\r\nline2\r\nline3");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 3;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 3;

			c.ExtendSelectionUp();
			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 3, c.Selection.EndCol);
			Assertion.Assert("Cursor is on the wrong side!", c.Selection.IsCursorAtStart);
			
			c.ExtendSelectionDown();
			Assertion.AssertEquals("StartLine is wrong!", 2, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 3, c.Selection.EndCol);
			
			c.ExtendSelectionDown();
			Assertion.AssertEquals("StartLine is wrong!", 2, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 3, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 3, c.Selection.EndCol);
			Assertion.Assert("Cursor is on the wrong side!", c.Selection.IsCursorAtEnd); 

			c.ExtendSelectionUp();

			Assertion.AssertEquals("StartLine is wrong!", 2, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 3, c.Selection.EndCol);
		}
		
		[Test]
		public void testUnexpandTabs1()
		{
			//li[]ne1
			TestCode c = new TestCode("line1");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 3;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 3;

			Position pos = c.UnexpandTabs(c.Selection);

			Assertion.AssertEquals("StartLine is wrong!", 1, pos.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, pos.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, pos.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 3, pos.EndCol);
		}
		
		[Test]
		public void testUnexpandTabs2()
		{
			//\tli[]ne1
			TestCode c = new TestCode("\tline1");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 7;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 7;

			Position pos = c.UnexpandTabs(c.Selection);

			Assertion.AssertEquals("StartLine is wrong!", 1, pos.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 4, pos.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, pos.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 4, pos.EndCol);
		}
		
		[Test]
		public void testUnexpandTabs3()
		{
			//\t\tline[]1
			TestCode c = new TestCode("\t\tline1");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 13;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 13;

			Position pos = c.UnexpandTabs(c.Selection);

			Assertion.AssertEquals("StartLine is wrong!", 1, pos.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 7, pos.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, pos.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 7, pos.EndCol);
		}

		[Test]
		public void testUnexpandTabs4()
		{
			//\t\tli[ne1
			//\t\t\tli]ne2
			TestCode c = new TestCode("\t\tline1\r\n\t\t\tline2");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 11;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 15;

			Position pos = c.UnexpandTabs(c.Selection);

			Assertion.AssertEquals("StartLine is wrong!", 1, pos.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 5, pos.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, pos.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 6, pos.EndCol);
		}

		[Test]
		public void testUnexpandTabs5()
		{
			//\t\tline1
			//\t\t\tl[in]e2
			TestCode c = new TestCode("\t\tline1\r\n\t\t\tline2");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 14;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 16;

			Position pos = c.UnexpandTabs(c.Selection);

			Assertion.AssertEquals("StartLine is wrong!", 2, pos.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 5, pos.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, pos.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 7, pos.EndCol);
		}
		
		[Test]
		public void testUnexpandTabs6()
		{
			//\t\tline1
			//\t\t \t[li]ne2
			TestCode c = new TestCode("\t\tline1\r\n\t\t \tline2");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 13;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 15;

			Position pos = c.UnexpandTabs(c.Selection);

			Assertion.AssertEquals("StartLine is wrong!", 2, pos.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 5, pos.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, pos.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 7, pos.EndCol);
		}

		[Test]
		public void testAdjustPositionForExpandedTabs1()
		{
			//li[]ne1
			TestCode c = new TestCode("line1");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 3;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 3;

			Position pos = c.AdjustPositionForExpandedTabs(c.Selection);

			Assertion.AssertEquals("StartLine is wrong!", 1, pos.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, pos.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, pos.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 3, pos.EndCol);
		}

		[Test]
		public void testAdjustPositionForExpandedTabs2()
		{
			//\tli[]ne1
			TestCode c = new TestCode("\tline1");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 4;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 4;

			Position pos = c.AdjustPositionForExpandedTabs(c.Selection);

			Assertion.AssertEquals("StartLine is wrong!", 1, pos.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 7, pos.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, pos.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 7, pos.EndCol);
		}

		[Test]
		public void testAdjustPositionForExpandedTabs3()
		{
			//\t\tline[]1
			TestCode c = new TestCode("\t\tline1");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 7;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 7;

			Position pos = c.AdjustPositionForExpandedTabs(c.Selection);

			Assertion.AssertEquals("StartLine is wrong!", 1, pos.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 13, pos.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, pos.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 13, pos.EndCol);
		}

		[Test]
		public void testAdjustPositionForExpandedTabs4()
		{
			//\t\tli[ne1
			//\t\t\tli]ne2
			TestCode c = new TestCode("\t\tline1\r\n\t\t\tline2");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 5;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 6;

			Position pos = c.AdjustPositionForExpandedTabs(c.Selection);

			Assertion.AssertEquals("StartLine is wrong!", 1, pos.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 11, pos.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, pos.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 15, pos.EndCol);
		}

		[Test]
		public void testDeleteSelectedText1()
		{
			//li[n]e1
			TestCode c = new TestCode("line1\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 3;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 4;

			c.DeleteSelectedText();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 3, c.Selection.EndCol);
			string expected = "lie1\r\n";
			Assertion.AssertEquals("Text after delete selected text is wrong", expected, c.Text);
		}

		[Test]
		public void testDeleteSelectedText2()
		{
			//line1
			//l[i]ne2
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 2;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 3;

			c.DeleteSelectedText();

			Assertion.AssertEquals("StartLine is wrong!", 2, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 2, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 2, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 2, c.Selection.EndCol);
			string expected = "line1\r\nlne2\r\n";
			Assertion.AssertEquals("Text after delete selected text is wrong", expected, c.Text);
		}

		[Test]
		public void testDeleteSelectedText3()
		{
			//li[ne1
			//li]ne2
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 3;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 3;

			c.DeleteSelectedText();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 3, c.Selection.EndCol);
			string expected = "line2\r\n";
			Assertion.AssertEquals("Text after delete selected text is wrong", expected, c.Text);
		}

		[Test]
		public void testDeleteSelectedText4()
		{
			//li[ne1
			//line2
			//li]ne3
			TestCode c = new TestCode("line1\r\nline2\r\nline3\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 3;
			c.Selection.EndLine = 3;
			c.Selection.EndCol = 3;

			c.DeleteSelectedText();

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 3, c.Selection.EndCol);
			string expected = "line3\r\n";
			Assertion.AssertEquals("Text after delete selected text is wrong", expected, c.Text);
		}

		[Test]
		public void testDeleteSelectedText5()
		{
			//line1
			//line2
			//[line3
			//line4
			//li]ne5
			TestCode c = new TestCode("line1\r\nline2\r\nline3\r\nline4\r\nline5\r\n");
			c.Selection.StartLine = 3;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 5;
			c.Selection.EndCol = 3;

			c.DeleteSelectedText();

			Assertion.AssertEquals("StartLine is wrong!", 3, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 3, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 1, c.Selection.EndCol);
			string expected = "line1\r\nline2\r\nne5\r\n";
			Assertion.AssertEquals("Text after delete selected text is wrong", expected, c.Text);
		}

		[Test]
		public void testSelectedText1()
		{
			//[line1]
			TestCode c = new TestCode("line1\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 6;

			string expected = "line1";
			Assertion.AssertEquals("SelectedText is wrong", expected, c.SelectedText);
		}

		[Test]
		public void testSelectedText2()
		{
			//[line1
			//line2]
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 6;

			string expected = "line1\r\nline2";
			Assertion.AssertEquals("SelectedText is wrong", expected, c.SelectedText);
		}

		[Test]
		public void testSelectedText3()
		{
			//[line1
			//line2
			//line3]
			TestCode c = new TestCode("line1\r\nline2\r\nline3\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 1;
			c.Selection.EndLine = 3;
			c.Selection.EndCol = 6;

			string expected = "line1\r\nline2\r\nline3";
			Assertion.AssertEquals("SelectedText is wrong", expected, c.SelectedText);
		}

		[Test]
		public void testSelectedText4()
		{
			//line1
			//li[ne2
			//line3
			//line]4
			//line5
			TestCode c = new TestCode("line1\r\nline2\r\nline3\r\nline4\r\nline5\r\n");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 3;
			c.Selection.EndLine = 4;
			c.Selection.EndCol = 5;

			string expected = "ne2\r\nline3\r\nline";
			Assertion.AssertEquals("SelectedText is wrong", expected, c.SelectedText);
		}

		[Test]
		public void testSelectedText5()
		{
			//l[i]ne1
			TestCode c = new TestCode("line1\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 2;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 3;

			string expected = "i";
			Assertion.AssertEquals("SelectedText is wrong", expected, c.SelectedText);
		}

		[Test]
		public void testSelectedText6()
		{
			// \tline1
			TestCode c = new TestCode("  \tline1\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 8;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 9;

			string expected = "e";
			Assertion.AssertEquals("SelectedText is wrong", expected, c.SelectedText);
		}

		[Test]
		public void testSelectedText7()
		{
			//   \tline1
			TestCode c = new TestCode("   \tline1\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 8;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 9;

			string expected = "e";
			Assertion.AssertEquals("SelectedText is wrong", expected, c.SelectedText);
		}

		[Test]
		public void testSelectedText8()
		{
			//    \tline1
			TestCode c = new TestCode("    \tline1\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 12;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 13;

			string expected = "e";
			Assertion.AssertEquals("SelectedText is wrong", expected, c.SelectedText);
		}

		[Test]
		public void testInsertTextInSelection1()
		{
			//line1
			TestCode c = new TestCode("line1\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 3;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 3;

			c.InsertTextInSelection("a");

			Assertion.AssertEquals("StartLine is wrong!", 1, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 4, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 4, c.Selection.EndCol);

			string expected = "liane1\r\n";
			Assertion.AssertEquals("SelectedText is wrong", expected, c.Text);
		}

		[Test]
		public void testInsertTextInSelection2()
		{
			//lin[]e1
			//line2
			TestCode c = new TestCode("line1\r\nline2\r\n");
			c.Selection.StartLine = 1;
			c.Selection.StartCol = 4;
			c.Selection.EndLine = 1;
			c.Selection.EndCol = 4;

			c.InsertTextInSelection("a\r\nb\r\nc");

			Assertion.AssertEquals("StartLine is wrong!", 3, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 2, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 3, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 2, c.Selection.EndCol);

			string expected = "lina\r\nb\r\nce1\r\nline2\r\n";
			Assertion.AssertEquals("SelectedText is wrong", expected, c.Text);
		}

		[Test]
		public void testInsertTextInSelection3()
		{
			//\tline1
			//\t\tli[]ne2
			//\t\t\tline3
			TestCode c = new TestCode("\tline1\r\n\t\tline2\r\n\t\t\tline3\r\n");
			c.Selection.StartLine = 2;
			c.Selection.StartCol = 11;
			c.Selection.EndLine = 2;
			c.Selection.EndCol = 11;

			c.InsertTextInSelection("\ta\r\n\tb\r\n\tc");

			Assertion.AssertEquals("StartLine is wrong!", 4, c.Selection.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 6, c.Selection.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 4, c.Selection.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 6, c.Selection.EndCol);

			string expected = "\tline1\r\n\t\tli\ta\r\n\tb\r\n\tcne2\r\n\t\t\tline3\r\n";
			Assertion.AssertEquals("SelectedText is wrong", expected, c.Text);
		}
	}
}