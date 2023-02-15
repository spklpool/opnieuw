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
using System.IO;
using NUnit.Framework;

namespace Opnieuw.Parsers.CSParser.Test
{
	[TestFixture]
	public class TokenizerTestBase : ParserTest
	{
		protected FileStream fs = null;
		
		[SetUp]
		public void SetUp()
		{
			if (System.IO.File.Exists(m_FileName))
			{
				System.IO.File.Delete(m_FileName);
			}
		}

		[TearDown]
		public void TearDown()
		{
 			if (fs != null)
 			{
 				fs.Close();
 			}
 			System.IO.File.Delete("TestFile.cs");
 			System.IO.File.Delete("TestFile1.cs");
 			System.IO.File.Delete("TestFile2.cs");
		}

		public Tokenizer TokenizeTestFile(string fileString)
		{
			WriteTestFile(fileString, m_FileName);
			fs = File.OpenRead(m_FileName);
			Tokenizer ret = new Tokenizer(fs, "", null);
			return ret;
		}
	}
}