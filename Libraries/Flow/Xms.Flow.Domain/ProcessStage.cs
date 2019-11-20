using PetaPoco;
using System;

namespace Xms.Flow.Domain
{
    [TableName("ProcessStage")]
    [PrimaryKey("ProcessStageId", AutoIncrement = false)]
    public class ProcessStage
    {
        public Guid ProcessStageId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public int Category { get; set; }
        public Guid EntityId { get; set; }
        public string RelationshipName { get; set; }
        public Guid WorkFlowId { get; set; }
        public int StageOrder { get; set; }
        public string Steps { get; set; }
    }
}