using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xms.Event.Abstractions;

namespace Xms.Authorization.Abstractions
{
    /// <summary>
    /// 权限状态更改事件
    /// </summary>
    [Description("权限状态更改事件")]
    public class AuthorizationStateChangedEvent : IEvent
    {
        public string ResourceName { get; set; }
        public List<Guid> ObjectId { get; set; }
        public bool State { get; set; }
    }
}