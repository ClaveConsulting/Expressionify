using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Clave.Expressionify
{
    public class ExpressionableQuery<T> : IQueryable<T>, IOrderedQueryable<T>, IAsyncEnumerable<T>
    {
        private readonly ExpressionableQueryProvider _provider;

        public ExpressionableQuery(ExpressionableQueryProvider provider, Expression expression)
        {
            _provider = provider;
            Expression = expression;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _provider.ExecuteQuery<T>(Expression).GetEnumerator();
        }

        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetEnumerator()
        {
            return _provider.ExecuteAsync<T>(Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Type ElementType => typeof(T);

        public Expression Expression { get; }

        public IQueryProvider Provider => _provider;
    }
}
