using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xms.Authorization.Abstractions;
using Xms.EntityData.Api.Models;
using Xms.Infrastructure.Utility;
using Xms.Sdk.Client;
using Xms.Security.Domain;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;

namespace Xms.EntityData.Api
{
    /// <summary>
    /// 实体数据共享接口
    /// </summary>
    [Route("{org}/api/data/share")]
    public class DataShareController : ApiControllerBase
    {
        private readonly IPrincipalObjectAccessService _principalObjectAccessService;
        private readonly IDataSharer _dataSharer;

        public DataShareController(IWebAppContext appContext
            , IPrincipalObjectAccessService principalObjectAccessService
            , IDataSharer dataSharer)
            : base(appContext)
        {
            _principalObjectAccessService = principalObjectAccessService;
            _dataSharer = dataSharer;
        }
        
        [Description("共享对象列表")]
        [HttpGet("SharedPrincipals")]
        public IActionResult SharedPrincipals(SharedPrincipalsModel model)
        {
            var result = _principalObjectAccessService.Query(n => n.Where(f => f.EntityId == model.EntityId && f.ObjectId == model.ObjectId));

            return JOk(result);
        }

        [Description("共享记录")]
        [HttpPost]
        public IActionResult Post(SharedModel model)
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
    }
}