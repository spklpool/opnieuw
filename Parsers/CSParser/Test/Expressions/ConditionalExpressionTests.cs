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
	public class ConditionalExpressionTest : TokenizerTestBase
	{
		[Test]
		public void testConditionalOrExpression()
		{
			//Preparation
			string fileString = "public class SomeClass { private void SomeMethod(bool b, bool c) { bool a = b || c;}}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			CompilationUnit unit = CompilationUnit.Parse(t);

			//General tests
			Class cls = unit.Members[0] as Class;
			MethodDeclaration method = cls.Members[0] as MethodDeclaration;
			DeclarationStatement s = method.Body.Statements[0] as DeclarationStatement;
			VariableDeclarator vd = s.LocalVariableDeclaration.VariableDeclarators[0];
			Assertion.Assert("Expected initializer to be an expression, but it was not!", vd.Initializer is Expression);
			Expression exp = vd.Initializer as Expression;
			ConditionalOrExpression theExp = exp as ConditionalOrExpression;
			AssertEquals("Pretty Print string is wrong!", " b || c", theExp.Generate());

			//Variable and expression tests
			AssertEquals("Number of children is wrong!", 2, exp.Children.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 2, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 2, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, theExp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 77, theExp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, theExp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 82, theExp.Position.EndCol);
		}

		[Test]
		public void testConditionalAndExpression2()
		{
			//Preparation
			string fileString = "public class SomeClass { private void SomeMethod(bool b, bool c) { bool a = b && c;}}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			CompilationUnit unit = CompilationUnit.Parse(t);

			//General tests
			Class cls = unit.Members[0] as Class;
			MethodDeclaration method = cls.Members[0] as MethodDeclaration;
			DeclarationStatement s = method.Body.Statements[0] as DeclarationStatement;
			VariableDeclarator vd = s.LocalVariableDeclaration.VariableDeclarators[0];
			Assertion.Assert("Expected initializer to be an expression, but it was not!", vd.Initializer is Expression);
			Expression exp = vd.Initializer as Expression;
			ConditionalAndExpression theExp = exp as ConditionalAndExpression;
			AssertEquals("Pretty Print string is wrong!", " b && c", theExp.Generate());

			//Variable and expression tests
			AssertEquals("Number of children is wrong!", 2, exp.Children.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 2, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 2, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, theExp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 77, theExp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, theExp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 82, theExp.Position.EndCol);
		}

		[Test]
		public void testInclusiveOrExpression2()
		{
			//Preparation
			string fileString = "public class SomeClass { private void SomeMethod(bool b, bool c) { bool a = b | c;}}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			CompilationUnit unit = CompilationUnit.Parse(t);

			//General tests
			Class cls = unit.Members[0] as Class;
			MethodDeclaration method = cls.Members[0] as MethodDeclaration;
			DeclarationStatement s = method.Body.Statements[0] as DeclarationStatement;
			VariableDeclarator vd = s.LocalVariableDeclaration.VariableDeclarators[0];
			Assertion.Assert("Expected initializer to be an expression, but it was not!", vd.Initializer is Expression);
			Expression exp = vd.Initializer as Expression;
			InclusiveOrExpression theExp = exp as InclusiveOrExpression;
			AssertEquals("Pretty Print string is wrong!", " b | c", theExp.Generate());

			//Variable and expression tests
			AssertEquals("Number of children is wrong!", 2, exp.Children.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 2, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 2, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, theExp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 77, theExp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, theExp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 81, theExp.Position.EndCol);
		}

		[Test]
		public void testExclusiveOrExpression2()
		{
			//Preparation
			string fileString = "public class SomeClass { private void SomeMethod(bool b, bool c) { bool a = b ^ c;}}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			CompilationUnit unit = CompilationUnit.Parse(t);

			//General tests
			Class cls = unit.Members[0] as Class;
			MethodDeclaration method = cls.Members[0] as MethodDeclaration;
			DeclarationStatement s = method.Body.Statements[0] as DeclarationStatement;
			VariableDeclarator vd = s.LocalVariableDeclaration.VariableDeclarators[0];
			Assertion.Assert("Expected initializer to be an expression, but it was not!", vd.Initializer is Expression);
			Expression exp = vd.Initializer as Expression;
			ExclusiveOrExpression theExp = exp as ExclusiveOrExpression;
			AssertEquals("Pretty Print string is wrong!", " b ^ c", theExp.Generate());

			//Variable and expression tests
			AssertEquals("Number of children is wrong!", 2, exp.Children.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 2, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 2, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, theExp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 77, theExp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, theExp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 81, theExp.Position.EndCol);
		}

		[Test]
		public void testAndExpression2()
		{
			//Preparation
			string fileString = "public class SomeClass { private void SomeMethod(bool b, bool c) { bool a = b & c;}}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			CompilationUnit unit = CompilationUnit.Parse(t);

			//General tests
			Class cls = unit.Members[0] as Class;
			MethodDeclaration method = cls.Members[0] as MethodDeclaration;
			DeclarationStatement s = method.Body.Statements[0] as DeclarationStatement;
			VariableDeclarator vd = s.LocalVariableDeclaration.VariableDeclarators[0];
			Assertion.Assert("Expected initializer to be an expression, but it was not!", vd.Initializer is Expression);
			Expression exp = vd.Initializer as Expression;
			AndExpression theExp = exp as AndExpression;
			AssertEquals("Pretty Print string is wrong!", " b & c", theExp.Generate());

			//Variable and expression tests
			AssertEquals("Number of children is wrong!", 2, exp.Children.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 2, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 2, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, theExp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 77, theExp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, theExp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 81, theExp.Position.EndCol);
		}

		[Test]
		public void testEqualityExpression1()
		{
			//Preparation
			string fileString = "public class SomeClass { private void SomeMethod(bool b, bool c) { bool a = b == c;}}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			CompilationUnit unit = CompilationUnit.Parse(t);

			//General tests
			Class cls = unit.Members[0] as Class;
			MethodDeclaration method = cls.Members[0] as MethodDeclaration;
			DeclarationStatement s = method.Body.Statements[0] as DeclarationStatement;
			VariableDeclarator vd = s.LocalVariableDeclaration.VariableDeclarators[0];
			Assertion.Assert("Expected initializer to be an expression, but it was not!", vd.Initializer is Expression);
			Expression exp = vd.Initializer as Expression;
			EqualityExpression theExp = exp as EqualityExpression;
			AssertEquals("Pretty Print string is wrong!", " b == c", theExp.Generate());

			//Variable and expression tests
			AssertEquals("Number of children is wrong!", 2, exp.Children.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 2, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 2, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, theExp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 77, theExp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, theExp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 82, theExp.Position.EndCol);
		}

		[Test]
		public void testEqualityExpression2()
		{
			//Preparation
			string fileString = "public class SomeClass { private void SomeMethod(bool b, bool c) { bool a = b != c;}}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			CompilationUnit unit = CompilationUnit.Parse(t);

			//General tests
			Class cls = unit.Members[0] as Class;
			MethodDeclaration method = cls.Members[0] as MethodDeclaration;
			DeclarationStatement s = method.Body.Statements[0] as DeclarationStatement;
			VariableDeclarator vd = s.LocalVariableDeclaration.VariableDeclarators[0];
			Assertion.Assert("Expected initializer to be an expression, but it was not!", vd.Initializer is Expression);
			Expression exp = vd.Initializer as Expression;
			EqualityExpression theExp = exp as EqualityExpression;
			AssertEquals("Pretty Print string is wrong!", " b != c", theExp.Generate());

			//Variable and expression tests
			AssertEquals("Number of children is wrong!", 2, exp.Children.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 2, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 2, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, theExp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 77, theExp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, theExp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 82, theExp.Position.EndCol);
		}

		[Test]
		public void testShiftExpression1()
		{
			//Preparation
			string fileString = "public class SomeClass { private void SomeMethod(bool b, bool c) { bool a = b << c;}}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			CompilationUnit unit = CompilationUnit.Parse(t);

			//General tests
			Class cls = unit.Members[0] as Class;
			MethodDeclaration method = cls.Members[0] as MethodDeclaration;
			DeclarationStatement s = method.Body.Statements[0] as DeclarationStatement;
			VariableDeclarator vd = s.LocalVariableDeclaration.VariableDeclarators[0];
			Assertion.Assert("Expected initializer to be an expression, but it was not!", vd.Initializer is Expression);
			Expression exp = vd.Initializer as Expression;
			ShiftExpression theExp = exp as ShiftExpression;
			AssertEquals("Pretty Print string is wrong!", " b << c", theExp.Generate());

			//Variable and expression tests
			AssertEquals("Number of children is wrong!", 2, exp.Children.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 2, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 2, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, theExp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 77, theExp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, theExp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 82, theExp.Position.EndCol);
		}

		[Test]
		public void testShiftExpression2()
		{
			//Preparation
			string fileString = "public class SomeClass { private void SomeMethod(bool b, bool c) { bool a = b >> c;}}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			CompilationUnit unit = CompilationUnit.Parse(t);

			//General tests
			Class cls = unit.Members[0] as Class;
			MethodDeclaration method = cls.Members[0] as MethodDeclaration;
			DeclarationStatement s = method.Body.Statements[0] as DeclarationStatement;
			VariableDeclarator vd = s.LocalVariableDeclaration.VariableDeclarators[0];
			Assertion.Assert("Expected initializer to be an expression, but it was not!", vd.Initializer is Expression);
			Expression exp = vd.Initializer as Expression;
			ShiftExpression theExp = exp as ShiftExpression;
			AssertEquals("Pretty Print string is wrong!", " b >> c", theExp.Generate());

			//Variable and expression tests
			AssertEquals("Number of children is wrong!", 2, exp.Children.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 2, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 2, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, theExp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 77, theExp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, theExp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 82, theExp.Position.EndCol);
		}

		[Test]
		public void testAdditiveExpression1()
		{
			//Preparation
			string fileString = "public class SomeClass { private void SomeMethod(int b, int c) { int a = b + c;}}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			CompilationUnit unit = CompilationUnit.Parse(t);

			//General tests
			Class cls = unit.Members[0] as Class;
			MethodDeclaration method = cls.Members[0] as MethodDeclaration;
			DeclarationStatement s = method.Body.Statements[0] as DeclarationStatement;
			VariableDeclarator vd = s.LocalVariableDeclaration.VariableDeclarators[0];
			Assertion.Assert("Expected initializer to be an expression, but it was not!", vd.Initializer is Expression);
			Expression exp = vd.Initializer as Expression;
			AdditiveExpression theExp = exp as AdditiveExpression;
			AssertEquals("Pretty Print string is wrong!", " b + c", theExp.Generate());

			//Variable and expression tests
			AssertEquals("Number of children is wrong!", 2, exp.Children.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 2, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 2, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, theExp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 74, theExp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, theExp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 78, theExp.Position.EndCol);
		}

		[Test]
		public void testAdditiveExpression2()
		{
			//Preparation
			string fileString = "public class SomeClass { private void SomeMethod(int b, int c) { int a = b - c;}}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			CompilationUnit unit = CompilationUnit.Parse(t);

			//General tests
			Class cls = unit.Members[0] as Class;
			MethodDeclaration method = cls.Members[0] as MethodDeclaration;
			DeclarationStatement s = method.Body.Statements[0] as DeclarationStatement;
			VariableDeclarator vd = s.LocalVariableDeclaration.VariableDeclarators[0];
			Assertion.Assert("Expected initializer to be an expression, but it was not!", vd.Initializer is Expression);
			Expression exp = vd.Initializer as Expression;
			AdditiveExpression theExp = exp as AdditiveExpression;
			AssertEquals("Pretty Print string is wrong!", " b - c", theExp.Generate());

			//Variable and expression tests
			AssertEquals("Number of children is wrong!", 2, exp.Children.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 2, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 2, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, theExp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 74, theExp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, theExp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 78, theExp.Position.EndCol);
		}

		[Test]
		public void testMultiplicativeExpression2()
		{
			//Preparation
			string fileString = "public class SomeClass { private void SomeMethod(int b, int c) { int a = b * c;}}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			CompilationUnit unit = CompilationUnit.Parse(t);

			//General tests
			Class cls = unit.Members[0] as Class;
			MethodDeclaration method = cls.Members[0] as MethodDeclaration;
			DeclarationStatement s = method.Body.Statements[0] as DeclarationStatement;
			VariableDeclarator vd = s.LocalVariableDeclaration.VariableDeclarators[0];
			Assertion.Assert("Expected initializer to be an expression, but it was not!", vd.Initializer is Expression);
			Expression exp = vd.Initializer as Expression;
			MultiplicativeExpression theExp = exp as MultiplicativeExpression;
			AssertEquals("Pretty Print string is wrong!", " b * c", theExp.Generate());

			//Variable and expression tests
			AssertEquals("Number of children is wrong!", 2, exp.Children.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 2, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 2, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, theExp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 74, theExp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, theExp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 78, theExp.Position.EndCol);
		}

		[Test]
		public void testMultiplicativeExpression4()
		{
			//Preparation
			string fileString = "public class SomeClass { private void SomeMethod(int b, int c) { int a = b / c;}}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			CompilationUnit unit = CompilationUnit.Parse(t);

			//General tests
			Class cls = unit.Members[0] as Class;
			MethodDeclaration method = cls.Members[0] as MethodDeclaration;
			DeclarationStatement s = method.Body.Statements[0] as DeclarationStatement;
			VariableDeclarator vd = s.LocalVariableDeclaration.VariableDeclarators[0];
			Assertion.Assert("Expected initializer to be an expression, but it was not!", vd.Initializer is Expression);
			Expression exp = vd.Initializer as Expression;
			MultiplicativeExpression theExp = exp as MultiplicativeExpression;
			AssertEquals("Pretty Print string is wrong!", " b / c", theExp.Generate());

			//Variable and expression tests
			AssertEquals("Number of children is wrong!", 2, exp.Children.Count);
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 2, exp.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, exp.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 2, exp.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, exp.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, theExp.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 74, theExp.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, theExp.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 78, theExp.Position.EndCol);
		}
	}
}
