using System;
using System.Linq.Expressions;
using Expressionate;

namespace Tests
{
    public static class ExtensionMethods
    {
        [Expressionate]
        public static int ToInt(this string value) => Convert.ToInt32(value);
    }

    public static class ExtensionMethods_Expressionate
    {
        public static Expression<Func<string, int>> ToInt { get; } = (string value) => Convert.ToInt32(value);
    }
}