using PetaPoco;
using System;
using Xms.Core;

namespace Xms.Flow.Domain
{
    [TableName("WorkFlowProcessLog")]
    [PrimaryKey("WorkFlowProcessLogId", AutoIncrement = false)]
    public class WorkFlowProcessLog
    {
        public Guid WorkFlowProcessLogId { get; set; } = Guid.NewGuid();

        public Guid WorkFlowProcessId { get; set; }

        public Guid WorkFlowInstanceId { get; set; }

        public Guid OperatorId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [ResultColumn]
        [LinkEntity(LinkFromFieldName = "OperatorId", LinkToTableName = "SystemUser", LinkToFieldName = "SystemUserId", TargetFieldName = "Name")]
        public string OperatorIdName { get; set; }
    }
}