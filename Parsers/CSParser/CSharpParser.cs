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
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;

namespace Opnieuw.Parsers.CSParser
{
	public class CSharpParser : ICodeParser
	{
		public CodeCompileUnit Parse(TextReader codeStream)
		{
			CodeCompileUnit ret = null;
			Tokenizer tokenizer = new Tokenizer(codeStream, "", null);
			CompilationUnit unit = CompilationUnit.Parse(tokenizer);
			if (false == (unit is MissingCompilationUnit))
			{
				ret = new CodeCompileUnit();
				PopulateCodeCompileUnit(unit, ret);
				ret.UserData.Add("SourceCompilationUnit", unit);
			}
			return ret;
		}

		public void PopulateCodeCompileUnit(CompilationUnit unit, CodeCompileUnit codeDomUnit)
		{
			CodeNamespace defaultNamespace = new CodeNamespace();
			foreach (UsingDirective ud in unit.UsingDirectives)
			{
				CodeNamespaceImport i = new CodeNamespaceImport(ud.NamespaceOrTypeName.Name);
				defaultNamespace.Imports.Add(i);
			}
			foreach (NamespaceMember m in unit.Members)
			{
				if (m is TypeDeclaration)
				{
//					AddTypeToNamespace(defaultNamespace, m as TypeDeclaration);
				}
			}
			codeDomUnit.Namespaces.Add(defaultNamespace);
			foreach (NamespaceMember m in unit.Members)
			{
				if (m is Namespace)
				{
					Namespace sourceNamespace = m as Namespace;
					CodeNamespace ns = new CodeNamespace(sourceNamespace.Name);
					codeDomUnit.Namespaces.Add(ns);
					foreach (UsingDirective namespaceUsingDirective in unit.UsingDirectives)
					{
						string name = namespaceUsingDirective.NamespaceOrTypeName.Name;
						CodeNamespaceImport namespaceImport = new CodeNamespaceImport(name);
						ns.Imports.Add(namespaceImport);
					}
					foreach (NamespaceMember nm in sourceNamespace.Members)
					{
						if (nm is TypeDeclaration)
						{
//							AddTypeToNamespace(ns, nm as TypeDeclaration);
						}
					}
				}
			}
		}

//		private void AddTypeToNamespace(CodeNamespace ns, TypeDeclaration type)
//		{
//			CodeTypeDeclaration codeType = new CodeTypeDeclaration(type.Name);
//			codeType.IsClass = type is Class;
//			codeType.IsEnum = type is EnumDeclaration;
//			codeType.IsInterface = type is Interface;
//			codeType.IsStruct = type is StructDeclaration;
//		}
	}
}
