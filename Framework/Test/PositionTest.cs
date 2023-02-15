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
using System.Collections;
using NUnit.Framework;

using Opnieuw.Framework;

namespace Opnieuw.Framework.Test
{
	[TestFixture]
	public class PositionTest : Assertion
	{
		private class SomePositionProvider : FundamentalPieceOfCode
		{
			public SomePositionProvider(int startLine, int startCol, int endLine, int endCol)
			{
				Position = new Position(startLine, startCol, endLine, endCol);
			}
        
			private FundamentalPieceOfCode m_Up = null;
			public FundamentalPieceOfCode Parent {
				get {
					return m_Up;
				}
				set {
					m_Up = value;
				}
			}

			private string m_LeadingCharacters = "";
			public string LeadingCharacters {
				get {
					return m_LeadingCharacters;
				}
				set {
					m_LeadingCharacters = value;
				}
			}

			Position m_Position = null;
			public Position Position {
				get {
					return m_Position;
				}
				set {
					m_Position = value;
				}
			}

			FundamentalPieceOfCodeCollection m_Pieces = new FundamentalPieceOfCodeCollection();
			public FundamentalPieceOfCodeCollection Pieces {
				get {
					return m_Pieces;
				}
			}
			
			public bool IsMissing {
				get {
					return false;
				}
			}
			
			public FundamentalPieceOfCode Comments {
				get {
					return null;
				}
			}

			public virtual void Format()
			{
			}

            public virtual void CloneFormat(FundamentalPieceOfCode cloningSource)
            {
            }
			
			public string Generate()
			{
				return "";
			}
			
			public void PropagateUp()
			{
			}
		}

		[Test]
		public void testConstructor1()
		{
			Position p = new Position(1, 2, 3, 4);
			Assertion.AssertEquals(1, p.StartLine);
			Assertion.AssertEquals(2, p.StartCol);
			Assertion.AssertEquals(3, p.EndLine);
			Assertion.AssertEquals(4, p.EndCol);
		}

		[Test]
		public void testConstructor2()
		{
			Position p1 = new Position(1, 2, 3, 4);
			Position p2 = new Position(p1);
			Assertion.AssertEquals(p1.StartLine, p2.StartLine);
			Assertion.AssertEquals(p1.StartCol, p2.StartCol);
			Assertion.AssertEquals(p1.EndLine, p2.EndLine);
			Assertion.AssertEquals(p1.EndCol, p2.EndCol);
		}

		[Test]
		public void testConstructor3()
		{
			FundamentalPieceOfCode startPP = (FundamentalPieceOfCode) new SomePositionProvider(1, 2, 3, 4);
			FundamentalPieceOfCode endPP = (FundamentalPieceOfCode) new SomePositionProvider(5, 6, 7, 8);
			Position testPosition = new Position(startPP, endPP);
			Assertion.AssertEquals(1, testPosition.StartLine);
			Assertion.AssertEquals(2, testPosition.StartCol);
			Assertion.AssertEquals(7, testPosition.EndLine);
			Assertion.AssertEquals(8, testPosition.EndCol);
		}

		[Test]
		public void testConstructor4()
		{
			Position startP = new Position(1, 2, 3, 4);
			Position endP = new Position(5, 6, 7, 8);
			Position testPosition = new Position(startP, endP);
			Assertion.AssertEquals(1, testPosition.StartLine);
			Assertion.AssertEquals(2, testPosition.StartCol);
			Assertion.AssertEquals(7, testPosition.EndLine);
			Assertion.AssertEquals(8, testPosition.EndCol);
		}

		[Test]
		public void testConstructor6()
		{
			Position startP = new Position(1, 2, 3, 4);
			Position endP = new Position(5, 6, 7, 8);
			Position testPosition = new Position(startP, endP);
			Assertion.AssertEquals(1, testPosition.StartLine);
			Assertion.AssertEquals(2, testPosition.StartCol);
			Assertion.AssertEquals(7, testPosition.EndLine);
			Assertion.AssertEquals(8, testPosition.EndCol);
		}

