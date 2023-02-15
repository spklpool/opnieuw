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
	public class EnumTest : TokenizerTestBase
	{
		[Test]
		public void testWithNoMembers()
		{
			//Preparation
			string fileString = "";
			fileString += "//Some comment.\r\n";
			fileString += "enum TestEnum\r\n";
			fileString += "{\r\n";
			fileString += "}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			EnumDeclaration enm = EnumDeclaration.parse(t) as EnumDeclaration;

			//PrettyPrint test
			AssertEquals("Pretty print string was wrong!", fileString, enm.Generate());

			//Position tests
			AssertEquals("StartLine is wrong!", 2, enm.Position.StartLine);
			AssertEquals("StartCol is wrong!", 1, enm.Position.StartCol);
			AssertEquals("EndLine is wrong!", 4, enm.Position.EndLine);
			AssertEquals("EndCol is wrong!", 1, enm.Position.EndCol);
		}

		[Test]
		public void testWithNoMembersAndBase()
		{
			//Preparation
			string fileString = "";
			fileString += "enum TestEnum : TestEnumBase\r\n";
			fileString += "{\r\n";
			fileString += "}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			EnumDeclaration enm = EnumDeclaration.parse(t) as EnumDeclaration;

			//General tests
			Assert("Expected an enum base but did not find it!", false == (enm.Base is MissingEnumBase));

			//PrettyPrint test
			AssertEquals("Pretty print string was wrong!", fileString, enm.Generate());

			//Position tests
			AssertEquals("StartLine is wrong!", 1, enm.Position.StartLine);
			AssertEquals("StartCol is wrong!", 1, enm.Position.StartCol);
			AssertEquals("EndLine is wrong!", 3, enm.Position.EndLine);
			AssertEquals("EndCol is wrong!", 1, enm.Position.EndCol);
		}

		[Test]
		public void testWithMembers()
		{
			//Preparation
			string fileString = "";
			fileString += "enum TestEnum : TestEnumBase\r\n";
			fileString += "{\r\n";
			fileString += "  firstMember,\r\n";
			fileString += "  secondMember,\r\n";
			fileString += "  thirdMember,\r\n";
			fileString += "  fourthMember\r\n";
			fileString += "}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			EnumDeclaration enm = EnumDeclaration.parse(t) as EnumDeclaration;

			//General tests
			AssertEquals("Number of children is wrong!", 4, enm.Members.Children.Count);
			Assert("Expected an enum base but did not find it!", false == (enm.Base is MissingEnumBase));
			AssertEquals("Number of enum members is wrong!", 4, enm.Members.Count);
			EnumMemberCollection enumMembers = enm.Members;
			AssertEquals("The name of the first enum member is wong!", "firstMember", enumMembers[0].Name);
			AssertEquals("The name of the second enum member is wong!", "secondMember", enumMembers[1].Name);

			//PrettyPrint test
			AssertEquals("Pretty print string was wrong!", fileString, enm.Generate());

			//Position tests
			AssertEquals("StartLine is wrong!", 3, enumMembers[0].Position.StartLine);
			AssertEquals("StartCol is wrong!", 3, enumMembers[0].Position.StartCol);
			AssertEquals("EndLine is wrong!", 3, enumMembers[0].Position.EndLine);
			AssertEquals("EndCol is wrong!", 13, enumMembers[0].Position.EndCol);
			AssertEquals("StartLine is wrong!", 4, enumMembers[1].Position.StartLine);
			AssertEquals("StartCol is wrong!", 3, enumMembers[1].Position.StartCol);
			AssertEquals("EndLine is wrong!", 4, enumMembers[1].Position.EndLine);
			AssertEquals("EndCol is wrong!", 14, enumMembers[1].Position.EndCol);
			AssertEquals("The name of the third enum member is wong!", "thirdMember", enumMembers[2].Name);
			AssertEquals("StartLine is wrong!", 5, enumMembers[2].Position.StartLine);
			AssertEquals("StartCol is wrong!", 3, enumMembers[2].Position.StartCol);
			AssertEquals("EndLine is wrong!", 5, enumMembers[2].Position.EndLine);
			AssertEquals("EndCol is wrong!", 13, enumMembers[2].Position.EndCol);
			AssertEquals("The name of the fourth enum member is wong!", "fourthMember", enumMembers[3].Name);
			AssertEquals("StartLine is wrong!", 6, enumMembers[3].Position.StartLine);
			AssertEquals("StartCol is wrong!", 3, enumMembers[3].Position.StartCol);
			AssertEquals("EndLine is wrong!", 6, enumMembers[3].Position.EndLine);
			AssertEquals("EndCol is wrong!", 14, enumMembers[3].Position.EndCol);
			AssertEquals("StartLine is wrong!", 1, enm.Position.StartLine);
			AssertEquals("StartCol is wrong!", 1, enm.Position.StartCol);
			AssertEquals("EndLine is wrong!", 7, enm.Position.EndLine);
			AssertEquals("EndCol is wrong!", 1, enm.Position.EndCol);
		}

		[Test]
		public void testWithMembersWithExpressions()
		{
			//Preparation
			string fileString = "";
			fileString += "enum TestEnum : TestEnumBase\r\n";
			fileString += "{\r\n";
			fileString += "  firstMember = 1,\r\n";
			fileString += "  secondMember = 2,\r\n";
			fileString += "  thirdMember = 3,\r\n";
			fileString += "  fourthMember = 4\r\n";
			fileString += "}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			EnumDeclaration enm = EnumDeclaration.parse(t) as EnumDeclaration;

			//General tests
			AssertEquals("Number of children is wrong!", 4, enm.Members.Children.Count);
			Assert("Expected an enum base but did not find it!", false == (enm.Base is MissingEnumBase));
			AssertEquals("Number of enum members is wrong!", 4, enm.Members.Count);
			EnumMemberCollection enumMembers = enm.Members;
			AssertEquals("The name of the first enum member is wong!", "firstMember", enumMembers[0].Name);
			AssertEquals("The name of the second enum member is wong!", "secondMember", enumMembers[1].Name);

			//PrettyPrint test
			AssertEquals("Pretty print string was wrong!", fileString, enm.Generate());

			//Position tests
			AssertEquals("StartLine is wrong!", 3, enumMembers[0].Position.StartLine);
			AssertEquals("StartCol is wrong!", 3, enumMembers[0].Position.StartCol);
			AssertEquals("EndLine is wrong!", 3, enumMembers[0].Position.EndLine);
			AssertEquals("EndCol is wrong!", 17, enumMembers[0].Position.EndCol);
			AssertEquals("StartLine is wrong!", 4, enumMembers[1].Position.StartLine);
			AssertEquals("StartCol is wrong!", 3, enumMembers[1].Position.StartCol);
			AssertEquals("EndLine is wrong!", 4, enumMembers[1].Position.EndLine);
			AssertEquals("EndCol is wrong!", 18, enumMembers[1].Position.EndCol);
			AssertEquals("The name of the third enum member is wong!", "thirdMember", enumMembers[2].Name);
			AssertEquals("StartLine is wrong!", 5, enumMembers[2].Position.StartLine);
			AssertEquals("StartCol is wrong!", 3, enumMembers[2].Position.StartCol);
			AssertEquals("EndLine is wrong!", 5, enumMembers[2].Position.EndLine);
			AssertEquals("EndCol is wrong!", 17, enumMembers[2].Position.EndCol);
			AssertEquals("The name of the fourth enum member is wong!", "fourthMember", enumMembers[3].Name);
			AssertEquals("StartLine is wrong!", 6, enumMembers[3].Position.StartLine);
			AssertEquals("StartCol is wrong!", 3, enumMembers[3].Position.StartCol);
			AssertEquals("EndLine is wrong!", 6, enumMembers[3].Position.EndLine);
			AssertEquals("EndCol is wrong!", 18, enumMembers[3].Position.EndCol);
			AssertEquals("StartLine is wrong!", 1, enm.Position.StartLine);
			AssertEquals("StartCol is wrong!", 1, enm.Position.StartCol);
			AssertEquals("EndLine is wrong!", 7, enm.Position.EndLine);
			AssertEquals("EndCol is wrong!", 1, enm.Position.EndCol);
		}

		[Test]
		public void testWithComments()
		{
			//Preparation
			string fileString = "";
			fileString += "//Single line comment\r\n";
			fileString += "enum TestEnum : TestEnumBase\r\n";
			fileString += "{\r\n";
			fileString += "  //FirstMember comment.\r\n";
			fileString += "  firstMember,\r\n";
			fileString += "  //SecondMember comment.\r\n";
			fileString += "  secondMember,\r\n";
			fileString += "  //ThirdMember comment.\r\n";
			fileString += "  thirdMember,\r\n";
			fileString += "  //FourthMember comment.\r\n";
			fileString += "  fourthMember\r\n";
			fileString += "}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			EnumDeclaration enm = EnumDeclaration.parse(t) as EnumDeclaration;

			//General tests
			AssertEquals("Number of children is wrong!", 4, enm.Members.Children.Count);
			Assert("Expected an enum base but did not find it!", false == (enm.Base is MissingEnumBase));
			AssertEquals("Number of enum members is wrong!", 4, enm.Members.Count);
			EnumMemberCollection enumMembers = enm.Members;
			AssertEquals("The name of the first enum member is wong!", "firstMember", enumMembers[0].Name);
			AssertEquals("The name of the second enum member is wong!", "secondMember", enumMembers[1].Name);

			//PrettyPrint test
			AssertEquals("Pretty print string was wrong!", fileString, enm.Generate());

			//Position tests
			AssertEquals("StartLine is wrong!", 5, enumMembers[0].Position.StartLine);
			AssertEquals("StartCol is wrong!", 3, enumMembers[0].Position.StartCol);
			AssertEquals("EndLine is wrong!", 5, enumMembers[0].Position.EndLine);
			AssertEquals("EndCol is wrong!", 13, enumMembers[0].Position.EndCol);
			AssertEquals("StartLine is wrong!", 7, enumMembers[1].Position.StartLine);
			AssertEquals("StartCol is wrong!", 3, enumMembers[1].Position.StartCol);
			AssertEquals("EndLine is wrong!", 7, enumMembers[1].Position.EndLine);
			AssertEquals("EndCol is wrong!", 14, enumMembers[1].Position.EndCol);
			AssertEquals("The name of the third enum member is wong!", "thirdMember", enumMembers[2].Name);
			AssertEquals("StartLine is wrong!", 9, enumMembers[2].Position.StartLine);
			AssertEquals("StartCol is wrong!", 3, enumMembers[2].Position.StartCol);
			AssertEquals("EndLine is wrong!", 9, enumMembers[2].Position.EndLine);
			AssertEquals("EndCol is wrong!", 13, enumMembers[2].Position.EndCol);
			AssertEquals("The name of the fourth enum member is wong!", "fourthMember", enumMembers[3].Name);
			AssertEquals("StartLine is wrong!", 11, enumMembers[3].Position.StartLine);
			AssertEquals("StartCol is wrong!", 3, enumMembers[3].Position.StartCol);
			AssertEquals("EndLine is wrong!", 11, enumMembers[3].Position.EndLine);
			AssertEquals("EndCol is wrong!", 14, enumMembers[3].Position.EndCol);
			AssertEquals("StartLine is wrong!", 2, enm.Position.StartLine);
			AssertEquals("StartCol is wrong!", 1, enm.Position.StartCol);
			AssertEquals("EndLine is wrong!", 12, enm.Position.EndLine);
			AssertEquals("EndCol is wrong!", 1, enm.Position.EndCol);
		}
	}
}
