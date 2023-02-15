#region Copyright (C) Pierre Boudreau
//This file is part of the Opnieuw project.
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
using System.Collections;

namespace Opnieuw.Parsers.CSParser
{
	public class KeywordCollection
	{
		private Hashtable keywords = new Hashtable();

		public KeywordCollection()
		{
			keywords.Add ("abstract", Token.ABSTRACT);
			keywords.Add ("as", Token.AS);
			keywords.Add ("add", Token.ADD);
			keywords.Add ("assembly", Token.ASSEMBLY);
			keywords.Add ("base", Token.BASE);
			keywords.Add ("bool", Token.BOOL);
			keywords.Add ("break", Token.BREAK);
			keywords.Add ("byte", Token.BYTE);
			keywords.Add ("case", Token.CASE);
			keywords.Add ("catch", Token.CATCH);
			keywords.Add ("char", Token.CHAR);
			keywords.Add ("checked", Token.CHECKED);
			keywords.Add ("class", Token.CLASS);
			keywords.Add ("const", Token.CONST);
			keywords.Add ("continue", Token.CONTINUE);
			keywords.Add ("decimal", Token.DECIMAL);
			keywords.Add ("default", Token.DEFAULT);
			keywords.Add ("delegate", Token.DELEGATE);
			keywords.Add ("do", Token.DO);
			keywords.Add ("double", Token.DOUBLE);
			keywords.Add ("else", Token.ELSE);
			keywords.Add ("enum", Token.ENUM);
			keywords.Add ("event", Token.EVENT);
			keywords.Add ("explicit", Token.EXPLICIT);
			keywords.Add ("extern", Token.EXTERN);
			keywords.Add ("false", Token.FALSE);
			keywords.Add ("finally", Token.FINALLY);
			keywords.Add ("fixed", Token.FIXED);
			keywords.Add ("float", Token.FLOAT);
			keywords.Add ("for", Token.FOR);
			keywords.Add ("foreach", Token.FOREACH);
			keywords.Add ("goto", Token.GOTO);
			keywords.Add ("get", Token.GET);
			keywords.Add ("if", Token.IF);
			keywords.Add ("implicit", Token.IMPLICIT);
			keywords.Add ("in", Token.IN);
			keywords.Add ("int", Token.INT);
			keywords.Add ("interface", Token.INTERFACE);
			keywords.Add ("internal", Token.INTERNAL);
			keywords.Add ("is", Token.IS);
			keywords.Add ("lock", Token.LOCK);
			keywords.Add ("long", Token.LONG);
			keywords.Add ("namespace", Token.NAMESPACE);
			keywords.Add ("new", Token.NEW);
			keywords.Add ("null", Token.NULL);
			keywords.Add ("object", Token.OBJECT);
			keywords.Add ("operator", Token.OPERATOR);
			keywords.Add ("out", Token.OUT);
			keywords.Add ("override", Token.OVERRIDE);
			keywords.Add ("params", Token.PARAMS);
			keywords.Add ("private", Token.PRIVATE);
			keywords.Add ("protected", Token.PROTECTED);
			keywords.Add ("public", Token.PUBLIC);
			keywords.Add ("readonly", Token.READONLY);
			keywords.Add ("ref", Token.REF);
			keywords.Add ("remove", Token.REMOVE);
			keywords.Add ("return", Token.RETURN);
			keywords.Add ("sbyte", Token.SBYTE);
			keywords.Add ("sealed", Token.SEALED);
			keywords.Add ("set", Token.SET);
			keywords.Add ("short", Token.SHORT);
			keywords.Add ("sizeof", Token.SIZEOF);
			keywords.Add ("stackalloc", Token.STACKALLOC);
			keywords.Add ("static", Token.STATIC);
			keywords.Add ("string", Token.STRING);
			keywords.Add ("struct", Token.STRUCT);
			keywords.Add ("switch", Token.SWITCH);
			keywords.Add ("this", Token.THIS);
			keywords.Add ("throw", Token.THROW);
			keywords.Add ("true", Token.TRUE);
			keywords.Add ("try", Token.TRY);
			keywords.Add ("typeof", Token.TYPEOF);
			keywords.Add ("uint", Token.UINT);
			keywords.Add ("ulong", Token.ULONG);
			keywords.Add ("unchecked", Token.UNCHECKED);
			keywords.Add ("unsafe", Token.UNSAFE);
			keywords.Add ("ushort", Token.USHORT);
			keywords.Add ("using", Token.USING);
			keywords.Add ("virtual", Token.VIRTUAL);
			keywords.Add ("void", Token.VOID);
			keywords.Add ("volatile", Token.VOLATILE);
			keywords.Add ("while", Token.WHILE);
//			keywords.Add ("field", Token.FIELD);
			keywords.Add ("method", Token.METHOD);
			keywords.Add ("module", Token.MODULE);
//			keywords.Add ("param", Token.PARAM);
			keywords.Add ("property", Token.PROPERTY);
//			keywords.Add ("type", Token.TYPE);
		}

		public bool is_keyword (string name)
		{
			bool res = keywords.Contains (name);
//			if (handle_get_set == false && (name == "get" || name == "set"))
//				return false;
//			if (handle_remove_add == false && (name == "remove" || name == "add"))
//				return false;
//			if (handle_assembly == false && (name == "assembly"))
//				return false;
			return res;
		}

		public bool Contains(string key)
		{
			return keywords.Contains(key);
		}

		public object this[string key] {
			get {
				return keywords[key];
			}
		}
	}
}
