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
	public class ExpressionTest : TokenizerTestBase
	{
		[Test]
		public void testParenthesizedExpression1()
		{
			//Preparation
			string fileString = "{int a, b; b = (a);}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			BlockStatement block = BlockStatement.parse(t) as BlockStatement;

			//General tests
			ExpressionStatement statement = block.Statements[1] as ExpressionStatement;
			AssignmentExpression statementExpression = statement.Expression as AssignmentExpression;
			Expression exp = statementExpression.Expression2;
			Assertion.Assert("Expected ParenthesizedExpression, but got something else!", exp is ParenthesizedExpression);
			ParenthesizedExpression theExp = exp as ParenthesizedExpression;
			Assertion.Assert("Expression is the wrong type!", theExp.InnerExpression is Identifier);

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 1, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 1, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 16, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 18, exp.Position.EndCol);
		}

		[Test]
		public void testParenthesizedExpression2()
		{
			//Preparation
			string fileString = "{int a, b, c, d, e; e = (a+b || c-d);}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			BlockStatement block = BlockStatement.parse(t) as BlockStatement;

			//General tests
			ExpressionStatement statement = block.Statements[1] as ExpressionStatement;
			AssignmentExpression statementExpression = statement.Expression as AssignmentExpression;
			Expression exp = statementExpression.Expression2;
			Assert("Expected ParenthesizedExpression, but got something else!", exp is ParenthesizedExpression);
			ParenthesizedExpression theExp = exp as ParenthesizedExpression;
			Assert("Exprected ConditionalOrExpression, but got something else.", theExp.InnerExpression is ConditionalOrExpression);
			ConditionalOrExpression innerExp = theExp.InnerExpression as ConditionalOrExpression;
            Assert("Exprected AdditiveExpression, but got something else.", innerExp.Expression1 is AdditiveExpression);
			Assert("Exprected AdditiveExpression, but got something else.", innerExp.Expression2 is AdditiveExpression);

			//Variable and expression tests
			AssertEquals("Number of Expressions in statement is wrong!", 15, block.Expressions.Count);
			AssertEquals("Number of declared variables is wrong!", 5, block.DeclaredVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 0, block.ReferencedVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 5, block.DeepReferencedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 0, block.ModifiedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 1, block.DeepModifiedVariables.Count);
			AssertEquals("Number of Expressions in statement is wrong!", 7, exp.Expressions.Count);
			AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 4, exp.DeepReferencedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			AssertEquals("StartCol is wrong!", 25, exp.Position.StartCol);
			AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			AssertEquals("EndCol is wrong!", 36, exp.Position.EndCol);
		}

		[Test]
		public void testNegationExpression()
		{
			//Preparation
			string fileString = "{int a, b; b = !a;}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			BlockStatement block = BlockStatement.parse(t) as BlockStatement;

			//General tests
			ExpressionStatement statement = block.Statements[1] as ExpressionStatement;
			AssignmentExpression statementExpression = statement.Expression as AssignmentExpression;
			Expression exp = statementExpression.Expression2;
			Assertion.Assert("Expected NegationExpression, but got something else!", exp is NegationExpression);
			NegationExpression theExp = exp as NegationExpression;

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 6, block.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 2, block.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 0, block.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 2, block.DeepReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, block.ModifiedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 1, block.DeepModifiedVariables.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 1, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 1, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 16, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 17, exp.Position.EndCol);
		}

		[Test]
		public void testAdditionExpression()
		{
			//Preparation
			string fileString = "{int a, b; b = +a;}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			BlockStatement block = BlockStatement.parse(t) as BlockStatement;

			//General tests
			ExpressionStatement statement = block.Statements[1] as ExpressionStatement;
			AssignmentExpression statementExpression = statement.Expression as AssignmentExpression;
			Expression exp = statementExpression.Expression2;
			Assertion.Assert("Expected AdditionExpression, but got something else!", exp is AdditionExpression);

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 6, block.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 2, block.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 0, block.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 2, block.DeepReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, block.ModifiedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 1, block.DeepModifiedVariables.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 1, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 1, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 16, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 17, exp.Position.EndCol);
		}

		[Test]
		public void testAdditionExpressionWithSpace()
		{
			//Preparation
			string fileString = "{int a, b; b = + a;}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			BlockStatement block = BlockStatement.parse(t) as BlockStatement;

			//General tests
			ExpressionStatement statement = block.Statements[1] as ExpressionStatement;
			AssignmentExpression statementExpression = statement.Expression as AssignmentExpression;
			Expression exp = statementExpression.Expression2;
			Assertion.Assert("Expected AdditionExpression, but got something else!", exp is AdditionExpression);

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 6, block.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 2, block.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 0, block.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 2, block.DeepReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, block.ModifiedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 1, block.DeepModifiedVariables.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 1, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 1, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 16, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 18, exp.Position.EndCol);
		}

		[Test]
		public void testSubtractionExpression()
		{
			//Preparation
			string fileString = "{int a, b; b = -a;}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			BlockStatement block = BlockStatement.parse(t) as BlockStatement;

			//General tests
			ExpressionStatement statement = block.Statements[1] as ExpressionStatement;
			AssignmentExpression statementExpression = statement.Expression as AssignmentExpression;
			Expression exp = statementExpression.Expression2;
			Assertion.Assert("Expected SubtractionExpression, but got something else!", exp is SubtractionExpression);

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 6, block.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 2, block.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 0, block.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 2, block.DeepReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, block.ModifiedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 1, block.DeepModifiedVariables.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 1, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 1, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 16, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 17, exp.Position.EndCol);
		}

		[Test]
		public void testPreIncrementExpression()
		{
			//Preparation
			string fileString = "{int a; ++a;}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			BlockStatement block = BlockStatement.parse(t) as BlockStatement;

			//General tests
			ExpressionStatement statement = block.Statements[1] as ExpressionStatement;
			Expression exp = statement.Expression;
			Assertion.Assert("Expected PreIncrementExpression, but got something else!", exp is PreIncrementExpression);

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 3, block.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 1, block.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 0, block.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 1, block.DeepReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, block.ModifiedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 1, block.DeepModifiedVariables.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 1, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 1, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 1, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 9, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 11, exp.Position.EndCol);
		}

		[Test]
		public void testPreDecrementExpression()
		{
			//Preparation
			string fileString = "{int a; --a;}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			BlockStatement block = BlockStatement.parse(t) as BlockStatement;

			//General tests
			ExpressionStatement statement = block.Statements[1] as ExpressionStatement;
			Expression exp = statement.Expression;
			Assertion.Assert("Expected PreDecrementExpression, but got something else!", exp is PreDecrementExpression);

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 3, block.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 1, block.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 0, block.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 1, block.DeepReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, block.ModifiedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 1, block.DeepModifiedVariables.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 1, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 1, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 1, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 9, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 11, exp.Position.EndCol);
		}

		[Test]
		public void testCastExpression1()
		{
			//Preparation
			string fileString = "{int a, b, c; c = (a)b;}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			BlockStatement block = BlockStatement.parse(t) as BlockStatement;

			//General tests
			ExpressionStatement statement = block.Statements[1] as ExpressionStatement;
			AssignmentExpression statementExpression = statement.Expression as AssignmentExpression;
			Expression exp = statementExpression.Expression2;
			Assertion.Assert("Expected CastExpression, but got something else!", exp is CastExpression);
			CastExpression theExp = exp as CastExpression;

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 1, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 1, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 19, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 22, exp.Position.EndCol);
		}

		[Test]
		public void testBaseAccessExpression1()
		{
			//Preparation
			string fileString = "{int a, b; b = base.a;}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			BlockStatement block = BlockStatement.parse(t) as BlockStatement;

			//General tests
			ExpressionStatement statement = block.Statements[1] as ExpressionStatement;
			AssignmentExpression statementExpression = statement.Expression as AssignmentExpression;
			Expression exp = statementExpression.Expression2;
			Assertion.Assert("Expected BaseAccessExpression, but got something else!", exp is BaseAccessExpression);
			BaseAccessExpression baseAccessExpression = exp as BaseAccessExpression;
			Assertion.AssertEquals("Member name is wrong!", "a", baseAccessExpression.Identifier.Name);
			Assertion.AssertEquals("Generate string is wrong!", fileString, block.Generate());

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 0, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 0, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 16, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 21, exp.Position.EndCol);
		}

		[Test]
		public void testBaseAccessExpression2()
		{
			//Preparation
			string fileString = "{int a, b; b = base[a];}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			BlockStatement block = BlockStatement.parse(t) as BlockStatement;

			//General tests
			ExpressionStatement statement = block.Statements[1] as ExpressionStatement;
			AssignmentExpression statementExpression = statement.Expression as AssignmentExpression;
			Expression exp = statementExpression.Expression2;
			Assertion.Assert("Expected BaseAccessExpression, but got something else!", exp is BaseAccessExpression);
			BaseAccessExpression theExp = exp as BaseAccessExpression;
			Assertion.AssertEquals("Expression count is wrong!", 1, theExp.ExpressionList.Count);
			Assertion.AssertEquals("Generate string is wrong!", fileString, block.Generate());

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 1, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 0, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 1, exp.DeepReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 16, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 22, exp.Position.EndCol);
		}

		[Test]
		public void testBaseAccessExpression3()
		{
			//Preparation
			string fileString = "{int a, b, c, d; d = base[a, b, c];}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			BlockStatement block = BlockStatement.parse(t) as BlockStatement;

			//General tests
			ExpressionStatement statement = block.Statements[1] as ExpressionStatement;
			AssignmentExpression statementExpression = statement.Expression as AssignmentExpression;
			Expression exp = statementExpression.Expression2;
			Assertion.Assert("Expected BaseAccessExpression, but got something else!", exp is BaseAccessExpression);
			BaseAccessExpression theExp = exp as BaseAccessExpression;
			Assertion.AssertEquals("Expression count is wrong!", 3, theExp.ExpressionList.Count);
			
			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 3, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 0, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 3, exp.DeepReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 22, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 34, exp.Position.EndCol);
		}

		[Test]
		public void testBaseAccessExpression4()
		{
			//Preparation
			string fileString = "{int a, b; b = base.not_a;}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			BlockStatement block = BlockStatement.parse(t) as BlockStatement;

			//General tests
			ExpressionStatement statement = block.Statements[1] as ExpressionStatement;
			AssignmentExpression statementExpression = statement.Expression as AssignmentExpression;
			Expression exp = statementExpression.Expression2;
			Assertion.Assert("Expected BaseAccessExpression, but got something else!", exp is BaseAccessExpression);
			BaseAccessExpression baseAccessExpression = exp as BaseAccessExpression;
			Assertion.AssertEquals("Member name is wrong!", "not_a", baseAccessExpression.Identifier.Name);

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 0, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 0, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 16, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 25, exp.Position.EndCol);
		}

		[Test]
		public void testThisAccessExpression()
		{
			//Preparation
			string fileString = "this;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Expected ThisAccessExpression, but got something else!", exp is ThisAccessExpression);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 4, exp.Position.EndCol);
		}

		[Test]
		public void testCheckedExpression1()
		{
			//Preparation
			string fileString = "{SomeClass someObject, someOtherObject; someObject = checked(someOtherObject);}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			BlockStatement block = BlockStatement.parse(t) as BlockStatement;

			//General tests
			ExpressionStatement statement = block.Statements[1] as ExpressionStatement;
			AssignmentExpression statementExpression = statement.Expression as AssignmentExpression;
			Expression exp = statementExpression.Expression2;
			Assertion.Assert("Expected CheckedExpression, but got something else!", exp is CheckedExpression);
			CheckedExpression theExp = exp as CheckedExpression;
			Assertion.Assert("Expression is wrong type!", theExp.Expression is Identifier);

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 1, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 1, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 54, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 77, exp.Position.EndCol);
		}

		[Test]
		public void testUncheckedExpression1()
		{
			//Preparation
			string fileString = "{SomeClass someObject, someOtherObject; someObject = unchecked(someOtherObject);}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			BlockStatement block = BlockStatement.parse(t) as BlockStatement;

			//General tests
			ExpressionStatement statement = block.Statements[1] as ExpressionStatement;
			AssignmentExpression statementExpression = statement.Expression as AssignmentExpression;
			Expression exp = statementExpression.Expression2;
			Assertion.Assert("Expected UncheckedExpression, but got something else!", exp is UncheckedExpression);
			UncheckedExpression theExp = exp as UncheckedExpression;
			Assertion.Assert("Expression is wrong type!", theExp.Expression is Identifier);

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 1, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 1, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 54, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 79, exp.Position.EndCol);
		}

		[Test]
		public void testObjectCreationExpression1()
		{
			//Preparation
			string fileString = "new SomeObject()";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Expected ObjectCreationExpression, but got something else!", exp is ObjectCreationExpression);
			ObjectCreationExpression theExp = exp as ObjectCreationExpression;
			Assertion.AssertEquals("Type name is wrong!", "SomeObject", theExp.Type.Name);
			Assertion.AssertEquals("Argument count is wrong!", 0, theExp.Arguments.Count);

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 0, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 0, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 16, exp.Position.EndCol);
		}

		[Test]
		public void testObjectCreationExpression2()
		{
			//Preparation
			string fileString = "new SomeOtherObject()";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Expected ObjectCreationExpression, but got something else!", exp is ObjectCreationExpression);
			ObjectCreationExpression theExp = exp as ObjectCreationExpression;
			Assertion.AssertEquals("Type name is wrong!", "SomeOtherObject", theExp.Type.Name);
			Assertion.AssertEquals("Argument count is wrong!", 0, theExp.Arguments.Count);

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 0, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 0, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 21, exp.Position.EndCol);
		}

		[Test]
		public void testObjectCreationExpressionWithArguments()
		{
			//Preparation
			string fileString = "new SomeObject(4, \"a\")";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			Assertion.Assert("Expected ObjectCreationExpression, but got something else!", exp is ObjectCreationExpression);

			//General tests
			ObjectCreationExpression theExp = exp as ObjectCreationExpression;
			Assertion.AssertEquals("Type name is wrong!", "SomeObject", theExp.Type.Name);
			Assertion.AssertEquals("Argument count is wrong!", 2, theExp.Arguments.Count);

			Argument arg1 = theExp.Arguments[0];
			Assertion.Assert("Expected Identifier but got something else!", arg1.Expression is IntegerLiteral);
			Assertion.AssertEquals("Argument should not be out.", false, arg1.IsOut);
			Assertion.AssertEquals("Argument should not be ref.", false, arg1.IsRef);
			Assertion.AssertEquals("StartLine is wrong!", 1, arg1.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 16, arg1.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, arg1.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 16, arg1.Position.EndCol);

			Argument arg2 = theExp.Arguments[1];
			Assertion.Assert("Expected Identifier but got something else!", arg2.Expression is StringLiteral);
			Assertion.AssertEquals("Argument should not be out.", false, arg2.IsOut);
			Assertion.AssertEquals("Argument should not be ref.", false, arg2.IsRef);
			Assertion.AssertEquals("StartLine is wrong!", 1, arg2.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 19, arg2.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, arg2.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 21, arg2.Position.EndCol);

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 2, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 0, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 22, exp.Position.EndCol);
		}

		[Test]
		public void testObjectCreationExpressionWithRefArgument()
		{
			//Preparation
			string fileString = "new SomeObject(ref a)";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Expected ObjectCreationExpression, but got something else!", exp is ObjectCreationExpression);
			ObjectCreationExpression theExp = exp as ObjectCreationExpression;
			Assertion.AssertEquals("Type name is wrong!", "SomeObject", theExp.Type.Name);
			Assertion.AssertEquals("Argument count is wrong!", 1, theExp.Arguments.Count);

			Argument arg1 = theExp.Arguments[0];
			Assertion.Assert("Expected Identifier but got something else!", arg1.Expression is Identifier);
			Assertion.AssertEquals("Argument should not be out.", false, arg1.IsOut);
			Assertion.AssertEquals("Argument should be ref.", true, arg1.IsRef);

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 1, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 0, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests (argument)
			Assertion.AssertEquals("StartLine is wrong!", 1, arg1.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 16, arg1.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, arg1.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 20, arg1.Position.EndCol);

			//Position tests (entire expression)
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 21, exp.Position.EndCol);
		}

		[Test]
		public void testObjectCreationExpressionWithOutArgument()
		{
			//Preparation
			string fileString = "{int a; SomeClass someObject = new SomeClass(out a);}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			BlockStatement block = BlockStatement.parse(t) as BlockStatement;

			//General tests
			DeclarationStatement statement = block.Statements[1] as DeclarationStatement;
			Expression exp = statement.LocalVariableDeclaration.VariableDeclarators[0].Initializer as Expression;
			Assertion.Assert("Expected ObjectCreationExpression, but got something else!", exp is ObjectCreationExpression);
			ObjectCreationExpression theExp = exp as ObjectCreationExpression;
			Assertion.AssertEquals("Type name is wrong!", "SomeClass", theExp.Type.Name);
			Assertion.AssertEquals("Argument count is wrong!", 1, theExp.Arguments.Count);
			Assertion.AssertEquals("Generate string is wrong!", fileString, block.Generate());

			Argument arg1 = theExp.Arguments[0];
			Assertion.Assert("Expected Identifier but got something else!", arg1.Expression is Identifier);
			Assertion.AssertEquals("Argument should be out.", true, arg1.IsOut);
			Assertion.AssertEquals("Argument should not be ref.", false, arg1.IsRef);

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 1, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 1, exp.DeepReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 1, exp.DeepModifiedVariables.Count);

			//Position tests (argument)
			Assertion.AssertEquals("StartLine is wrong!", 1, arg1.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 46, arg1.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, arg1.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 50, arg1.Position.EndCol);

			//Position tests (entire espression)
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 32, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 51, exp.Position.EndCol);
		}

		[Test]
		public void testTypeofExpression()
		{
			//Preparation
			string fileString = "typeof(SomeType)";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Expected TypeofExpression, but got something else!", exp is TypeofExpression);
			TypeofExpression theExp = exp as TypeofExpression;
			Assertion.AssertEquals("Type name is wrong!", "SomeType", theExp.Type.Name);
			Assertion.AssertEquals("Generate string is wrong!", fileString, exp.Generate());

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 0, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 0, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 16, exp.Position.EndCol);
		}

		[Test]
		public void testElementAccessExpression1()
		{
			//Preparation
			string fileString = "someArray[9]";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Expected ElementAccessExpression, but got something else!", exp is ElementAccessExpression);
			ElementAccessExpression theExp = exp as ElementAccessExpression;
			Assertion.AssertEquals("Generate string is wrong!", fileString, exp.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 12, exp.Position.EndCol);
		}

		[Test]
		public void testDimSeperators1()
		{
			//Preparation
			string fileString = ", ,  ,,";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			DimSeperatorCollection col = DimSeperatorCollection.parse(t);

			//General tests
			Assertion.AssertEquals("DimSeperator count is wrong!", 4, col.Count);
			Assertion.AssertEquals("Generate string is wrong!", fileString, col.Generate());

			DimSeperator sep1 = col[0];
			Assertion.AssertEquals("StartLine is wrong!", 1, sep1.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, sep1.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, sep1.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 1, sep1.Position.EndCol);

			DimSeperator sep2 = col[1];
			Assertion.AssertEquals("StartLine is wrong!", 1, sep2.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, sep2.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, sep2.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 3, sep2.Position.EndCol);

			DimSeperator sep3 = col[2];
			Assertion.AssertEquals("StartLine is wrong!", 1, sep3.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 6, sep3.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, sep3.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 6, sep3.Position.EndCol);

			DimSeperator sep4 = col[3];
			Assertion.AssertEquals("StartLine is wrong!", 1, sep4.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 7, sep4.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, sep4.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 7, sep4.Position.EndCol);
		}

		[Test]
		public void testRankSpecifiers1()
		{
			//Preparation
			string fileString = "[, ,  ,,][,][]";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			RankSpecifierCollection col = RankSpecifierCollection.parse(t);

			//General tests
			Assertion.AssertEquals("RankSpecifiers count is wrong!", 3, col.Count);
			RankSpecifier rs1 = col[0];
			RankSpecifier rs2 = col[1];
			RankSpecifier rs3 = col[2];
			Assertion.AssertEquals("Generate string is wrong!", fileString, col.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, rs1.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, rs1.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, rs1.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 9, rs1.Position.EndCol);

			Assertion.AssertEquals("StartLine is wrong!", 1, rs2.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 10, rs2.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, rs2.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 12, rs2.Position.EndCol);

			Assertion.AssertEquals("StartLine is wrong!", 1, rs3.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 13, rs3.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, rs3.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 14, rs3.Position.EndCol);
		}

		[Test]
		public void testArrayCreationExpression1()
		{
			//Preparation
			string fileString = "new int[100, 8][,][]";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = ArrayCreationExpression.parse(t);

			//General tests
			Assertion.Assert("Expected ArrayCreationExpression but got something else!", exp is ArrayCreationExpression);
			ArrayCreationExpression theExp = exp as ArrayCreationExpression;
			Assertion.AssertEquals("Generate string is wrong!", fileString, theExp.Generate());
			
			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 20, exp.Position.EndCol);
		}
	}
}
