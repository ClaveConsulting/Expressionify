namespace Clave.Expressionify.Tests.DbContextExtensions
{
    public static partial class TestEntityExtensions
    {
        [Expressionify]
        public static string GetName(this TestEntity testEntity, string prefix) => prefix + " " + testEntity.Name;
    }
}
