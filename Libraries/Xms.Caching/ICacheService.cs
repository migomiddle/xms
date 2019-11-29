using System.Collections.Generic;

namespace Xms.Caching
{
    /// <summary>
    /// 缓存服务
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// 空值占位符
        /// </summary>
        string FAKE_NULL { get; }

        /// <summary>
        /// 是否分布式
        /// </summary>
        bool IsDistributedCache { get; }

        /// <summary>
        /// 清空缓存所有数据
        /// </summary>
        void Clear();

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Contains(string key);

        /// <summary>
        /// 是否存在于列表
        /// </summary>
        /// <param name="key"></param>
        /// <param name="itemKey"></param>
        /// <returns></returns>
        bool ContainsListItem(string key, string itemKey);

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object Get(string key);

        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key) where T : class;

        /// <summary>
        /// 获取列表所有记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        List<T> GetListAll<T>(string key) where T : class;

        /// <summary>
        /// 获取列表中的一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="itemKey"></param>
        /// <returns></returns>
        T GetListItem<T>(string key, string itemKeyValue) where T : class;

        /// <summary>
        /// 获取列表中的多条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="itemKey"></param>
        /// <returns></returns>
        List<T> GetListItems<T>(string key, params string[] itemKeyValue) where T : class;

        /// <summary>
        /// 获取列表中的多条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="keyPattern"></param>
        /// <returns></returns>
        List<T> GetListItemsByPattern<T>(string key, params string[] keyPattern) where T : class;

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="keyPattern"></param>
        void RemoveByPattern(string keyPattern);

        /// <summary>
        /// 移除列表中的记录
        /// </summary>
        /// <param name="key"></param>
        /// <param name="itemKeyValue"></param>
        void RemoveListItem<T>(string key, params string[] itemKeyValue) where T : class;

        /// <summary>
        /// 移除列表中的记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="itemKeyPattern">模糊键</param>
        void RemoveListItemByPattern<T>(string key, params string[] itemKeyPattern) where T : class;

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cacheTime"></param>
        void Set(string key, object value, int? cacheTime = null);

        /// <summary>
        /// 设置列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="itemKey"></param>
        /// <param name="value"></param>
        /// <param name="cacheTime"></param>
        void SetList<T>(string key, string itemKey, List<T> value, int? cacheTime = null) where T : class;

        /// <summary>
        /// 设置列表
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keyValues"></param>
        /// <param name="cacheTime"></param>
        void SetList<T>(string key, Dictionary<string, T> keyValues, int? cacheTime = null) where T : class;

        /// <summary>
        /// 设置列表中的一条记录
        /// </summary>
        /// <param name="key"></param>
        /// <param name="itemKey"></param>
        /// <param name="value"></param>
        /// <param name="cacheTime"></param>
        void SetListItem<T>(string key, string itemKey, T value, int? cacheTime = null) where T : class;

        /// <summary>
        /// 按照缓存版本获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        CacheVersion<T> GetVersionItems<T>(string key) where T : class;

        /// <summary>
        /// 按照缓存版本设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="cacheVersion"></param>
        void SetVersionItems<T>(string key, CacheVersion<T> cacheVersion) where T : class;

        /// <summary>
        /// 直接获缓存取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetTObject<T>(string key);

        /// <summary>
        /// 直接设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetTObject<T>(string key, T value);
    }
}