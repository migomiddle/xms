using PetaPoco;
using System;
using System.Diagnostics;
using Xms.Core;

namespace Xms.Configuration.Domain
{
    /// <summary>
    /// 配置信息
    /// </summary>
    [DebuggerDisplay("{Name}: {Value}")]
    [TableName("Settings")]
    [PrimaryKey("SettingsId", AutoIncrement = false)]
    public partial class Setting
    {
        public Setting()
        {
        }

        public Setting(string name, string value, Guid organizationId)
        {
            this.Name = name;
            this.Value = value;
            this.OrganizationId = organizationId;
        }

        public Guid SettingsId { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public string Value { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public Guid CreatedBy { get; set; }
        public RecordState StateCode { get; set; } = RecordState.Enabled;
        public int StatusCode { get; set; } = 0;
        public Guid OrganizationId { get; set; }
    }
}