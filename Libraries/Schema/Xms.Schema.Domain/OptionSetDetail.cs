using PetaPoco;
using System;

namespace Xms.Schema.Domain
{
    [TableName("OptionSetDetail")]
    [PrimaryKey("OptionSetDetailId", AutoIncrement = false)]
    public class OptionSetDetail
    {
        public Guid OptionSetDetailId { get; set; } = Guid.NewGuid();
        public Guid OptionSetId { get; set; }
        public int Value { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public int DisplayOrder { get; set; }
    }
}