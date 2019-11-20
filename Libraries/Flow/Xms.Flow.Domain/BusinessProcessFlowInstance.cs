using PetaPoco;
using System;

namespace Xms.Flow.Domain
{
    [TableName("BusinessProcessFlowInstance")]
    [PrimaryKey("BusinessProcessFlowInstanceId", AutoIncrement = false)]
    public class BusinessProcessFlowInstance
    {
        public Guid BusinessProcessFlowInstanceId { get; set; } = Guid.NewGuid();
        public Guid? ProcessStageId { get; set; }
        public Guid? Entity1Id { get; set; }
        public Guid? Entity2Id { get; set; }
        public Guid? Entity3Id { get; set; }
        public Guid? Entity4Id { get; set; }
        public Guid? Entity5Id { get; set; }
        public Guid WorkFlowId { get; set; }
        public Guid ProcessEntityId { get; set; }
    }
}