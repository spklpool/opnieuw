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
using System.Drawing;
using System.CodeDom;

namespace Opnieuw.Framework
{
	public delegate void CodeEventHandler();

	/// <summary>
	/// This represents a unit of code.
	/// </summary>
	public class Code
	{
		public event CodeEventHandler CurrentLineTextChanged;
		public event CodeEventHandler TextChanged;
		public event StatusChangedEventHandler StatusChanged;
		public event CodeSelectionChangeEventHandler SelectionChanging;
		public event CodeSelectionChangeEventHandler SelectionChanged;

		/// <summary>
		/// Standard constuctor.
		/// </summary>
		/// <param name="text"></param>
		public Code(string text)
		{
			m_Text = text;
			InitializeText();
		}

		/// <summary>
		/// Creates a Code object from a CompilationUnit
		/// </summary>
		/// <param name="CompilationUnit"></param>
		public Code(CodeCompileUnit compileUnit)
		{
			m_CompileUnit = compileUnit;
			SetTextFromCompileUnit();
			InitializeText();
		}
		
		protected CodeCompileUnit m_CompileUnit = null;

		protected virtual void SetTextFromCompileUnit()
		{
		}

		public void InitializeText()
		{
			m_Lines = new CodeLineCollection();
			if (m_Text != "")
			{
				string currentLine = "";
				for (int i=0; i<m_Text.Length; i++)
				{
					currentLine += m_Text[i];
					if(m_Text[i] == '\n')
					{
						m_Lines.Add(CreateLine(currentLine));
						currentLine = "";
					}
				}
				if (currentLine != "")
				{
					m_Lines.Add(CreateLine(currentLine));
				}
				InitializeLineParts();
			}
		}

		private void InitializeLineParts()
		{
			for (int i=0; i<m_Lines.Count; i++)
			{
				m_Lines[i].ResetTextAndParts(m_Lines[i].Text);
				m_Lines[i].SplitForSelection(Selection, i+1);
			}
		}

		private void ResetSelectionParts(int startLine, int endLine)
		{
			for (int i=0; i<m_Lines.Count; i++)
			{
				if ((i>=(startLine-1)) && (i<=(endLine-1)))
				{
					m_Lines[i].SplitForSelection(Selection, i+1);
				}
			}
		}
		
		private bool m_NotificationsEnabled = true;
		protected bool NotificationsEnabled {
			get {
				return m_NotificationsEnabled;
			}
			set {
				m_NotificationsEnabled = value;
			}
		}

		protected void FireCurrentLineTextChanged() {
			if (CurrentLineTextChanged != null) {
				CurrentLineTextChanged();
			}
		}

		protected void FireTextChanged() {
			if (TextChanged != null) {
				TextChanged();
			}
		}
		
		protected void FireStatusChanged(string newStatus) {
			if ((StatusChanged != null) && (NotificationsEnabled)){
				StatusChanged(newStatus);
			}
		}
	
		protected void FireSelectionChanged(Position pos) {
			if ((SelectionChanged != null) && (NotificationsEnabled)) {
				SelectionChanged(pos);
			}
		}
	
		protected void FireSelectionChanging(Position oldSelection) {
			if (SelectionChanging != null) {
				SelectionChanging(oldSelection);
			}
		}
		
		/// <summary>
		/// Creates a new CodeLine derived class with the specified text.  The 
		/// intention is that each language will provide its own Code and CodeLine 
		/// derived classes and override this method.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		protected virtual CodeLine CreateLine(string text)
		{
			return new CodeLine(text);
		}

		protected CodeSelection m_Selection = new CodeSelection(1, 1, 1, 1);
		public CodeSelection Selection {
			get {
				return m_Selection;
			}
			set {
				m_Selection = value;
			}
		}

		public string SelectedText {
			get {
				string ret = "";
				if (Selection.StartLine == Selection.EndLine)
				{
					if(Selection.StartCol != Selection.EndCol)
					{
						ret = Lines[Selection.StartLine-1].Substring(Selection.StartCol-1, (Selection.EndCol-1) - (Selection.StartCol-1));
					}
				}
				else
				{
					ret += Lines[Selection.StartLine-1].Substring(Selection.StartCol-1, Lines[Selection.StartLine-1].ExtendedLength - (Selection.StartCol-1)) + "\r\n";
					for (int i=Selection.StartLine+1; i<Selection.EndLine; i++)
					{
						ret += Lines[i-1].Text + "\r\n";
					}
					ret += Lines[Selection.EndLine-1].Substring(0, Selection.EndCol-1);
				}
				return ret;
			}
		}

