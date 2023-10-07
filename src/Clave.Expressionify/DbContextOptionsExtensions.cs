namespace Clave.Expressionify;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

public static class DbContextOptionsExtensions
{
    /// <summary>
    /// Use Expressionify within your queries.
    /// Transforms your expressions by replacing any [Expressionify] extension methods with the expressionised versions of those methods.
    /// </summary>
    public static DbContextOptionsBuilder<TContext> UseExpressionify<TContext>(this DbContextOptionsBuilder<TContext> optionsBuilder, Action<ExpressionifyDbContextOptionsBuilder>? expressionifyOptionsAction = null)
        where TContext : DbContext
    {
        return (DbContextOptionsBuilder<TContext>)UseExpressionify((DbContextOptionsBuilder)optionsBuilder, expressionifyOptionsAction);
    }

    /// <summary>
    /// Use Expressionify within your queries.
    /// Transforms your expressions by replacing any [Expressionify] extension methods with the expressionised versions of those methods.
    /// </summary>
    public static DbContextOptionsBuilder UseExpressionify(this DbContextOptionsBuilder optionsBuilder, Action<ExpressionifyDbContextOptionsBuilder>? expressionifyOptionsAction = null)
    {
        var extension = GetOrCreateExtension(optionsBuilder);
        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

        expressionifyOptionsAction?.Invoke(new ExpressionifyDbContextOptionsBuilder(optionsBuilder));

        return optionsBuilder;
    }

    private static ExpressionifyDbContextOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.Options.FindExtension<ExpressionifyDbContextOptionsExtension>()
        ?? new ExpressionifyDbContextOptionsExtension();
}