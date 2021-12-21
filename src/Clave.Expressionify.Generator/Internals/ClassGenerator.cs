using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Clave.Expressionify.Generator.Internals
{
    public static class ClassGenerator
    {
        public static TypeDeclarationSyntax WithOnlyTheseProperties(this TypeDeclarationSyntax type, IEnumerable<MemberDeclarationSyntax> properties)
        {
            // Add the public modifier: (public static partial class Order)
            return TypeDeclaration(type.Kind(), type.Identifier)
                .WithModifiers(type.Modifiers)
                .AddMembers(properties.ToArray());
        }

        public static string WithOnlyTheseTypes(this SyntaxNode root, TypeDeclarationSyntax member)
        {
            var namespaceName = root.DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .FirstOrDefault()?.Name

                ?? root.DescendantNodes()
                .OfType<FileScopedNamespaceDeclarationSyntax>()
                .FirstOrDefault()
                .Name;

            var usings = root.DescendantNodes()
                .OfType<UsingDirectiveSyntax>()
                .ToArray();

            return NamespaceDeclaration(namespaceName)
                .AddUsings(usings)
                .AddMembers(member)
                .NormalizeWhitespace()
                .ToFullString();
        }
    }
}
