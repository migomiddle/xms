using System;
using Xms.DataMapping.Abstractions;
using Xms.Module.Abstractions;

namespace Xms.DataMapping
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
                return DataMappingDefaults.ModuleName;
            }
        }

        public Action<ModuleDescriptor> Configure()
        {
            return (o) =>
            {
                o.Identity = 13;
                o.Name = this.Name;
            };
        }

        public void OnStarting()
        {
            Solution.Abstractions.SolutionComponentCollection.Configure((o) =>
            {
                o.Module = Module.Core.ModuleCollection.GetDescriptor(this.Name);
                o.ComponentsEndpoint = "/api/entitymap/solutioncomponents";
            });
        }
    }
}