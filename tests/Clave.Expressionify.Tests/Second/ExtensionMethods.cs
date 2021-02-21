using System;

namespace Clave.Expressionify.Tests.Second
{
    public static partial class ExtensionMethods
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
}