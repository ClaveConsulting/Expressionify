using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Clave.Expressionify
{
    public class ExpressionifyQueryTranslationPreprocessor : QueryTranslationPreprocessor
    {
        readonly QueryTranslationPreprocessor _innerPreprocessor;

        public ExpressionifyQueryTranslationPreprocessor(QueryTranslationPreprocessor innerPreprocessor, QueryTranslationPreprocessorDependencies dependencies, QueryCompilationContext compilationContext) 
            : base(dependencies, compilationContext)
        {
            _innerPreprocessor = innerPreprocessor;
        }

        public override Expression Process(Expression query)
        {
            var expandedExpression = new ExpressionifyVisitor().Visit(query);
            return _innerPreprocessor.Process(expandedExpression);
        }
    }
}