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
using Opnieuw.Framework;

namespace Opnieuw.Parsers.CSParser
{
	public interface TypeDeclaration : NamespaceMember, ClassMember, NamedMember
	{
	}

	public class MissingTypeDeclaration : TypeDeclaration
	{
		public string Name {
			get {
				return "";
			}
		}

		public NamespaceMember LookupType(string name)
		{
			return new MissingNamespaceMember();
		}

		public string FullyQualifiedParentNamespace {
			get {
				return "";
			}
		}

		public virtual VariableCollection DeclaredVariables {
			get {
				return new VariableCollection();
			}
		}

		public virtual VariableCollection ModifiedVariables {
			get {
				return new VariableCollection();
			}
		}
	}
}
