namespace Clave.Expressionify;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

public class ExpressionableQuery<T> : IQueryable<T>, IOrderedQueryable<T>, IAsyncEnumerable<T>
{
    private readonly ExpressionableQueryProvider _provider;

    public ExpressionableQuery(ExpressionableQueryProvider provider, Expression expression)
    {
        _provider = provider;
        Expression = expression;
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return _provider.ExecuteQuery<T>(Expression).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _provider.ExecuteQuery<T>(Expression).GetEnumerator();
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return _provider.ExecuteQueryAsync<T>(Expression).GetAsyncEnumerator(cancellationToken);
    }

    public Type ElementType => typeof(T);

    public Expression Expression { get; }

    public IQueryProvider Provider => _provider;
}
