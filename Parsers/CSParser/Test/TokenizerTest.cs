#region Copyright (C) Pierre Boudreau
//This file is part of the Opnieuw C# parser.
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
using System.IO;
using Opnieuw.Parsers.CSParser;

namespace Opnieuw.Parsers.CSParser.Test
{
	[TestFixture]
	public class TokenizerTest : TokenizerTestBase
	{

		[Test]
		public void testCheckToken1()
		{
			string fileString = "class Test{}";

			WriteTestFile(fileString, m_FileName);
			fs = File.OpenRead(m_FileName);
			Tokenizer t = new Tokenizer(fs, "", null);

			t.nextToken(); // class
			t.nextToken(); // Test
			t.nextToken(); // {
			bool exceptionCaught = false;
			try
			{
				t.checkToken(Token.OPEN_BRACE);
			}
			catch (ParserException)
			{
				exceptionCaught = true;
			}
			Assertion.AssertEquals("Exception was expected but not received!", false, exceptionCaught);
		}

		[Test]
		public void testCheckToken2()
		{
			string fileString = "class Test{}";

			WriteTestFile(fileString, m_FileName);
			fs = File.OpenRead(m_FileName);
			Tokenizer t = new Tokenizer(fs, "", null);

			t.nextToken(); // class
			t.nextToken(); // Test
			t.nextToken(); // {
			bool exceptionCaught = false;
			try
			{
				t.checkToken(Token.NONE);
			}
			catch (ParserException e)
			{
				Assertion.AssertEquals("Expected token type was wrong in exception!", Token.NONE, e.Expected);
				Assertion.AssertEquals("StartLine is wrong!", 1, e.Actual.Position.StartLine);
				Assertion.AssertEquals("StartCol is wrong!", 11, e.Actual.Position.StartCol);
				Assertion.AssertEquals("EndLine is wrong!", 1, e.Actual.Position.EndLine);
				Assertion.AssertEquals("EndCol is wrong!", 11, e.Actual.Position.EndCol);
				exceptionCaught = true;
			}
			Assertion.AssertEquals("Exception was expected but not received!", true, exceptionCaught);
		}

		[Test]
		public void testBookmark1() 
		{
			string fileString = "";
			fileString += "class Test{}\r\n";

			WriteTestFile(fileString, m_FileName);
			fs = File.OpenRead(m_FileName);
			Tokenizer t = new Tokenizer(fs, "", null);

			t.nextToken(); // class
			AssertToken(t.CurrentToken, "class", 1, 1, 1, 5);
			t.nextToken(); // Test
			t.setBookmark();
			AssertToken(t.CurrentToken, "Test", 1, 7, 1, 10);
			t.nextToken(); // {
			AssertToken(t.CurrentToken, "{", 1, 11, 1, 11);
			t.returnToBookmark();
			AssertToken(t.CurrentToken, "Test", 1, 7, 1, 10);
			t.nextToken(); // {
			AssertToken(t.CurrentToken, "{", 1, 11, 1, 11);
		}

		[Test]
		public void testBookmarkStack1() 
		{
			string fileString = "";
			fileString += "class Test{}\r\n";

			WriteTestFile(fileString, m_FileName);
			fs = File.OpenRead(m_FileName);
			Tokenizer t = new Tokenizer(fs, "", null);

			t.nextToken(); // class
			AssertToken(t.CurrentToken, "class", 1, 1, 1, 5);
			t.nextToken(); // Test
			t.setBookmark();
			AssertToken(t.CurrentToken, "Test", 1, 7, 1, 10);
			t.nextToken(); // {
			t.setBookmark();
			AssertToken(t.CurrentToken, "{", 1, 11, 1, 11);
			t.nextToken(); // }
			t.returnToBookmark();
			AssertToken(t.CurrentToken, "{", 1, 11, 1, 11);
			t.returnToBookmark();
			AssertToken(t.CurrentToken, "Test", 1, 7, 1, 10);
		}

		[Test]
		public void testBookmarkStack2() 
		{
			string fileString = "";
			fileString += "1 2 3 4 5 6 7 8 9";

			WriteTestFile(fileString, m_FileName);
			fs = File.OpenRead(m_FileName);
			Tokenizer t = new Tokenizer(fs, "", null);

			t.nextToken(); // 1
			AssertToken(t.CurrentToken, "1", 1, 1, 1, 1);
			t.nextToken(); // 2
			t.setBookmark();
			AssertToken(t.CurrentToken, "2", 1, 3, 1, 3);
			t.nextToken(); // 3
			t.setBookmark();
			AssertToken(t.CurrentToken, "3", 1, 5, 1, 5);
			t.nextToken(); // 4
			t.returnToBookmark();
			AssertToken(t.CurrentToken, "3", 1, 5, 1, 5);
			t.nextToken(); // 4
			t.setBookmark();
			AssertToken(t.CurrentToken, "4", 1, 7, 1, 7);
			t.nextToken(); // 5
			AssertToken(t.CurrentToken, "5", 1, 9, 1, 9);
			t.nextToken(); // 6
			t.cancelBookmark();
			AssertToken(t.CurrentToken, "6", 1, 11, 1, 11);
			t.nextToken(); // 7
			AssertToken(t.CurrentToken, "7", 1, 13, 1, 13);
			t.returnToBookmark();
			AssertToken(t.CurrentToken, "2", 1, 3, 1, 3);
		}

