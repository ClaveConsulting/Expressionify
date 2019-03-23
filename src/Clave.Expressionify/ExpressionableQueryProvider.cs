using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Clave.Expressionify
{
    internal class ExpressionableQueryProvider : IAsyncQueryProvider
    {
        private readonly IQueryProvider _underlyingQueryProvider;

        internal ExpressionableQueryProvider(IQueryProvider underlyingQueryProvider)
        {
            _underlyingQueryProvider = underlyingQueryProvider;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new ExpressionableQuery<TElement>(this, expression);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            try
            {
                var elementType = expression.Type.GetElementType();
                var type = typeof(ExpressionableQuery<>).MakeGenericType(elementType);
                var args = new object[] { this, expression };
                return (IQueryable)Activator.CreateInstance(type, args);
            }
            catch (System.Reflection.TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }

        internal IEnumerable<T> ExecuteQuery<T>(Expression expression)
        {
            return _underlyingQueryProvider.CreateQuery<T>(Visit(expression)).AsEnumerable();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _underlyingQueryProvider.Execute<TResult>(Visit(expression));
        }

        public object Execute(Expression expression)
        {
            return _underlyingQueryProvider.Execute(Visit(expression));
        }

        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            return ((IAsyncQueryProvider)_underlyingQueryProvider).ExecuteAsync<TResult>(Visit(expression));
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return ((IAsyncQueryProvider)_underlyingQueryProvider).ExecuteAsync<TResult>(Visit(expression), cancellationToken);
        }

        private Expression Visit(Expression exp)
        {
            return new ExpressionifyVisitor(_underlyingQueryProvider).Visit(exp);
        }
    }
}
