#region Copyright (C) Pierre Boudreau
//This file is part of the Opnieuw project.
//
//pierre.boudreau@alphacentauri.biz
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
using System.CodeDom;
using System.Collections;
using System.Windows.Forms;
using Opnieuw.Framework;
using Opnieuw.Parsers.CSParser;

namespace Opnieuw.ExtractMethod
{
	public class ExtractMethodRefactoring : CSRefactoring
	{
		public ExtractMethodRefactoring()
		{
		}

		public string NewMethodName {
			get {
                String ret = "";
				if (PiecesOfCode.Count > 0) {
                    if ((PiecesOfCode[0].Comments as PositionTokenCollection).Count > 0) {
                        ret = ((PiecesOfCode[0].Comments as PositionTokenCollection)[0].Value as Comment).ContainedText;
                        if (Identifier.parse(ret) is MissingIdentifier) {
                            ret = "";
                        }
                    }
                }
                return ret;
			}
		}

		/// <summary>
		/// Determines if the refactoring is available with the currently selected nodes.
		/// </summary>
		public override bool IsAvailable {
			get {
				bool ret = true;
				ret = (ExtractedStatements.Count > 0) &&
                      AllNodesAreStatementsOrSingleExpression &&
					  AllNodesAreUnderAppropriateCommonParent && 
					  NodesDoNotContainNeededVariableDeclaration;
				return ret;
			}
		}

		public override CodeCompileUnitCollection ChangingCodeCompileUnits {
			get {
				CodeCompileUnitCollection ret = new CodeCompileUnitCollection();
				if (IsAvailable) {
					ret.Add(CreateExistingClassCodeCompileUnit());
				}
				return ret;
			}
		}

		private CodeCompileUnit CreateExistingClassCodeCompileUnit()
		{
            String tempMethodName = NewMethodName;
            PositionCollection highlightPositions = new PositionCollection();
            ClassMember rootClassMember = PiecesOfCode.RootClassMember as ClassMember;
            CompilationUnit workingClone = CompilationUnit.parse(PiecesOfCode.RootCompilationUnit.Generate());
            Class workingCloneClass = workingClone.FromTrailOfBreadCrumbs(PiecesOfCode.RootClass.Crumbs) as Class;
			
            //NewMethod
            StatementCollection bodyStatements = GetUnindentedBodyStatements();
            if (tempMethodName.Equals("")) {
                tempMethodName = "SpecifyTheNameInACommentOnTopOfTheExtractedStatements";
            } else {
				RemoveCommentFromFirstStatement(bodyStatements);
            }
            if (!(ReturnVariable is MissingVariable)) {
                ReturnStatement returnStatement = new ReturnStatement(Identifier.parse(ReturnVariable.Name));
                returnStatement.PropagateUp();
                returnStatement.Format();
                bodyStatements.Add(returnStatement);
            }
            Statement body = new BlockStatement(bodyStatements);
            MethodDeclaration newMethod = new MethodDeclaration(ReturnType, tempMethodName, ParameterList, body);
            newMethod.Reindent((rootClassMember as PieceOfCode).Indent);
			InsertNewMethod(newMethod, workingCloneClass);
            
            //RemoveExtractedStatements
            PieceOfCodeCollection statementsToRemove = new PieceOfCodeCollection();
            for (int i=0; i<ExtractedStatements.Count; i++) {
                Statement statement = ExtractedStatements[i];
                Statement statementToRemove = workingClone.FromTrailOfBreadCrumbs(statement.Crumbs) as Statement;
                if (statementToRemove != null) {
                    statementsToRemove.Add(statementToRemove);
                }
            }
            StatementContainer container = statementsToRemove[0].ParentStatementContainer;
            Expression newInvocationExpression = new InvocationExpression(Identifier.parse(tempMethodName), new PositionToken(Position.Missing, "(", Token.OPEN_PARENS), Arguments, new PositionToken(Position.Missing, ")", Token.CLOSE_PARENS));
            Expression newExpression = newInvocationExpression;
            Statement newStatement = null;
            if (false == (ReturnVariable is MissingVariable)) {
                if (PiecesOfCode.NeededVariableDeclarations.Count == 1) {
                    LocalVariableDeclaration lvd = new LocalVariableDeclaration(DataType.parse(ReturnVariable.Type.Name), new VariableDeclaratorCollection(new VariableDeclarator(Identifier.parse(ReturnVariable.Name), newInvocationExpression)));
                    newStatement = new DeclarationStatement(lvd);
                } else {
                    newExpression = new AssignmentExpression(Identifier.parse(ReturnVariable.Name), newInvocationExpression);
                    newStatement = new ExpressionStatement(newExpression, new PositionToken(Position.Missing, ";", Token.SEMICOLON));
                }
            } else {
                newStatement = new ExpressionStatement(newExpression, new PositionToken(Position.Missing, ";", Token.SEMICOLON));
            }
            newStatement.LeadingCharacters = "\r\n";

            //Reindent the new statement to the same level as the first removed one
            newStatement.Format();
            int newStatementIndent = statementsToRemove[0].Indent;
            String newStatementBlock = newStatement.Generate();
            String indentedNewStatementBlock = GenericBlockOfCode.Reindent(newStatementBlock, newStatementIndent, 4);
            Statement indentedNewStatement = Statement.parse(indentedNewStatementBlock);

            //Remove old statements and add new one
            container.Statements.InsertAfter(ExtractedStatements[0], indentedNewStatement);
            for (int i=0; i<statementsToRemove.Count; i++) {
                container.Statements.Remove(statementsToRemove[i] as Statement);
            }
            workingClone.PropagateUp();
            TrailOfBreadCrumbs newStatementCrumbs = indentedNewStatement.Crumbs;

            //Regenerate the whole compilation unit to get positions right and
            //setup highlights for changed positions.
            CompilationUnit ret = CompilationUnit.parse(workingClone.Generate());
            String checkCompilationUnitString = ret.Generate();
            Class retClass = ret.FromTrailOfBreadCrumbs(PiecesOfCode.RootClass.Crumbs) as Class;
            Statement retNewStatement = ret.FromTrailOfBreadCrumbs(newStatementCrumbs) as Statement;
            highlightPositions.Add(new Position((retClass.Members[newMethod.Name] as FundamentalPieceOfCode).Position));
            highlightPositions.Add(new Position(retNewStatement.Position));
            return WrapInCodeCompileUnit(ret, PiecesOfCode.RootCompilationUnit.SourceFilePath, highlightPositions);
		}

