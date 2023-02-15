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
	public class QualifiedIdentifier : PrimaryExpression
	{
		public new static Expression parse(TokenProvider tokenizer)
		{
			Expression ret = new MissingExpression();
			IdentifierCollection col = new IdentifierCollection();
			PositionToken pt = tokenizer.CurrentToken;
			Position startPosition = pt.Position;
			Position lastPosition = pt.Position;
			while (Token.IDENTIFIER == pt.Type)
			{
				lastPosition = new Position(pt);
				col.Add(new Identifier(pt));
				pt = tokenizer.nextToken();
				if (Token.DOT == pt.Type)
				{
					pt = tokenizer.nextToken();
				}
				else
				{
					break;
				}
			}
			Position position = new Position(startPosition, lastPosition);
			if (col.Count > 0)
			{
				ret = new QualifiedIdentifier(col);
			}
			return ret;
		}
		
		public new static QualifiedIdentifier parse(string name)
		{
			Tokenizer t = new Tokenizer(new StringReader(name), "", null);
            t.nextToken();
            return QualifiedIdentifier.parse(t) as QualifiedIdentifier;
		}

		public QualifiedIdentifier(IdentifierCollection identifiers) :
        base(identifiers)
		{
		}

		public IdentifierCollection Identifiers {
			get {
				return Pieces[0] as IdentifierCollection;
			}
		}

		public string Name {
			get {
				string ret = "";
				foreach (Identifier identifier in Identifiers)
				{
					ret += identifier.Name;
					ret += ".";
				}
				if (ret.Length > 0)
				{
					ret = ret.Substring(0, ret.Length-1);
				}
				return ret;
			}
		}

		public override string Generate()
		{
			System.Text.StringBuilder ret = new System.Text.StringBuilder();
			for(int i=0; i<Identifiers.Count; i++)
			{
				ret.Append(Identifiers[i].Generate());
				if (i<(Identifiers.Count-1))
				{
					//TODO:  What if there are spaces and stuff arount 
					//this dot?
					ret.Append(".");
				}
			}
			return ret.ToString();
		}
	}

	public class MissingQualifiedIdentifier : QualifiedIdentifier
	{
		public MissingQualifiedIdentifier() :
		base(new IdentifierCollection())
		{
		}
	}
}
