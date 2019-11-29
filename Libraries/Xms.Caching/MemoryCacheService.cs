using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using Xms.Infrastructure.Threading;
using Xms.Infrastructure.Utility;

namespace Xms.Caching
{
    /// <summary>
    /// 内存缓存服务
    /// </summary>
    public class MemoryCacheService : ICacheService
    {
        private const string REGION_NAME = "__XMS__";
        private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();
        private readonly IMemoryCache _cache;

        public MemoryCacheService()
        {
            _cache = new MemoryCache(Options.Create(new MemoryCacheOptions() { }));
        }

        public MemoryCacheService(CacheOptions options)
        {
            _cache = new MemoryCache(Options.Create(new MemoryCacheOptions() { }));
        }

        public string FAKE_NULL => "_NULL";

        public List<string> Keys
        {
            get
            {
                const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
                var entries = _cache.GetType().GetField("_entries", flags).GetValue(_cache);
                var keys = new List<string>();
                if (!(entries is IDictionary cacheItems))
                {
                    return keys;
                }

                foreach (DictionaryEntry cacheItem in cacheItems)
                {
                    keys.Add(cacheItem.Key.ToString());
                }
                return keys;
            }
        }

        public bool IsDistributedCache
        {
            get { return false; }
        }

        public void Clear()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public bool Contains(string key)
        {
            return _cache.TryGetValue(BuildKey(key), out _);
        }

        public bool ContainsListItem(string key, string itemKey)
        {
            var items = _cache.Get<Dictionary<string, object>>(BuildKey(key));
            if (items.NotEmpty())
            {
                return items.ContainsKey(BuildKey(itemKey));
            }
            return false;
        }

        public object Get(string key)
        {
            return _cache.Get(BuildKey(key));
        }

        public T Get<T>(string key) where T : class
        {
            return _cache.Get<T>(BuildKey(key));
        }

        public List<T> GetListAll<T>(string key) where T : class
        {
            var items = _cache.Get<Dictionary<string, T>>(BuildKey(key));
            List<T> result = null;
            if (items.NotEmpty())
            {
                result = new List<T>();
                foreach (var item in items)
                {
                    T v = item.Key.Contains(FAKE_NULL) ? null : item.Value;
                    result.Add(v);
                }
            }
            return result;
        }

        public T GetListItem<T>(string key, string itemKeyValue) where T : class
        {
            T result = null;
            var items = _cache.Get<Dictionary<string, T>>(BuildKey(key));
            if (items.NotEmpty())
            {
                if (items.TryGetValue(itemKeyValue, out result))
                {
                    return result;
                }
            }
            return result;
        }

        public List<T> GetListItems<T>(string key, params string[] itemKeyValue) where T : class
        {
            var items = _cache.Get<Dictionary<string, T>>(BuildKey(key));
            List<T> result = null;
            if (items.NotEmpty())
            {
                result = new List<T>();
                foreach (var itemk in itemKeyValue)
                {
                    if (items.TryGetValue(itemk, out T v))
                    {
                        result.Add(v);
                    }
                }
            }
            return result;
        }

