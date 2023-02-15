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
using Opnieuw.Framework;

namespace Opnieuw.Parsers.CSParser
{
	public class AccessorDeclarations : PieceOfCode
	{
		public static AccessorDeclarations parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			AccessorDeclarations ret = AccessorDeclarations.Missing;
			AccessorDeclaration firstAccessor = GetAccessorDeclaration.parse(tokenizer);
			if (firstAccessor is MissingAccessorDeclaration)
			{
				firstAccessor = SetAccessorDeclaration.parse(tokenizer);
			}
			AccessorDeclaration secondAccessor = GetAccessorDeclaration.parse(tokenizer);
			if (secondAccessor is MissingAccessorDeclaration)
			{
				secondAccessor = SetAccessorDeclaration.parse(tokenizer);
			}
			if ((false == (firstAccessor is MissingAccessorDeclaration)) ||
				(false == (secondAccessor is MissingAccessorDeclaration)))
			{
				ret = new AccessorDeclarations(firstAccessor, secondAccessor);
			}
			tokenizer.endBookmark(ret is MissingAccessorDeclarations);
			return ret;
        }

        public static AccessorDeclarations parse(string name)
        {
            Tokenizer t = new Tokenizer(new StringReader(name), "", null);
            t.nextToken();
            return AccessorDeclarations.parse(t) as AccessorDeclarations;
        }

        public AccessorDeclarations(AccessorDeclaration firstAccessor, AccessorDeclaration secondAccessor) :
		base(firstAccessor, secondAccessor)
		{
		}

		public AccessorDeclaration FirstAccessor {
			get {
				return Pieces[0] as AccessorDeclaration;
			}
		}
		
		public AccessorDeclaration SecondAccessor {
			get {
				return Pieces[1] as AccessorDeclaration;
			}
		}
		
		public AccessorDeclaration GetAccessor {
			get {
				if (FirstAccessor is GetAccessorDeclaration) {
					return FirstAccessor;
				} else if (SecondAccessor is GetAccessorDeclaration) {
					return SecondAccessor;
				} else {
					return AccessorDeclaration.Missing;
				}
			}
		}

		public AccessorDeclaration SetAccessor {
			get {
				if (FirstAccessor is SetAccessorDeclaration) {
					return FirstAccessor;
				} else if (SecondAccessor is SetAccessorDeclaration) {
					return SecondAccessor;
				} else {
					return AccessorDeclaration.Missing;
				}
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				if (false == (FirstAccessor is MissingAccessorDeclaration))
				{
					ret.Add(FirstAccessor);
				}
				if (false == (SecondAccessor is MissingAccessorDeclaration))
				{
					ret.Add(SecondAccessor);
				}
				return ret;
			}
		}
		
		protected static AccessorDeclarations m_Missing = new MissingAccessorDeclarations();
		public static AccessorDeclarations Missing {
			get {
				return m_Missing;
			}
		}
	}

	public class MissingAccessorDeclarations : AccessorDeclarations
	{
		public MissingAccessorDeclarations() :
		base(AccessorDeclaration.Missing, AccessorDeclaration.Missing)
		{
		}
	}
}
