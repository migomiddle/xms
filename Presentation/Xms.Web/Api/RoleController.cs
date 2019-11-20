using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xms.Authorization.Abstractions;
using Xms.Core;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Security.Abstractions;
using Xms.Security.Domain;
using Xms.Security.Resource;
using Xms.Security.Role;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;
using Xms.Web.Models;

namespace Xms.Web.Api
{
    /// <summary>
    /// 安全角色接口
    /// </summary>
    [Route("{org}/api/role")]
    public class RoleController : ApiControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IRoleObjectAccessService _roleObjectAccessService;
        private readonly IResourceOwnerService _resourceOwnerService;

        public RoleController(IWebAppContext appContext
            , IRoleService roleService
            , IRoleObjectAccessService roleObjectAccessService
            , IResourceOwnerService resourceOwnerService
            )
            : base(appContext)
        {
            _roleService = roleService;
            _roleObjectAccessService = roleObjectAccessService;
            _resourceOwnerService = resourceOwnerService;
        }

        /// <summary>
        /// 查询所有安全角色
        /// </summary>
        /// <returns></returns>
        [Description("查询所有安全角色")]
        [HttpGet("getall")]
        public IActionResult GetAll([FromQuery]RoleModel model)
        {
            FilterContainer<Role> filter = FilterContainerBuilder.Build<Role>();
            filter.And(n => n.OrganizationId == CurrentUser.OrganizationId);
            if (model.Name.IsNotEmpty())
            {
                filter.And(n => n.Name.Like(model.Name));
            }
            if (model.State.HasValue)
            {
                filter.And(n => n.StateCode == model.State.Value);
            }
            var result = _roleService.QueryPaged(x => x.Page(model.Page, model.PageSize)
            .Where(filter)
                    .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection)));
            return JOk(result);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Description("角色编辑")]
        public IActionResult Post([FromBody]EditRoleModel model)
        {
            if (ModelState.IsValid)
            {
                Role entity = new Role();

                if (model.RoleId.HasValue && !model.RoleId.Value.Equals(Guid.Empty))
                {
                    entity = _roleService.FindById(model.RoleId.Value);
                    entity.Description = model.Description;
                    entity.ModifiedBy = CurrentUser.SystemUserId;
                    entity.ModifiedOn = DateTime.Now;
                    if (!entity.Name.IsCaseInsensitiveEqual("administrator"))
                    {
                        entity.Name = model.Name;
                        entity.StateCode = model.State;
                    }
                    return _roleService.Update(entity).UpdateResult(T);
                }
                else
                {
                    if (model.Name.IsCaseInsensitiveEqual(RoleDefaults.ADMINISTRATOR))
                    {
                        return JError(T["role_name_disabled"]);
                    }
                    else
                    {
                        model.CopyTo(entity);
                        entity.RoleId = Guid.NewGuid();
                        entity.OrganizationId = CurrentUser.OrganizationId;
                        entity.CreatedBy = CurrentUser.SystemUserId;
                        entity.StateCode = RecordState.Enabled;
                        return _roleService.Create(entity).CreateResult(T);
                    }
                }
            }
            return SaveFailure(GetModelErrors());
        }

        [Description("安全角色权限")]
        [HttpGet("Permissions")]
        public IActionResult Permissions(Guid roleId, string resourceName)
        {
            return JOk(_roleObjectAccessService.QueryRolePermissions(roleId, resourceName));
        }

        /// <summary>
        /// 安全角色权限保存
        /// </summary>
        /// <returns></returns>
        [Description("安全角色权限保存")]
        [HttpPost("SaveRolePermissions")]
        public IActionResult SaveRolePermissions([FromBody]EditRolePermissionsModel model)
        {
            var resourceOwner = _resourceOwnerService.FindByName(model.ResourceName);
            if (resourceOwner == null)
            {
                return NotFound();
            }
            var Role = _roleService.FindById(model.RoleId);
            if (Role.Name.IsCaseInsensitiveEqual(RoleDefaults.ADMINISTRATOR))
            {
                return JError(T["notallow_edit"]);
            }
            if (ModelState.IsValid)
            {
                _roleObjectAccessService.DeleteByRole(model.RoleId, resourceOwner.ModuleName);
                if (model.ObjectId.NotEmpty())
                {
                    List<RoleObjectAccess> roleObjectAccess = new List<RoleObjectAccess>();
                    var objectTypeCode = Module.Core.ModuleCollection.GetIdentity(resourceOwner.ModuleName);
                    int i = 0;
                    foreach (var item in model.ObjectId)
                    {
                        var roa = new RoleObjectAccess
                        {
                            RoleObjectAccessId = Guid.NewGuid(),
                            RoleId = model.RoleId,
                            ObjectId = item,
                            ObjectTypeCode = objectTypeCode
                        };
                        if (model.Mask == null)
                        {
                            roa.AccessRightsMask = 1;
                        }
                        else if (model.Mask != null && model.Mask[i] > 0)
                        {
                            roa.AccessRightsMask = (int)model.Mask[i];
                        }
                        if (roa.AccessRightsMask > 0 && !roleObjectAccess.Exists(x => x.ObjectId == item && x.ObjectTypeCode == objectTypeCode))
                        {
                            roleObjectAccess.Add(roa);
                        }
                        i++;
                    }

                    if (roleObjectAccess.NotEmpty())
                    {
                        _roleObjectAccessService.CreateMany(roleObjectAccess);
                    }
                }
                return SaveSuccess();
            }
            return SaveFailure(GetModelErrors());
        }

        /// <summary>
        /// 复制角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [Description("复制角色")]
        [HttpPost("CopyRole")]
        public IActionResult CopyRole(Guid roleId, string name)
        {
            var role = _roleService.FindById(roleId);
            if (role != null)
            {
                var newRole = new Role
                {
                    CreatedBy = CurrentUser.SystemUserId,
                    Name = name,
                    OrganizationId = CurrentUser.OrganizationId,
                    RoleId = Guid.NewGuid(),
                    StateCode = RecordState.Enabled
                };
                var flag = _roleService.Create(newRole);
                if (flag)
                {
                    var roaList = _roleObjectAccessService.Query(x => x.Where(f => f.RoleId == roleId));
                    if (roaList.NotEmpty())
                    {
                        roaList.ForEach((o) =>
                        {
                            o.RoleObjectAccessId = Guid.NewGuid();
                            o.RoleId = newRole.RoleId;
                        });
                        _roleObjectAccessService.CreateMany(roaList);
                    }
                    return SaveSuccess();
                }
            }
            return SaveFailure();
        }

        [Description("删除角色")]
        [HttpDelete]
        public IActionResult Delete([FromBody]DeleteManyModel model)
        {
            return _roleService.DeleteById(model.RecordId).DeleteResult(T);
        }

        [Description("设置安全角色可用状态")]
        [HttpPost("State")]
        public IActionResult SetRoleState([FromBody]SetRecordStateModel model)
        {
            return _roleService.Update(x => x.Set(n => n.StateCode, model.IsEnabled ? RecordState.Enabled : (int)RecordState.Disabled)
                .Where(n => n.RoleId.In(model.RecordId))
                ).UpdateResult(T);
        }
    }
}