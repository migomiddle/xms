using Newtonsoft.Json;
using System;

namespace Xms.Session
{
    /// <summary>
    /// 会话服务
    /// </summary>
    public class SessionService : ISessionService
    {
        private readonly ISession _session;

        public SessionService(ISession session)
        {
            _session = session;
        }

        public void Set(string key, object value, int? expiration = null)
        {
            _session.Set(key, value, expiration);
        }

        public void Remove(string key)
        {
            _session.Remove(key);
        }

        public string GetValue(string key)
        {
            return _session.Get(key);
        }

        public void SetValue<T>(string key, T entry, int? expiration)
        {
            if (String.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("key must have a value", "key");
            }

            _session.Set(key, JsonConvert.SerializeObject(entry), expiration);
        }

        public T GetValue<T>(string key) where T : class
        {
            if (String.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("key must have a value", "key");
            }

            var sessionItem = _session.Get<T>(key);

            if (sessionItem == null)
            {
                return null;// default(T);
            }

            return sessionItem;// JsonConvert.DeserializeObject<T>(sessionItem.ToString());
        }

        public string GetId()
        {
            return _session.GetId();
        }
    }
}