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
using System.Collections;

using Opnieuw.Framework;

namespace Opnieuw.Parsers.CSParser  
{
	public class ClassMemberCollection : PieceOfCodeCollectionBase, IEnumerable 
	{
		public static ClassMemberCollection parse(TokenProvider tokenizer)
		{
			ClassMemberCollection ret = new ClassMemberCollection();
			ClassMember member = CompilationUnit.parseClassMember(tokenizer);
			while (false == (member is MissingClassMember))
			{
				ret.Add(member);
				member = CompilationUnit.parseClassMember(tokenizer);
			}
			return ret;
		}
		
		public static ClassMemberCollection parse(string input)
		{
			Tokenizer t = new Tokenizer(new StringReader(input), "", null);
            t.nextToken();
            return ClassMemberCollection.parse(t) as ClassMemberCollection;
		}

		public ClassMemberCollection ()
		{
		}

		public void Add (ClassMember value)
		{
			data.Add (value as FundamentalPieceOfCode);
		}

        public void InsertBefore(String memberName, ClassMember val)
        {
            for (int i = 0; i < Count; i++ ) {
                if (this[i].Name.Equals(memberName)) {
                    data.Insert(i, val as FundamentalPieceOfCode);
                    return;
                }
            }
        }

        public void InsertAfter(String memberName, ClassMember val)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Name.Equals(memberName))
                {
                    if (i == (Count-1)) {
                        data.Add(val as FundamentalPieceOfCode);
                    } else {
                        data.Insert(i+1, val as FundamentalPieceOfCode);
                    }
                    return;
                }
            }
        }

        public ClassMember this[String name]
        {
            get
            {
                ClassMember ret = null;
                for (int i = 0; i < Count; i++) {
                    if (this[i].Name.Equals(name)) {
                        ret = this[i];
                    }
                }
                return ret;
            }
        }

        public new ClassMember this[int index] {
			get {
				return data [index] as ClassMember;
			}
		}

		private class Enumerator : PieceOfCodeCollectionBaseEnumerator
		{
			public Enumerator(ClassMemberCollection collection) :
			base(collection)
			{
			}

			public new ClassMember Current {
				get {
					if (currentIndex == collection.Count)
						throw new InvalidOperationException ();
					return collection [currentIndex] as ClassMember;
				}
			}
		}
		
		public new IEnumerator GetEnumerator ()
		{
			return new ClassMemberCollection.Enumerator (this);
		}
	}
}
