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
using System.CodeDom;
using System.CodeDom.Compiler;

namespace Opnieuw.Framework
{
	public class MissingCodeGenerator : CodeGenerator
	{
		protected override string NullToken {
			get {
				return "";
			}
		}

		protected override void OutputType(CodeTypeReference typeRef)
		{
		}

		protected override void GenerateBaseReferenceExpression(CodeBaseReferenceExpression e)
		{
		}

		protected override void GenerateCastExpression(CodeCastExpression e)
		{
		}

		protected override void GenerateDelegateCreateExpression(CodeDelegateCreateExpression e)
		{
		}

		protected override void GenerateFieldReferenceExpression(CodeFieldReferenceExpression e)
		{
		}

		protected override void GenerateArgumentReferenceExpression(CodeArgumentReferenceExpression e)
		{
		}

		protected override void GenerateVariableReferenceExpression(CodeVariableReferenceExpression e)
		{
		}

		protected override void GenerateArrayCreateExpression(CodeArrayCreateExpression e)
		{
		}

		protected override void GenerateArrayIndexerExpression(CodeArrayIndexerExpression e)
		{
		}

		protected override void GenerateSnippetExpression(CodeSnippetExpression e)
		{
		}

		protected override void GenerateMethodInvokeExpression(CodeMethodInvokeExpression e)
		{
		}

		protected override void GenerateIndexerExpression(CodeIndexerExpression e)
		{
		}

		protected override void GenerateMethodReferenceExpression(CodeMethodReferenceExpression e)
		{
		}

		protected override void GenerateEventReferenceExpression(CodeEventReferenceExpression e)
		{
		}

		protected override void GenerateDelegateInvokeExpression(CodeDelegateInvokeExpression e)
		{
		}

		protected override void GenerateObjectCreateExpression(CodeObjectCreateExpression e)
		{
		}

		protected override void GeneratePropertyReferenceExpression(CodePropertyReferenceExpression e)
		{
		}

		protected override void GenerateThisReferenceExpression(CodeThisReferenceExpression e)
		{
		}

		protected override void GeneratePropertySetValueReferenceExpression(CodePropertySetValueReferenceExpression e)
		{
		}

		protected override void GenerateExpressionStatement(CodeExpressionStatement s)
		{
		}

		protected override void GenerateIterationStatement(CodeIterationStatement s)
		{
		}

		protected override void GenerateThrowExceptionStatement(CodeThrowExceptionStatement s)
		{
		}

		protected override void GenerateComment(CodeComment c)
		{
		}

		protected override void GenerateMethodReturnStatement(CodeMethodReturnStatement s)
		{
		}

		protected override void GenerateConditionStatement(CodeConditionStatement s)
		{
		}

		protected override void GenerateTryCatchFinallyStatement(CodeTryCatchFinallyStatement s)
		{
		}

		protected override void GenerateAssignStatement(CodeAssignStatement s)
		{
		}

		protected override void GenerateAttachEventStatement(CodeAttachEventStatement s)
		{
		}

		protected override void GenerateRemoveEventStatement(CodeRemoveEventStatement s)
		{
		}

		protected override void GenerateGotoStatement(CodeGotoStatement s)
		{
		}

		protected override void GenerateLabeledStatement(CodeLabeledStatement s)
		{
		}

		protected override void GenerateVariableDeclarationStatement(CodeVariableDeclarationStatement s)
		{
		}

		protected override void GenerateLinePragmaStart(CodeLinePragma s)
		{
		}

		protected override void GenerateLinePragmaEnd(CodeLinePragma s)
		{
		}

		protected override void GenerateEvent(CodeMemberEvent e, CodeTypeDeclaration d)
		{
		}

		protected override void GenerateField(CodeMemberField s)
		{
		}

		protected override void GenerateSnippetMember(CodeSnippetTypeMember s)
		{
		}

		protected override void GenerateEntryPointMethod(CodeEntryPointMethod e, CodeTypeDeclaration d)
		{
		}

		protected override void GenerateMethod(CodeMemberMethod e, CodeTypeDeclaration d)
		{
		}

		protected override void GenerateProperty(CodeMemberProperty e, CodeTypeDeclaration d)
		{
		}

		protected override void GenerateConstructor(CodeConstructor e, CodeTypeDeclaration d)
		{
		}

		protected override void GenerateTypeConstructor(CodeTypeConstructor c)
		{
		}

		protected override void GenerateTypeStart(CodeTypeDeclaration c)
		{
		}

		protected override void GenerateTypeEnd(CodeTypeDeclaration c)
		{
		}

		protected override void GenerateNamespaceStart(CodeNamespace n)
		{
		}

		protected override void GenerateNamespaceEnd(CodeNamespace c)
		{
		}

		protected override void GenerateNamespaceImport(CodeNamespaceImport n)
		{
		}

		protected override void GenerateAttributeDeclarationsStart(CodeAttributeDeclarationCollection c)
		{
		}

		protected override void GenerateAttributeDeclarationsEnd(CodeAttributeDeclarationCollection c)
		{
		}

		protected override bool Supports(GeneratorSupport s)
		{
			return true;
		}

		protected override bool IsValidIdentifier(string value)
		{
			return true;
		}

		protected override string CreateEscapedIdentifier(string value)
		{
			return "";
		}

		protected override string CreateValidIdentifier(string value)
		{
			return "";
		}

		protected override string GetTypeOutput(CodeTypeReference value)
		{
			return "";
		}

		protected override string QuoteSnippetString(string value)
		{
			return "";
		}
	}
}
