using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Clave.Expressionify.Generator.Internals;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;

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
                Execute(context, syntaxReceiver.Methods);
        }

        private record Expressioned(
            MethodDeclarationSyntax Original, 
            PropertyDeclarationSyntax Replaced, 
            (TypeDeclarationSyntax? Head, IEnumerator<TypeDeclarationSyntax> Tail)? Path)
        {
            public static Expressioned Create(MethodDeclarationSyntax m) => new(
                m, 
                m.ToExpressionProperty(), 
                m.Ancestors().OfType<TypeDeclarationSyntax>().Reverse().HeadAndTail());
        }

        private static void Execute(GeneratorExecutionContext context, IEnumerable<MethodDeclarationSyntax> methods)
        {
            try
            {
                var i = 0;

                static MemberDeclarationSyntax[] Group(IEnumerable<Expressioned> methods) =>
                    methods
                        .GroupBy(x => x.Path?.Head, x => x with { Path = x.Path?.Tail.HeadAndTail() })
                        .Select(g => g.Key.WithOnlyTheseMembers(g
                            .Where(x => x.Path is null)
                            .Select(x => x.Replaced)
                            .GroupBy(p => p.Identifier.Text)
                            .SelectMany(x => x.Select((y, i) => y.GeneratedName(i)))
                            .Concat(Group(g.Where(x => x.Path is not null)))))
                        .ToArray();

                var replacedTypes = methods
                    .Select(Expressioned.Create)
                    .GroupBy(m => m.Original.SyntaxTree.GetRoot(), (root, x) => (root.SyntaxTree.FilePath, root.WithOnlyTheseTypes(Group(x))));

                foreach (var (path, source) in replacedTypes)
                {
                    context.AddSource(Path.GetFileNameWithoutExtension(path)+$"_expressionify_{i++}.cs", SourceText.From(source.ToFullString(), Encoding.UTF8));
                }
            }
            catch (Exception e)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor("EXPR001", "Error generating expression", $"{e.Message}\n{e.StackTrace}", "error",
                        DiagnosticSeverity.Error, true, e.StackTrace), Location.None));
            }
        }

        private class ExpressionifySyntaxReceiver : ISyntaxReceiver
        {
            public readonly HashSet<TypeDeclarationSyntax> Types = new HashSet<TypeDeclarationSyntax>();

            public readonly HashSet<MethodDeclarationSyntax> Methods = new HashSet<MethodDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is MethodDeclarationSyntax methodDeclaration)
                {
                    if (!methodDeclaration.HasExpressionBody()) return;
                    if (!methodDeclaration.HasExpressionifyAttribute()) return;
                    if (!methodDeclaration.IsStatic()) return;

                    if (methodDeclaration.Ancestors().OfType<TypeDeclarationSyntax>().LastOrDefault() is { } type)
                    {
                        if (!type.Modifiers.Includes(SyntaxKind.PartialKeyword)) return;

                        Methods.Add(methodDeclaration);

                        Types.Add(type);
                    }
                }
            }
        }
    }
}
