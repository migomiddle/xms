using System.Collections.Generic;
using System.ComponentModel;
using Xms.Core.Data;
using Xms.Event.Abstractions;
using Xms.Security.Domain;

namespace Xms.Sdk.Event
{
    [Description("实体共享前事件")]
    public class EntitySharingEvent : IEvent
    {
        public Entity Data { get; set; }
        public Schema.Domain.Entity EntityMetadata { get; set; }
        public List<PrincipalObjectAccess> Principals { get; set; }
    }
}