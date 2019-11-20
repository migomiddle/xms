using PetaPoco;
using System;

namespace Xms.Schema.Domain
{
    [TableName("stringmap")]
    [PrimaryKey("stringmapid", AutoIncrement = false)]
    public class StringMap
    {
        public Guid StringMapId { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public int Value { get; set; }

        public Guid AttributeId { get; set; }

        public int DisplayOrder { get; set; }
        public string AttributeName { get; set; }
        public string EntityName { get; set; }
    }
}