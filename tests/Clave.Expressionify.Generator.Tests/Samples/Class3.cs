namespace Clave.Expressionify.Generator.Tests.Samples
{
    public static partial class Class3
    {
        [Expressionify]
        public static int Foo(int x) => 8;

        [Expressionify]
        public static int Foo(string x) => 0;
    }
}
