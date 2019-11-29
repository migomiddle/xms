using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Infrastructure.Utility;

namespace Xms.Caching
{
    /// <summary>
    /// 缓存服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CacheManager<T> where T : class
    {
        private static readonly ICacheService _cache = CacheServiceBuilder.Build();

        private readonly Func<T, string> _buildKey;

        private Func<List<T>> _preCache;

        private bool _cacheEnabled => _cache != null && _platformSettingCacheEnabled;

        private bool _platformSettingCacheEnabled;

        private readonly string _cacheKey;

        private readonly string _preCacheKey;

        private readonly string _cacheVersion;

        private readonly string _cacheIndex;

        public CacheManager(string cacheKey, Func<T, string> buildKey, bool platformSettingCacheEnabled = true, Func<List<T>> preCache = null)
        {
            _cacheKey = cacheKey;
            _buildKey = buildKey;
            _preCacheKey = _cacheKey + ".precache";
            _cacheVersion = _cacheKey + ".version";
            _cacheIndex = _cacheKey + ".index";
            _preCache = preCache;
            _platformSettingCacheEnabled = platformSettingCacheEnabled;
            EnsurePreCached();
        }

        public CacheManager(string cacheKey, bool platformSettingCacheEnabled = true)
        {
            _cacheKey = cacheKey;
            _buildKey = DefaultBuildKey;
            _preCacheKey = _cacheKey + ".precache";
            _cacheVersion = _cacheKey + ".version";
            _cacheIndex = _cacheKey + ".index";
            _preCache = null;
            _platformSettingCacheEnabled = platformSettingCacheEnabled;
            EnsurePreCached();
        }

        private string DefaultBuildKey(T entity)
        {
            return "";
        }

        private CacheManager<T> EnsurePreCached()
        {
            if (_cacheEnabled && _preCache != null && !_cache.Contains(_preCacheKey))
            {
                _cache.Set(_preCacheKey, true);
                var data = _preCache();
                if (data.NotEmpty())
                {
                    SetListItem(data);
                }
                else
                {
                    SetNullValue(_cacheKey);
                }
            }
            return this;
        }

        public bool Exists()
        {
            if (!_cacheEnabled)
            {
                return false;
            }
            //precache
            //this.EnsurePreCached();

            return _cache.Contains(_cacheKey);
        }

        private bool TryGet(string key, out T value)
        {
            //precache
            //this.EnsurePreCached();

            value = default(T);

            object obj = _cache.Get<object>(key);

            if (obj != null)
            {
                if (obj.Equals(_cache.FAKE_NULL))
                {
                    return true;
                }

                value = (T)obj;
                return true;
            }

            return false;
        }

        public T Get(Func<T> acquirer)
        {
            if (!_cacheEnabled)
            {
                return acquirer();
            }

            if (TryGet(_cacheKey, out T value))
            {
                return value;
            }
            value = acquirer();
            Set(value);
            return value;
        }

        public T GetItem(Func<T> acquirer, string itemKey)
        {
            if (!_cacheEnabled)
            {
                return acquirer();
            }
            //precache
            //this.EnsurePreCached();

            var result = _cache.GetListItem<T>(_cacheKey, itemKey);
            if (result != null)
            {
                return result;
            }
            result = acquirer();
            if (result != null)
            {
                SetListItem(result);
            }
            else
            {
                SetNullValue(_cacheKey);
            }
            return result;
        }

        public T GetItemByPattern(Func<T> acquirer, string keyPattern)
        {
            if (!_cacheEnabled)
            {
                return acquirer();
            }
            //precache
            //this.EnsurePreCached();

            var matches = _cache.GetListItemsByPattern<T>(_cacheKey, keyPattern);
            if (matches.NotEmpty())
            {
                return matches.First();
            }
            var value = acquirer();
            if (value != null)
            {
                SetListItem(value);
                return value;
            }
            else
            {
                SetNullValue(_cacheKey);
            }
            return null;
        }

        public List<T> GetItems(Func<List<T>> acquirer)
        {
            if (!_cacheEnabled)
            {
                return acquirer();
            }
            //precache
            //this.EnsurePreCached();

            var result = _cache.GetListAll<T>(_cacheKey);
            if (result.IsEmpty())
            {
                result = acquirer();
                if (result.NotEmpty())
                {
                    SetListItem(result);
                    return result;
                }
                else
                {
                    SetNullValue(_cacheKey);
                }
            }
            return result;
        }

        public List<T> GetItemsByPattern(Func<List<T>> acquirer, params string[] keyPattern)
        {
            if (!_cacheEnabled)
            {
                return acquirer();
            }
            //precache
            //this.EnsurePreCached();
            List<T> result = _cache.GetListItemsByPattern<T>(_cacheKey, keyPattern);
            if (result.NotEmpty())
            {
                return result;
            }
            result = acquirer();
            if (result.NotEmpty())
            {
                SetListItem(result);
                return result;
            }
            else
            {
                foreach (var k in keyPattern)
                {
                    SetNullValue(k.Replace("*", ""));
                }
            }
            return result;
        }

        public CacheManager<T> Remove()
        {
            if (!_cacheEnabled)
            {
                return this;
            }

            _cache.Clear();//.Remove(_cacheKey);
            return this;
        }

        public CacheManager<T> Remove(T entity)
        {
            if (!_cacheEnabled)
            {
                return this;
            }

            _cache.RemoveListItem<T>(_cacheKey, _buildKey(entity));
            return this;
        }

        public CacheManager<T> RemoveByPattern(string pattern)
        {
            if (!_cacheEnabled)
            {
                return this;
            }
            _cache.RemoveByPattern(pattern);
            return this;
        }

        public CacheManager<T> RemoveItemByPattern(string pattern)
        {
            if (!_cacheEnabled)
            {
                return this;
            }
            _cache.RemoveListItemByPattern<T>(pattern);
            return this;
        }

        public CacheManager<T> Set(T entity)
        {
            if (!_cacheEnabled)
            {
                return this;
            }

            _cache.Set(_cacheKey, entity);
            return this;
        }

        public CacheManager<T> SetListItem(T entity)
        {
            if (!_cacheEnabled)
            {
                return this;
            }

            _cache.SetListItem(_cacheKey, _buildKey(entity), entity);
            return this;
        }

        public CacheManager<T> SetListItem(IEnumerable<T> entities)
        {
            if (!_cacheEnabled)
            {
                return this;
            }

            Dictionary<string, T> datas = new Dictionary<string, T>();
            foreach (var item in entities)
            {
                datas.Add(_buildKey(item), item);
            }
            _cache.SetList(_cacheKey, datas);
            return this;
        }

        public CacheManager<T> SetNullValue(string key)
        {
            if (!_cacheEnabled)
            {
                return this;
            }

            _cache.SetListItem<T>(_cacheKey, _cache.FAKE_NULL + "/" + key + "/", null);
            return this;
        }

        #region 缓存版本管理

        private string GetRealKey(string key)
        {
            return _cacheKey + ":" + key;
        }

        public int UpdateVersion()
        {
            int version = _cache.GetTObject<int>(_cacheVersion) + 1;
            _cache.SetTObject<int>(_cacheVersion, version);
            return version;
        }

        public List<T> GetVersionItems(string key, Func<List<T>> acquirer)
        {
            if (!_cacheEnabled)
            {
                return acquirer();
            }
            var realkey = GetRealKey(key);
            realkey = "ls;" + realkey;
            int version = _cache.GetTObject<int>(_cacheVersion);
            var result = _cache.GetVersionItems<T>(realkey);
            bool updateCache;
            if (result != null)
            {
                if (result.Version == version)
                {
                    return Clone(result.Items);
                }
                else
                {
                    updateCache = true;
                }
            }
            else
            {
                updateCache = true;
            }

            if (updateCache)
            {
                var items = acquirer();
                if (items == null) { return items; }
                var cacheVersion = new CacheVersion<T>()
                {
                    Version = version,
                    Items = items
                };
                SetVersionItems(key, cacheVersion);
                return Clone(items);
            }
            return null;
        }

        public CacheManager<T> SetVersionItems(string key, CacheVersion<T> cacheVersion)
        {
            if (!_cacheEnabled)
            {
                return this;
            }
            var realkey = GetRealKey(key);
            realkey = "ls;" + realkey;
            _cache.SetVersionItems<T>(realkey, cacheVersion);
            return this;
        }

        private Dictionary<string, List<string>> GetIndex()
        {
            var dicIndex = _cache.GetTObject<Dictionary<string, List<string>>>(_cacheIndex);
            if (dicIndex == null)
            {
                dicIndex = new Dictionary<string, List<string>>();
                _cache.SetTObject(_cacheIndex, dicIndex);
            }
            return dicIndex;
        }

        private void SetIndex(string key, List<string> list)
        {
            lock (string.Intern(_cacheKey))
            {
                var dicIndex = GetIndex();
                if (!dicIndex.TryGetValue(key, out List<string> result))
                {
                    dicIndex.Add(key, list);
                }
            }
        }

        public CacheManager<T> SetEntity(T entity)
        {
            if (!_cacheEnabled)
            {
                return this;
            }
            var dicIndex = GetIndex();
            foreach (var kp in dicIndex)
            {
                var key = kp.Key + "/" + GetIndexValue(kp.Value, entity);
                _cache.SetTObject(GetRealKey(key), entity);
            }

            UpdateVersion();
            return this;
        }

        public CacheManager<T> RemoveEntity(T entity)
        {
            if (!_cacheEnabled)
            {
                return this;
            }
            var dicIndex = GetIndex();
            foreach (var kp in dicIndex)
            {
                var key = kp.Key + "/" + GetIndexValue(kp.Value, entity);
                _cache.Remove(GetRealKey(key));
            }
            UpdateVersion();
            return this;
        }

        /// <summary>
        /// 根据索引字段获取索引值
        /// </summary>
        /// <param name="indexFieldName"></param>
        /// <param name="tObject"></param>
        /// <returns></returns>
        public static string GetIndexValue(List<string> lsIndexFieldName, T tObject)
        {
            Type t = typeof(T);

            var listname = new List<string>();
            foreach (var prop in t.GetProperties())
            {
                var lst = lsIndexFieldName.Where(x => x.IsCaseInsensitiveEqual(prop.Name)).ToList();
                if (lst.Count > 0)
                {
                    string sname = prop.GetValue(tObject, null) == null ? "" : prop.GetValue(tObject, null).ToString();
                    listname.Add(sname);
                }
            }
            return string.Join("/", listname);
        }

        public T Get(Dictionary<string, string> dic, Func<T> acquirer)
        {
            if (!_cacheEnabled)
            {
                return acquirer();
            }
            var keys = new List<string>();
            var keysvalue = new List<string>();
            foreach (var kp in dic)
            {
                keys.Add(kp.Key.ToLower());
                keysvalue.Add(kp.Value.ToLower());
            }
            keys.Sort();
            keysvalue.Sort();
            var keyIndex = string.Join("/", keys);
            //字段+值为索引
            var keyValue = keyIndex + "/" + string.Join("/", keysvalue);
            var realkey = GetRealKey(keyValue);
            if (TryGetTObject(realkey, out T value))
            {
                return Clone(value);
            }
            value = acquirer();
            if (value == null) { return value; }
            Set(realkey, value);
            SetIndex(keyIndex, keys);
            return Clone(value);
        }

        private bool TryGetTObject(string key, out T value)
        {
            value = default(T);

            object obj = _cache.GetTObject<object>(key);

            if (obj != null)
            {
                if (obj.Equals(_cache.FAKE_NULL))
                {
                    return true;
                }

                value = (T)obj;
                return true;
            }

            return false;
        }

        public CacheManager<T> Set(string key, T entity)
        {
            if (!_cacheEnabled)
            {
                return this;
            }

            _cache.SetTObject(key, entity);
            return this;
        }

        public T Clone(T item)
        {
            var newItem = Activator.CreateInstance<T>();
            item.CopyTo(newItem);
            return newItem;
        }

        public List<T> Clone(List<T> items)
        {
            var itemsClone = new List<T>();
            foreach (var item in items)
            {
                itemsClone.Add(Clone(item));
            }
            return itemsClone;
        }

        #endregion 缓存版本管理
    }
}