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
using System.Collections;
using Opnieuw.Framework;

namespace Opnieuw.Parsers.CSParser
{
	public class SwitchSectionCollection : PieceOfCodeCollectionBase, IEnumerable 
	{
		public static SwitchSectionCollection parse(TokenProvider tokenizer)
		{
			SwitchSectionCollection ret = new SwitchSectionCollection();
			SwitchSection section = SwitchSection.parse(tokenizer);
			while (false == (section is MissingSwitchSection))
			{
				ret.Add(section);
				section = SwitchSection.parse(tokenizer);
			}
			return ret;
		}

		public SwitchSectionCollection()
		{
		}
		public SwitchSectionCollection(SwitchSection val)
		{
			Add(val);
		}

		public void Add (SwitchSection value)
		{
			data.Add (value);
			AdjustPosition();
		}

		public new SwitchSection this[int index] {
			get {
				return data [index] as SwitchSection;
			}
			set {
				data [index] = value;
			}
		}

		private class Enumerator : PieceOfCodeCollectionBaseEnumerator
		{
			public Enumerator(SwitchSectionCollection collection) :
			base(collection)
			{
			}

			public new SwitchSection Current {
				get {
					if (currentIndex == collection.Count)
						throw new InvalidOperationException ();
					return collection [currentIndex] as SwitchSection;
				}
			}
		}
		
		public new IEnumerator GetEnumerator ()
		{
			return new SwitchSectionCollection.Enumerator (this);
		}
	}
}
