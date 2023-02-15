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

namespace Opnieuw.Parsers.CSParser
{
	public class Variable
	{
		public Variable()
		{
		}

		public Variable(string name, string type, VariableInitializer initialValue, PieceOfCode containingScope)
		{
			m_Name = name;
			m_Type = DataType.parse(type);
			m_InitialValue = initialValue;
            m_ContainingScope = containingScope;
		}

		public Variable(string name, DataType type, VariableInitializer initialValue, PieceOfCode containingScope)
		{
			m_Name = name;
			m_Type = type;
			m_InitialValue = initialValue;
            m_ContainingScope = containingScope;
		}

		public Variable(string name, DataType type, ModifierCollection modifiers, PieceOfCode containingScope)
		{
			m_Name = name;
			m_Type = type;
			m_InitialValue = new MissingVariableInitializer();
			m_Modifiers = modifiers;
            m_ContainingScope = containingScope;
		}

		string m_Name;
		public string Name {
			get {
				return m_Name;
			}
		}

		DataType m_Type = new MissingDataType();
		public DataType Type {
			get {
				return m_Type;
			}
			set {
				m_Type = value;
			}
		}

		VariableInitializer m_InitialValue = null;
		public VariableInitializer InitialValue {
			get {
				return m_InitialValue;
			}
		}

		private ModifierCollection m_Modifiers = new ModifierCollection();
		public ModifierCollection Modifiers {
			get {
				return m_Modifiers;
			}
			set {
				m_Modifiers = value;
			}
		}

        protected PieceOfCode m_ContainingScope = new MissingPieceOfCode();
        public PieceOfCode ContainingScope {
            get {
                return m_ContainingScope;
            }
        }

		public bool IsPrivate {
			get {
				//defaults to private
				bool ret = true;
				foreach (Modifier m in Modifiers)
				{
					if ((m.Name == "protected") ||
						(m.Name == "public"))
					{
						ret = false;
					}
				}
				return ret;
			}
		}

		public bool IsProtected {
			get {
				bool ret = false;
				foreach (Modifier m in Modifiers)
				{
					if (m.Name == "protected")
					{
						ret = true;
					}
				}
				return ret;
			}
		}

		public bool IsPublic {
			get {
				bool ret = false;
				foreach (Modifier m in Modifiers)
				{
					if (m.Name == "public")
					{
						ret = true;
					}
				}
				return ret;
			}
		}
	}

	public class MissingVariable : Variable
	{
	}
}
