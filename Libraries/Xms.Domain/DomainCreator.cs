using System;
using System.Collections.Concurrent;

namespace Xms.Domain
{
    /// <summary>
    /// domain creator
    /// </summary>
    public static class DomainCreator
    {
        public static T Get<T>() where T : new()
        {
            return new T();
            //var t = DomainMaps.Get<T>();
            //return t != null ? (T)Activator.CreateInstance(t) : default(T);
        }
    }

    public class DomainMaps
    {
        private static readonly ConcurrentDictionary<Type, Type> _domainTypes = new ConcurrentDictionary<Type, Type>();

        public static void Add<TInterface, TImplement>()
        {
            _domainTypes.TryAdd(typeof(TInterface), typeof(TImplement));
        }

        public static void Add(Type interfaceType, Type implementType)
        {
            _domainTypes.TryAdd(interfaceType, implementType);
        }

        public static Type Get<TInterface>()
        {
            _domainTypes.TryGetValue(typeof(TInterface), out Type impl);
            return impl;
        }

        public static Type Get(Type interfaceType)
        {
            _domainTypes.TryGetValue(interfaceType, out Type impl);
            return impl;
        }
    }
}