using System;
using Xms.Module.Abstractions;

namespace Xms.Business.FormStateRule
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
                return FormStateRuleDefaults.ModuleName;
            }
        }

        public Action<ModuleDescriptor> Configure()
        {
            return (o) =>
            {
                o.Identity = 16;
                o.Name = this.Name;
            };
        }

        public void OnStarting()
        {
            Solution.Abstractions.SolutionComponentCollection.Configure((o) =>
            {
                o.Module = Module.Core.ModuleCollection.GetDescriptor(this.Name);
                o.ComponentsEndpoint = $"/api/{this.Name}/solutioncomponents";
            });
        }
    }
}