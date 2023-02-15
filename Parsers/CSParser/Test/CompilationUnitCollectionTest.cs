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
	public class CompilationUnitCollectionTest : ParserTest
	{
		[Test]
		public void testSingleUnit()
		{
			CompilationUnitCollection col = new CompilationUnitCollection();
			string testString1 = "namespace someNamespace{class someClass{public void someMethod() {}}}";
			col.Add(ParseTestFile(testString1));
			Assertion.AssertEquals("Number of units is wrong!", 1, col.Count);
		}

		[Test]
		public void testTwoDifferentUnits()
		{
			CompilationUnitCollection col = new CompilationUnitCollection();
			string fileName1 = "test1.cs";
			string testString1 = "namespace someNamespace1{class someClass1{public void someMethod1() {}}}";
			WriteTestFile(testString1, fileName1);
			col.Add(ParseExistingFile(fileName1));
			string fileName2 = "test2.cs";
			string testString2 = "namespace someNamespace2{class someClass2{public void someMethod2() {}}}";
			WriteTestFile(testString2, fileName2);
			col.Add(ParseExistingFile(fileName2));
			
			Assertion.AssertEquals("Number of units is wrong!", 2, col.Count);
			Assertion.AssertEquals("Number of namespaces is wrong!", 2, col.NamespaceMembers.Count);

 			System.IO.File.Delete("test1.cs");
 			System.IO.File.Delete("test2.cs");
		}

		[Test]
		public void testTwoDifferentUnitsContributingToOneNamespace()
		{
			CompilationUnitCollection col = new CompilationUnitCollection();
			string fileName1 = "test1.cs";
			string testString1 = "namespace someNamespace{using someOtherNamespace1; class someClass1{public void someMethod1() {}}}";
			WriteTestFile(testString1, fileName1);
			col.Add(ParseExistingFile(fileName1));
			string fileName2 = "test2.cs";
			string testString2 = "namespace someNamespace{using someOtherNamespace2; class someClass2{public void someMethod2() {}}}";
			WriteTestFile(testString2, fileName2);
			col.Add(ParseExistingFile(fileName2));
			Namespace ns = col.NamespaceMembers[0] as Namespace;

			Assertion.AssertEquals("Number of units is wrong!", 2, col.Count);
			Assertion.AssertEquals("Number of namespaces is wrong!", 1, col.NamespaceMembers.Count);
			Assertion.AssertEquals("Number of members in namespace is wrong!", 2, ns.Members.Count);
			Assertion.AssertEquals("Number of using directives in namespace is wrong!", 2, ns.UsingDirectives.Count);

 			System.IO.File.Delete("test1.cs");
 			System.IO.File.Delete("test2.cs");
		}

		[Test]
		public void testTwoIdenticalUnits()
		{
			CompilationUnitCollection col = new CompilationUnitCollection();
			string fileName1 = "test1.cs";
			string testString1 = "namespace someNamespace{class someClass{public void someMethod() {}}}";
			WriteTestFile(testString1, fileName1);
			col.Add(ParseExistingFile(fileName1));
			string fileName2 = "test2.cs";
			string testString2 = "namespace someNamespace{class someClass{public void someMethod() {}}}";
			WriteTestFile(testString2, fileName2);
			col.Add(ParseExistingFile(fileName2));
			Assertion.AssertEquals("Number of units is wrong!", 1, col.Count);

 			System.IO.File.Delete("test1.cs");
 			System.IO.File.Delete("test2.cs");
		}
	}
}