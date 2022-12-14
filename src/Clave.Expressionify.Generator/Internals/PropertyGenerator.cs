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

        public static MethodDeclarationSyntax GeneratedName(this MethodDeclarationSyntax p, int i)
        {
            return p.WithIdentifier(Identifier($"{p.Identifier.Text}_Expressionify_{i}"));
        }

        public static PropertyDeclarationSyntax ToExpressionProperty(this MethodDeclarationSyntax method)
        {
            return PropertyDeclaration(GetExpressionType(method), method.Identifier.ValueText)
                .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.StaticKeyword)))
                .WithAccessorList(GetOnly())
                .WithInitializer(EqualsValueClause(GetBody(method)))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                .WithTrailingTrivia(Whitespace("\n"));
        }

        public static MethodDeclarationSyntax ToExpressionMethod(this MethodDeclarationSyntax method)
        {
            return MethodDeclaration(GetExpressionType(method), method.Identifier)
                .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.StaticKeyword)))
                .WithTypeParameterList(method.TypeParameterList)
                .WithConstraintClauses(method.ConstraintClauses)
                .WithExpressionBody(ArrowExpressionClause(GetBody(method)))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                .NormalizeWhitespace();
        }

        private static ParenthesizedLambdaExpressionSyntax GetBody(BaseMethodDeclarationSyntax method)
        {
            return
                ParenthesizedLambdaExpression(
                    ParameterList(SeparatedList(method.ParameterList.Parameters.Select(p => p.WithModifiers(TokenList())))),
                    method.ExpressionBody!.Expression
                )
            ;
        }

        public static AccessorListSyntax GetOnly() =>
            AccessorList(
                Token(SyntaxKind.OpenBraceToken),
                SingletonList(
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))),
                Token(SyntaxKind.CloseBraceToken)
            );

        private static QualifiedNameSyntax GetExpressionType(MethodDeclarationSyntax method) =>
            Expression(TypeArgumentList(
                SingletonSeparatedList<TypeSyntax>(Func(TypeArgumentList(
                    SeparatedList(method.ParameterList.Parameters
                        .Select(p => p.Type!)
                        .Concat(new[] { method.ReturnType })))))));

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
