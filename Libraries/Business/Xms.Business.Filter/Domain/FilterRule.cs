using PetaPoco;
using System;
using Xms.Core;
using Xms.Core.Data;

namespace Xms.Business.Filter.Domain
{
    [TableName("FilterRule")]
    [PrimaryKey("FilterRuleId", AutoIncrement = false)]
    public class FilterRule
    {
        public Guid FilterRuleId { get; set; } = Guid.NewGuid();
        public Guid EntityId { get; set; }
        public string Name { get; set; }
        public string Conditions { get; set; }
        public string ToolTip { get; set; }
        public string EventName { get; set; }
        public RecordState StateCode { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int ComponentState { get; set; }

        [ResultColumn]
        [LinkEntity(typeof(Entity), LinkFromFieldName = "EntityId", LinkToFieldName = "EntityId", TargetFieldName = "Name")]
        public string EntityName { get; set; }
    }
}