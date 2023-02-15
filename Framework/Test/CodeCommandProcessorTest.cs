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
using NUnit.Framework;
using Opnieuw.Framework;

namespace Opnieuw.Framework.Test
{
	public class TestCodeCommand : CodeCommand
	{
		public bool DoWasCalled = false;
		public bool UndoWasCalled = false;
		public bool RedoWasCalled = false;
		
		public override void Do()
		{
			DoWasCalled = true;
		}
		
		public override void Undo()
		{
			UndoWasCalled = true;
		}
		
		public override void Redo()
		{
			RedoWasCalled = true;
		}
	}
	
	[TestFixture]
	public class CodeCommandProcessorTest : Assertion
	{
		[Test]
		public void testDoUndo1()
		{
			TestCodeCommand command = new TestCodeCommand();
			CodeCommandProcessor processor = new CodeCommandProcessor();
			AssertEquals(0, processor.UndoableCommandCount);
			AssertEquals(0, processor.RedoableCommandCount);
			
			Assert("Do should not have been called yet.", command.DoWasCalled == false);
			Assert("Undo should not have been called yet.", command.UndoWasCalled == false);
			
			processor.Do(command);

			Assert("Do should have been called by now.", command.DoWasCalled == true);
			Assert("Undo should not have been called yet.", command.UndoWasCalled == false);
			
			AssertEquals("Undoable command count is wrong.", 1,  processor.UndoableCommandCount);
			AssertEquals("Redoable command count is wrong.", 0, processor.RedoableCommandCount);
			
			processor.Undo();
			
			AssertEquals("Undoable command count is wrong.", 0,  processor.UndoableCommandCount);
			AssertEquals("Redoable command count is wrong.", 1, processor.RedoableCommandCount);
			
			Assert(command.DoWasCalled == true);
			Assert(command.UndoWasCalled == true);
		}
	}
}