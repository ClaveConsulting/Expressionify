namespace Clave.Expressionify.Tests.DbContextExtensions
{
    public static partial class TestEntityExtensions
    {
        [Expressionify]
        public static string GetName(this TestEntity testEntity, string prefix) => prefix + " " + testEntity.Name;

        [Expressionify]
        public static bool NameEquals(this TestEntity testEntity, string name) => testEntity.Name == name;

        [Expressionify]
        public static bool IsJohnDoe(this TestEntity testEntity) => testEntity.Name == "John Doe";

        [Expressionify]
        public static bool IsSomething(this TestEntity testEntity) => testEntity.Name == Name;

        [Expressionify]
        public static bool IsRecent(this TestEntity testEntity) => testEntity.Created > TestTimeProvider.UtcNow.AddDays(-1);

        [Expressionify]
        public static TestView ToTestView(this TestEntity testEntity, TestAddress? address)
            => new() { Name = testEntity.Name, Street = address == null ? null : address.Street };

        public static string Name => "Something";
    }
}
