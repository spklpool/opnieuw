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

using Opnieuw.Framework;

namespace Opnieuw.Parsers.CSParser 
{
	using System;
	using System.Collections;

    public class ModifierCollection : PieceOfCodeCollectionBase, IEnumerable
    {
		public static ModifierCollection parse(TokenProvider tokenizer)
		{
			ModifierCollection ret = new ModifierCollection();
			while ((Token.NEW == tokenizer.CurrentToken.Type) ||
				   (Token.PUBLIC == tokenizer.CurrentToken.Type) ||
				   (Token.PROTECTED == tokenizer.CurrentToken.Type) ||
				   (Token.INTERNAL == tokenizer.CurrentToken.Type) ||
				   (Token.PRIVATE == tokenizer.CurrentToken.Type) ||
				   (Token.ABSTRACT == tokenizer.CurrentToken.Type) ||
				   (Token.EXTERN == tokenizer.CurrentToken.Type) ||
				   (Token.SEALED == tokenizer.CurrentToken.Type) ||
				   (Token.STATIC == tokenizer.CurrentToken.Type) ||
				   (Token.OVERRIDE == tokenizer.CurrentToken.Type) || 
				   (Token.VIRTUAL == tokenizer.CurrentToken.Type))
			{
				ret.Add(new Modifier(tokenizer.CurrentToken));
				tokenizer.nextToken();
			}
			return ret;
		}

		public ModifierCollection ()
		{
		}
		public ModifierCollection (Modifier mod)
		{
			Add(mod);
		}

		public void Add (Modifier value)
		{
			data.Add (value);
			AdjustPosition();
		}

		public new Modifier this[int index] {
			get {
				return data [index] as Modifier;
			}
			set {
				data [index] = value;
			}
        }

        private class Enumerator : PieceOfCodeCollectionBaseEnumerator
        {
            public Enumerator(ModifierCollection collection) :
			base(collection)
            {
            }

            public new Modifier Current
            {
                get
                {
                    if (currentIndex == collection.Count)
                        throw new InvalidOperationException();
                    return collection[currentIndex] as Modifier;
                }
            }
        }

        public new IEnumerator GetEnumerator()
        {
            return new ModifierCollection.Enumerator(this);
        }
    }
}
