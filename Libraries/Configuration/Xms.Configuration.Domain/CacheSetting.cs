using System.Collections.Generic;

namespace Xms.Configuration.Domain
{
    /// <summary>
    /// 缓存服务配置
    /// </summary>
    public sealed class CacheSetting
    {
        public const string CACHE_KEY = "$CacheServiceSetting$";
        public bool Enabled { get; set; }
        public List<CacheServiceProvider> Providers { get; set; }
    }

    public sealed class CacheServiceProvider
    {
        public string Type { get; set; }
        public bool Enabled { get; set; }
        public int Priority { get; set; }
        public int TimeOut { get; set; }
        public string Host { get; set; }
    }
}