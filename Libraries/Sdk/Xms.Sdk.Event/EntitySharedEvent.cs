using System.Collections.Generic;
using System.ComponentModel;
using Xms.Core.Data;
using Xms.Event.Abstractions;
using Xms.Security.Domain;

namespace Xms.Sdk.Event
{
    [Description("实体共享后事件")]
    public class EntitySharedEvent : IEvent
    {
        public Entity Data { get; set; }
        public Schema.Domain.Entity EntityMetadata { get; set; }
        public List<PrincipalObjectAccess> Principals { get; set; }
    }
}