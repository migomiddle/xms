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
    /// 组织管理创建接口
    /// </summary>
    [Route("{org}/api/org/create")]
    public class OrgCreaterController : ApiControllerBase
    {
        private readonly IDataCreater _dataCreater;
        private readonly ILanguageService _languageService;
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;

        public OrgCreaterController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IAttributeFinder attributeFinder            
            , ILanguageService languageService            
            , IDataCreater dataCreater)
            : base(appContext)
        {
            _dataCreater = dataCreater;
            _entityFinder = entityFinder;
            _attributeFinder = attributeFinder;            
            _languageService = languageService;            
        }

        [HttpGet]
        [Description("新建组织")]
        public IActionResult Get()
        {
            EditOrganizationModel model = new EditOrganizationModel();
            model.EntityMeta = _entityFinder.FindByName("organization");
            model.Attributes = _attributeFinder.FindByEntityId(model.EntityMeta.EntityId);
            model.StatusCode = (int)RecordState.Enabled;
            model.LanguageList = _languageService.Query(n => n.Sort(s => s.SortAscending(f => f.UniqueId)));
            model.Datas = new Entity("organization");
            return JOk(model);
        }

        [HttpPost]        
        [Description("新建组织-保存")]
        public IActionResult Post(EditOrganizationModel model)
        {
            string msg = string.Empty;
            if (ModelState.IsValid)
            {
                Entity entity = new Entity("organization");
                entity.SetIdValue(Guid.NewGuid());
                entity.SetAttributeValue("Name", model.Name);
                entity.SetAttributeValue("LanguageId", (int)model.LanguageId);
                entity.SetAttributeValue("State", model.StatusCode);
                entity.SetAttributeValue("Description", model.Description);
                entity.SetAttributeValue("BaseCurrencyId", new EntityReference("currency", model.BaseCurrencyId));
                entity.SetAttributeValue("ManagerId", model.ManagerId);
                var id = _dataCreater.Create(entity);
                return JOk(T["created_success"], new { id = id });
            }
            msg = GetModelErrors(ModelState);
            return JOk(T["created_error"] + ": " + msg);
        }


    }

}