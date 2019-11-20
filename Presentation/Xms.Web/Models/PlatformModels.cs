using System;
using Xms.Logging.AppLog.Domain;
using Xms.Web.Framework.Paging;

namespace Xms.Web.Models
{
    public class LogModel : BasePaged<VisitedLog>
    {
        public int StatusCode { get; set; }

        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }

        public string Url { get; set; }
        public string ClientIp { get; set; }

        public string Description { get; set; }
    }

    public class LogDetailModel
    {
        public VisitedLog LogDetail { get; set; }
    }
}