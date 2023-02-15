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
using System.Collections;

using Opnieuw.Framework;

namespace Opnieuw.Parsers.CSParser
{
	public class PieceOfCode : FundamentalPieceOfCode
	{
		public PieceOfCode()
		{
            m_Pieces = new FundamentalPieceOfCodeCollection();
		}

		public PieceOfCode(params FundamentalPieceOfCode[] list) :
		this(new Position(list))
		{
            m_Pieces = new FundamentalPieceOfCodeCollection();
			for (int i=0; i<list.Length; i++)
			{
				Pieces.Add(list[i]);
			}
		}

		public PieceOfCode(Position position)
		{
            m_Pieces = new FundamentalPieceOfCodeCollection();
			m_Position = position;
		}

        public FundamentalPieceOfCode Comments {
            get {
                for (int i = 0; i < Pieces.Count; i++) {
                    if (!Pieces[i].IsMissing) {
                        return Pieces[i].Comments;
                    }
                }
                return new PositionTokenCollection();
            }
        }

        public TrailOfBreadCrumbs Crumbs {
            get {
                TrailOfBreadCrumbs ret = new TrailOfBreadCrumbs();
                FundamentalPieceOfCode next = this;
                while ((next != null) && (!(next is CompilationUnit)))
                {
                    ret.Add(next);
                    next = next.Parent;
                }
                return ret;
            }
        }

        private FundamentalPieceOfCode m_Up = null;
        public FundamentalPieceOfCode Parent {
            get {
                return m_Up;
            }
            set {
                m_Up = value;
            }
        }

		protected FundamentalPieceOfCodeCollection m_Pieces = null;
		public virtual FundamentalPieceOfCodeCollection Pieces {
			get {
				return m_Pieces;
			}
		}

        public virtual void PropagateUp()
        {
            foreach (FundamentalPieceOfCode poc in Pieces) {
                poc.Parent = this;
                poc.PropagateUp();
            }
        }

        public virtual void Format()
        {
            FormattingProperties props = FormattingPropertiesCollection.Instance[this.GetType()];
            for (int i = 0; i < Pieces.Count; i++) {
                if (props != null) {
                    if (props.TokenLeadingCharacters[i] != "") {
                        Pieces[i].LeadingCharacters = props.TokenLeadingCharacters[i];
                    }
                }
                Pieces[i].Format();
            }
        }

        public virtual void CloneFormat(FundamentalPieceOfCode cloningSource)
        {
            for (int i=0; i<Pieces.Count; i++) {
                Pieces[i].CloneFormat(cloningSource.Pieces[i]);
            }
        }

        public virtual void Reindent(int newIndent)
        {
        }

		public virtual string Generate()
		{
			System.Text.StringBuilder ret = new System.Text.StringBuilder();
			foreach (FundamentalPieceOfCode piece in Pieces)
			{
				ret.Append(piece.Generate());
			}
			return ret.ToString();
		}

		protected string ReplaceWithEscape(char source)
		{
			string ret = "" + source;
			switch (source)
			{
				case '\'':
					ret = "\\'";
					break;
				case '\"':
					ret = "\\\"";
					break;
				case '\\':
					ret = "\\\\";
					break;
				case '\0':
					ret = "\\0";
					break;
				case '\a':
					ret = "\\a";
					break;
				case '\b':
					ret = "\\b";
					break;
				case '\f':
					ret = "\\f";
					break;
				case '\n':
					ret = "\\n";
					break;
				case '\r':
					ret = "\\r";
					break;
				case '\t':
					ret = "\\t";
					break;
				case '\v':
					ret = "\\v";
					break;
			}
			return ret;
		}

		protected static void trace(string message)
		{
			Console.WriteLine(message);
		}

		protected string ReplaceWithEscapes(string source)
		{
			string ret = "";
			for(int i=0; i<source.Length; i++)
			{
				ret += ReplaceWithEscape(source[i]);
			}
			return ret;
		}

