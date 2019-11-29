using System;
using Xms.Module.Abstractions;

namespace Xms.Data.Export
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
                return "DataExport";
            }
        }

        public Action<ModuleDescriptor> Configure()
        {
            return (o) =>
            {
                o.Identity = 18;
                o.Name = this.Name;
            };
        }

        public void OnStarting()
        {
        }
    }
}