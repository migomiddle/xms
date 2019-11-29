using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xms.Plugin.Domain;

namespace Xms.Plugin
{
    public interface IEntityPluginCreater
    {
        bool Create(EntityPlugin entity);

        Task<bool> Create(EntityPlugin entity, IFormFile file);

        bool Create(EntityPlugin entity, string fileName);

        Task<List<PluginAnalysis>> BeforehandLoad(IFormFile file);

        List<PluginAnalysis> BeforehandLoad(string fileName);
    }
}