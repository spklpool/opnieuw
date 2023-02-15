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
	public class Attribute : PieceOfCode
	{
		/// <summary>
		/// attribute-name attribute-argumentsopt 
		/// </summary>
		public static Attribute parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			Attribute ret = new MissingAttribute();
			Expression name = QualifiedIdentifier.parse(tokenizer);
			if (name is QualifiedIdentifier)
			{
				AttributeArguments arguments = AttributeArguments.parse(tokenizer);
				ret = new Attribute(name as QualifiedIdentifier, arguments);
			}
			tokenizer.endBookmark(ret is MissingAttribute);
			return ret;
		}

		public Attribute()
		{
			Pieces.Add(new MissingQualifiedIdentifier());
			Pieces.Add(new MissingAttributeArguments());
		}

		public Attribute(QualifiedIdentifier identifier, AttributeArguments arguments) :
		base(identifier, arguments)
		{
		}

		public Attribute(QualifiedIdentifier identifier) :
		base(identifier, new MissingAttributeArguments())
		{
		}

		public QualifiedIdentifier Identifier {
			get {
				return Pieces[0] as QualifiedIdentifier;
			}
		}
		public string Name {
			get {
				return Identifier.Name;
			}
		}

		public AttributeArguments Arguments {
			get {
				return Pieces[1] as AttributeArguments;
			}
		}
	}

	class MissingAttribute : Attribute
	{
	}
}
