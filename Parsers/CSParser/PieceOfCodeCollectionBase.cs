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
	public class PieceOfCodeCollectionBase : PieceOfCode, IEnumerable 
	{
		protected FundamentalPieceOfCodeCollection data = null;
        
        public PieceOfCodeCollectionBase()
        {
            data = new FundamentalPieceOfCodeCollection();
        }

		public override bool IsMissing {
			get {
				return (data.Count == 0);
			}
		}

        public override void Format()
        {
            foreach (PieceOfCode poc in this)
            {
                poc.Format();
            }
        }
		
		public override string Generate()
		{
			System.Text.StringBuilder ret = new System.Text.StringBuilder();
			for (int i=0; i<data.Count; i++)
			{
				FundamentalPieceOfCode fpoc = this[i] as FundamentalPieceOfCode;
				ret.Append(fpoc.Generate());
			}
			return ret.ToString();
		}

		protected void AdjustPosition()
		{
			foreach (object obj in data)
			{
				PieceOfCode poc = (PieceOfCode)obj;
				if (!(poc.Position is InvalidPosition))
				{
					if (poc.StartsBefore(Position))
					{
						Position oldPosition = new Position(Position);
						m_Position = new Position(poc.Position, oldPosition);
					}
					if (poc.EndsAfter(Position))
					{
						Position oldPosition = new Position(Position);
						m_Position = new Position(oldPosition, poc.Position);
					}
				}
			}
		}

        public override void checkNotMissing()
        {
            if (data.Count == 0) {
                throw new ParserException();
            }
        }

        private string m_LeadingCharacters = "";
		public override string LeadingCharacters {
			get {
				if (data.Count == 0) {
					return m_LeadingCharacters;
				} else {
					return (data[0] as FundamentalPieceOfCode).LeadingCharacters;
				}
			}
			set {
				if (data.Count == 0) {
					m_LeadingCharacters = value;
				} else {
					(data[0] as FundamentalPieceOfCode).LeadingCharacters = value;
				}
			}
		}

		public int Count {
			get {
				return data.Count;
			}
		}

		public FundamentalPieceOfCode this[int index] {
			get {
				return data [index] as FundamentalPieceOfCode;
			}
			set {
				data [index] = value;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				foreach (PieceOfCode poc in this)
				{
					ret.Add(poc);
				}
				return ret;
			}
		}
		
		public override FundamentalPieceOfCodeCollection Pieces {
			get {
    			return data;
			}
		}

		public void Reverse ()
		{
			data.Reverse();
		}

		public void Clear ()
		{
			data.Clear ();
		}

		protected class PieceOfCodeCollectionBaseEnumerator : IEnumerator {
			protected PieceOfCodeCollectionBase collection;
			protected int currentIndex = -1;

			internal PieceOfCodeCollectionBaseEnumerator (PieceOfCodeCollectionBase collection)
			{
				this.collection = collection;
			}

			public object Current {
				get {
					if (currentIndex == collection.Count)
						throw new InvalidOperationException ();
					return collection [currentIndex];
				}
			}

			public bool MoveNext ()
			{
				if (currentIndex > collection.Count)
					throw new InvalidOperationException ();
				return ++currentIndex < collection.Count;
			}

			public void Reset ()
			{
				currentIndex = -1;
			}
		}
		
		public IEnumerator GetEnumerator ()
		{
			return new PieceOfCodeCollectionBase.PieceOfCodeCollectionBaseEnumerator(this);
		}
	}
}
