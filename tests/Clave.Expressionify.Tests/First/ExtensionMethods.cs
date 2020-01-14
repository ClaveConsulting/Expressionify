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

        [Expressionify]
        public static double Squared(this double a) => a * a;
    }

    public static class ExtensionMethods_Expressionify
    {
        public static Expression<Func<string, int>> ToInt_0 { get; } = (string value) => Convert.ToInt32(value);

        public static Expression<Func<string, double>> ToDouble_0 { get; } = (string value) => Convert.ToDouble(value);

        public static Expression<Func<string, string, int>> Pluss_0 { get; } = (string a, string b) => a.ToInt() + b.ToInt();

        public static Expression<Func<int, int>> Squared_0 { get; } = (int a) => a * a;

        public static Expression<Func<double, double>> Squared_1 { get; } = (double a) => a * a;
    }
}