		[Test]
		public void testCancelBookmark() 
		{
			string fileString = "";
			fileString += "class Test{}\r\n";

			WriteTestFile(fileString, m_FileName);
			fs = File.OpenRead(m_FileName);
			Tokenizer t = new Tokenizer(fs, "", null);

			t.nextToken(); // class
			AssertToken(t.CurrentToken, "class", 1, 1, 1, 5);
			t.nextToken(); // Test
			t.setBookmark();
			AssertToken(t.CurrentToken, "Test", 1, 7, 1, 10);
			t.nextToken(); // {
			t.cancelBookmark();
			t.setBookmark();
			AssertToken(t.CurrentToken, "{", 1, 11, 1, 11);
			t.nextToken(); // }
			t.returnToBookmark();
			AssertToken(t.CurrentToken, "{", 1, 11, 1, 11);
			t.returnToBookmark(); // this one was cancelled
			AssertToken(t.CurrentToken, "{", 1, 11, 1, 11);
		}

		[Test]
		public void testBookmark2() 
		{
			string fileString = "";
			fileString += "class Test{int someMethod(int someParameter, int someOtherParam){return 3}}\r\n";

			WriteTestFile(fileString, m_FileName);
			fs = File.OpenRead(m_FileName);
			Tokenizer t = new Tokenizer(fs, "", null);

			t.nextToken(); // class
			AssertToken(t.CurrentToken, "class", 1, 1, 1, 5);
			t.nextToken(); // Test
			t.setBookmark();
			AssertToken(t.CurrentToken, "Test", 1, 7, 1, 10);
			t.nextToken(); // {
			AssertToken(t.CurrentToken, "{", 1, 11, 1, 11);
			t.returnToBookmark();
			AssertToken(t.CurrentToken, "Test", 1, 7, 1, 10);
			t.nextToken(); // {
			AssertToken(t.CurrentToken, "{", 1, 11, 1, 11);
			t.nextToken(); // int
			AssertToken(t.CurrentToken, "int", 1, 12, 1, 14);
			t.nextToken(); // someMethod
			AssertToken(t.CurrentToken, "someMethod", 1, 16, 1, 25);
			t.nextToken(); // (
			AssertToken(t.CurrentToken, "(", 1, 26, 1, 26);
			t.nextToken(); // int
			AssertToken(t.CurrentToken, "int", 1, 27, 1, 29);
			t.nextToken(); // someParameter
			AssertToken(t.CurrentToken, "someParameter", 1, 31, 1, 43);
			t.nextToken(); // ,
			AssertToken(t.CurrentToken, ",", 1, 44, 1, 44);
			t.nextToken(); // int
			AssertToken(t.CurrentToken, "int", 1, 46, 1, 48);
			t.nextToken(); // someOtherParam
			t.setBookmark();
			AssertToken(t.CurrentToken, "someOtherParam", 1, 50, 1, 63);
			t.nextToken(); // )
			AssertToken(t.CurrentToken, ")", 1, 64, 1, 64);
			t.nextToken();
			t.nextToken();
			t.previousToken();
			t.nextToken();
			t.previousToken();
			t.nextToken();
			t.previousToken();
			t.previousToken();
			t.returnToBookmark();
			AssertToken(t.CurrentToken, "someOtherParam", 1, 50, 1, 63);
			t.nextToken(); // )
			AssertToken(t.CurrentToken, ")", 1, 64, 1, 64);
			t.nextToken(); // {
			AssertToken(t.CurrentToken, "{", 1, 65, 1, 65);
			t.nextToken(); // return
			AssertToken(t.CurrentToken, "return", 1, 66, 1, 71);
			t.nextToken(); // 3
			AssertToken(t.CurrentToken, "3", 1, 73, 1, 73);
			t.nextToken(); // }
			AssertToken(t.CurrentToken, "}", 1, 74, 1, 74);
			t.nextToken(); // }
			AssertToken(t.CurrentToken, "}", 1, 75, 1, 75);
		}

