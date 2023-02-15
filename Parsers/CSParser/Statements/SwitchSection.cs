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
    public class SwitchSection : PieceOfCode, StatementContainer
    {
		public static SwitchSection parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			SwitchSection ret = new MissingSwitchSection();
			SwitchLabelCollection labels = SwitchLabelCollection.parse(tokenizer);
			if (labels.Count > 0)
			{
				StatementCollection statements = StatementCollection.parse(tokenizer);
				if (statements.Count > 0)
				{
					ret = new SwitchSection(labels, statements);
				}
			}
			tokenizer.endBookmark(ret is MissingSwitchSection);
			return ret;
		}

		public SwitchSection() :
		base(new SwitchLabelCollection(), new StatementCollection())
		{
		}

		public SwitchSection(SwitchLabelCollection labels, StatementCollection statements) :
		base(labels, statements)
		{
		}

		public SwitchLabelCollection Labels {
			get {
				return Pieces[0] as SwitchLabelCollection;
			}
		}

		public StatementCollection Statements {
			get {
				return Pieces[1] as StatementCollection;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Labels);
				ret.Add(Statements);
				return ret;
			}
		}
	}

	public class MissingSwitchSection : SwitchSection
	{
	}
}
