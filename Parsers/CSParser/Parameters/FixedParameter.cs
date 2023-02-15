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
	public class FixedParameter : Parameter
    {
        #region static parsing code
        //attributesopt parameter-modifieropt type identifier
        public static FixedParameter parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			FixedParameter ret = new MissingFixedParameter();
			AttributeSectionCollection attributes = AttributeSectionCollection.parse(tokenizer);
			ParameterModifier modifier = ParameterModifier.parse(tokenizer);
			DataType type = DataType.parse(tokenizer);
			if (false == (type is MissingDataType))
			{
				Identifier identifier = Identifier.parse(tokenizer);
				if (false == (identifier is MissingIdentifier))
				{
					ret = new FixedParameter(attributes, modifier, type, identifier);
				}
			}
			tokenizer.endBookmark(ret is MissingFixedParameter);
			return ret;
        }
        #endregion

        #region constructors
        public FixedParameter() :
		base(new AttributeSectionCollection(), new MissingParameterModifier(), new MissingDataType(), new MissingIdentifier())
		{
		}

		public FixedParameter(AttributeSectionCollection attributes, ParameterModifier modifier, DataType type, Identifier identifier) :
		base(attributes, modifier, type, identifier)
		{
        }
        #endregion

        public override void Format()
        {
            base.Format();
            if (AttributeSections.Count == 0) {
                if (Modifier is MissingParameterModifier){
                    Type.LeadingCharacters = "";
                } else {
                    Modifier.LeadingCharacters = "";
                }
            }
        }

        public ParameterModifier Modifier {
			get {
				return Pieces[1] as ParameterModifier;
			}
		}

		public DataType Type {
			get {
				return Pieces[2] as DataType;
			}
		}
		
		public Identifier Identifier {
			get {
				return Pieces[3] as Identifier;
			}
		}

		public override string Name {
			get {
				return Identifier.Name;
			}
		}

		public override string AsText {
			get {
				return Identifier.Name;
			}
		}
	}

	public class MissingFixedParameter : FixedParameter
	{
	}
}
