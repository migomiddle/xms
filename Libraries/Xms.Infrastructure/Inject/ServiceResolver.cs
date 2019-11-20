using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Xms.Infrastructure.Inject
{
    public class ServiceResolver : IServiceResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T Get<T>()
        {
            try
            {
                return _serviceProvider.GetRequiredService<T>();
            }
            catch
            {
                return default(T);
            }
        }

        public object Get(Type serviceType)
        {
            try
            {
                return _serviceProvider.GetRequiredService(serviceType);
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<T> GetAll<T>()
        {
            return _serviceProvider.GetServices<T>();
        }

        public IEnumerable<object> GetAll(Type serviceType)
        {
            return _serviceProvider.GetServices(serviceType);
        }

        public object ResolveUnregistered(Type type)
        {
            Exception innerException = null;
            foreach (var constructor in type.GetConstructors())
            {
                try
                {
                    //try to resolve constructor parameters
                    var parameters = constructor.GetParameters().Select(parameter =>
                    {
                        var service = _serviceProvider.GetService(parameter.ParameterType);
                        if (service == null)
                        {
                            throw new XmsException("Unknown dependency");
                        }

                        return service;
                    });

                    //all is ok, so create instance
                    return Activator.CreateInstance(type, parameters.ToArray());
                }
                catch (Exception ex)
                {
                    innerException = ex;
                }
            }

            throw new XmsException("No constructor was found that had all the dependencies satisfied.", innerException);
        }
    }
}