using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xms.Plugin.Domain;

namespace Xms.Plugin
{
    public interface IEntityPluginFileProvider
    {
        string GetBaseDirectory();

        bool LoadAssembly(EntityPlugin entity);

        Task<string> Save(IFormFile file);

        string Save(string fileName);

        Task<List<PluginAnalysis>> BeforehandLoad(IFormFile file);

        List<PluginAnalysis> BeforehandLoad(string fileName);
    }
}