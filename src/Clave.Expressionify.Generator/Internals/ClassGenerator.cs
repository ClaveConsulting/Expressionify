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
        public static ClassDeclarationSyntax WithOnlyTheseProperties(this ClassDeclarationSyntax c, IEnumerable<MemberDeclarationSyntax> properties)
        {
            // Add the public modifier: (public static partial class Order)
            return ClassDeclaration(c.Identifier.Text)
                .WithModifiers(c.Modifiers)
                .AddMembers(properties.ToArray());
        }

        public static string WithOnlyTheseClasses(this SyntaxNode root, ClassDeclarationSyntax @class)
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
                .AddMembers(@class)
                .NormalizeWhitespace()
                .ToFullString();
        }
    }
}
