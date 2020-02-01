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
    /// 组织管理查询接口
    /// </summary>
    [Route("{org}/api/org")]
    public class OrgController : ApiControllerBase
    {
        private readonly IOrganizationService _organizationService;
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly ILanguageService _languageService;
        private readonly IDataFinder _dataFinder;

        public OrgController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IAttributeFinder attributeFinder
            , IOrganizationService organizationService
            , ILanguageService languageService
            , IDataFinder dataFinder)
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _attributeFinder = attributeFinder;
            _organizationService = organizationService;
            _languageService = languageService;
            _dataFinder = dataFinder;
        }

        [Description("组织列表")]
        [HttpGet]
        public IActionResult Get([FromQuery]OrganizationModel model)
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
            if (model.StatusCode.HasValue)
            {
                container.And(n => n.StatusCode == model.StatusCode.Value);
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
            return JOk(model);
        }

        [HttpGet("{id}")]
        [Description("查询组织Id")]
        public IActionResult Get(Guid id)
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
            return JOk(model);
        }

    }

}