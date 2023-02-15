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
	public class VariousTest : ParserTest
	{
		[Test]
		public void test1()
		{
			//Preparation
			System.Text.StringBuilder fileString = new System.Text.StringBuilder();
			fileString.Append("namespace Opnieuw.Parsers.CSParser\r\n");
			fileString.Append("{\r\n");
			fileString.Append("	public class EnumBase : PieceOfCode\r\n");
			fileString.Append("	{\r\n");
			fileString.Append("		public static EnumBase parse(TokenProvider tokenizer)");
			fileString.Append("		{");
			fileString.Append("			if (Token.COLON == tokenizer.CurrentToken.Type)");
			fileString.Append("			{");
			fileString.Append("				PositionToken colonToken = tokenizer.CurrentToken;");
			fileString.Append("				tokenizer.nextToken();");
			fileString.Append("				DataType type = DataType.parse(tokenizer);");
			fileString.Append("				type.checkNotMissing();");
			fileString.Append("				EnumBase ret = new EnumBase(type, colonToken);");
			fileString.Append("				return ret;");
			fileString.Append("			}");
			fileString.Append("			else");
			fileString.Append("			{");
			fileString.Append("				return new MissingEnumBase();");
			fileString.Append("			}");
			fileString.Append("		}");
			fileString.Append("");
			fileString.Append("		public EnumBase()");
			fileString.Append("		{");
			fileString.Append("		}");
			fileString.Append("");
			fileString.Append("		public EnumBase(DataType type, PositionToken colonToken)");
			fileString.Append("		{");
			fileString.Append("			m_ColonToken = colonToken;");
			fileString.Append("			m_Type = type;");
			fileString.Append("		}");
			fileString.Append("		");
			fileString.Append("		protected PositionToken m_ColonToken = PositionToken.Missing;");
			fileString.Append("		public PositionToken ColonToken {");
			fileString.Append("			get {");
			fileString.Append("				return m_ColonToken;");
			fileString.Append("			}");
			fileString.Append("		}");
			fileString.Append("");
			fileString.Append("		protected DataType m_Type = new MissingDataType();");
			fileString.Append("		public DataType Type {");
			fileString.Append("			get {");
			fileString.Append("				return m_Type;");
			fileString.Append("			}");
			fileString.Append("		}");
			fileString.Append("");
			fileString.Append("		public override string Generate()\r\n");
			fileString.Append("		{\r\n");
			fileString.Append("			System.Text.StringBuilder ret = new System.Text.StringBuilder();\r\n");
			fileString.Append("			ret.Append(m_ColonToken.PrettyPrint());\r\n");
			fileString.Append("			ret.Append(m_Type.PrettyPrint());\r\n");
			fileString.Append("			return ret.ToString();\r\n");
			fileString.Append("		}\r\n");
			fileString.Append("	}\r\n");
			fileString.Append("}");

			//Execution
			CompilationUnit unit = ParseTestFile(fileString.ToString());

			//General tests
			AssertEquals("Pretty Print string is wrong!", fileString.ToString(), unit.Generate());
		}
	}
}