		public void KeyPress(char keyCharacter)
		{
			//Test
			string insert = "";
			insert += keyCharacter;
			CurrentLine.Insert(Selection.EndCol-1, insert);
			m_Lines[Selection.StartLine-1].ResetTextAndParts(CurrentLine.Text);
			m_Lines[Selection.StartLine-1].SplitForSelection(Selection, Selection.StartLine);
			ResetTextFromCodeLines();
			FireCurrentLineTextChanged();
			MoveCursorRight();
		}

		public void DeleteSelectedText()
		{
			if (!Selection.IsZeroLength)
			{
				if (Selection.StartLine == Selection.EndLine)
				{
					if(Selection.StartCol != Selection.EndCol)
					{
						CurrentLine.Remove(Selection.StartCol-1, Selection.EndCol-1);
						ResetCursorPosition(Selection.StartLine, Selection.StartCol);
						m_Lines[Selection.StartLine-1].ResetTextAndParts(CurrentLine.Text);
						m_Lines[Selection.StartLine-1].SplitForSelection(Selection, Selection.StartLine);
						ResetTextFromCodeLines();
						FireCurrentLineTextChanged();
					}
				}
				else
				{
					string endPart = Lines[Selection.EndLine-1].Substring(Selection.EndCol-1, Lines[Selection.EndLine-1].ExtendedLength);
					Lines[Selection.StartLine-1].Remove(Selection.StartCol-1, Lines[Selection.StartLine-1].ExtendedLength);
					Lines[Selection.StartLine-1].Text += endPart;
					for (int i=Selection.EndLine; i>Selection.StartLine; i--)
					{
						Lines.RemoveAt(i-1);
					}
					ResetCursorPosition(Selection.StartLine, Selection.StartCol);
					ResetTextFromCodeLines();
					InitializeText();
					FireTextChanged();
				}
				FireSelectionChanged(Selection);
			}
		}

		public Position UnexpandTabs(Position position)
		{
			Position ret = new Position(position.StartLine, position.StartCol, 
										position.EndLine, position.EndCol);
			int i = 0;
			if (position.StartLine != position.EndLine)
			{
				for(i=0; i<Lines[ret.StartLine-1].Text.Length; i++)
				{
					if (i<=position.StartCol)
					{
						if (Lines[ret.StartLine-1].Text[i] == '\t') {
							ret.StartCol -= 3;
						}
					}
				}
				for(i=0; i<Lines[ret.EndLine-1].Text.Length; i++)
				{
					if (i<=position.EndCol)
					{
						if (Lines[position.EndLine-1].Text[i] == '\t') {
							ret.EndCol -= 3;
						}
					}
				}
			}
			else
			{
				int tempStartCol = 0;
				int tempEndCol = 0;
				for(i=0; i<Lines[ret.StartLine-1].Text.Length; i++)
				{
					int startAdjustment = 0;
					int endAdjustment = 0;
					if (Lines[ret.StartLine-1].Text[i] == '\t') 
					{
						if (i<=ret.StartCol)
						{
							startAdjustment = (4-(tempStartCol%4));
							ret.StartCol -= (startAdjustment-1);
						}
						if (i<=position.EndCol)
						{
							endAdjustment = (4-(tempEndCol%4));
							ret.EndCol -= (endAdjustment-1);
						}
						tempEndCol += startAdjustment;
						tempStartCol += endAdjustment;
					}
					else
					{
						tempStartCol ++;
						tempEndCol ++;
					}
				}
			}
			return ret;
		}

