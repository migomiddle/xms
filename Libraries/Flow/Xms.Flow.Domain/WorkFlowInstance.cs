using PetaPoco;
using System;
using System.Collections.Generic;
using Xms.Core;
using Xms.Flow.Abstractions;

namespace Xms.Flow.Domain
{
    [TableName("WorkFlowInstance")]
    [PrimaryKey("WorkFlowInstanceId", AutoIncrement = false)]
    public class WorkFlowInstance
    {
        public Guid WorkFlowInstanceId { get; set; } = Guid.NewGuid();
        public Guid WorkFlowId { get; set; }
        public string Description { get; set; }
        public Guid EntityId { get; set; }
        public Guid ObjectId { get; set; }
        public Guid ApplicantId { get; set; }
        public int Attachments { get; set; }
        public WorkFlowProcessState StateCode { get; set; }
        public DateTime? CompletedOn { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [ResultColumn]
        [LinkEntity(LinkFromFieldName = "ApplicantId", LinkToTableName = "SystemUser", LinkToFieldName = "SystemUserId", TargetFieldName = "Name")]
        public string ApplicantName { get; set; }

        [ResultColumn]
        [LinkEntity(LinkFromFieldName = "EntityId", LinkToTableName = "Entity", LinkToFieldName = "EntityId", TargetFieldName = "Name")]
        public string EntityName { get; set; }

        [ResultColumn]
        [LinkEntity(LinkFromFieldName = "EntityId", LinkToTableName = "Entity", LinkToFieldName = "EntityId", TargetFieldName = "LocalizedName")]
        public string EntityLocalizedName { get; set; }

        [Ignore]
        public List<WorkFlowProcess> Steps { get; set; }
    }
}