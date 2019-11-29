using System;
using Xms.Module.Abstractions;

namespace Xms.Configuration
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
                return "Configuration";
            }
        }

        public Action<ModuleDescriptor> Configure()
        {
            return (o) =>
            {
                o.Identity = 17;
                o.Name = this.Name;
            };
        }

        public void OnStarting()
        {
        }
    }
}