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
	public class DeclarationStatement : Statement
    {
#region static parsing code
        /// <summary>
        /// local-variable-declaration ;
		/// local-constant-declaration ; 
		/// </summary>
		public new static Statement parse(TokenProvider tokenizer)
		{
			tokenizer.setBookmark();
			Statement ret = new MissingStatement();
			if (Token.CONST ==  tokenizer.CurrentToken.Type)
			{
				LocalConstantDeclaration lcd = LocalConstantDeclaration.parse(tokenizer);
				if (false == (lcd is MissingLocalConstantDeclaration))
				{
					if (Token.SEMICOLON == tokenizer.CurrentToken.Type)
					{
						PositionToken semicolonToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // ;
						ret = new DeclarationStatement(lcd, semicolonToken);
					}
				}
			}
			else
			{
				LocalVariableDeclaration lvd = LocalVariableDeclaration.parse(tokenizer);
				if (false == (lvd is MissingLocalVariableDeclaration))
				{
					if (Token.SEMICOLON == tokenizer.CurrentToken.Type)
					{
						PositionToken semicolonToken = tokenizer.CurrentToken;
						tokenizer.nextToken(); // ;
						ret = new DeclarationStatement(lvd, semicolonToken);
					}
				}
			}
			tokenizer.endBookmark(ret is MissingStatement);
			return ret;
        }
#endregion

#region constructors
        public DeclarationStatement(LocalVariableDeclaration lvd, PositionToken semicolonToken) :
		base(lvd, new MissingLocalConstantDeclaration(), semicolonToken)
		{
		}
        
        public DeclarationStatement(LocalVariableDeclaration lvd) :
		base(lvd, new MissingLocalConstantDeclaration(), 
             new PositionToken(Position.Missing, ";", Token.SEMICOLON))
        {
        }

        public DeclarationStatement(LocalConstantDeclaration lcd, PositionToken semicolonToken) :
		base(new MissingLocalVariableDeclaration(), lcd, semicolonToken)
		{
        }

        public DeclarationStatement(LocalConstantDeclaration lcd) :
		base(new MissingLocalVariableDeclaration(), lcd, 
             new PositionToken(Position.Missing, ";", Token.SEMICOLON))
        {
        }
#endregion

        public LocalVariableDeclaration LocalVariableDeclaration {
			get {
				return Pieces[0] as LocalVariableDeclaration;
			}
		}

		public LocalConstantDeclaration LocalConstantDeclaration {
			get {
				return Pieces[1] as LocalConstantDeclaration;
			}
		}
		
		public PositionToken SemicolonToken {
			get {
				return Pieces[2] as PositionToken;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				if (false == (LocalVariableDeclaration is MissingLocalVariableDeclaration))
				{
					ret.Add(LocalVariableDeclaration);
				}
				if (false == (LocalConstantDeclaration is MissingLocalConstantDeclaration))
				{
					ret.Add(LocalConstantDeclaration);
				}
				return ret;
			}
		}

		public override VariableCollection DeclaredVariables {
			get {
				VariableCollection ret = new VariableCollection();
				foreach (VariableDeclarator vd in LocalVariableDeclaration.VariableDeclarators)
				{
					ret.Add(new Variable(vd.Name, LocalVariableDeclaration.Type, vd.Initializer, this.Parent as PieceOfCode));
				}
				foreach (VariableDeclarator vd in LocalConstantDeclaration.VariableDeclarators)
				{
					ret.Add(new Variable(vd.Name, LocalVariableDeclaration.Type, vd.Initializer, this.Parent as PieceOfCode));
				}
				return ret;
			}
		}

		public override VariableCollection ModifiedVariables {
			get {
				VariableCollection ret = new VariableCollection();
				foreach (VariableDeclarator vd in LocalVariableDeclaration.VariableDeclarators)
				{
					if (false == (vd.Initializer is MissingVariableInitializer))
					{
						ret.Add(new Variable(vd.Name, LocalVariableDeclaration.Type, vd.Initializer, this.Parent as PieceOfCode));
					}
				}
				foreach (VariableDeclarator vd in LocalConstantDeclaration.VariableDeclarators)
				{
					if (false == (vd.Initializer is MissingVariableInitializer))
					{
						ret.Add(new Variable(vd.Name, LocalVariableDeclaration.Type, vd.Initializer, this.Parent as PieceOfCode));
					}
				}
				return ret;
			}
		}
	}
}
