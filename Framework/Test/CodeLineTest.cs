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
	public class TestCode : Code
	{
		public TestCode(string text) :
			base(text)
		{
		}

		protected override CodeLine CreateLine(string text)
		{
			char[] CHARS_TO_TRIM = new char[2] {'\r', '\n'};
			return new TestCodeLine(text.Trim(CHARS_TO_TRIM));
		}
	}
	
	public class TestCodeLine : CodeLine
	{
		public TestCodeLine(string text) :
		base(text)
		{
		}

		public override ColoredStringPartCollection PartsUpTo(int columnIndex)
		{
			ColoredStringPartCollection ret = new ColoredStringPartCollection();
			ret.Add(new ColoredStringPart(0, m_Text));
			return ret;
		}
	}

	public class TestCodeLineWithParts : CodeLine
	{
		public TestCodeLineWithParts(string text) :
		base(text)
		{
		}

		public override ColoredStringPartCollection PartsUpTo(int columnIndex)
		{
			ColoredStringPartCollection ret = new ColoredStringPartCollection();
			ret.Add(new ColoredStringPart(0, m_Text.Substring(0, 6)));
			ret.Add(new ColoredStringPart(6, m_Text.Substring(6, m_Text.Length-6)));
			return ret;
		}
	}

	[TestFixture]
	public class CodeLineTest
	{
		[Test]
		public void testParts1()
		{
			TestCode c = new TestCode("line1");
			Assertion.AssertEquals("Wrong number of parts!", 1, c.Lines.Count);
		}
		
		[Test]
		public void testParts2()
		{
			TestCodeLineWithParts c = new TestCodeLineWithParts("public\t");
			Assertion.AssertEquals(2, c.Parts.Count);
			Assertion.AssertEquals("public", c.Parts[0].Text);
			Assertion.AssertEquals(6, c.Parts[0].ExtendedLength);
			Assertion.AssertEquals("\t", c.Parts[1].Text);
			Assertion.AssertEquals(2, c.Parts[1].ExtendedLength);
		}
		
		[Test]
		public void testExtendedLength()
		{
			TestCodeLine l = new TestCodeLine("line1\t");
			Assertion.AssertEquals("Extended Length is wrong!", 8, l.ExtendedLength);
		}
		
		[Test]
		public void testInsert1()
		{
			TestCodeLine l = new TestCodeLine("line1");
			Assertion.AssertEquals("Extended Length is wrong!", 5, l.ExtendedLength);
			l.Insert(2, "x");
			Assertion.AssertEquals("Extended Length is wrong!", 6, l.ExtendedLength);
			Assertion.AssertEquals("Line after insertion is wrong!", "lixne1", l.Text);
		}
		
		[Test]
		public void testInsert2()
		{
			TestCodeLine l = new TestCodeLine("line1");
			Assertion.AssertEquals("Extended Length is wrong!", 5, l.ExtendedLength);
			l.Insert(0, "x");
			Assertion.AssertEquals("Extended Length is wrong!", 6, l.ExtendedLength);
			Assertion.AssertEquals("Line after insertion is wrong!", "xline1", l.Text);
		}
		
		[Test]
		public void testInsert3()
		{
			TestCodeLine l = new TestCodeLine("line1");
			Assertion.AssertEquals("Extended Length is wrong!", 5, l.ExtendedLength);
			l.Insert(5, "aaa");
			Assertion.AssertEquals("Extended Length is wrong!", 8, l.ExtendedLength);
			Assertion.AssertEquals("Line after insertion is wrong!", "line1aaa", l.Text);
		}
		
		[Test]
		public void testInsert4()
		{
			TestCodeLine l = new TestCodeLine("\tline1");
			Assertion.AssertEquals("Extended Length is wrong!", 9, l.ExtendedLength);
			l.Insert(5, "aaa");
			Assertion.AssertEquals("Extended Length is wrong!", 12, l.ExtendedLength);
			Assertion.AssertEquals("Line after insertion is wrong!", "\tlaaaine1", l.Text);
		}

		[Test]
		public void testInsert5()
		{
			TestCodeLine l = new TestCodeLine("\t\tline1");
			Assertion.AssertEquals("Extended Length is wrong!", 13, l.ExtendedLength);
			l.Insert(9, "aaa");
			Assertion.AssertEquals("Extended Length is wrong!", 16, l.ExtendedLength);
			Assertion.AssertEquals("Line after insertion is wrong!", "\t\tlaaaine1", l.Text);
		}

		[Test]
		public void testInsert6()
		{
			TestCodeLine l = new TestCodeLine("\t \tline1");
			Assertion.AssertEquals("Extended Length is wrong!", 13, l.ExtendedLength);
			l.Insert(9, "aaa");
			Assertion.AssertEquals("Extended Length is wrong!", 16, l.ExtendedLength);
			Assertion.AssertEquals("Line after insertion is wrong!", "\t \tlaaaine1", l.Text);
		}

		[Test]
		public void testInsert7()
		{
			TestCodeLine l = new TestCodeLine("\t   \tline1");
			Assertion.AssertEquals("Extended Length is wrong!", 13, l.ExtendedLength);
			l.Insert(9, "aaa");
			Assertion.AssertEquals("Extended Length is wrong!", 16, l.ExtendedLength);
			Assertion.AssertEquals("Line after insertion is wrong!", "\t   \tlaaaine1", l.Text);
		}

		[Test]
		public void testInsert8()
		{
			TestCodeLine l = new TestCodeLine("   \t\tline1");
			Assertion.AssertEquals("Extended Length is wrong!", 13, l.ExtendedLength);
			l.Insert(9, "aaa");
			Assertion.AssertEquals("Extended Length is wrong!", 16, l.ExtendedLength);
			Assertion.AssertEquals("Line after insertion is wrong!", "   \t\tlaaaine1", l.Text);
		}

		[Test]
		public void testInsert9()
		{
			TestCodeLine l = new TestCodeLine("\t\tline1");
			Assertion.AssertEquals("Extended Length is wrong!", 13, l.ExtendedLength);
			l.Insert(4, "aaa");
			Assertion.AssertEquals("Extended Length is wrong!", 13, l.ExtendedLength);
			Assertion.AssertEquals("Line after insertion is wrong!", "\taaa\tline1", l.Text);
		}

		[Test]
		public void testRemove1()
		{
			TestCodeLine l = new TestCodeLine("line1");
			Assertion.AssertEquals("Extended Length is wrong!", 5, l.ExtendedLength);
			l.Remove(1, 2);
			Assertion.AssertEquals("Extended Length is wrong!", 4, l.ExtendedLength);
			Assertion.AssertEquals("Line after removal is wrong!", "lne1", l.Text);
		}
		
		[Test]
		public void testRemove2()
		{
			TestCodeLine l = new TestCodeLine("line1");
			Assertion.AssertEquals("Extended Length is wrong!", 5, l.ExtendedLength);
			l.Remove(0, 1);
			Assertion.AssertEquals("Extended Length is wrong!", 4, l.ExtendedLength);
			Assertion.AssertEquals("Line after removal is wrong!", "ine1", l.Text);
		}

		[Test]
		public void testRemove3()
		{
			TestCodeLine l = new TestCodeLine("line1");
			Assertion.AssertEquals("Extended Length is wrong!", 5, l.ExtendedLength);
			l.Remove(4, 5);
			Assertion.AssertEquals("Extended Length is wrong!", 4, l.ExtendedLength);
			Assertion.AssertEquals("Line after removal is wrong!", "line", l.Text);
		}

		[Test]
		public void testRemove4()
		{
			TestCodeLine l = new TestCodeLine("\tline1");
			Assertion.AssertEquals("Extended Length is wrong!", 9, l.ExtendedLength);
			l.Remove(5, 6);
			Assertion.AssertEquals("Extended Length is wrong!", 8, l.ExtendedLength);
			Assertion.AssertEquals("Line after removal is wrong!", "\tlne1", l.Text);
		}

		[Test]
		public void testRemove5()
		{
			TestCodeLine l = new TestCodeLine("\tline1");
			Assertion.AssertEquals("Extended Length is wrong!", 9, l.ExtendedLength);
			l.Remove(7, 9);
			Assertion.AssertEquals("Extended Length is wrong!", 7, l.ExtendedLength);
			Assertion.AssertEquals("Line after removal is wrong!", "\tlin", l.Text);
		}

		[Test]
		public void testRemove6()
		{
			TestCodeLine l = new TestCodeLine(" \tline1");
			Assertion.AssertEquals("Extended Length is wrong!", 9, l.ExtendedLength);
			l.Remove(7, 9);
			Assertion.AssertEquals("Extended Length is wrong!", 7, l.ExtendedLength);
			Assertion.AssertEquals("Line after removal is wrong!", " \tlin", l.Text);
		}

		[Test]
		public void testRemove7()
		{
			TestCodeLine l = new TestCodeLine("\t  \tline1");
			Assertion.AssertEquals("Extended Length is wrong!", 13, l.ExtendedLength);
			l.Remove(11, 13);
			Assertion.AssertEquals("Extended Length is wrong!", 11, l.ExtendedLength);
			Assertion.AssertEquals("Line after removal is wrong!", "\t  \tlin", l.Text);
		}

		[Test]
		public void testCollectionInsertAfter()
		{
			CodeLineCollection collection = new CodeLineCollection();
			collection.Add(new TestCodeLine("1"));
			collection.Add(new TestCodeLine("2"));
			collection.Add(new TestCodeLine("3"));
			collection.Add(new TestCodeLine("4"));
			collection.InsertAfter(2, new TestCodeLine("3.5"));
			Assertion.AssertEquals("Collection count is wrong!", 5, collection.Count);
			Assertion.AssertEquals("Item 1 is wrong!", "1", collection[0].Text);
			Assertion.AssertEquals("Item 2 is wrong!", "2", collection[1].Text);
			Assertion.AssertEquals("Item 3 is wrong!", "3", collection[2].Text);
			Assertion.AssertEquals("Item 4 is wrong!", "3.5", collection[3].Text);
			Assertion.AssertEquals("Item 5 is wrong!", "4", collection[4].Text);
		}
		
		[Test]
		public void testLeftPositionSnap1()
		{
			TestCodeLine l = new TestCodeLine("line1");
			Assertion.AssertEquals("Snap position is wrong!", 1, l.LeftPositionSnap(1));
		}

		[Test]
		public void testLeftPositionSnap2()
		{
			TestCodeLine l = new TestCodeLine("line1");
			Assertion.AssertEquals("Snap position is wrong!", 2, l.LeftPositionSnap(2));
		}
		
		[Test]
		public void testLeftPositionSnap3()
		{
			TestCodeLine l = new TestCodeLine("\tline1");
			Assertion.AssertEquals("Snap position is wrong!", 1, l.LeftPositionSnap(2));
		}
		
		[Test]
		public void testLeftPositionSnap4()
		{
			TestCodeLine l = new TestCodeLine(" \tline1");
			Assertion.AssertEquals("Snap position is wrong!", 2, l.LeftPositionSnap(4));
		}
		
		[Test]
		public void testLeftPositionSnap5()
		{
			TestCodeLine l = new TestCodeLine("  \tline1");
			Assertion.AssertEquals("Snap position is wrong!", 3, l.LeftPositionSnap(4));
		}
		
		[Test]
		public void testLeftPositionSnap6()
		{
			TestCodeLine l = new TestCodeLine("  \tline1");
			Assertion.AssertEquals("Snap position is wrong!", 5, l.LeftPositionSnap(5));
		}
		
		[Test]
		public void testLeftPositionSnap7()
		{
			TestCodeLine l = new TestCodeLine("a\tb\tc");
			Assertion.AssertEquals("Snap position is wrong!", 6, l.LeftPositionSnap(8));
		}

		[Test]
		public void testClosestPositionSnap1()
		{
			TestCodeLine l = new TestCodeLine("line1");
			Assertion.AssertEquals("Snap position is wrong!", 1, l.ClosestPositionSnap(1));
		}

		[Test]
		public void testClosestPositionSnap2()
		{
			TestCodeLine l = new TestCodeLine("line1");
			Assertion.AssertEquals("Snap position is wrong!", 2, l.ClosestPositionSnap(2));
		}
		
		[Test]
		public void testClosestPositionSnap3()
		{
			TestCodeLine l = new TestCodeLine("\tline1");
			Assertion.AssertEquals("Snap position is wrong!", 1, l.ClosestPositionSnap(2));
		}
		
		[Test]
		public void testClosestPositionSnap4()
		{
			TestCodeLine l = new TestCodeLine(" \tline1");
			Assertion.AssertEquals("Snap position is wrong!", 5, l.ClosestPositionSnap(4));
		}
		
		[Test]
		public void testClosestPositionSnap5()
		{
			TestCodeLine l = new TestCodeLine("  \tline1");
			Assertion.AssertEquals("Snap position is wrong!", 5, l.ClosestPositionSnap(4));
		}
		
		[Test]
		public void testClosestPositionSnap6()
		{
			TestCodeLine l = new TestCodeLine("  \tline1");
			Assertion.AssertEquals("Snap position is wrong!", 5, l.ClosestPositionSnap(5));
		}
		
		[Test]
		public void testClosestPositionSnap7()
		{
			TestCodeLine l = new TestCodeLine("a\tb\tc");
			Assertion.AssertEquals("Snap position is wrong!", 9, l.ClosestPositionSnap(8));
		}
		
		[Test]
		public void testClosestPositionSnap8()
		{
			TestCodeLine l = new TestCodeLine("\t");
			Assertion.AssertEquals("Snap position is wrong!", 5, l.ClosestPositionSnap(4));
		}
		
		[Test]
		public void testClosestPositionSnap9()
		{
			TestCodeLine l = new TestCodeLine("\t\t\tNewMethodName();");
			Assertion.AssertEquals("Snap position is wrong!", 9, l.ClosestPositionSnap(10));
		}
	}
}
