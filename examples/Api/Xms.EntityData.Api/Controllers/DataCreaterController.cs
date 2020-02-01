using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xms.Core.Data;
using Xms.Infrastructure.Utility;
using Xms.Schema.Attribute;
using Xms.Schema.Entity;
using Xms.Sdk.Client;
using Xms.Sdk.Extensions;
using Xms.EntityData.Api.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Framework.Mvc;

namespace Xms.EntityData.Api
{
    /// <summary>
    /// 数据新增接口
    /// </summary>
    [Route("{org}/api/data/create")]
    public class DataCreaterController : ApiControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IDataCreater _dataCreater;
        private readonly IDataMapper _dataMapper;

        public DataCreaterController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IAttributeFinder attributeFinder
            , IDataCreater dataCreater
            , IDataMapper dataMapper)
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _attributeFinder = attributeFinder;
            _dataCreater = dataCreater;
            _dataMapper = dataMapper;
        }

        [Description("创建记录")]
        [HttpPost]
        public IActionResult Post(CreateRecordModel model)
        {
            if (model.Data.IsEmpty())
            {
                return JError("data is empty");
            }
            Schema.Domain.Entity entityMeta = null;
            if (model.EntityId.HasValue && !model.EntityId.Value.Equals(Guid.Empty))
            {
                entityMeta = _entityFinder.FindById(model.EntityId.Value);
            }
            else if (model.EntityName.IsNotEmpty())
            {
                entityMeta = _entityFinder.FindByName(model.EntityName);
            }
            if (entityMeta == null)
            {
                return NotFound();
            }
            var childAttributes = _attributeFinder.FindByEntityName(entityMeta.Name);
            if (model.Data.StartsWith("["))
            {
                var details = new List<Entity>();
                var items = JArray.Parse(model.Data.UrlDecode());
                if (items.Count > 0)
                {
                    foreach (var c in items)
                    {
                        dynamic root = JObject.Parse(c.ToString());
                        Entity detail = new Entity(entityMeta.Name);
                        foreach (JProperty p in root)
                        {
                            var attr = childAttributes.Find(n => n.Name.IsCaseInsensitiveEqual(p.Name));
                            if (attr != null && p.Value != null)
                            {
                                detail.SetAttributeValue(p.Name.ToString().ToLower(), detail.WrapAttributeValue(_entityFinder, attr, p.Value.ToString()));
                            }
                        }
                        details.Add(detail);
                    }
                }
                return _dataCreater.CreateMany(details).CreateResult(T);
            }
            else
            {
                dynamic root = JObject.Parse(model.Data.UrlDecode());
                Entity detail = new Entity(entityMeta.Name);
                foreach (JProperty p in root)
                {
                    var attr = childAttributes.Find(n => n.Name.IsCaseInsensitiveEqual(p.Name));
                    if (attr != null)
                    {
                        detail.SetAttributeValue(p.Name.ToString().ToLower(), detail.WrapAttributeValue(_entityFinder, attr, p.Value.ToString()));
                    }
                }
                var id = _dataCreater.Create(detail);
                return CreateSuccess(new { id = id });
            }
        }

        [Description("从实体映射新建记录")]
        [HttpPost("map")]
        public IActionResult Map(CreateFromMapModel args)
        {
            if (!args.SourceEntityId.HasValue || args.SourceEntityId.Value.Equals(Guid.Empty))
            {
                if (args.SourceEntityName.IsNotEmpty())
                {
                    var entityMeta = _entityFinder.FindByName(args.SourceEntityName);
                    args.SourceEntityId = entityMeta.EntityId;
                }
            }
            if (!args.TargetEntityId.HasValue || args.TargetEntityId.Value.Equals(Guid.Empty))
            {
                if (args.TargetEntityName.IsNotEmpty())
                {
                    var entityMeta = _entityFinder.FindByName(args.TargetEntityName);
                    args.TargetEntityId = entityMeta.EntityId;
                }
            }
            var newId = _dataMapper.Create(args.SourceEntityId.Value, args.TargetEntityId.Value, args.SourceEntityId.Value);

            return CreateSuccess(new { entityid = args.TargetEntityId, id = newId });
        }
    }
}