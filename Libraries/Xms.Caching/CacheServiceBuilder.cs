using System;
using Xms.Infrastructure.Utility;

namespace Xms.Caching
{
    public static class CacheServiceBuilder
    {
        /// <summary>
        /// 获取缓存实例
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        //根据平台配置参数获取缓存实例
        public static ICacheService Build()
        {
            return new MemoryCacheService();
            //var cacheSetting = ServiceLocator.Get<ISettingFinder>().Get<Core.Domain.Configuration.CacheSetting>();
            //if (cacheSetting.Enabled)
            //{
            //    if (cacheSetting.Providers.NotEmpty())
            //    {
            //        cacheSetting.Providers.RemoveAll(n => n.Enabled == false);
            //        if (cacheSetting.Providers.NotEmpty())
            //        {
            //            var provider = cacheSetting.Providers.OrderBy(n => n.Priority).First();
            //            CacheOptions options = new CacheOptions()
            //            {
            //                Host = provider.Host
            //                ,
            //                TimeOut = provider.TimeOut
            //            };
            //            var instance = (ICacheService)Activator.CreateInstance(Type.GetType(provider.Type, false, true), options);
            //            return instance;
            //        }
            //    }
            //    return new MemoryCacheService();
            //}
            return null;
        }

        /// <summary>
        /// 缓存预热
        /// </summary>
        public static void PreLoad()
        {
            //实现了ICachePreload接口的所有类
            var types = AssemblyHelper.GetClassOfType(typeof(ICachePreload), "xms.*.dll");

            foreach (var t in types)
            {
                var instance = (ICachePreload)Activator.CreateInstance(t);
                instance.Load();
            }
        }
    }

    public enum CachingProvider
    {
        Memory
        , ThirdParty
    }
}