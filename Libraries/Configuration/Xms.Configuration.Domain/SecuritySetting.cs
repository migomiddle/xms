namespace Xms.Configuration.Domain
{
    public class SecuritySetting
    {
        public const string CACHE_KEY = "$SecuritySetting$";
        public string AntiForgeryTokenSalt { get; set; }
    }
}