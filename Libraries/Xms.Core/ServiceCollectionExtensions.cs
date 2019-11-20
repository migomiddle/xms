using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xms.Infrastructure.Inject;
using Xms.Infrastructure.Utility;

namespace Xms.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterAll(this IServiceCollection services, IConfiguration configuration)
        {
            var types = AssemblyHelper.GetClassOfType(typeof(IServiceRegistrar), "Xms.*.dll");
            foreach (var t in types)
            {
                var instance = (IServiceRegistrar)Activator.CreateInstance(t);
                instance.Add(services, configuration);
            }
            return services;
        }

        public static IServiceCollection RegisterScope<TService>(this IServiceCollection services)
        {
            var serviceType = typeof(TService);
            return Register(services, serviceType, ServiceLifetime.Scoped);
        }

        public static IServiceCollection RegisterScope(this IServiceCollection services, Type serviceType)
        {
            return Register(services, serviceType, ServiceLifetime.Scoped);
        }

        public static IServiceCollection Register(this IServiceCollection services, Type serviceType, ServiceLifetime serviceLifetime)
        {
            var implementTypes = AssemblyHelper.GetClassOfType(serviceType, "Xms.*.dll");
            if (serviceType.IsGenericType)
            {
                foreach (var impl in implementTypes)
                {
                    var it = impl.FindInterfaces((type, criteria) =>
                    {
                        var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                        return isMatch;
                    }, serviceType);
                    foreach (var i in it)
                    {
                        services.Add(new ServiceDescriptor(i, impl, serviceLifetime));
                    }
                }
            }
            else
            {
                foreach (var impl in implementTypes)
                {
                    services.Add(new ServiceDescriptor(serviceType, impl, serviceLifetime));
                }
            }
            return services;
        }
    }
}