using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xms.Plugin.Domain;

namespace Xms.Plugin
{
    public interface IEntityPluginUpdater
    {
        Task<bool> Update(EntityPlugin entity, IFormFile file);

        bool Update(EntityPlugin entity);

        bool UpdateState(IEnumerable<Guid> ids, bool isEnabled);
    }
}