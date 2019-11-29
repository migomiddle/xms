using System;
using Xms.Form.Abstractions;
using Xms.Module.Abstractions;

namespace Xms.Form
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
                return FormDefaults.ModuleName;
            }
        }

        public Action<ModuleDescriptor> Configure()
        {
            return (o) =>
            {
                o.Identity = 9;
                o.Name = this.Name;
            };
        }

        public void OnStarting()
        {
            //Dependency.Abstractions.DependencyComponentTypes.Add(this.Name, 9);
            //Solution.Abstractions.SolutionComponentTypes.Add(this.Name, 9);
            Solution.Abstractions.SolutionComponentCollection.Configure((o) =>
            {
                o.Module = Module.Core.ModuleCollection.GetDescriptor(this.Name);
                o.ComponentsEndpoint = "/api/schema/form/solutioncomponents";
            });
            //Security.Abstractions.ResourceOwnerCollection.Configure((o) => {
            //    o.Module = Module.Core.ModuleCollection.GetDescriptor(this.Name);
            //    o.ResourceEndpoint = "/api/schema/form/privilegeresource";
            //});
        }
    }

    public class DashBoardModuleEntry : IModule
    {
        public string Name
        {
            get
            {
                return DashBoardDefaults.ModuleName;
            }
        }

        public Action<ModuleDescriptor> Configure()
        {
            return (o) =>
            {
                o.Identity = 9;
                o.Name = this.Name;
            };
        }

        public void OnStarting()
        {
            //Dependency.Abstractions.DependencyComponentTypes.Add(this.Name, 3);
            //Solution.Abstractions.SolutionComponentTypes.Add(this.Name, 9);
            Solution.Abstractions.SolutionComponentCollection.Configure((o) =>
            {
                o.Module = Module.Core.ModuleCollection.GetDescriptor(this.Name);
                o.ComponentsEndpoint = "/api/dashboard/solutioncomponents";
            });
            //Security.Abstractions.ResourceOwnerCollection.Configure((o) => {
            //    o.Module = Module.Core.ModuleCollection.GetDescriptor(this.Name);
            //    o.ResourceEndpoint = "/api/dashboard/privilegeresource";
            //});
        }
    }
}