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
	public class IndexerDeclaration : ModifyablePieceOfCodeWithAttributes, ClassMember, StructMember
    {
#region static parsing code

        /// <summary>
        /// attributesopt indexer-modifiersopt type this [ formal-parameter-list ] { accessor-declarations } 
		/// attributesopt indexer-modifiersopt type interface-type . this [ formal-parameter-list ]  { accessor-declarations } 
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
				DataType interfaceType = DataType.parse(tokenizer);
				PositionToken dotToken = PositionToken.Missing;
				if (false == (interfaceType is MissingDataType))
				{
					if (Token.DOT == tokenizer.CurrentToken.Type)
					{
						dotToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // .
					}
				}
				if (Token.THIS == tokenizer.CurrentToken.Type)
				{
					PositionToken thisToken = tokenizer.CurrentToken;
					tokenizer.nextToken(); // this
					if (Token.OPEN_BRACKET == tokenizer.CurrentToken.Type)
					{
						PositionToken openBracketToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // [
						FormalParameterList parameters = FormalParameterList.parse(tokenizer);
						if (Token.CLOSE_BRACKET == tokenizer.CurrentToken.Type)
						{
							PositionToken closeBracketToken = tokenizer.CurrentToken;
							tokenizer.nextToken(); // ]
							if (Token.OPEN_BRACE == tokenizer.CurrentToken.Type)
							{
								PositionToken openBraceToken = tokenizer.CurrentToken;
								tokenizer.nextToken(); // {
								AccessorDeclarations accessors = AccessorDeclarations.parse(tokenizer);
								if (Token.CLOSE_BRACE == tokenizer.CurrentToken.Type)
								{
									PositionToken closeBraceToken = tokenizer.CurrentToken;
									tokenizer.nextToken(); // }
									ret = new IndexerDeclaration(attributes, modifiers, type, 
																 interfaceType, dotToken, thisToken, 
																 openBracketToken, parameters, 
																 closeBracketToken, openBraceToken, 
																 accessors, closeBraceToken);
								}
							}
						}
					}
				}
			}
			tokenizer.endBookmark(ret is MissingClassMember);
			return ret;
        }
#endregion

        #region constructors
        public IndexerDeclaration() :
		base(new AttributeSectionCollection(), new ModifierCollection(), 
			 new MissingDataType(), new MissingDataType(), 
			 PositionToken.Missing, PositionToken.Missing, 
			 PositionToken.Missing, new MissingFormalParameterList(), 
			 PositionToken.Missing, PositionToken.Missing, 
			 AccessorDeclarations.Missing, PositionToken.Missing)
		{
        }
        public IndexerDeclaration(DataType type, 
                                  FormalParameterList parameters, 
                                  AccessorDeclarations accessors) :
		base(new AttributeSectionCollection(), new ModifierCollection(), 
			 type, 
             new MissingDataType(), 
			 PositionToken.Missing, 
             new PositionToken(Position.Missing, "this", Token.THIS), 
			 new PositionToken(Position.Missing, "[", Token.OPEN_BRACKET), 
             parameters, 
			 new PositionToken(Position.Missing, "]", Token.CLOSE_BRACKET), 
             new PositionToken(Position.Missing, "{", Token.OPEN_BRACE), 
			 accessors, 
             new PositionToken(Position.Missing, "}", Token.CLOSE_BRACE))
        {
        }

        public IndexerDeclaration(AttributeSectionCollection attributes, 
								  ModifierCollection modifiers, 
								  DataType type, DataType interfaceType, 
								  PositionToken dotToken, PositionToken thisToken,
								  PositionToken openBracketToken, FormalParameterList parameters, 
								  PositionToken closeBracketToken, PositionToken openBraceToken,
								  AccessorDeclarations accessors, PositionToken closeBraceToken) :
		base(attributes, modifiers, type, interfaceType, dotToken, thisToken, openBracketToken, 
			 parameters, closeBracketToken, openBraceToken, accessors, closeBraceToken)
		{
        }
        #endregion

        public override void Format()
        {
            base.Format();
            if (InterfaceType is MissingDataType) {
                ThisToken.LeadingCharacters = " ";
            }
        }

        public DataType Type {
			get {
				return Pieces[2] as DataType;
			}
		}

		public DataType InterfaceType {
			get {
				return Pieces[3] as DataType;
			}
		}

		public PositionToken DotToken {
			get {
				return Pieces[4] as PositionToken;
			}
		}

		public PositionToken ThisToken {
			get {
				return Pieces[5] as PositionToken;
			}
		}

		public PositionToken OpenBracketToken {
			get {
				return Pieces[6] as PositionToken;
			}
		}

		public FormalParameterList Parameters {
			get {
				return Pieces[7] as FormalParameterList;
			}
		}

		public PositionToken CloseBracketToken {
			get {
				return Pieces[8] as PositionToken;
			}
		}

		public PositionToken OpenBraceToken {
			get {
				return Pieces[9] as PositionToken;
			}
		}

		public AccessorDeclarations Accessors {
			get {
				return Pieces[10] as AccessorDeclarations;
			}
		}

		public PositionToken CloseBraceToken {
			get {
				return Pieces[11] as PositionToken;
			}
		}

		public String Name {
			get {
				return "";
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Accessors);
				return ret;
			}
		}
	}

	public class MissingIndexerDeclaration : IndexerDeclaration
	{
	}
}
