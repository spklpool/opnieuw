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
	public class StatementCollection : PieceOfCodeCollectionBase, IEnumerable 
	{
		public static StatementCollection parse(TokenProvider tokenizer)
		{
			StatementCollection ret = new StatementCollection();
			Statement stmt = Statement.parse(tokenizer);
			while (false == (stmt is MissingStatement))
			{
				ret.Add(stmt);
				stmt = Statement.parse(tokenizer);
			}
			return ret;
		}

		public StatementCollection()
		{
		}
		public StatementCollection(Statement val)
		{
			Add(val);
		}
		public StatementCollection(StatementCollection val)
		{
			foreach (Statement statement in val) {
				Add(statement);
			}
		}

		public void Add (Statement value)
		{
			data.Add (value);
			AdjustPosition();
		}
		
		public void Add(StatementCollection val)
		{
			foreach (Statement statement in val) {
				Add(statement);
			}
        }

        public void InsertAfter(Statement target, Statement val)
        {
            for (int i = 0; i < Count; i++) {
                if (this[i].Position.ContainsInclusive(target.Position)) {
                    if (i == (Count - 1)) {
                        data.Add(val);
                    } else {
                        data.Insert(i + 1, val);
                    }
                    return;
                }
            }
        }

        public void Remove(Statement item)
        {
            data.Remove(item);
        }

        public new Statement this[int index] {
			get {
				return data [index] as Statement;
			}
			set {
				data [index] = value;
			}
		}

		private class Enumerator : PieceOfCodeCollectionBaseEnumerator
		{
			public Enumerator(StatementCollection collection) :
			base(collection)
			{
			}

			public new Statement Current {
				get {
					if (currentIndex == collection.Count)
						throw new InvalidOperationException ();
					return collection [currentIndex] as Statement;
				}
			}
		}
		
		public new IEnumerator GetEnumerator ()
		{
			return new StatementCollection.Enumerator (this);
		}

		public override string AsText {
			get {
				return "Statements";
			}
		}
	}
}
