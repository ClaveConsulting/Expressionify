namespace Clave.Expressionify.Generator;
using System.Collections.Immutable;
using System.Linq;
using Clave.Expressionify.Generator.Internals;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ExpressionifyAnalyzer : DiagnosticAnalyzer
{
    public const string StaticId = "EXPR001";
    public const string ExpressionBodyId = "EXPR002";
    public const string PartialClassId = "EXPR003";
    public const string NullPropagationId = "EXPR004";

    public static readonly DiagnosticDescriptor StaticRule = new DiagnosticDescriptor(
        id: StaticId,
        title: "Method must be static",
        messageFormat: "Method {0} marked with [Expressionify] must be static",
        category: "Syntax",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ExpressionBodyRule = new DiagnosticDescriptor(
        id: ExpressionBodyId,
        title: "Method must have expression body",
        messageFormat: "Method {0} marked with [Expressionify] must have expression body",
        category: "Syntax",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor PartialClassRule = new DiagnosticDescriptor(
        id: PartialClassId,
        title: "Class must be partial",
        messageFormat: "Class containing a method marked with [Expressionify] must be partial",
        category: "Syntax",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor NullPropagationRule = new DiagnosticDescriptor(
        id: NullPropagationId,
        title: "An expressionify method may not contain a null propagating operator",
        messageFormat: "Method {0} marked with [Expressionify] may not contain null propagation operator",
        category: "Syntax",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        StaticRule,
        ExpressionBodyRule,
        PartialClassRule,
        NullPropagationRule);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        if (!(context.Node is MethodDeclarationSyntax methodDeclaration)) return;

        if (!methodDeclaration.HasExpressionifyAttribute()) return;

        if (!methodDeclaration.IsStatic())
        {
            context.ReportDiagnostic(Diagnostic.Create(
                StaticRule,
                methodDeclaration.GetLocation(),
                methodDeclaration.Identifier.ToString()));
        }

        if (!methodDeclaration.HasExpressionBody())
        {
            context.ReportDiagnostic(Diagnostic.Create(
                ExpressionBodyRule,
                methodDeclaration.GetLocation(),
                methodDeclaration.Identifier.ToString()));
        }

        if (methodDeclaration.FindAncestorMissingPartialKeyword() is SyntaxNode typeNode)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                PartialClassRule,
                typeNode.GetLocation()));
        }

        if (methodDeclaration.FindNullPropagationNode() is SyntaxNode memberNode)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                NullPropagationRule,
                memberNode.GetLocation(),
                methodDeclaration.Identifier.ToString()));
        }
    }
}
