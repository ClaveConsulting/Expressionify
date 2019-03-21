using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Expressionate.CodeGen
{
  public static class ClassGenerator
    {
        public static string WithOnlyTheseProperties(this SyntaxNode oldClass, IEnumerable<PropertyDeclarationSyntax> properties)
        {
            var namespaceName = oldClass.DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .First()
                .Name;

            var usings = oldClass.DescendantNodes()
                .OfType<UsingDirectiveSyntax>()
                .ToArray();

            var className = oldClass.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .First()
                .Identifier
                .Text;

            // Create a namespace: (namespace CodeGenerationSample)
            var @namespace = NamespaceDeclaration(namespaceName).NormalizeWhitespace();

            // Add System using statement: (using System)
            @namespace = @namespace.AddUsings(usings);

            //  Create a class: (class Order)
            var classDeclaration = ClassDeclaration($"{className}_Expressionate");

            // Add the public modifier: (public class Order)
            classDeclaration = classDeclaration.AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword));

            classDeclaration = classDeclaration.AddMembers(properties.ToArray());

            @namespace = @namespace.AddMembers(classDeclaration);

            return @namespace
                .NormalizeWhitespace()
                .ToFullString();
        }
    }
}
