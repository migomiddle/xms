using PetaPoco;
using System;
using Xms.Core;

namespace Xms.Plugin.Domain
{
    [TableName("EntityPlugin")]
    [PrimaryKey("EntityPluginId", AutoIncrement = false)]
    public class EntityPlugin
    {
        public Guid EntityPluginId { get; set; } = Guid.NewGuid();

        public Guid EntityId { get; set; }

        public string AssemblyName { get; set; }

        public string ClassName { get; set; }

        public string EventName { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public Guid SolutionId { get; set; }
        public int ComponentState { get; set; }
        public int ProcessOrder { get; set; }

        /// <summary>
        /// 0实体插件 1视图插件 2表单插件
        public int TypeCode { get; set; }

        public Guid BusinessObjectId { get; set; }
        public RecordState StateCode { get; set; }
        public Guid OrganizationId { get; set; }
        public bool IsVisibled { get; set; }
    }
}