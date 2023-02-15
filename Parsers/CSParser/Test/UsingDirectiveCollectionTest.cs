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
using Opnieuw.Parsers.CSParser;

namespace Opnieuw.Parsers.CSParser.Test
{
	[TestFixture]
	public class UsingDirectiveCollectionTest : ParserTest
	{
		[Test]
		public void testAFewUsingsAndANamespace()
		{
			string fileString = "";
			fileString += "using System;\r\n";
			fileString += "using NUnit.Framework;\r\n";
			fileString += "using System.IO;\r\n";
			fileString += "using Opnieuw.Parsers.CSParser;\r\n";

			CompilationUnit unit = ParseTestFile(fileString);
			AssertEquals("Using directive count is wrong!", 4, unit.UsingDirectives.Count);

			UsingDirective directive1 = unit.UsingDirectives[0];
			UsingDirective directive2 = unit.UsingDirectives[1];
			UsingDirective directive3 = unit.UsingDirectives[2];
			UsingDirective directive4 = unit.UsingDirectives[3];

			AssertEquals("StartLine is wrong!", 1, directive1.Position.StartLine);
			AssertEquals("StartCol is wrong!", 1, directive1.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, directive1.Position.EndLine);
			AssertEquals("EndCol is wrong!", 13, directive1.Position.EndCol);

			AssertEquals("StartLine is wrong!", 2, directive2.Position.StartLine);
			AssertEquals("StartCol is wrong!", 1, directive2.Position.StartCol);
			AssertEquals("EndLine is wrong!", 2, directive2.Position.EndLine);
			AssertEquals("EndCol is wrong!", 22, directive2.Position.EndCol);

			AssertEquals("StartLine is wrong!", 3, directive3.Position.StartLine);
			AssertEquals("StartCol is wrong!", 1, directive3.Position.StartCol);
			AssertEquals("EndLine is wrong!", 3, directive3.Position.EndLine);
			AssertEquals("EndCol is wrong!", 16, directive3.Position.EndCol);

			AssertEquals("StartLine is wrong!", 4, directive4.Position.StartLine);
			AssertEquals("StartCol is wrong!", 1, directive4.Position.StartCol);
			AssertEquals("EndLine is wrong!", 4, directive4.Position.EndLine);
			AssertEquals("EndCol is wrong!", 31, directive4.Position.EndCol);
		}
	}
}
