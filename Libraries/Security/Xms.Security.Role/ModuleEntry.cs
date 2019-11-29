using System;
using Xms.Module.Abstractions;

namespace Xms.Security.Role
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
                return "Role";
            }
        }

        public Action<ModuleDescriptor> Configure()
        {
            return (o) =>
            {
                o.Identity = 15;
                o.Name = this.Name;
            };
        }

        public void OnStarting()
        {
            //Solution.Abstractions.SolutionComponentCollection.Configure((o) => {
            //    o.Module = Module.Core.ModuleCollection.GetDescriptor(this.Name);
            //    o.ComponentsEndpoint = "/api/security/role/solutioncomponents";
            //});
        }
    }
}