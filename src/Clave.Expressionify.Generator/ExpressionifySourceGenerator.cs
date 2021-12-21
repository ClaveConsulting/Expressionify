using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Clave.Expressionify.Generator.Internals;
using Microsoft.CodeAnalysis.CSharp;

namespace Clave.Expressionify.Generator
{
    [Generator]
    public class ExpressionifySourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new ExpressionifySyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is ExpressionifySyntaxReceiver syntaxReceiver)
                Execute(context, syntaxReceiver.Types);
        }

        private static void Execute(GeneratorExecutionContext context, IEnumerable<TypeDeclarationSyntax> types)
        {
            try
            {
                var i = 0;
                foreach (var type in types)
                {
                    var generatedClass = type.GenerateExpressionType();
                    if (generatedClass == null) continue;
                    var source = type.SyntaxTree.GetRoot().WithOnlyTheseTypes(generatedClass);

                    context.AddSource($"Generated_{i++}.cs", SourceText.From(source, Encoding.UTF8));
                }
            }
            catch (Exception e)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor("EXPR001", "Error generating expression", e.Message+"\n"+e.StackTrace, "error",
                        DiagnosticSeverity.Error, true, e.StackTrace), Location.None));
            }
        }

        private class ExpressionifySyntaxReceiver : ISyntaxReceiver
        {
            public readonly HashSet<TypeDeclarationSyntax> Types = new HashSet<TypeDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is MethodDeclarationSyntax methodDeclaration)
                {
                    if (!methodDeclaration.HasExpressionBody()) return;
                    if (!methodDeclaration.HasExpressionifyAttribute()) return;
                    if (!methodDeclaration.IsStatic()) return;

                    if (methodDeclaration.Ancestors().OfType<TypeDeclarationSyntax>().FirstOrDefault() is { } type)
                    {
                        if (!type.Modifiers.Includes(SyntaxKind.PartialKeyword)) return;

                        Types.Add(type);
                    }
                }
            }
        }
    }
}
