using System;
using Xms.Module.Abstractions;
using Xms.QueryView.Abstractions;

namespace Xms.QueryView
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
                return QueryViewDefaults.ModuleName;
            }
        }

        public Action<ModuleDescriptor> Configure()
        {
            return (o) =>
            {
                o.Identity = 8;
                o.Name = this.Name;
            };
        }

        public void OnStarting()
        {
            Solution.Abstractions.SolutionComponentCollection.Configure((o) =>
            {
                o.Module = Module.Core.ModuleCollection.GetDescriptor(this.Name);
                o.ComponentsEndpoint = "/api/schema/queryview/solutioncomponents";
            });
        }
    }
}