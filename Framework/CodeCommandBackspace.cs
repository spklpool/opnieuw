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
	public class CodeCommandBackspace : CodeCommand
	{
		Code m_Code = null;
		CodeSelection m_Selection = null;
		string m_SelectedText = "";
		bool m_IsDone = false;

		public CodeCommandBackspace(Code code)
		{
			m_Code = code;
		}

		public override void Do()
		{
			if (m_IsDone == false)
			{
				if (m_Code.Selection.IsZeroLength)
				{
					m_Code.ExtendSelectionLeft();
				}
				m_Selection = new CodeSelection(m_Code.Selection);
				m_SelectedText = m_Code.SelectedText;
				m_Code.DeleteSelectedText();
				m_IsDone = true;
			}
		}

		public override void Undo()
		{
			if (m_IsDone == true)
			{
				m_Code.ResetSelection(m_Selection.StartLine, m_Selection.StartCol, 
									  m_Selection.StartLine, m_Selection.StartCol);
				m_Code.InsertTextInSelection(m_SelectedText);
				m_IsDone = false;
			}
		}

		public override void Redo()
		{
			if (m_IsDone == false)
			{
				m_Code.ResetSelection(m_Selection.StartLine, m_Selection.StartCol, 
									  m_Selection.EndLine, m_Selection.EndCol);
				m_Code.DeleteSelectedText();
				m_IsDone = true;
			}
		}
	}
}
