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
	public class AssignmentExpression : Expression
	{
		public AssignmentExpression(Expression expression1, PositionToken token, Expression expression2) :
		base(expression1, token, expression2)
		{
		}

        public AssignmentExpression(Expression exp1, Expression exp2) :
		base(exp1, 
             new PositionToken(Position.Missing, "=", Token.ASSIGN), 
             exp2)
        {
        }

		public Expression Expression1 {
			get {
				return Pieces[0] as Expression;
			}
		}

		public PositionToken AssignToken {
			get {
				return Pieces[1] as PositionToken;
			}
		}

		public Expression Expression2 {
			get {
				return Pieces[2] as Expression;
			}
		}

		public override PieceOfCodeCollection Children {
			get {
				PieceOfCodeCollection ret = new PieceOfCodeCollection();
				ret.Add(Expression1);
				ret.Add(Expression2);
				return ret;
			}
		}
	
		public override VariableCollection ModifiedVariables {
			get {
				VariableCollection ret = new VariableCollection();
				if (Expression1 is Identifier)
				{
					Identifier identifier = Expression1 as Identifier;
					Variable variable = FindParentScopeVariableDeclaration(identifier.Name);
					if (false == variable is MissingVariable)
					{
						ret.Add(variable);
					}
				}
				return ret;
			}
		}
	}

	public class AdditionAssignmentExpression : AssignmentExpression
	{
		public AdditionAssignmentExpression(Expression exp1, PositionToken token, Expression exp2) :
		base(exp1, token, exp2)
		{
		}

        public AdditionAssignmentExpression(Expression exp1, Expression exp2) :
		base(exp1, 
             new PositionToken(Position.Missing, "+=", Token.OP_ADD_ASSIGN), 
             exp2)
        {
        }
    }

	public class SubtractionAssignmentExpression : AssignmentExpression
	{
		public SubtractionAssignmentExpression(Expression exp1, PositionToken token, Expression exp2) :
		base(exp1, token, exp2)
		{
		}
	}

	public class MultiplicationAssignmentExpression : AssignmentExpression
	{
		public MultiplicationAssignmentExpression(Expression exp1, PositionToken token, Expression exp2) :
		base(exp1, token, exp2)
		{
		}
	}

	public class DivisionAssignmentExpression : AssignmentExpression
	{
		public DivisionAssignmentExpression(Expression exp1, PositionToken token, Expression exp2) :
		base(exp1, token, exp2)
		{
		}
	}

	public class ModuloAssignmentExpression : AssignmentExpression
	{
		public ModuloAssignmentExpression(Expression exp1, PositionToken token, Expression exp2) :
		base(exp1, token, exp2)
		{
		}
	}

	public class AndAssignmentExpression : AssignmentExpression
	{
		public AndAssignmentExpression(Expression exp1, PositionToken token, Expression exp2) :
		base(exp1, token, exp2)
		{
		}
	}

	public class OrAssignmentExpression : AssignmentExpression
	{
		public OrAssignmentExpression(Expression exp1, PositionToken token, Expression exp2) :
		base(exp1, token, exp2)
		{
		}
	}

	public class ExclusiveOrAssignmentExpression : AssignmentExpression
	{
		public ExclusiveOrAssignmentExpression(Expression exp1, PositionToken token, Expression exp2) :
		base(exp1, token, exp2)
		{
		}
	}

	public class ShiftLeftAssignmentExpression : AssignmentExpression
	{
		public ShiftLeftAssignmentExpression(Expression exp1, PositionToken token, Expression exp2) :
		base(exp1, token, exp2)
		{
		}
	}

	public class ShiftRightAssignmentExpression : AssignmentExpression
	{
		public ShiftRightAssignmentExpression(Expression exp1, PositionToken token, Expression exp2) :
		base(exp1, token, exp2)
		{
		}
	}
}
