using System;

namespace Clave.Expressionify.Tests.DbContextExtensions
{
    public class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime Created { get; set; }
    }

    public class TestAddress
    {
        public string? City { get; set; }
        public string? Street { get; set; }
    }

    public class TestView
    {
        public string? Name { get; set; }
        public string? Street { get; set; }
    }

    public static class TestTimeProvider
    {
        public static DateTime UtcNow => new(2022, 3, 4, 5, 6, 7);
    }
}
