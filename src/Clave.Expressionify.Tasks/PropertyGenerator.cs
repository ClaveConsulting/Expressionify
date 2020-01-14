using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Clave.Expressionify.Tasks
{
    public static class Functionality
    {
        public static IReadOnlyList<ClassDeclarationSyntax> TransformClasses(this SyntaxNode root)
        {
            return root
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Select(GenerateExpressionClass)
                .Where(x => x != null)
                .ToList();
        }

        private static ClassDeclarationSyntax GenerateExpressionClass(ClassDeclarationSyntax c)
        {
            var props = c.TransformProperties()
                .GroupBy(p => p.Identifier.Text)
                .SelectMany(x => x.Select((p, i) => p.WithIdentifier(Identifier($"{p.Identifier.Text}_{i}"))))
                .ToList();

            return props.Any() ? c.WithOnlyTheseProperties(props) : null;
        }

        public static IReadOnlyList<PropertyDeclarationSyntax> TransformProperties(this SyntaxNode root)
        {
            var methods = root
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Where(m => m.AttributeLists
                    .SelectMany(l => l.Attributes)
                    .Any(a => a.Name.ToString() == "Expressionify"));

            var properties = new List<PropertyDeclarationSyntax>();

            foreach (var method in methods)
            {
                if (method.ExpressionBody == null)
                {
                    var line = 1 + method.Identifier.GetLocation().GetMappedLineSpan().StartLinePosition.Line;
                    throw new CodeGenException($"({line}): error 0: A method with [Expressionify] attribute must have expression body");
                }

                if (method.Modifiers.Any(m => m.Kind() == SyntaxKind.StaticKeyword) == false)
                {
                    var line = 1 + method.Identifier.GetLocation().GetMappedLineSpan().StartLinePosition.Line;
                    throw new CodeGenException($"({line}): error 0: A method with [Expressionify] attribute must be static. Make it an extension method if you need to use properties of the object");

                }

                try
                {
                    properties.Add(method.ToExpressionProperty());
                }
                catch (Exception e)
                {
                    throw new CodeGenException($" Error parsing {method.Identifier}: {e.Message}");
                }
            }

            return properties;
        }

        public static PropertyDeclarationSyntax ToExpressionProperty(this MethodDeclarationSyntax method)
        {
            var parameterTypes = method.ParameterList.Parameters.Select(p => p.Type).Concat(new[] { method.ReturnType });

            var type = GetExpressionType(parameterTypes);

            return PropertyDeclaration(type, method.Identifier.ValueText)
                .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                .WithAccessorList(GetOnly())
                .WithInitializer(GetBody(method))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                .WithTrailingTrivia(Whitespace("\n"));
        }

        private static EqualsValueClauseSyntax GetBody(MethodDeclarationSyntax method)
        {
            return EqualsValueClause(
                ParenthesizedLambdaExpression(
                    ParameterList(SeparatedList(method.ParameterList.Parameters.Select(p => p.WithModifiers(TokenList())))),
                    method.ExpressionBody.Expression
                )
            );
        }

        public static AccessorListSyntax GetOnly() =>
            AccessorList(
                Token(SyntaxKind.OpenBraceToken),
                SingletonList(
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))),
                Token(SyntaxKind.CloseBraceToken)
            );

        private static QualifiedNameSyntax GetExpressionType(IEnumerable<TypeSyntax> parameters) =>
            Expression(TypeArgumentList(
                SingletonSeparatedList<TypeSyntax>(Func(TypeArgumentList(
                    SeparatedList(parameters))))));

        private static QualifiedNameSyntax Func(TypeArgumentListSyntax types) =>
            QualifiedName(System,
                GenericName(Identifier("Func")).WithTypeArgumentList(types));

        private static QualifiedNameSyntax Expression(TypeArgumentListSyntax genericPart) =>
            QualifiedName(
                QualifiedName(QualifiedName(System, IdentifierName("Linq")), IdentifierName("Expressions")),
                GenericName(Identifier("Expression")).WithTypeArgumentList(genericPart));

        private static IdentifierNameSyntax System => IdentifierName("System");
    }
}
