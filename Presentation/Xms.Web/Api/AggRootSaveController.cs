using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xms.Core;
using Xms.Core.Data;
using Xms.File;
using Xms.Flow;
using Xms.Infrastructure.Utility;
using Xms.Schema.Attribute;
using Xms.Schema.Entity;
using Xms.Schema.RelationShip;
using Xms.Sdk.Abstractions;
using Xms.Sdk.Client;
using Xms.Sdk.Client.AggRoot;
using Xms.Sdk.Extensions;
using Xms.Web.Api.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;

namespace Xms.Web.Api
{
    /// <summary>
    /// 实体数据保存接口
    /// </summary>
    [Route("{org}/api/data/aggrootsave")]
    public class AggRootSaveController : ApiControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IRelationShipFinder _relationShipFinder;

        private readonly IDataCreater _dataCreater;
        private readonly IDataUpdater _dataUpdater;

        private readonly IAggCreater _aggCreater;
        private readonly IAggUpdater _aggUpdater;
        private readonly IAggFinder _aggFinder;

        private readonly IAttachmentCreater _attachmentCreater;

        private readonly IBusinessProcessFlowInstanceUpdater _businessProcessFlowInstanceUpdater;

        public AggRootSaveController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IAttributeFinder attributeFinder
            , IRelationShipFinder relationShipFinder

            , IDataCreater dataCreater
            , IDataUpdater dataUpdater

            , IAggCreater aggCreater
            , IAggUpdater aggUpdater
            , IAggFinder aggFinder

            , IAttachmentCreater attachmentCreater

            , IBusinessProcessFlowInstanceUpdater businessProcessFlowInstanceUpdater)
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _attributeFinder = attributeFinder;
            _relationShipFinder = relationShipFinder;

            _dataCreater = dataCreater;
            _dataUpdater = dataUpdater;

            _aggCreater = aggCreater;
            _aggUpdater = aggUpdater;
            _aggFinder = aggFinder;

            _attachmentCreater = attachmentCreater;

            _businessProcessFlowInstanceUpdater = businessProcessFlowInstanceUpdater;
        }

        [Description("保存记录")]
        [HttpPost]
        public async Task<IActionResult> Post([FromForm]SaveDataModel model)
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

            AggregateRoot aggregateRoot = new AggregateRoot();
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
                    thisId = Guid.NewGuid();
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
                    //thisId = _dataCreater.Create(entity);
                    if (!model.StageId.Equals(Guid.Empty))//业务流程的阶段
                    {
                        _businessProcessFlowInstanceUpdater.UpdateForward(model.BusinessFlowId, model.BusinessFlowInstanceId, model.StageId, thisId);
                    }
                }
                else
                {
                    thisId = model.RecordId.Value;
                    entity.SetIdValue(model.RecordId.Value);
                }
                aggregateRoot.MainEntity = entity;
                aggregateRoot.ChildEntities = new List<RefEntity>();
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
                            OperationTypeEnum? entitystatus = root.entitystatus;
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
                            RefEntity refEntity = new RefEntity()
                            {
                                Name = name,
                                Relationshipname = relationshipname,
                                Entityid = model.EntityId,
                                Entitystatus = entitystatus
                            };

                            foreach (JProperty p in data)
                            {
                                var attr = childAttributes.Find(n => n.Name.IsCaseInsensitiveEqual(p.Name));
                                if (attr != null && p.Value != null)
                                {
                                    detail.SetAttributeValue(p.Name.ToString().ToLower(), detail.WrapAttributeValue(_entityFinder, attr, p.Value.ToString()));
                                }
                                refEntity.Entity = detail;
                            }
                            //关联主记录ID
                            if (refname.IsNotEmpty())
                            {
                                detail.SetAttributeValue(refname, new EntityReference(entityMetaData.Name, thisId));
                            }
                            aggregateRoot.ChildEntities.Add(refEntity);
                        }
                    }
                }

                //附件保存
                var files = Request.Form.Files;
                if (files.Count > 0)
                {
                    var result = await _attachmentCreater.CreateManyAsync(model.EntityId, thisId, files.ToList()).ConfigureAwait(false);
                    for (int i = 0; i < files.Count; i++)
                    {
                        var attr = attributeMetaDatas.Find(n => n.Name.IsCaseInsensitiveEqual(files[i].Name));
                        if (attr != null)
                        {
                            var etAttachement = result.Where(x => x["Name"].ToString().IsCaseInsensitiveEqual(files[i].FileName)).First();
                            if (etAttachement != null)
                            {
                                entity.SetAttributeValue(files[i].Name, entity.WrapAttributeValue(_entityFinder, attr, etAttachement["CDNPath"].ToString()));
                            }
                        }
                    }
                }

                if (isNew)
                {
                    thisId = _aggCreater.Create(aggregateRoot, thisId, model.FormId);
                }
                else
                {
                    _aggUpdater.Update(aggregateRoot, model.FormId);
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
    }
}