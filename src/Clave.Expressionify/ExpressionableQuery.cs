using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Clave.Expressionify
{
    public class ExpressionableQuery<T> : IQueryable<T>
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

        public Type ElementType => typeof(T);

        public Expression Expression { get; }

        public IQueryProvider Provider => _provider;
    }
}
