using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Clave.Expressionify
{
    public class ExpressionifyQueryTranslationPreprocessor : QueryTranslationPreprocessor
    {
        private readonly QueryTranslationPreprocessor _innerPreprocessor;

        public ExpressionifyQueryTranslationPreprocessor(
            QueryTranslationPreprocessor innerPreprocessor, 
            QueryTranslationPreprocessorDependencies dependencies,
            QueryCompilationContext compilationContext)
            : base(dependencies, compilationContext)
        {
            _innerPreprocessor = innerPreprocessor;
        }

        public override Expression Process(Expression query)
        {
            var visitor = new ExpressionifyVisitor();
            query = visitor.Visit(query);

            if (visitor.HasReplacedCalls)
               EnsureNoNewParametersExist(query);

            return _innerPreprocessor.Process(query);
        }

        private void EnsureNoNewParametersExist(Expression query)
        {
            var visitor = new ParameterExtractingExpressionVisitor(
                Dependencies.EvaluatableExpressionFilter,
                new ThrowOnParameterAccess(),
                QueryCompilationContext.ContextType,
                QueryCompilationContext.Model,
                QueryCompilationContext.Logger,
                parameterize: true,
                generateContextAccessors: false);

            visitor.ExtractParameters(query);
        }

        private class ThrowOnParameterAccess : IParameterValues
        {
            public void AddParameter(string name, object? value)
                => throw new InvalidOperationException(
                    "Adding parameters in a cached query context is not allowed. Explicitly call .Expressionify() on the query or use ExpressionEvaluationMode.Always.");
        
            public IReadOnlyDictionary<string, object?> ParameterValues
                => throw new InvalidOperationException(
                    "Accessing parameters in a cached query context is not allowed. Explicitly call .Expressionify() on the query or use ExpressionEvaluationMode.Always.");
        }
    }
}