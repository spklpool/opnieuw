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
	public class EventAccessorDeclarations : PieceOfCode
	{
		public static EventAccessorDeclarations parse(TokenProvider tokenizer)
		{
			EventAccessorDeclarations ret = new MissingEventAccessorDeclarations();
			tokenizer.setBookmark();
			AddAccessorDeclaration addAccessor = AddAccessorDeclaration.parse(tokenizer);
			addAccessor.checkNotMissing();
			RemoveAccessorDeclaration removeAccessor = RemoveAccessorDeclaration.parse(tokenizer);
			removeAccessor.checkNotMissing();
			ret = new EventAccessorDeclarations(addAccessor, removeAccessor);
			tokenizer.endBookmark(ret is MissingEventAccessorDeclarations);
			return ret;
		}

		public EventAccessorDeclarations()
		{
			Pieces.Add(AddAccessorDeclaration.Missing);
			Pieces.Add(RemoveAccessorDeclaration.Missing);
		}

		public EventAccessorDeclarations(AddAccessorDeclaration addAccessor, 
										 RemoveAccessorDeclaration removeAccessor) :
		base(addAccessor, removeAccessor)
		{
		}

		public AddAccessorDeclaration AddAccessor {
			get {
				return Pieces[0] as AddAccessorDeclaration;
			}
		}

		public RemoveAccessorDeclaration RemoveAccessor {
			get {
				return Pieces[1] as RemoveAccessorDeclaration;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(AddAccessor);
				ret.Add(RemoveAccessor);
				return ret;
			}
		}
	}

	public class MissingEventAccessorDeclarations : EventAccessorDeclarations
	{
	}
}
