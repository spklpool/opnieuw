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
using System.IO;
using NUnit.Framework;
using Opnieuw.Framework;

namespace Opnieuw.Parsers.CSParser.Test
{
	[TestFixture]
	public class LiteralTest : TokenizerTestBase
	{
		[Test]
		public void testBoolLiteralTrue()
		{
			//Preparation
			string fileString = "true";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assert("Expected BoolLiteral, but got something else!", exp is BoolLiteral);
			BoolLiteral boolLiteralExpression = exp as BoolLiteral;
			AssertEquals("Value is wrong!", true, boolLiteralExpression.Value);
			AssertEquals("Pretty Print string is wrong!", "true", boolLiteralExpression.Generate());

			//Position tests
			AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			AssertEquals("StartCol is wrong!", 1, exp.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			AssertEquals("EndCol is wrong!", 4, exp.Position.EndCol);
		}

		[Test]
		public void testBoolLiteralFalse()
		{
			//Preparation
			string fileString = "false";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Expected BoolLiteral, but got something else!", exp is BoolLiteral);
			BoolLiteral boolLiteralExpression = exp as BoolLiteral;
			Assertion.AssertEquals("Value is wrong!", false, boolLiteralExpression.Value);
			AssertEquals("Pretty Print string is wrong!", "false", boolLiteralExpression.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 5, exp.Position.EndCol);
		}

		[Test]
		public void testCharLiteral1()
		{
			//Preparation
			string fileString = "'a'";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Expected CharLiteral, but got something else!", exp is CharLiteral);
			CharLiteral charLiteralExpression = exp as CharLiteral;
			Assertion.AssertEquals("Value is wrong!", 'a', charLiteralExpression.Value);
			AssertEquals("Pretty Print string is wrong!", "'a'", charLiteralExpression.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 3, exp.Position.EndCol);
		}

		[Test]
		public void testCharLiteral2()
		{
			//Preparation
			string fileString = "'b'";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Expected CharLiteral, but got something else!", exp is CharLiteral);
			CharLiteral charLiteralExpression = exp as CharLiteral;
			Assertion.AssertEquals("Value is wrong!", 'b', charLiteralExpression.Value);
			AssertEquals("Pretty Print string is wrong!", "'b'", charLiteralExpression.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 3, exp.Position.EndCol);
		}

		[Test]
		public void testIntegerLiteral1()
		{
			//Preparation
			string fileString = "123";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Expected IntegerLiteral, but got something else!", exp is IntegerLiteral);
			IntegerLiteral integerLiteral = exp as IntegerLiteral;
			Assertion.AssertEquals("Value is wrong!", 123, integerLiteral.Value);
			AssertEquals("Pretty Print string is wrong!", "123", integerLiteral.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 3, exp.Position.EndCol);
		}

		[Test]
		public void testRealLiteral1()
		{
			//Preparation
			string fileString = "int 123.33";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Expected RealLiteral, but got something else!", exp is RealLiteral);
			RealLiteral realLiteral = exp as RealLiteral;
			Assertion.AssertEquals("Value is wrong!", 123.33, realLiteral.Value);
			AssertEquals("Pretty Print string is wrong!", " 123.33", realLiteral.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 5, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 10, exp.Position.EndCol);
		}

		[Test]
		public void testStringLiteral()
		{
			//Preparation
			string fileString = " \"Some string\"";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Expected StringLiteral, but got something else!", exp is StringLiteral);
			StringLiteral stringLiteral = exp as StringLiteral;
			Assertion.AssertEquals("Value is wrong!", "Some string", stringLiteral.Value);
			AssertEquals("Pretty Print string is wrong!", " \"Some string\"", stringLiteral.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 2, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 14, exp.Position.EndCol);
		}

		[Test]
		public void testNullLiteral()
		{
			//Preparation
			string fileString = "null";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Expected NullLiteral, but got something else!", exp is NullLiteral);
			NullLiteral nullLiteral = exp as NullLiteral;
			AssertEquals("Pretty Print string is wrong!", "null", nullLiteral.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 4, exp.Position.EndCol);
		}
	}
}
