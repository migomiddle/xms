using System;
using Xms.Module.Abstractions;
using Xms.Schema.Abstractions;

namespace Xms.Schema.StringMap
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
                return StringMapDefaults.ModuleName;
            }
        }

        public Action<ModuleDescriptor> Configure()
        {
            return (o) =>
            {
                o.Identity = 36;
                o.Name = this.Name;
            };
        }

        public void OnStarting()
        {
        }
    }
}