		[Test]
		public void testIsContainedBy()
		{
			Position checkPosition = null;
			Position s = new Position(0, 0, 0, 0);

			Assertion.AssertEquals(false, s.IsContainedBy(null));

			checkPosition = new Position(1, 1, 1, 10);
			Assertion.AssertEquals(false, s.IsContainedBy(null));

			s = new Position(2, 2, 2, 5);

			Assertion.AssertEquals(false, s.IsContainedBy(null));

			checkPosition = new Position(2, 1, 2, 10);
			Assertion.AssertEquals(true, s.IsContainedBy(checkPosition));

			checkPosition = new Position(2, 2, 2, 10);
			Assertion.AssertEquals(true, s.IsContainedBy(checkPosition));

			checkPosition = new Position(2, 1, 2, 5);
			Assertion.AssertEquals(true, s.IsContainedBy(checkPosition));

			checkPosition = new Position(2, 3, 2, 10);
			Assertion.AssertEquals(false, s.IsContainedBy(checkPosition));

			checkPosition = new Position(2, 1, 2, 3);
			Assertion.AssertEquals(false, s.IsContainedBy(checkPosition));

			checkPosition = new Position(2, 3, 2, 4);
			Assertion.AssertEquals(false, s.IsContainedBy(checkPosition));

			checkPosition = new Position(2, 1, 2, 10);
			Assertion.AssertEquals(true, s.IsContainedBy(checkPosition));

			checkPosition = new Position(1, 1, 3, 1);
			Assertion.AssertEquals(true, s.IsContainedBy(checkPosition));

			checkPosition = new Position(2, 1, 3, 1);
			Assertion.AssertEquals(true, s.IsContainedBy(checkPosition));

			checkPosition = new Position(1, 1, 2, 10);
			Assertion.AssertEquals(true, s.IsContainedBy(checkPosition));

			checkPosition = new Position(2, 10, 2, 10);
			Assertion.AssertEquals(false, s.IsContainedBy(checkPosition));

			checkPosition = new Position(2, 1, 2, 1);
			Assertion.AssertEquals(false, s.IsContainedBy(checkPosition));

			checkPosition = new Position(1, 1, 1, 1);
			Assertion.AssertEquals(false, s.IsContainedBy(checkPosition));

			checkPosition = new Position(3, 1, 3, 1);
			Assertion.AssertEquals(false, s.IsContainedBy(checkPosition));
		}
		
		[Test]
		public void testIsZeroLength1()
		{
			CodeSelection selection = new CodeSelection(1, 1, 1, 1);
			Assertion.Assert("Selection should be zero length!", selection.IsZeroLength);
		}
		
		[Test]
		public void testIsZeroLength2()
		{
			CodeSelection selection = new CodeSelection(1, 2, 1, 1);
			Assertion.Assert("Selection should NOT be zero length!", !selection.IsZeroLength);
		}

		[Test]
		public void testContains()
		{
			Position checkPosition = null;
			Position s = new Position(0, 0, 0, 0);

			Assertion.AssertEquals(false, s.Contains(null));
			Assertion.AssertEquals(false, s.Contains(0, 0));

			checkPosition = new Position(1, 1, 1, 10);
			Assertion.AssertEquals(false, s.Contains(null));

			s = new Position(2, 2, 5, 5);

			Assertion.AssertEquals(false, s.Contains(null));

			checkPosition = new Position(2, 3, 2, 3);
			Assertion.AssertEquals(true, s.Contains(checkPosition));
			Assertion.AssertEquals(true, s.Contains(2, 3));

			checkPosition = new Position(2, 3, 2, 5);
			Assertion.AssertEquals(true, s.Contains(checkPosition));

			checkPosition = new Position(2, 2, 2, 3);
			Assertion.AssertEquals(false, s.Contains(checkPosition));
			Assertion.AssertEquals(false, s.Contains(2, 2));

			checkPosition = new Position(2, 2, 2, 2);
			Assertion.AssertEquals(false, s.Contains(checkPosition));
			
			s = new Position(1, 2, 1, 5);
			
			checkPosition = new Position(1, 3, 1, 5);
			Assertion.AssertEquals(false, s.Contains(checkPosition));
		}

