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
using Opnieuw.Framework;

namespace Opnieuw.Parsers.CSParser.Test
{
	[TestFixture]
	public class DelegateTest : ParserTest
	{
		[Test]
		public void testSimple()
		{
			//Preparation
			string fileString = "";
			fileString += "namespace TestNamespace\r\n";
			fileString += "{\r\n";
			fileString += "  [attribute] public delegate int D2(int c, double d);\r\n";
			fileString += "}";

			//Execution
			CompilationUnit unit = ParseTestFile(fileString);

			//General tests
			Assertion.AssertEquals("Namespace count is wrong! ", 1, unit.Members.Count);
			Namespace ns = unit.Members[0] as Namespace;
			Assertion.AssertEquals("Namespace name is wrong! ", "TestNamespace", ns.Name);
			Assertion.AssertEquals("Member count is wrong! ", 1, ns.Members.Count);
			Assertion.Assert("Expected Delegate but got something else!", ns.Members[0] is Delegate);
			Delegate dlg = ns.Members[0] as Delegate;
			Assertion.AssertEquals("Delegate type is wrong!", "int", dlg.Type.Name);
			Assertion.AssertEquals("Delegate name is wrong!", "D2", dlg.Name);
			Assertion.AssertEquals("Delegate fixed parameter count is wrong!", 2, dlg.Parameters.FixedParameters.Count);
			FixedParameter param1 = dlg.Parameters.FixedParameters[0];
			Assertion.AssertEquals("First parameter type is wrong!", "int", param1.Type.Name);
			Assertion.AssertEquals("First parameter name is wrong!", "c", param1.Name);
			FixedParameter param2 = dlg.Parameters.FixedParameters[1];
			Assertion.AssertEquals("Second parameter type is wrong!", "double", param2.Type.Name);
			Assertion.AssertEquals("Second parameter name is wrong!", "d", param2.Name);

			//PrettyPrint test
			Assertion.AssertEquals("PrettyPrint is wrong!", fileString, unit.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 3, param1.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 38, param1.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 3, param1.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 42, param1.Position.EndCol);

			Assertion.AssertEquals("StartLine is wrong!", 3, param2.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 45, param2.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 3, param2.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 52, param2.Position.EndCol);

			Assertion.AssertEquals("StartLine is wrong!", 3, dlg.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, dlg.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 3, dlg.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 54, dlg.Position.EndCol);
		}
		
		[Test]
		public void testWithComment()
		{
			//Preparation
			string fileString = "";
			fileString += "namespace TestNamespace\r\n";
			fileString += "{\r\n";
			fileString += "  //Single line comment.\r\n";
			fileString += "  [attribute] public delegate int D2(int c, double d);\r\n";
			fileString += "}";

			//Execution
			CompilationUnit unit = ParseTestFile(fileString);

			//General tests
			Assertion.AssertEquals("Namespace count is wrong! ", 1, unit.Members.Count);
			Namespace ns = unit.Members[0] as Namespace;
			Assertion.AssertEquals("Namespace name is wrong! ", "TestNamespace", ns.Name);
			Assertion.AssertEquals("Member count is wrong! ", 1, ns.Members.Count);
			Assertion.Assert("Expected Delegate but got something else!", ns.Members[0] is Delegate);
			Delegate dlg = ns.Members[0] as Delegate;
			Assertion.AssertEquals("Delegate type is wrong!", "int", dlg.Type.Name);
			Assertion.AssertEquals("Delegate name is wrong!", "D2", dlg.Name);
			Assertion.AssertEquals("Delegate fixed parameter count is wrong!", 2, dlg.Parameters.FixedParameters.Count);
			FixedParameter param1 = dlg.Parameters.FixedParameters[0];
			Assertion.AssertEquals("First parameter type is wrong!", "int", param1.Type.Name);
			Assertion.AssertEquals("First parameter name is wrong!", "c", param1.Name);
			FixedParameter param2 = dlg.Parameters.FixedParameters[1];
			Assertion.AssertEquals("Second parameter type is wrong!", "double", param2.Type.Name);
			Assertion.AssertEquals("Second parameter name is wrong!", "d", param2.Name);
			
			//PrettyPrint test
			Assertion.AssertEquals("PrettyPrint is wrong!", fileString, unit.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 4, param1.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 38, param1.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 4, param1.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 42, param1.Position.EndCol);

			Assertion.AssertEquals("StartLine is wrong!", 4, param2.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 45, param2.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 4, param2.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 52, param2.Position.EndCol);

			Assertion.AssertEquals("StartLine is wrong!", 4, dlg.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, dlg.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 4, dlg.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 54, dlg.Position.EndCol);
		}
	}
}
