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
               query = EvaluateExpression(query);

            return _innerPreprocessor.Process(query);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "<Pending>")]
        private Expression EvaluateExpression(Expression query)
        {
            // 1) Ensure that no new parameters are introduced when creating the query
            // 2) This expression visitor also makes slight optimizations, like replacing evaluatable expressions.

            // With EF10, ParameterExtractingExpressionVisitor was removed and replaced by ExpressionTreeFuncletizer
            // ExpressionTreeFuncletizer now uses Dictionary<string, object?> instead of IParameterValues
            var funcletizer = new ExpressionTreeFuncletizer(
                QueryCompilationContext.Model,
                Dependencies.EvaluatableExpressionFilter,
                QueryCompilationContext.ContextType,
                generateContextAccessors: false,
                QueryCompilationContext.Logger);

            var throwOnAccess = new ThrowOnParameterAccess();
            var result = funcletizer.ExtractParameters(query, throwOnAccess, parameterize: true, clearParameterizedValues: true);

            // Check if parameters were added by accessing the base Dictionary
            // ExpressionTreeFuncletizer bypasses our 'new' method overrides because Dictionary<> is not designed
            // to be subclassed, so we check the Count via the base class after funcletization completes
            if (((Dictionary<string, object?>)throwOnAccess).Count > 0)
            {
                throw new InvalidOperationException(
                    "Adding parameters in a cached query context is not allowed. " +
                    $"Explicitly call .{nameof(ExpressionifyExtension.Expressionify)}() on the query or use {nameof(ExpressionEvaluationMode)}.{nameof(ExpressionEvaluationMode.FullCompatibilityButSlow)}.");
            }

            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "<Pending>")]
        private class ThrowOnParameterAccess : Dictionary<string, object?>
        {
            // This class exists primarily for documentation purposes - to make it clear that parameter
            // access should throw an exception in cached mode. However, ExpressionTreeFuncletizer bypasses
            // these 'new' method overrides by calling base Dictionary<> methods directly.
            // The actual check happens in EvaluateExpression() after funcletization completes.

            private static InvalidOperationException CreateException()
                => new InvalidOperationException(
                    "Adding parameters in a cached query context is not allowed. " +
                    $"Explicitly call .{nameof(ExpressionifyExtension.Expressionify)}() on the query or use {nameof(ExpressionEvaluationMode)}.{nameof(ExpressionEvaluationMode.FullCompatibilityButSlow)}.");

            public new object? this[string key]
            {
                get => throw CreateException();
                set => throw CreateException();
            }

            public new void Add(string key, object? value)
                => throw CreateException();

            public new bool TryAdd(string key, object? value)
                => throw CreateException();

            public new bool TryGetValue(string key, out object? value)
                => throw CreateException();

            public new bool ContainsKey(string key)
                => throw CreateException();

            public new int Count
                => throw CreateException();
        }
    }
}