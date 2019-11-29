using System;
using Xms.Localization.Abstractions;
using Xms.Module.Abstractions;

namespace Xms.Localization
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
                return LocalizationDefaults.ModuleName;
            }
        }

        public Action<ModuleDescriptor> Configure()
        {
            return (o) =>
            {
                o.Identity = 22;
                o.Name = this.Name;
            };
        }

        public void OnStarting()
        {
        }
    }
}