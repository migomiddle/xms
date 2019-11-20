using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Schema.Abstractions;
using Xms.Schema.Attribute;
using Xms.Schema.Entity;
using Xms.Schema.Extensions;
using Xms.Security.Abstractions;
using Xms.Web.Api.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Framework.Infrastructure;
using Xms.Web.Models;

namespace Xms.Web.Api
{
    /// <summary>
    /// 字段元数据接口
    /// </summary>
    [Route("{org}/api/schema/attribute")]
    public class AttributeMetadataController : ApiControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IDefaultAttributeProvider _defaultAttributeProvider;
        private readonly IAttributeUpdater _attributeUpdater;

        public AttributeMetadataController(IWebAppContext appContext
            , IEntityFinder entityService
            , IAttributeFinder attributeService
            , IDefaultAttributeProvider defaultAttributeProvider
            , IAttributeUpdater attributeUpdater)
            : base(appContext)
        {
            _entityFinder = entityService;
            _attributeFinder = attributeService;
            _defaultAttributeProvider = defaultAttributeProvider;
            _attributeUpdater = attributeUpdater;
        }

        [Description("字段列表")]
        [HttpGet]
        public IActionResult Get([FromQuery]GetAttributesModel model)
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
                result.RemoveAll(n => n.IsSystemControl());
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
        [HttpGet("getbyentityid/{entityid}/{name?}")]
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
        [HttpGet("getbyentityname/{entityname}/{name?}")]
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

        [Description("获取系统标准字段")]
        [HttpGet("SystemAttributes/{entitymask}")]
        public IActionResult GetSystemAttributes(EntityMaskEnum entityMask)
        {
            return JOk(_defaultAttributeProvider.GetSysAttributes(new Schema.Domain.Entity { BusinessFlowEnabled = true, WorkFlowEnabled = true, EntityMask = entityMask }));
        }

        [Description("查询字段权限资源")]
        [HttpGet("PrivilegeResource")]
        public IActionResult PrivilegeResource(bool? authorizationEnabled)
        {
            FilterContainer<Schema.Domain.Attribute> filter = FilterContainerBuilder.Build<Schema.Domain.Attribute>();
            filter.And(x => x.AttributeTypeName != AttributeTypeIds.PRIMARYKEY && x.Name.NotIn(AttributeDefaults.SystemAttributes));
            if (authorizationEnabled.HasValue)
            {
                filter.And(x => x.AuthorizationEnabled == authorizationEnabled);
            }
            var data = _attributeFinder.Query(x => x.Select(s => new { s.AttributeId, s.LocalizedName, s.EntityId, s.AuthorizationEnabled })
            .Where(filter)
            .Sort(s => s.SortAscending(f => f.Name))
            );
            if (data.NotEmpty())
            {
                var result = new List<PrivilegeResourceItem>();
                var entities = _entityFinder.FindAll()?.Where(x => x.IsCustomizable).OrderBy(x => x.LocalizedName).ToList();
                foreach (var item in entities)
                {
                    var attributes = data.Where(x => x.EntityId == item.EntityId);
                    if (!attributes.Any())
                    {
                        continue;
                    }
                    var group1 = new PrivilegeResourceItem
                    {
                        Label = item.LocalizedName,
                        Children = attributes.Select(x => (new PrivilegeResourceItem { Id = x.AttributeId, Label = x.LocalizedName, AuthorizationEnabled = x.AuthorizationEnabled })).ToList()
                    };
                    result.Add(group1);
                }
                return JOk(result);
            }
            return JOk();
        }

        [Description("启用字段权限")]
        [HttpPost("AuthorizationEnabled")]
        public IActionResult AuthorizationEnabled(UpdateAuthorizationStateModel model)
        {
            var authorizations = _attributeFinder.Query(x => x.Where(w => w.AuthorizationEnabled == true));
            if (authorizations.NotEmpty())
            {
                _attributeUpdater.UpdateAuthorization(false, authorizations.Select(x => x.AttributeId).ToArray());
            }
            if (Arguments.HasValue(model.ObjectId))
            {
                _attributeUpdater.UpdateAuthorization(true, model.ObjectId);
            }
            return SaveSuccess();
        }
    }
}