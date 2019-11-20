using System;

namespace Xms.Core.Org
{
    public interface IOrgDataServer
    {
        Guid OrganizationBaseId { get; set; }
        string UniqueName { get; set; }
        string DataServerName { get; set; }
        string DataAccountName { get; set; }
        string DataPassword { get; set; }
        string DatabaseName { get; set; }
    }
}