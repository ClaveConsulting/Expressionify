namespace Clave.Expressionify.Tests.DbContextExtensions;
using Microsoft.EntityFrameworkCore;

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions options) : base(options)
    { }

    public DbSet<TestEntity> TestEntities { get; set; } = null!;
}
