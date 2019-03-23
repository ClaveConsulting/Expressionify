using System.Linq;

namespace Expressionate
{
    public static class ExpressionateExtension
    {
        /// <summary>
        /// Transforms your expression by replacing any [ExpandQuery] or [ReplaceWithExpression(MethodName = ...)]
        /// extension methods with the expressionised versions of those methods. This allows the extensions to be used
        /// by another visitor such as EntityFramework. Should be used at the start of a query.
        /// </summary>
        /// <typeparam name="T">the type of the queryable</typeparam>
        /// <param name="source">The input queryable</param>
        /// <returns>A queryable which has any of the tagged extension methods replaced.</returns>
        public static IQueryable<T> Expressionate<T>(this IQueryable<T> source)
        {
            if (source is ExpressionableQuery<T> result)
            {
                return result;
            }

            return new ExpressionableQueryProvider(source.Provider).CreateQuery<T>(source.Expression);
        }
    }
}
