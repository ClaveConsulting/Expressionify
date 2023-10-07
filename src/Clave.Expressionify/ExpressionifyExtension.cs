namespace Clave.Expressionify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class ExpressionifyExtension
{
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

    internal static bool MatchesTypeOf(this MethodInfo property, MethodInfo method)
    {
        var methodTypes = method.GetParameters().Select(p => p.ParameterType).Concat(new[] { method.ReturnType });
        var propertyTypes = property.ReturnType.GetGenericArguments()[0].GetGenericArguments();

        return methodTypes.SequenceEqual(propertyTypes);
    }

    internal static T CreateInstance<T>(this Type type, params object?[]? args)
    {
        if (Activator.CreateInstance(type, args) is T result)
        {
            return result;
        }
        else
        {
            throw new Exception($"Type {type.FullName} is not of type {typeof(T).FullName}");
        }
    }
}
