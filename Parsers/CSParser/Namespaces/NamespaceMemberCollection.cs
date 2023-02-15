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
using System.Collections;
using Opnieuw.Framework;

namespace Opnieuw.Parsers.CSParser
{
	/// <summary>
	/// namespace-member-declaration
	/// namespace-member-declarations namespace-member-declaration 
	/// </summary>
	public class NamespaceMemberCollection : PieceOfCodeCollectionBase, IEnumerable 
	{
		public static NamespaceMemberCollection parse(TokenProvider tokenizer)
		{
			NamespaceMemberCollection ret = new NamespaceMemberCollection();
			NamespaceMember namespaceMember = Namespace.parseMember(tokenizer);
			while ((false == (namespaceMember is MissingNamespaceMember)) &&
				   (false == (namespaceMember is MissingTypeDeclaration)))
			{
				ret.Add(namespaceMember);
				namespaceMember = Namespace.parseMember(tokenizer);
			}
			return ret;
		}
		
		public static NamespaceMemberCollection parse(string input)
		{
			Tokenizer t = new Tokenizer(new StringReader(input), "", null);
            t.nextToken();
            return NamespaceMemberCollection.parse(t) as NamespaceMemberCollection;
		}

		public NamespaceMemberCollection()
		{
		}
		public NamespaceMemberCollection(NamespaceMember val)
		{
			Add(val);
		}

		public NamespaceCollection Namespaces {
			get {
				NamespaceCollection ret = new NamespaceCollection();
				foreach (NamespaceMember member in data)
				{
					if (member is Namespace)
					{
						ret.Add(member as Namespace);
					}
				}
				return ret;
			}
		}

 		public ClassCollection Classes {
			get {
				ClassCollection ret = new ClassCollection();
				foreach (NamespaceMember member in data)
				{
					if (member is Class)
					{
						ret.Add(member as Class);
					}
				}
				return ret;
			}
		}

 		public InterfaceCollection Interfaces {
			get {
				InterfaceCollection ret = new InterfaceCollection();
				foreach (NamespaceMember member in data)
				{
					if (member is Interface)
					{
						ret.Add(member as Interface);
					}
				}
				return ret;
			}
		}

		public void Add (NamespaceMember value)
		{
			data.Add (value as FundamentalPieceOfCode);
			AdjustPosition();
		}

		public void AddFullyQualifiedNamespaceMember(NamespaceMember member)
		{
			if (member is Namespace)
			{
				Namespace ns = member as Namespace;
				Namespace existing = GetFullyQualifiedNamespace(ns.FullyQualifiedName);
				if (existing is MissingNamespace)
				{
					Namespace existingParent = GetFullyQualifiedNamespace(member.FullyQualifiedParentNamespace);
					if (existingParent is MissingNamespace)
					{
						data.Add(member as FundamentalPieceOfCode);
					}
					else
					{
						//Our parent namespace exists, add ourselves to it.
						existingParent.Members.Add(member);
					}
				}
				else
				{
					if (existing.Contributors.Count == 0)
					{
						//We are the second contributor to this namespace.  It is
						//time to create a virtual one that contains both.
						ContainerNamespace container = new ContainerNamespace(ns, existing);
						data.Remove(existing);
						data.Add(container);
					}
					else
					{
						//We already have a container namespace for all contributors
						//so we will add ourselves to it.
						foreach (NamespaceMember child in ns.Members)
						{
							existing.Members.Add(child);
							existing.Contributors.Add(ns);
						}
					}
				}
			}
		}

		private Namespace GetFullyQualifiedNamespace(string name)
		{
			Namespace ret = new MissingNamespace();
			foreach (Namespace ns in this.Namespaces)
			{
				Namespace check = checkNamespaceForName(ns, name);
				if (false == (check is MissingNamespace))
				{
					ret = check;
					break;
				}
			}
			return ret;
		}

		private Namespace checkNamespaceForName(Namespace ns, string name)
		{
			Namespace ret = new MissingNamespace();
			if (ns.FullyQualifiedName == name)
			{
				ret = ns;
			}
			else
			{
				foreach (Namespace inner in ns.Namespaces)
				{
					Namespace check = checkNamespaceForName(inner, name);
					if (false == (check is MissingNamespace))
					{
						ret = check;
						break;
					}
				}
			}
			return ret;
		}

		public new NamespaceMember this[int index] {
			get {
				return data [index] as NamespaceMember;
			}
		}

		private class Enumerator : PieceOfCodeCollectionBaseEnumerator
		{
			public Enumerator(NamespaceMemberCollection collection) :
			base(collection)
			{
			}

			public new NamespaceMember Current {
				get {
					if (currentIndex == collection.Count)
						throw new InvalidOperationException ();
					return collection [currentIndex] as NamespaceMember;
				}
			}
		}
		
		public new IEnumerator GetEnumerator ()
		{
			return new NamespaceMemberCollection.Enumerator (this);
		}
	}
}
