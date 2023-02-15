#region Copyright (C) Pierre Boudreau
//This file is part of the Opnieuw C# parser.
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
using System.IO;
using Opnieuw.Framework;
using Opnieuw.Parsers.CSParser;

namespace Opnieuw.Parsers.CSParser.Test
{
	[TestFixture]
	public class AttributeTest : TokenizerTestBase
	{
		[Test]
		public void testSimple()
		{
			string fileString = "[ AttributeTest ]";
			
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();
			AttributeSectionCollection attributeSections = AttributeSectionCollection.parse(t);
			Assertion.AssertEquals("Generate is wrong!", fileString, attributeSections.Generate());

			Assertion.AssertEquals("Attribute section count is wrong!", 1, attributeSections.Count);
			Assertion.AssertEquals("StartLine is wrong!", 1, attributeSections.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, attributeSections.Position.StartCol);
			Assertion.AssertEquals("1EndLine is wrong!", 1, attributeSections.Position.EndLine);
			Assertion.AssertEquals("1EndCol is wrong!", 17, attributeSections.Position.EndCol);

			Assertion.AssertEquals("Attribute count is wong!", 1, attributeSections[0].Attributes.Count);
			Assertion.AssertEquals("StartLine is wrong!", 1, attributeSections[0].Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, attributeSections[0].Position.StartCol);
			Assertion.AssertEquals("2EndLine is wrong!", 1, attributeSections[0].Position.EndLine);
			Assertion.AssertEquals("2EndCol is wrong!", 17, attributeSections[0].Position.EndCol);

			Assertion.AssertEquals("Attribute name is wrong!", "AttributeTest", attributeSections[0].Attributes[0].Name);
			Assertion.AssertEquals("StartLine is wrong!", 1, attributeSections[0].Attributes[0].Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, attributeSections[0].Attributes[0].Position.StartCol);
			Assertion.AssertEquals("3EndLine is wrong!", 1, attributeSections[0].Attributes[0].Position.EndLine);
			Assertion.AssertEquals("3EndCol is wrong!", 15, attributeSections[0].Attributes[0].Position.EndCol);
		}

		[Test]
		public void testWithArguments()
		{
			string fileString = "[SomeAttribute(1, Version = 1)]";
			
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();
			AttributeSectionCollection attributeSections = AttributeSectionCollection.parse(t);
			Assertion.AssertEquals("Generate is wrong!", fileString, attributeSections.Generate());

			Assertion.AssertEquals("Attribute section count is wrong!", 1, attributeSections.Count);
			Assertion.AssertEquals("StartLine is wrong!", 1, attributeSections.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, attributeSections.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, attributeSections.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 31, attributeSections.Position.EndCol);

			Assertion.AssertEquals("Attribute count is wong!", 1, attributeSections[0].Attributes.Count);
			Assertion.AssertEquals("StartLine is wrong!", 1, attributeSections[0].Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, attributeSections[0].Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, attributeSections[0].Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 31, attributeSections[0].Position.EndCol);

			Assertion.AssertEquals("Attribute name is wrong!", "SomeAttribute", attributeSections[0].Attributes[0].Name);
			Assertion.AssertEquals("StartLine is wrong!", 1, attributeSections[0].Attributes[0].Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 2, attributeSections[0].Attributes[0].Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, attributeSections[0].Attributes[0].Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 30, attributeSections[0].Attributes[0].Position.EndCol);
		}

		[Test]
		public void testTwoSectionsWithArguments()
		{
			string fileString = "[First(1, 2)][SomeAttribute(1, Version = 1)]";
			
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();
			AttributeSectionCollection attributeSections = AttributeSectionCollection.parse(t);
			Assertion.AssertEquals("Generate is wrong!", fileString, attributeSections.Generate());

			Assertion.AssertEquals("Attribute section count is wrong!", 2, attributeSections.Count);
			Assertion.AssertEquals("StartLine is wrong!", 1, attributeSections.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, attributeSections.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, attributeSections.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 44, attributeSections.Position.EndCol);

			Assertion.AssertEquals("Attribute count is wong!", 1, attributeSections[0].Attributes.Count);
			Assertion.AssertEquals("StartLine is wrong!", 1, attributeSections[0].Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, attributeSections[0].Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, attributeSections[0].Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 13, attributeSections[0].Position.EndCol);

			Assertion.AssertEquals("Attribute count is wong!", 1, attributeSections[1].Attributes.Count);
			Assertion.AssertEquals("StartLine is wrong!", 1, attributeSections[1].Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 14, attributeSections[1].Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, attributeSections[1].Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 44, attributeSections[1].Position.EndCol);
		}

		[Test]
		public void testOneSectionWithTargetSpecifier()
		{
			string fileString = "[assembly : SomeAttribute(1)]";

			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();
			AttributeSectionCollection attributeSections = AttributeSectionCollection.parse(t);
			Assertion.AssertEquals("Generate is wrong!", fileString, attributeSections.Generate());

			Assertion.AssertEquals("Attribute section count is wrong!", 1, attributeSections.Count);
			Assertion.AssertEquals("StartLine is wrong!", 1, attributeSections.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, attributeSections.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, attributeSections.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 29, attributeSections.Position.EndCol);
		}
	}
}
