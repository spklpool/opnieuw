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
using System.Collections;

namespace Opnieuw.Parsers.CSParser
{
	public class Token 
	{
		public const int EOF = 1;
		public const int NONE = 2;
		public const int ERROR = 3;
		public const int ABSTRACT = 4;
		public const int AS = 5;
		public const int ADD = 6;
		public const int ASSEMBLY = 7;
		public const int BASE = 8;
		public const int BOOL = 9;
		public const int BREAK = 10;
		public const int BYTE = 11;
		public const int CASE = 12;
		public const int CATCH = 13;
		public const int CHAR = 14;
		public const int CHECKED = 15;
		public const int CLASS = 16;
		public const int CONST = 17;
		public const int CONTINUE = 18;
		public const int DECIMAL = 19;
		public const int DEFAULT = 20;
		public const int DELEGATE = 21;
		public const int DO = 22;
		public const int DOUBLE = 23;
		public const int ELSE = 24;
		public const int ENUM = 25;
		public const int EVENT = 26;
		public const int EXPLICIT = 27;
		public const int EXTERN = 28;
		public const int FALSE = 29;
		public const int FINALLY = 30;
		public const int FIXED = 31;
		public const int FLOAT = 32;
		public const int FOR = 33;
		public const int FOREACH = 34;
		public const int GOTO = 35;
		public const int IF = 36;
		public const int IMPLICIT = 37;
		public const int IN = 38;
		public const int INT = 39;
		public const int INTERFACE = 40;
		public const int INTERNAL = 41;
		public const int IS = 42;
		public const int LOCK = 43;
		public const int LONG = 44;
		public const int NAMESPACE = 45;
		public const int NEW = 46;
		public const int NULL = 47;
		public const int OBJECT = 48;
		public const int OPERATOR = 49;
		public const int OUT = 50;
		public const int OVERRIDE = 51;
		public const int PARAMS = 52;
		public const int PRIVATE = 53;
		public const int PROTECTED = 54;
		public const int PUBLIC = 55;
		public const int READONLY = 56;
		public const int REF = 57;
		public const int RETURN = 58;
		public const int REMOVE = 59;
		public const int SBYTE = 60;
		public const int SEALED = 61;
		public const int SHORT = 62;
		public const int SIZEOF = 63;
		public const int STACKALLOC = 64;
		public const int STATIC = 65;
		public const int STRING = 66;
		public const int STRUCT = 67;
		public const int SWITCH = 68;
		public const int THIS = 69;
		public const int THROW = 70;
		public const int TRUE = 71;
		public const int TRY = 72;
		public const int TYPEOF = 73;
		public const int UINT = 74;
		public const int ULONG = 75;
		public const int UNCHECKED = 76;
		public const int UNSAFE = 77;
		public const int USHORT = 78;
		public const int USING = 79;
		public const int VIRTUAL = 80;
		public const int VOID = 81;
		public const int VOLATILE = 82;
		public const int WHILE = 83;
		public const int GET = 84;
		public const int get = 85;
		public const int SET = 86;
		public const int set = 87;
		public const int OPEN_BRACE = 88;
		public const int CLOSE_BRACE = 89;
		public const int OPEN_BRACKET = 90;
		public const int CLOSE_BRACKET = 91;
		public const int OPEN_PARENS = 92;
		public const int CLOSE_PARENS = 93;
		public const int DOT = 94;
		public const int COMMA = 95;
		public const int COLON = 96;
		public const int SEMICOLON = 97;
		public const int TILDE = 98;
		public const int PLUS = 99;
		public const int MINUS = 100;
		public const int BANG = 101;
		public const int ASSIGN = 102;
		public const int OP_LT = 103;
		public const int OP_GT = 104;
		public const int BITWISE_AND = 105;
		public const int BITWISE_OR = 106;
		public const int STAR = 107;
		public const int PERCENT = 108;
		public const int DIV = 109;
		public const int CARRET = 110;
		public const int INTERR = 111;
		public const int OP_INC = 112;
		public const int OP_DEC = 113;
		public const int OP_SHIFT_LEFT = 114;
		public const int OP_SHIFT_RIGHT = 115;
		public const int OP_LE = 116;
		public const int OP_GE = 117;
		public const int OP_EQ = 118;
		public const int OP_NE = 119;
		public const int OP_AND = 120;
		public const int OP_OR = 121;
		public const int OP_MULT_ASSIGN = 122;
		public const int OP_DIV_ASSIGN = 123;
		public const int OP_MOD_ASSIGN = 124;
		public const int OP_ADD_ASSIGN = 125;
		public const int OP_SUB_ASSIGN = 126;
		public const int OP_SHIFT_LEFT_ASSIGN = 127;
		public const int OP_SHIFT_RIGHT_ASSIGN = 128;
		public const int OP_AND_ASSIGN = 129;
		public const int OP_XOR_ASSIGN = 130;
		public const int OP_OR_ASSIGN = 131;
		public const int OP_PTR = 132;
		public const int LITERAL_INTEGER = 133;
		public const int LITERAL_FLOAT = 134;
		public const int LITERAL_DOUBLE = 135;
		public const int LITERAL_DECIMAL = 136;
		public const int LITERAL_CHARACTER = 137;
		public const int LITERAL_STRING = 138;
		public const int IDENTIFIER = 139;
		public const int SINGLE_LINE_COMMENT = 140;
		public const int DELIMITED_COMMENT = 141;
		public const int FIELD = 142;
		public const int METHOD = 143;
		public const int MODULE = 144;
		public const int PARAM = 145;
		public const int PROPERTY = 146;
		public const int TYPE = 147;

