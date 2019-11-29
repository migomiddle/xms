using System;
using Xms.Module.Abstractions;

namespace Xms.Notify.Sms
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
                return "Notify.Sms";
            }
        }

        public Action<ModuleDescriptor> Configure()
        {
            return (o) =>
            {
                o.Identity = 27;
                o.Name = this.Name;
            };
        }

        public void OnStarting()
        {
        }
    }
}