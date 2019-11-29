using System;
using Xms.Module.Abstractions;
using Xms.Schema.Abstractions;

namespace Xms.Schema.Attribute
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
                return AttributeDefaults.ModuleName;
            }
        }

        public Action<ModuleDescriptor> Configure()
        {
            return (o) =>
            {
                o.Identity = 1;
                o.Name = this.Name;
            };
        }

        public void OnStarting()
        {
            //Security.Abstractions.ResourceOwnerCollection.Configure((o) => {
            //    o.Module = Module.Core.ModuleCollection.GetDescriptor(this.Name);
            //    o.ResourceEndpoint = $"/api/schema/attribute/privilegeresource";
            //});
        }
    }
}