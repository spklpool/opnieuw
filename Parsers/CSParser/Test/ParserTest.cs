#region Copyright (C) Pierre Boudreau
//This file is part of the Opnieuw C# parser.
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
using NUnit.Framework;
using System.IO;
using Opnieuw.Parsers.CSParser;

namespace Opnieuw.Parsers.CSParser.Test
{
	public class ParserTest : Assertion
	{
		protected string m_FileName = "TestFile.cs";

		protected void WriteTestFile(string fileString, string fileName) {
			if (System.IO.File.Exists(m_FileName))
			{
				System.IO.File.Delete(m_FileName);
			}
			System.IO.FileStream fs = null;
			System.IO.StreamWriter sw = null;
			fs = System.IO.File.OpenWrite(fileName);
			sw = new System.IO.StreamWriter(fs);
			sw.Write(fileString);
			sw.Flush();
			sw.Close();
		}
		
		protected CompilationUnit ParseTestFile(string fileString)
		{
			WriteTestFile(fileString, m_FileName);
			FileStream fs = File.OpenRead(m_FileName);
			Tokenizer t = new Tokenizer(fs, "", null);
			CompilationUnit ret = CompilationUnit.Parse(t);
			fs.Close();
			return ret;
		}

		protected CompilationUnit ParseExistingFile(string fileName)
		{
			FileStream fs = File.OpenRead(fileName);
			Tokenizer t = new Tokenizer(fs, "", null);
			CompilationUnit ret = CompilationUnit.Parse(t);
			fs.Close();
			return ret;
		}
	}
}