		[Test]
		public void testContainsInclusive()
		{
			Position checkPosition = null;
			Position s = new Position(0, 0, 0, 0);

			Assertion.AssertEquals(false, s.ContainsInclusive(null));
			Assertion.AssertEquals(true, s.ContainsInclusive(0, 0));

			checkPosition = new Position(1, 1, 1, 10);
			Assertion.AssertEquals(false, s.ContainsInclusive(null));

			s = new Position(2, 2, 5, 5);

			Assertion.AssertEquals(false, s.ContainsInclusive(null));

			checkPosition = new Position(2, 3, 2, 3);
			Assertion.AssertEquals(true, s.ContainsInclusive(checkPosition));
			Assertion.AssertEquals(true, s.ContainsInclusive(2, 3));

			checkPosition = new Position(2, 3, 2, 5);
			Assertion.AssertEquals(true, s.ContainsInclusive(checkPosition));

			checkPosition = new Position(2, 2, 2, 3);
			Assertion.AssertEquals(true, s.ContainsInclusive(checkPosition));
			Assertion.AssertEquals(true, s.ContainsInclusive(2, 2));

			checkPosition = new Position(2, 2, 2, 2);
			Assertion.AssertEquals(true, s.ContainsInclusive(checkPosition));

			checkPosition = new Position(2, 2, 5, 5);
			Assertion.AssertEquals(true, s.ContainsInclusive(checkPosition));
		}

		[Test]
		public void testIsIntersectedBy()
		{
			Position checkPosition = null;
			Position s = new Position(0, 0, 0, 0);

			Assertion.AssertEquals(false, s.IsIntersectedBy(null));

			checkPosition = new Position(1, 1, 1, 10);
			Assertion.AssertEquals(false, s.IsIntersectedBy(null));

			s = new Position(2, 2, 2, 5);

			Assertion.AssertEquals(false, s.IsIntersectedBy(null));

			checkPosition = new Position(2, 1, 2, 10);
			Assertion.AssertEquals(false, s.IsIntersectedBy(checkPosition));

			checkPosition = new Position(2, 2, 2, 10);
			Assertion.AssertEquals(false, s.IsIntersectedBy(checkPosition));

			checkPosition = new Position(2, 1, 2, 5);
			Assertion.AssertEquals(false, s.IsIntersectedBy(checkPosition));

			checkPosition = new Position(2, 3, 2, 10);
			Assertion.AssertEquals(true, s.IsIntersectedBy(checkPosition));

			checkPosition = new Position(2, 1, 2, 3);
			Assertion.AssertEquals(true, s.IsIntersectedBy(checkPosition));

			checkPosition = new Position(2, 3, 2, 4);
			Assertion.AssertEquals(false, s.IsIntersectedBy(checkPosition));

			checkPosition = new Position(2, 1, 2, 10);
			Assertion.AssertEquals(false, s.IsIntersectedBy(checkPosition));

			checkPosition = new Position(1, 1, 3, 1);
			Assertion.AssertEquals(false, s.IsIntersectedBy(checkPosition));

			checkPosition = new Position(2, 1, 3, 1);
			Assertion.AssertEquals(false, s.IsIntersectedBy(checkPosition));

			checkPosition = new Position(1, 1, 2, 10);
			Assertion.AssertEquals(false, s.IsIntersectedBy(checkPosition));

			checkPosition = new Position(2, 10, 2, 10);
			Assertion.AssertEquals(false, s.IsIntersectedBy(checkPosition));

			checkPosition = new Position(2, 1, 2, 1);
			Assertion.AssertEquals(false, s.IsIntersectedBy(checkPosition));

			checkPosition = new Position(1, 1, 1, 1);
			Assertion.AssertEquals(false, s.IsIntersectedBy(checkPosition));

			checkPosition = new Position(3, 1, 3, 1);
			Assertion.AssertEquals(false, s.IsIntersectedBy(checkPosition));
		}

