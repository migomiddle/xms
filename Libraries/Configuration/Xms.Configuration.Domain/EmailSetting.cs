using System.Collections.Generic;

namespace Xms.Configuration.Domain
{
    public class EmailSetting
    {
        public const string CACHE_KEY = "$EmailSetting$";
        public string Host { get; set; }
        public string Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string From { get; set; }
        public string FromName { get; set; }
        public List<EmailBody> Bodies { get; set; }
    }

    public class EmailBody
    {
        public string Name { get; set; }

        public string Content { get; set; }
    }
}