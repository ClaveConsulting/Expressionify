using Microsoft.EntityFrameworkCore.Query;

namespace Clave.Expressionify;

public class ExpressionifyQueryTranslationPreprocessorFactory : IQueryTranslationPreprocessorFactory
{
    private readonly IQueryTranslationPreprocessorFactory _innerFactory;
    private readonly QueryTranslationPreprocessorDependencies _preprocessorDependencies;

    public ExpressionifyQueryTranslationPreprocessorFactory(IQueryTranslationPreprocessorFactory innerFactory, QueryTranslationPreprocessorDependencies preprocessorDependencies)
    {
        _innerFactory = innerFactory;
        _preprocessorDependencies = preprocessorDependencies;
    }

    public QueryTranslationPreprocessor Create(QueryCompilationContext queryCompilationContext)
    {
        var preprocessor = _innerFactory.Create(queryCompilationContext);
        return new ExpressionifyQueryTranslationPreprocessor(preprocessor, _preprocessorDependencies, queryCompilationContext);
    }
}