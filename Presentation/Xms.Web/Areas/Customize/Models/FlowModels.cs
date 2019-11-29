using System;
using System.Collections.Generic;
using Xms.Core;
using Xms.Flow.Domain;
using Xms.Web.Framework.Paging;

namespace Xms.Web.Customize.Models
{
    public class WorkFlowModel : BasePaged<WorkFlow>
    {
        public string Name { get; set; }
        public Guid SolutionId { get; set; }
        public bool LoadData { get; set; }
    }

    public class CreateWorkFlowModel
    {
        //public Guid WorkFlowId { get; set; }

        public string Name { get; set; }
        public Guid EntityId { get; set; }

        public string Description { get; set; }

        public RecordState StateCode { get; set; }
        public bool EnabledAuthorization { get; set; }

        public List<WorkFlowStep> Steps { get; set; }
        public Guid SolutionId { get; set; }
        public string StepData { get; set; }
    }

    public class EditWorkFlowModel
    {
        public Guid WorkFlowId { get; set; }
        public Guid EntityId { get; set; }
        public Schema.Domain.Entity EntityMetas { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public RecordState StateCode { get; set; }
        public bool EnabledAuthorization { get; set; }

        public List<WorkFlowStep> Steps { get; set; }
        public string StepData { get; set; }
        public Guid SolutionId { get; set; }
    }

    public class WorkFlowInstanceModel : BasePaged<WorkFlowInstance>
    {
        public WorkFlow FlowInfo { get; set; }
        public Guid WorkFlowId { get; set; }
    }

    public class WorkFlowInstanceDetailModel : BasePaged<WorkFlowInstance>
    {
        public WorkFlow FlowInfo { get; set; }
    }

    public class WorkFlowProcessModel : BasePaged<WorkFlowProcess>
    {
        public Guid WorkFlowInstanceId { get; set; }
        public WorkFlowInstance InstanceInfo { get; set; }
        public WorkFlow FlowInfo { get; set; }
    }

    public class CreateBusinessFlowModel
    {
        //public Guid WorkFlowId { get; set; }

        public string Name { get; set; }
        public Guid EntityId { get; set; }

        public string Description { get; set; }

        public RecordState StateCode { get; set; }

        public List<ProcessStage> ProcessStages { get; set; }
        public string StepData { get; set; }
        public Guid SolutionId { get; set; }
    }

    public class EditBusinessFlowModel
    {
        public Guid WorkFlowId { get; set; }
        public Guid EntityId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public RecordState StateCode { get; set; }

        public List<ProcessStage> ProcessStages { get; set; }
        public string StepData { get; set; }
        public Guid SolutionId { get; set; }
    }

    public class SetFlowAuthorizationStateModel
    {
        public Guid[] RecordId { get; set; }
        public bool IsAuthorization { get; set; }
    }
}