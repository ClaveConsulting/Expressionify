using System;
using System.Linq.Expressions;

namespace Clave.Expressionify.Tests.Second
{
    public static class ExtensionMethods
    {
        [Expressionify]
        public static int ToInt(this string value, bool extra) => Convert.ToInt32(value);

        [Expressionify]
        public static double ToDouble(this string value) => Convert.ToDouble(value);

        [Expressionify]
        public static int Pluss(this string a, string b) => a.ToInt(false) + b.ToInt(false);

        [Expressionify]
        public static int Squared(this int a) => a * a;
    }

    public static class ExtensionMethods_Expressionify
    {
        public static Expression<Func<string, bool, int>> ToInt { get; } = (string value, bool extra) => Convert.ToInt32(value);

        public static Expression<Func<string, double>> ToDouble { get; } = (string value) => Convert.ToDouble(value);

        public static Expression<Func<string, string, int>> Pluss { get; } = (string a, string b) => a.ToInt(false) + b.ToInt(false);

        public static Expression<Func<int, int>> Squared { get; } = (int a) => a * a;
    }
}