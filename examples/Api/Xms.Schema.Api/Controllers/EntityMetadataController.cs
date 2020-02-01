using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xms.Api.Core.Controller;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Schema.Abstractions;
using Xms.Schema.Api.Models;
using Xms.Schema.Entity;
using Xms.Web.Framework.Context;
//using Xms.Web.Framework.Controller;

namespace Xms.Schema.Api
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
            if (model.AuthorizationEnabled.HasValue)
            {
                filter.And(x => x.AuthorizationEnabled == model.AuthorizationEnabled.Value);
            }
            if (model.IsCustomizable.HasValue)
            {
                filter.And(x => x.IsCustomizable == model.IsCustomizable.Value);
            }
            if (model.LogEnabled.HasValue)
            {
                filter.And(x => x.LogEnabled == model.LogEnabled.Value);
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
            List<Schema.Domain.Entity> result;
            if (model.SolutionId.HasValue)
            {
                var pagedResult = _entityFinder.QueryPaged(x => x
                .Page(1, 250000)
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
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        public ValuesController()
        {

        }

        [HttpGet]
        public ActionResult<string> Get(){
            return "hello world";
        }
    }
}