		public Position AdjustPositionForExpandedTabs(Position position)
		{
			Position ret = new Position(position.StartLine, position.StartCol, position.EndLine, position.EndCol);
			int i = 0;
			if (position.StartLine != position.EndLine)
			{
				for(i=0; i<Lines[ret.StartLine-1].Text.Length; i++)
				{
					if (i<=position.StartCol)
					{
						if (Lines[ret.StartLine-1].Text[i] == '\t') {
							ret.StartCol += 3;
						}
					}
				}
				for(i=0; i<Lines[ret.EndLine-1].Text.Length; i++)
				{
					if (i<=position.EndCol)
					{
						if (Lines[position.EndLine-1].Text[i] == '\t') {
							ret.EndCol += 3;
						}
					}
				}
			}
			else
			{
				for(i=0; i<Lines[ret.StartLine-1].Text.Length; i++)
				{
					if (Lines[ret.StartLine-1].Text[i] == '\t') {
						if (i<=position.StartCol)
						{
							ret.StartCol += 3;
						}
						if (i<=position.EndCol)
						{
							ret.EndCol += 3;
						}
					}
				}
			}
			return ret;
		}

		protected Color m_SelectedForeColor = Color.White;
		public Color SelectedForeColor {
			get {
				return m_SelectedForeColor;
			}
		}

		protected Color m_SelectedBackColor = Color.DarkBlue;
		public Color SelectedBackColor {
			get {
				return m_SelectedBackColor;
			}
		}

		protected Color m_ForeColor = Color.Black;
		public Color ForeColor {
			get {
				return m_ForeColor;
			}
		}

		protected Color m_BackColor = Color.White;
		public Color BackColor {
			get {
				return m_BackColor;
			}
		}

		/// <summary>
		/// Returns the entire code text.
		/// </summary>
		protected string m_Text = "";
		public string Text {
			get {
				return m_Text;
			}
			set {
				m_Text = value;
				InitializeText();
			}
		}
		
		/// <summary>
		/// Returns the entire code text broken up into lines.
		/// </summary>
		protected CodeLineCollection m_Lines = new CodeLineCollection();
		public CodeLineCollection Lines {
			get {
				return m_Lines;
			}
		}

		public void InsertLineAt(int index, string text)
		{
			m_Lines.InsertAfter(index, CreateLine(text));
		}

		public CodeLine CurrentLine {
			get {
				return Lines[Selection.EndLine-1];
			}
		}
		
		public CodeLine SelectionStartLine {
			get {
				return Lines[Selection.StartLine-1];
			}
		}

		public CodeLine SelectionEndLine {
			get {
				return Lines[Selection.EndLine-1];
			}
		}

		public void ResetSelectionFromUnexpandedPosition(Position position)
		{
			Position adj = AdjustPositionForExpandedTabs(position);
			ResetSelection(adj.StartLine, adj.StartCol, adj.EndLine, adj.EndCol+1);
		}

		public void ResetSelection(int startLine, int startCol, int endLine, int endCol)
		{
			if ((startLine == Selection.StartLine) && (startCol == Selection.StartCol) &&
				(endLine == Selection.EndLine) && (endCol == Selection.EndCol))
			{
				//Guard clause for no change.
				return;
			}
			FireSelectionChanging(Selection);
			int oldStartLine = Selection.StartLine;
			int oldEndLine = Selection.EndLine;
			Selection.StartLine = startLine;
			Selection.StartCol = startCol;
			Selection.EndLine = endLine;
			Selection.EndCol = endCol;
			if (Selection.StartCol > (Lines[Selection.StartLine-1].ExtendedLength+1))
			{
				Selection.StartCol = Lines[Selection.StartLine-1].ExtendedLength+1;
			}
			if (Selection.EndCol > (Lines[Selection.EndLine-1].ExtendedLength+1))
			{
				Selection.EndCol = Lines[Selection.EndLine-1].ExtendedLength+1;
			}
			if (Selection.StartLine > Selection.EndLine)
			{
				ResetSelection(Selection.EndLine, Selection.EndCol, Selection.StartLine, Selection.StartCol);
				Selection.IsCursorAtStart = true;
			}
			ResetSelectionParts(oldStartLine, oldEndLine);
			ResetSelectionParts(Selection.StartLine, Selection.EndLine);
			FireSelectionChanged(Selection);
		}

		public void ResetCursorPosition(int line, int col)
		{
			ResetSelection(line, col, line, col);
		}
		