		[Test]
		public void testStartsBefore()
		{
			Position checkPosition = null;
			Position s = new Position(0, 0, 0, 0);

			Assertion.AssertEquals(true, s.StartsBefore(null));

			checkPosition = new Position(1, 1, 1, 10);
			Assertion.AssertEquals(true, s.StartsBefore(checkPosition));

			s = new Position(2, 2, 2, 5);

			Assertion.AssertEquals(true, s.StartsBefore(null));

			checkPosition = new Position(2, 1, 2, 10);
			Assertion.AssertEquals(false, s.StartsBefore(checkPosition));

			checkPosition = new Position(2, 2, 2, 10);
			Assertion.AssertEquals(false, s.StartsBefore(checkPosition));

			checkPosition = new Position(2, 3, 2, 5);
			Assertion.AssertEquals(true, s.StartsBefore(checkPosition));

			checkPosition = new Position(2, 3, 2, 10);
			Assertion.AssertEquals(true, s.StartsBefore(checkPosition));

			checkPosition = new Position(2, 1, 2, 3);
			Assertion.AssertEquals(false, s.StartsBefore(checkPosition));

			checkPosition = new Position(1, 1, 3, 1);
			Assertion.AssertEquals(false, s.StartsBefore(checkPosition));

			checkPosition = new Position(3, 1, 3, 1);
			Assertion.AssertEquals(true, s.StartsBefore(checkPosition));
		}

		[Test]
		public void testEndsAfter()
		{
			Position checkPosition = null;
			Position s = new Position(0, 0, 0, 0);

			Assertion.AssertEquals(true, s.EndsAfter(null));

			checkPosition = new Position(1, 1, 1, 10);
			Assertion.AssertEquals(false, s.EndsAfter(checkPosition));

			s = new Position(2, 2, 2, 5);

			Assertion.AssertEquals(true, s.EndsAfter(null));

			checkPosition = new Position(2, 1, 2, 10);
			Assertion.AssertEquals(false, s.EndsAfter(checkPosition));

			checkPosition = new Position(2, 3, 2, 5);
			Assertion.AssertEquals(false, s.EndsAfter(checkPosition));

			checkPosition = new Position(2, 3, 2, 10);
			Assertion.AssertEquals(false, s.EndsAfter(checkPosition));

			checkPosition = new Position(2, 1, 2, 3);
			Assertion.AssertEquals(true, s.EndsAfter(checkPosition));

			checkPosition = new Position(1, 1, 3, 1);
			Assertion.AssertEquals(false, s.EndsAfter(checkPosition));

			checkPosition = new Position(3, 1, 3, 1);
			Assertion.AssertEquals(false, s.EndsAfter(checkPosition));
		}

		[Test]
		public void testGetCharIndexAndLengthFromPosition1()
		{
			Position position = new Position(1, 1, 1, 1);
			string[] lines = new String[4] {"1234567890", "1234567890", "1234567890", "1234567890"};
			int charIndex, length;
			position.GetCharIndexAndLengthFromPosition(lines, 4, out charIndex, out length);
			Assertion.AssertEquals("charIndex is wrong!", 0, charIndex);
			Assertion.AssertEquals("length is wrong!", 1, length);
		}

		[Test]
		public void testGetCharIndexAndLengthFromPosition2()
		{
			Position position = new Position(1, 1, 1, 2);
			string[] lines = new String[4] {"1234567890", "1234567890", "1234567890", "1234567890"};
			int charIndex, length;
			position.GetCharIndexAndLengthFromPosition(lines, 4, out charIndex, out length);
			Assertion.AssertEquals("charIndex is wrong!", 0, charIndex);
			Assertion.AssertEquals("length is wrong!", 2, length);
		}