		[Test]
		public void testTwoKeywords() 
		{
			string fileString = "";
			fileString += "int long";

			WriteTestFile(fileString, m_FileName);
			fs = File.OpenRead(m_FileName);
			Tokenizer t = new Tokenizer(fs, "", null);

			t.nextToken(); // int
			AssertToken(t.CurrentToken, "int", 1, 1, 1, 3);
			t.nextToken(); // long
			AssertToken(t.CurrentToken, "long", 1, 5, 1, 8);
		}

		[Test]
		public void testManyKeywords() 
		{
			string fileString = "";
			fileString += "int long int long class namespace interface";

			WriteTestFile(fileString, m_FileName);
			fs = File.OpenRead(m_FileName);
			Tokenizer t = new Tokenizer(fs, "", null);

			t.nextToken(); // int
			AssertToken(t.CurrentToken, "int", 1, 1, 1, 3);
			t.nextToken(); // long
			AssertToken(t.CurrentToken, "long", 1, 5, 1, 8);
			t.nextToken(); // int
			AssertToken(t.CurrentToken, "int", 1, 10, 1, 12);
			t.nextToken(); // long
			AssertToken(t.CurrentToken, "long", 1, 14, 1, 17);
			t.nextToken(); // class
			AssertToken(t.CurrentToken, "class", 1, 19, 1, 23);
			t.nextToken(); // namespace
			AssertToken(t.CurrentToken, "namespace", 1, 25, 1, 33);
			t.nextToken(); // interface
			AssertToken(t.CurrentToken, "interface", 1, 35, 1, 43);
		}

		[Test]
		public void testManyKeywordsAndOneInt() 
		{
			string fileString = "";
			fileString += "int long int 3456 class namespace interface";

			WriteTestFile(fileString, m_FileName);
			fs = File.OpenRead(m_FileName);
			Tokenizer t = new Tokenizer(fs, "", null);

			t.nextToken(); // int
			AssertToken(t.CurrentToken, "int", 1, 1, 1, 3);
			t.nextToken(); // long
			AssertToken(t.CurrentToken, "long", 1, 5, 1, 8);
			t.nextToken(); // int
			AssertToken(t.CurrentToken, "int", 1, 10, 1, 12);
			t.nextToken(); // 3456
			AssertToken(t.CurrentToken, "3456", 1, 14, 1, 17);
			t.nextToken(); // class
			AssertToken(t.CurrentToken, "class", 1, 19, 1, 23);
			t.nextToken(); // namespace
			AssertToken(t.CurrentToken, "namespace", 1, 25, 1, 33);
			t.nextToken(); // interface
			AssertToken(t.CurrentToken, "interface", 1, 35, 1, 43);
		}

		[Test]
		public void testBackingUp() 
		{
			string fileString = "";
			fileString += "class Test{}\r\n";

			WriteTestFile(fileString, m_FileName);
			fs = File.OpenRead(m_FileName);
			Tokenizer t = new Tokenizer(fs, "", null);

			t.nextToken(); // class
			AssertToken(t.CurrentToken, "class", 1, 1, 1, 5);
			t.nextToken(); // Test
			AssertToken(t.CurrentToken, "Test", 1, 7, 1, 10);
			t.previousToken(); // class
			AssertToken(t.CurrentToken, "class", 1, 1, 1, 5);
			t.nextToken(); // Test
			AssertToken(t.CurrentToken, "Test", 1, 7, 1, 10);
			t.nextToken(); // {
			AssertToken(t.CurrentToken, "{", 1, 11, 1, 11);
		}

		[Test]
		public void testIdentifier1() 
		{
			string fileString = "";
			fileString += "class Test{}\r\n";

			WriteTestFile(fileString, m_FileName);
			fs = File.OpenRead(m_FileName);
			Tokenizer t = new Tokenizer(fs, "", null);

			t.nextToken(); // class
			AssertToken(t.CurrentToken, "class", 1, 1, 1, 5);
			t.nextToken(); // Test
			AssertToken(t.CurrentToken, "Test", 1, 7, 1, 10);
			t.nextToken(); // {
			AssertToken(t.CurrentToken, "{", 1, 11, 1, 11);
		}

