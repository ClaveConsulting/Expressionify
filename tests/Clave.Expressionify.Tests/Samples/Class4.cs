namespace Clave.Expressionify.Tests.Samples;
using System.Collections.Generic;
using System.Linq;

public static partial class Class4
{
    [Expressionify]
    private static int Foo(string x) => 8;

    [Expressionify]
    public static int Something(IEnumerable<string> x) => x.Select(Foo).Sum();

    public static partial class NestedClass1
    {
        [Expressionify]
        public static string Bar(int x) => $"={8+x}";
    }
}
