using System.Collections.Generic;

namespace Clave.Expressionify.Generator
{
    internal static class Extensions
    {
        public static (T Head, IEnumerator<T> Tail)? HeadAndTail<T>(this IEnumerable<T> enumerable) => enumerable.GetEnumerator().HeadAndTail();

        public static (T Head, IEnumerator<T> Tail)? HeadAndTail<T>(this IEnumerator<T> enumerator)
        {
            if (!enumerator.MoveNext()) return null;
            return (enumerator.Current, enumerator);
        }
    }
}
