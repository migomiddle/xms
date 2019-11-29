using PetaPoco;
using System;
using System.Collections.Generic;
using Xms.Core;

namespace Xms.Flow.Domain
{
    [TableName("WorkFlow")]
    [PrimaryKey("WorkFlowId", AutoIncrement = false)]
    public class WorkFlow
    {
        public Guid WorkFlowId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public RecordState StateCode { get; set; }
        public Guid EntityId { get; set; }
        public int Category { get; set; }
        public bool AuthorizationEnabled { get; set; }
        public Guid SolutionId { get; set; }
        public int ComponentState { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [Ignore]
        public List<WorkFlowStep> Steps { get; set; }

        [Ignore]
        public List<ProcessStage> Stages { get; set; }
    }
}