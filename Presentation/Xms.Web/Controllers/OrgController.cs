using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
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
using Xms.Web.Framework.Controller;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;
using Xms.Web.Models;

namespace Xms.Web.Controllers
{
    /// <summary>
    /// 组织管理控制器
    /// </summary>
    public class OrgController : WebControllerBase
    {
        private readonly IOrganizationService _organizationService;
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly ILanguageService _languageService;
        private readonly IDataFinder _dataFinder;
        private readonly IDataUpdater _dataUpdater;
        private readonly IDataCreater _dataCreater;

        public OrgController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IAttributeFinder attributeFinder
            , IOrganizationService organizationService
            , ILanguageService languageService
            , IDataFinder dataFinder
            , IDataUpdater dataUpdater
            , IDataCreater dataCreater)
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _attributeFinder = attributeFinder;
            _organizationService = organizationService;
            _languageService = languageService;
            _dataFinder = dataFinder;
            _dataUpdater = dataUpdater;
            _dataCreater = dataCreater;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Organizations");
        }

        #region 组织

        [Description("组织列表")]
        public IActionResult Organizations(OrganizationModel model)
        {
            model.EntityMeta = _entityFinder.FindByName("organization");
            model.Attributes = _attributeFinder.FindByEntityId(model.EntityMeta.EntityId);
            FilterContainer<Organization.Domain.Organization> container = FilterContainerBuilder.Build<Organization.Domain.Organization>();
            if (model.Name.IsNotEmpty())
            {
                container.And(n => n.Name.Like(model.Name));
            }
            if (model.Name.IsNotEmpty())
            {
                container.And(n => n.Name.Like(model.Name));
            }
            if (model.State.HasValue)
            {
                container.And(n => n.State == model.State.Value);
            }
            if (model.GetAll)
            {
                var result = _organizationService.Query(x => x
                    .Where(container)
                    .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                    );
                model.Items = result;
                model.TotalItems = result.Count;
            }
            else
            {
                if (!model.PageSizeBySeted)
                {
                    model.PageSize = CurrentUser.UserSettings.PagingLimit;
                }
                PagedList<Organization.Domain.Organization> result = _organizationService.QueryPaged(x => x
                    .Page(model.Page, model.PageSize)
                    .Where(container)
                    .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                    );

                model.Items = result.Items;
                model.TotalItems = result.TotalItems;
            }
            return DynamicResult(model);
        }

        [HttpGet]
        [Description("新建组织")]
        public IActionResult CreateOrganization()
        {
            EditOrganizationModel model = new EditOrganizationModel();
            model.EntityMeta = _entityFinder.FindByName("organization");
            model.Attributes = _attributeFinder.FindByEntityId(model.EntityMeta.EntityId);
            model.State = (int)RecordState.Enabled;
            model.LanguageList = _languageService.Query(n => n.Sort(s => s.SortAscending(f => f.UniqueId)));
            model.Datas = new Entity("organization");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("新建组织-保存")]
        public IActionResult CreateOrganization(EditOrganizationModel model)
        {
            string msg = string.Empty;
            if (ModelState.IsValid)
            {
                Entity entity = new Entity("organization");
                entity.SetIdValue(Guid.NewGuid());
                entity.SetAttributeValue("Name", model.Name);
                entity.SetAttributeValue("LanguageId", (int)model.LanguageId);
                entity.SetAttributeValue("State", model.State);
                entity.SetAttributeValue("Description", model.Description);
                entity.SetAttributeValue("BaseCurrencyId", new EntityReference("currency", model.BaseCurrencyId));
                entity.SetAttributeValue("ManagerId", model.ManagerId);
                var id = _dataCreater.Create(entity);
                return JOk(T["created_success"], new { id = id });
            }
            msg = GetModelErrors(ModelState);
            return JOk(T["created_error"] + ": " + msg);
        }

        [HttpGet]
        [Description("组织编辑")]
        public IActionResult EditOrganization(Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                return NotFound();
            }
            EditOrganizationModel model = new EditOrganizationModel();
            model.OrganizationId = id;
            model.EntityMeta = _entityFinder.FindByName("organization");
            model.Attributes = _attributeFinder.FindByEntityId(model.EntityMeta.EntityId);
            model.LanguageList = _languageService.Query(n => n.Where(f => f.StateCode == RecordState.Enabled).Sort(s => s.SortAscending(f => f.UniqueId)));
            model.Datas = _dataFinder.RetrieveById("organization", id);
            model.Datas["manageridname"] = string.Empty;
            if (!model.Datas.GetGuidValue("managerid").Equals(Guid.Empty))
            {
                var manager = _dataFinder.RetrieveById("systemuser", model.Datas.GetGuidValue("managerid"));
                if (manager != null && manager.Count > 0)
                {
                    model.Datas["manageridname"] = manager.GetStringValue("name");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("组织信息保存")]
        public IActionResult EditOrganization(EditOrganizationModel model)
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

        [Description("删除组织")]
        [HttpPost]
        public IActionResult DeleteOrganization([FromBody]DeleteManyModel model)
        {
            return _organizationService.DeleteById(model.RecordId).DeleteResult(T);
        }

        [Description("设置组织可用状态")]
        [HttpPost]
        public IActionResult SetOrganizationActive([FromBody]SetRecordStateModel model)
        {
            return _organizationService.Update(x => x.Set(n => n.State, model.IsEnabled ? RecordState.Enabled : (int)RecordState.Disabled)
                .Where(n => n.OrganizationId.In(model.RecordId))
                ).UpdateResult(T);
        }

        #endregion 组织
    }
}