		[Test]
		public void testPunctuation() 
		{
			string fileString = "";
			fileString += "class Test{\r\n";
			fileString += "  SomeObjectType SomeObject;\r\n";
			fileString += "  int i[5];\r\n";
			fileString += "  public Test() {\r\n";
			fileString += "    i = SomeObject.Value;\r\n";
			fileString += "    i++;\r\n";
			fileString += "    i--;\r\n";
			fileString += "    i+=1;\r\n";
			fileString += "    i-=1;\r\n";
			fileString += "    i*=1;\r\n";
			fileString += "    i/=1;\r\n";
			fileString += "    i%=1;\r\n";
			fileString += "    i&=1;\r\n";
			fileString += "    i|=1;\r\n";
			fileString += "    i^=1;\r\n";
			fileString += "    i<<=1;\r\n";
			fileString += "    i>>=1;\r\n";
			fileString += "    i=1&2;\r\n";
			fileString += "    i=1|2;\r\n";
			fileString += "    i=1^2;\r\n";
			fileString += "    i=1<<2;\r\n";
			fileString += "    i=1>>2;\r\n";
			fileString += "    i=((a->b)~c==d)?1:2;\r\n";
			fileString += "    int x, y;\r\n";
			fileString += "    switch (i){\r\n";
			fileString += "      case 3:\r\n";
			fileString += "    }\r\n";
			fileString += "    i = 1+2-3*4/5%2;\r\n";
			fileString += "    someBool = !((a < b) && (b > c) || (c <= d) && (d >= e) || (e == f) && (f != g));\r\n";
			fileString += "  }\r\n";
			fileString += "}\r\n";

			WriteTestFile(fileString, m_FileName);
			fs = File.OpenRead(m_FileName);
			Tokenizer t = new Tokenizer(fs, "", null);

			t.nextToken(); // class
			t.nextToken(); // Test
			t.nextToken(); // {
			AssertToken(t.CurrentToken, "{", 1, 11, 1, 11);
			t.nextToken(); // SomeObjectType
			t.nextToken(); // SomeObject
			t.nextToken(); // ;
			t.nextToken(); // int
			t.nextToken(); // i
			t.nextToken(); // [
			AssertToken(t.CurrentToken, "[", 3, 8, 3, 8);
			t.nextToken(); // 5
			t.nextToken(); // ]
			AssertToken(t.CurrentToken, "]", 3, 10, 3, 10);
			t.nextToken(); // ;
			t.nextToken(); // public
			t.nextToken(); // Test
			t.nextToken(); // {
			t.nextToken(); // }
			t.nextToken(); // {
			t.nextToken(); // i
			t.nextToken(); // =
			AssertToken(t.CurrentToken, "=", 5, 7, 5, 7);
			t.nextToken(); // SomeObject
			t.nextToken(); // .
			AssertToken(t.CurrentToken, ".", 5, 19, 5, 19);
			t.nextToken(); // Value
			t.nextToken(); // ;
			AssertToken(t.CurrentToken, ";", 5, 25, 5, 25);
			t.nextToken(); // i
			t.nextToken(); // ++
			AssertToken(t.CurrentToken, "++", 6, 6, 6, 7);
			t.nextToken(); // ;
			t.nextToken(); // i
			t.nextToken(); // --
			AssertToken(t.CurrentToken, "--", 7, 6, 7, 7);
			t.nextToken(); // ;
			t.nextToken(); // i
			t.nextToken(); // +=
			AssertToken(t.CurrentToken, "+=", 8, 6, 8, 7);
			t.nextToken(); // 1
			t.nextToken(); // ;
			t.nextToken(); // i
			t.nextToken(); // -=
			AssertToken(t.CurrentToken, "-=", 9, 6, 9, 7);
			t.nextToken(); // 1
			t.nextToken(); // ;
			t.nextToken(); // i
			t.nextToken(); // *=
			AssertToken(t.CurrentToken, "*=", 10, 6, 10, 7);
			t.nextToken(); // 1
			t.nextToken(); // ;
			t.nextToken(); // i
			t.nextToken(); // /=
			AssertToken(t.CurrentToken, "/=", 11, 6, 11, 7);
			t.nextToken(); // 1
			t.nextToken(); // ;
			t.nextToken(); // i
			t.nextToken(); // %=
			AssertToken(t.CurrentToken, "%=", 12, 6, 12, 7);
			t.nextToken(); // 1
			t.nextToken(); // ;
			t.nextToken(); // i
			t.nextToken(); // &=
			AssertToken(t.CurrentToken, "&=", 13, 6, 13, 7);
			t.nextToken(); // 1
			t.nextToken(); // ;
			t.nextToken(); // i
			t.nextToken(); // |=
			AssertToken(t.CurrentToken, "|=", 14, 6, 14, 7);
			t.nextToken(); // 1
			t.nextToken(); // ;
			t.nextToken(); // i
			t.nextToken(); // ^=
			AssertToken(t.CurrentToken, "^=", 15, 6, 15, 7);
			t.nextToken(); // 1
			t.nextToken(); // ;
			t.nextToken(); // i
			t.nextToken(); // <<=
			AssertToken(t.CurrentToken, "<<=", 16, 6, 16, 8);
			t.nextToken(); // 1
			t.nextToken(); // ;
			t.nextToken(); // i
			t.nextToken(); // >>=
			AssertToken(t.CurrentToken, ">>=", 17, 6, 17, 8);
			t.nextToken(); // 1
			t.nextToken(); // ;
			t.nextToken(); // i
			t.nextToken(); // =
			t.nextToken(); // 1
			t.nextToken(); // &
			AssertToken(t.CurrentToken, "&", 18, 8, 18, 8);
			t.nextToken(); // 2
			t.nextToken(); // ;
			t.nextToken(); // i
			t.nextToken(); // =
			t.nextToken(); // 1
			t.nextToken(); // |
			AssertToken(t.CurrentToken, "|", 19, 8, 19, 8);
			t.nextToken(); // 2
			t.nextToken(); // ;
			t.nextToken(); // i
			t.nextToken(); // =
			t.nextToken(); // 1
			t.nextToken(); // ^
			AssertToken(t.CurrentToken, "^", 20, 8, 20, 8);
			t.nextToken(); // 2
			t.nextToken(); // ;
			t.nextToken(); // i
			t.nextToken(); // =
			t.nextToken(); // 1
			t.nextToken(); // <<
			AssertToken(t.CurrentToken, "<<", 21, 8, 21, 9);
			t.nextToken(); // 2
			t.nextToken(); // ;
			t.nextToken(); // i
			t.nextToken(); // =
			t.nextToken(); // 1
			t.nextToken(); // >>
			AssertToken(t.CurrentToken, ">>", 22, 8, 22, 9);
			t.nextToken(); // 2
			t.nextToken(); // ;
			t.nextToken(); // i
			t.nextToken(); // =
			t.nextToken(); // (
			t.nextToken(); // (
			t.nextToken(); // a
			t.nextToken(); // ->
			AssertToken(t.CurrentToken, "->", 23, 10, 23, 11);
			t.nextToken(); // b
			t.nextToken(); // )
			t.nextToken(); // ~
			AssertToken(t.CurrentToken, "~", 23, 14, 23, 14);
			t.nextToken(); // c
			t.nextToken(); // ==
			t.nextToken(); // d
			t.nextToken(); // )
			t.nextToken(); // ?
			AssertToken(t.CurrentToken, "?", 23, 20, 23, 20);
			t.nextToken(); // 1
			t.nextToken(); // :
			t.nextToken(); // 2
			t.nextToken(); // ;
			t.nextToken(); // int
			t.nextToken(); // x
			t.nextToken(); // ,
			AssertToken(t.CurrentToken, ",", 24, 10, 24, 10);
			t.nextToken(); // y
			t.nextToken(); // ;
			t.nextToken(); // switch
			t.nextToken(); // (
			t.nextToken(); // i
			t.nextToken(); // )
			t.nextToken(); // {
			t.nextToken(); // case
			t.nextToken(); // 3
			t.nextToken(); // :
			AssertToken(t.CurrentToken, ":", 26, 13, 26, 13);
			t.nextToken(); // }
			t.nextToken(); // i
			t.nextToken(); // =
			t.nextToken(); // 1
			t.nextToken(); // +
			AssertToken(t.CurrentToken, "+", 28, 10, 28, 10);
			t.nextToken(); // 2
			t.nextToken(); // -
			AssertToken(t.CurrentToken, "-", 28, 12, 28, 12);
			t.nextToken(); // 3
			t.nextToken(); // *
			AssertToken(t.CurrentToken, "*", 28, 14, 28, 14);
			t.nextToken(); // 4
			t.nextToken(); // /
			AssertToken(t.CurrentToken, "/", 28, 16, 28, 16);
			t.nextToken(); // 5
			t.nextToken(); // %
			AssertToken(t.CurrentToken, "%", 28, 18, 28, 18);
			t.nextToken(); // 2
			t.nextToken(); // ;
			t.nextToken(); // someBool
			t.nextToken(); // =
			t.nextToken(); // !
			AssertToken(t.CurrentToken, "!", 29, 16, 29, 16);
			t.nextToken(); // (
			t.nextToken(); // (
			t.nextToken(); // a
			t.nextToken(); // <
			AssertToken(t.CurrentToken, "<", 29, 21, 29, 21);
			t.nextToken(); // b
			t.nextToken(); // )
			t.nextToken(); // &&
			AssertToken(t.CurrentToken, "&&", 29, 26, 29, 27);
			t.nextToken(); // (
			t.nextToken(); // b
			t.nextToken(); // >
			AssertToken(t.CurrentToken, ">", 29, 32, 29, 32);
			t.nextToken(); // c
			t.nextToken(); // )
			t.nextToken(); // ||
			AssertToken(t.CurrentToken, "||", 29, 37, 29, 38);
			t.nextToken(); // (
			t.nextToken(); // c
			t.nextToken(); // <=
			AssertToken(t.CurrentToken, "<=", 29, 43, 29, 44);
			t.nextToken(); // d
			t.nextToken(); // )
			t.nextToken(); // &&
			t.nextToken(); // (
			t.nextToken(); // d
			t.nextToken(); // >=
			AssertToken(t.CurrentToken, ">=", 29, 55, 29, 56);
			t.nextToken(); // e
			t.nextToken(); // )
			t.nextToken(); // ||
			t.nextToken(); // (
			t.nextToken(); // e
			t.nextToken(); // ==
			AssertToken(t.CurrentToken, "==", 29, 67, 29, 68);
			t.nextToken(); // f
			t.nextToken(); // )
			t.nextToken(); // &&
			t.nextToken(); // (
			t.nextToken(); // f
			t.nextToken(); // !=
			AssertToken(t.CurrentToken, "!=", 29, 79, 29, 80);
			t.nextToken(); // g
			t.nextToken(); // )
			t.nextToken(); // )
			t.nextToken(); // ;
			t.nextToken(); // }
			AssertToken(t.CurrentToken, "}", 30, 3, 30, 3);
			t.nextToken(); // }
			AssertToken(t.CurrentToken, "}", 31, 1, 31, 1);
		}