		private Hashtable m_TokenTable = new Hashtable();
		public Token()
		{
			m_TokenTable.Add (Token.ABSTRACT, "abstract");
			m_TokenTable.Add (Token.AS, "as");
			m_TokenTable.Add (Token.ADD, "add");
			m_TokenTable.Add (Token.ASSEMBLY, "assembly");
			m_TokenTable.Add (Token.BASE, "base");
			m_TokenTable.Add (Token.BOOL, "bool");
			m_TokenTable.Add (Token.BREAK, "break");
			m_TokenTable.Add (Token.BYTE, "byte");
			m_TokenTable.Add (Token.CASE, "case");
			m_TokenTable.Add (Token.CATCH, "catch");
			m_TokenTable.Add (Token.CHAR, "char");
			m_TokenTable.Add (Token.CHECKED, "checked");
			m_TokenTable.Add (Token.CLASS, "class");
			m_TokenTable.Add (Token.CONST, "const");
			m_TokenTable.Add (Token.CONTINUE, "continue");
			m_TokenTable.Add (Token.DECIMAL, "decimal");
			m_TokenTable.Add (Token.DEFAULT, "default");
			m_TokenTable.Add (Token.DELEGATE, "delegate");
			m_TokenTable.Add (Token.DO, "do");
			m_TokenTable.Add (Token.DOUBLE, "double");
			m_TokenTable.Add (Token.ELSE, "else");
			m_TokenTable.Add (Token.ENUM, "enum");
			m_TokenTable.Add (Token.EVENT, "event");
			m_TokenTable.Add (Token.EXPLICIT, "explicit");
			m_TokenTable.Add (Token.EXTERN, "extern");
			m_TokenTable.Add (Token.FALSE, "false");
			m_TokenTable.Add (Token.FINALLY, "finally");
			m_TokenTable.Add (Token.FIXED, "fixed");
			m_TokenTable.Add (Token.FLOAT, "float");
			m_TokenTable.Add (Token.FOR, "for");
			m_TokenTable.Add (Token.FOREACH, "foreach");
			m_TokenTable.Add (Token.GOTO, "goto");
			m_TokenTable.Add (Token.GET, "get");
			m_TokenTable.Add (Token.IF, "if");
			m_TokenTable.Add (Token.IMPLICIT, "implicit");
			m_TokenTable.Add (Token.IN, "in");
			m_TokenTable.Add (Token.INT, "int");
			m_TokenTable.Add (Token.INTERFACE, "interface");
			m_TokenTable.Add (Token.INTERNAL, "internal");
			m_TokenTable.Add (Token.IS, "is");
			m_TokenTable.Add (Token.LOCK, "lock");
			m_TokenTable.Add (Token.LONG, "long");
			m_TokenTable.Add (Token.NAMESPACE, "namespace");
			m_TokenTable.Add (Token.NEW, "new");
			m_TokenTable.Add (Token.NULL, "null");
			m_TokenTable.Add (Token.OBJECT, "object");
			m_TokenTable.Add (Token.OPERATOR, "operator");
			m_TokenTable.Add (Token.OUT, "out");
			m_TokenTable.Add (Token.OVERRIDE, "override");
			m_TokenTable.Add (Token.PARAMS, "params");
			m_TokenTable.Add (Token.PRIVATE, "private");
			m_TokenTable.Add (Token.PROTECTED, "protected");
			m_TokenTable.Add (Token.PUBLIC, "public");
			m_TokenTable.Add (Token.READONLY, "readonly");
			m_TokenTable.Add (Token.REF, "ref");
			m_TokenTable.Add (Token.REMOVE, "remove");
			m_TokenTable.Add (Token.RETURN, "return");
			m_TokenTable.Add (Token.SBYTE, "sbyte");
			m_TokenTable.Add (Token.SEALED, "sealed");
			m_TokenTable.Add (Token.SET, "set");
			m_TokenTable.Add (Token.SHORT, "short");
			m_TokenTable.Add (Token.SIZEOF, "sizeof");
			m_TokenTable.Add (Token.STACKALLOC, "stackalloc");
			m_TokenTable.Add (Token.STATIC, "static");
			m_TokenTable.Add (Token.STRING, "string");
			m_TokenTable.Add (Token.STRUCT, "struct");
			m_TokenTable.Add (Token.SWITCH, "switch");
			m_TokenTable.Add (Token.THIS, "this");
			m_TokenTable.Add (Token.THROW, "throw");
			m_TokenTable.Add (Token.TRUE, "true");
			m_TokenTable.Add (Token.TRY, "try");
			m_TokenTable.Add (Token.TYPEOF, "typeof");
			m_TokenTable.Add (Token.UINT, "uint");
			m_TokenTable.Add (Token.ULONG, "ulong");
			m_TokenTable.Add (Token.UNCHECKED, "unchecked");
			m_TokenTable.Add (Token.UNSAFE, "unsafe");
			m_TokenTable.Add (Token.USHORT, "ushort");
			m_TokenTable.Add (Token.USING, "using");
			m_TokenTable.Add (Token.VIRTUAL, "virtual");
			m_TokenTable.Add (Token.VOID, "void");
			m_TokenTable.Add (Token.VOLATILE, "volatile");
			m_TokenTable.Add (Token.WHILE, "while");
//			m_TokenTable.Add (Token.FIELD, "field");
			m_TokenTable.Add (Token.METHOD, "method");
			m_TokenTable.Add (Token.MODULE, "module");
			m_TokenTable.Add (Token.PARAM, "param");
			m_TokenTable.Add (Token.PROPERTY, "property");
			m_TokenTable.Add (Token.OPEN_BRACE, "{");
			m_TokenTable.Add (Token.CLOSE_BRACE, "}");
			m_TokenTable.Add (Token.OPEN_BRACKET, "[");
			m_TokenTable.Add (Token.CLOSE_BRACKET, "]");
			m_TokenTable.Add (Token.OPEN_PARENS, "(");
			m_TokenTable.Add (Token.CLOSE_PARENS, ")");
		}

		public string lookupToken(int token)
		{
			if (m_TokenTable.ContainsKey(token))
			{
				return (string)m_TokenTable[token];
			}
			else
			{
				return "?";
			}
		}
	}
}
