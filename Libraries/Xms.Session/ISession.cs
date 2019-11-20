namespace Xms.Session
{
    public interface ISession
    {
        void Set(string key, object value, int? expiration);

        T Get<T>(string key) where T : class;

        string Get(string key);

        void Remove(string key);

        void Clear();

        string GetId();
    }
}