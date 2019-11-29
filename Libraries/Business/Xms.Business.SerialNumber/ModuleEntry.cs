using System;
using Xms.Module.Abstractions;

namespace Xms.Business.SerialNumber
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
                return SerialNumberRuleDefaults.ModuleName;
            }
        }

        public Action<ModuleDescriptor> Configure()
        {
            return (o) =>
            {
                o.Identity = 7;
                o.Name = this.Name;
            };
        }

        public void OnStarting()
        {
            //Dependency.Abstractions.DependencyComponentTypes.Add(this.Name, 6);
            Solution.Abstractions.SolutionComponentCollection.Configure((o) =>
            {
                o.Module = Module.Core.ModuleCollection.GetDescriptor(this.Name);
                o.ComponentsEndpoint = "/api/serialnumberrule/solutioncomponents";
            });
        }
    }
}