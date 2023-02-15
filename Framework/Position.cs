#region Copyright (C) Pierre Boudreau
//This file is part of the Opnieuw project.
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

namespace Opnieuw.Framework
{
	public class Position
	{
		private int m_StartLine;
		private int m_StartCol;
		private int m_EndLine;
		private int m_EndCol;

		public Position(int startLine, int startCol, int endLine, int endCol)
		{
			m_StartLine = startLine;
			m_StartCol = startCol;
			m_EndLine = endLine;
			m_EndCol = endCol;
		}

		public Position(Position p) : this(p, p)
		{
		}

        public Position(Position start, Position end)
		{
			Initialize(start, end);
		}

		private void Initialize(Position start, Position end)
		{
			m_StartLine = start.StartLine;
			m_StartCol = start.StartCol;
			m_EndLine = end.EndLine;
			m_EndCol = end.EndCol;
		}

		public Position(params FundamentalPieceOfCode[] list) 
		{
			Position startPosition = null;
			Position endPosition = null;
			for ( int i = 0 ; i < list.Length ; i++ )
			{
				if (null == startPosition)
				{
					if (!(list[i].Position is InvalidPosition))
					{
						startPosition = (list[i]).Position;
					}
				}
			}
			for ( int i = (list.Length-1) ; i >= 0 ; i-- )
			{
				if (null == endPosition)
				{
					if (!(list[i].Position is InvalidPosition))
					{
						endPosition = (list[i]).Position;
					}
				}
			}
			if ((null != startPosition) && (null != endPosition))
			{
				Initialize(startPosition, endPosition);
			}
		}

		public int StartLine {
			get {
				return m_StartLine;
			}
			set {
				m_StartLine = value;
			}
		}

		public int StartCol {
			get {
				return m_StartCol;
			}
			set {
				m_StartCol = value;
			}
		}

		public int EndLine {
			get {
				return m_EndLine;
			}
			set {
				m_EndLine = value;
			}
		}

		public int EndCol {
			get {
				return m_EndCol;
			}
			set {
				m_EndCol = value;
			}
		}
		
		public bool IsZeroLength {
			get {
				return ((StartLine == EndLine) && (StartCol == EndCol));
			}
		}

		public bool IsIntersectedBy(Position checkPosition)
		{
			bool ret = false;
			if (null != checkPosition)
			{
				if ((Contains(checkPosition.StartLine, checkPosition.StartCol)) ^
				    (Contains(checkPosition.EndLine, checkPosition.EndCol)))
				{
					ret = true;
				}
			}
			return ret;
		}

		public bool Contains(int line, int col)
		{
			bool ret = false;

			if ((line == StartLine) && (line == EndLine))
			{
				if ((StartCol < col) && (EndCol > col))
				{
					ret = true;
				}
			}
			else if ((line == StartLine) && (col > StartCol))
			{
				ret = true;
			}
			else if ((line == EndLine) && (col < EndCol))
			{
				ret = true;
			}
			else if ((line > StartLine) && (line < EndLine))
			{
				ret = true;
			}

			return ret;
		}
		
		public bool ContainsInclusive(int line, int col)
		{
			bool ret = false;

			if ((line == StartLine) && (line == EndLine))
			{
				if ((col >= StartCol) && (col <= EndCol))
				{
					ret = true;
				}
			}
			else if ((line == StartLine) && (col >= StartCol))
			{
				ret = true;
			}
			else if ((line == EndLine) && (col <= EndCol))
			{
				ret = true;
			}
			else if ((line > StartLine) && (line < EndLine))
			{
				ret = true;
			}

			return ret;
		}

		public bool Contains(Position checkPosition)
		{
			bool ret = false;
			if ((null != checkPosition) && 
				(!IsContainedBy(checkPosition)))
			{
				if ((Contains(checkPosition.StartLine, checkPosition.StartCol)) && 
					(Contains(checkPosition.EndLine, checkPosition.EndCol)))
				{
					ret = true;
				}
			}
			return ret;
		}

		public bool ContainsInclusive(Position checkPosition)
		{
			bool ret = false;
			if (null != checkPosition)
			{
				if ((ContainsInclusive(checkPosition.StartLine, checkPosition.StartCol)) && 
					(ContainsInclusive(checkPosition.EndLine, checkPosition.EndCol)))
				{
					ret = true;
				}
			}
			return ret;
		}

		public bool IsContainedBy(Position checkPosition)
		{
			bool ret = false;
			if (null != checkPosition)
			{
				if ((checkPosition.StartLine < StartLine) &&
					(checkPosition.EndLine > EndLine))
				{
					ret = true;
				}
				else if ((checkPosition.StartLine == StartLine) &&
						 (checkPosition.EndLine == EndLine))
				{
					ret = ((checkPosition.StartCol <= StartCol) &&
 						   (checkPosition.EndCol >= EndCol));
				}
				else if ((checkPosition.StartLine == StartLine) &&
						 (checkPosition.EndLine > EndLine))
				{
					ret = (checkPosition.StartCol <= StartCol);
				}
				else if ((checkPosition.StartLine < StartLine) &&
						 (checkPosition.EndLine == EndLine))
				{
					ret = (checkPosition.EndCol >= EndCol);
				}
			}
			return ret;
		}

