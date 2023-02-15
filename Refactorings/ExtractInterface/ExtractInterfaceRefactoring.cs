#region Copyright (C) Pierre Boudreau
//This file is part of the Opnieuw project.
//
//pierre.boudreau@alphacentauri.biz
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
using System.Windows.Forms;
using Opnieuw.Parsers.CSParser;
using Opnieuw.Framework;

namespace Opnieuw.ExtractInterface
{
	/// <summary>
	/// This is the class for the Extract Interface refactoring.
	/// </summary>
	public class ExtractInterfaceRefactoring : CSRefactoring
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public ExtractInterfaceRefactoring()
		{
		}

		/// <summary>
		/// Prepares a string with the default interface name based
		/// on the selected nodes.  The default name is IClassName.
		/// </summary>
		public string InterfaceName {
			get {
				string ret = "";
				if (PiecesOfCode.RootClass != null)
				{
					ret = "I" + PiecesOfCode.RootClass.Name;
				}
				return ret;
			}
		}

		/// <summary>
		/// Determines if the refactoring is available with the currently selected nodes.
		/// </summary>
		public override bool IsAvailable {
			get {
				bool ret = true;
				if ((PiecesOfCode.Count < 0) || (null == PiecesOfCode.RootClass)) {
					ret = false;
				} else {
					foreach (PieceOfCode poc in PiecesOfCode) {
						if ((false == (poc is MethodDeclaration)) && 
							(false == (poc is PropertyDeclaration))) {
							ret = false;
							break;
						}
					}
				}
				return ret;
			}
		}

		public override CodeCompileUnitCollection ChangingCodeCompileUnits {
			get {
				CodeCompileUnitCollection ret = new CodeCompileUnitCollection();
				if (IsAvailable) {
					ret.Add(CreateNewInterfaceCodeCompileUnit());
					ret.Add(CreateExistingClassCodeCompileUnit());
				}
				return ret;
			}
		}

		private CodeCompileUnit CreateExistingClassCodeCompileUnit()
		{
			PositionCollection highlightPositions1 = new PositionCollection();
			CompilationUnit workingClone = CompilationUnit.parse(PiecesOfCode.RootCompilationUnit.Generate());
			Class workingCloneClass = workingClone.FromTrailOfBreadCrumbs(PiecesOfCode.RootClass.Crumbs) as Class;
			CompilationUnit ret = null;
			if (workingCloneClass.Base is MissingBaseTypeList) {
				DataTypeCollection types = new DataTypeCollection();
				BaseTypeList baseTypeList = new BaseTypeList(types);
				workingCloneClass.Base = baseTypeList;
				workingCloneClass.Base.TypeCollection.Add(new DataType(InterfaceName));
				ret = CompilationUnit.parse(workingClone.Generate());
				Class newWorkingCloneClass = ret.FromTrailOfBreadCrumbs(PiecesOfCode.RootClass.Crumbs) as Class;
				highlightPositions1.Add(newWorkingCloneClass.Base.Position);
			} else {
				workingCloneClass.Base.TypeCollection.Add(new DataType(InterfaceName));
				ret = CompilationUnit.parse(workingClone.Generate());
				Class newWorkingCloneClass = ret.FromTrailOfBreadCrumbs(PiecesOfCode.RootClass.Crumbs) as Class;
				highlightPositions1.Add(newWorkingCloneClass.Base.TypeCollection[newWorkingCloneClass.Base.TypeCollection.Count-1].Position);
			}
			return WrapInCodeCompileUnit(ret, PiecesOfCode.RootCompilationUnit.SourceFilePath, highlightPositions1);
		}

		private CodeCompileUnit CreateNewInterfaceCodeCompileUnit()
		{
			CompilationUnit newInterfaceUnit = CreateNewInterface();
			CompilationUnit newUnitCopy = CompilationUnit.parse(newInterfaceUnit.Generate());
			PositionCollection highlightPositions2 = new PositionCollection();
			highlightPositions2.Add(newUnitCopy.Namespaces[0].Position);
            String newFileName = PiecesOfCode.RootCompilationUnit.SourceFilePath;
            newFileName = newFileName.Substring(0, newFileName.LastIndexOf('\\'));
            newFileName += "\\" + InterfaceName + ".cs";
			return WrapInCodeCompileUnit(newUnitCopy, newFileName, highlightPositions2);
		}

		private CompilationUnit CreateNewInterface()
		{
			InterfaceMemberCollection members = new InterfaceMemberCollection();
			foreach (PieceOfCode poc in PiecesOfCode)	
			{
				InterfaceMember interfaceMember = null;
				if (poc is MethodDeclaration) {
					MethodDeclaration member = poc as MethodDeclaration;
					interfaceMember = new InterfaceMethod(DataType.parse(member.Type.Name),
														  member.Name, 
														  member.Parameters) as InterfaceMember;
				}
				if (poc is PropertyDeclaration) {
					PropertyDeclaration member = poc as PropertyDeclaration;
					InterfaceGetAccessor getAccessor = new EmptyInterfaceGetAccessor();
					InterfaceSetAccessor setAccessor = new EmptyInterfaceSetAccessor();
					if (!(member.AccessorDeclarations.GetAccessor is MissingAccessorDeclaration)) {
						getAccessor = new InterfaceGetAccessor();
					}
					if (!(member.AccessorDeclarations.SetAccessor is MissingAccessorDeclaration)) {
						setAccessor = new InterfaceSetAccessor();
					}
					InterfaceAccessors accessors = new InterfaceAccessors(getAccessor, setAccessor);
					InterfaceProperty prop = new InterfaceProperty(DataType.parse(member.Type.Name), member.Name, accessors); 
					interfaceMember = prop as InterfaceMember;
				}
				if (null != interfaceMember) {
					members.Add(interfaceMember); 
				}
			}
			Interface newInterface = new Interface(InterfaceName, new MissingBaseTypeList(), members);
			NamespaceMemberCollection namespaceMembers = new NamespaceMemberCollection();
			namespaceMembers.Add(newInterface);
			Namespace ns = new Namespace(PiecesOfCode.RootNamespace.Identifier, namespaceMembers);
			NamespaceMemberCollection unitMembers = new NamespaceMemberCollection();
			unitMembers.Add(ns);
            CompilationUnit ret = new CompilationUnit(4, new UsingDirectiveCollection(), 
													  new AttributeSectionCollection(), 
													  unitMembers);
            ret.Format();
			return ret;
		}
	}
}
