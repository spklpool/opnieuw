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
	/// <summary>
	/// The ViewControler is a point of contact for the multiple views
	/// of the data.  It coordinates activities and events between them
	/// and forwards information received from the outside world (mostly
	/// the model).
	/// </summary>
	public abstract class ViewControler
	{
		public event ParsingStartedEventHandler ParsingStarted;
		public event ParsingDoneEventHandler ParsingDone;
		public event ParsingFileEventHandler ParsingFile;
		public event ParsingFileDoneEventHandler ParsingFileDone;
		public event StatusChangedEventHandler StatusChanged;
		public event CodeTreeViewEventHandler NodeSelectionChanged;

		protected abstract void AddCodePage(CodeCompileUnit unit);
		public abstract CodeCompileUnit Parse(string fileName);
		public abstract CodeDomProvider CodeDomProvider {
			get;
		}

		/// <summary>
		/// Registers the controler to the events it needs to listen to.
		/// </summary>
		private void RegisterEventHandlers()
		{
			Model.ParsingStarted += new ParsingStartedEventHandler(this.Model_ParsingStarted);
			Model.ParsingFileDone += new ParsingFileDoneEventHandler(this.Model_ParsingFileDone);
			Model.ParsingFile += new ParsingFileEventHandler(this.Model_ParsingFile);
			Model.ParsingDone += new ParsingDoneEventHandler(this.Model_ParsingDone);
		}

		/// <summary>
		/// This is a helper to fire the StatusChanged event to all listeners.
		/// </summary>
		public void FireStatusChanged(string newStatus) 
		{
			if (null != StatusChanged) 
			{
				StatusChanged(newStatus);
			}
		}

		/// <summary>
		/// Constructor that initializes the model used by this controler.
		/// </summary>
		public ViewControler(Model model)
		{
			m_Model = model;
			RegisterEventHandlers();
		}

		/// <summary>
		/// Returns the model.
		/// </summary>
		protected Model m_Model = null;
		public Model Model {
			get {
				return m_Model;
			}
		}

		/// <summary>
		/// Helper to fire the ParsingStarted event.
		/// </summary>
		protected void FireParsingStarted(int fileCount) {
			if (null != ParsingStarted)	{
				ParsingStarted(fileCount);
			}
		}

		/// <summary>
		/// Helper to fire the ParsingDone event.
		/// </summary>
		protected void FireParsingDone() {
			if (null != ParsingDone) {
				ParsingDone();
			}
		}

		/// <summary>
		/// Helper to fire the ParsingFile event.
		/// </summary>
		protected void FireParsingFile(string fileName) {
			if (null != ParsingFile) {
				ParsingFile(fileName);
			}
		}

		/// <summary>
		/// Helper to fire the ParsingFileDone event.
		/// </summary>
		protected void FireParsingFileDone(string fileName, CodeCompileUnit unit, bool success) {
			if (null != ParsingFileDone) {
				ParsingFileDone(fileName, unit, success);
			}
		}

		/// <summary>
		/// Helper to fire the NodeSelectionChanged event.
		/// </summary>
		protected void FireNodeSelectionChanged(object source, ArrayList selectedNodes) {
			if (null != NodeSelectionChanged) {
				NodeSelectionChanged(source, selectedNodes);
			}
		}

		/// <summary>
		/// Removes the specified file from the tab pages.
		/// </summary>
		protected void RemoveTabPage(string name)
		{
			CodeViewTabs.RemovePage(name);
		}

		/// <summary>
		/// Returns the file name and extension from a sting
		/// containing the full path.
		/// </summary>
		protected string FileNameFromPath(string fileName)
		{
			return fileName.Substring(fileName.LastIndexOf("\\") + 1, fileName.Length - fileName.LastIndexOf("\\") - 1);
		}
		
		/// <summary>
		/// Event handler that gets called when the parsing of
		/// files is completed.
		/// </summary>
		private void Model_ParsingDone() {
			CodeTreeView.EndReloading();
			FireParsingDone();
		}

		/// <summary>
		/// Event handler that gets called when the parsing of
		/// a file is starting.
		/// </summary>
		private void Model_ParsingFile(string fileName) {
			CodeTreeView.Refresh();
			RemoveFileFromCodeTreeView(fileName);
			FireParsingFile(fileName);
		}

		protected abstract void RemoveFileFromCodeTreeView(string fileName);

		/// <summary>
		/// Event handler that gets called when the parsing of
		/// a file has completed.
		/// </summary>
		private void Model_ParsingFileDone(string fileName, CodeCompileUnit unit, bool success) {
			if (success == true)
			{
				CodeTreeView.PopulateTree(unit);
				AddCodePage(unit);
			}
			FireParsingFileDone(fileName, unit, success);
		}

		/// <summary>
		/// Event handler that gets called when the parsing of
		/// files has started.
		/// </summary>
		private void Model_ParsingStarted(int fileCount) {
			CodeTreeView.StartReloading();
			FireParsingStarted(fileCount);
		}

		/// <summary>
		/// Returns all the compilation units currently contained in the model.
		/// </summary>
		public CodeCompileUnitCollection CodeCompileUnits {
			get {
				return Model.CodeCompileUnits;
			}
		}

		/// <summary>
		/// Returns the tree view with the code outline.
		/// </summary>
		protected ICodeTreeView m_CodeTreeView = null;
		public ICodeTreeView CodeTreeView {
			get {
				return m_CodeTreeView;
			}
			set {
				m_CodeTreeView = value;
				m_CodeTreeView.NodeSelectionChangedWithInfo += new CodeTreeViewEventHandler(this.CodeTreeView_NodeSelectionChanged);
			}
		}

		/// <summary>
		/// Returns the tab control that contains all the code views.
		/// </summary>
		protected ICodeTabControl m_CodeViewTabs = null;
		public ICodeTabControl CodeViewTabs{
			get {
				return m_CodeViewTabs;
			}
			set {
				m_CodeViewTabs = value;
			}
		}

		/// <summary>
		/// Clears the contents of all the views that the 
		/// controler is controling.
		/// </summary>
		public void Clear()
		{
			CodeCompileUnits.Clear();
			CodeViewTabs.ClearPages();
			CodeTreeView.ClearNodes();
			Model.Clear();
		}

		/// <summary>
		/// Parses the specified files.
		/// </summary>
		public void Parse(string[] fileNames)
		{
			Model.Parse(fileNames);
		}

		/// <summary>
		/// Reloads all the views that the controler is controling.
		/// </summary>
		public void ReloadAllFiles()
		{
			CodeViewTabs.ClearPages();
			CodeTreeView.ClearNodes();
			Model.ReloadAllFiles();
		}

		/// <summary>
		/// Selects nodes in the model contained within the bounds of the specified
		/// position in the specified file.
		/// </summary>
		public void SelectNodesByPositionAndFile(Position position, string fileName)
		{
			Model.SelectNodesByPositionAndFile(position, fileName);
		}

		/// <summary>
		/// Event handler that gets called when the selection changes in the code view.
		/// </summary>
		public void SelectionChanged(Position position, string fileName)
		{
			Model.SelectNodesByPositionAndFile(position, fileName);
			FireNodeSelectionChanged(CodeViewTabs, Model.SelectedNodes);
			CodeTreeView.Select(Model.SelectedNodes);
			//FireStatusChanged(position.ToString() + " - " + Model.StripTabs(position, fileName).ToString());
			FireStatusChanged(position.ToString());
		}

		/// <summary>
		/// Event handler that gets called when the node selection changes in the tree view.
		/// </summary>
		/// <param name="selectedNodes"></param>
		public virtual void CodeTreeView_NodeSelectionChanged(object source, ArrayList selectedNodes)
		{
			Model.SelectNodes(selectedNodes);
			FireNodeSelectionChanged(source, selectedNodes);
			CodeViewTabs.Select(selectedNodes);
		}
	}
}
