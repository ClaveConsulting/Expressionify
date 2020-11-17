using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Clave.Expressionify
{
    public class ExpressionableQueryProvider : IAsyncQueryProvider
    {
        private readonly IQueryProvider _underlyingQueryProvider;

        public ExpressionableQueryProvider(IQueryProvider underlyingQueryProvider)
        {
            _underlyingQueryProvider = underlyingQueryProvider;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new ExpressionableQuery<TElement>(this, expression);

        public IQueryable CreateQuery(Expression expression)
        {
            try
            {
                var type = expression.Type.GetElementType();

                if(type == null) throw new Exception($"Expression type is strange {expression.Type.FullName}");

                return typeof(ExpressionableQuery<>)
                    .MakeGenericType(type)
                    .CreateInstance<IQueryable>(this, expression);
            }
            catch (System.Reflection.TargetInvocationException e)
            {
                throw e.InnerException ?? e;
            }
        }

        internal IEnumerable<T> ExecuteQuery<T>(Expression expression) => _underlyingQueryProvider.CreateQuery<T>(Visit(expression)).AsEnumerable();

        internal IAsyncEnumerable<T> ExecuteQueryAsync<T>(Expression expression) => _underlyingQueryProvider.CreateQuery<T>(Visit(expression)).AsAsyncEnumerable();

        public TResult Execute<TResult>(Expression expression) => _underlyingQueryProvider.Execute<TResult>(Visit(expression));

        public object? Execute(Expression expression) => _underlyingQueryProvider.Execute(Visit(expression));

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            if (_underlyingQueryProvider is IAsyncQueryProvider provider)
            {
                return provider.ExecuteAsync<TResult>(Visit(expression), cancellationToken);
            }

            throw new Exception("This shouldn't happen");
        }

        private static Expression Visit(Expression exp) => new ExpressionifyVisitor().Visit(exp);
    }
}
