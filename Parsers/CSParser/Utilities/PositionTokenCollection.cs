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
	public class PositionTokenCollection : PieceOfCode, IEnumerable {

		FundamentalPieceOfCodeCollection data = new FundamentalPieceOfCodeCollection();
		
		//
		// Constructors
		//
		public PositionTokenCollection()
		{
		}
		public PositionTokenCollection(PositionToken val)
		{
			Add(val);
		}
		public PositionTokenCollection(PositionTokenCollection val)
		{
			foreach (PositionToken token in val)
			{
				Add(token);	
			}
		}
		public PositionTokenCollection(object obj)
		{
			if (obj is PositionToken)
			{
				Add(obj as PositionToken);
			}
		}
		public PositionTokenCollection(object obj1, object obj2)
		{
			if (obj1 is PositionTokenCollection)
			{
				PositionTokenCollection tempCol = (PositionTokenCollection)obj1;
				foreach(PositionToken val in tempCol)
				{
					Add(val);
				}
			}
			if (obj2 is PositionToken)
			{
				Add(obj2 as PositionToken);
			}
		}
		
		public override FundamentalPieceOfCodeCollection Pieces {
			get {
    			return data;
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
			foreach (PositionToken tok in this)
			{
				ret.Append(tok.Generate());
			}
			return ret.ToString();
		}

        public override void PropagateUp()
        {
            foreach (FundamentalPieceOfCode poc in Pieces) {
                poc.Parent = this;
                poc.PropagateUp();
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

		//
		// Properties
		//
		public int Count {
			get {
				return data.Count;
			}
		}

		/// <summary>
		/// Finds the token that starts at the specified position
		/// and returns a list of comments (if any) immediately 
		/// before it as PositionToken objects.
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public PositionTokenCollection CommentTokensBeforePosition(Position position)
		{
			PositionTokenCollection ret = new PositionTokenCollection();
			PositionToken pt = TokenBeforePosition(position);
			while (null != pt)
			{
				if (pt.Value is Comment)
				{
					ret.Add(pt);
				}
				else
				{
					break;
				}
				pt = TokenBeforePosition(pt.Position);
			}
			return ret;
		}

		/// <summary>
		/// Finds the token that starts at the specified position
		/// and returns a list of comments (if any) immediately 
		/// before it.
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public CommentCollection CommentsBeforePosition(Position position)
		{
			CommentCollection ret = new CommentCollection();
			PositionToken pt = TokenBeforePosition(position);
			while (null != pt)
			{
				if (pt.Value is Comment)
				{
					ret.Add(pt.Value as Comment);
				}
				else
				{
					break;
				}
				pt = TokenBeforePosition(pt.Position);
			}
			return ret;
		}

		/// <summary>
		/// Finds and returns all the tokens within the start and end of the
		/// position.
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public PositionTokenCollection TokensWithinPosition(Position position)
		{
			PositionTokenCollection ret = new PositionTokenCollection();
			for(int i=0; i<data.Count; i++)
			{
				PositionToken pt = data[i] as PositionToken;
				if (position.ContainsInclusive(pt.Position))
				{
					ret.Add(data[i] as PositionToken);
				}
			}
			return ret;
		}

		/// <summary>
		/// Finds the token that starts at the specified position
		/// and returns the token immediately before it in the list.
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public PositionToken TokenBeforePosition(Position position)
		{
			PositionToken ret = null;
			for(int i=0; i<data.Count; i++)
			{
				PositionToken pt = data[i] as PositionToken;
				if ((pt.Position.StartLine == position.StartLine) &&
					(pt.Position.StartCol == position.StartCol))
				{
					if (i >= 1)
					{
						ret = data[i-1] as PositionToken;
					}
					break;
				}
			}
			return ret;
		}

		/// <summary>
		/// Finds the token that ends at the specified position and
		/// returns the token immediately after it.
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public PositionToken TokenAfterPosition(Position position)
		{
			PositionToken ret = null;
			for(int i=0; i<data.Count-1; i++)
			{
				PositionToken pt = data[i] as PositionToken;
				if ((pt.Position.EndLine == position.EndLine) &&
					(pt.Position.EndCol == position.EndCol))
				{
					if (i >= 1)
					{
						ret = data[i+1] as PositionToken;
					}
					break;
				}
			}
			return ret;
		}

		//
		// Methods
		//
		public void Add (PositionToken value)
		{
			data.Add (value);
		}

		public void AddRange (PositionToken [] values)
		{
			foreach (PositionToken ca in values) 
				data.Add (ca);
		}

		public void Clear ()
		{
			data.Clear ();
			m_Position = Position.Missing;
		}

		public void Reverse ()
		{
			data.Reverse();
		}

		private class Enumerator : IEnumerator {
			private PositionTokenCollection collection;
			private int currentIndex = -1;

			internal Enumerator (PositionTokenCollection collection)
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
			return new PositionTokenCollection.Enumerator (this);
		}

		public PositionToken this[int index] {
			get {
				return data [index] as PositionToken;
			}
			set {
				data [index] = value;
			}
		}

		public void Remove (object value)
		{
			data.Remove (value);
		}

		public void RemoveAt (int index)
		{
			data.RemoveAt (index);
		}
	}
}
