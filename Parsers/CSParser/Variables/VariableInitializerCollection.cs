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
using Opnieuw.Framework;

namespace Opnieuw.Parsers.CSParser
{
	public class VariableInitializerCollection : CommaSeperatedCollection, IEnumerable 
	{
		public static VariableInitializerCollection parse(TokenProvider tokenizer)
		{
			VariableInitializerCollection ret = new VariableInitializerCollection();
			VariableInitializer initializer = VariableInitializer.parse(tokenizer);
			while(false == (initializer is MissingVariableInitializer))
			{
				if (Token.COMMA == tokenizer.CurrentToken.Type)
				{
					ret.Add(initializer, tokenizer.CurrentToken);
					tokenizer.nextToken(); // ,
					initializer = VariableInitializer.parse(tokenizer);
				}
				else
				{
					ret.Add(initializer);
					break;
				}
			}
			return ret;
		}

		public VariableInitializerCollection()
		{
		}
		public VariableInitializerCollection(VariableInitializer val)
		{
			Add(val);
		}

		public override ExpressionCollection Expressions {
			get {
				ExpressionCollection ret = new ExpressionCollection();
				foreach (PieceOfCode poc in this)
				{
					if (poc is Expression)
					{
						ret.Add(poc as Expression);
					}
					ret.Add(poc.Expressions);
				}
				return ret;
			}
		}

		public void Add (VariableInitializer value)
		{
			data.Add (value);
			AdjustPosition();
		}
		public void Add (VariableInitializer value, PositionToken commaToken)
		{
			Add (value);
			commas.Add (commaToken);
			AdjustPosition();
		}

		public new VariableInitializer this[int index] {
			get {
				return data [index] as VariableInitializer;
			}
			set {
				data [index] = value;
			}
		}

		private class Enumerator : PieceOfCodeCollectionBaseEnumerator
		{
			public Enumerator(VariableInitializerCollection collection) :
			base(collection)
			{
			}

			public new VariableInitializer Current {
				get {
					if (currentIndex == collection.Count)
						throw new InvalidOperationException ();
					return collection [currentIndex] as VariableInitializer;
				}
			}
		}
		
		public new IEnumerator GetEnumerator ()
		{
			return new VariableInitializerCollection.Enumerator (this);
		}
	}
}
