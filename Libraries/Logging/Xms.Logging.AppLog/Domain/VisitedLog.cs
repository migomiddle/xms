using PetaPoco;
using System;
using Xms.Core;
using Xms.Logging.Abstractions;

namespace Xms.Logging.AppLog.Domain
{
    [TableName("VisitedLog")]
    [PrimaryKey("VisitedLogId", AutoIncrement = false)]
    public class VisitedLog
    {
        public Guid VisitedLogId { get; set; } = Guid.NewGuid();
        public LogLevel LogLevel { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid SystemUserId { get; set; }
        public int StatusCode { get; set; }
        public string ClientIP { get; set; }
        public string Url { get; set; }
        public string UrlReferrer { get; set; }
        public int TypeCode { get; set; } = 2;
        public Guid OrganizationId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        #region 视图字段

        [ResultColumn]
        [LinkEntity(LinkFromFieldName = "systemuserid", LinkToTableName = "SystemUser", LinkToFieldName = "systemuserid", TargetFieldName = "name")]
        public string UserName { get; set; }

        #endregion 视图字段
    }
}