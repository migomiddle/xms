using System;
using Xms.Module.Abstractions;

namespace Xms.Logging.DataLog
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
                return "DataLog";
            }
        }

        public Action<ModuleDescriptor> Configure()
        {
            return (o) =>
            {
                o.Identity = 24;
                o.Name = this.Name;
            };
        }

        public void OnStarting()
        {
        }
    }
}