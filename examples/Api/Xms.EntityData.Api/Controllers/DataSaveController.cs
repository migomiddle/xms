using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xms.Core.Data;
using Xms.Flow;
using Xms.Infrastructure.Utility;
using Xms.Schema.Attribute;
using Xms.Schema.Entity;
using Xms.Schema.RelationShip;
using Xms.Sdk.Abstractions;
using Xms.Sdk.Client;
using Xms.Sdk.Extensions;
using Xms.EntityData.Api.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;

namespace Xms.EntityData.Api
{
    /// <summary>
    /// 实体数据保存接口
    /// </summary>
    [Route("{org}/api/data/save")]
    public class DataSaveController : ApiControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IRelationShipFinder _relationShipFinder;
        private readonly IDataCreater _dataCreater;
        private readonly IDataUpdater _dataUpdater;
        private readonly IBusinessProcessFlowInstanceUpdater _businessProcessFlowInstanceUpdater;

        public DataSaveController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IAttributeFinder attributeFinder
            , IRelationShipFinder relationShipFinder
            , IDataCreater dataCreater
            , IDataUpdater dataUpdater
            , IBusinessProcessFlowInstanceUpdater businessProcessFlowInstanceUpdater)
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _attributeFinder = attributeFinder;
            _relationShipFinder = relationShipFinder;
            _dataCreater = dataCreater;
            _dataUpdater = dataUpdater;
            _businessProcessFlowInstanceUpdater = businessProcessFlowInstanceUpdater;
        }
        
        [Description("保存记录")]
        [HttpPost]
        public IActionResult Post(SaveDataModel model)
        {
            if (model.EntityId.Equals(Guid.Empty))
            {
                return NotFound();
            }
            var entityMetaData = _entityFinder.FindById(model.EntityId);
            if (entityMetaData == null)
            {
                return NotFound();
            }
            var attributeMetaDatas = _attributeFinder.FindByEntityId(model.EntityId);
            bool isNew = !(model.RecordId.HasValue && !model.RecordId.Value.Equals(Guid.Empty));
            var thisId = Guid.Empty;
            try
            {
                Core.Data.Entity entity = new Core.Data.Entity(entityMetaData.Name);
                dynamic headData = JObject.Parse(model.Data);
                foreach (JProperty p in headData)
                {
                    var attr = attributeMetaDatas.Find(n => n.Name.IsCaseInsensitiveEqual(p.Name));
                    if (attr != null && p.Value != null)
                    {
                        entity.SetAttributeValue(p.Name.ToString().ToLower(), entity.WrapAttributeValue(_entityFinder, attr, p.Value.ToString()));
                    }
                }
                if (isNew)
                {
                    if (model.RelationShipName.IsNotEmpty() && model.ReferencedRecordId.HasValue)//如果存在关联关系
                    {
                        var relationShipMetas = _relationShipFinder.FindByName(model.RelationShipName);
                        if (null != relationShipMetas && relationShipMetas.ReferencingEntityId == model.EntityId && entity.GetStringValue(relationShipMetas.ReferencingAttributeName).IsEmpty())
                        {
                            //设置当前记录关联字段的值
                            entity.SetAttributeValue(relationShipMetas.ReferencingAttributeName, new EntityReference(relationShipMetas.ReferencedEntityName, model.ReferencedRecordId.Value));
                        }
                    }
                    if (!model.StageId.Equals(Guid.Empty))//业务流程的阶段
                    {
                        entity.SetAttributeValue("StageId", model.StageId);
                    }
                    thisId = _dataCreater.Create(entity);
                    if (!model.StageId.Equals(Guid.Empty))//业务流程的阶段
                    {
                        _businessProcessFlowInstanceUpdater.UpdateForward(model.BusinessFlowId, model.BusinessFlowInstanceId, model.StageId, thisId);
                    }
                }
                else
                {
                    thisId = model.RecordId.Value;
                    entity.SetIdValue(model.RecordId.Value);
                    _dataUpdater.Update(entity);
                }
                //单据体
                if (model.Child.IsNotEmpty())
                {
                    var childs = JArray.Parse(model.Child.UrlDecode());
                    if (childs.Count > 0)
                    {
                        List<Core.Data.Entity> childEntities = new List<Core.Data.Entity>();
                        List<string> entityNames = new List<string>();
                        foreach (var c in childs)
                        {
                            dynamic root = JObject.Parse(c.ToString());
                            string name = root.name, relationshipname = root.relationshipname, refname = string.Empty;
                            if (!entityNames.Exists(n => n.IsCaseInsensitiveEqual(name)))
                            {
                                entityNames.Add(name);
                            }

                            var data = root.data;
                            var childAttributes = _attributeFinder.FindByEntityName(name);
                            if (relationshipname.IsNotEmpty())
                            {
                                var relationShipMetas = _relationShipFinder.FindByName(relationshipname);
                                if (null != relationShipMetas && relationShipMetas.ReferencedEntityId == model.EntityId)
                                {
                                    refname = relationShipMetas.ReferencingAttributeName;
                                }
                            }
                            Core.Data.Entity detail = new Core.Data.Entity(name);
                            foreach (JProperty p in data)
                            {
                                var attr = childAttributes.Find(n => n.Name.IsCaseInsensitiveEqual(p.Name));
                                if (attr != null && p.Value != null)
                                {
                                    detail.SetAttributeValue(p.Name.ToString().ToLower(), detail.WrapAttributeValue(_entityFinder, attr, p.Value.ToString()));
                                }
                            }
                            //关联主记录ID
                            if (refname.IsNotEmpty())
                            {
                                detail.SetAttributeValue(refname, new EntityReference(entityMetaData.Name, thisId));
                            }
                            childEntities.Add(detail);
                        }
                        //批量创建记录
                        if (childEntities.NotEmpty())
                        {
                            foreach (var item in entityNames)
                            {
                                var items = childEntities.Where(n => n.Name.IsCaseInsensitiveEqual(item)).ToList();
                                var creatingRecords = items.Where(n => n.Name.IsCaseInsensitiveEqual(item) && n.GetIdValue().Equals(Guid.Empty)).ToList();
                                if (creatingRecords.NotEmpty())
                                {
                                    _dataCreater.CreateMany(creatingRecords);
                                }
                                if (!isNew)
                                {
                                    foreach (var updItem in items.Where(n => n.Name.IsCaseInsensitiveEqual(item) && !n.GetIdValue().Equals(Guid.Empty)))
                                    {
                                        _dataUpdater.Update(updItem);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return JError(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            if (isNew)
            {
                return CreateSuccess(new { id = thisId });
            }
            return UpdateSuccess(new { id = thisId });
        }

        [Description("保存单据体数据")]
        [HttpPost("SaveChilds")]
        public IActionResult SaveChilds(SaveChildDataModel model)
        {
            if (model.Child.IsEmpty())
            {
                return JError("data is empty");
            }
            var childs = JArray.Parse(model.Child.UrlDecode());
            if (childs.Count > 0)
            {
                var entityMeta = _entityFinder.FindByName(model.EntityName);
                if (entityMeta == null)
                {
                    return NotFound();
                }
                var entityid = entityMeta.EntityId;
                List<Entity> childEntities = new List<Entity>();
                List<string> entityNames = new List<string>();
                foreach (var c in childs)
                {
                    dynamic root = JObject.Parse(c.ToString());
                    string name = root.name, relationshipname = root.relationshipname, refname = string.Empty;
                    if (!entityNames.Exists(n => n.IsCaseInsensitiveEqual(name)))
                    {
                        entityNames.Add(name);
                    }

                    var data = root.data;
                    var childAttributes = _attributeFinder.FindByEntityName(name);
                    if (relationshipname.IsNotEmpty())
                    {
                        var relationShipMetas = _relationShipFinder.FindByName(relationshipname);
                        if (null != relationShipMetas && relationShipMetas.ReferencedEntityId == entityid)
                        {
                            refname = relationShipMetas.ReferencingAttributeName;
                        }
                    }
                    Entity detail = new Entity(name);
                    foreach (JProperty p in data)
                    {
                        var attr = childAttributes.Find(n => n.Name.IsCaseInsensitiveEqual(p.Name));
                        if (attr != null && p.Value != null)
                        {
                            detail.SetAttributeValue(p.Name.ToString().ToLower(), detail.WrapAttributeValue(_entityFinder, attr, p.Value.ToString()));
                        }
                    }
                    //关联主记录ID
                    if (refname.IsNotEmpty())
                    {
                        detail.SetAttributeValue(refname, new EntityReference(entityMeta.Name, model.ParentId));
                    }
                    childEntities.Add(detail);
                }
                //批量创建记录
                if (childEntities.NotEmpty())
                {
                    foreach (var item in entityNames)
                    {
                        var items = childEntities.Where(n => n.Name.IsCaseInsensitiveEqual(item)).ToList();
                        var creatingRecords = items.Where(n => n.Name.IsCaseInsensitiveEqual(item) && n.GetIdValue().Equals(Guid.Empty)).ToList();
                        if (creatingRecords.NotEmpty())
                        {
                            _dataCreater.CreateMany(creatingRecords);
                        }
                        if (creatingRecords.Count < childEntities.Count)
                        {
                            var updatingRecords = items.Where(n => n.Name.IsCaseInsensitiveEqual(item) && !n.GetIdValue().Equals(Guid.Empty)).ToList();
                            foreach (var updItem in updatingRecords)
                            {
                                _dataUpdater.Update(updItem);
                            }
                        }
                    }
                }
            }
            return SaveSuccess();
        }
    }
}