		public void MoveCursorRight()
		{
			if (Selection.EndCol <= CurrentLine.ExtendedLength)
			{
				int adjustment = 1;
				if (CurrentLine.Substring(Selection.EndCol-1, 1) == "\t")
				{
					adjustment = 4-((Selection.EndCol-1)%4);
				}
				ResetCursorPosition(Selection.EndLine, Selection.EndCol+adjustment);
			}
			else
			{
				if (Selection.EndLine < Lines.Count)
				{
					ResetCursorPosition(Selection.EndLine+1, 1);
				}
			}
		}

		private void ReduceSelectionLeft()
		{
			if (Selection.StartCol <= SelectionStartLine.ExtendedLength)
			{
				int adjustment = 1;
				if (CurrentLine.Substring(Selection.StartCol-1, 1) == "\t")
				{
					adjustment = 4-((Selection.StartCol-1)%4);
				}
				ResetSelection(Selection.StartLine, Selection.StartCol+adjustment,
							   Selection.EndLine, Selection.EndCol);
			}
			else
			{
				ResetSelection(Selection.StartLine+1, 1,
							   Selection.EndLine, Selection.EndCol);
			}
		}

		public void ExtendSelectionRight()
		{
			if ((Selection.StartLine == Selection.EndLine) && 
				(Selection.StartCol == Selection.EndCol))
			{
				Selection.IsCursorAtEnd = true;
			}
			if (Selection.IsCursorAtEnd)
			{
				if (Selection.EndCol <= Lines[Selection.EndLine-1].ExtendedLength)
				{
					int adjustment = 1;
					if (Lines[Selection.EndLine-1].Substring(Selection.EndCol-1, 1) == "\t")
					{
						adjustment = 4-((Selection.EndCol-1)%4);
					}
					ResetSelection(Selection.StartLine, Selection.StartCol,
								   Selection.EndLine, Selection.EndCol+adjustment);
				}
				else
				{
					if (Selection.EndLine < Lines.Count)
					{
						ResetSelection(Selection.StartLine, Selection.StartCol, 
									   Selection.EndLine+1, 1);
					}
				}
			}
			else
			{
				ReduceSelectionLeft();
			}
		}
		
		public void ExtendSelectionToHome()
		{
			int startLine = Selection.StartLine;
			int startCol = Selection.StartCol;
			int endLine = Selection.EndLine;
			int endCol = Selection.EndCol;
			if (Selection.IsCursorAtStart) {
				startCol = 1;
			} else {
				if (Selection.StartLine != Selection.EndLine)
				{
					endCol = 1;
				}
				else
				{
					endCol = startCol;
					startCol = 1;
				}
			}
			ResetSelection(startLine, startCol, endLine, endCol);
		}
		
		public void ExtendSelectionToEnd()
		{
			int startLine = Selection.StartLine;
			int startCol = Selection.StartCol;
			int endLine = Selection.EndLine;
			int endCol = Selection.EndCol;
			if (Selection.IsCursorAtStart) {
				if (Selection.StartLine != Selection.EndLine) {
					startCol = Lines[Selection.StartLine-1].ExtendedLength+1;
				} else {
					startCol = endCol;
					endCol = Lines[Selection.EndLine-1].ExtendedLength+1;
				}
			} else {
				endCol = Lines[Selection.EndLine-1].ExtendedLength+1;
			}
			ResetSelection(startLine, startCol, endLine, endCol);
		}
		
		public void ExtendSelectionToPoint(int line, int col)
		{
			Position incomming = new Position(line, col, line, col);
			int startLine = Selection.StartLine;
			int startCol = Selection.StartCol;
			int endLine = Selection.EndLine;
			int endCol = Selection.EndCol;
			if (incomming.StartsBefore(Selection))
			{
				startLine = line;
				startCol = col;
			}
			else if (incomming.EndsAfter(Selection))
			{
				endLine = line;
				endCol = col;
			}
			ResetSelection(startLine, startCol, endLine, endCol);
		}
		
		public void MoveCursorLeft()
		{
			if (Selection.StartCol != Selection.EndCol) {
				ResetSelection(Selection.StartLine, Selection.StartCol, 
							   Selection.StartLine, Selection.StartCol);
			} else if (Selection.StartCol > 1) {
				if (CurrentLine.Substring(Selection.StartCol-2,1) == "\t") {
					ResetCursorPosition(Selection.StartLine, CurrentLine.LeftPositionSnap(Selection.StartCol-1));
				} else {
					ResetCursorPosition(Selection.StartLine, Selection.StartCol - 1);
				}
			} else if (Selection.StartCol <= 1) {
				if (Selection.StartLine > 1) {
					ResetCursorPosition(Selection.StartLine-1, Lines[Selection.StartLine-2].ExtendedLength+1);
				}
			}
		}

