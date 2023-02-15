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
using NUnit.Framework;
using Opnieuw.Framework;

namespace Opnieuw.Framework.Test
{
	[TestFixture]
	public class ColoredStringPartTest
	{
		[Test]
		public void testSplit1()
		{
			ColoredStringPart part = new ColoredStringPart(0, "line1", Color.Black);
			ColoredStringPartCollection parts = part.Split(0, 3, 3);
			Assertion.AssertEquals(2, parts.Count);
			Assertion.AssertEquals("li", parts[0].Text);
			Assertion.AssertEquals("ne1", parts[1].Text);
		}

		[Test]
		public void testSplit2()
		{
			ColoredStringPart part = new ColoredStringPart(0, "line1", Color.Black);
			ColoredStringPartCollection parts = part.Split(0, 1, 1);
			Assertion.AssertEquals(1, parts.Count);
			Assertion.AssertEquals("line1", parts[0].Text);
		}

		[Test]
		public void testSplit3()
		{
			ColoredStringPart part = new ColoredStringPart(0, "line1", Color.Black);
			ColoredStringPartCollection parts = part.Split(0, 6, 6);
			Assertion.AssertEquals(1, parts.Count);
			Assertion.AssertEquals("line1", parts[0].Text);
		}

		[Test]
		public void testSplit4()
		{
			ColoredStringPart part = new ColoredStringPart(0, "line1", Color.Black);
			ColoredStringPartCollection parts = part.Split(0, 4, 5);
			Assertion.AssertEquals(3, parts.Count);
			Assertion.AssertEquals("lin", parts[0].Text);
			Assertion.AssertEquals("e", parts[1].Text);
			Assertion.AssertEquals("1", parts[2].Text);
		}

		[Test]
		public void testSplit5()
		{
			ColoredStringPart part = new ColoredStringPart(0, "  line1", Color.Black);
			ColoredStringPartCollection parts = part.Split(0, 1, 5);
			Assertion.AssertEquals(2, parts.Count);
			Assertion.AssertEquals("  li", parts[0].Text);
			Assertion.AssertEquals("ne1", parts[1].Text);
		}

		[Test]
		public void testSplit6()
		{
			ColoredStringPart part = new ColoredStringPart(0, "  line1    ", Color.Black);
			ColoredStringPartCollection parts = part.Split(0, 2, 10);
			Assertion.AssertEquals(3, parts.Count);
			Assertion.AssertEquals(" ", parts[0].Text);
			Assertion.AssertEquals(" line1  ", parts[1].Text);
			Assertion.AssertEquals("  ", parts[2].Text);
		}

		[Test]
		public void testSplit7()
		{
			ColoredStringPart part = new ColoredStringPart(0, "line1", Color.Black);
			ColoredStringPartCollection parts = part.Split(0, 2, 10);
			Assertion.AssertEquals(2, parts.Count);
			Assertion.AssertEquals("l", parts[0].Text);
			Assertion.AssertEquals("ine1", parts[1].Text);
		}

		[Test]
		public void testSplit8()
		{
			ColoredStringPart part = new ColoredStringPart(0, "line1", Color.Black);
			ColoredStringPartCollection parts = part.Split(0, 2, 10);
			Assertion.AssertEquals(2, parts.Count);
			Assertion.AssertEquals("l", parts[0].Text);
			Assertion.AssertEquals("ine1", parts[1].Text);
		}

		[Test]
		public void testSplitForSelection1()
		{
			//     l[i]ne1
			ColoredStringPart part = new ColoredStringPart(0, "line1", Color.Black);
			Position selection = new Position(1,7,1,8);
			ColoredStringPartCollection parts = part.SplitForSelection(selection, 1, 5, Color.Black, Color.Black);
			Assertion.AssertEquals(3, parts.Count);
			Assertion.AssertEquals("l", parts[0].Text);
			Assertion.AssertEquals("i", parts[1].Text);
			Assertion.AssertEquals("ne1", parts[2].Text);
			
		}

		[Test]
		public void testSplitForSelection2()
		{
			//     [  ]line1
			ColoredStringPart part = new ColoredStringPart(0, "line1", Color.Black);
			Position selection = new Position(1,6,1,8);
			ColoredStringPartCollection parts = part.SplitForSelection(selection, 1, 7, Color.Black, Color.Black);
			Assertion.AssertEquals(1, parts.Count);
			Assertion.AssertEquals("line1", parts[0].Text);
		}

		[Test]
		public void testSplitForSelection3()
		{
			//     [  li]ne1
			ColoredStringPart part = new ColoredStringPart(0, "line1", Color.Black);
			Position selection = new Position(1,6,1,10);
			ColoredStringPartCollection parts = part.SplitForSelection(selection, 1, 7, Color.Black, Color.Black);
			Assertion.AssertEquals(2, parts.Count);
			Assertion.AssertEquals("li", parts[0].Text);
			Assertion.AssertEquals("ne1", parts[1].Text);
		}

		[Test]
		public void testSubstring1()
		{
			//l[in]e1
			ColoredStringPart part = new ColoredStringPart(0, "line1", Color.Black);
			string sub = part.Substring(1, 2);
			Assertion.AssertEquals("in", sub);
		}

		[Test]
		public void testSubstring2()
		{
			//[lin]e1
			ColoredStringPart part = new ColoredStringPart(0, "line1", Color.Black);
			string sub = part.Substring(0, 3);
			Assertion.AssertEquals("lin", sub);
		}

		[Test]
		public void testSubstring3()
		{
			//lin[e1]
			ColoredStringPart part = new ColoredStringPart(0, "line1", Color.Black);
			string sub = part.Substring(3, 2);
			Assertion.AssertEquals("e1", sub);
		}
	}
}