        public List<T> GetListItemsByPattern<T>(string key, params string[] keyPattern) where T : class
        {
            var items = _cache.Get<Dictionary<string, T>>(BuildKey(key));
            List<T> result = null;
            if (items.NotEmpty())
            {
                result = new List<T>();
                foreach (var kp in keyPattern)
                {
                    var regex = new Regex(BuildPattern(kp), RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    var matches = items.Where(x => regex.IsMatch(x.Key) && x.Value != null).Select(x => x.Value);
                    result.AddRange(matches);
                }
            }
            return result;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void RemoveByPattern(string keyPattern)
        {
            var regex = new Regex(BuildPattern(keyPattern), RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matchesKeys = Keys.Where(key => regex.IsMatch(key)).ToList();
            foreach (var key in matchesKeys)
            {
                _cache.Remove(key);
            }
        }

        public void RemoveListItemByPattern<T>(string key, params string[] itemKeyPattern) where T : class
        {
            var items = _cache.Get<Dictionary<string, T>>(BuildKey(key));
            if (items.NotEmpty())
            {
                using (_rwLock.GetWriteLock())
                {
                    if (items.NotEmpty())
                    {
                        foreach (var itemk in itemKeyPattern)
                        {
                            var regex = new Regex(BuildPattern(itemk), RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
                            var matchesKeys = items.Where(n => regex.IsMatch(n.Key));
                            if (matchesKeys != null)
                            {
                                foreach (var item in matchesKeys)
                                {
                                    items.Remove(item.Key);
                                }
                            }
                        }
                        _cache.Set(BuildKey(key), items);
                        items = null;
                    }
                }
            }
        }

        public void RemoveListItem<T>(string key, params string[] itemKeyValue) where T : class
        {
            var items = _cache.Get<Dictionary<string, T>>(BuildKey(key));
            if (items.NotEmpty())
            {
                using (_rwLock.GetWriteLock())
                {
                    if (items.NotEmpty())
                    {
                        foreach (var itemk in itemKeyValue)
                        {
                            items.Remove(itemk);
                        }
                        _cache.Set(BuildKey(key), items);
                        items = null;
                    }
                }
            }
        }

        public void Set(string key, object value, int? cacheTime = null)
        {
            _cache.Set(BuildKey(key), value, GetOptions(cacheTime));
        }

        public void SetList<T>(string key, string itemKey, List<T> value, int? cacheTime = null) where T : class
        {
            //先把原有的数据加载出来，然后插入新记录
            Dictionary<string, T> items = _cache.Get<Dictionary<string, T>>(BuildKey(key));
            if (items == null)
            {
                items = new Dictionary<string, T>(StringComparer.InvariantCultureIgnoreCase);
            }
            var t = typeof(T);
            using (_rwLock.GetWriteLock())
            {
                if (value != null)
                {
                    foreach (var item in value)
                    {
                        string itemKeyValue = t.GetProperty(itemKey).PropertyType.GetFieldValue(item).ToString();
                        items[itemKeyValue] = item;
                    }
                    //重新放回缓存
                    _cache.Set(BuildKey(key), items, GetOptions(cacheTime));
                    value = null;
                }
            }
        }

        public void SetList<T>(string key, Dictionary<string, T> keyValues, int? cacheTime = null) where T : class
        {
            //先把原有的数据加载出来，然后插入新记录
            var items = _cache.Get<Dictionary<string, T>>(BuildKey(key));
            if (items == null)
            {
                items = new Dictionary<string, T>(keyValues, StringComparer.InvariantCultureIgnoreCase);
            }
            using (_rwLock.GetWriteLock())
            {
                if (keyValues != null)
                {
                    foreach (var item in keyValues)
                    {
                        items[item.Key] = item.Value;
                    }
                    //重新放回缓存
                    _cache.Set(BuildKey(key), items, GetOptions(cacheTime));
                    keyValues = null;
                }
            }
        }

        public void SetListItem<T>(string key, string itemKey, T value, int? cacheTime = null) where T : class
        {
            var items = _cache.Get<Dictionary<string, T>>(BuildKey(key));
            if (items == null)
            {
                items = new Dictionary<string, T>(StringComparer.InvariantCultureIgnoreCase);
            }
            using (_rwLock.GetWriteLock())
            {
                items[itemKey] = value;
                //重新放回缓存
                _cache.Set(BuildKey(key), items, GetOptions(cacheTime));
            }
        }

        private string BuildKey(string key)
        {
            return key.HasValue() ? REGION_NAME + key : null;
        }

        private string BuildPattern(string pattern)
        {
            var p = pattern;
            p = p.Replace("/*", "/.*").Replace("*/", ".*/");
            return p;
        }

        private MemoryCacheEntryOptions GetOptions(int? duration = 30)
        {
            var options = new MemoryCacheEntryOptions()
            .AddExpirationToken(new CancellationChangeToken(_cancellationTokenSource.Token))
                .RegisterPostEvictionCallback(PostEviction);
            if (duration.HasValue)
            {
                options.SetSlidingExpiration(TimeSpan.FromMinutes(duration.Value));
            }
            return options;
        }

        private void PostEviction(object key, object value, EvictionReason reason, object state)
        {
            if (reason == EvictionReason.Replaced)
                return;

            this.Remove(key.ToString());
        }

        public CacheVersion<T> GetVersionItems<T>(string key) where T : class
        {
            var cacheVersion = _cache.Get<CacheVersion<T>>(key);
            return cacheVersion;
        }

        public void SetVersionItems<T>(string key, CacheVersion<T> cacheVersion) where T : class
        {
            using (_rwLock.GetWriteLock())
            {
                _cache.Set<CacheVersion<T>>(key, cacheVersion);
            }
        }

        public T GetTObject<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        public void SetTObject<T>(string key, T value)
        {
            using (_rwLock.GetWriteLock())
            {
                _cache.Set<T>(key, value);
            }
        }
    }
}