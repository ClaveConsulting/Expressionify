namespace Clave.Expressionify.Tests.Samples;

public partial class GenericClass<T>
{
    [Expressionify]
    public static int Foo(string x) => 8;
}