		[Test]
		public void testGetCharIndexAndLengthFromPosition3()
		{
			Position position = new Position(1, 1, 2, 2);
			string[] lines = new String[4] {"1234567890", "1234567890", "1234567890", "1234567890"};
			int charIndex, length;
			position.GetCharIndexAndLengthFromPosition(lines, 4, out charIndex, out length);
			Assertion.AssertEquals("charIndex is wrong!", 0, charIndex);
			Assertion.AssertEquals("length is wrong!", 13, length);
		}

		[Test]
		public void testGetCharIndexAndLengthFromPosition4()
		{
			Position position = new Position(1, 1, 3, 2);
			string[] lines = new String[4] {"1234567890", "1234567890", "1234567890", "1234567890"};
			int charIndex, length;
			position.GetCharIndexAndLengthFromPosition(lines, 4, out charIndex, out length);
			Assertion.AssertEquals("charIndex is wrong!", 0, charIndex);
			Assertion.AssertEquals("length is wrong!", 24, length);
		}

		[Test]
		public void testGetCharIndexAndLengthFromPosition5()
		{
			Position position = new Position(1, 1, 3, 2);
			string[] lines = new String[4] {"1234567890", "\t234567890", "1234567890", "1234567890"};
			int charIndex, length;
			position.GetCharIndexAndLengthFromPosition(lines, 4, out charIndex, out length);
			Assertion.AssertEquals("charIndex is wrong!", 0, charIndex);
			Assertion.AssertEquals("length is wrong!", 24, length);
		}
		
		[Test]
		public void testChangingPositionsTo1()
		{
			Position pos = new Position(1, 2, 1, 3);
			PositionCollection col = pos.ChangingPositionsTo(new Position(1, 2, 1, 3));
			AssertEquals("Number of positions is wrong!", 0, col.Count);
		}
		
		[Test]
		public void testChangingPositionsTo2()
		{
			Position pos = new Position(1, 2, 1, 3);

			PositionCollection col = pos.ChangingPositionsTo(new Position(1, 3, 1, 4));
			AssertEquals("Number of positions is wrong!", 1, col.Count);
			
			IEnumerator enumerator = col.GetEnumerator();
			enumerator.MoveNext();
			Position pos1 = enumerator.Current as Position;
			AssertEquals("StartLine is wrong!", 1, pos1.StartLine);
			AssertEquals("StartCol is wrong!", 2, pos1.StartCol);
			AssertEquals("EndLine is wrong!", 1, pos1.EndLine);
			AssertEquals("EndCol is wrong!", 4, pos1.EndCol);
		}
		
		[Test]
		public void testChangingPositionsTo3()
		{
			Position pos = new Position(1, 7, 1, 8);

			PositionCollection col = pos.ChangingPositionsTo(new Position(1, 7, 1, 9));
			AssertEquals("Number of positions is wrong!", 1, col.Count);
			
			IEnumerator enumerator = col.GetEnumerator();
			enumerator.MoveNext();
			Position pos1 = enumerator.Current as Position;
			AssertEquals("StartLine is wrong!", 1, pos1.StartLine);
			AssertEquals("StartCol is wrong!", 8, pos1.StartCol);
			AssertEquals("EndLine is wrong!", 1, pos1.EndLine);
			AssertEquals("EndCol is wrong!", 9, pos1.EndCol);
		}
		
		[Test]
		public void testChangingPositionsTo4()
		{
			Position pos = new Position(1, 7, 1, 8);

			PositionCollection col = pos.ChangingPositionsTo(new Position(1, 6, 1, 8));
			AssertEquals("Number of positions is wrong!", 1, col.Count);
			
			IEnumerator enumerator = col.GetEnumerator();
			enumerator.MoveNext();
			Position pos1 = enumerator.Current as Position;
			AssertEquals("StartLine is wrong!", 1, pos1.StartLine);
			AssertEquals("StartCol is wrong!", 6, pos1.StartCol);
			AssertEquals("EndLine is wrong!", 1, pos1.EndLine);
			AssertEquals("EndCol is wrong!", 7, pos1.EndCol);
		}
		