		[Test]
		public void testNumberInt() 
		{
			//TODO:  Add more tests for signed/unsigned and other
			//       types of integer declarations.
			string fileString = "";
			fileString += "class Test{\r\n";
			fileString += "  int i=345;\r\n";
			fileString += "}\r\n";

			WriteTestFile(fileString, m_FileName);
			fs = File.OpenRead(m_FileName);
			Tokenizer t = new Tokenizer(fs, "", null);

			t.nextToken(); // class
			AssertToken(t.CurrentToken, "class", 1, 1, 1, 5);
			t.nextToken(); // Test
			AssertToken(t.CurrentToken, "Test", 1, 7, 1, 10);
			t.nextToken(); //{
			AssertToken(t.CurrentToken, "{", 1, 11, 1, 11);
			t.nextToken(); //int
			AssertToken(t.CurrentToken, "int", 2, 3, 2, 5);
			t.nextToken(); //i
			AssertToken(t.CurrentToken, "i", 2, 7, 2, 7);
			t.nextToken(); //=
			AssertToken(t.CurrentToken, "=", 2, 8, 2, 8);
			t.nextToken(); //345
			AssertToken(t.CurrentToken, "345", 2, 9, 2, 11);
		}

		[Test]
		public void testNumberInt2() 
		{
			string fileString = "";
			fileString += "int 345";

			WriteTestFile(fileString, m_FileName);
			fs = File.OpenRead(m_FileName);
			Tokenizer t = new Tokenizer(fs, "", null);

			t.nextToken(); //int
			t.nextToken(); //345
			AssertToken(t.CurrentToken, "345", 1, 5, 1, 7);
		}

