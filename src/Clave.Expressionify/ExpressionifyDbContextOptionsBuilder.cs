namespace Clave.Expressionify;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

public class ExpressionifyDbContextOptionsBuilder
{
    private readonly DbContextOptionsBuilder _optionsBuilder;

    internal ExpressionifyDbContextOptionsBuilder(DbContextOptionsBuilder optionsBuilder) => _optionsBuilder = optionsBuilder;

    public ExpressionifyDbContextOptionsBuilder WithEvaluationMode(ExpressionEvaluationMode mode) => WithOption(e => e.WithEvaluationMode(mode));

    /// <summary>
    ///     Sets an option by cloning the extension used to store the settings. This ensures the builder
    ///     does not modify options that are already in use elsewhere.
    /// </summary>
    /// <param name="setAction">An action to set the option.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    private ExpressionifyDbContextOptionsBuilder WithOption(Func<ExpressionifyDbContextOptionsExtension, ExpressionifyDbContextOptionsExtension> setAction)
    {
        var extension = setAction(_optionsBuilder.Options.FindExtension<ExpressionifyDbContextOptionsExtension>()!);
        ((IDbContextOptionsBuilderInfrastructure)_optionsBuilder).AddOrUpdateExtension(extension);

        return this;
    }
}