		public virtual string LeadingCharacters {
			get {
                for (int i = 0; i < Pieces.Count; i++) {
                    if (Pieces[i] is PieceOfCode) {
                        PieceOfCode poc = Pieces[i] as PieceOfCode;
                        if (!poc.IsMissing) {
                            return poc.LeadingCharacters;
                        }
                    } else {
                        return Pieces[0].LeadingCharacters;
                    }
                }
                return "";
            }
			set {
                for (int i = 0; i < Pieces.Count; i++) {
                    if (Pieces[i] is PieceOfCode) {
                        PieceOfCode poc = Pieces[i] as PieceOfCode;
                        if (!poc.IsMissing) {
                            poc.LeadingCharacters = value;
                            break;
                        }
                    } else {
                        Pieces[0].LeadingCharacters = value;
                        break;
                    }
                }
			}
		}

		/// <summary>
		/// Returns the parent compilation unit that contains this piece of code or
		/// MissingCompilationUnit if it can not be found.
		/// </summary>
		public CompilationUnit ParentCompilationUnit {
			get {
				CompilationUnit ret = new MissingCompilationUnit();
                FundamentalPieceOfCode next = this;
                while (next != null) {
                    if (next is CompilationUnit) {
                        ret = next as CompilationUnit;
                        break;
                    } else {
                        next = next.Parent;
                    }
                }
                return ret;
			}
		}

		public StatementContainer ParentStatementContainer {
			get {
                StatementContainer ret = null;
                FundamentalPieceOfCode next = this.Parent;
                while (next != null) {
                    if (next is StatementContainer) {
                        ret = next as StatementContainer;
                        break;
                    } else {
                        next = next.Parent;
                    }
                }
                return ret;
			}
		}

		/// <summary>
		/// Returns the parent namespace that contains this piece of code or
		/// MissingNamespace if it can not be found.
		/// </summary>
		public Namespace ParentNamespace {
			get {
                Namespace ret = new MissingNamespace();
                FundamentalPieceOfCode next = this.Parent;
                while (next != null) {
                    if (next is Namespace) {
                        ret = next as Namespace;
                        break;
                    } else {
                        next = next.Parent;
                    }
                }
                return ret;
			}
		}

		/// <summary>
		/// Returns the parent class member that contains this piece of code or
		/// MissingClassMember if it can not be found.
		/// </summary>
		public ClassMember ParentClassMember {
			get {
                ClassMember ret = new MissingClassMember();
                FundamentalPieceOfCode next = this;
                while (next != null) {
                    if (next is ClassMember) {
                        ret = next as ClassMember;
                        break;
                    } else {
                        next = next.Parent;
                    }
                }
                return ret;
			}
		}

		protected Position m_Position = Position.Missing;
		public virtual Position Position {
			get {
				return m_Position;
			}
		}

		public bool IsIntersectedBy(Position checkPosition)
		{
			bool ret = false;
			if (null != m_Position)
			{
				ret = m_Position.IsIntersectedBy(checkPosition);
			}
			return ret;
		}

		public bool Contains(int line, int col)
		{
			bool ret = false;
			if (null != m_Position)
			{
				ret = m_Position.Contains(line, col);
			}
			return ret;
		}

		public bool ContainsInclusive(int line, int col)
		{
			bool ret = false;
			if (null != m_Position)
			{
				ret = m_Position.ContainsInclusive(line, col);
			}
			return ret;
		}

		public bool Contains(Position checkPosition)
		{
			bool ret = false;
			if (null != m_Position)
			{
				ret = m_Position.Contains(checkPosition);
			}
			return ret;
		}

		public bool ContainsInclusive(Position checkPosition)
		{
			bool ret = false;
			if (null != m_Position)
			{
				ret = m_Position.ContainsInclusive(checkPosition);
			}
			return ret;
		}

		public bool IsContainedBy(Position checkPosition)
		{
			bool ret = false;
			if (null != m_Position)
			{
				ret = m_Position.IsContainedBy(checkPosition);
			}
			return ret;
		}

