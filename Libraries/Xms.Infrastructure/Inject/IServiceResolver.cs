using System;
using System.Collections.Generic;

namespace Xms.Infrastructure.Inject
{
    public interface IServiceResolver
    {
        object Get(Type serviceType);

        T Get<T>();

        IEnumerable<object> GetAll(Type serviceType);

        IEnumerable<T> GetAll<T>();

        object ResolveUnregistered(Type type);
    }
}