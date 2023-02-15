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
	public class ArrayCreationExpression : PrimaryExpression
	{
		/// <summary>
		/// new non-array-type [ expression-list ] rank-specifiersopt array-initializeropt
		/// </summary>
		public new static Expression parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			Expression ret = new MissingExpression();
			if (Token.NEW == tokenizer.CurrentToken.Type)
			{
				PositionToken newToken = tokenizer.CurrentToken;
				tokenizer.nextToken(); // new
                DataType type = DataType.parse(tokenizer);
				tokenizer.setBookmark();
				ExpressionCollection expressions = new ExpressionCollection();
				PositionToken potentialLastToken = newToken;
				PositionToken openBracketToken = PositionToken.Missing;
				PositionToken closeBracketToken = PositionToken.Missing;
				if (Token.OPEN_BRACKET == tokenizer.CurrentToken.Type)
				{
					openBracketToken = tokenizer.CurrentToken;
					tokenizer.nextToken(); // [
					expressions = ExpressionCollection.parse(tokenizer);
					if (expressions.Count != 0)
					{
						potentialLastToken = tokenizer.CurrentToken;
						closeBracketToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // ]
					}
				}
				tokenizer.endBookmark(expressions.Count == 0);
				RankSpecifierCollection rankSpecifiers = RankSpecifierCollection.parse(tokenizer);
				ArrayInitializer arrayInitializer = ArrayInitializer.parse(tokenizer);
				ret = new ArrayCreationExpression(newToken, type, openBracketToken, expressions, closeBracketToken, rankSpecifiers, arrayInitializer);
			}
			tokenizer.endBookmark(ret is MissingExpression);
			return ret;
		}

		public ArrayCreationExpression(PositionToken newToken, DataType type, 
									   PositionToken openBracketToken, 
									   ExpressionCollection expressionList, 
									   PositionToken closeBracketToken, 
									   RankSpecifierCollection rankSpecifiers, 
									   ArrayInitializer arrayInitializer) :
		base(newToken, type, openBracketToken, expressionList, closeBracketToken, rankSpecifiers, arrayInitializer)
		{
			//System.Console.WriteLine("Creating ArrayCreationExpression with position: " + m_Position.ToString());
			//System.Console.WriteLine("newToken position: " + newToken.Position.ToString());
			//System.Console.WriteLine("type position: " + type.Position.ToString());
			//System.Console.WriteLine("expressionList position: " + expressionList.Position.ToString());
			//System.Console.WriteLine("closeBracketToken position: " + closeBracketToken.Position.ToString());
			//System.Console.WriteLine("rankSpecifiers position: " + rankSpecifiers.Position.ToString());
			//System.Console.WriteLine("arrayInitializer position: " + arrayInitializer.Position.ToString());
			//if (arrayInitializer.Position is InvalidPosition)
			//{
			//	System.Console.WriteLine("arrayInitializer position is invalid");
			//}
			//else
			//{
			//	System.Console.WriteLine("arrayInitializer position is NOT invalid");
			//}
		}

		public PositionToken NewToken {
			get {
				return Pieces[0] as PositionToken;
			}
		}

		public DataType Type {
			get {
				return Pieces[1] as DataType;
			}
		}

		public PositionToken OpenBracketToken {
			get {
				return Pieces[2] as PositionToken;
			}
		}

		public ExpressionCollection ExpressionList {
			get {
				return Pieces[3] as ExpressionCollection;
			}
		}

		public PositionToken CloseBracketToken {
			get {
				return Pieces[4] as PositionToken;
			}
		}

		public RankSpecifierCollection RankSpecifiers {
			get {
				return Pieces[5] as RankSpecifierCollection;
			}
		}

		public ArrayInitializer ArrayInitializer {
			get {
				return Pieces[6] as ArrayInitializer;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(ExpressionList);
				ret.Add(RankSpecifiers);
				ret.Add(ArrayInitializer);
				return ret;
			}
		}
	}
}
