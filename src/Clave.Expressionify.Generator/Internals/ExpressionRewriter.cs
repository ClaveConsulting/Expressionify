namespace Clave.Expressionify.Generator.Internals;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

internal sealed class ExpressionRewriter : CSharpSyntaxRewriter
{
    private readonly Compilation _compilation;
    private readonly SyntaxNode _root;

    public ExpressionRewriter(Compilation compilation, SyntaxNode root)
    {
        _compilation = compilation;
        _root = root;
    }

    public ExpressionSyntax VisitExpression(ExpressionSyntax expression) =>
        (ExpressionSyntax)Visit(expression);

    public override SyntaxNode Visit(SyntaxNode node)
    {
        var res = base.Visit(node);

        if (SyntaxFactory.AreEquivalent(node, res))
            return res;

        return _root
            .ReplaceNode(node, res)
            .DescendantNodes()
            .Where(x => x.FullSpan.Start == node.FullSpan.Start)
            .Where(x => x.FullSpan.Length == res.FullSpan.Length)
            .First(x => x.GetType() == res.GetType());
    }

    public override SyntaxNode? VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node)
    {
        var expression = Visit(node.Expression);
        var expressionType = _compilation
            .ReplaceSyntaxTree(_root.SyntaxTree, expression.SyntaxTree)
            .GetSemanticModel(expression.SyntaxTree)
            .GetTypeInfo(expression)
            .Type;

        var forgiver = expressionType is INamedTypeSymbol
        {
            IsValueType: true,
            OriginalDefinition.SpecialType: SpecialType.System_Nullable_T,
        }
            ? "!.Value"
            : "!";

        return SyntaxFactory.ParseExpression($"{expression}{forgiver}{Visit(node.WhenNotNull)}");
    }
}
