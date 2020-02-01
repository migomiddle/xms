using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using Xms.Api.Core.Controller;
using Xms.Api.Org.Models;
using Xms.Core;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.Organization;
using Xms.Schema.Attribute;
using Xms.Schema.Entity;
using Xms.Sdk.Abstractions;
using Xms.Sdk.Client;
using Xms.Sdk.Extensions;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;


namespace Xms.Api.Org.Controllers
{
    /// <summary>
    /// 组织管理删除接口
    /// </summary>
    [Route("{org}/api/org/delete")]
    public class OrgDeleterController : ApiControllerBase
    {
        private readonly IOrganizationService _organizationService;

        public OrgDeleterController(IWebAppContext appContext
            , IOrganizationService organizationService)
            : base(appContext)
        {
            _organizationService = organizationService;
        }

        [Description("删除组织")]
        [HttpPost]
        public IActionResult Post([FromBody]DeleteManyModel model)
        {
            return _organizationService.DeleteById(model.RecordId).DeleteResult(T);
        }


    }

}