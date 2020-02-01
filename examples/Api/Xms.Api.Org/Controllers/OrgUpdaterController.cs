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
    /// 组织管理控更新接口
    /// </summary>
    [Route("{org}/api/org/update")]
    public class OrgUpdaterController : ApiControllerBase
    {
        private readonly IOrganizationService _organizationService;
        private readonly IDataUpdater _dataUpdater;

        public OrgUpdaterController(IWebAppContext appContext
            , IOrganizationService organizationService
            , IDataUpdater dataUpdater)
            : base(appContext)
        {
            _organizationService = organizationService;
            _dataUpdater = dataUpdater;
        }

        [HttpPost]        
        [Description("组织信息保存")]
        public IActionResult Post(EditOrganizationModel model)
        {
            string msg = string.Empty;
            if (ModelState.IsValid)
            {
                Entity entity = new Entity("organization");
                entity.SetIdValue(model.OrganizationId.Value);
                entity.SetAttributeValue("Name", model.Name);
                entity.SetAttributeValue("LanguageId", (int)model.LanguageId);
                //entity.SetAttributeValue("State", model.State);
                entity.SetAttributeValue("Description", model.Description);
                entity.SetAttributeValue("BaseCurrencyId", new EntityReference("currency", model.BaseCurrencyId));
                entity.SetAttributeValue("ManagerId", model.ManagerId);
                _dataUpdater.Update(entity);
                return JOk(T["saved_success"]);
            }
            msg = GetModelErrors(ModelState);
            return JError(T["saved_error"] + ": " + msg);
        }

        [Description("设置组织可用状态")]
        [HttpPost("setstate")]
        public IActionResult Post(SetRecordStateModel model)
        {
            return _organizationService.Update(x => x.Set(n => n.StatusCode, model.IsEnabled ? RecordState.Enabled : (int)RecordState.Disabled)
                .Where(n => n.OrganizationId.In(model.RecordId))
                ).UpdateResult(T);
        }





    }

}