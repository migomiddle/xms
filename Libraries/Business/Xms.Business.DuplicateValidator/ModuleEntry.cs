using System;
using Xms.Module.Abstractions;

namespace Xms.Business.DuplicateValidator
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
                return DuplicateRuleDefaults.ModuleName;
            }
        }

        public Action<ModuleDescriptor> Configure()
        {
            return (o) =>
            {
                o.Identity = 11;
                o.Name = this.Name;
            };
        }

        public void OnStarting()
        {
            //Dependency.Abstractions.DependencyComponentTypes.Add(this.Name, 4);
            //Solution.Abstractions.SolutionComponentTypes.Add(this.Name, 11);
            Solution.Abstractions.SolutionComponentCollection.Configure((o) =>
            {
                o.Module = Module.Core.ModuleCollection.GetDescriptor(this.Name);
                o.ComponentsEndpoint = $"/api/{this.Name}/solutioncomponents";
            });
        }
    }
}