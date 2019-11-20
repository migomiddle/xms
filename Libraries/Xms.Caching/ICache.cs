using System.Collections.Generic;

namespace Xms.Caching
{
    /// <summary>
    /// Cache holder interface
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// null value placement
        /// </summary>
        string FAKE_NULL { get; }

        /// <summary>
        /// Gets all entries in the cache
        /// </summary>
        IEnumerable<KeyValuePair<string, object>> Entries { get; }

        /// <summary>
        /// Gets a cache item associated with the specified key
        /// </summary>
        /// <param name="key">The cache item key</param>
        /// <returns>Cached item value</returns>
        object Get(string key);

        /// <summary>
        /// Gets a cache item associated with the specified key
        /// </summary>
        /// <param name="key">The cache item key</param>
        /// <returns>Cached item value</returns>
        T Get<T>(string key) where T : class, new();

        /// <summary>
        /// Adds the cache item with the specified key
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="data">Data</param>
        /// <param name="cacheTime">Cache time in minutes</param>
        void Set(string key, object value, int? cacheTime);

        /// <summary>
        /// Gets a value indicating whether an item associated with the specified key exists in the cache
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>Result</returns>
        bool Contains(string key);

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">/key</param>
        void Remove(string key);

        /// <summary>
        /// Clear cache items
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets a value indicating whether reads and writes to this cache should be thread safe
        /// </summary>
        bool IsSingleton { get; }
    }
}