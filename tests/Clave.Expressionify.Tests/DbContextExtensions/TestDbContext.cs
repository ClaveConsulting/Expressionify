using Microsoft.EntityFrameworkCore;

namespace Clave.Expressionify.Tests.DbContextExtensions
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions options) : base(options)
        { }

        public DbSet<TestEntity> TestEntities { get; set; } = null!;
        public DbSet<TestEntity2> TestEntities2 { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestEntity2>().HasQueryFilter(x => x.IsFoo());
        }
    }
}
