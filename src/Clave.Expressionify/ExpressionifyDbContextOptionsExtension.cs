using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Clave.Expressionify
{
    public class ExpressionifyDbContextOptionsExtension : IDbContextOptionsExtension
    {
        public ExpressionifyDbContextOptionsExtension()
        { }

        public ExpressionifyDbContextOptionsExtension(ExpressionifyDbContextOptionsExtension copyFrom)
        {
            EvaluationMode = copyFrom.EvaluationMode;
        }

        public DbContextOptionsExtensionInfo Info => new ExtensionInfo(this);
        public ExpressionEvaluationMode EvaluationMode { get; private set; } = ExpressionEvaluationMode.LimitedCompatibilityButCached;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "<Pending>")]
        public void ApplyServices(IServiceCollection services)
        {
            if (EvaluationMode == ExpressionEvaluationMode.FullCompatibilityButSlow)
                AddDecorator<IQueryCompiler, ExpressionableQueryCompiler>(services);

            else if (EvaluationMode == ExpressionEvaluationMode.LimitedCompatibilityButCached)
                AddDecorator<IQueryTranslationPreprocessorFactory, ExpressionifyQueryTranslationPreprocessorFactory>(services);

            else
                throw new NotSupportedException($"Unsupported {nameof(EvaluationMode)}");
        }

        private static void AddDecorator<TService, TDecorator>(IServiceCollection services)
            where TDecorator : TService
        {
            var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(TService));
            if (descriptor == null || descriptor.ImplementationType == null && descriptor.ImplementationFactory == null && descriptor.ImplementationInstance == null)
                throw new InvalidOperationException($"No {typeof(TService).Name} is configured yet. Please configure a database provider first.");

            // Replace service with decorator. Factory creates the decorator with an instance of the decorated type, based on the original registration.
            services.Replace(ServiceDescriptor.Describe(
                descriptor.ServiceType,
                provider => ActivatorUtilities.CreateInstance(provider, typeof(TDecorator), GetInstance(provider, descriptor)),
                descriptor.Lifetime));

            static object GetInstance(IServiceProvider provider, ServiceDescriptor descriptor)
            {
                return descriptor.ImplementationInstance
                    ?? descriptor.ImplementationFactory?.Invoke(provider)
                    ?? ActivatorUtilities.GetServiceOrCreateInstance(provider, descriptor.ImplementationType!);
            }
        }

        public void Validate(IDbContextOptions options)
        {
            // No options to validate
        }

        public ExpressionifyDbContextOptionsExtension WithEvaluationMode(ExpressionEvaluationMode evaluationMode)
        {
            var clone = Clone();
            clone.EvaluationMode = evaluationMode;
            return clone;
        }

        private ExpressionifyDbContextOptionsExtension Clone() => new(this);

        private class ExtensionInfo : DbContextOptionsExtensionInfo
        {
            private readonly ExpressionifyDbContextOptionsExtension _extension;

            public ExtensionInfo(ExpressionifyDbContextOptionsExtension extension) : base(extension)
            {
                _extension = extension;
            }

            public override bool IsDatabaseProvider => false;
            public override string LogFragment => string.Empty;

            public override int GetServiceProviderHashCode()
            {
                // Hash all options here
                return _extension.EvaluationMode.GetHashCode();
            }

            public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
            {
                // Check if all options are the same
                return other is ExtensionInfo otherInfo
                    && otherInfo._extension.EvaluationMode == _extension.EvaluationMode;
            }

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
                debugInfo["Expressionify:EvaluationMode"] = _extension.EvaluationMode.ToString();
                // Add values of options here
            }
        }
    }
}