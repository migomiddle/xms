using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using Xms.Core.Data;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.Organization;
using Xms.Schema.Attribute;
using Xms.Schema.Entity;
using Xms.Sdk.Client;
using Xms.Sdk.Extensions;
using Xms.Security.Principal;
using Xms.SiteMap;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Framework.Mvc;
using Xms.Web.Models;

namespace Xms.Web.Controllers
{
    /// <summary>
    /// 用户设置控制器
    /// </summary>
    public class UserController : AuthorizedControllerBase
    {
        private readonly ISystemUserService _userService;
        private readonly IPrivilegeTreeBuilder _privilegeTreeBuilder;
        private readonly ISystemUserPermissionService _systemUserPermissionService;

        public UserController(IWebAppContext appContext
            , ISystemUserService userService
            , IPrivilegeTreeBuilder privilegeTreeBuilder
            , ISystemUserPermissionService systemUserPermissionService)
            : base(appContext)
        {
            _userService = userService;
            _privilegeTreeBuilder = privilegeTreeBuilder;
            _systemUserPermissionService = systemUserPermissionService;
        }

        [AllowAnonymous]
        [Description("获取用户权限菜单")]
        public IActionResult UserPrivilegesTree()
        {
            if (!CurrentUser.IsSuperAdmin && WebContext.PlatformSettings.ShowMenuInUserPrivileges)
            {
                var result = _systemUserPermissionService.GetPrivileges(CurrentUser.SystemUserId);
                result.RemoveAll(n => n.IsVisibled == false);
                var treeData = _privilegeTreeBuilder.Build(result, Guid.Empty);
                dynamic contact = new ExpandoObject();
                contact.label = T["tree_root"];
                contact.id = Guid.Empty;
                contact.children = treeData;

                List<dynamic> results = new List<dynamic>();
                results.Add(contact);

                return JsonResult(results);
            }
            else
            {
                var result = _privilegeTreeBuilder.Build(x => x
                .Where(f => f.OrganizationId == CurrentUser.OrganizationId && f.IsVisibled == true)
                .Sort(s => s.SortAscending(ss => ss.DisplayOrder))
                );
                return JsonResult(result);
            }
        }

        [Description("更改个人密码")]
        public IActionResult ResetMyPassword()
        {
            ResetMyPasswordModel model = new ResetMyPasswordModel();
            return View(model);
        }

        [Description("更改个人密码")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetMyPassword(ResetMyPasswordModel model)
        {
            string msg = string.Empty;
            bool flag = false;
            if (model.NewPassword.Length < 6 || model.NewPassword.Length > 16)
            {
                ModelState.AddModelError("newpassword", T["user_password_lengthrange"]);
            }
            if (!model.NewPassword.IsCaseInsensitiveEqual(model.ConfirmPassword))
            {
                ModelState.AddModelError("newpassword", T["user_password_notequal"]);
            }
            var user = _userService.FindById(CurrentUser.SystemUserId);
            string password = SecurityHelper.MD5(model.OriginalPassword + user.Salt);
            if (!password.IsCaseInsensitiveEqual(user.Password))
            {
                ModelState.AddModelError("OriginalPassword", T["user_originalpassword_invalid"]);
            }
            if (ModelState.IsValid)
            {
                string newPassword = SecurityHelper.MD5(model.NewPassword + user.Salt);
                bool result = _userService.Update(x => x
                    .Set(n => n.Password, newPassword)
                    .Where(n => n.SystemUserId == CurrentUser.SystemUserId)
                );

                flag = result;
                if (flag)
                {
                    msg = T["updated_success"];
                }
                else
                {
                    msg = T["updated_error"];
                }
                return JsonResult(flag, msg);
            }
            msg = GetModelErrors(ModelState);
            return JsonResult(flag, msg);
        }
    }

    [Route("{org}/user/[action]")]
    public class UserSettingsController : AuthenticatedControllerBase
    {
        private readonly ILanguageService _languageService;
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IDataFinder _dataFinder;
        private readonly IDataUpdater _dataUpdater;

        public UserSettingsController(IWebAppContext appContext
            , ILanguageService languageService
            , IEntityFinder entityFinder
            , IAttributeFinder attributeFinder
            , IDataFinder dataFinder
            , IDataUpdater dataUpdater)
            : base(appContext)
        {
            _languageService = languageService;
            _entityFinder = entityFinder;
            _attributeFinder = attributeFinder;
            _dataFinder = dataFinder;
            _dataUpdater = dataUpdater;
        }

        #region 用户参数

        [Description("设置个人首页")]
        [HttpPost]
        public IActionResult SetUserHomePage(string homePage)
        {
            if (homePage.IsEmpty())
            {
                return JError(T["parameter_error"]);
            }
            Entity entity = new Entity("SystemUserSettings");
            entity.SetIdValue(CurrentUser.SystemUserId)
            .SetAttributeValue("homepagearea", homePage);

            return _dataUpdater.Update(entity).UpdateResult(T);
        }

        [Description("设置个人选项")]
        public IActionResult UserSettings()
        {
            UserSettingsModel model = new UserSettingsModel();
            model.EntityMeta = _entityFinder.FindByName("SystemUserSettings");
            model.AttributesMeta = _attributeFinder.FindByEntityId(model.EntityMeta.EntityId);
            model.Languages = _languageService.Query(n => n.Sort(s => s.SortAscending(f => f.Name)));
            model.EntityDatas = _dataFinder.RetrieveById("SystemUserSettings", CurrentUser.SystemUserId);
            model.SystemUserSettingsId = model.EntityDatas.GetIdValue();
            model.LanguageUniqueId = model.EntityDatas.GetIntValue("languageuniqueid");
            model.PagingLimit = model.EntityDatas.GetIntValue("paginglimit");
            model.CurrencyId = model.EntityDatas.GetGuidValue("TransactionCurrencyId");
            model.EnabledNotification = model.EntityDatas.GetBoolValue("EnabledNotification");
            return View($"~/Views/User/{WebContext.ActionName}.cshtml", model);
        }

        [Description("设置个人选项")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UserSettings(UserSettingsModel model)
        {
            if (ModelState.IsValid)
            {
                Entity entity = new Entity("SystemUserSettings");
                entity.SetIdValue(model.SystemUserSettingsId)
                .SetAttributeValue("paginglimit", model.PagingLimit)
                .SetAttributeValue("languageuniqueid", model.LanguageUniqueId)
                .SetAttributeValue("transactioncurrencyid", model.CurrencyId)
                .SetAttributeValue("EnabledNotification", model.EnabledNotification);

                _dataUpdater.Update(entity);
                if (model.LanguageUniqueId != (int)CurrentUser.UserSettings.LanguageId)
                {
                    WebContext.T.ReFresh();
                }

                return SaveSuccess();
            }

            return SaveFailure(GetModelErrors());
        }

        #endregion 用户参数
    }
}