		public bool StartsBefore(Position checkPosition)
		{
			bool ret = false;
			if (null != m_Position)
			{
				ret = m_Position.StartsBefore(checkPosition);
			}
			return ret;
		}

		public bool EndsAfter(Position checkPosition)
		{
			bool ret = false;
			if (null != m_Position)
			{
				ret = m_Position.EndsAfter(checkPosition);
			}
			return ret;
		}

		public virtual string AsText {
			get {
				return this.GetType().Name;
			}
		}

		/// <summary>
		/// The children of a PieceOfCode are all the sub nodes that make 
		/// it up.  The returned collection contains all these children in
		/// the order they appear in the code.
		/// Note:  The order IS important.
		/// </summary>
		public virtual PieceOfCodeCollection Children {
			get {
				return new PieceOfCodeCollection();
			}
		}

		private PieceOfCodeCollection RecursiveChildren(PieceOfCode element)
		{
			PieceOfCodeCollection ret = new PieceOfCodeCollection();
			ret.Add(element);
			foreach (PieceOfCode child in element.Children)
			{
				ret.Add(RecursiveChildren(child));
			}
			return ret;
		}

		protected PieceOfCodeCollection m_DeepChildren = null;
		public virtual PieceOfCodeCollection DeepChildren {
			get {
				if (m_DeepChildren == null)
				{
					m_DeepChildren = new PieceOfCodeCollection();
					foreach (PieceOfCode child in Children)
					{
						m_DeepChildren.Add(RecursiveChildren(child));
					}
				}
				return m_DeepChildren;
			}
		}

		public Variable FindParentScopeVariableDeclaration(string name) 
		{
			Variable ret = new MissingVariable();
			FundamentalPieceOfCode node = this;
			do {
                if (node is PieceOfCode) {
				    if ((node as PieceOfCode).DeepDeclaredVariables.ContainsKey(name)) {
					    ret = (node as PieceOfCode).DeepDeclaredVariables[name];
					    break;
				    } else {
					    node = (node as PieceOfCode).Parent;
				    }
                } else {
                    node = (node as PieceOfCode).Parent;
                }
			} while(node != null);
			return ret;
		}

		private ExpressionCollection RecursiveChildExpressions(PieceOfCode element)
		{
			ExpressionCollection ret = new ExpressionCollection();
			if (element is Expression) {
				ret.Add(element as Expression);
			}
			foreach (PieceOfCode child in element.Children) {
				ret.Add(RecursiveChildExpressions(child));
			}
			return ret;
		}

		public virtual ExpressionCollection Expressions {
			get {
				ExpressionCollection ret = new ExpressionCollection();	
				foreach (PieceOfCode element in Children) {
					ret.Add(RecursiveChildExpressions(element));
				}
				return ret;
			}
		}

		private VariableCollection RecursiveDeclaredVariables(PieceOfCode element)
		{
			VariableCollection ret = new VariableCollection();
			ret.Add(element.DeclaredVariables);
			foreach (PieceOfCode child in element.Children) {
				ret.Add(RecursiveDeclaredVariables(child));
			}
			return ret;
		}

		protected VariableCollection m_DeepDeclaredVariables = null;
		public virtual VariableCollection DeepDeclaredVariables {
			get {
				if (m_DeepDeclaredVariables == null) {
					m_DeepDeclaredVariables = RecursiveDeclaredVariables(this);
				}
				return m_DeepDeclaredVariables;
			}
		}

		public virtual VariableCollection DeclaredVariables {
			get {
				return new VariableCollection();
			}
		}

		private VariableCollection RecursiveModifiedVariables(PieceOfCode element)
		{
			VariableCollection ret = new VariableCollection();
			ret.Add(element.ModifiedVariables);
			foreach (PieceOfCode child in element.Children) {
				ret.Add(RecursiveModifiedVariables(child));
			}
			return ret;
		}

		protected VariableCollection m_DeepModifiedVariables = null;
		public virtual VariableCollection DeepModifiedVariables {
			get {
				if (m_DeepModifiedVariables == null) {
					m_DeepModifiedVariables = RecursiveModifiedVariables(this);
				}
				return m_DeepModifiedVariables;
			}
		}

