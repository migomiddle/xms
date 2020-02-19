using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xms.Authorization.Abstractions;
using Xms.Core.Data;
using Xms.Infrastructure.Utility;
using Xms.Organization;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Client;
using Xms.Sdk.Extensions;
using Xms.Security.Abstractions;
using Xms.Security.Domain;
using Xms.Security.Principal;
using Xms.Security.Resource;
using Xms.Security.Role;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Framework.Infrastructure;
using Xms.Web.Framework.Mvc;
using Xms.Web.Models;

namespace Xms.Web.Controllers
{
    /// <summary>
    /// 安全管理控制器
    /// </summary>
    public class SecurityController : AuthorizedControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ISystemUserService _userService;
        private readonly ISystemUserRolesService _systemUserRolesService;
        private readonly IRoleObjectAccessService _roleObjectAccessService;
        private readonly IDataCreater _dataCreater;
        private readonly IDataFinder _dataFinder;
        private readonly IDataDeleter _dataDeleter;
        private readonly IResourceOwnerService _resourceOwnerService;

        public SecurityController(IWebAppContext appContext
            , ISystemUserService userService
            , ISystemUserRolesService systemUserRolesService
            , IRoleService roleService
            , IRoleObjectAccessService roleObjectAccessService
            , IDataCreater dataCreater
            , IDataFinder dataFinder
            , IDataDeleter dataDeleter
            , IResourceOwnerService resourceOwnerService)
            : base(appContext)
        {
            _roleService = roleService;
            _userService = userService;
            _systemUserRolesService = systemUserRolesService;
            _roleObjectAccessService = roleObjectAccessService;
            _dataCreater = dataCreater;
            _dataFinder = dataFinder;
            _dataDeleter = dataDeleter;
            _resourceOwnerService = resourceOwnerService;
        }

        #region 用户权限

        /// <summary>
        /// 重设用户角色
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        [Description("重设用户角色")]
        [HttpPost]
        public IActionResult AssignRolesToUser([FromBody]AddUserRolesModel model)
        {
            foreach (var rid in model.RoleId)
            {
                var role = _roleService.FindById(rid);
                if (role.Name.IsCaseInsensitiveEqual(RoleDefaults.ADMINISTRATOR))
                {
                    return JError(T["notallow_edit"]);
                }
            }
            if (model.UserId.IsEmpty())
            {
                return NotSpecifiedRecord();
            }
            foreach (var uid in model.UserId)
            {
                var userRoles = new List<SystemUserRoles>();
                foreach (var rid in model.RoleId)
                {
                    var entity = new SystemUserRoles
                    {
                        SystemUserId = uid,
                        RoleId = rid,
                        SystemUserRoleId = Guid.NewGuid()
                    };
                    userRoles.Add(entity);
                }
                _systemUserRolesService.UpdateUserRoles(uid, userRoles);
            }
            return SaveSuccess();
        }

        [Description("添加用户角色")]
        [HttpPost]
        public IActionResult AddUserRoles([FromBody]AddUserRolesModel model)
        {
            if (model.UserId.IsEmpty() || model.RoleId.IsEmpty())
            {
                return NotSpecifiedRecord();
            }
            var userRoles = new List<SystemUserRoles>();
            var existsUserRoles = _systemUserRolesService.Query(x => x.Where(f => f.SystemUserId.In(model.UserId) && f.RoleId.In(model.RoleId)));
            foreach (var uid in model.UserId)
            {
                foreach (var rid in model.RoleId)
                {
                    if (existsUserRoles.Exists(x => x.RoleId == rid && x.SystemUserId == uid))
                    {
                        continue;
                    }
                    var entity = new SystemUserRoles
                    {
                        SystemUserId = uid,
                        RoleId = rid,
                        SystemUserRoleId = Guid.NewGuid()
                    };
                    userRoles.Add(entity);
                }
            }
            if (userRoles.NotEmpty())
            {
                _systemUserRolesService.CreateMany(userRoles);
            }
            return SaveSuccess();
        }

        [Description("移除用户角色")]
        [HttpPost]
        public IActionResult RemoveUserRoles([FromBody]RemoveUserRolesModel model)
        {
            bool flag = false;
            foreach (var uid in model.UserId)
            {
                flag = _systemUserRolesService.DeleteById(uid, model.RoleId);
            }

            return flag.DeleteResult(T);
        }

        #endregion 用户权限

        #region 团队权限

        [AllowAnonymous]
        [Description("设置团队安全角色")]
        public IActionResult AssignRolesToTeam(Guid[] teamId, Guid[] roleId)
        {
            if (!Arguments.HasValue(teamId) || !Arguments.HasValue(roleId))
            {
                return NotSpecifiedRecord();
            }
            foreach (var tid in teamId)
            {
                List<Guid> needDeleted = new List<Guid>();
                var query = new QueryExpression("TeamRoles", CurrentUser.UserSettings.LanguageId);
                query.ColumnSet.AddColumns("teamid", "roleid");
                query.Criteria.AddCondition("teamid", ConditionOperator.Equal, tid);
                var datas = _dataFinder.RetrieveAll(query);
                var addEntities = new List<Entity>();
                foreach (var item in roleId)
                {
                    if (!datas.Any(n => n.GetGuidValue("roleid") == item))
                    {
                        Entity entity = new Entity("TeamRoles");
                        entity.SetAttributeValue("teamid", teamId);
                        entity.SetAttributeValue("roleid", item);
                        addEntities.Add(entity);
                    }
                }
                var b = datas.Where(n => !roleId.Contains(n.GetGuidValue("roleid"))).Select(n => n.GetGuidValue("teamroleid")).ToList();
                needDeleted.AddRange(b);
                if (needDeleted.NotEmpty())
                {
                    //delete
                    _dataDeleter.Delete("teamroles", needDeleted);
                }
                if (addEntities.NotEmpty())
                {
                    _dataCreater.CreateMany(addEntities);
                }
            }
            return SaveSuccess();
        }

