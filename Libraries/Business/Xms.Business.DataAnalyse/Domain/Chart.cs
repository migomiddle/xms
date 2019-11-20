using PetaPoco;
using System;
using Xms.Core;
using Xms.Schema.Domain;

namespace Xms.Business.DataAnalyse.Domain
{
    [TableName("Chart")]
    [PrimaryKey("ChartId", AutoIncrement = false)]
    public class Chart
    {
        public Guid ChartId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid EntityId { get; set; }
        public string PresentationConfig { get; set; }
        public string DataConfig { get; set; }
        public RecordState StateCode { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public Guid? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid SolutionId { get; set; }
        public int ComponentState { get; set; }

        [ResultColumn]
        [LinkEntity(typeof(Entity), LinkFromFieldName = "EntityId", LinkToFieldName = "EntityId", TargetFieldName = "Name")]
        public string EntityName { get; set; }
    }
}