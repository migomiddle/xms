using System;
using System.Collections.Generic;

namespace Xms.Security.Abstractions
{
    public class PrivilegeResourceItem
    {
        public Guid? Id { get; set; }
        public string Label { get; set; }
        public bool AuthorizationEnabled { get; set; }
        public List<PrivilegeResourceItem> Children { get; set; }
        public Guid? GroupId { get; set; }
    }
}