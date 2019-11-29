using System;
using Xms.Module.Abstractions;

namespace Xms.Solution
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
                return "Solution";
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