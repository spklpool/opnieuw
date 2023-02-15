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

namespace Opnieuw.Parsers.CSParser
{
	/// <summary>
	/// Defines an interface to an object that can 
	/// supply a stream of tokens.
	/// </summary>
	public interface TokenProvider
	{
		/// <summary>
		/// Returns the current token.
		/// </summary>
		PositionToken CurrentToken {
			get;
		}

		/// <summary>
 		/// Verifies that the current token is of the specified 
		/// type.  If it is not, a ParserExpeption is thrown.
		/// </summary>
		PositionToken checkToken(int expected);

		/// <summary>
		/// Advances to the next token and returns it.
		/// </summary>
		PositionToken nextToken();

		/// <summary>
		/// Backs up by one token and returns it.
		/// </summary>
		PositionToken previousToken();

		/// <summary>
		/// Marks the current position so we can return to it later.
		/// </summary>
		void setBookmark();

		/// <summary>
		/// Returns to or cancels the last bookmark.
		/// </summary>
		/// <param name="isReturn"></param>
		void endBookmark(bool isReturn);

		/// <summary>
		/// Returns to the last bookmark.
		/// </summary>
		void returnToBookmark();

		/// <summary>
		/// Cancels the bookmark without returning to the marked location.
		/// </summary>
		void cancelBookmark();

		/// <summary>
		/// Scans all the tokens and returns them in a collection.
		/// </summary>
		PositionTokenCollection ScanAllTokens();
		
		/// <summary>
		/// Rerurns all the tokens that have been scanned by the provider.
		/// </summary>
		PositionTokenCollection Tokens {
			get ;
		}
	}
}
