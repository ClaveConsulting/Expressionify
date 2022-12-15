namespace Clave.Expressionify.Tests.DbContextExtensions;
using System;

public class TestEntity
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required DateTime Created { get; set; }
}

public class TestAddress
{
    public required string City { get; set; }
    public required string Street { get; set; }
}

public class TestView
{
    public required string Name { get; set; }
    public required string Street { get; set; }
}

public static class TestTimeProvider
{
    public static DateTime UtcNow => new(2022, 3, 4, 5, 6, 7);
}
