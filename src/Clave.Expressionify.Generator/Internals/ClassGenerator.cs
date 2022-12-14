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
        public static TypeDeclarationSyntax WithOnlyTheseMembers(this TypeDeclarationSyntax type, IEnumerable<MemberDeclarationSyntax> members)
            => TypeDeclaration(type.Kind(), type.Identifier)
                .WithModifiers(type.Modifiers)
                .WithTypeParameterList(type.TypeParameterList)
                .AddMembers(members.ToArray());

        public static SyntaxNode WithOnlyTheseTypes(this SyntaxNode root, IEnumerable<MemberDeclarationSyntax> members)
        {
            var namespaceName = root.DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .FirstOrDefault()?.Name

                ?? root.DescendantNodes()
                .OfType<FileScopedNamespaceDeclarationSyntax>()
                .FirstOrDefault().Name;

            var usings = root.DescendantNodes()
                .OfType<UsingDirectiveSyntax>()
                .ToArray();

            return NamespaceDeclaration(namespaceName)
                .AddUsings(usings)
                .AddMembers(members.ToArray())
                .NormalizeWhitespace();
        }
    }
}