		public virtual VariableCollection ModifiedVariables {
			get {
				return new VariableCollection();
			}
		}

		private VariableCollection RecursiveReferencedVariables(PieceOfCode element)
		{
			VariableCollection ret = new VariableCollection();
			ret.Add(element.ReferencedVariables);
			foreach (PieceOfCode child in element.Children) {
				ret.Add(RecursiveReferencedVariables(child));
			}
			return ret;
		}

		protected VariableCollection m_DeepReferencedVariables = null;
		public virtual VariableCollection DeepReferencedVariables {
			get {
				if (m_DeepReferencedVariables == null) {
					m_DeepReferencedVariables = RecursiveReferencedVariables(this);
				}
				return m_DeepReferencedVariables;
			}
		}

		private VariableCollection RecursiveReferencedVariablesExcludingDeclaration(PieceOfCode element)
		{
			VariableCollection ret = new VariableCollection();
			foreach (Variable var in element.ReferencedVariables) {
				if (false == element.DeclaredVariables.Contains(var)) {
					ret.Add(element.ReferencedVariables);
				}
			}
			foreach (PieceOfCode child in element.Children) {
				ret.Add(RecursiveReferencedVariables(child));
			}
			return ret;
		}

		protected VariableCollection m_DeepReferencedVariablesExcludingDeclaration = null;
		public virtual VariableCollection DeepReferencedVariablesExcludingDeclaration {
			get {
				if (m_DeepReferencedVariablesExcludingDeclaration == null) {
					m_DeepReferencedVariablesExcludingDeclaration = RecursiveReferencedVariablesExcludingDeclaration(this);
				}
				return m_DeepReferencedVariablesExcludingDeclaration;
			}
		}

		protected VariableCollection m_ReferencedVariables = null;
		public virtual VariableCollection ReferencedVariables {
			get {
				if (m_ReferencedVariables == null) {
					m_ReferencedVariables = new VariableCollection();
					foreach (PieceOfCode child in Children) {
						if (child is Identifier) {
							Identifier identifier = child as Identifier;
							Variable variable = FindParentScopeVariableDeclaration(identifier.Name);
							if (false == variable is MissingVariable) {
								m_ReferencedVariables.Add(variable);
							}
						}
					}
				}
				return m_ReferencedVariables;
			}
		}

		public FundamentalPieceOfCodeCollection SelectNodesFromPosition(Position position)
		{
			FundamentalPieceOfCodeCollection ret = new FundamentalPieceOfCodeCollection();
			SelectNodesFromPosition(this, position, ret);
			return ret;
		}
		
		public FundamentalPieceOfCodeCollection SelectNodesFromPosition(FundamentalPieceOfCode poc, Position position, FundamentalPieceOfCodeCollection ret)
		{
			if (ret.Contains(poc) == false) {
				if ((position.ContainsInclusive(poc.Position))) {
					ret.Add(poc);
				} else {
					foreach (FundamentalPieceOfCode piece in poc.Pieces) {
						SelectNodesFromPosition(piece, position, ret);
					}
				}
			}
			return ret;
		}

        public int Indent {
            get {
                int ret = 0;
                if (Pieces.Count > 0) {
                    String indentString = LeadingCharacters;
			        for (int i=(indentString.Length-1); i>=0; i--) {
				        if ((indentString[i] == '\r') || (indentString[i] == '\n')) {
                            break;
				        } else if (indentString[i] == '\t') {
                            ret += 4;
                        } else {
					        ret ++;
				        }
			        }
                }
			    return ret;
            }
        }

        public virtual bool IsMissing {
            get {
                bool ret = false;
                try {
                    checkNotMissing();
                } catch (ParserException) {
                    ret = true;
                }
                return ret;
            }
        }

        public virtual void checkNotMissing()
		{
			//Nothing to do.  Override this in Missing classes.
		}
	}

	public class MissingPieceOfCode : PieceOfCode
	{
	}
}
