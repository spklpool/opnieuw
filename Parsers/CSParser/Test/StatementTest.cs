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
	public class StatementTest : TokenizerTestBase
	{
		[Test]
		public void testTryStatement1()
		{
			//Preparation
			string fileString = "try{}catch{}finally{}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected TryStatement, but got something else!", s is TryStatement);
			TryStatement stmt = s as TryStatement;
			AssertEquals("Pretty Print string is wrong!", fileString, stmt.Generate());

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 0, stmt.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, stmt.DeclaredVariables.Count);
			Assertion.AssertEquals("Number of referenced variables is wrong!", 0, stmt.ReferencedVariables.Count);
			Assertion.AssertEquals("Number of modified variables is wrong!", 0, stmt.ModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 21, s.Position.EndCol);
		}

		[Test]
		public void testTryStatement2()
		{
			//Preparation
			string fileString = "try{}catch(ParserException e){}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected TryStatement, but got something else!", s is TryStatement);
			AssertEquals("Pretty Print string is wrong!", fileString, s.Generate());
		}

		[Test]
		public void testCatchClause1()
		{
			//Preparation
			string fileString = "catch{}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			CatchClause cc = CatchClause.parse(t);

			//General tests
			Assertion.Assert("Expected CatchClause, but got something else!", (false == cc is MissingCatchClause));
			AssertEquals("Pretty Print string is wrong!", fileString, cc.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, cc.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, cc.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, cc.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 7, cc.Position.EndCol);
		}

		[Test]
		public void testCatchClauseCollection1()
		{
			//Preparation
			string fileString = "catch{}catch{}catch{}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			CatchClauseCollection ccc = CatchClauseCollection.parse(t);

			//General tests
			Assertion.AssertEquals("CatchClause count is wrong!", 3, ccc.Count);
			AssertEquals("Pretty Print string is wrong!", fileString, ccc.Generate());
		}

		[Test]
		public void testCatchClauseCollection2()
		{
			//Preparation
			string fileString = "catch (ParserException e){;;}catch(Exception e2){;}catch{;}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			CatchClauseCollection ccc = CatchClauseCollection.parse(t);

			//General tests
			Assertion.AssertEquals("CatchClause count is wrong!", 3, ccc.Count);
			AssertEquals("Pretty Print string is wrong!", fileString, ccc.Generate());
		}

		[Test]
		public void testCatchClause2()
		{
			//Should throw exception because open curly brace is replaced 
			//with open parens.
			string fileString = "catch(}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			bool exceptionCaught = false;
			try
			{
				CatchClause cc = CatchClause.parse(t);
			}
			catch (ParserException)
			{
				exceptionCaught = true;
			}
			Assertion.AssertEquals("Expected exception but didn't get it!", true, exceptionCaught);
		}

		[Test]
		public void testFinallyClause1()
		{
			//Preparation
			string fileString = "finally{}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			FinallyClause fc = FinallyClause.parse(t);

			//General tests
			Assertion.AssertEquals("Finally clause is missing!", false, fc is MissingFinallyClause);
			AssertEquals("Pretty Print string is wrong!", fileString, fc.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, fc.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, fc.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, fc.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 9, fc.Position.EndCol);
		}

		[Test]
		public void testIfStatement1()
		{
			//Preparation
			string fileString = "{int a, b; if(b==1){a++;};}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			BlockStatement block = BlockStatement.parse(t) as BlockStatement;
			Statement s = block.Statements[1];

			//General tests
			Assertion.Assert("Expected IfStatement, but got something else!", s is IfStatement);
			IfStatement stmt = s as IfStatement;
			Assertion.Assert("Expected EqualityExpression, but got something else!", stmt.Expression is EqualityExpression);
			Assertion.Assert("Expected BlockStatement, but got something else!", stmt.IfPartStatement is BlockStatement);
			AssertEquals("Pretty Print string is wrong!", " if(b==1){a++;}", s.Generate());

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 5, stmt.Expressions.Count);
			AssertEquals("Number of declared variables is wrong!", 0, stmt.DeclaredVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 0, stmt.ReferencedVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 2, stmt.DeepReferencedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 0, stmt.ModifiedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 1, stmt.DeepModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, stmt.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 12, stmt.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, stmt.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 25, stmt.Position.EndCol);
		}

		[Test]
		public void testIfStatement2()
		{
			//Preparation
			string fileString = "{int a, b, c; if(a==1){if(3==b){c--;}}}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			BlockStatement block = BlockStatement.parse(t) as BlockStatement;
			Statement s = block.Statements[1];

			//General tests
            Assertion.Assert("Expected IfStatement, but got something else!", s is IfStatement);
			IfStatement stmt = s as IfStatement;
			Assertion.Assert("Expected EqualityExpression, but got something else!", stmt.Expression is EqualityExpression);
			Assertion.Assert("Expected BlockStatement, but got something else!", stmt.IfPartStatement is BlockStatement);
			BlockStatement body = stmt.IfPartStatement as BlockStatement;
			Assertion.Assert("Expected IfStatement, but got something else!", body.Statements[0] is IfStatement);
			IfStatement innerStatement = body.Statements[0] as IfStatement;
			Assertion.Assert("Expected BlockStatement, but got something else!", innerStatement.IfPartStatement is BlockStatement);
			AssertEquals("Pretty Print string is wrong!", " if(a==1){if(3==b){c--;}}", s.Generate());

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 8, stmt.Expressions.Count);
			AssertEquals("Number of declared variables is wrong!", 0, stmt.DeclaredVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 0, stmt.ReferencedVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 3, stmt.DeepReferencedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 0, stmt.ModifiedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 1, stmt.DeepModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, stmt.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 15, stmt.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, stmt.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 38, stmt.Position.EndCol);
		}

		[Test]
		public void testIfStatement3()
		{
			//Preparation
			string fileString = "{int a, b; if(a==1){ if(b){}} else {--a;}}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			BlockStatement block = BlockStatement.parse(t) as BlockStatement;
			Statement s = block.Statements[1];

			//General tests
			Assertion.Assert("Expected IfStatement, but got something else!", s is IfStatement);
			IfStatement stmt = s as IfStatement;
			Assertion.Assert("Expected EqualityExpression, but got something else!", stmt.Expression is EqualityExpression);
			Assertion.Assert("Expected BlockStatement, but got something else!", stmt.IfPartStatement is BlockStatement);
			BlockStatement body = stmt.IfPartStatement as BlockStatement;
			Assertion.Assert("Expected IfStatement, but got something else!", body.Statements[0] is IfStatement);
			IfStatement innerStatement = body.Statements[0] as IfStatement;
			Assertion.Assert("Expected BlockStatement, but got something else!", innerStatement.IfPartStatement is BlockStatement);
			Assertion.Assert("Expected BlockStatement, but got something else!", stmt.ElsePartStatement is BlockStatement);
			BlockStatement elseBlock = stmt.ElsePartStatement as BlockStatement;
			Assertion.Assert("Expected ExpressionStatement but got something else!", elseBlock.Statements[0] is ExpressionStatement);
			AssertEquals("Pretty Print string is wrong!", " if(a==1){ if(b){}} else {--a;}", s.Generate());

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 6, stmt.Expressions.Count);
			AssertEquals("Number of declared variables is wrong!", 0, stmt.DeclaredVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 0, stmt.ReferencedVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 2, stmt.DeepReferencedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 0, stmt.ModifiedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 1, stmt.DeepModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, stmt.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 12, stmt.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, stmt.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 41, stmt.Position.EndCol);
		}

		[Test]
		public void testIfStatement4()
		{
			//Preparation
			string fileString = "if (m_coll.Count==0) return;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected IfStatement, but got something else!", s is IfStatement);
			IfStatement stmt = s as IfStatement;
			AssertEquals("Pretty Print string is wrong!", fileString, s.Generate());

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 4, s.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, s.DeclaredVariables.Count);
		}

		[Test]
		public void testIfStatement5()
		{
			//Preparation
			string fileString = "if(a){;} else if (b) return;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected IfStatement, but got something else!", s is IfStatement);
			IfStatement stmt = s as IfStatement;
			AssertEquals("Pretty Print string is wrong!", fileString, s.Generate());
		}

		[Test]
		public void testWhileStatement1()
		{
			//Preparation
			string fileString = "{bool a = true; while(a){a=false;}}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			BlockStatement block = BlockStatement.parse(t) as BlockStatement;
			Statement s = block.Statements[1];

			//General tests
			Assertion.Assert("Expected WhileStatement, but got something else!", s is WhileStatement);
			WhileStatement stmt = s as WhileStatement;
			Assertion.Assert("Expression is the wrong type!", stmt.Expression is Identifier);
			Assertion.Assert("Expected BlockStatement, but got something else!", stmt.Statement is BlockStatement);
			AssertEquals("Pretty Print string is wrong!", " while(a){a=false;}", s.Generate());

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 4, stmt.Expressions.Count);
			AssertEquals("Number of declared variables is wrong!", 0, stmt.DeclaredVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 1, stmt.ReferencedVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 1, stmt.DeepReferencedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 0, stmt.ModifiedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 1, stmt.DeepModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 17, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 34, s.Position.EndCol);
		}

		[Test]
		public void testWhileStatement2()
		{
			//Preparation
			string fileString = "while(a != 9){ while(false) ;}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected WhileStatement, but got something else!", s is WhileStatement);
			WhileStatement ws = s as WhileStatement;
			Assertion.Assert("Expression is the wrong type!", ws.Expression is EqualityExpression);
			Assertion.Assert("Expected BlockStatement, but got something else!", ws.Statement is BlockStatement);
			AssertEquals("Pretty Print string is wrong!", "while(a != 9){ while(false) ;}", s.Generate());

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 4, s.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, s.DeclaredVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 30, s.Position.EndCol);
		}

		[Test]
		public void testWhileStatement3()
		{
			//Preparation
			string fileString = "while (((line[i] == ' ') || (line[i] == '\\t')) && (spacesRemoved <= spacesToRemove)){}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected WhileStatement, but got something else!", s is WhileStatement);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, s.DeclaredVariables.Count);
			AssertEquals("Pretty Print string is wrong!", fileString, s.Generate());
		}

		[Test]
		public void testDoStatement1()
		{
			//Preparation
			string fileString = "{bool a = true; do{a=false;}while(a);}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			BlockStatement block = BlockStatement.parse(t) as BlockStatement;
			Statement s = block.Statements[1];

			//General tests
			Assertion.Assert("Expected DoStatement, but got something else!", s is DoStatement);
			DoStatement stmt = s as DoStatement;
			Assertion.Assert("Expression is the wrong type!", stmt.Expression is Identifier);
			Assertion.Assert("Expected BlockStatement, but got something else!", stmt.Statement is BlockStatement);
			AssertEquals("Pretty Print string is wrong!", fileString, block.Generate());

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 4, stmt.Expressions.Count);
			AssertEquals("Number of declared variables is wrong!", 0, stmt.DeclaredVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 1, stmt.ReferencedVariables.Count);
			AssertEquals("Number of referenced variables is wrong!", 1, stmt.DeepReferencedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 0, stmt.ModifiedVariables.Count);
			AssertEquals("Number of modified variables is wrong!", 1, stmt.DeepModifiedVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 17, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 37, s.Position.EndCol);
		}

		[Test]
		public void testForeachStatement1()
		{
			//Preparation
			string fileString = "foreach(SomeType a in SomeObject) ;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected ForeachStatement, but got something else!", s is ForeachStatement);
			ForeachStatement fs = s as ForeachStatement;
			Assertion.AssertEquals("Type name is wrong!", "SomeType", fs.Type.Name);
			Assertion.AssertEquals("Identifier name is wrong!", "a", fs.Identifier.Name);
			Assertion.Assert("Expression is the wrong type!", fs.Expression is Identifier);
			Assertion.Assert("Expected EmptyStatement, but got something else!", fs.Statement is EmptyStatement);
			AssertEquals("Pretty Print string is wrong!", "foreach(SomeType a in SomeObject) ;", s.Generate());

			//Variable and expression tests
			Assertion.AssertEquals("Number of Expressions in statement is wrong!", 1, s.Expressions.Count);
			Assertion.AssertEquals("Number of declared variables is wrong!", 1, s.DeclaredVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 35, s.Position.EndCol);
		}
		
		[Test]
		public void testForeachStatement2()
		{
			string fileString = "foreach (object obj in m_SelectedNodes);";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected ForeachStatement, but got something else!", s is ForeachStatement);
			ForeachStatement fs = s as ForeachStatement;
			Assertion.AssertEquals("Number of declared variables is wrong!", 1, s.DeclaredVariables.Count);
		}

		[Test]
		public void testForStatement1()
		{
			//Preparation
			string fileString = "for(int a=3; a<4; a++) ;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected ForStatement, but got something else!", s is ForStatement);
			ForStatement fs = s as ForStatement;
			Assertion.Assert("Expected EmptyStatement, but got something else!", fs.Statement is EmptyStatement);
			AssertEquals("Pretty Print string is wrong!", fileString, s.Generate());

			//Variable and expression tests
			Assertion.AssertEquals("Number of declared variables is wrong!", 1, s.DeclaredVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 24, s.Position.EndCol);
		}

		[Test]
		public void testForStatement2()
		{
			//Preparation
			string fileString = "for (pil=m_items; pil.m_pi!=null; pil=pil.m_next){}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected ForStatement, but got something else!", s is ForStatement);
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, s.DeclaredVariables.Count);
			AssertEquals("Pretty Print string is wrong!", fileString, s.Generate());
		}

		[Test]
		public void testForStatement3()
		{
			//Preparation
			string fileString = "for (;(ch=m_lexer.GetChar())!=0;){}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected ForStatement, but got something else!", s is ForStatement);
			AssertEquals("Pretty Print string is wrong!", fileString, s.Generate());

			//Variable and expression tests
			Assertion.AssertEquals("Number of declared variables is wrong!", 0, s.DeclaredVariables.Count);
		}

		[Test]
		public void testBreakStatement1()
		{
			//Preparation
			string fileString = "break;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected BreakStatement, but got something else!", s is BreakStatement);
			AssertEquals("Pretty Print string is wrong!", fileString, s.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 6, s.Position.EndCol);
		}

		[Test]
		public void testContinueStatement1()
		{
			//Preparation
			string fileString = "continue;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected ContinueStatement, but got something else!", s is ContinueStatement);
			AssertEquals("Pretty Print string is wrong!", fileString, s.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 9, s.Position.EndCol);
		}

		[Test]
		public void testReturnStatement1()
		{
			//Preparation
			string fileString = "return x;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected ReturnStatement, but got something else!", s is ReturnStatement);
			ReturnStatement r = s as ReturnStatement;
			Assertion.Assert("Expression is the wrong type!", r.Expression is Identifier);
			Identifier identifier = r.Expression as Identifier;
			Assertion.AssertEquals("Identifier name is wrong!", "x", identifier.Name);
			AssertEquals("Pretty Print string is wrong!", "return x;", s.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 9, s.Position.EndCol);
		}

		[Test]
		public void testGotoStatement1()
		{
			//Preparation
			string fileString = "goto x;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected GotoStatement, but got something else!", s is GotoStatement);
			GotoStatement g = s as GotoStatement;
			Assertion.Assert("Expected Identifier, but got something else!", g.Expression is Identifier);
			Identifier id = g.Expression as Identifier;
			Assertion.AssertEquals("Identifier name is wrong!", "x", id.Name);
			AssertEquals("Pretty Print string is wrong!", "goto x;", s.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 7, s.Position.EndCol);
		}

		[Test]
		public void testGotoStatement2()
		{
			//Preparation
			string fileString = "goto case 3;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected GotoCaseStatement, but got something else!", s is GotoCaseStatement);
			GotoCaseStatement g = s as GotoCaseStatement;
			Assertion.Assert("Expected IntegerLiteral, but got something else!", g.Expression is IntegerLiteral);
			AssertEquals("Pretty Print string is wrong!", "goto case 3;", s.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 12, s.Position.EndCol);
		}

		[Test]
		public void testGotoStatement3()
		{
			//Preparation
			string fileString = "goto default;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected GotoDefaultCaseStatement, but got something else!", s is GotoDefaultCaseStatement);
			AssertEquals("Pretty Print string is wrong!", "goto default;", s.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 13, s.Position.EndCol);
		}

		[Test]
		public void testThrowStatement()
		{
			//Preparation
			string fileString = "throw x;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected ThrowStatement, but got something else!", s is ThrowStatement);
			ThrowStatement r = s as ThrowStatement;
			Assertion.Assert("Expression is the wrong type!", r.Expression is Identifier);
			Identifier identifier = r.Expression as Identifier;
			Assertion.AssertEquals("Identifier name is wrong!", "x", identifier.Name);
			Assertion.AssertEquals("PrettyPrint is wrong!", fileString, s.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 8, s.Position.EndCol);
		}

		[Test]
		public void testCheckedStatement1()
		{
			//Preparation
			string fileString = "checked{;}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected CheckedStatement, but got something else!", s is CheckedStatement);
			CheckedStatement stmt = s as CheckedStatement;
			Assertion.AssertEquals("Block statement count is wrong!", 1, stmt.Block.Statements.Count);
			AssertEquals("Pretty Print string is wrong!", fileString, s.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 10, s.Position.EndCol);
		}

		[Test]
		public void testUnheckedStatement1()
		{
			//Preparation
			string fileString = "unchecked{;}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected UncheckedStatement, but got something else!", s is UncheckedStatement);
			UncheckedStatement stmt = s as UncheckedStatement;
			Assertion.AssertEquals("Block statement count is wrong!", 1, stmt.Block.Statements.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 12, s.Position.EndCol);
		}

		[Test]
		public void testLockStatement1()
		{
			//Preparation
			string fileString = "lock(a) ;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected LockStatement, but got something else!", s is LockStatement);
			LockStatement stmt = s as LockStatement;
			Assertion.Assert("Inner statement is the wrong type!", stmt.Statement is EmptyStatement);
			Assertion.AssertEquals("PrettyPrint is wrong!", fileString, s.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 9, s.Position.EndCol);
		}

		[Test]
		public void testUsingStatement1()
		{
			//Preparation
			string fileString = "using(int a = 3) ;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected UsingStatement, but got something else!", s is UsingStatement);
			UsingStatement stmt = s as UsingStatement;
			Assertion.Assert("Inner statement is the wrong type!", stmt.Statement is EmptyStatement);
			Assertion.AssertEquals("PrettyPrint string is wrong!", fileString, stmt.Generate());

			//Variable and expression tests
			Assertion.AssertEquals("Number of declared variables is wrong!", 1, stmt.DeclaredVariables.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 18, s.Position.EndCol);
		}

		[Test]
		public void testUsingStatement2()
		{
			//Preparation
			string fileString = "using(a) ;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected UsingStatement, but got something else!", s is UsingStatement);
			UsingStatement stmt = s as UsingStatement;
			Assertion.Assert("Inner statement is the wrong type!", stmt.Statement is EmptyStatement);
			Assertion.AssertEquals("PrettyPrint string is wrong!", fileString, stmt.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 10, s.Position.EndCol);
		}

		[Test]
		public void testSwitchLabel1()
		{
			//Preparation
			string fileString = "case 1:";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			SwitchLabel sl = SwitchLabel.parse(t);

			//General tests
			Assertion.Assert("Expected SwitchLabel but got MissingSwitchLabel!", false == (sl is MissingSwitchLabel));

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, sl.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, sl.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, sl.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 7, sl.Position.EndCol);
		}

		[Test]
		public void testSwitchLabelColection1()
		{
			//Preparation
			string fileString = "case 1: case 2: case 3:";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			SwitchLabelCollection col = SwitchLabelCollection.parse(t);

			//General tests
			Assertion.AssertEquals("Count in wrong!", 3, col.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, col.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, col.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, col.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 23, col.Position.EndCol);
		}

		[Test]
		public void testSwitchSection1()
		{
			//Preparation
			string fileString = "case 1: ; break;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			SwitchSection ss = SwitchSection.parse(t);

			//General tests
			Assertion.Assert("Expected SwitchSection but got MissingSwitchLabel!", false == (ss is MissingSwitchSection));

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, ss.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, ss.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, ss.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 16, ss.Position.EndCol);
		}

		[Test]
		public void testSwitchSectionColection1()
		{
			//Preparation
			string fileString = "case 1: break; case 2: ; break; case 3: ;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			SwitchSectionCollection col = SwitchSectionCollection.parse(t);

			//General tests
			Assertion.AssertEquals("Count in wrong!", 3, col.Count);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, col.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, col.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, col.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 41, col.Position.EndCol);
		}

		[Test]
		public void testSwitchBlock1()
		{
			//Preparation
			string fileString = "{case 1: break;}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			SwitchBlock sb = SwitchBlock.parse(t);

			//General tests
			Assertion.Assert("Expected SwitchBlock but got MissingSwitchBlock!", false == (sb is MissingSwitchBlock));

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, sb.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, sb.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, sb.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 16, sb.Position.EndCol);
		}

		[Test]
		public void testSwitchStatement1()
		{
			//Preparation
			string fileString = "switch(a){case 1: break;}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//PrettyPrint test
			AssertEquals("Pretty print string was wrong!", fileString, s.Generate());

			//General tests
			Assertion.Assert("Expected SwitchStatement, but got something else!", s is SwitchStatement);
			SwitchStatement stmt = s as SwitchStatement;

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, stmt.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, stmt.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, stmt.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 25, stmt.Position.EndCol);
		}

		[Test]
		public void testSwitchStatement2()
		{
			//Preparation
			string fileString = "switch(yytext[p]) {case 'n' : ns += '\n'; break; default: ns += yytext[p]; break;}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected SwitchStatement, but got something else!", s is SwitchStatement);
		}

		[Test]
		public void testBlockStatement1()
		{
			//Preparation
			string fileString = "{if(b);;}";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected BlockStatement, but got something else!", s is BlockStatement);
			BlockStatement block = s as BlockStatement;
			Assertion.AssertEquals("Block statement count is wrong!", 2, block.Statements.Count);
			Assertion.Assert("Expected IfStatement, but got something else!", block.Statements[0] is IfStatement);
			Assertion.Assert("Expected EmptyStatement, but got something else!", block.Statements[1] is EmptyStatement);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, block.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, block.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, block.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 9, block.Position.EndCol);
		}

		[Test]
		public void testEmptyStatement1()
		{
			//Preparation
			string fileString = ";";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected EmptyStatement, but got something else!", s is EmptyStatement);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 1, s.Position.EndCol);
		}

		[Test]
		public void testLabeledStatement1()
		{
			//Preparation
			string fileString = "theLabel: ;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected LabeledStatement, but got something else!", s is LabeledStatement);
			LabeledStatement labeledStatement = s as LabeledStatement;
			Assertion.AssertEquals("Identifier is wrong!", "theLabel", labeledStatement.Identifier.Name);
			Assertion.Assert("Statement is the wrong type!", labeledStatement.Statement is EmptyStatement);

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 11, s.Position.EndCol);
		}

		[Test]
		public void testDeclarationStatement1()
		{
			//Preparation
			string fileString = "int a;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected DeclarationStatement, but got something else!", s is DeclarationStatement);
			DeclarationStatement stmt = s as DeclarationStatement;
			AssertEquals("Pretty Print string is wrong!", "int a;", stmt.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 6, s.Position.EndCol);
		}

		[Test]
		public void testDeclarationStatement2()
		{
			//Preparation
			string fileString = "int a = 3;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected DeclarationStatement, but got something else!", s is DeclarationStatement);
			DeclarationStatement stmt = s as DeclarationStatement;
			Assertion.AssertEquals("Variable declarator count is wrong!", 1, stmt.LocalVariableDeclaration.VariableDeclarators.Count);
			VariableDeclarator vd1 = stmt.LocalVariableDeclaration.VariableDeclarators[0];
			Assertion.AssertEquals("Variable declarator name is wrong!", "a", vd1.Name);
			Assertion.Assert("Variable initializer is the wrong type!", vd1.Initializer is IntegerLiteral);
			AssertEquals("Pretty Print string is wrong!", "int a = 3;", s.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 10, s.Position.EndCol);
		}

		[Test]
		public void testDeclarationStatement3()
		{
			//Preparation
			string fileString = "TreeNode n0 = m_coll[0] as TreeNode;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected DeclarationStatement, but got something else!", s is DeclarationStatement);
			AssertEquals("Pretty Print string is wrong!", "TreeNode n0 = m_coll[0] as TreeNode;", s.Generate());
		}

		[Test]
		public void testDeclarationStatement4()
		{
			//Preparation
			string fileString = "CSymbol s = (new CSymbol(yyp)).Resolve();";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected DeclarationStatement, but got something else!", s is DeclarationStatement);
			AssertEquals("Pretty Print string is wrong!", "CSymbol s = (new CSymbol(yyp)).Resolve();", s.Generate());
		}

		[Test]
		public void testDeclarationStatement5()
		{
			//Preparation
			string fileString = "OpnieuwConfiguration.RefactoringRow[] refactorings = configuration.RefactoringList[0].GetRefactoringRows();";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected DeclarationStatement, but got something else!", s is DeclarationStatement);
			DeclarationStatement stmt = s as DeclarationStatement;
			AssertEquals("Pretty Print string is wrong!", "OpnieuwConfiguration.RefactoringRow[] refactorings = configuration.RefactoringList[0].GetRefactoringRows();", s.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 107, s.Position.EndCol);
		}

		[Test]
		public void testDeclarationStatementWithConstant()
		{
			//Preparation
			string fileString = "const int a = 3;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected DeclarationStatement, but got something else!", s is DeclarationStatement);
			DeclarationStatement stmt = s as DeclarationStatement;
			Assertion.AssertEquals("Variable declarator count is wrong!", 1, stmt.LocalConstantDeclaration.VariableDeclarators.Count);
			VariableDeclarator vd1 = stmt.LocalConstantDeclaration.VariableDeclarators[0];
			Assertion.AssertEquals("Variable declarator name is wrong!", "a", vd1.Name);
			Assertion.Assert("Variable initializer is the wrong type!", vd1.Initializer is IntegerLiteral);
			AssertEquals("Pretty Print string is wrong!", "const int a = 3;", s.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 16, s.Position.EndCol);
		}

		[Test]
		public void testExpressionStatement1()
		{
			//Preparation
			string fileString = "a += 3;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected ExpressionStatement, but got something else!", s is ExpressionStatement);
			ExpressionStatement stmt = s as ExpressionStatement;
			Assertion.Assert("Statement expression is the wrong type!", stmt.Expression is AssignmentExpression);
			AssertEquals("Pretty Print string is wrong!", "a += 3;", s.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 7, s.Position.EndCol);
		}

		[Test]
		public void testExpressionStatement2()
		{
			//Preparation
			string fileString = "System.Windows.Forms.MessageBox(\"Helo World!\");";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected ExpressionStatement, but got something else!", s is ExpressionStatement);
			ExpressionStatement stmt = s as ExpressionStatement;
			Assertion.Assert("Statement expression is the wrong type!", stmt.Expression is InvocationExpression);
			AssertEquals("Pretty Print string is wrong!", "System.Windows.Forms.MessageBox(\"Helo World!\");", s.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 47, s.Position.EndCol);
		}

		[Test]
		public void testExpressionStatement3()
		{
			//Preparation
			string fileString = "new SomeClass();";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected ExpressionStatement, but got something else!", s is ExpressionStatement);
			ExpressionStatement stmt = s as ExpressionStatement;
			Assertion.Assert("Statement expression is the wrong type!", stmt.Expression is ObjectCreationExpression);
			AssertEquals("Pretty Print string is wrong!", "new SomeClass();", s.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 16, s.Position.EndCol);
		}

		[Test]
		public void testExpressionStatement4()
		{
			//Preparation
			string fileString = "a++;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected ExpressionStatement, but got something else!", s is ExpressionStatement);
			ExpressionStatement stmt = s as ExpressionStatement;
			Assertion.Assert("Statement expression is the wrong type!", stmt.Expression is PostIncrementExpression);
			AssertEquals("Pretty Print string is wrong!", fileString, s.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 4, s.Position.EndCol);
		}

		[Test]
		public void testExpressionStatement5()
		{
			//Preparation
			string fileString = "a--;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected ExpressionStatement, but got something else!", s is ExpressionStatement);
			ExpressionStatement stmt = s as ExpressionStatement;
			Assertion.Assert("Statement expression is the wrong type!", stmt.Expression is PostDecrementExpression);
			AssertEquals("Pretty Print string is wrong!", fileString, s.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 4, s.Position.EndCol);
		}

		[Test]
		public void testExpressionStatement6()
		{
			//Preparation
			string fileString = "++a;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected ExpressionStatement, but got something else!", s is ExpressionStatement);
			ExpressionStatement stmt = s as ExpressionStatement;
			Assertion.Assert("Statement expression is the wrong type!", stmt.Expression is PreIncrementExpression);
			AssertEquals("Pretty Print string is wrong!", fileString, s.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 4, s.Position.EndCol);
		}

		[Test]
		public void testExpressionStatement7()
		{
			//Preparation
			string fileString = "--a;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected ExpressionStatement, but got something else!", s is ExpressionStatement);
			ExpressionStatement stmt = s as ExpressionStatement;
			Assertion.Assert("Statement expression is the wrong type!", stmt.Expression is PreDecrementExpression);
			AssertEquals("Pretty Print string is wrong!", fileString, s.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 4, s.Position.EndCol);
		}

		[Test]
		public void testExpressionStatement8()
		{
			//Preparation
			string fileString = "this.BrowserTree ++;";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected ExpressionStatement, but got something else!", s is ExpressionStatement);
			ExpressionStatement stmt = s as ExpressionStatement;
			Assertion.Assert("Statement expression is the wrong type!", stmt.Expression is PostIncrementExpression);
			AssertEquals("Pretty Print string is wrong!", "this.BrowserTree ++;", s.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 20, s.Position.EndCol);
		}


		[Test]
		public void testExpressionStatement9()
		{
			//Preparation
			string fileString = "refactorings = configuration.RefactoringList[0].GetRefactoringRows();";
			Tokenizer t = TokenizeTestFile(fileString);
			t.nextToken();

			//Execution
			Statement s = Statement.parse(t);

			//General tests
			Assertion.Assert("Expected ExpressionStatement, but got something else!", s is ExpressionStatement);
			ExpressionStatement stmt = s as ExpressionStatement;
			Assertion.Assert("Statement expression is the wrong type!", stmt.Expression is AssignmentExpression);
			AssertEquals("Pretty Print string is wrong!", "refactorings = configuration.RefactoringList[0].GetRefactoringRows();", s.Generate());

			//Position tests
			Assertion.AssertEquals("StartLine is wrong!", 1, s.Position.StartLine);
			Assertion.AssertEquals("StartCol is wrong!", 1, s.Position.StartCol);
			Assertion.AssertEquals("EndLine is wrong!", 1, s.Position.EndLine);
			Assertion.AssertEquals("EndCol is wrong!", 69, s.Position.EndCol);
		}
	}
}
