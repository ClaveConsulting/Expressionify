using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Clave.Expressionify
{
    public static class DbContextOptionsExtensions
    {
        /// <summary>
        /// Use Expressionify within your queries.
        /// Transforms your expressions by replacing any [Expressionify] extension methods with the expressionised versions of those methods.
        /// </summary>
        public static DbContextOptionsBuilder<TContext> UseExpressionify<TContext>(this DbContextOptionsBuilder<TContext> optionsBuilder)
            where TContext : DbContext
        {
            return (DbContextOptionsBuilder<TContext>)UseExpressionify((DbContextOptionsBuilder)optionsBuilder);
        }

        /// <summary>
        /// Use Expressionify within your queries.
        /// Transforms your expressions by replacing any [Expressionify] extension methods with the expressionised versions of those methods.
        /// </summary>
        public static DbContextOptionsBuilder UseExpressionify(this DbContextOptionsBuilder optionsBuilder)
        {
            var extension = GetOrCreateExtension(optionsBuilder);
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            return optionsBuilder;
        }

        private static ExpressionifyDbContextOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder) 
            => optionsBuilder.Options.FindExtension<ExpressionifyDbContextOptionsExtension>()
            ?? new ExpressionifyDbContextOptionsExtension();
    }
}
