using Microsoft.EntityFrameworkCore;

namespace Clave.Expressionify.Tests.DbContextExtensions
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions options) : base(options)
        { }

        public DbSet<TestEntity> TestEntities { get; set; } = null!;
    }
}
