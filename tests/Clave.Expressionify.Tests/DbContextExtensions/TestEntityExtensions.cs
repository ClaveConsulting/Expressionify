using System;

namespace Clave.Expressionify.Tests.DbContextExtensions
{
    public static partial class TestEntityExtensions
    {
        [Expressionify]
        public static string GetName(this TestEntity testEntity, string prefix) => prefix + " " + testEntity.Name;

        [Expressionify]
        public static bool IsSomething(this TestEntity testEntity) => testEntity.Name == Name;

        public static string Name => "Something";
    }
}
