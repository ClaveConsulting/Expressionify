using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Document = Microsoft.CodeAnalysis.Document;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Clave.Expressionify.Generator
{

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ExpressionifyCodeFixProvider)), Shared]
    public class ExpressionifyCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
            ExpressionifyAnalyzer.StaticId,
            ExpressionifyAnalyzer.PartialClassId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location;

            // Find the type declaration identified by the diagnostic.
            var syntaxNode = root!.FindNode(diagnostic.Location.SourceSpan);

            if (diagnostic.Id == ExpressionifyAnalyzer.StaticId)
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: "Add static keyword",
                        createChangedDocument: c =>
                            FixMissingStatic(context.Document, root, (syntaxNode as MethodDeclarationSyntax)!),
                        equivalenceKey: ExpressionifyAnalyzer.StaticId),
                    diagnostic);

            if (diagnostic.Id == ExpressionifyAnalyzer.PartialClassId)
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: "Add partial keyword",
                        createChangedDocument: c =>
                            FixMissingPartial(context.Document, root, (syntaxNode as ClassDeclarationSyntax)!),
                        equivalenceKey: ExpressionifyAnalyzer.PartialClassId),
                    diagnostic);
        }

        private static Task<Document> FixMissingStatic(Document contextDocument, SyntaxNode root, MethodDeclarationSyntax method)
        {
            return Task.FromResult(contextDocument.WithSyntaxRoot(root.ReplaceNode(method, method.AddModifiers(Token(SyntaxKind.StaticKeyword)))));
        }

        private static Task<Document> FixMissingPartial(Document contextDocument, SyntaxNode root, ClassDeclarationSyntax @class)
        {
            return Task.FromResult(contextDocument.WithSyntaxRoot(root.ReplaceNode(@class, @class.AddModifiers(Token(SyntaxKind.PartialKeyword)))));
        }
    }
}