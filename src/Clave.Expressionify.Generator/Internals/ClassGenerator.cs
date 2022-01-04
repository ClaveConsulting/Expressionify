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
            => TypeDeclaration(type.Kind(), type.Identifier)
                .WithModifiers(type.Modifiers)
                .AddMembers(properties.ToArray());

        public static void PathsToTree(this IReadOnlyList<TypeDeclarationSyntax[]> paths)
        {
            // A, B, C
            // A, B
            // A
            // D
            // D, E
            // D, F

            // groupBy [0], skip 1

            // A: 
            //  B, C
            //  B
            //  -
            // D:
            //  -
            //  E
            //  F


        }

        public static string WithOnlyTheseTypes(this SyntaxNode root, params MemberDeclarationSyntax[] members)
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
                .AddMembers(members)
                .NormalizeWhitespace()
                .ToFullString();
        }
    }
}
