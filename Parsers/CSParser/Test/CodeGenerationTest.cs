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
	public class CodeGenerationTest : ParserTest
    {
        [Test]
        public void testAssignmentExpression1()
        {
            AssignmentExpression expression = new AssignmentExpression(Identifier.parse("i"), Literal.parse("2"));
            expression.Format();
            string result = expression.Generate();
            AssertEquals("Result is wrong!", "i = 2", result);
        }

        [Test]
        public void testUsingStatement1()
        {
            LocalVariableDeclaration declaration = new LocalVariableDeclaration(DataType.parse("int"), new VariableDeclaratorCollection(new VariableDeclarator(Identifier.parse("i"), Literal.parse("0"))));
            EmptyStatement innerStatement = new EmptyStatement();
            UsingStatement statement = new UsingStatement(declaration, innerStatement);
            statement.Format();
            string result = statement.Generate();
            System.Text.StringBuilder expected = new System.Text.StringBuilder();
            expected.Append("\r\n");
            expected.Append("using (int i = 0)\r\n");
            expected.Append("\t;");
            AssertEquals("Result is wrong!", expected.ToString(), result);
        }

        [Test]
        public void testUsingStatement2()
        {
            LocalVariableDeclaration declaration = new LocalVariableDeclaration(DataType.parse("int"), new VariableDeclaratorCollection(new VariableDeclarator(Identifier.parse("i"), Literal.parse("0"))));
            Statement innerStatement = new BlockStatement(new StatementCollection(new EmptyStatement()));
            UsingStatement statement = new UsingStatement(declaration, innerStatement);
            statement.Format();
            string result = statement.Generate();
            System.Text.StringBuilder expected = new System.Text.StringBuilder();
            expected.Append("\r\n");
            expected.Append("using (int i = 0) {\r\n");
            expected.Append("\t;\r\n");
            expected.Append("}");
            AssertEquals("Result is wrong!", expected.ToString(), result);
        }

        [Test]
        public void testLabeledStatement1()
        {
            Statement innerStatement = new EmptyStatement();
            LabeledStatement statement = new LabeledStatement(Identifier.parse("someLabel"), innerStatement);
            statement.Format();
            string result = statement.Generate();
            System.Text.StringBuilder expected = new System.Text.StringBuilder();
            expected.Append("\r\n");
            expected.Append("someLabel:\r\n");
            expected.Append("\t;");
            AssertEquals("Result is wrong!", expected.ToString(), result);
        }

        [Test]
        public void testLabeledStatement2()
        {
            Statement innerStatement = new BlockStatement(new StatementCollection(new EmptyStatement()));
            LabeledStatement statement = new LabeledStatement(Identifier.parse("someLabel"), innerStatement);
            statement.Format();
            string result = statement.Generate();
            System.Text.StringBuilder expected = new System.Text.StringBuilder();
            expected.Append("\r\n");
            expected.Append("someLabel:\r\n");
            expected.Append("{\r\n");
            expected.Append("\t;\r\n");
            expected.Append("}");
            AssertEquals("Result is wrong!", expected.ToString(), result);
        }

        [Test]
        public void testTryStatement1()
        {
            Statement tryStatement = new BlockStatement(new StatementCollection(new EmptyStatement()));
            Statement catchStatement = new BlockStatement(new StatementCollection(new EmptyStatement()));
            CatchClause catchClause = new CatchClause(DataType.parse("Exception"), Identifier.parse("e"), catchStatement);
            CatchClauseCollection catchClauses = new CatchClauseCollection(catchClause);
            TryStatement statement = new TryStatement(tryStatement, catchClauses);
            statement.Format();
            string result = statement.Generate();
            System.Text.StringBuilder expected = new System.Text.StringBuilder();
            expected.Append("\r\n");
            expected.Append("try {\r\n");
            expected.Append("\t;\r\n");
            expected.Append("} catch(Exception e) {\r\n");
            expected.Append("\t;\r\n");
            expected.Append("}");
            AssertEquals("Result is wrong!", expected.ToString(), result);
        }

        [Test]
        public void testTryStatement2()
        {
            Statement tryStatement = new BlockStatement(new StatementCollection(new EmptyStatement()));
            Statement catchStatement = new BlockStatement(new StatementCollection(new EmptyStatement()));
            CatchClause catchClause = new CatchClause(DataType.parse("Exception"), Identifier.parse("e"), catchStatement);
            TryStatement statement = new TryStatement(tryStatement, catchClause);
            statement.Format();
            string result = statement.Generate();
            System.Text.StringBuilder expected = new System.Text.StringBuilder();
            expected.Append("\r\n");
            expected.Append("try {\r\n");
            expected.Append("\t;\r\n");
            expected.Append("} catch(Exception e) {\r\n");
            expected.Append("\t;\r\n");
            expected.Append("}");
            AssertEquals("Result is wrong!", expected.ToString(), result);
        }

        [Test]
        public void testTryStatement3()
        {
            Statement tryStatement = new BlockStatement(new StatementCollection(new EmptyStatement()));
            Statement catchStatement = new BlockStatement(new StatementCollection(new EmptyStatement()));
            Statement finallyStatement = new BlockStatement(new StatementCollection(new EmptyStatement()));
            CatchClause catchClause = new CatchClause(DataType.parse("Exception"), Identifier.parse("e"), catchStatement);
            FinallyClause finallyClause = new FinallyClause(finallyStatement);
            TryStatement statement = new TryStatement(tryStatement, catchClause, finallyClause);
            statement.Format();
            string result = statement.Generate();
            System.Text.StringBuilder expected = new System.Text.StringBuilder();
            expected.Append("\r\n");
            expected.Append("try {\r\n");
            expected.Append("\t;\r\n");
            expected.Append("} catch(Exception e) {\r\n");
            expected.Append("\t;\r\n");
            expected.Append("} finally {\r\n");
            expected.Append("\t;\r\n");
            expected.Append("}");
            AssertEquals("Result is wrong!", expected.ToString(), result);
        }

        [Test]
        public void testDeclarationStatement1()
        {
            LocalVariableDeclaration declaration = new LocalVariableDeclaration(DataType.parse("int"), new VariableDeclaratorCollection(new VariableDeclarator(Identifier.parse("i"), Literal.parse("0"))));
            DeclarationStatement statement = new DeclarationStatement(declaration);
            statement.Format();
            string result = statement.Generate();
            System.Text.StringBuilder expected = new System.Text.StringBuilder();
            expected.Append("\r\n");
            expected.Append("int i = 0;");
            AssertEquals("Result is wrong!", expected.ToString(), result);
        }

        [Test]
        public void testCheckedStatement1()
        {
            BlockStatement innerStatement = new BlockStatement(new StatementCollection(new EmptyStatement()));
            CheckedStatement statement = new CheckedStatement(innerStatement);
            statement.Format();
            string result = statement.Generate();
            System.Text.StringBuilder expected = new System.Text.StringBuilder();
            expected.Append("\r\n");
            expected.Append("checked\r\n");
            expected.Append("{\r\n");
            expected.Append("\t;\r\n");
            expected.Append("}");
            AssertEquals("Result is wrong!", expected.ToString(), result);
        }

        [Test]
        public void testWhileStatement1()
        {
            Statement innerStatement = new EmptyStatement();
            WhileStatement statement = new WhileStatement(Literal.parse("true"), innerStatement);
            statement.Format();
            string result = statement.Generate();
            System.Text.StringBuilder expected = new System.Text.StringBuilder();
            expected.Append("\r\n");
            expected.Append("while (true)\r\n");
            expected.Append("\t;");
            AssertEquals("Result is wrong!", expected.ToString(), result);
        }

        [Test]
        public void testWhileStatement2()
        {
            Statement innerStatement = new BlockStatement(new StatementCollection(new EmptyStatement()));
            WhileStatement statement = new WhileStatement(Literal.parse("true"), innerStatement);
            statement.Format();
            string result = statement.Generate();
            System.Text.StringBuilder expected = new System.Text.StringBuilder();
            expected.Append("\r\n");
            expected.Append("while (true)\r\n");
            expected.Append("{\r\n");
            expected.Append("\t;\r\n");
            expected.Append("}");
            AssertEquals("Result is wrong!", expected.ToString(), result);
        }

        [Test]
        public void testDoStatement1()
        {
            Statement innerStatement = new EmptyStatement();
            DoStatement statement = new DoStatement(innerStatement, Literal.parse("true"));
            statement.Format();
            string result = statement.Generate();
            System.Text.StringBuilder expected = new System.Text.StringBuilder();
            expected.Append("\r\n");
            expected.Append("do\r\n");
            expected.Append("\t;\r\n");
            expected.Append("while (true);");
            AssertEquals("Result is wrong!", expected.ToString(), result);
        }

        [Test]
        public void testDoStatement2()
        {
            Statement innerStatement = new BlockStatement(new StatementCollection(new EmptyStatement()));
            DoStatement statement = new DoStatement(innerStatement, Literal.parse("true"));
            statement.Format();
            string result = statement.Generate();
            System.Text.StringBuilder expected = new System.Text.StringBuilder();
            expected.Append("\r\n");
            expected.Append("do\r\n");
            expected.Append("{\r\n");
            expected.Append("\t;\r\n");
            expected.Append("}\r\n");
            expected.Append("while (true);");
            AssertEquals("Result is wrong!", expected.ToString(), result);
        }

        [Test]
        public void testForeachStatement1()
        {
            Statement innerStatement = new EmptyStatement();
            ForeachStatement statement = new ForeachStatement(DataType.parse("SomeType"), Identifier.parse("someVariable"), Identifier.parse("someOtherVariable"), innerStatement);
            statement.Format();
            string result = statement.Generate();
            System.Text.StringBuilder expected = new System.Text.StringBuilder();
            expected.Append("\r\n");
            expected.Append("foreach (SomeType someVariable in someOtherVariable)\r\n");
            expected.Append("\t;");
            AssertEquals("Result is wrong!", expected.ToString(), result);
        }

        [Test]
        public void testForeachStatement2()
        {
            Statement innerStatement = new BlockStatement(new StatementCollection(new EmptyStatement()));
            ForeachStatement statement = new ForeachStatement(DataType.parse("SomeType"), Identifier.parse("someVariable"), Identifier.parse("someOtherVariable"), innerStatement);
            statement.Format();
            string result = statement.Generate();
            System.Text.StringBuilder expected = new System.Text.StringBuilder();
            expected.Append("\r\n");
            expected.Append("foreach (SomeType someVariable in someOtherVariable)\r\n");
            expected.Append("{\r\n");
            expected.Append("\t;\r\n");
            expected.Append("}");
            AssertEquals("Result is wrong!", expected.ToString(), result);
        }

        [Test]
        public void testForStatement1()
        {
            ForInitializer initializer = new ForInitializer(new LocalVariableDeclaration(DataType.parse("int"), new VariableDeclaratorCollection(new VariableDeclarator(Identifier.parse("i"), new PositionToken(Position.Missing, "=", Token.ASSIGN), Literal.parse("0")))), new ExpressionCollection());
            Expression condition = new EqualityExpression(Identifier.parse("i"), new PositionToken(Position.Missing, "<", Token.OP_LT), Literal.parse("10"));
            ExpressionCollection iterator = new ExpressionCollection();
            iterator.Add(new PostIncrementExpression(Identifier.parse("i")));
            Statement innerStatement = new EmptyStatement();
            ForStatement statement = new ForStatement(initializer, condition, iterator, innerStatement);
            statement.Format();
            string result = statement.Generate();
            System.Text.StringBuilder expected = new System.Text.StringBuilder();
            expected.Append("\r\n");
            expected.Append("for (int i = 0; i<10; i++)\r\n");
            expected.Append("\t;");
            AssertEquals("Result is wrong!", expected.ToString(), result);
        }

        [Test]
        public void testForStatement2()
        {
            ForInitializer initializer = new MissingForInitializer();
            Expression condition = new MissingExpression();
            ExpressionCollection iterator = new ExpressionCollection();
            Statement innerStatement = new BlockStatement(new StatementCollection(new EmptyStatement()));
            ForStatement statement = new ForStatement(initializer, condition, iterator, innerStatement);
            statement.Format();
            string result = statement.Generate();
            System.Text.StringBuilder expected = new System.Text.StringBuilder();
            expected.Append("\r\n");
            expected.Append("for (;;)\r\n");
            expected.Append("{\r\n");
            expected.Append("\t;\r\n");
            expected.Append("}");
            AssertEquals("Result is wrong!", expected.ToString(), result);
        }

        [Test]
        public void testIfStatement1()
        {
            Expression expression = Expression.parse("true");
            Statement ifStatement = new EmptyStatement();
            Statement elseStatement = new EmptyStatement();
            IfStatement statement = new IfStatement(expression, ifStatement, elseStatement);
            statement.Format();
            string result = statement.Generate();
            System.Text.StringBuilder expected = new System.Text.StringBuilder();
            expected.Append("\r\nif (true)\r\n");
            expected.Append("\t;\r\n");
            expected.Append("else\r\n");
            expected.Append("\t;");
            AssertEquals("Result is wrong!", expected.ToString(), result);
        }

        [Test]
        public void testIfStatement2()
        {
            Expression expression = Expression.parse("true");
            Statement ifStatement = new BlockStatement();
            ifStatement.Statements.Add(new EmptyStatement());
            Statement elseStatement = new BlockStatement();
            elseStatement.Statements.Add(new EmptyStatement());
            IfStatement statement = new IfStatement(expression, ifStatement, elseStatement);
            statement.Format();
            string result = statement.Generate();
            System.Text.StringBuilder expected = new System.Text.StringBuilder();
            expected.Append("\r\n");
            expected.Append("if (true) {\r\n");
            expected.Append("\t;\r\n");
            expected.Append("} else {\r\n");
            expected.Append("\t;\r\n");
            expected.Append("}");
            AssertEquals("Result is wrong!", expected.ToString(), result);
        }

        [Test]
        public void testReturnStatement1()
        {
            Statement statement = new ReturnStatement(Expression.parse("true"));
            statement.Format();
            string result = statement.Generate();
            System.Text.StringBuilder expected = new System.Text.StringBuilder();
            AssertEquals("Result is wrong!", "\r\nreturn true;", result);
        }

        [Test]
		public void testInterfaceEmpty()
		{
			Interface interf = new Interface("test1", 
											 new MissingBaseTypeList(), 
											 new InterfaceMemberCollection());
            interf.Format();
			string result = interf.Generate();
			System.Text.StringBuilder expected = new System.Text.StringBuilder();
			expected.Append("\r\ninterface test1\r\n");
			expected.Append("{\r\n");
			expected.Append("}");
			AssertEquals("Result is wrong!", expected.ToString(), result);
		}

		[Test]
		public void testInterfaceWithMethod()
		{
			InterfaceMemberCollection members = new InterfaceMemberCollection();
			InterfaceMethod method = new InterfaceMethod(DataType.parse("bool"), 
														 "methodName", 
														 new MissingFormalParameterList());
			members.Add(method);
			Interface interf = new Interface("test1", 
											 new MissingBaseTypeList(), 
											 members);
            interf.Format();
			string result = interf.Generate();
			System.Text.StringBuilder expected = new System.Text.StringBuilder();
			expected.Append("\r\ninterface test1\r\n");
			expected.Append("{\r\n");
			expected.Append("\tbool methodName();\r\n");
			expected.Append("}");
			AssertEquals("Result is wrong!", expected.ToString(), result);
		}

		[Test]
		public void testInterfaceWithProperty()
		{
			InterfaceMemberCollection members = new InterfaceMemberCollection();
			InterfaceGetAccessor getAccessor = new InterfaceGetAccessor();
			InterfaceSetAccessor setAccessor = new InterfaceSetAccessor();
			InterfaceAccessors accessors = new InterfaceAccessors(getAccessor, setAccessor);
			InterfaceProperty property = new InterfaceProperty(DataType.parse("bool"),  
															   "PropertyName", accessors);
			members.Add(property);
			Interface interf = new Interface("test1", 
											 new MissingBaseTypeList(), 
											 members);
            interf.Format();
			string result = interf.Generate();
			System.Text.StringBuilder expected = new System.Text.StringBuilder();
			expected.Append("\r\ninterface test1\r\n");
			expected.Append("{\r\n");
			expected.Append("\tbool PropertyName {\r\n");
			expected.Append("\t\tget;\r\n");
			expected.Append("\t\tset;\r\n");
			expected.Append("\t}\r\n");
			expected.Append("}");
			AssertEquals("Result is wrong!", expected.ToString(), result);
		}

		[Test]
		public void testInterfaceWithPropertyAndAttributes()
		{
			InterfaceMemberCollection members = new InterfaceMemberCollection();
			InterfaceGetAccessor getAccessor = new InterfaceGetAccessor(new Attribute(QualifiedIdentifier.parse("SomeAttribute")));
            InterfaceSetAccessor setAccessor = new InterfaceSetAccessor(new Attribute(QualifiedIdentifier.parse("SomeAttribute")));
            InterfaceAccessors accessors = new InterfaceAccessors(getAccessor, setAccessor);
			InterfaceProperty property = new InterfaceProperty(DataType.parse("bool"),  
															   "PropertyName", accessors);
			members.Add(property);
			Interface interf = new Interface(new Attribute(QualifiedIdentifier.parse("SomeAttribute")),
                                             "test1", 
											 new MissingBaseTypeList(), 
											 members);
            interf.Format();
			string result = interf.Generate();
			System.Text.StringBuilder expected = new System.Text.StringBuilder();
            expected.Append("\r\n[SomeAttribute]\r\n");
            expected.Append("interface test1\r\n");
			expected.Append("{\r\n");
			expected.Append("\tbool PropertyName {\r\n");
			expected.Append("\t\t[SomeAttribute]\r\n");
			expected.Append("\t\tget;\r\n");
			expected.Append("\t\t[SomeAttribute]\r\n");
			expected.Append("\t\tset;\r\n");
			expected.Append("\t}\r\n");
			expected.Append("}");
			AssertEquals("Result is wrong!", expected.ToString(), result);
        }

        [Test]
        public void testSimpleClass()
        {
            Class cls = new Class("SomeClass");
            cls.Format();
            System.Text.StringBuilder expected = new System.Text.StringBuilder();
            expected.Append("\r\nclass SomeClass\r\n");
            expected.Append("{\r\n");
            expected.Append("}");
            AssertEquals("Result is wrong!", expected.ToString(), cls.Generate());
        }

        [Test]
        public void testClassWithMethod()
        {
            ClassMemberCollection members = new ClassMemberCollection();
            ClassMember member = new MethodDeclaration(DataType.parse("void"), "SomeMethod", new MissingFormalParameterList(), new BlockStatement(new StatementCollection()));
            members.Add(member);
            Class cls = new Class("SomeClass", members);
            cls.Format();
            System.Text.StringBuilder expected = new System.Text.StringBuilder();
            expected.Append("\r\nclass SomeClass\r\n");
            expected.Append("{\r\n");
            expected.Append("\tvoid SomeMethod()\r\n");
            expected.Append("\t{\r\n");
            expected.Append("\t}\r\n");
            expected.Append("}");
            AssertEquals("Result is wrong!", expected.ToString(), cls.Generate());
        }

        [Test]
        public void testClassWithMethodAndAttribute()
        {
            ClassMemberCollection members = new ClassMemberCollection();
            ClassMember member = new MethodDeclaration(new Attribute(QualifiedIdentifier.parse("SomeAttribute")), 
                                                       DataType.parse("void"), 
                                                       "SomeMethod", 
                                                       new MissingFormalParameterList(), 
                                                       new BlockStatement());
            members.Add(member);
            Class cls = new Class("SomeClass", members);
            cls.Format();
            System.Text.StringBuilder expected = new System.Text.StringBuilder();
            expected.Append("\r\nclass SomeClass\r\n");
            expected.Append("{\r\n");
            expected.Append("\t[SomeAttribute]\r\n");
            expected.Append("\tvoid SomeMethod()\r\n");
            expected.Append("\t{\r\n");
            expected.Append("\t}\r\n");
            expected.Append("}");
            AssertEquals("Result is wrong!", expected.ToString(), cls.Generate());
        }

        [Test]
        public void testClassWithProperty()
        {
            ClassMemberCollection members = new ClassMemberCollection();
            Statement getStatements = new BlockStatement();
            getStatements.Statements.Add(new EmptyStatement());
            Statement setStatements = new BlockStatement();
            setStatements.Statements.Add(new EmptyStatement());
            GetAccessorDeclaration get = new GetAccessorDeclaration(new AttributeSectionCollection(), getStatements);
            AccessorDeclaration set = new SetAccessorDeclaration(new AttributeSectionCollection(), setStatements);
            AccessorDeclarations accessors = new AccessorDeclarations(get, set);
            ClassMember member = new PropertyDeclaration(DataType.parse("void"),
                                                         "SomeProperty",
                                                         accessors);
            members.Add(member);
            Class cls = new Class("SomeClass", members);
            cls.Format();
            System.Text.StringBuilder expected = new System.Text.StringBuilder();
            expected.Append("\r\nclass SomeClass\r\n");
            expected.Append("{\r\n");
            expected.Append("\tvoid SomeProperty {\r\n");
            expected.Append("\t\tget {\r\n");
            expected.Append("\t\t\t;\r\n");
            expected.Append("\t\t}\r\n");
            expected.Append("\t\tset {\r\n");
            expected.Append("\t\t\t;\r\n");
            expected.Append("\t\t}\r\n");
            expected.Append("\t}\r\n");
            expected.Append("}");
            AssertEquals("Result is wrong!", expected.ToString(), cls.Generate());
        }
    }
}
