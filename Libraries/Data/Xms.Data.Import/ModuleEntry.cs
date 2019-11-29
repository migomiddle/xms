using System;
using Xms.Module.Abstractions;

namespace Xms.Data.Import
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
                return "DataImport";
            }
        }

        public Action<ModuleDescriptor> Configure()
        {
            return (o) =>
            {
                o.Identity = 19;
                o.Name = this.Name;
            };
        }

        public void OnStarting()
        {
        }
    }
}