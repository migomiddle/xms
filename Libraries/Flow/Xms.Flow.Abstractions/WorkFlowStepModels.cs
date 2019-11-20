using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using Xms.Sdk.Abstractions.Query;

namespace Xms.Flow.Abstractions
{
    public class ProcessStep
    {
        public Guid StepId { get; set; }
        public string AttributeName { get; set; }
        public string DisplayName { get; set; }
        public bool IsRequired { get; set; }
    }

    public class WorkFlowStepCondition
    {
        public Guid PrevStepId { get; set; }
        public Guid NextStepId { get; set; }
        public LogicalOperator LogicalOperator { get; set; }
        public List<ConditionExpression> Conditions { get; set; }
    }

    public class WorkFlowStepAuthAttributes
    {
        public string Name { get; set; }
        public int PermissionMask { get; set; }
    }

    public class WorkFlowStepHandler
    {
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public WorkFlowStepHandlerType Type { get; set; }

        public Guid Id { get; set; }
    }

    public enum WorkFlowStepHandlerType
    {
        SystemUser,
        Team,
        Post,
        Job,
        Roles
    }

    public enum WorkFlowStepReturnType
    {
        PrevStep,
        FirstStep,
        SpecifyStep,
        ReSubmit
    }
}