		[Test]
		public void testNumberInt3() 
		{
			string fileString = "";
			fileString += "int 345;";

			WriteTestFile(fileString, m_FileName);
			fs = File.OpenRead(m_FileName);
			Tokenizer t = new Tokenizer(fs, "", null);

			t.nextToken(); //int
			t.nextToken(); //345
			AssertToken(t.CurrentToken, "345", 1, 5, 1, 7);
			t.nextToken(); //;
			AssertToken(t.CurrentToken, ";", 1, 8, 1, 8);
		}

		[Test]
		public void testTwoNumberInt() 
		{
			string fileString = "";
			fileString += "345 897";

			WriteTestFile(fileString, m_FileName);
			fs = File.OpenRead(m_FileName);
			Tokenizer t = new Tokenizer(fs, "", null);

			t.nextToken(); //345
			AssertToken(t.CurrentToken, "345", 1, 1, 1, 3);
			t.nextToken(); //345
			AssertToken(t.CurrentToken, "897", 1, 5, 1, 7);
		}

		[Test]
		public void testNumberFloat1()
		{
			string fileString = "int 123.33";

			Tokenizer t = TokenizeTestFile(fileString);

			t.nextToken(); // int
			AssertToken(t.CurrentToken, "int", 1, 1, 1, 3);
			t.nextToken(); // 123.33
			AssertToken(t.CurrentToken, "123.33", 1, 5, 1, 10);
		}

		[Test]
		public void testNumberFloat2()
		{
			string fileString = "123.33f 432.88F";

			Tokenizer t = TokenizeTestFile(fileString);

			t.nextToken(); // 123.33f
			AssertToken(t.CurrentToken, "123.33", 1, 1, 1, 7);
			t.nextToken(); // 432.88F
			AssertToken(t.CurrentToken, "432.88", 1, 9, 1, 15);
		}

		[Test]
		public void testNumberFloat3()
		{
			string fileString = "2E3f 4e5f";

			Tokenizer t = TokenizeTestFile(fileString);

			t.nextToken(); // 2E3f
			AssertToken(t.CurrentToken, "2000", 1, 1, 1, 4);
			t.nextToken(); // 4e5f
			AssertToken(t.CurrentToken, "400000", 1, 6, 1, 9);
		}

