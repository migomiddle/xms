using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using Xms.Core.Data;
using Xms.Infrastructure.Utility;
using Xms.Schema.Attribute;
using Xms.Schema.Entity;
using Xms.Schema.Extensions;
using Xms.Sdk.Client;
using Xms.Sdk.Extensions;
using Xms.EntityData.Api.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Framework.Mvc;

namespace Xms.EntityData.Api
{
    /// <summary>
    /// 数据更新接口
    /// </summary>
    [Route("{org}/api/data/update")]
    public class DataUpdatererController : ApiControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IDataUpdater _dataUpdater;

        public DataUpdatererController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IAttributeFinder attributeFinder
            , IDataUpdater dataUpdater)
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _attributeFinder = attributeFinder;
            _dataUpdater = dataUpdater;
        }

        [Description("更新记录")]
        [HttpPost]
        public IActionResult Post(DataUpdateModel model)
        {
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
            var primaryAttr = childAttributes.Find(n => n.TypeIsPrimaryKey());
            if (model.Data.StartsWith("["))
            {
                //var details = new List<Entity>();
                var items = JArray.Parse(model.Data.UrlDecode());
                if (items.Count > 0)
                {
                    foreach (var c in items)
                    {
                        dynamic root = JObject.Parse(c.ToString());
                        Entity detail = new Entity(entityMeta.Name);
                        foreach (JProperty p in root)
                        {
                            if (p.Name.IsCaseInsensitiveEqual("id"))
                            {
                                detail.SetIdValue(Guid.Parse(p.Value.ToString()), primaryAttr.Name);
                            }
                            else
                            {
                                var attr = childAttributes.Find(n => n.Name.IsCaseInsensitiveEqual(p.Name));
                                if (attr != null)
                                {
                                    detail.SetAttributeValue(p.Name.ToString().ToLower(), detail.WrapAttributeValue(_entityFinder, attr, p.Value.ToString()));
                                }
                            }
                        }
                        //details.Add(detail);
                        _dataUpdater.Update(detail);
                    }
                }
                //_organizationServiceProxy.UpdateMany(details);
                return UpdateSuccess();
            }
            else
            {
                Entity detail = new Entity(entityMeta.Name);
                dynamic root = JObject.Parse(model.Data.UrlDecode());
                foreach (JProperty p in root)
                {
                    if (p.Name.IsCaseInsensitiveEqual("id"))
                    {
                        detail.SetIdValue(Guid.Parse(p.Value.ToString()), primaryAttr.Name);
                    }
                    else
                    {
                        var attr = childAttributes.Find(n => n.Name.IsCaseInsensitiveEqual(p.Name));
                        if (attr != null)
                        {
                            detail.SetAttributeValue(p.Name.ToString().ToLower(), detail.WrapAttributeValue(_entityFinder, attr, p.Value.ToString()));
                        }
                    }
                }
                _dataUpdater.Update(detail);
            }
            return UpdateSuccess();
        }

        [Description("更改记录状态")]
        [HttpPost("state")]
        public IActionResult State(SetEntityRecordStateModel model)
        {
            if (model.RecordId.IsEmpty())
            {
                return NotSpecifiedRecord();
            }
            var entityMeta = _entityFinder.FindById(model.EntityId);
            var flag = false;
            foreach (var item in model.RecordId)
            {
                Entity entity = new Entity(entityMeta.Name);
                entity.SetIdValue(item);
                entity.SetAttributeValue("statecode", model.State);
                flag = _dataUpdater.Update(entity);
            }
            return flag.UpdateResult(T);
        }
    }
}