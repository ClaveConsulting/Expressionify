using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Clave.Expressionify
{
    public static class ExpressionifyExtension
    {
        private const string ErrorMessage = @"You should not call Expressionify() before Include() and ThenInclude().
Change the order so that Expressionify() is called after all calls to Include() and ThenInclude().";

        /// <summary>
        /// Transforms your expression by replacing any [Expressionify]
        /// extension methods with the expressionised versions of those methods. This allows the extensions to be used
        /// by another visitor such as EntityFramework. Should be used at the start of a query.
        /// </summary>
        /// <typeparam name="T">the type of the queryable</typeparam>
        /// <param name="source">The input queryable</param>
        /// <returns>A queryable which has any of the tagged extension methods replaced.</returns>
        public static IQueryable<T> Expressionify<T>(this IQueryable<T> source)
        {
            if (source is ExpressionableQuery<T> result)
            {
                return result;
            }

            return new ExpressionableQueryProvider(source.Provider).CreateQuery<T>(source.Expression);
        }

        /// <summary>
        /// This method throws an exception because you have called it after calling Expressionify()
        /// To fix this, change the order of your method chain so that Expressionify() is called after
        /// Include() and ThenInclude()
        /// </summary>
        public static IIncludableQueryable<TEntity, TProperty> Include<TEntity, TProperty>(this ExpressionableQuery<TEntity> source, Expression<Func<TEntity, TProperty>> navigationPropertyPath) where TEntity : class
        {
            throw new Exception(ErrorMessage);
        }

        /// <summary>
        /// This method throws an exception because you have called it after calling Expressionify()
        /// To fix this, change the order of your method chain so that Expressionify() is called after
        /// Include() and ThenInclude()
        /// </summary>
        public static IIncludableQueryable<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(this ExpressionableQuery<TEntity> source, Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath) where TEntity : class
        {
            throw new Exception(ErrorMessage);
        }
    }
}
