using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Clave.Expressionify.Tasks
{
    public static class ClassGenerator
    {
        public static ClassDeclarationSyntax WithOnlyTheseProperties(this ClassDeclarationSyntax oldClass, IEnumerable<PropertyDeclarationSyntax> properties)
        {
            var className = oldClass.Identifier.Text;

            // Add the public modifier: (public class Order)
            return ClassDeclaration($"{className}_Expressionify")
                .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                .AddMembers(properties.ToArray());
        }

        public static string WithOnlyTheseClasses(this SyntaxNode root, params ClassDeclarationSyntax[] classes)
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
            
            @namespace = @namespace.AddMembers(classes);

            return @namespace
                .NormalizeWhitespace()
                .ToFullString();
        }
    }
}
