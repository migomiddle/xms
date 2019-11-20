namespace Xms.Session
{
    public interface ISessionService
    {
        string GetId();

        string GetValue(string key);

        T GetValue<T>(string key) where T : class;

        void Remove(string key);

        void Set(string key, object value, int? expiration = null);

        void SetValue<T>(string key, T entry, int? expiration);
    }
}