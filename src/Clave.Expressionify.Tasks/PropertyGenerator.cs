using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Clave.Expressionify.CodeGen
{
    public static class Functionality
    {
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
                ParenthesizedLambdaExpression(method.ExpressionBody.Expression)
                    .WithParameterList(ParameterList(SeparatedList(method.ParameterList.Parameters.Select(p => p.WithModifiers(TokenList()))))));
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
