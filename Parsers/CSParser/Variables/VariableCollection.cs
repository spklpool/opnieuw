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

namespace Opnieuw.Parsers.CSParser 
{
	using System;
	using System.Collections;

	public class VariableCollection : PieceOfCode, IEnumerable {

		Hashtable data;
		
		//
		// Constructors
		//
		public VariableCollection ()
		{
			data = new Hashtable ();
		}
		public VariableCollection (Variable v)
		{
			data = new Hashtable ();
			Add(v);
		}
		public VariableCollection (VariableCollection col)
		{
			foreach (Variable v in col)
			{
				Add(v);
			}
		}

		public int Count {
			get {
				return data.Count;
			}
		}

		public void Add (Variable value)
		{
			if (!data.ContainsKey(value.Name))
			{
				data.Add (value.Name, value);
			}
		}

		public void Add(object obj1, object obj2)
		{
			string typeString = "";
			if (obj2 is DataType)
			{
				typeString = ((DataType)obj2).Name;
				if (obj1 is VariableDeclarator)
				{
					Add((VariableDeclarator)obj1, typeString);
				}
				else if	(obj1 is VariableDeclaratorCollection)
				{
					Add((VariableDeclaratorCollection)obj1, typeString);
				}
			}
			if (obj1 is DataType)
			{
				typeString = ((DataType)obj1).Name;
				if (obj2 is VariableDeclarator)
				{
					Add((VariableDeclarator)obj2, typeString);
				}
				else if	(obj2 is VariableDeclaratorCollection)
				{
					Add((VariableDeclaratorCollection)obj2, typeString);
				}
			}
		}

		public void Add(object obj, string typeString)
		{
			if (obj is VariableDeclarator)
			{
				Add((VariableDeclarator)obj, typeString);
			}
			else if	(obj is VariableDeclaratorCollection)
			{
				Add((VariableDeclaratorCollection)obj, typeString);
			}
		}

		public void Add(VariableDeclarator vd, string typeString)
		{
			if (!data.ContainsKey(vd.Name))
			{
				Variable v = new Variable(vd.Name, typeString, vd.Initializer, vd);
				Add(v);
			}
		}

		public void Add(VariableDeclaratorCollection declaratorCol, string typeString)
		{
			foreach (VariableDeclarator vd in declaratorCol)
			{
				Add(vd, typeString);
			}
		}

		public void Add(VariableCollection col)
		{
			foreach (Variable v in col)
			{
				Add(v);
			}
		}

		public void Remove (Variable value)
		{
			data.Remove(value.Name);
		}

		public bool ContainsKey (string key)
		{
			return data.ContainsKey(key);
		}

		public bool Contains (Variable value)
		{
			return data.ContainsKey(value.Name);
		}

		public bool ContainsAnyOf (VariableCollection col)
		{
			bool ret = false;
			foreach (Variable var in col)
			{
				if (Contains(var))
				{
					ret = true;
					break;
				}
			}
			return ret;
		}

		public Variable this[string key] {
			get {
				return data[key] as Variable;
			}
		}

		private class Enumerator : IEnumerator {
			private System.Collections.IEnumerator innerEnumerator = null;

			internal Enumerator (Hashtable innerData)
			{
				innerEnumerator = innerData.GetEnumerator();
			}

			public object Current {
				get {
					return ((DictionaryEntry)innerEnumerator.Current).Value;
				}
			}

			public bool MoveNext ()
			{
				return innerEnumerator.MoveNext();
			}

			public void Reset ()
			{
				innerEnumerator.Reset();
			}
		}

		public IEnumerator GetEnumerator ()
		{
			return new VariableCollection.Enumerator (data);
		}
	}
}
