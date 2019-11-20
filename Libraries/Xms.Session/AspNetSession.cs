using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Xms.Session
{
    /// <summary>
    /// .net session provider
    /// </summary>
    public class AspNetSession : ISession
    {
        private readonly HttpContext _httpContext;

        public AspNetSession(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext.HttpContext;
        }

        public void Clear()
        {
            _httpContext.Session?.Clear();
        }

        public T Get<T>(string key) where T : class
        {
            var value = _httpContext.Session?.GetString(key);

            return value == null ? default(T) :
                JsonConvert.DeserializeObject<T>(value);
        }

        public string Get(string key)
        {
            return _httpContext.Session?.GetString(key);
        }

        public void Remove(string key)
        {
            _httpContext.Session?.Remove(key);
        }

        public void Set(string key, object value, int? expiration)
        {
            _httpContext.Session?.SetString(key, value is string ? value.ToString() : JsonConvert.SerializeObject(value));
        }

        public string GetId()
        {
            return _httpContext.Session?.Id;
        }
    }
}