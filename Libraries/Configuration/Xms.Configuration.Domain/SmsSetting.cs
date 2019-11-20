using System.Collections.Generic;

namespace Xms.Configuration.Domain
{
    public class SmsSetting
    {
        public const string CACHE_KEY = "$SmsSetting$";
        public string Url { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }

        public List<SmsBody> Bodies { get; set; }
    }

    public class SmsBody
    {
        public string Name { get; set; }

        public string Content { get; set; }
    }
}