		[Test]
		public void testChangingPositionsTo5()
		{
			Position pos = new Position(1, 6, 1, 8);

			PositionCollection col = pos.ChangingPositionsTo(new Position(1, 7, 1, 8));
			AssertEquals("Number of positions is wrong!", 1, col.Count);
			
			IEnumerator enumerator = col.GetEnumerator();
			enumerator.MoveNext();
			Position pos1 = enumerator.Current as Position;
			AssertEquals("StartLine is wrong!", 1, pos1.StartLine);
			AssertEquals("StartCol is wrong!", 6, pos1.StartCol);
			AssertEquals("EndLine is wrong!", 1, pos1.EndLine);
			AssertEquals("EndCol is wrong!", 7, pos1.EndCol);
		}
		
		[Test]
		public void testChangingPositionsTo6()
		{
			Position pos = new Position(1, 7, 1, 9);

			PositionCollection col = pos.ChangingPositionsTo(new Position(1, 7, 1, 8));
			AssertEquals("Number of positions is wrong!", 1, col.Count);
			
			IEnumerator enumerator = col.GetEnumerator();
			enumerator.MoveNext();
			Position pos1 = enumerator.Current as Position;
			AssertEquals("StartLine is wrong!", 1, pos1.StartLine);
			AssertEquals("StartCol is wrong!", 8, pos1.StartCol);
			AssertEquals("EndLine is wrong!", 1, pos1.EndLine);
			AssertEquals("EndCol is wrong!", 9, pos1.EndCol);
		}
		
		[Test]
		public void testChangingPositionsTo7()
		{
			Position pos = new Position(1, 3, 2, 3);

			PositionCollection col = pos.ChangingPositionsTo(new Position(1, 3, 2, 4));
			AssertEquals("Number of positions is wrong!", 1, col.Count);
			
			IEnumerator enumerator = col.GetEnumerator();
			enumerator.MoveNext();
			Position pos1 = enumerator.Current as Position;
			AssertEquals("StartLine is wrong!", 2, pos1.StartLine);
			AssertEquals("StartCol is wrong!", 3, pos1.StartCol);
			AssertEquals("EndLine is wrong!", 2, pos1.EndLine);
			AssertEquals("EndCol is wrong!", 4, pos1.EndCol);
		}
		
		[Test]
		public void testChangingPositionsTo8()
		{
			Position pos = new Position(1, 1, 1, 5);

			PositionCollection col = pos.ChangingPositionsTo(new Position(1, 3, 1, 4));
			AssertEquals("Number of positions is wrong!", 2, col.Count);
			
			IEnumerator enumerator = col.GetEnumerator();
			enumerator.MoveNext();
			Position pos1 = enumerator.Current as Position;
			AssertEquals("StartLine is wrong!", 1, pos1.StartLine);
			AssertEquals("StartCol is wrong!", 1, pos1.StartCol);
			AssertEquals("EndLine is wrong!", 1, pos1.EndLine);
			AssertEquals("EndCol is wrong!", 3, pos1.EndCol);
			
			enumerator.MoveNext();
			Position pos2 = enumerator.Current as Position;
			AssertEquals("StartLine is wrong!", 1, pos2.StartLine);
			AssertEquals("StartCol is wrong!", 4, pos2.StartCol);
			AssertEquals("EndLine is wrong!", 1, pos2.EndLine);
			AssertEquals("EndCol is wrong!", 5, pos2.EndCol);
		}
		
		[Test]
		public void testChangingPositionsTo9()
		{
			Position pos = new Position(6, 3, 7, 4);

			PositionCollection col = pos.ChangingPositionsTo(new Position(3, 3, 6, 3));
			AssertEquals("Number of positions is wrong!", 1, col.Count);
			
			IEnumerator enumerator = col.GetEnumerator();
			enumerator.MoveNext();
			Position pos1 = enumerator.Current as Position;
			AssertEquals("StartLine is wrong!", 3, pos1.StartLine);
			AssertEquals("StartCol is wrong!", 3, pos1.StartCol);
			AssertEquals("EndLine is wrong!", 7, pos1.EndLine);
			AssertEquals("EndCol is wrong!", 4, pos1.EndCol);
		}
	}
}
