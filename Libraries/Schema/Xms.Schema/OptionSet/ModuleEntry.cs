using System;
using Xms.Module.Abstractions;
using Xms.Schema.Abstractions;

namespace Xms.Schema.OptionSet
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
                return OptionSetDefaults.ModuleName;
            }
        }

        public Action<ModuleDescriptor> Configure()
        {
            return (o) =>
            {
                o.Identity = 1;
                o.Name = this.Name;
            };
        }

        public void OnStarting()
        {
            //Dependency.Abstractions.DependencyComponentTypes.Add(this.Name, 8);
            //Solution.Abstractions.SolutionComponentTypes.Add(this.Name, 1);
            Solution.Abstractions.SolutionComponentCollection.Configure((o) =>
            {
                o.Module = Module.Core.ModuleCollection.GetDescriptor(this.Name);
                o.ComponentsEndpoint = "/api/schema/optionset/solutioncomponents";
            });
        }
    }
}