		[Test]
		public void testDontEatUpFollowingToken1()
		{
			string fileString = "int a = 4e5f;";

			Tokenizer t = TokenizeTestFile(fileString);

			t.nextToken(); // int
			t.nextToken(); // a
			t.nextToken(); // =
			t.nextToken(); // 4e5f
			AssertToken(t.CurrentToken, "400000", 1, 9, 1, 12);
			t.nextToken(); // ;
			AssertToken(t.CurrentToken, ";", 1, 13, 1, 13);
		}

		[Test]
		public void testDontEatUpFollowingToken2()
		{
			string fileString = "int a = 1.0;";

			Tokenizer t = TokenizeTestFile(fileString);

			t.nextToken(); // int
			t.nextToken(); // a
			t.nextToken(); // =
			t.nextToken(); // 1.0
			AssertToken(t.CurrentToken, "1", 1, 9, 1, 11);
			t.nextToken(); // ;
			AssertToken(t.CurrentToken, ";", 1, 12, 1, 12);
		}

		[Test]
		public void testDontEatUpFollowingToken3()
		{
			string fileString = "int a = 1e3d;";

			Tokenizer t = TokenizeTestFile(fileString);

			t.nextToken(); // int
			t.nextToken(); // a
			t.nextToken(); // =
			t.nextToken(); // 1e3d
			AssertToken(t.CurrentToken, "1000", 1, 9, 1, 12);
			t.nextToken(); // ;
			AssertToken(t.CurrentToken, ";", 1, 13, 1, 13);
		}

		[Test]
		public void testNumberFloat4() 
		{
			string fileString = "";
			fileString += "class Test{\r\n";
			fileString += "  float i=3E5F;\r\n";
			fileString += "  float i=3e5F;\r\n";
			fileString += "  float i=3.5F;\r\n";
			fileString += "  float i=345.45f;\r\n";
			fileString += "  float i=345f;\r\n";
			fileString += "  decimal i=35M;\r\n";
			fileString += "  decimal i=3.5M;\r\n";
			fileString += "  decimal i=345.45m;\r\n";
			fileString += "  decimal i=345m;\r\n";
			fileString += "  double i=34E5D;\r\n";
			fileString += "  double i=34e5D;\r\n";
			fileString += "  double i=345.45D;\r\n";
			fileString += "  double i=345.45d;\r\n";
			fileString += "  double i=345d;\r\n";
			fileString += "}\r\n";

			WriteTestFile(fileString, m_FileName);
			fs = File.OpenRead(m_FileName);
			Tokenizer t = new Tokenizer(fs, "", null);

			t.nextToken(); // class
			t.nextToken(); // Test
			t.nextToken(); //{
			t.nextToken(); //float
			t.nextToken(); //i
			t.nextToken(); //=
			t.nextToken(); //3E5F
			AssertToken(t.CurrentToken, "300000", 2, 11, 2, 14);
			t.nextToken(); //;
			t.nextToken(); //float
			t.nextToken(); //i
			t.nextToken(); //=
			t.nextToken(); //3e5F
			AssertToken(t.CurrentToken, "300000", 3, 11, 3, 14);
			t.nextToken(); //;
			t.nextToken(); //float
			t.nextToken(); //i
			t.nextToken(); //=
			t.nextToken(); //3.5F
			AssertToken(t.CurrentToken, "3.5", 4, 11, 4, 14);
			t.nextToken(); //;
			t.nextToken(); //float
			t.nextToken(); //i
			t.nextToken(); //=
			t.nextToken(); //345.45f
			AssertToken(t.CurrentToken, "345.45", 5, 11, 5, 17);
			t.nextToken(); //;
			t.nextToken(); //float
			t.nextToken(); //i
			t.nextToken(); //=
			t.nextToken(); //345f
			AssertToken(t.CurrentToken, "345", 6, 11, 6, 14);
			t.nextToken(); //;
			t.nextToken(); //decimal
			t.nextToken(); //i
			t.nextToken(); //=
			t.nextToken(); //35M
			AssertToken(t.CurrentToken, "35", 7, 13, 7, 15);
			t.nextToken(); //;
			t.nextToken(); //decimal
			t.nextToken(); //i
			t.nextToken(); //=
			t.nextToken(); //3.5M
			AssertToken(t.CurrentToken, "3.5", 8, 13, 8, 16);
			t.nextToken(); //;
			t.nextToken(); //decimal
			t.nextToken(); //i
			t.nextToken(); //=
			t.nextToken(); //345.45m
			AssertToken(t.CurrentToken, "345.45", 9, 13, 9, 19);
			t.nextToken(); //;
			t.nextToken(); //decimal
			t.nextToken(); //i
			t.nextToken(); //=
			t.nextToken(); //345m
			AssertToken(t.CurrentToken, "345", 10, 13, 10, 16);
			t.nextToken(); //;
			t.nextToken(); //double
			t.nextToken(); //i
			t.nextToken(); //=
			t.nextToken(); //34E5D
			AssertToken(t.CurrentToken, "3400000", 11, 12, 11, 16);
			t.nextToken(); //;
			t.nextToken(); //double
			t.nextToken(); //i
			t.nextToken(); //=
			t.nextToken(); //34e5D
			AssertToken(t.CurrentToken, "3400000", 12, 12, 12, 16);
			t.nextToken(); //;
			t.nextToken(); //double
			t.nextToken(); //i
			t.nextToken(); //=
			t.nextToken(); //345.45D
			AssertToken(t.CurrentToken, "345.45", 13, 12, 13, 18);
			t.nextToken(); //;
			t.nextToken(); //double
			t.nextToken(); //i
			t.nextToken(); //=
			t.nextToken(); //345.45d
			AssertToken(t.CurrentToken, "345.45", 14, 12, 14, 18);
			t.nextToken(); //;
			t.nextToken(); //double
			t.nextToken(); //i
			t.nextToken(); //=
			t.nextToken(); //345d
			AssertToken(t.CurrentToken, "345", 15, 12, 15, 15);
		}

