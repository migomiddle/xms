using System;
using Xms.Module.Abstractions;

namespace Xms.Security.Resource
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
                return "Security.Resource";
            }
        }

        public Action<ModuleDescriptor> Configure()
        {
            return (o) =>
            {
                o.Identity = 33;
                o.Name = this.Name;
            };
        }

        public void OnStarting()
        {
        }
    }
}