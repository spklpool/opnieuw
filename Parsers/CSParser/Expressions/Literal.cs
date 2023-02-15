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
using Opnieuw.Framework;

namespace Opnieuw.Parsers.CSParser
{
	public class Literal : Expression
	{
		public static bool parse(TokenProvider tokenizer, ref Expression expression)
		{
			if (Token.TRUE == tokenizer.CurrentToken.Type) {
				expression = new BoolLiteral(tokenizer.CurrentToken, true);
				tokenizer.nextToken();
				return true;
			} else if (Token.FALSE == tokenizer.CurrentToken.Type) {
				expression = new BoolLiteral(tokenizer.CurrentToken, false);
				tokenizer.nextToken();
				return true;
			} else if (Token.LITERAL_CHARACTER == tokenizer.CurrentToken.Type) {
				expression = new CharLiteral(tokenizer.CurrentToken, (char)tokenizer.CurrentToken.Value);
				tokenizer.nextToken();
				return true;
			} else if (Token.LITERAL_INTEGER == tokenizer.CurrentToken.Type) {
				expression = new IntegerLiteral(tokenizer.CurrentToken, tokenizer.CurrentToken.Value);
				tokenizer.nextToken();
				return true;
			} else if (Token.LITERAL_DOUBLE == tokenizer.CurrentToken.Type) {
				expression = new RealLiteral(tokenizer.CurrentToken, (double)tokenizer.CurrentToken.Value);
				tokenizer.nextToken();
				return true;
			} else if (Token.LITERAL_FLOAT == tokenizer.CurrentToken.Type) {
				expression = new RealLiteral(tokenizer.CurrentToken, (float)tokenizer.CurrentToken.Value);
				tokenizer.nextToken();
				return true;
			} else if (Token.LITERAL_DECIMAL == tokenizer.CurrentToken.Type) {
				expression = new DecimalLiteral(tokenizer.CurrentToken, (decimal)tokenizer.CurrentToken.Value);
				tokenizer.nextToken();
				return true;
			} else if (Token.LITERAL_STRING == tokenizer.CurrentToken.Type)	{
				expression = new StringLiteral(tokenizer.CurrentToken, (string)tokenizer.CurrentToken.Value);
				tokenizer.nextToken();
				return true;
			} else if (Token.NULL == tokenizer.CurrentToken.Type) {
				expression = new NullLiteral(tokenizer.CurrentToken);
				tokenizer.nextToken();
				return true;
			}
			return false;
		}

		public Literal(PositionToken token) :
		base(token)
		{
		}

		public PositionToken LiteralToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}

		public override string Generate()
		{
			return LiteralToken.Generate();
		}
	}

	public class BoolLiteral : Literal
	{
		public BoolLiteral(PositionToken token, bool val) :
		base(token)
		{
			m_Value = val;
		}

		protected bool m_Value = false;
		public bool Value {
			get {
				return m_Value;
			}
		}
	}

	public class CharLiteral : Literal
	{
		public CharLiteral(PositionToken token, char val) :
		base(token)
		{
			m_Value = val;
		}

		protected char m_Value = ' ';
		public char Value {
			get {
				return m_Value;
			}
		}

		public override string Generate()
		{
			System.Text.StringBuilder ret = new System.Text.StringBuilder();
			ret.Append(LiteralToken.CommentTokens.Generate());
			ret.Append(LiteralToken.LeadingCharacters);
			ret.Append("'" + ReplaceWithEscape(m_Value) + "'");
			return ret.ToString();
		}
	}

	public class IntegerLiteral : Literal
	{
		public IntegerLiteral(PositionToken token, object val) : 
		base(token)
		{
			m_Value = val;
		}

		protected object m_Value = 0;
		public object Value {
			get {
				return m_Value;
			}
		}
	}

	public class RealLiteral : Literal
	{
		public RealLiteral(PositionToken token, double val) :
		base(token)
		{
			m_Value = val;
		}

		protected double m_Value = 0.0;
		public double Value {
			get {
				return m_Value;
			}
		}
	}

	public class DecimalLiteral : Literal
	{
		public DecimalLiteral(PositionToken token, decimal val) :
		base(token)
		{
			m_Value = val;
		}

		protected decimal m_Value = 0m;
		public decimal Value {
			get {
				return m_Value;
			}
		}
	}

	public class StringLiteral : Literal
	{
		public StringLiteral(PositionToken token, string val) :
		base(token)
		{
			m_Value = val;
		}

		protected string m_Value = "";
		public string Value {
			get {
				return m_Value;
			}
		}
		
		public override string Generate()
		{
			System.Text.StringBuilder ret = new System.Text.StringBuilder();
			ret.Append(LiteralToken.CommentTokens.Generate());
			ret.Append(LiteralToken.LeadingCharacters);
			ret.Append("\"" + ReplaceWithEscapes(m_Value) + "\"");
			return ret.ToString();
		}
	}

	public class NullLiteral : Literal
	{
		public NullLiteral(PositionToken token) :
		base(token)
		{
		}
	}
}
