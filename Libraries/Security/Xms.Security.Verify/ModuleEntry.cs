using System;
using Xms.Module.Abstractions;

namespace Xms.Security.Verify
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
                return "Security.Verify";
            }
        }

        public Action<ModuleDescriptor> Configure()
        {
            return (o) =>
            {
                o.Identity = 25;
                o.Name = this.Name;
            };
        }

        public void OnStarting()
        {
        }
    }
}