using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Clave.Expressionify.Generator.Internals
{
    public static class PropertyGenerator
    {
        public static PropertyDeclarationSyntax GeneratedName(this PropertyDeclarationSyntax p, int i)
        {
            return p.WithIdentifier(Identifier($"{p.Identifier.Text}_Expressionify_{i}"));
        }

        public static PropertyDeclarationSyntax ToExpressionProperty(this MethodDeclarationSyntax method)
        {
            var parameterTypes = method.ParameterList.Parameters
                .Select(p => p.Type)
                .Concat(new[] { method.ReturnType });

            var type = GetExpressionType(parameterTypes);

            return PropertyDeclaration(type, method.Identifier.ValueText)
                .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.StaticKeyword)))
                .WithAccessorList(GetOnly())
                .WithInitializer(GetBody(method))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                .WithTrailingTrivia(Whitespace("\n"));
        }

        private static EqualsValueClauseSyntax GetBody(BaseMethodDeclarationSyntax method)
        {
            return EqualsValueClause(
                ParenthesizedLambdaExpression(
                    ParameterList(SeparatedList(method.ParameterList.Parameters.Select(p => p.WithModifiers(TokenList())))),
                    method.ExpressionBody!.Expression
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
