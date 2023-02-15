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
using System.CodeDom;
using System.CodeDom.Compiler;

namespace Opnieuw.Framework
{
	public delegate void NodeSelectionChangedEventHandler(ArrayList selectedNodes);

	public abstract class Model
	{
		//Parsing events
		public event ParsingStartedEventHandler ParsingStarted;
		public event ParsingDoneEventHandler ParsingDone;
		public event ParsingFileEventHandler ParsingFile;
		public event ParsingFileDoneEventHandler ParsingFileDone;
		public event NodeSelectionChangedEventHandler NodeSelectionChanged;

		public abstract Code CreateCode(string filename);
		public abstract Code CreateCode(CodeCompileUnit unit);
		public abstract CodeCompileUnit Parse(string filename);
		public abstract void SelectNodesByPositionAndFile(Position position, string fileName);
		public abstract Position StripTabs(Position position, string fileName);
		
		protected CodeCompileUnitCollection m_CodeCompileUnitCollection = new CodeCompileUnitCollection();
		public CodeCompileUnitCollection CodeCompileUnits {
			get {
				return m_CodeCompileUnitCollection;
			}
		}

		protected ArrayList m_SelectedNodes = new ArrayList();
		public ArrayList SelectedNodes {
			get {
				return m_SelectedNodes;
			}
		}

		public void Clear()
		{
			CodeCompileUnits.Clear();
			SelectedNodes.Clear();
		}

		protected void FireParsingStarted(int fileCount) {
			if (ParsingStarted != null)	{
				ParsingStarted(fileCount);
			}
		}

		protected void FireParsingDone() {
			if (ParsingDone != null) {
				ParsingDone();
			}
		}

		protected void FireParsingFile(string fileName) {
			if (ParsingFile != null) {
				ParsingFile(fileName);
			}
		}

		protected void FireParsingFileDone(string fileName, CodeCompileUnit unit, bool success) {
			if (ParsingFileDone != null) {
				ParsingFileDone(fileName, unit, success);
			}
		}

		protected void FireNodeSelectionChanged(ArrayList selectedNodes)
		{
			if (NodeSelectionChanged != null)
			{
				NodeSelectionChanged(selectedNodes);
			}
		}

		public CodeCompileUnitCollection Parse(string[] fileNames)
		{
			CodeCompileUnitCollection ret = new CodeCompileUnitCollection();
			FireParsingStarted(fileNames.GetLength(0));
			foreach (string filename in fileNames) 
			{
				FireParsingFile(filename);
				CodeCompileUnit unit = Parse(filename);
				if (unit != null) 
				{
					m_CodeCompileUnitCollection.Add(unit);
					ret.Add(unit);
				}
				FireParsingFileDone(filename, unit, unit != null);
			}
			FireParsingDone();
			return ret;
		}

		public void ReloadAllFiles()
		{
			CodeCompileUnitCollection temp = new CodeCompileUnitCollection();
			FireParsingStarted(m_CodeCompileUnitCollection.Count);
			foreach (CodeCompileUnit unit in m_CodeCompileUnitCollection)
			{
				string fileName = (string)unit.UserData["SourceFilePath"];
				FireParsingFile(fileName);
				CodeCompileUnit newUnit = Parse(fileName);
				if (newUnit != null)
				{
					newUnit.UserData["SourceFilePath"] = fileName;
					newUnit.UserData["Code"] = CreateCode(newUnit);
					temp.Add(newUnit);
				}
				FireParsingFileDone(fileName, newUnit, newUnit != null);
			}
			m_CodeCompileUnitCollection = temp;
			FireParsingDone();
		}

		public void SelectNodes(ArrayList selectedNodes)
		{
			m_SelectedNodes = selectedNodes;
			FireNodeSelectionChanged(selectedNodes);
		}
	}
}
