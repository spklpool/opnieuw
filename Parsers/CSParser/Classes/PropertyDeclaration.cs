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
	public class PropertyDeclaration : ModifyablePieceOfCodeWithAttributes, ClassMember, StructMember
    {
#region static parsing code
        /// <summary>
        /// attributesopt property-modifiersopt type member-name { accessor-declarations } 
		/// </summary>
		public static ClassMember parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			ClassMember ret = new MissingClassMember();
			AttributeSectionCollection attributes = AttributeSectionCollection.parse(tokenizer);
			ModifierCollection modifiers = ModifierCollection.parse(tokenizer);
			DataType type = DataType.parse(tokenizer);
			if (false == (type is MissingDataType))
			{
				Expression exp = QualifiedIdentifier.parse(tokenizer);
				if (exp is QualifiedIdentifier)
				{
					QualifiedIdentifier identifier = exp as QualifiedIdentifier;
					if (Token.OPEN_BRACE == tokenizer.CurrentToken.Type)
					{
						PositionToken openBraceToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // {
						AccessorDeclarations accessors = AccessorDeclarations.parse(tokenizer);
						if (false == (accessors is MissingAccessorDeclarations))
						{
							PositionToken closeBraceToken = tokenizer.checkToken(Token.CLOSE_BRACE);
							ret = new PropertyDeclaration(attributes, modifiers, type, identifier, openBraceToken, accessors, closeBraceToken);
						}
					}
				}
			}
			tokenizer.endBookmark(ret is MissingClassMember);
			return ret;
		}
#endregion

#region constructors
        public PropertyDeclaration(AttributeSectionCollection attributes, ModifierCollection modifiers, 
								   DataType type, QualifiedIdentifier identifier, PositionToken openBraceToken,
								   AccessorDeclarations accessors, PositionToken closeBraceToken) :
		base(attributes, modifiers, type, identifier, openBraceToken, accessors, closeBraceToken)
		{
        }

        public PropertyDeclaration(DataType type, String name,
                                   AccessorDeclarations accessors) :
		base(new AttributeSectionCollection(), new ModifierCollection(), 
             type, 
             QualifiedIdentifier.parse(name), 
             new PositionToken(Position.Missing, "{", Token.OPEN_BRACE),
             accessors,
             new PositionToken(Position.Missing, "}", Token.CLOSE_BRACE))
        {
        }
#endregion

        public override void Format()
        {
            base.Format();
            string accessorsBlock = AccessorDeclarations.Generate();
            string newAccessorsBlock = GenericBlockOfCode.Reindent(accessorsBlock, 4, 4);
            AccessorDeclarations newAccessors = AccessorDeclarations.parse(newAccessorsBlock);
            Pieces[5] = newAccessors;
        }


        public DataType Type {
			get {
				return Pieces[2] as DataType;
			}
		}

		public QualifiedIdentifier Identifier {
			get {
				return Pieces[3] as QualifiedIdentifier;
			}
		}
		public string Name {
			get {
				return Identifier.Name;
			}
		}

		public PositionToken OpenBraceToken {
			get {
				return Pieces[4] as PositionToken;
			}
		}

		public AccessorDeclarations AccessorDeclarations {
			get {
				return Pieces[5] as AccessorDeclarations;
			}
		}

		public PositionToken CloseBraceToken {
			get {
				return Pieces[6] as PositionToken;
			}
		}

		public override string AsText {
			get {
				return Identifier.Name;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(AccessorDeclarations.Children);
				return ret;
			}
		}
	}
}
