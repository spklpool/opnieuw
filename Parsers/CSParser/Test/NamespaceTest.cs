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
	public class NamespaceTest : ParserTest
	{
		[Test]
		public void testSimple()
		{
			//Preparation
			string fileString = "";
			fileString += "namespace n\r\n";
			fileString += "{\r\n";
			fileString += "}";

			//Execution
			CompilationUnit unit = ParseTestFile(fileString);

			//General tests
			NamespaceMemberCollection members = unit.Members;
			Assertion.AssertEquals("Namespace member count is wrong!", 1, members.Count);
			Namespace n = members[0] as Namespace;
			Assertion.AssertEquals("Fully qualified parent namespace name is wrong!", "", members[0].FullyQualifiedParentNamespace);

			//PrettyPrint test
			AssertEquals("Pretty print string was wrong!", fileString, n.Generate());

			//Position tests
			AssertEquals("StartLine is wrong!", 1, n.Position.StartLine);
			AssertEquals("StartCol is wrong!", 1, n.Position.StartCol);
			AssertEquals("EndLine is wrong!", 3, n.Position.EndLine);
			AssertEquals("EndCol is wrong!", 1, n.Position.EndCol);
		}

		[Test]
		public void testTwoNamespaces()
		{
			//Preparation
			string fileString = "";
			fileString += "namespace n\r\n";
			fileString += "{\r\n";
			fileString += "  using System;\r\n";
			fileString += "  //this is a comment.\r\n";
			fileString += "  namespace innerN\r\n";
			fileString += "  {\r\n";
			fileString += "    using InnerSystem;\r\n";
			fileString += "  }\r\n";
			fileString += "}";

			//Execution
			CompilationUnit unit = ParseTestFile(fileString);

			//General tests
			NamespaceMemberCollection members = unit.Members;
			Assertion.AssertEquals("Namespace member count is wrong!", 1, members.Count);
			Namespace n = members[0] as Namespace;
			Namespace n2 = n.Namespaces[0];
			Assertion.AssertEquals("Fully qualified parent namespace name is wrong!", "", n.FullyQualifiedParentNamespace);
			Assertion.AssertEquals("Fully qualified parent namespace name is wrong!", "n", n2.FullyQualifiedParentNamespace);

			//PrettyPrint test
			AssertEquals("Pretty print string was wrong!", fileString, n.Generate());

			//Position tests
			AssertEquals("StartLine is wrong!", 1, n.Position.StartLine);
			AssertEquals("StartCol is wrong!", 1, n.Position.StartCol);
			AssertEquals("EndLine is wrong!", 9, n.Position.EndLine);
			AssertEquals("EndCol is wrong!", 1, n.Position.EndCol);
		}

		[Test]
		public void testNotSoSimple()
		{
			//Preparation
			string fileString = "";
			fileString += "namespace n1\r\n";
			fileString += "{\r\n";
			fileString += "  using System;\r\n";
			fileString += "  namespace n2\r\n";
			fileString += "  {\r\n";
			fileString += "    using InnerSystem;\r\n";
			fileString += "  }\r\n";
			fileString += "  namespace n3\r\n";
			fileString += "  {\r\n";
			fileString += "    using InnerSystem;\r\n";
			fileString += "    namespace n4\r\n";
			fileString += "    {\r\n";
			fileString += "      using InnerSystemTwo;\r\n";
			fileString += "      using systemAlias = System;\r\n";
			fileString += "    }\r\n";
			fileString += "  }\r\n";
			fileString += "}";

			//Execution
			CompilationUnit unit = ParseTestFile(fileString);

			//General tests
			NamespaceMemberCollection members = unit.Members;
			Namespace n1 = members[0] as Namespace;
			Namespace n2 = n1.Members[0] as Namespace;
			Namespace n3 = n1.Members[1] as Namespace;
			Namespace n4 = n3.Members[0] as Namespace;
			Assertion.AssertEquals("Namespace member count is wrong!", 1, members.Count);
			Assertion.AssertEquals("First namespace name is wrong!", "n1", n1.Name);
			Assertion.AssertEquals("Count of inner namespaces in n1 is wrong!", 2, n1.Members.Count);
			Assertion.AssertEquals("Fully qualified parent namespace name is wrong!", "", n1.FullyQualifiedParentNamespace);
			Assertion.AssertEquals("Second namespace name is wrong!", "n2", n2.Name);
			Assertion.AssertEquals("Using directives count of namespace 2 is wrong!", 1, n2.UsingDirectives.Count);
			Assertion.AssertEquals("Fully qualified parent namespace name is wrong!", "n1", n2.FullyQualifiedParentNamespace);
			Assertion.AssertEquals("Third namespace name is wrong!", "n3", n3.Name);
			Assertion.AssertEquals("Using directives count of namespace 3 is wrong!", 1, n3.UsingDirectives.Count);
			Assertion.AssertEquals("Fully qualified parent namespace name is wrong!", "n1", n3.FullyQualifiedParentNamespace);
			Assertion.AssertEquals("Fourth namespace name is wrong!", "n4", n4.Name);
			Assertion.AssertEquals("Using directives count of namespace 4 is wrong!", 2, n4.UsingDirectives.Count);
			Assertion.AssertEquals("Namespace member count of namespace 4 is wrong!", 0, n4.Members.Count);
			Assertion.AssertEquals("Fully qualified parent namespace name is wrong!", "n1.n3", n4.FullyQualifiedParentNamespace);
			
			//PrettyPrint test
			AssertEquals("Pretty print string was wrong!", fileString, n1.Generate());

			//Position tests
			AssertEquals("StartLine is wrong!", 1, n1.Position.StartLine);
			AssertEquals("StartCol is wrong!", 1, n1.Position.StartCol);
			AssertEquals("EndLine is wrong!", 17, n1.Position.EndLine);
			AssertEquals("EndCol is wrong!", 1, n1.Position.EndCol);
			AssertEquals("StartLine is wrong!", 4, n2.Position.StartLine);
			AssertEquals("StartCol is wrong!", 3, n2.Position.StartCol);
			AssertEquals("EndLine is wrong!", 7, n2.Position.EndLine);
			AssertEquals("EndCol is wrong!", 3, n2.Position.EndCol);
			AssertEquals("StartLine is wrong!", 8, n3.Position.StartLine);
			AssertEquals("StartCol is wrong!", 3, n3.Position.StartCol);
			AssertEquals("EndLine is wrong!", 16, n3.Position.EndLine);
			AssertEquals("EndCol is wrong!", 3, n3.Position.EndCol);
			AssertEquals("StartLine is wrong!", 11, n4.Position.StartLine);
			AssertEquals("StartCol is wrong!", 5, n4.Position.StartCol);
			AssertEquals("EndLine is wrong!", 15, n4.Position.EndLine);
			AssertEquals("EndCol is wrong!", 5, n4.Position.EndCol);
		}
	}
}
