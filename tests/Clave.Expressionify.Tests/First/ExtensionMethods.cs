using System;
using System.Linq.Expressions;

namespace Clave.Expressionify.Tests.First
{
    public static partial class ExtensionMethods
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
}