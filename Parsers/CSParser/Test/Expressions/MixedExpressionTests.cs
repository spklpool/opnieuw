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
	public class MixedExpressionTest : TokenizerTestBase
	{
		[Test]
		public void testMixedExpression1()
		{
			//Preparation
			string fileString = "((line[i] == ' ') || (line[i] == '\\t')) && (spacesRemoved <= spacesToRemove)";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("An expression type is wrong!", exp is ConditionalAndExpression);
			ConditionalAndExpression theExp = exp as ConditionalAndExpression;
			Assertion.Assert("An expression type is wrong!", theExp.Expression1 is ParenthesizedExpression);
			ParenthesizedExpression expression1 = theExp.Expression1 as ParenthesizedExpression;
			Assertion.Assert("An expression type is wrong!", expression1.InnerExpression is ConditionalOrExpression);
			ConditionalOrExpression expression1Inner = expression1.InnerExpression as ConditionalOrExpression;
			Assertion.Assert("An expression type is wrong!", theExp.Expression1 is ParenthesizedExpression);
			ParenthesizedExpression expression2 = theExp.Expression2 as ParenthesizedExpression;
			Assertion.Assert("An expression type is wrong!", expression2.InnerExpression is RelationalExpression);
			RelationalExpression expression2Inner = expression2.InnerExpression as RelationalExpression;
			AssertEquals("Pretty Print string is wrong!", "((line[i] == ' ') || (line[i] == '\\t')) && (spacesRemoved <= spacesToRemove)", theExp.Generate());

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions is wrong!", 18, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 0, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, theExp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, theExp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, theExp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 76, theExp.Position.EndCol);
			//expression1
			Assertion.AssertEquals("StartLine is wrong!", 1, expression1.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, expression1.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, expression1.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 39, expression1.Position.EndCol);
			//expression2
			Assertion.AssertEquals("StartLine is wrong!", 1, expression2.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 44, expression2.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, expression2.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 76, expression2.Position.EndCol);
		}

		[Test]
		public void testMixedExpression2()
		{
			//Preparation
			string fileString = "this.Details.Controls.AddRange(new System.Windows.Forms.Control[] {this.Info})";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("An expression type is wrong!", exp is InvocationExpression);
			InvocationExpression theExp = exp as InvocationExpression;
			Assertion.Assert("An expression type is wrong!", theExp.Expression is MemberAccessExpression);
			MemberAccessExpression memberAccess = theExp.Expression as MemberAccessExpression;
			ArgumentCollection arguments = theExp.Arguments;
			Assertion.AssertEquals("Argument count is wrong!", 1, arguments.Count);
			Assertion.Assert("An expression type is wrong!", arguments[0].Expression is ArrayCreationExpression);
			ArrayCreationExpression arrayCreation = arguments[0].Expression as ArrayCreationExpression;
			ArrayInitializer arrayInitializer = arrayCreation.ArrayInitializer;
			Assertion.Assert("An expression type is wrong!", arrayInitializer.Initializers[0] is MemberAccessExpression);
			MemberAccessExpression memberAccessInInitializer = arrayInitializer.Initializers[0] as MemberAccessExpression;
			Assertion.Assert("An expression type is wrong!", memberAccessInInitializer.Expression is ThisAccessExpression);
			ThisAccessExpression thisAccessInInitializer = memberAccessInInitializer.Expression as ThisAccessExpression;

			//Generate test
			AssertEquals("Pretty Print string is wrong!", fileString, theExp.Generate());

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 7, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 0, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);
		}

		[Test]
		public void testMixedExpression3()
		{
			//Preparation
			string fileString = "m_prod.m_prec!=0 && Precedence.Check(s.m_prec,Precedence.PrecType.nonassoc,0)==m_prod.m_prec";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("An expression type is wrong!", exp is ConditionalAndExpression);
			ConditionalAndExpression theExp = exp as ConditionalAndExpression;
			Assertion.Assert("An expression type is wrong!", theExp.Expression1 is EqualityExpression);
			Assertion.Assert("An expression type is wrong!", theExp.Expression2 is EqualityExpression);
			EqualityExpression expression1 = theExp.Expression1 as EqualityExpression;
			EqualityExpression expression2 = theExp.Expression2 as EqualityExpression;
			Assertion.Assert("An expression type is wrong!", expression1.Expression1 is MemberAccessExpression);
			Assertion.Assert("An expression type is wrong!", expression1.Expression2 is IntegerLiteral);
			MemberAccessExpression expression1MemberAccess = expression1.Expression1 as MemberAccessExpression;
			IntegerLiteral expression1Literal = expression1.Expression2 as IntegerLiteral;
			Assertion.Assert("An expression type is wrong!", expression2.Expression1 is InvocationExpression);
			Assertion.Assert("An expression type is wrong!", expression2.Expression2 is MemberAccessExpression);
			InvocationExpression expression2Invocation = expression2.Expression1 as InvocationExpression;
			MemberAccessExpression expression2EqualityExpression = expression2.Expression2 as MemberAccessExpression;
			AssertEquals("Pretty Print string is wrong!", fileString, theExp.Generate());

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 3, expression1.Expressions.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 8, expression2Invocation.Expressions.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 1, expression2EqualityExpression.Expressions.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 11, expression2.Expressions.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 16, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 0, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, theExp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, theExp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, theExp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 92, theExp.Position.EndCol);
		}

		[Test]
		public void testMixedExpression4()
		{
			//Preparation
			string fileString = "new CSToolsException(12,string.Format(1,m_prod.m_pno,pr.m_prod.m_pno) + string.Format(1,ps.m_state,s.yytext))";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("An expression type is wrong!", exp is ObjectCreationExpression);
			ObjectCreationExpression theExp = exp as ObjectCreationExpression;
			Assertion.AssertEquals("Argument count is wrong!", 2, theExp.Arguments.Count);
			Assertion.Assert("Expression type is wrong!", theExp.Arguments[0].Expression is IntegerLiteral);
			Assertion.Assert("Expression type is wrong!", theExp.Arguments[1].Expression is AdditiveExpression);
			AdditiveExpression additiveExpression = theExp.Arguments[1].Expression as AdditiveExpression;
            Assertion.Assert("Expression type is wrong!", additiveExpression.Expression1 is InvocationExpression);
			InvocationExpression invocation1 = additiveExpression.Expression1 as InvocationExpression;
			Assertion.Assert("Expression type is wrong!", additiveExpression.Expression2 is InvocationExpression);
			InvocationExpression invocation2 = additiveExpression.Expression2 as InvocationExpression;
			AssertEquals("Pretty Print string is wrong!", fileString, theExp.Generate());

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 7, invocation1.Expressions.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 6, invocation2.Expressions.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 15, additiveExpression.Expressions.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 17, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 0, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);
		}

		[Test]
		public void testMixedExpression5()
		{
			//Preparation
			string fileString = "s.m_parser.m_symbols.erh.Error(new CSToolsException())";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("An expression type is wrong!", exp is InvocationExpression);
			InvocationExpression theExp = exp as InvocationExpression;
			Assertion.Assert("An expression type is wrong!", theExp.Expression is MemberAccessExpression);
			MemberAccessExpression memberAccess1 = theExp.Expression as MemberAccessExpression;
			Assertion.Assert("An expression type is wrong!", memberAccess1.Expression is MemberAccessExpression);
			MemberAccessExpression memberAccess2 = memberAccess1.Expression as MemberAccessExpression;
			Assertion.Assert("An expression type is wrong!", memberAccess2.Expression is MemberAccessExpression);
			MemberAccessExpression memberAccess3 = memberAccess2.Expression as MemberAccessExpression;
			Assertion.Assert("An expression type is wrong!", memberAccess3.Expression is MemberAccessExpression);
			MemberAccessExpression memberAccess4 = memberAccess3.Expression as MemberAccessExpression;
			AssertEquals("Pretty Print string is wrong!", fileString, theExp.Generate());

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 1, memberAccess4.Expressions.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 2, memberAccess3.Expressions.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 3, memberAccess2.Expressions.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 4, memberAccess1.Expressions.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 6, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 0, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);
		}

		[Test]
		public void testMixedExpression6()
		{
			//Preparation
			string fileString = "s.m_parser.m_symbols.erh.Error(new CSToolsException(12,1+2))";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("An expression type is wrong!", exp is InvocationExpression);
			InvocationExpression theExp = exp as InvocationExpression;
			Assertion.Assert("An expression type is wrong!", theExp.Expression is MemberAccessExpression);
			MemberAccessExpression memberAccess1 = theExp.Expression as MemberAccessExpression;
			Assertion.Assert("An expression type is wrong!", memberAccess1.Expression is MemberAccessExpression);
			MemberAccessExpression memberAccess2 = memberAccess1.Expression as MemberAccessExpression;
			Assertion.Assert("An expression type is wrong!", memberAccess2.Expression is MemberAccessExpression);
			MemberAccessExpression memberAccess3 = memberAccess2.Expression as MemberAccessExpression;
			Assertion.Assert("An expression type is wrong!", memberAccess3.Expression is MemberAccessExpression);
			MemberAccessExpression memberAccess4 = memberAccess3.Expression as MemberAccessExpression;
			AssertEquals("Pretty Print string is wrong!", fileString, exp.Generate());

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 1, memberAccess4.Expressions.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 2, memberAccess3.Expressions.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 3, memberAccess2.Expressions.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 4, memberAccess1.Expressions.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 10, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 0, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);
		}

		[Test]
		public void testMixedExpression7()
		{
			//Preparation
			string fileString = "Error(new CSToolsException(12,string.Format(1,m_prod.m_pno,pr.m_prod.m_pno)+string.Format(1,ps.m_state,s.yytext)))";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("An expression type is wrong!", exp is InvocationExpression);
			InvocationExpression theExp = exp as InvocationExpression;
			Assertion.AssertEquals("Argument count is wrong!", 1, theExp.Arguments.Count);
			Assertion.Assert("An expression type is wrong!", theExp.Arguments[0].Expression is ObjectCreationExpression);
			AssertEquals("Pretty Print string is wrong!", fileString, exp.Generate());
		}

		[Test]
		public void testMixedExpression8()
		{
			//Preparation
			string fileString = "s.m_parser.m_symbols.erh.Error(new CSToolsException(0,string.Format(1,2,3)+0))";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Got a missing expression!", false == (exp is MissingExpression));
			AssertEquals("Pretty Print string is wrong!", fileString, exp.Generate());
		}

		[Test]
		public void testMixedExpression9()
		{
			//Preparation
			string fileString = "string.Format(1,2,3)";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Got a missing expression!", false == (exp is MissingExpression));
			AssertEquals("Pretty Print string is wrong!", fileString, exp.Generate());
		}

		[Test]
		public void testMixedExpression10()
		{
			//Preparation
			string fileString = "(System.Windows.Forms.ImageListStreamer)(resources.GetObject(1))";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Expected CastExpression but got something else!", exp is CastExpression);
			AssertEquals("Pretty Print string is wrong!", fileString, exp.Generate());
		}

		[Test]
		public void testMixedExpression11()
		{
			//Preparation
			string fileString = "this.Icons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject(\"Icons.ImageStream\")))";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Expected AssignmentExpression but got something else!", exp is AssignmentExpression);
			AssertEquals("Pretty Print string is wrong!", fileString, exp.Generate());
		}

		[Test]
		public void testMixedExpression12()
		{
			//Preparation
			string fileString = "(new CSymbol(yyp)).Resolve";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Got a missing expression!", false == (exp is MissingExpression));
			AssertEquals("Pretty Print string is wrong!", "(new CSymbol(yyp)).Resolve", exp.Generate());
		}

		[Test]
		public void testMixedExpression13()
		{
			//Preparation
			string fileString = "checked((anExpression))";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Expected CheckedExpression, but got something else!", exp is CheckedExpression);
			CheckedExpression checkedExpression = exp as CheckedExpression;
			Assertion.Assert("Expression is wrong type!", checkedExpression.Expression is ParenthesizedExpression);
			AssertEquals("Pretty Print string is wrong!", "checked((anExpression))", exp.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 23, exp.Position.EndCol);
		}

		[Test]
		public void testMixedExpression14()
		{
			//Preparation
			string fileString = "unchecked((anExpression))";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Expected UncheckedExpression, but got something else!", exp is UncheckedExpression);
			UncheckedExpression uncheckedExpression = exp as UncheckedExpression;
			Assertion.Assert("Expression is wrong type!", uncheckedExpression.Expression is ParenthesizedExpression);
			AssertEquals("Pretty Print string is wrong!", "unchecked((anExpression))", exp.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, exp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, exp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, exp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 25, exp.Position.EndCol);
		}

		[Test]
		public void testMixedExpression15()
		{
			//Preparation
			string fileString = "new System.Drawing.Font(9.75F)";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Expected ObjectCreationExpression, but got something else!", exp is ObjectCreationExpression);
			ObjectCreationExpression theExp = exp as ObjectCreationExpression;
		}

		[Test]
		public void testMixedExpression16()
		{
			//Preparation
			string fileString = "((System.Byte)(0))";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			Assertion.Assert("Expected ParenthesizedExpression, but got something else!", exp is ParenthesizedExpression);
			ParenthesizedExpression theExp = exp as ParenthesizedExpression;
		}

		[Test]
		public void testMixedExpression17()
		{
			//Preparation
			string fileString = "object[] param = new object[1] {fileName};";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement stmt = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected DeclarationStatement, but got " + stmt.ToString(), stmt is DeclarationStatement);
			DeclarationStatement theStatement = stmt as DeclarationStatement;
		}

		[Test]
		public void testMixedExpression18()
		{
			//Preparation
			string fileString = "new EnumBase(type, colonToken)";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Expression exp = Expression.parse(t);

			//General tests
			AssertEquals("Pretty Print string is wrong!", fileString, exp.Generate());
		}
	}
}
