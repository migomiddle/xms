using System;
using Xms.Module.Abstractions;

namespace Xms.Security.DataAuthorization
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
                return DataAuthorizationDefaults.ModuleName;
            }
        }

        public Action<ModuleDescriptor> Configure()
        {
            return (o) =>
            {
                o.Identity = 8;
                o.Name = this.Name;
            };
        }

        public void OnStarting()
        {
            //Security.Abstractions.ResourceOwnerCollection.Configure((o) => {
            //    o.Module = Module.Core.ModuleCollection.GetDescriptor(this.Name);
            //    o.ResourceEndpoint = $"/api/v1/{this.Name}/privilegeresource";
            //    o.UIEndpoint = "/role/EditRoleEntityPermissions";
            //});
        }
    }
}