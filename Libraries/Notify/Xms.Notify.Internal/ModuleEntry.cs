using System;
using Xms.Module.Abstractions;

namespace Xms.Notify.Internal
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
                return "Notify.Internal";
            }
        }

        public Action<ModuleDescriptor> Configure()
        {
            return (o) =>
            {
                o.Identity = 26;
                o.Name = this.Name;
            };
        }

        public void OnStarting()
        {
        }
    }
}