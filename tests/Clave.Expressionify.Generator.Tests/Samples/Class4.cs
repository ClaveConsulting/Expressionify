using System.Collections.Generic;
using System.Linq;
using Clave.Expressionify.Generator.Tests;

namespace Clave.Expressionify.Generator.Tests.Samples
{
    public static partial class Class4
    {
        [Expressionify]
        public static int Foo(string x) => 8;

        [Expressionify]
        public static int Something(IEnumerable<string> x) => x.Select(Foo).Sum();
    }
}
