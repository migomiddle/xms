using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xms.Authorization.Abstractions;
using Xms.Infrastructure.Utility;
using Xms.Sdk.Client;
using Xms.Security.Domain;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Models;

namespace Xms.Web.Controllers
{
    /// <summary>
    /// 实体数据共享控制器
    /// </summary>
    [Route("{org}/entity/[action]")]
    public class EntitySharingController : AuthenticatedControllerBase
    {
        private readonly IPrincipalObjectAccessService _principalObjectAccessService;
        private readonly IDataSharer _dataSharer;

        public EntitySharingController(IWebAppContext appContext
            , IPrincipalObjectAccessService principalObjectAccessService
            , IDataSharer dataSharer)
            : base(appContext)
        {
            _principalObjectAccessService = principalObjectAccessService;
            _dataSharer = dataSharer;
        }

        #region 共享

        [Description("共享对象列表")]
        public IActionResult SharedPrincipals(SharedPrincipalsModel model)
        {
            var result = _principalObjectAccessService.Query(n => n.Where(f => f.EntityId == model.EntityId && f.ObjectId == model.ObjectId));

            return DynamicResult(result);
        }

        [Description("共享记录")]
        public IActionResult Sharing(SharedModel model)
        {
            model.Principals = _principalObjectAccessService.Query(n => n.Where(f => f.EntityId == model.EntityId && f.ObjectId == model.ObjectId));
            return View($"~/Views/Entity/{WebContext.ActionName}.cshtml", model);
        }

        [HttpPost]
        [Description("共享记录")]
        [ValidateAntiForgeryToken]
        public IActionResult Shared(SharedModel model)
        {
            if (model.EntityId.Equals(Guid.Empty) || model.ObjectId.Equals(Guid.Empty))
            {
                return JError(T["parameter_error"]);
            }
            List<PrincipalObjectAccess> list = null;
            if (model.PrincipalsJson.IsNotEmpty())
            {
                list = list.DeserializeFromJson(model.PrincipalsJson.UrlDecode());
            }
            var flag = _dataSharer.Share(model.EntityId, model.ObjectId, list);
            if (flag)
            {
                return JOk(T["operation_success"]);
            }
            return JError(T["operation_error"]);
        }

        #endregion 共享
    }
}