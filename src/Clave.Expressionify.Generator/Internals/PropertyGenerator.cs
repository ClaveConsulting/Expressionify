using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Clave.Expressionify.Generator.Internals
{
    public static class PropertyGenerator
    {
        public static MethodDeclarationSyntax GeneratedName(this MethodDeclarationSyntax p, int i)
            => p.WithIdentifier(Identifier($"{p.Identifier.Text}_Expressionify_{i}"));

        public static MethodDeclarationSyntax ToExpressionMethod(this MethodDeclarationSyntax method)
            => MethodDeclaration(GetExpressionType(method), method.Identifier)
                .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.StaticKeyword)))
                .WithTypeParameterList(method.TypeParameterList)
                .WithConstraintClauses(method.ConstraintClauses)
                .WithExpressionBody(ArrowExpressionClause(GetBody(method)))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                .NormalizeWhitespace();

        private static ParenthesizedLambdaExpressionSyntax GetBody(BaseMethodDeclarationSyntax method)
            => ParenthesizedLambdaExpression(
                    ParameterList(SeparatedList(method.ParameterList.Parameters.Select(p => p.WithModifiers(TokenList())))),
                    method.ExpressionBody!.Expression
                );

        private static QualifiedNameSyntax GetExpressionType(MethodDeclarationSyntax method) =>
            Expression(Func(
                SeparatedList(method.ParameterList.Parameters
                    .Select(p => p.Type!)
                    .ToArray()
                    .Append(method.ReturnType))
            ));

        private static QualifiedNameSyntax Func(SeparatedSyntaxList<TypeSyntax> types) =>
            QualifiedName(
                IdentifierName("System"),
                GenericName(Identifier("Func"))
                    .WithTypeArgumentList(TypeArgumentList(types))
            );

        private static QualifiedNameSyntax Expression(TypeSyntax genericPart) =>
            QualifiedName(
                QualifiedName(QualifiedName(IdentifierName("System"), IdentifierName("Linq")), IdentifierName("Expressions")),
                GenericName(Identifier("Expression"))
                    .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(genericPart)))
            );
    }
}
