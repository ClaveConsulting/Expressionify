using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Clave.Expressionify.Generator.Internals
{
    public static class Checks
    {
        public static bool HasExpressionifyAttribute(this MethodDeclarationSyntax m) =>
            m.AttributeLists.SelectMany(l => l.Attributes).Any(a => a.Name.ToString() == "Expressionify");

        public static bool IsStatic(this MethodDeclarationSyntax method) =>
            method.Modifiers.Includes(SyntaxKind.StaticKeyword);

        public static bool Includes(this SyntaxTokenList modifiers, SyntaxKind modifier) =>
            modifiers.Any(m => m.IsKind(modifier));

        public static bool HasExpressionBody(this MethodDeclarationSyntax method) =>
            method.ExpressionBody is not null;

        public static TypeDeclarationSyntax? FindAncestorMissingPartialKeyword(this MethodDeclarationSyntax method) =>
            method.Ancestors().OfType<TypeDeclarationSyntax>().FirstOrDefault(t => !t.Modifiers.Includes(SyntaxKind.PartialKeyword));

        public static SyntaxNode? FindNullPropagationNode(this MethodDeclarationSyntax method) =>
            method.DescendantNodes().OfType<ConditionalAccessExpressionSyntax>().FirstOrDefault();
    }
}