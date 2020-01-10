using System;
using System.Linq.Expressions;

namespace Clave.Expressionify.Tests.First
{
    public static class ExtensionMethods
    {
        [Expressionify]
        public static int ToInt(this string value) => Convert.ToInt32(value);

        [Expressionify]
        public static double ToDouble(this string value) => Convert.ToDouble(value);

        [Expressionify]
        public static int Pluss(this string a, string b) => a.ToInt() + b.ToInt();

        [Expressionify]
        public static int Squared(this int a) => a * a;
    }

    public static class ExtensionMethods_Expressionify
    {
        public static Expression<Func<string, int>> ToInt { get; } = (string value) => Convert.ToInt32(value);

        public static Expression<Func<string, double>> ToDouble { get; } = (string value) => Convert.ToDouble(value);

        public static Expression<Func<string, string, int>> Pluss { get; } = (string a, string b) => a.ToInt() + b.ToInt();

        public static Expression<Func<int, int>> Squared { get; } = (int a) => a * a;
    }
}