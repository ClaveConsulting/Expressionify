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
                .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword), Token(SyntaxKind.PartialKeyword))
                .AddMembers(properties.ToArray());
        }

        public static string WithOnlyTheseClasses(this SyntaxNode root, ClassDeclarationSyntax @class)
        {
            var namespaceName = root.DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .First()
                .Name;

            var usings = root.DescendantNodes()
                .OfType<UsingDirectiveSyntax>()
                .ToArray();


            // Create a namespace: (namespace CodeGenerationSample)
            var @namespace = NamespaceDeclaration(namespaceName).NormalizeWhitespace();

            // Add System using statement: (using System)
            @namespace = @namespace.AddUsings(usings);

            @namespace = @namespace.AddMembers(@class);

            return @namespace
                .NormalizeWhitespace()
                .ToFullString();
        }
    }
}
