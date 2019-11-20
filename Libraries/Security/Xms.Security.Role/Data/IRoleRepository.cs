using System;
using Xms.Core.Data;

namespace Xms.Security.Role.Data
{
    public interface IRoleRepository : IRepository<Domain.Role>
    {
        string GetRolesXml(Guid solutionId);
    }
}