		public void ExtendSelectionLeft()
		{
			if ((Selection.StartLine == Selection.EndLine) && 
				(Selection.StartCol == Selection.EndCol))
			{
				Selection.IsCursorAtStart = true;
			}
			if (Selection.IsCursorAtStart) {
				if (Selection.StartCol > 1) {
					ResetSelection(Selection.StartLine, SelectionStartLine.LeftPositionSnap(Selection.StartCol-1), 
								   Selection.EndLine, Selection.EndCol);
				} else {
					if (Selection.StartLine > 1) {
						ResetSelection(Selection.StartLine-1, Lines[Selection.StartLine-2].ExtendedLength+1, 
									   Selection.EndLine, Selection.EndCol);
					}
				}
			} else {
				ReduceSelectionRight();
			}
		}

		public void ReduceSelectionRight()
		{
			if (Selection.EndCol > 1) {
				ResetSelection(Selection.StartLine, Selection.StartCol, 
							   Selection.EndLine, CurrentLine.LeftPositionSnap(Selection.EndCol-1));
			} else {
				if (Selection.EndLine > 1) {
					ResetSelection(Selection.StartLine, Selection.StartCol,
						Selection.EndLine-1, Lines[Selection.EndLine-1].ExtendedLength+1);
				}
			}
		}

		public void ExtendSelectionUp()
		{
			int startLine = Selection.StartLine;
			int startCol = Selection.StartCol;
			int endLine = Selection.EndLine;
			int endCol = Selection.EndCol;
			if ((startLine == endLine) && (startCol == endCol)) {
				Selection.IsCursorAtStart = true;
			}
			if (Selection.IsCursorAtStart) {
				if (Selection.StartLine > 1) {
					startLine --;
				} else {
					startCol = 1;
				}
			} else {
				endLine --;
			}
			ResetSelection(startLine, startCol, endLine, endCol);
		}
		
		public void ExtendSelectionDown()
		{
			int startLine = Selection.StartLine;
			int startCol = Selection.StartCol;
			int endLine = Selection.EndLine;
			int endCol = Selection.EndCol;
			if ((startLine == endLine) && (startCol == endCol)) {
				Selection.IsCursorAtEnd = true;
			}
			if (Selection.IsCursorAtStart) {
				if (Selection.StartLine < Lines.Count) {
					startLine ++;
				} else {
					if (Selection.StartLine == Selection.EndLine) {
						startLine = endLine;
						startCol = endCol;
						endCol = SelectionEndLine.ExtendedLength+1;
					}
				}
			} else {
				if (endLine < Lines.Count) {
					endLine ++;
					endCol = Lines[endLine-1].ClosestPositionSnap(endCol);
				} else {
					endCol = SelectionEndLine.ExtendedLength+1;
				}
				if (endCol > (SelectionEndLine.ExtendedLength+1)) {
					endCol = SelectionEndLine.ExtendedLength+1;
				}
			}
			ResetSelection(startLine, startCol, endLine, endCol);
		}

		public void MoveCursorUp()
		{
			if (Selection.EndLine > 1)
			{
				ResetCursorPosition(Selection.EndLine-1, Lines[Selection.EndLine-2].ClosestPositionSnap(Selection.EndCol));
			}
		}

		public void MoveCursorDown()
		{
			if (Selection.EndLine < Lines.Count)
			{
				ResetCursorPosition(Selection.EndLine+1, Lines[Selection.EndLine].ClosestPositionSnap(Selection.EndCol));
			}
		}
		
