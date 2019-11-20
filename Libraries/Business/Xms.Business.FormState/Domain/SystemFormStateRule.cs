using PetaPoco;
using System;
using Xms.Form.Abstractions;

namespace Xms.Business.FormStateRule.Domain
{
    [TableName("SystemFormStateRule")]
    [PrimaryKey("SystemFormStateRuleId", AutoIncrement = false)]
    public class SystemFormStateRule
    {
        public Guid SystemFormStateRuleId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public FormState FormState { get; set; }
        public string CommandRules { get; set; }
        public Guid EntityId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public Guid ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid OrganizationId { get; set; }
        public int ComponentState { get; set; }
        public Guid SolutionId { get; set; }
    }
}