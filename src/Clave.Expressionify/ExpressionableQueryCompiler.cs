using System;
using System.Linq.Expressions;
using System.Threading;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Clave.Expressionify
{
    public class ExpressionableQueryCompiler : IQueryCompiler
    {
        private readonly IQueryCompiler _decoratedCompiler;

        public ExpressionableQueryCompiler(IQueryCompiler decoratedCompiler)
        {
            _decoratedCompiler = decoratedCompiler;
        }
        public Func<QueryContext, TResult> CreateCompiledAsyncQuery<TResult>(Expression query) => _decoratedCompiler.CreateCompiledAsyncQuery<TResult>(Visit(query));

        public Func<QueryContext, TResult> CreateCompiledQuery<TResult>(Expression query) => _decoratedCompiler.CreateCompiledQuery<TResult>(Visit(query));

        public TResult Execute<TResult>(Expression query) => _decoratedCompiler.Execute<TResult>(Visit(query));

        public TResult ExecuteAsync<TResult>(Expression query, CancellationToken cancellationToken) => _decoratedCompiler.ExecuteAsync<TResult>(Visit(query), cancellationToken);

        private static Expression Visit(Expression exp) => new ExpressionifyVisitor().Visit(exp);
    }
}
