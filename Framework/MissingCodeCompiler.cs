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
using System.CodeDom.Compiler;

namespace Opnieuw.Framework
{
	public class MissingCodeCompiler : ICodeCompiler
	{
		public CompilerResults CompileAssemblyFromDom(CompilerParameters options, CodeCompileUnit compilationUnit)
		{
			return new CompilerResults(new TempFileCollection());
		}

		public CompilerResults CompileAssemblyFromDomBatch(CompilerParameters options, CodeCompileUnit[] compilationUnits)
		{
			return new CompilerResults(new TempFileCollection());
		}

		public CompilerResults CompileAssemblyFromFile(CompilerParameters options, string fileName)
		{
			return new CompilerResults(new TempFileCollection());
		}

		public CompilerResults CompileAssemblyFromFileBatch(CompilerParameters options,	string[] fileNames)
		{
			return new CompilerResults(new TempFileCollection());
		}

		public CompilerResults CompileAssemblyFromSource(CompilerParameters options, string source)
		{
			return new CompilerResults(new TempFileCollection());
		}

		public CompilerResults CompileAssemblyFromSourceBatch(CompilerParameters options, string[] sources)
		{
			return new CompilerResults(new TempFileCollection());
		}
	}
}