		public bool StartsBefore(Position checkPosition)
		{
			bool ret = false;
			if ((null == checkPosition) || checkPosition is InvalidPosition)
			{
				ret = true;
			}
			else
			{
				if (StartLine < checkPosition.StartLine)
				{
					ret = true;
				}
				else if ((StartLine == checkPosition.StartLine) &&
						(StartCol < checkPosition.StartCol))
				{
					ret = true;
				}
			}
			return ret;
		}

		public bool EndsBefore(Position checkPosition)
		{
			bool ret = false;
			if ((null == checkPosition) || checkPosition is InvalidPosition)
			{
				ret = true;
			}
			else
			{
				if (EndLine < checkPosition.StartLine)
				{
					ret = true;
				}
				else if ((EndLine == checkPosition.StartLine) &&
						(EndCol < checkPosition.StartCol))
				{
					ret = true;
				}
			}
			return ret;
		}

		public bool EndsAfter(Position checkPosition)
		{
			bool ret = false;
			if ((null == checkPosition) || checkPosition is InvalidPosition)
			{
				ret = true;
			}
			else
			{
				if (EndLine > checkPosition.EndLine)
				{
					ret = true;
				}
				else if ((EndLine == checkPosition.EndLine) &&
						(EndCol > checkPosition.EndCol))
				{
					ret = true;
				}
			}
			return ret;
		}

		public bool StartsAfter(Position checkPosition)
		{
			bool ret = false;
			if ((null == checkPosition) || checkPosition is InvalidPosition)
			{
				ret = true;
			}
			else
			{
				if (StartLine > checkPosition.EndLine)
				{
					ret = true;
				}
				else if ((StartLine == checkPosition.EndLine) &&
						(StartCol > checkPosition.EndCol))
				{
					ret = true;
				}
			}
			return ret;
		}

		public bool GetCharIndexAndLengthFromPosition(string[] lines, int tabWidth, out int charIndex, out int length)
		{
			int lineCount = 1;
			charIndex = 0;
			length = 1;
			for (lineCount=0; lineCount < StartLine-1; lineCount ++)
			{
				charIndex += (lines[lineCount].Length + 1);
			}
			if (lineCount == (StartLine - 1))
			{
				int startIndex = StartCol;
				charIndex += (startIndex - 1);
			}
			while (lineCount < (EndLine - 1))
			{
				length += (lines[lineCount].Length + 1);
				lineCount ++;
			}
			if (lineCount == (EndLine - 1))
			{
				length += (EndCol - StartCol);
			}
			return true;
		}
		
		public PositionCollection ChangingPositionsTo(Position newPosition)
		{
			PositionCollection ret = new PositionCollection();
			if (ContainsInclusive(newPosition)) {
				//Return two positions for the changing part before
				//and after this one.
				Position pos1 = new Position(StartLine, 
									 StartCol,
									 newPosition.StartLine, 
									 newPosition.StartCol);
				Position pos2 = new Position(newPosition.EndLine, 
									 newPosition.EndCol,
									 EndLine, 
									 EndCol);
				if (!pos1.IsZeroLength) ret.Add(pos1);
				if (!pos2.IsZeroLength) ret.Add(pos2);
			} else if (newPosition.ContainsInclusive(this)) {
				//Return two positions for the parts before
				//and after the new position that are not 
				//covered by this one.
				Position pos1 = new Position(newPosition.StartLine, 
									 newPosition.StartCol,
									 StartLine, 
									 StartCol);
				Position pos2 = new Position(EndLine, 
									 EndCol,
									 newPosition.EndLine, 
									 newPosition.EndCol);
				if (!pos1.IsZeroLength) ret.Add(pos1);
				if (!pos2.IsZeroLength) ret.Add(pos2);
			} else if ((newPosition.StartLine == EndLine) && 
					   (newPosition.StartCol == EndCol)) {
				ret.Add(new Position(StartLine, 
									 StartCol, 
									 newPosition.EndLine, 
									 newPosition.EndCol));	
			} else if ((StartLine == newPosition.EndLine) && 
					   (StartCol == newPosition.EndCol)) {
				ret.Add(new Position(newPosition.StartLine, 
									 newPosition.StartCol, 
									 EndLine, 
									 EndCol));	
			} else if (newPosition.EndsBefore(this) || newPosition.StartsAfter(this)) {
				//There is no overlap.  Return the two positions.
				ret.Add(new Position(this));
				ret.Add(new Position(newPosition));
			}
			return ret;
		}

		public override string ToString()
		{
			string ret = "(" + StartLine + "," + StartCol + ")";
			if ((StartLine != EndLine) || (StartCol != EndCol))
			{
				ret += "(" + EndLine + "," + EndCol  + ")";
			}
			return ret;
		}
		
		protected static InvalidPosition m_MissingPosition = new InvalidPosition();
		public static Position Missing {
			get {
				return m_MissingPosition;
			}
		}
	}

	public class InvalidPosition : Position
	{
		public InvalidPosition() :
		base(0, 0, 0, 0)
		{
		}
	}
}
