using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clave.Expressionify.Generator
{
    [Generator]
    public class ExpressionifySourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var syntaxReceiver = (ExpressionifySyntaxReceiver)context.SyntaxReceiver;

            var i = 0;
            foreach (var @class in syntaxReceiver.Classes)
            {
                var generatedClass = @class.GenerateExpressionClass();
                if (generatedClass == null) continue;
                var source = @class.SyntaxTree.GetRoot().WithOnlyTheseClasses(generatedClass);

                context.AddSource($"Generated_{i++}.cs", SourceText.From(source, Encoding.UTF8));
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new ExpressionifySyntaxReceiver());
        }

        class ExpressionifySyntaxReceiver : ISyntaxReceiver
        {
            public List<ClassDeclarationSyntax> Classes { get; private set; } = new List<ClassDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is ClassDeclarationSyntax classDeclaration)
                {
                    Classes.Add(classDeclaration);
                }
            }
        }
    }
}
