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
	public class CompilationUnitTest : ParserTest
	{
		[Test]
		public void testOneUsingDirective()
		{
			string fileString = "using System;";
			CompilationUnit unit = ParseTestFile(fileString);
			AssertEquals("Namespace count is wrong! ", 0, unit.Namespaces.Count);
			AssertEquals("Using directive count is wrong! ", 1, unit.UsingDirectives.Count);
			Assertion.AssertEquals("PrettyPring string is wrong!", fileString, unit.Generate());
		}

		[Test]
		public void testOneUsingDirectiveAndOneNamespace()
		{
			string fileString = "using System; namespace ns{}";
			CompilationUnit unit = ParseTestFile(fileString);
			AssertEquals("Namespace count is wrong! ", 1, unit.Namespaces.Count);
			AssertEquals("Using directive count is wrong! ", 1, unit.UsingDirectives.Count);
			Assertion.AssertEquals("PrettyPring string is wrong!", fileString, unit.Generate());
		}

		[Test]
		public void testOneUsingDirectiveAndNestedNamespaces()
		{
			string fileString = "using System; namespace ns{namespace ns2{}}";
			CompilationUnit unit = ParseTestFile(fileString);
			AssertEquals("Namespace count is wrong! ", 1, unit.Namespaces.Count);
			AssertEquals("Using directive count is wrong! ", 1, unit.Namespaces[0].Namespaces.Count);
			AssertEquals("Using directive count is wrong! ", 1, unit.UsingDirectives.Count);
			Assertion.AssertEquals("PrettyPring string is wrong!", fileString, unit.Generate());
		}

		[Test]
		public void testOneUsingDirectiveInNamespaces()
		{
			string fileString = "namespace ns{using System;}";
			CompilationUnit unit = ParseTestFile(fileString);
			AssertEquals("Namespace count is wrong! ", 1, unit.Namespaces.Count);
			AssertEquals("Using directive count is wrong! ", 1, unit.Namespaces[0].UsingDirectives.Count);
			Assertion.AssertEquals("PrettyPring string is wrong!", fileString, unit.Generate());
		}
	}
}
