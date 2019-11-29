using PetaPoco;
using System;

namespace Xms.Module.Domain
{
    [TableName("Module")]
    [PrimaryKey("ModuleId", AutoIncrement = false)]
    public class Module
    {
        public Guid ModuleId { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public string LocalizedName { get; set; }

        public int Identity { get; set; }

        public string EntryClassName { get; set; }
    }
}