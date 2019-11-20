using PetaPoco;
using System;

namespace Xms.Data.Import.Domain
{
    [TableName("ImportMap")]
    [PrimaryKey("ImportMapId", AutoIncrement = false)]
    public class ImportMap
    {
        public Guid ImportMapId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public string MapCustomizations { get; set; }
        public int MapType { get; set; }
        public string TargetEntityName { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}