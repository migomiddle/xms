using System;
using Xms.Module.Abstractions;
using Xms.WebResource.Abstractions;

namespace Xms.WebResource
{
    /// <summary>
    /// 模块描述
    /// </summary>
    public class ModuleEntry : IModule
    {
        public string Name
        {
            get
            {
                return WebResourceDefaults.ModuleName;
            }
        }

        public Action<ModuleDescriptor> Configure()
        {
            return (o) =>
            {
                o.Identity = 2;
                o.Name = this.Name;
            };
        }

        public void OnStarting()
        {
            //Solution.Abstractions.SolutionComponentTypes.Add(this.Name, 2);
            Solution.Abstractions.SolutionComponentCollection.Configure((o) =>
            {
                o.Module = Module.Core.ModuleCollection.GetDescriptor(this.Name);
                o.ComponentsEndpoint = "/api/webresource/solutioncomponents";
            });
        }
    }
}