using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Schema.Abstractions;
using Xms.Schema.Entity;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Client;
using Xms.Solution.Abstractions;
using Xms.Web.Api.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;

namespace Xms.Web.Api
{
    /// <summary>
    /// 实体元数据接口
    /// </summary>
    [Route("{org}/api/schema/entity")]
    public class EntityMetadataController : ApiControllerBase
    {
        private readonly IEntityFinder _entityFinder;

        public EntityMetadataController(IWebAppContext appContext
            , IEntityFinder entityService)
            : base(appContext)
        {
            _entityFinder = entityService;
        }

        [Description("查询实体元数据列表")]
        [HttpGet]
        public IActionResult Get([FromQuery]RetrieveEntityModel model)
        {
            FilterContainer<Schema.Domain.Entity> filter = FilterContainerBuilder.Build<Schema.Domain.Entity>();
            filter.And(x => x.OrganizationId == CurrentUser.OrganizationId);
            if (model.Name.NotEmpty())
            {
                filter.And(x => x.Name.In(model.Name));
            }
            if (model.IsAuthorization.HasValue)
            {
                filter.And(x => x.AuthorizationEnabled == model.IsAuthorization.Value);
            }
            if (model.IsCustomizable.HasValue)
            {
                filter.And(x => x.IsCustomizable == model.IsCustomizable.Value);
            }
            if (model.IsLoged.HasValue)
            {
                filter.And(x => x.LogEnabled == model.IsLoged.Value);
            }
            if (model.DuplicateEnabled.HasValue)
            {
                filter.And(x => x.DuplicateEnabled == model.DuplicateEnabled.Value);
            }
            if (model.WorkFlowEnabled.HasValue)
            {
                filter.And(x => x.WorkFlowEnabled == model.WorkFlowEnabled.Value);
            }
            if (model.BusinessFlowEnabled.HasValue)
            {
                filter.And(x => x.BusinessFlowEnabled == model.BusinessFlowEnabled.Value);
            }
            if (model.GetAll)
            {
                model.PageSize = 25000;
            }
            else if (!model.PageSizeBySeted && CurrentUser.UserSettings.PagingLimit > 0)
            {
                model.PageSize = CurrentUser.UserSettings.PagingLimit;
            }
            model.PageSize = model.PageSize > WebContext.PlatformSettings.MaxFetchRecords ? WebContext.PlatformSettings.MaxFetchRecords : model.PageSize;
            List<Schema.Domain.Entity> result;
            if (model.SolutionId.HasValue)
            {
                var pagedResult = _entityFinder.QueryPaged(x => x
                .Page(1, model.PageSize)
                .Where(filter), model.SolutionId.Value, true);
                result = pagedResult.Items;
            }
            else
            {
                result = _entityFinder.Query(x => x
                   .Where(filter));
            }
            return JOk(result);
        }

        [Description("查询实体元数据")]
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var result = _entityFinder.FindById(id);
            return JOk(result);
        }

        [Description("查询实体元数据")]
        [HttpGet("getbyname/{name}")]
        public IActionResult Get(string name)
        {
            var result = _entityFinder.FindByName(name);
            return JOk(result);
        }

        [Description("多对一关联实体列表")]
        [HttpGet("getmanytoone/{entityid}")]
        public IActionResult GetManyToOne(Guid entityid)
        {
            List<Schema.Domain.Entity> result = _entityFinder.QueryRelated(entityid, RelationShipType.ManyToOne);

            return JOk(result);
        }

        [Description("一对多关联实体列表")]
        [HttpGet("getonetomany/{entityid}")]
        public IActionResult GetOneToMany(Guid entityid)
        {
            List<Schema.Domain.Entity> result = _entityFinder.QueryRelated(entityid, RelationShipType.OneToMany);

            return JOk(result);
        }

        [Description("解决方案组件")]
        [HttpGet("SolutionComponents")]
        public IActionResult SolutionComponents([FromQuery]GetSolutionComponentsModel model)
        {
            var data = _entityFinder.QueryPaged(x => x.Page(model.Page, model.PageSize), model.SolutionId, model.InSolution);
            if (data.Items.NotEmpty())
            {
                var result = data.Items.Select(x => (new SolutionComponentItem { ObjectId = x.EntityId, Name = x.Name, LocalizedName = x.LocalizedName, ComponentTypeName = EntityDefaults.ModuleName, CreatedOn = x.CreatedOn })).ToList();
                return JOk(new PagedList<SolutionComponentItem>()
                {
                    CurrentPage = model.Page
                    ,
                    ItemsPerPage = model.PageSize
                    ,
                    Items = result
                    ,
                    TotalItems = data.TotalItems
                    ,
                    TotalPages = data.TotalPages
                });
            }
            return JOk(data);
        }
    }

    [Route("{org}/api/schema/entitytree")]
    public class EntityTreeController : ApiControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IDataFinder _dataFinder;

        public EntityTreeController(IWebAppContext appContext
            , IEntityFinder entityService
            , IDataFinder dataFinder)
            : base(appContext)
        {
            _entityFinder = entityService;
            _dataFinder = dataFinder;
        }

        [Description("实体列表-JSON树格式")]
        [HttpGet()]
        public IActionResult Get(Guid? solutionId)
        {
            var entities = _entityFinder.QueryPaged(x => x
            .Page(1, 25000)
            .Where(f => f.OrganizationId == CurrentUser.OrganizationId)
           .Select(n => new { n.EntityId, n.Name, n.LocalizedName, n.EntityGroups })
               .Sort(n => n.SortAscending(f => f.LocalizedName)), solutionId.Value, true);
            var groups = _dataFinder.RetrieveAll("entitygroup", new List<string> { "name" }, new OrderExpression("name", OrderType.Ascending));
            if (entities.Items.Count(x => x.EntityGroups.IsEmpty()) > 0)
            {
                var nullGroup = new Xms.Core.Data.Entity("entitygroup");
                nullGroup.SetIdValue(Guid.Empty);
                nullGroup["name"] = "未分组";
                groups.Insert(0, nullGroup);
                foreach (var item in entities.Items.Where(x => x.EntityGroups.IsEmpty()))
                {
                    item.EntityGroups = Guid.Empty.ToString();
                }
            }
            List<dynamic> result = new List<dynamic>();
            foreach (var group in groups)
            {
                dynamic g = new ExpandoObject();
                g.label = group["name"];
                g.id = group.Id;
                g.children = entities.Items.Where(x => x.EntityGroups.IsNotEmpty() && x.EntityGroups.IndexOf(group.Id.ToString(), StringComparison.InvariantCultureIgnoreCase) >= 0)?.Select(x => new { id = x.EntityId, label = x.LocalizedName }).ToList();
                result.Add(g); ;
            }

            return JOk(result);
        }
    }
}