		void InsertNewMethod(MethodDeclaration newMethod,Class workingCloneClass)
		{
			ClassMember extractionSource = PiecesOfCode.RootClassMember as ClassMember;
			workingCloneClass.Members.InsertAfter(extractionSource.Name, newMethod);
		}

		void RemoveCommentFromFirstStatement(StatementCollection bodyStatements)
		{
			if ((bodyStatements[0].Comments as PositionTokenCollection).Count > 0)
			{
				PositionToken commentToken = (bodyStatements[0].Comments as PositionTokenCollection)[0] as PositionToken;
				(commentToken.Parent as PositionTokenCollection).Remove(commentToken);
			}
		}

		StatementCollection GetUnindentedBodyStatements()
		{
			StatementCollection bodyStatements = new StatementCollection();
			foreach (Statement statement in ExtractedStatements)
			{
                Statement clone = statement.Clone as Statement;
                clone.Reindent(0);
				bodyStatements.Add(clone);
			}
            bodyStatements.PropagateUp();
			return bodyStatements;
		}

		private StatementCollection ExtractedStatements {
			get {
				StatementCollection ret = new StatementCollection();
				foreach (PieceOfCode poc in PiecesOfCode) {
					if (poc is Statement) {
						ret.Add(poc as Statement);
					}
				}
				return ret;
			}
		}

		/// <summary>
		/// Returns the first modified local scope variable.  This will be
		/// the return value for the extracted method.  If there are more 
		/// than one modified local scope variable, the rest should be 
		/// passed in as reference parameters.
		/// </summary>
		private Variable ReturnVariable {
			get {
				Variable ret = new MissingVariable();
				if (PiecesOfCode.NeededVariableDeclarations.Count == 1) {
					foreach (Variable v in PiecesOfCode.NeededVariableDeclarations) {
						ret = v;
						break;
					}
				} else {
					if (PiecesOfCode.ModifiedLocalScopeVariables.Count == 1) {
						foreach (Variable v in PiecesOfCode.ModifiedLocalScopeVariables) {
							ret = v;
						}
					}
				}
				return ret;
			}
        }

        private DataType ReturnType {
            get {
                DataType ret = DataType.parse(" void");
                if (!(ReturnVariable is MissingVariable)) {
                    ret = DataType.parse(ReturnVariable.Type.Name);
                }
                return ret;
            }
        }

