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
	public class PositionToken : FundamentalPieceOfCode
	{
		protected Position pos;
		protected Object val;
		protected int type;

		public PositionToken(Position pos, Object val, int type)
		{
			this.pos = pos;
			this.val = val;
			this.type = type;
		}

        public void PropagateUp()
        {
            foreach (FundamentalPieceOfCode poc in Pieces) {
                poc.Parent = this;
                poc.PropagateUp();
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

		public Position Position {
			get {
				return pos;
			}
			set {
				pos = value;
			}
		}

		public Object Value {
			get {
				return val;
			}
		}

		public int Type {
			get {
				return type;
			}
		}

		protected string m_LeadingCharacters = "";
		public string LeadingCharacters {
			get {
				return m_LeadingCharacters;
			}
			set {
				m_LeadingCharacters = value;
			}
		}

        FundamentalPieceOfCodeCollection m_Pieces = new FundamentalPieceOfCodeCollection();
		public FundamentalPieceOfCodeCollection Pieces {
			get {
				return m_Pieces;
			}
		}
		
		protected PositionTokenCollection m_CommentTokens = new PositionTokenCollection();
		public PositionTokenCollection CommentTokens {
			get {
				return m_CommentTokens;
			}
			set {
				m_CommentTokens = value;
                m_Pieces.Clear();
                m_Pieces.Add(m_CommentTokens);
			}
		}

		public string Text {
			get {
				string ret = "";
				if (val is string) {
					if (type == Token.LITERAL_STRING) {
						ret = "\"" + (val as string) + "\"";
					} else if (type == Token.LITERAL_CHARACTER) {
						switch ((char)val)
						{
							case '\r':
								ret += "'\\r'";
								break;
							case '\n':
								ret += "'\\n'";
								break;
							case '\t':
								ret += "'\\t'";
								break;
							case '\f':
								ret += "'\\f'";
								break;
							case '\v':
								ret += "'\\v'";
								break;
							default:
								ret += "\'" + (char)val + "\'";
								break;
						}
					} else {
						ret = val as string;
					}
				} else if (val is Comment) {
					ret = (val as Comment).Text;
				} else if (val is int) {
					int intVal = (int)val;
					ret += intVal;
				} else if (val is long) {
					long longVal = (long)val;
					ret += longVal;
				} else if (val is float) {
					float floatVal = (float)val;
					ret += floatVal;
				} else if (val is decimal) {
					decimal decimalVal = (decimal)val;
					ret += decimalVal;
				} else if (val is double) {
					double doubleVal = (double)val;
					ret += doubleVal;
				} else if (val is char) {
					switch ((char)val)
					{
						case '\r':
							ret += "'\\r'";
							break;
						case '\n':
							ret += "'\\n'";
							break;
						case '\t':
							ret += "'\\t'";
							break;
						case '\f':
							ret += "'\\f'";
							break;
						case '\v':
							ret += "'\\v'";
							break;
						default:
							ret += "\'" + (char)val + "\'";
							break;
					}
				}
				return ret.Replace("\\", "\\\\");
			}
		}

        public bool IsMissing {
            get {
                return false;
            }
        }

        public FundamentalPieceOfCode Comments {
            get {
                return CommentTokens;
            }
        }

        public void Format()
        {
        }

        public virtual void CloneFormat(FundamentalPieceOfCode cloningSource)
        {
            LeadingCharacters = cloningSource.LeadingCharacters;
        }

		public virtual string Generate()
		{
			System.Text.StringBuilder ret = new System.Text.StringBuilder();
			ret.Append(CommentTokens.Generate());
			ret.Append(LeadingCharacters);
			ret.Append(Text);
			return ret.ToString();
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
		
		private static MissingPositionToken m_MissingPositionToken = new MissingPositionToken();
		public static MissingPositionToken Missing {
			get {
				return m_MissingPositionToken;
			}
		}
	}
	
	public class MissingPositionToken : PositionToken
	{
		public MissingPositionToken() :
		base(Position.Missing, null, Token.NONE)
		{
        }

        public override string Generate()
        {
            return "";
        }
    }
}
