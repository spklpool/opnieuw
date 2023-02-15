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
	public class InvocationExpressionTest : TokenizerTestBase
	{
		[Test]
		public void testInvocationExpression1()
		{
			//Preparation
			string fileString = "a(b)";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Expected InvocationExpression, but got something else!", exp is InvocationExpression);
			InvocationExpression theExp = exp as InvocationExpression;
			AssertEquals("Pretty Print string is wrong!", "a(b)", theExp.Generate());

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions is wrong!", 2, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 0, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Argument position tests
			Argument arg1 = theExp.Arguments[0];
			Assertion.AssertEquals("StartLine is wrong!", 1, arg1.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, arg1.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, arg1.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 3, arg1.Position.EndCol);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, theExp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, theExp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, theExp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 4, theExp.Position.EndCol);
		}

		[Test]
		public void testInvocationExpression2()
		{
			//Preparation
			string fileString = "System.Windows.Forms.MessageBox.Show(\"Hello World!\")";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Expected InvocationExpression, but got something else!", exp is InvocationExpression);
			InvocationExpression theExp = exp as InvocationExpression;
			Assertion.AssertEquals("Argument count is wrong!", 1, theExp.Arguments.Count);
			Assertion.Assert("Expression is the wrong type!", theExp.Arguments[0].Expression is StringLiteral);
			AssertEquals("Pretty Print string is wrong!", "System.Windows.Forms.MessageBox.Show(\"Hello World!\")", theExp.Generate());

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions is wrong!", 6, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 0, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, theExp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, theExp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, theExp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 52, theExp.Position.EndCol);
		}

		[Test]
		public void testInvocationExpression3()
		{
			//Preparation
			string fileString = "SomeObject.SomeMethod(someParameter)";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Expected InvocationExpression, but got something else!", exp is InvocationExpression);
			InvocationExpression theExp = exp as InvocationExpression;
			AssertEquals("Pretty Print string is wrong!", "SomeObject.SomeMethod(someParameter)", theExp.Generate());

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions is wrong!", 3, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 0, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, theExp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, theExp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, theExp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 36, theExp.Position.EndCol);
		}

		[Test]
		public void testInvocationExpression4()
		{
			//Preparation
			string fileString = "this.Details.Controls.AddRange(new System.Windows.Forms.Control[] {this.Info})";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Expected InvocationExpression, but got something else!", exp is InvocationExpression);
			InvocationExpression theExp = exp as InvocationExpression;
			
			//Generate test
			AssertEquals("Pretty Print string is wrong!", fileString, theExp.Generate());

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions is wrong!", 7, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 0, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Argument 1 tests
			Argument arg1 = theExp.Arguments[0];
			Assertion.Assert("Expected ArrayCreationExpression as agrument, but got something else!", arg1.Expression is ArrayCreationExpression);
			Assertion.AssertEquals("StartLine is wrong!", 1, arg1.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 32, arg1.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, arg1.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 77, arg1.Position.EndCol);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, theExp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, theExp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, theExp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 78, theExp.Position.EndCol);
		}

		[Test]
		public void testInvocationExpression5()
		{
			//Preparation
			string fileString = "this.Controls.AddRange(new System.Windows.Forms.Control[] {this.ButtonPannel,this.Details,this.splitter1})";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Expected InvocationExpression, but got something else!", exp is InvocationExpression);
			InvocationExpression theExp = exp as InvocationExpression;

			//Generate test
			AssertEquals("Pretty Print string is wrong!", fileString, theExp.Generate());

			//Argument 1 tests
			Argument arg1 = theExp.Arguments[0];
			Assertion.Assert("Expected ArrayCreationExpression as agrument, but got something else!", arg1.Expression is ArrayCreationExpression);
			Assertion.AssertEquals("StartLine is wrong!", 1, arg1.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 24, arg1.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, arg1.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 105, arg1.Position.EndCol);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, theExp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, theExp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, theExp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 106, theExp.Position.EndCol);
		}

		[Test]
		public void testInvocationExpression6()
		{
			//Preparation
			string fileString = "someObject.AddRange(new Control[] {this.Info})";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Expected InvocationExpression, but got something else!", exp is InvocationExpression);
			InvocationExpression theExp = exp as InvocationExpression;
			
			//Generate test
			AssertEquals("Pretty Print string is wrong!", fileString, theExp.Generate());

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 5, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 0, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Argument 1 tests
			Argument arg1 = theExp.Arguments[0];
			Assertion.Assert("Expected ArrayCreationExpression as agrument, but got something else!", arg1.Expression is ArrayCreationExpression);
			Assertion.AssertEquals("StartLine is wrong!", 1, arg1.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 21, arg1.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, arg1.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 45, arg1.Position.EndCol);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, theExp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, theExp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, theExp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 46, theExp.Position.EndCol);
		}

		[Test]
		public void testInvocationExpression7()
		{
			//Preparation
			string fileString = "public class SomeClass { private void SomeMethod(SomeOtherClass someObject, int someParameter) { someObject.SomeMethod(someParameter);}}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Class cls = Namespace.parseType(t) as Class;

			//General tests
			MethodDeclaration method = cls.Members[0] as MethodDeclaration;
			ExpressionStatement s = method.Body.Statements[0] as ExpressionStatement;
			Assertion.Assert("Expected InvocationExpression, but got something else!", s.Expression is InvocationExpression);
			InvocationExpression theExp = s.Expression as InvocationExpression;
			
			//Generate test
			AssertEquals("Pretty Print string is wrong!", fileString, cls.Generate());

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions is wrong!", 3, theExp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, theExp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 2, theExp.DeepReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, theExp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, theExp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 98, theExp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, theExp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 133, theExp.Position.EndCol);
		}

		[Test]
		public void testInvocationExpression8()
		{
			//Preparation
			string fileString = "SomeMethod()";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Expected InvocationExpression, but got something else!", exp is InvocationExpression);
			InvocationExpression theExp = exp as InvocationExpression;
			
			//Generate test
			AssertEquals("Pretty Print string is wrong!", fileString, theExp.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, theExp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, theExp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, theExp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 12, theExp.Position.EndCol);
		}

		[Test]
		public void testInvocationExpression9()
		{
			//Preparation
			string fileString = "\t\tSomeMethod()";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Expected InvocationExpression, but got something else!", exp is InvocationExpression);
			InvocationExpression theExp = exp as InvocationExpression;
			
			//Generate test
			AssertEquals("Pretty Print string is wrong!", fileString, theExp.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, theExp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 3, theExp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, theExp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 14, theExp.Position.EndCol);
		}
	}
}
