using System;
using System.CodeDom;
using Opnieuw.Framework;
using System.Collections;
using Opnieuw.Parsers.CSParser;

namespace Opnieuw.Refactorings.RenamePrivateMethod
{
	public class RenamePrivateMethodRefactoring : Refactoring
	{
        public override void OnNodeSelectionChanged(object source, ArrayList selectedNodes)
        {
			base.OnNodeSelectionChanged(source, selectedNodes);
			bool isAvailable = false;
			if (selectedNodes.Count == 1) {
				if (selectedNodes[0] is MethodDeclaration) {
					MethodDeclaration method = selectedNodes[0] as MethodDeclaration;
					if (method.IsPrivate == true) {
						isAvailable = true;
					}
				}
			}
			FireAvailabilityChanged(isAvailable);
		}

		public override CodeChangeCommandCollection CodeChanges {
			get {
				CodeChangeCommandCollection ret = new CodeChangeCommandCollection();
				MethodDeclaration method = m_SelectedNodes[0] as MethodDeclaration;
				CompilationUnit parentCompilationUnit = method.ParentCompilationUnit;

				//rename the method
				Position insertPosition = new Position(method.Identifier.Position);
				insertPosition.EndCol ++;
				ReplaceCodeChangeCommand command1 = new ReplaceCodeChangeCommand(
														  parentCompilationUnit.SourceFilePath, 
														  new CodeReplacementCollection(new CodeReplacement(insertPosition, m_NewMethodName)));
				ret.Add(command1);

				//find and replace all the invocations 
				CodeReplacementCollection codeReplacements = new CodeReplacementCollection();
				if (method.Parent is Class)
				{
					Class cls = method.Parent as Class;
					foreach (Expression expression in cls.Expressions)
					{
						if (expression is InvocationExpression)
						{
							InvocationExpression invocation = expression as InvocationExpression;
							if (invocation.Expression is MemberAccessExpression)
							{
								MemberAccessExpression memberAccess = invocation.Expression as MemberAccessExpression;
								if ((memberAccess.Identifier.Name == method.Identifier.Name) &&
									(memberAccess.Expression is ThisAccessExpression))
								{
									//we have something like this.methodName()
									Position tempInsertPosition = new Position(memberAccess.Identifier.Position);
									tempInsertPosition.EndCol ++;
									codeReplacements.Add(new CodeReplacement(tempInsertPosition, m_NewMethodName));
								}
							}
							else if (invocation.Expression is Identifier)
							{
								Identifier identifier = invocation.Expression as Identifier;
								if (identifier.Name == method.Identifier.Name)
								{
									//we have something like methodName()
									Position tempInsertPosition = new Position(identifier.Position);
									tempInsertPosition.EndCol ++;
									codeReplacements.Add(new CodeReplacement(tempInsertPosition, m_NewMethodName));
								}
							}
						}
					}
				}
				if (codeReplacements.Count > 0)
				{
					ReplaceCodeChangeCommand tempCommand = new ReplaceCodeChangeCommand(
											parentCompilationUnit.SourceFilePath,
											codeReplacements);
					ret.Add(tempCommand);
				}
				ret.Reverse();
				return ret;
			}
		}

		protected string m_NewMethodName = "";
		public string NewMethodName {
			set {
				m_NewMethodName = value;
			}
		}
	}
}
