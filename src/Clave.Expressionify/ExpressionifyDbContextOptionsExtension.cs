using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Clave.Expressionify
{
    public class ExpressionifyDbContextOptionsExtension : IDbContextOptionsExtension
    {
        public void ApplyServices(IServiceCollection services)
        {
            AddDecorator<IQueryTranslationPreprocessorFactory, ExpressionifyQueryTranslationPreprocessorFactory>(services);
        }

        private static void AddDecorator<TService, TDecorator>(IServiceCollection services)
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
                if (descriptor.ImplementationInstance != null)
                    return descriptor.ImplementationInstance;

                if (descriptor.ImplementationFactory != null)
                    return descriptor.ImplementationFactory(provider);

                return ActivatorUtilities.GetServiceOrCreateInstance(provider, descriptor.ImplementationType!);
            }
        }

        public void Validate(IDbContextOptions options)
        {
            // No options to validate
        }

        public DbContextOptionsExtensionInfo Info => new ExtensionInfo(this);

        private class ExtensionInfo : DbContextOptionsExtensionInfo
        {
            public ExtensionInfo(IDbContextOptionsExtension extension) : base(extension)
            { }

            public override bool IsDatabaseProvider => false;
            public override string LogFragment => string.Empty;

            public override int GetServiceProviderHashCode()
            {
                // As long as there are no options, we can just return 0.
                // Once options get added, hash them.
                return 0;
            }

            public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
            {
                // As long as there are no options, we can just return true.
                // Once options get added, check if they're the same.
                return true;
            }

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
                debugInfo["Expressionify:Enabled"] = "true";
                // Add values of options here
            }
        }
    }
}