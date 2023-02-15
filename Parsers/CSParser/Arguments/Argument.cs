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
	public class Argument : PieceOfCode
	{
		/// <summary>
		/// expression
		/// ref variable-reference
		/// out variable-reference 
		/// </summary>
		public static Argument parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			Argument ret = Argument.Missing;
			if ((Token.REF == tokenizer.CurrentToken.Type) || (Token.OUT == tokenizer.CurrentToken.Type))
			{
				PositionToken startToken = tokenizer.CurrentToken;
				tokenizer.nextToken();
				Expression exp = Expression.parse(tokenizer);			
				if (false == (exp is MissingExpression))
				{
					ret = new Argument(startToken, exp);
				}
			}
			else
			{
				Expression exp = Expression.parse(tokenizer);			
				if (false == (exp is MissingExpression))
				{
					ret = new Argument(exp);
				}
			}
			tokenizer.endBookmark(ret is MissingArgument);
			return ret;
		}

		public Argument(Expression expression) :
		base(expression.Position)
		{
			Pieces.Add(PositionToken.Missing);
			Pieces.Add(expression);
		}

		public Argument(PositionToken startToken, Expression expression) :
		base(new Position(startToken, expression))
		{
			Pieces.Add(startToken);
			Pieces.Add(expression);
		}

		public PositionToken StartToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}

		public Expression Expression {
			get {
				return Pieces[1] as Expression;
			}
		}

		public bool IsOut {
			get {
				return StartToken.Type == Token.OUT;
			}
		}

		public bool IsRef {
			get {
				return StartToken.Type == Token.REF;
			}
		}

		/// <summary>
		/// Returns a collection of this object's children.
		/// </summary>
		public override PieceOfCodeCollection Children {
			get {
				return new PieceOfCodeCollection(Expression);
			}
		}

		/// <summary>
		/// Overrides the default ModifiedVariables to return ref and out parameters which
		/// can be modified.
		/// </summary>
		public override VariableCollection ModifiedVariables {
			get {
				VariableCollection ret = new VariableCollection();
				if(IsOut || IsRef)
				{
					if (Expression is Identifier)
					{
						Identifier identifier = Expression as Identifier;
						Variable variable = FindParentScopeVariableDeclaration(identifier.Name);
						if (false == variable is MissingVariable)
						{
							ret.Add(variable);
						}
					}
				}
				return ret;
			}
		}
		
		private static Argument m_Missing = new MissingArgument();
		public static Argument Missing {
			get {
				return m_Missing;
			}
		}
	}

	public class MissingArgument : Argument
	{
		public MissingArgument() :
		base(PositionToken.Missing, new MissingExpression())
		{
		}
	}
}