        private FormalParameterList ParameterList
        {
            get
            {
                string ret = "";
                if (PiecesOfCode.ReferencedLocalScopeVariables.Count > 0)
                {
                    foreach (Variable v in PiecesOfCode.ReferencedUnmodifiedLocalScopeVariables)
                    {
                        ret += v.Type.Name + " " + v.Name + ", ";
                    }
                    if (PiecesOfCode.ModifiedLocalScopeVariables.Count == 1)
                    {
                        foreach (Variable v in PiecesOfCode.ModifiedLocalScopeVariables)
                        {
                            ret += v.Type.Name + " " + v.Name;
                        }
                    }
                    else
                    {
                        foreach (Variable v in PiecesOfCode.ModifiedLocalScopeVariables)
                        {
                            ret += "ref " + v.Type.Name + " " + v.Name + ", ";
                        }
                        ret = ret.Substring(0, ret.Length - 2);
                    }
                }
                return FormalParameterList.parse(ret);
            }
        }

        /// <summary>
        /// Returns a string of the arguments in the call to the new method.
        /// </summary>
        private ArgumentCollection Arguments
        {
            get
            {
                string ret = "";
                if (PiecesOfCode.ReferencedLocalScopeVariables.Count > 0)
                {
                    foreach (Variable v in PiecesOfCode.ReferencedUnmodifiedLocalScopeVariables)
                    {
                        ret += v.Name + ", ";
                    }
                    foreach (Variable v in PiecesOfCode.ModifiedLocalScopeVariables)
                    {
                        ret += "ref " + v.Name + ", ";
                    }
                    ret = ret.Substring(0, ret.Length - 2);
                }
                return ArgumentCollection.parse(ret);
            }
        }

        /// <summary>
		/// This verifies that the nodes to extract do not contain a variable declaration 
		/// for a variable that is used outside of the extracted nodes.  If such a node
		/// exists and we extract it, the code would no longer compile.
		/// </summary>
		private bool NodesDoNotContainNeededVariableDeclaration {
			get {
				VariableCollection neededVariableDeclarations = PiecesOfCode.NeededVariableDeclarations;
				bool ret = false;
				if (PiecesOfCode.Count >=0)
				{
					if ((neededVariableDeclarations.Count == 1) &&
						(PiecesOfCode[0].DeclaredVariables.ContainsAnyOf(neededVariableDeclarations)))
					{
						ret = true;
					}
				}
				if ((ret == false) &&
					(neededVariableDeclarations.Count == 0))
				{
					ret = true;
				}
				return ret;
			}
		}

		/// <summary>
		/// This is not strictly necessary.  The idea is that if we do not modify more
		/// than one local scope variable, we do not need to pass in parameters by 
		/// reference to the extracted method.  We might want to add a property to 
		/// enable/disable this check in the future and support out parameters.
		/// </summary>
		private bool NodesDoNotModifyMoreThanOneLocalScopeVariable {
			get {
				//TODO:  Test this implementation.
				bool ret = true;
				if (PiecesOfCode.ModifiedLocalScopeVariables.Count > 1)
				{
					ret = false;
				}
				return ret;
			}
		}

		/// <summary>
		/// It doesn't seem to make sense to extract anything else...
		/// </summary>
		private bool AllNodesAreStatementsOrSingleExpression {
			get {
				bool allNodesAreStatements = true;
				foreach (PieceOfCode poc in PiecesOfCode)
				{
					if (false == (poc is Statement))
					{
						allNodesAreStatements = false;
						break;
					}
				}
				return ((allNodesAreStatements == true) || IsSingleExpression);
			}
		}

		/// <summary>
		/// Verifies if the selected nodes are one and only one expression.
		/// </summary>
		private bool IsSingleExpression {
			get {
				return ((PiecesOfCode.Count == 1) && (PiecesOfCode[0] is Expression));
			}
		}

		/// <summary>
		/// All nodes must be under the same parent and that parent must be of the
		/// appropriate type.
		/// </summary>
		private bool AllNodesAreUnderAppropriateCommonParent {
			get {
                bool ret = false;
				PieceOfCode testNode = PiecesOfCode.ParentOfSelectedNodes;
				ret = ((testNode is ConstructorDeclaration) || 
					   (testNode is DestructorDeclaration) || 
					   (testNode is MethodDeclaration) || 
					   (testNode is PropertyDeclaration));
                if (ret == false) {
                    ClassMember testClassMember = testNode.ParentClassMember;
				    ret = ((testClassMember is ConstructorDeclaration) || 
				           (testClassMember is DestructorDeclaration) || 
				           (testClassMember is MethodDeclaration) || 
			               (testClassMember is PropertyDeclaration));
                }
                return ret;			}
		}
	}
}
