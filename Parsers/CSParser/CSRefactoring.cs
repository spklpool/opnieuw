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
using System.CodeDom;
using System.Collections;

using Opnieuw.Framework;

namespace Opnieuw.Parsers.CSParser
{
    public class CSRefactoring : Refactoring
    {
        public event RefactoringChangedHandler Changed;

        protected bool m_PreviousAvailability = false;

        public CSRefactoring()
        {
        }

		/// <summary>
		/// Checks if the event has any consumers and fires it.
		/// </summary>
		protected void FireChanged()
		{
			if (Changed != null) {
				Changed();
			}
		}

		/// <summary>
		/// Determines if the refactoring is available with the currently 
		/// selected nodes.
		/// </summary>
		public virtual bool IsAvailable {
			get {
				return false;
			}
		}

		protected CodeCompileUnit WrapInCodeCompileUnit(CompilationUnit unit, String sourceFileName, PositionCollection highlightPositions)
		{
			CodeCompileUnit ret = new CodeCompileUnit();
			CSharpParser parser2 = new CSharpParser();
			parser2.PopulateCodeCompileUnit(unit, ret);
			ret.UserData.Add("SourceCompilationUnit", unit);
			ret.UserData.Add("SourceFilePath", sourceFileName);
			ret.UserData.Add("HighlightPositions", highlightPositions);
			return ret;			
		}

		/// <summary>
		/// Returns the code changes necessary to execute the refactoring with the 
		/// current properties.
		/// </summary>
		public override CodeChangeCommandCollection CodeChanges {
			get 
			{
				CodeChangeCommandCollection ret = new CodeChangeCommandCollection();
                foreach (CodeCompileUnit unit in ChangingCodeCompileUnits) {
                    CompilationUnit csUnit = unit.UserData["SourceCompilationUnit"] as CompilationUnit;
                    String filePath = unit.UserData["SourceFilePath"] as String;
                    CodeReplacementCollection replacements = new CodeReplacementCollection();
                    replacements.Add(new CodeReplacement(new InvalidPosition(), csUnit.Generate()));
                    ReplaceCodeChangeCommand replaceCommand = new ReplaceCodeChangeCommand(filePath, replacements);
                    if (!System.IO.File.Exists(filePath)) {
                        CreateFileCodeChangeCommand createFileCommand = new CreateFileCodeChangeCommand(filePath);
                        ret.Add(createFileCommand);
                    }
                    ret.Add(replaceCommand);
                }
                return ret;
			}
		}

		/// <summary>
		/// Event handler that gets called when the node selection changes in the model.
		/// </summary>
		public override void OnNodeSelectionChanged(object source, ArrayList selectedNodes)
		{
			base.OnNodeSelectionChanged(source, selectedNodes);
			if (IsAvailable != m_PreviousAvailability)
			{
				FireAvailabilityChanged(IsAvailable);
				m_PreviousAvailability = IsAvailable;
			}
			FireChanged();
		}

		/// <summary>
		/// Returns the selected nodes as a PieceOfCodeCollection.
		/// </summary>
		public PieceOfCodeCollection PiecesOfCode {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				foreach (object obj in m_SelectedNodes)
				{
					if (obj is PieceOfCode)
					{
						ret.Add(obj as PieceOfCode);
					}
				}
				return ret;
			}
		}
    }
}
