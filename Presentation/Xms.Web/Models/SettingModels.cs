using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xms.Configuration.Domain;

namespace Xms.Web.Models
{
    public class PlatformSettingModel
    {
        public string AppName { get; set; }

        public int Status { get; set; }

        public string ClosedReason { get; set; }

        public int LogLevel { get; set; }
        public bool ShowMenuInUserPrivileges { get; set; }

        public string AppVersion { get; set; }

        public bool LogEnabled { get; set; }
        public bool DataLogEnabled { get; set; }
        public bool VerifyCodeEnabled { get; set; }
        public int MaxFetchRecords { get; set; }
        public bool CacheEnabled { get; set; }
    }

    public class SmsSettingModel
    {
        public string Url { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }

        public List<SmsBody> Bodies { get; set; }

        public List<string> Name { get; set; }
        public List<string> Content { get; set; }
    }

    public class EmailSettingModel
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string From { get; set; }
        public string FromName { get; set; }
        public List<EmailBody> Bodies { get; set; }

        public List<string> Name { get; set; }
        public List<string> Content { get; set; }
    }

    public class SendEmailsModel
    {
        [Required]
        public string EmailAddresses { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Content { get; set; }
    }

    public class UploadSettingModel
    {
        public string FileExts { get; set; }
        public long MaxSize { get; set; }
        public string FormatName { get; set; }
    }

    public class CacheSettingModel
    {
        public bool Enabled { get; set; }
        public List<CacheServiceProvider> Providers { get; set; }
    }
}