using System;
using Xms.Module.Abstractions;
using Xms.RibbonButton.Abstractions;

namespace Xms.RibbonButton
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
                return RibbonButtonDefaults.ModuleName;
            }
        }

        public Action<ModuleDescriptor> Configure()
        {
            return (o) =>
            {
                o.Identity = 16;
                o.Name = this.Name;
            };
        }

        public void OnStarting()
        {
            //Security.Abstractions.ResourceOwnerCollection.Configure((o) => {
            //    o.Module = Module.Core.ModuleCollection.GetDescriptor(this.Name);
            //    o.ResourceEndpoint = "/api/v1/ribbonbutton/privilegeresource";
            //    o.UIEndpoint = "/role/EditRoleButtonPermissions";
            //});
        }
    }
}