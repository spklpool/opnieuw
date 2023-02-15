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
using Opnieuw.Framework;
using Opnieuw.Parsers.CSParser;

namespace Opnieuw.Parsers.CSParser.Test
{
	[TestFixture]
	public class CSCodeLineTest
	{
		[Test]
		public void testParts1()
		{
			CSCodeLine line = new CSCodeLine("void void void");
			Assertion.AssertEquals("Wrong number of parts!", 5, line.Parts.Count);
		}
		
		[Test]
		public void testParts2()
		{
			CSCodeLine line = new CSCodeLine("line1");
			Assertion.AssertEquals("Wrong number of parts!", 1, line.Parts.Count);
		}

		[Test]
		public void testParts3()
		{
			CSCodeLine line = new CSCodeLine("void 1 public 2");
			Assertion.AssertEquals("Wrong number of parts!", 4, line.Parts.Count);
		}

		[Test]
		public void testParts4()
		{
			CSCodeLine line = new CSCodeLine("//some comment");
			Assertion.AssertEquals("Wrong number of parts!", 1, line.Parts.Count);
		}

		[Test]
		public void testPartsUpTo1()
		{
			CSCodeLine line = new CSCodeLine("void void void");
			ColoredStringPartCollection parts = line.PartsUpTo(7);
			Assertion.AssertEquals("Wrong number of parts!", 3, parts.Count);
			Assertion.AssertEquals("Part 1 is wrong!", "void", parts[0].StringPart);
			Assertion.AssertEquals("Part 2 is wrong!", " ", parts[1].StringPart);
			Assertion.AssertEquals("Part 3 is wrong!", "vo", parts[2].StringPart);
		}

		[Test]
		public void testPartsUpTo2()
		{
			CSCodeLine line = new CSCodeLine("void something void somethingElse");
			ColoredStringPartCollection parts = line.PartsUpTo(17);
			Assertion.AssertEquals("Wrong number of parts!", 3, parts.Count);
			Assertion.AssertEquals("Part 1 is wrong!", "void", parts[0].StringPart);
			Assertion.AssertEquals("Part 2 is wrong!", " something ", parts[1].StringPart);
			Assertion.AssertEquals("Part 3 is wrong!", "vo", parts[2].StringPart);
		}

		[Test]
		public void testPartsUpTo3()
		{
			CSCodeLine line = new CSCodeLine("void something void somethingElse");
			ColoredStringPartCollection parts = line.PartsUpTo(5);
			Assertion.AssertEquals("Wrong number of parts!", 2, parts.Count);
			Assertion.AssertEquals("Part 1 is wrong!", "void", parts[0].StringPart);
			Assertion.AssertEquals("Part 2 is wrong!", " ", parts[1].StringPart);
		}

		[Test]
		public void testPartsUpTo4()
		{
			CSCodeLine line = new CSCodeLine("void something void somethingElse");
			ColoredStringPartCollection parts = line.PartsUpTo(21);
			Assertion.AssertEquals("Wrong number of parts!", 4, parts.Count);
			Assertion.AssertEquals("Part 1 is wrong!", "void", parts[0].StringPart);
			Assertion.AssertEquals("Part 2 is wrong!", " something ", parts[1].StringPart);
			Assertion.AssertEquals("Part 3 is wrong!", "void", parts[2].StringPart);
			Assertion.AssertEquals("Part 4 is wrong!", " s", parts[3].StringPart);
		}

		[Test]
		public void testPartsUpTo5()
		{
			CSCodeLine line = new CSCodeLine("void /*Some comment*/ void somethingElse");
			ColoredStringPartCollection parts = line.PartsUpTo(23);
			Assertion.AssertEquals("Wrong number of parts!", 5, parts.Count);
			Assertion.AssertEquals("Part 1 is wrong!", "void", parts[0].StringPart);
			Assertion.AssertEquals("Part 2 is wrong!", " ", parts[1].StringPart);
			Assertion.AssertEquals("Part 3 is wrong!", "/*Some comment*/", parts[2].StringPart);
			Assertion.AssertEquals("Part 4 is wrong!", " ", parts[3].StringPart);
			Assertion.AssertEquals("Part 5 is wrong!", "v", parts[4].StringPart);
		}

		[Test]
		public void testPartsUpTo6()
		{
			CSCodeLine line = new CSCodeLine("void /*Some comment*/ void somethingElse");
			ColoredStringPartCollection parts = line.PartsUpTo(13);
			Assertion.AssertEquals("Wrong number of parts!", 3, parts.Count);
			Assertion.AssertEquals("Part 1 is wrong!", "void", parts[0].StringPart);
			Assertion.AssertEquals("Part 2 is wrong!", " ", parts[1].StringPart);
			Assertion.AssertEquals("Part 3 is wrong!", "/*Some c", parts[2].StringPart);
		}
	}
}
