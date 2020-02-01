using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Schema.Api.Models;
using Xms.Schema.Attribute;
using Xms.Schema.Entity;
using Xms.Schema.Extensions;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;

namespace Xms.Schema.Api
{
    /// <summary>
    /// 字段元数据接口
    /// </summary>
    [Route("{org}/api/schema/attribute")]
    public class AttributeMetadataController : ApiControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;

        public AttributeMetadataController(IWebAppContext appContext
            , IEntityFinder entityService
            , IAttributeFinder attributeService)
            : base(appContext)
        {
            _entityFinder = entityService;
            _attributeFinder = attributeService;
        }


        [Description("字段列表")]
        [HttpGet]
        public IActionResult Get([FromQuery]RetrieveAttributesModel model)
        {
            if (model.EntityId.Equals(Guid.Empty))
            {
                return NotFound();
            }
            var entity = _entityFinder.FindById(model.EntityId);
            if (entity == null)
            {
                return NotFound();
            }

            FilterContainer<Schema.Domain.Attribute> container = FilterContainerBuilder.Build<Schema.Domain.Attribute>();
            container.And(n => n.EntityId == model.EntityId);
            if (model.AttributeTypeName != null && model.AttributeTypeName.Length > 0)
            {
                container.And(n => n.AttributeTypeName.In(model.AttributeTypeName));
            }
            List<Schema.Domain.Attribute> result = _attributeFinder.Query(x => x
                .Where(container)
                .Sort(n => n.OnFile("name").ByDirection(SortDirection.Asc))
                );
            if (model.FilterSysAttribute)
            {
                //container.And(n => !n.IsSystemControl());
                result.RemoveAll(n => !n.IsSystemControl());
            }
            return JOk(result);
        }

        [Description("查询字段元数据")]
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var result = _attributeFinder.FindById(id);
            return JOk(result);
        }

        [Description("查询字段元数据")]
        [HttpGet("getbyentityid/{entityid}/{name}")]
        public IActionResult Get(Guid entityid, string name)
        {
            if (name.IsEmpty())
            {
                var result = _attributeFinder.FindByEntityId(entityid);
                return JOk(result);
            }
            else
            {
                var result = _attributeFinder.Find(entityid, name);
                return JOk(result);
            }
        }

        [Description("查询字段元数据")]
        [HttpGet("getbyentityname/{entityname}/{name}")]
        public IActionResult Get(string entityname, string name)
        {
            if (name.IsEmpty())
            {
                var result = _attributeFinder.FindByEntityName(entityname);
                return JOk(result);
            }
            else
            {
                var result = _attributeFinder.Find(entityname, name);
                return JOk(result);
            }
        }
    }
}