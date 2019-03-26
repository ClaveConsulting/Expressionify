using System;
using System.Linq.Expressions;
using Clave.Expressionify;

namespace Tests
{
    public static class ExtensionMethods
    {
        [Expressionify]
        public static int ToInt(this string value) => Convert.ToInt32(value);

        [Expressionify]
        public static double ToDouble(this string value) => Convert.ToDouble(value);
    }

    public static class ExtensionMethods_Expressionify
    {
        public static Expression<Func<string, int>> ToInt { get; } = (string value) => Convert.ToInt32(value);

        public static Expression<Func<string, double>> ToDouble { get; } = (string value) => Convert.ToDouble(value);
    }
}