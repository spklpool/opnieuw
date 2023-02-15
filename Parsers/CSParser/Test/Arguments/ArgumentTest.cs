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
	public class ArgumentTest : TokenizerTestBase
	{
		[Test]
		public void testSimplest()
		{
			//Preparation
			string fileString = "b";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Argument arg = Argument.parse(t);

			//General tests
			Assert("Argument is invalid!", false == (arg is MissingArgument));

			//Variable and expression tests
			AssertEquals("Number of children is wrong!", 1, arg.Children.Count);
			AssertEquals("Number of Expressions is wrong!", 1, arg.Expressions.Count);
			AssertEquals("Number of declared variables is wrong!", 0, arg.DeclaredVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 0, arg.ReferencedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 0, arg.ModifiedVariables.Count);

			//Position tests
			AssertEquals("StartLine is wrong!", 1, arg.Position.StartLine);
			AssertEquals("StartCol is wrong!", 1, arg.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, arg.Position.EndLine);
			AssertEquals("EndCol is wrong!", 1, arg.Position.EndCol);
		}

		[Test]
		public void testSimpleExpression()
		{
			//Preparation
			string fileString = "{int a, b; SomeMethod(/*A Comment.*/1 + (a * b));}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			BlockStatement block = BlockStatement.parse(t) as BlockStatement;
			ExpressionStatement expressionStatement = block.Statements[1] as ExpressionStatement;
			InvocationExpression invocation = expressionStatement.Expression as InvocationExpression;
			Argument arg = invocation.Arguments[0];

			//General tests
			Assertion.Assert("Argument is invalid!", false == (arg is MissingArgument));

			//Variable and expression tests
			AssertEquals("Number of children is wrong!", 1, arg.Children.Count);
			AssertEquals("Number of Expressions is wrong!", 6, arg.Expressions.Count);
			AssertEquals("Number of declared variables is wrong!", 0, arg.DeclaredVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 2, arg.DeepReferencedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 0, arg.ModifiedVariables.Count);

			//Generate test
			AssertEquals("Pretty print string was wrong!", fileString, block.Generate());

			//Position tests
			AssertEquals("StartLine is wrong!", 1, arg.Position.StartLine);
			AssertEquals("StartCol is wrong!", 37, arg.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, arg.Position.EndLine);
			AssertEquals("EndCol is wrong!", 47, arg.Position.EndCol);
		}

		[Test]
		public void testRefObject()
		{
			//Preparation
			string fileString = "{SomeClass someObject = new SomeClass(); SomeMethod(ref someObject);}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			BlockStatement block = BlockStatement.parse(t) as BlockStatement;
			ExpressionStatement expressionStatement = block.Statements[1] as ExpressionStatement;
			InvocationExpression invocation = expressionStatement.Expression as InvocationExpression;
			Argument arg = invocation.Arguments[0];

			//General tests
			Assert("Argument is invalid!", false == (arg is MissingArgument));
			AssertEquals("Expect ref but was not!", true, arg.IsRef);

			//Variable and expression tests
			AssertEquals("Number of children is wrong!", 1, arg.Children.Count);
			AssertEquals("Number of Expressions is wrong!", 1, arg.Expressions.Count);
			AssertEquals("Number of declared variables is wrong!", 0, arg.DeclaredVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 1, arg.DeepReferencedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 1, arg.ModifiedVariables.Count);

			//Generate test
			AssertEquals("Pretty print string was wrong!", fileString, block.Generate());

			//Position tests
			AssertEquals("StartLine is wrong!", 1, arg.Position.StartLine);
			AssertEquals("StartCol is wrong!", 53, arg.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, arg.Position.EndLine);
			AssertEquals("EndCol is wrong!", 66, arg.Position.EndCol);
		}

		[Test]
		public void testOutObject()
		{
			//Preparation
			string fileString = "{SomeClass someObject = new SomeClass(); SomeMethod(out someObject);}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			BlockStatement block = BlockStatement.parse(t) as BlockStatement;
			ExpressionStatement expressionStatement = block.Statements[1] as ExpressionStatement;
			InvocationExpression invocation = expressionStatement.Expression as InvocationExpression;
			Argument arg = invocation.Arguments[0];

			//General tests
			Assert("Argument is invalid!", false == (arg is MissingArgument));
			AssertEquals("Expect out but was not!", true, arg.IsOut);

			//Variable and expression tests
			AssertEquals("Number of children is wrong!", 1, arg.Children.Count);
			AssertEquals("Number of Expressions is wrong!", 1, arg.Expressions.Count);
			AssertEquals("Number of declared variables is wrong!", 0, arg.DeclaredVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 1, arg.DeepReferencedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 1, arg.ModifiedVariables.Count);

			//Generate test
			AssertEquals("Pretty print string was wrong!", fileString, block.Generate());

			//Position tests
			AssertEquals("StartLine is wrong!", 1, arg.Position.StartLine);
			AssertEquals("StartCol is wrong!", 53, arg.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, arg.Position.EndLine);
			AssertEquals("EndCol is wrong!", 66, arg.Position.EndCol);
		}

		[Test]
		public void testWithComment()
		{
			//Preparation
			string fileString = "";
			fileString += "{\r\n";
			fileString += "  SomeClass someObject = new SomeClass();\r\n";
			fileString += "  SomeMethod(\r\n";
			fileString += "  //ref comment\r\n";
			fileString += "  ref someObject);\r\n";
			fileString += "}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			BlockStatement block = BlockStatement.parse(t) as BlockStatement;
			ExpressionStatement expressionStatement = block.Statements[1] as ExpressionStatement;
			InvocationExpression invocation = expressionStatement.Expression as InvocationExpression;
			Argument arg = invocation.Arguments[0];

			//Generate test
			AssertEquals("Pretty print string was wrong!", fileString, block.Generate());
		}
	}
}
