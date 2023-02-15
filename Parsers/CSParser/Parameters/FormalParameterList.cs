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
	public class FormalParameterList : PieceOfCode
	{
		public static FormalParameterList parse(TokenProvider tokenizer)
		{
			FixedParameterCollection fixedParameters = FixedParameterCollection.parse(tokenizer);
			ParameterArray parameterArray = ParameterArray.parse(tokenizer);
			FormalParameterList ret = new FormalParameterList(fixedParameters, parameterArray);
			return ret;
        }

        public static FormalParameterList parse(string name)
        {
            Tokenizer t = new Tokenizer(new StringReader(name), "", null);
            t.nextToken();
            return FormalParameterList.parse(t) as FormalParameterList;
        }

        public FormalParameterList() :
		base(new FixedParameterCollection(), new MissingParameterArray())
		{
		}

		public FormalParameterList(FixedParameterCollection fixedParameters, ParameterArray parameterArray) :
		base(fixedParameters, parameterArray)
		{
		}

		public FixedParameterCollection FixedParameters {
			get {
				return Pieces[0] as FixedParameterCollection;
			}
		}

		public ParameterArray ParameterArray {
			get {
				return Pieces[1] as ParameterArray;
			}
		}

		public ParameterCollection Parameters {
			get {
				ParameterCollection ret = new ParameterCollection();
				foreach (FixedParameter param in FixedParameters)
				{
					ret.Add(param);
				}
				ret.Add(ParameterArray);
				return ret;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(FixedParameters.Children);
				if (false == (ParameterArray is MissingParameterArray))
				{
					ret.Add(ParameterArray);
				}
				return ret;
			}
		}
	}

	public class MissingFormalParameterList : FormalParameterList
	{
		public override void checkNotMissing()
		{
			throw new ParserException();
		}
	}
}