        #endregion 团队权限

        #region 对象权限

        [Description("设置对象角色权限")]
        [HttpPost]
        public IActionResult AssigningObjectRoles([FromBody]RoleObjectAccessListModel model)
        {
            int typeCode = Module.Core.ModuleCollection.GetIdentity(model.ObjectTypeName);
            model.Roles = _roleService.Query(n => n.Where(f => f.OrganizationId == CurrentUser.OrganizationId && f.StateCode == Core.RecordState.Enabled).Sort(s => s.SortAscending(f => f.Name)));
            model.Accesses = _roleObjectAccessService.Query(n => n.Where(f => f.ObjectId == model.ObjectId && f.ObjectTypeCode == typeCode));
            return View(model);
        }

        [Description("保存对象角色权限")]
        [HttpPost]
        public IActionResult AssignedObjectRoles([FromBody]RoleObjectAccessModel model)
        {
            if (model.ObjectId.Equals(Guid.Empty))
            {
                return NotSpecifiedRecord();
            }
            _roleObjectAccessService.DeleteByObjectId(model.ObjectId, model.ObjectTypeName);
            if (model.EnabledAuthorization && model.AssignRoleId.NotEmpty())
            {
                if (model.AssignRoleId.NotEmpty())
                {
                    _roleObjectAccessService.CreateMany(model.ObjectTypeName, model.ObjectId, model.AssignRoleId.ToArray());
                }
            }
            return SaveSuccess();
        }

        #endregion 对象权限

        #region 用户密码

        [HttpGet]
        [Description("用户密码修改")]
        public IActionResult EditUserPassword(Guid id)
        {
            EditUserPasswordModel model = new EditUserPasswordModel();
            var entity = _userService.FindById(id);
            if (entity != null)
            {
                model.SystemUserId = id;
                model.Name = entity.Name;
                model.NewPassword = string.Empty;
            }
            else
            {
                return NotFound();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("用户密码修改")]
        public IActionResult EditUserPassword(EditUserPasswordModel model)
        {
            if (model.NewPassword.Length < 6 || model.NewPassword.Length > 16)
            {
                ModelState.AddModelError("newpassword", T["user_password_lengthrange"]);
            }
            if (!model.NewPassword.IsCaseInsensitiveEqual(model.ConfirmPassword))
            {
                ModelState.AddModelError("newpassword", T["user_password_notequal"]);
            }
            if (ModelState.IsValid)
            {
                var user = _userService.FindById(model.SystemUserId);
                string password = SecurityHelper.MD5(model.NewPassword + user.Salt);
                bool result = _userService.Update(x => x
                    .Set(n => n.Password, password)
                    .Where(n => n.SystemUserId == model.SystemUserId)
                );
                return result.UpdateResult(T);
            }
            return UpdateFailure(GetModelErrors());
        }

        #endregion 用户密码

        #region 对话框

        [Description("角色对话框")]
        public IActionResult RolesDialog([FromBody]RoleDialogModel model)
        {
            if (model.UserId.NotEmpty() && model.UserId.Count == 1)
            {
                var id = model.UserId.First();
                QueryExpression query = new QueryExpression("SystemUserRoles");
                query.AddColumns("roleid");
                query.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, id);
                var userRolesData = _dataFinder.RetrieveAll(query);
                if (userRolesData.NotEmpty())
                {
                    model.SelectedRoles = userRolesData.Select(x => x.GetGuidValue("roleid")).ToList();
                }
            }
            if (model.TeamId.NotEmpty() && model.TeamId.Count == 1)
            {
                var id = model.TeamId.First();
                QueryExpression query = new QueryExpression("TeamRoles");
                query.AddColumns("roleid");
                query.Criteria.AddCondition("teamid", ConditionOperator.Equal, id);
                var userRolesData = _dataFinder.RetrieveAll(query);
                if (userRolesData.NotEmpty())
                {
                    model.SelectedRoles = userRolesData.Select(x => x.GetGuidValue("roleid")).ToList();
                }
            }
            var result = _roleService.QueryPaged(x => x
                .Where(n => n.OrganizationId == CurrentUser.OrganizationId)
                .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
            );
            model.Items = result.Items;
            model.TotalItems = result.TotalItems;
            return View(model);
        }

        #endregion 对话框

        [Description("权限资源管理")]
        public IActionResult PrivilegeResources(string resourceName)
        {
            if (resourceName.IsEmpty())
            {
                return NotFound();
            }
            var resourceOwner = _resourceOwnerService.FindByName(resourceName);
            if (resourceOwner == null)
            {
                return NotFound();
            }
            PrivilegeResourceModel model = new PrivilegeResourceModel
            {
                ResourceName = resourceName
                ,
                ResourceOwnerDescriptor = resourceOwner
            };
            return View(model);
        }
    }
}