using PetaPoco;
using System;
using Xms.Core;

namespace Xms.QueryView.Domain
{
    [TableName("QueryView")]
    [PrimaryKey("QueryViewId", AutoIncrement = false)]
    public class QueryView
    {
        public Guid QueryViewId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public bool IsCustomButton { get; set; }
        public string FetchConfig { get; set; }
        public string LayoutConfig { get; set; }
        public string AggregateConfig { get; set; }
        public string CustomButtons { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? ModifiedOn { get; set; }
        public Guid? ModifiedBy { get; set; }
        public Guid? OwnerId { get; set; }
        public bool IsPrivate { get; set; }
        public RecordState StateCode { get; set; }

        public Guid EntityId { get; set; }
        public string SqlString { get; set; }
        public bool AuthorizationEnabled { get; set; }
        public Guid? TargetFormId { get; set; }

        public Guid SolutionId { get; set; }
        public int ComponentState { get; set; }

        [ResultColumn]
        [LinkEntity(LinkFromFieldName = "EntityId", LinkToTableName = "Entity", LinkToFieldName = "EntityId", TargetFieldName = "Name")]
        public string EntityName { get; set; }
    }
}