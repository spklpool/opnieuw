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
using NUnit.Framework;
using Opnieuw.Framework;
using Opnieuw.Parsers.CSParser;

namespace Opnieuw.Parsers.CSParser.Test
{
	[TestFixture]
	public class CSViewControlerTest : TokenizerTestBase
	{
		[Test]
		public void testMultiFile1()
		{
			string fileString1 = "namespace ns {}";
			WriteTestFile(fileString1, "TestFile1.cs");
			string fileString2 = "namespace ns {}";
			WriteTestFile(fileString2, "TestFile2.cs");

			string[] fileNames = new String[2] {"TestFile1.cs", "TestFile2.cs"};

			ViewControler controler = new CSViewControler();
			controler.Parse(fileNames);
			Assertion.AssertEquals("Number of units is wrong!", 2, controler.CodeCompileUnits.Count);
		}
	}
}
