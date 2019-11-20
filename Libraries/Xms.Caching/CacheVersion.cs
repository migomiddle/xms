using System.Collections.Generic;

namespace Xms.Caching
{
    public class CacheVersion<T>
    {
        public int Version { get; set; }
        public List<T> Items { get; set; }
    }
}