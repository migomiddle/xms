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
using Xms.RibbonButton.Abstractions;
using Xms.Schema.Abstractions;
using Xms.Schema.Entity;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Client;
using Xms.Security.DataAuthorization;
using Xms.Security.Domain;
using Xms.Security.Principal;
using Xms.Security.Resource;
using Xms.Security.Role;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Models;

namespace Xms.Web.Controllers
{
    /// <summary>
    /// 安全角色控制器
    /// </summary>
    public class RoleController : AuthorizedControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IEntityPermissionService _entityPermissionService;
        private readonly IRoleObjectAccessService _roleObjectAccessService;
        private readonly ISystemUserRolesService _systemUserRolesService;
        private readonly IEntityFinder _entityFinder;
        private readonly IResourceOwnerService _resourceOwnerService;
        private readonly IDataFinder _dataFinder;

        public RoleController(IWebAppContext appContext
            , IRoleService roleService
            , IEntityPermissionService entityPermissionService
            , IRoleObjectAccessService roleObjectAccessService
            , ISystemUserRolesService systemUserRolesService
            , IEntityFinder entityFinder
            , IResourceOwnerService resourceOwnerService
            , IDataFinder dataFinder)
            : base(appContext)
        {
            _roleService = roleService;
            _entityPermissionService = entityPermissionService;
            _roleObjectAccessService = roleObjectAccessService;
            _systemUserRolesService = systemUserRolesService;
            _entityFinder = entityFinder;
            _resourceOwnerService = resourceOwnerService;
            _dataFinder = dataFinder;
        }

        [Description("角色列表")]
        public IActionResult Index(RoleModel model)
        {
            FilterContainer<Role> container = FilterContainerBuilder.Build<Role>();
            container.And(n => n.OrganizationId == CurrentUser.OrganizationId);
            if (model.Name.IsNotEmpty())
            {
                container.And(n => n.Name.Like(model.Name));
            }
            if (model.State.HasValue)
            {
                container.And(n => n.StateCode == model.State);
            }
            if (model.GetAll)
            {
                var result = _roleService.Query(x => x
                    .Where(container)
                    .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                );
                model.Items = result;
                model.TotalItems = result.Count;
            }
            else
            {
                if (!model.PageSizeBySeted && CurrentUser.UserSettings.PagingLimit > 0)
                {
                    model.PageSize = CurrentUser.UserSettings.PagingLimit;
                }
                var result = _roleService.QueryPaged(x => x
                    .Page(model.Page, model.PageSize)
                    .Where(container)
                    .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                );
                model.Items = result.Items;
                model.TotalItems = result.TotalItems;
            }
            return DynamicResult(model, "~/Views/Security/Roles.cshtml");
        }

        [HttpGet]
        [Description("角色编辑")]
        public IActionResult EditRole(Guid? id)
        {
            EditRoleModel model = new EditRoleModel();
            model.State = RecordState.Enabled;
            if (id.HasValue && !id.Value.Equals(Guid.Empty))
            {
                var entity = _roleService.FindById(id.Value);
                if (entity != null)
                {
                    entity.CopyTo(model);
                }
            }
            return View($"~/Views/Security/{WebContext.ActionName}.cshtml", model);
        }

        [Description("角色用户")]
        public IActionResult UsersInRole(UsersInRoleModel model)
        {
            FilterContainer<SystemUserRoles> container = FilterContainerBuilder.Build<SystemUserRoles>();
            container.And(n => n.RoleId == model.RoleId);
            var result = _systemUserRolesService.QueryPaged(x => x
                .Page(model.Page, model.PageSize)
                .Where(container)
            //.Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
            );
            model.Items = result.Items;
            model.TotalItems = result.TotalItems;
            return DynamicResult(model, $"~/Views/Security/{WebContext.ActionName}.cshtml");
        }

        [HttpGet]
        [Description("角色字段权限编辑")]
        public IActionResult EditRoleFieldPermissions(Guid roleId)
        {
            if (roleId.IsEmpty())
            {
                return NotFound();
            }
            var resourceOwner = _resourceOwnerService.FindByName(AttributeDefaults.ModuleName);
            if (resourceOwner == null)
            {
                return NotFound();
            }
            EditRolePermissionsModel model = new EditRolePermissionsModel
            {
                ResourceName = AttributeDefaults.ModuleName,
                ResourceOwnerDescriptor = resourceOwner,
                Role = _roleService.FindById(roleId),
                RoleId = roleId,
                RoleObjectAccess = _roleObjectAccessService.QueryRolePermissions(roleId, DataAuthorizationDefaults.ModuleName)
            };
            return DynamicResult(model, $"~/Views/Security/{WebContext.ActionName}.cshtml");
        }

        [HttpGet]
        [Description("角色实体权限编辑")]
        public IActionResult EditRoleEntityPermissions(Guid roleId)
        {
            if (roleId.IsEmpty())
            {
                return NotFound();
            }
            var entityGroups = _dataFinder.RetrieveAll("entitygroup", new List<string> { "name" }, new OrderExpression { AttributeName = "name", OrderType = OrderType.Ascending });
            EditRoleEntityPermissionsModel model = new EditRoleEntityPermissionsModel
            {
                ResourceName = DataAuthorizationDefaults.ModuleName,
                Role = _roleService.FindById(roleId),
                RoleId = roleId,
                RoleObjectAccess = _roleObjectAccessService.QueryRolePermissions(roleId, DataAuthorizationDefaults.ModuleName),

                EntityPermissions = _entityPermissionService.Query(x => x
                        .Sort(n => n.SortAscending(s => s.Name))
                ),
                Entities = _entityFinder.Query(x => x.Where(n => n.AuthorizationEnabled == true)
                        .Sort(n => n.SortAscending(s => s.Name))
                    ),
                EntityGroups = entityGroups
            };
            return DynamicResult(model, $"~/Views/Security/{WebContext.ActionName}.cshtml");
        }

        [HttpGet]
        [Description("角色按钮权限编辑")]
        public IActionResult EditRoleButtonPermissions(Guid roleId)
        {
            if (roleId.IsEmpty())
            {
                return NotFound();
            }
            var resourceOwner = _resourceOwnerService.FindByName(RibbonButtonDefaults.ModuleName);
            if (resourceOwner == null)
            {
                return NotFound();
            }
            EditRoleButtonPermissionsModel model = new EditRoleButtonPermissionsModel
            {
                ResourceName = RibbonButtonDefaults.ModuleName,
                Role = _roleService.FindById(roleId),
                RoleId = roleId,
                RoleObjectAccesses = _roleObjectAccessService.QueryRolePermissions(roleId, RibbonButtonDefaults.ModuleName),
                ResourceOwnerDescriptor = resourceOwner
            };
            return DynamicResult(model, $"~/Views/Security/{WebContext.ActionName}.cshtml");
        }

        [Description("安全角色权限编辑")]
        [HttpGet]
        public IActionResult EditRolePermissions(EditRolePermissionsModel model)
        {
            var resourceOwner = _resourceOwnerService.FindByName(model.ResourceName);
            if (resourceOwner == null)
            {
                return NotFound();
            }
            model.Role = _roleService.FindById(model.RoleId);
            model.ResourceOwnerDescriptor = resourceOwner;
            return DynamicResult(model, $"~/Views/Security/{WebContext.ActionName}.cshtml");
        }
    }
}