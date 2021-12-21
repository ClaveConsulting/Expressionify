namespace Clave.Expressionify.Tests.Samples
{
    public partial record Record1(string Name)
    {
        [Expressionify]
        public static Record1 Create(string name) => new Record1(name);
    }
}