		public void Backspace()
		{
			FireSelectionChanging(Selection);
			NotificationsEnabled = false;
			if ((Selection.StartLine == Selection.EndLine) && (Selection.StartCol != Selection.EndCol))
			{
				DeleteSelectedText();
				ResetCursorPosition(Selection.StartLine, Selection.StartCol);
			}
			else if ((Selection.StartLine > 1) && (Selection.StartCol == 1))
			{
				ResetCursorPosition(Selection.StartLine-1, Lines[Selection.StartLine-2].ExtendedLength+1);
				string tempReplacementLine = CurrentLine.Text + Lines[Selection.StartLine].Text;
				Lines[Selection.StartLine-1].Text = tempReplacementLine;
				Lines.RemoveAt(Selection.StartLine);
				ResetTextFromCodeLines();
				FireTextChanged();
			}
			else
			{
				if (Selection.StartCol > 1)
				{
					ExtendSelectionLeft();
					DeleteSelectedText();
					FireCurrentLineTextChanged();
				}
				ResetTextFromCodeLines();
			}
			NotificationsEnabled = true;
			FireSelectionChanged(Selection);
		}
		
		public void Home()
		{
			ResetCursorPosition(Selection.EndLine, 1);
		}
		
		public void End()
		{
			FireCurrentLineTextChanged();
			ResetCursorPosition(Selection.EndLine, CurrentLine.ExtendedLength+1);
			FireCurrentLineTextChanged();
			FireSelectionChanged(Selection);
		}
		
		public void Delete()
		{
			if ((Selection.StartLine == Selection.EndLine) && 
				(Selection.StartCol == Selection.EndCol))
			{
				CurrentLine.Remove(Selection.StartCol-1, Selection.StartCol);
			}
			else
			{
				DeleteSelectedText();
			}
			ResetTextFromCodeLines();
			FireTextChanged();
			FireSelectionChanged(Selection);
		}
		
		public void Enter()
		{
			FireSelectionChanging(Selection);
			DeleteSelectedText();
			string textToInsert = CurrentLine.Substring(Selection.StartCol-1, CurrentLine.ExtendedLength-Selection.StartCol+1);
			CurrentLine.Text = CurrentLine.Substring(0, CurrentLine.ExtendedLength - (CurrentLine.ExtendedLength-Selection.StartCol+1));
			InsertLineAt(Selection.StartLine-1, textToInsert);
			ResetCursorPosition(Selection.StartLine+1, 1);
			ResetTextFromCodeLines();
			FireTextChanged();
			FireSelectionChanged(Selection);
		}
		
		public void InsertTextInSelection(string text)
		{
			DeleteSelectedText();
			string tempText = "";
			int i=0;
			foreach (CodeLine line in Lines)
			{
				if (i==(Selection.StartLine-1))
				{
					tempText += CurrentLine.Substring(0, Selection.StartCol-1);
					tempText += text;
					tempText += CurrentLine.Substring(Selection.StartCol-1, CurrentLine.ExtendedLength);
					tempText += "\r\n";
				}
				else
				{
					tempText += line.Text + "\r\n";
				}
				i++;
			}
			m_Text = tempText;
			InitializeText();
			//Now find our where the cursor should be
			int tempLine = Selection.StartLine;
			int tempCol = Selection.StartCol;
			Code cursorCalculation = new Code(text);
			if (cursorCalculation.Lines.Count > 1)
			{
				tempLine = Selection.StartLine + cursorCalculation.Lines.Count-1;
				tempCol = cursorCalculation.Lines[cursorCalculation.Lines.Count-1].ExtendedLength + 1;
			}
			else
			{
				cursorCalculation.Lines[0].Insert(0, CurrentLine.Substring(0, Selection.StartCol-1));
				tempCol = cursorCalculation.Lines[0].ExtendedLength + 1;
			}
			ResetCursorPosition(tempLine, tempCol);
			FireTextChanged();
		}

		private int m_WidestLine = 0;
		public int WidestLine {
			get {
				if (m_WidestLine == 0) {
					ResetTextFromCodeLines();
					return m_WidestLine;
				} else {
					return m_WidestLine;
				}
			}
		}

		public void ResetTextFromCodeLines()
		{
			System.Text.StringBuilder tempText = new System.Text.StringBuilder();
			foreach (CodeLine line in Lines)
			{
				if (line.ExtendedLength > m_WidestLine)
				{
					m_WidestLine = line.ExtendedLength;
				}
				tempText.Append(line.Text + "\r\n");
			}
			m_Text = tempText.ToString();
		}
		
		public void PageUp()
		{
		}
		
		public void PageDown()
		{
		}
	}
}
