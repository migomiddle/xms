using PetaPoco;
using System;
using Xms.Core;

namespace Xms.Logging.DataLog.Domain
{
    [TableName("EntityLog")]
    [PrimaryKey("EntityLogId", AutoIncrement = false)]
    public class EntityLog
    {
        public Guid EntityLogId { get; set; } = Guid.NewGuid();
        public Guid EntityId { get; set; }
        public Guid UserId { get; set; }
        public Guid RecordId { get; set; }
        public string ChangeData { get; set; }
        public string AttributeMask { get; set; }
        public OperationTypeEnum OperationType { get; set; }
        public Guid OrganizationId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [ResultColumn]
        [LinkEntity(LinkFromFieldName = "UserId", LinkToTableName = "SystemUser", LinkToFieldName = "SystemUserId", TargetFieldName = "Name")]
        public string UserIdName { get; set; }
    }
}