		[Test]
		public void testNumberHex() 
		{
			string fileString = "";
			fileString += "class Test{\r\n";
			fileString += "  int i=0x345;\r\n";
			fileString += "}\r\n";

			WriteTestFile(fileString, m_FileName);
			fs = File.OpenRead(m_FileName);
			Tokenizer t = new Tokenizer(fs, "", null);

			t.nextToken(); //class
			AssertToken(t.CurrentToken, "class", 1, 1, 1, 5);
			t.nextToken(); //Test
			AssertToken(t.CurrentToken, "Test", 1, 7, 1, 10);
			t.nextToken(); //{
			AssertToken(t.CurrentToken, "{", 1, 11, 1, 11);
			t.nextToken(); //int
			AssertToken(t.CurrentToken, "int", 2, 3, 2, 5);
			t.nextToken(); //i
			AssertToken(t.CurrentToken, "i", 2, 7, 2, 7);
			t.nextToken(); //=
			AssertToken(t.CurrentToken, "=", 2, 8, 2, 8);
			t.nextToken(); //0x345
			AssertToken(t.CurrentToken, "837", 2, 9, 2, 14);
		}

		[Test]
		public void testSmallBit1()
		{
			string fileString = "++a";
			WriteTestFile(fileString, m_FileName);
			fs = File.OpenRead(m_FileName);
			Tokenizer t = new Tokenizer(fs, "", null);
			t.nextToken(); // ++
			AssertToken(t.CurrentToken, "++", 1, 1, 1, 2);
			t.nextToken(); // a
			AssertToken(t.CurrentToken, "a", 1, 3, 1, 3);
		}

		[Test]
		public void testString1()
		{
			string fileString = "\"some string\"";
			WriteTestFile(fileString, m_FileName);
			fs = File.OpenRead(m_FileName);
			Tokenizer t = new Tokenizer(fs, "", null);
			t.nextToken(); // "some string"
			AssertToken(t.CurrentToken, "\"some string\"", 1, 1, 1, 13);
		}
		
		[Test]
		public void testString2()
		{
			string fileString = "1 , \"some string\"";
			WriteTestFile(fileString, m_FileName);
			fs = File.OpenRead(m_FileName);
			Tokenizer t = new Tokenizer(fs, "", null);
			t.nextToken(); // 1
			t.nextToken(); // ,
			t.nextToken(); // "some string"
			AssertToken(t.CurrentToken, "\"some string\"", 1, 5, 1, 17);
		}

		[Test]
		public void testString3()
		{
			string fileString = "int \"some string\"";
			WriteTestFile(fileString, m_FileName);
			fs = File.OpenRead(m_FileName);
			Tokenizer t = new Tokenizer(fs, "", null);
			t.nextToken(); // int
			t.nextToken(); // "some string"
			AssertToken(t.CurrentToken, "\"some string\"", 1, 5, 1, 17);
		}

		[Test]
		public void testString4()
		{
			string fileString = "    \"some string\"";
			WriteTestFile(fileString, m_FileName);
			fs = File.OpenRead(m_FileName);
			Tokenizer t = new Tokenizer(fs, "", null);
			t.nextToken(); // "some string"
			AssertToken(t.CurrentToken, "\"some string\"", 1, 5, 1, 17);
		}

		private void AssertToken(PositionToken pt, string val, int startLine, int startCol, int endLine, int endCol)
		{
			Assertion.AssertEquals("Token text is wrong!", val, pt.Text);
			Assertion.AssertEquals("StartLine is wrong!", startLine, pt.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", startCol, pt.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", endLine, pt.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", endCol, pt.Position.EndCol);
		}
	}
}
