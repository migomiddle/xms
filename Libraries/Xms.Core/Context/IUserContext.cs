using System;

namespace Xms.Core.Context
{
    public interface IUserContext
    {
        Guid BusinessUnitId { get; set; }
        string BusinessUnitIdName { get; set; }
        bool IsSuperAdmin { get; set; }
        string LoginName { get; set; }
        Guid OrganizationId { get; set; }
        Guid SystemUserId { get; set; }
        string UserName { get; set; }
    }
}