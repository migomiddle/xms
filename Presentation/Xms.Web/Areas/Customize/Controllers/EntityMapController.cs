using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xms.Core;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.DataMapping;
using Xms.DataMapping.Domain;
using Xms.Infrastructure.Utility;
using Xms.Schema.Abstractions;
using Xms.Schema.Attribute;
using Xms.Schema.Entity;
using Xms.Schema.RelationShip;
using Xms.Solution;
using Xms.Web.Customize.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;

namespace Xms.Web.Customize.Controllers
{
    /// <summary>
    /// 实体转换规则管理控制器
    /// </summary>
    public class EntityMapController : CustomizeBaseController
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IRelationShipFinder _relationShipFinder;
        private readonly IEntityMapCreater _entityMapCreater;
        private readonly IEntityMapUpdater _entityMapUpdater;
        private readonly IEntityMapFinder _entityMapFinder;
        private readonly IEntityMapDeleter _entityMapDeleter;
        private readonly IAttributeMapCreater _attributeMapCreater;
        private readonly IAttributeMapFinder _attributeMapFinder;

        public EntityMapController(IWebAppContext appContext
            , ISolutionService solutionService
            , IEntityFinder entityFinder
            , IAttributeFinder attributeFinder
            , IRelationShipFinder relationShipFinder
            , IEntityMapCreater entityMapCreater
            , IEntityMapUpdater entityMapUpdater
            , IEntityMapFinder entityMapFinder
            , IEntityMapDeleter entityMapDeleter
            , IAttributeMapCreater attributeMapCreater
            , IAttributeMapFinder attributeMapFinder)
            : base(appContext, solutionService)
        {
            _entityFinder = entityFinder;
            _attributeFinder = attributeFinder;
            _relationShipFinder = relationShipFinder;
            _entityMapCreater = entityMapCreater;
            _entityMapUpdater = entityMapUpdater;
            _entityMapFinder = entityMapFinder;
            _entityMapDeleter = entityMapDeleter;
            _attributeMapCreater = attributeMapCreater;
            _attributeMapFinder = attributeMapFinder;
        }

        [Description("实体转换关系")]
        public IActionResult Index(EntityMapModel model)
        {
            if (!model.LoadData)
            {
                return DynamicResult(model);
            }
            FilterContainer<EntityMap> filter = FilterContainerBuilder.Build<EntityMap>();
            filter.And(x => x.ParentEntityMapId == Guid.Empty);
            if (!model.EntityId.Equals(Guid.Empty))
            {
                filter.And(n => n.TargetEntityId == model.EntityId);
            }
            if (model.GetAll)
            {
                model.Page = 1;
                model.PageSize = WebContext.PlatformSettings.MaxFetchRecords;
            }
            else if (!model.PageSizeBySeted && CurrentUser.UserSettings.PagingLimit > 0)
            {
                model.PageSize = CurrentUser.UserSettings.PagingLimit;
            }
            model.PageSize = model.PageSize > WebContext.PlatformSettings.MaxFetchRecords ? WebContext.PlatformSettings.MaxFetchRecords : model.PageSize;
            PagedList<EntityMap> result = _entityMapFinder.QueryPaged(x => x
                .Page(model.Page, model.PageSize)
                .Where(filter)
                .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                );

            model.Items = result.Items;
            model.TotalItems = result.TotalItems;
            model.SolutionId = SolutionId.Value;
            return DynamicResult(model);
        }

        [Description("创建实体转换关系")]
        public IActionResult CreateEntityMap(Guid entityid = default(Guid))
        {
            EditEntityMapModel model = new EditEntityMapModel
            {
                SolutionId = SolutionId.Value,
                EntityId = entityid,
                TargetEntityMetaData = entityid == default(Guid) ? null : _entityFinder.FindById(entityid),
                Attributes = entityid == default(Guid) ? null : _attributeFinder.Query(n => n
                .Where(f => f.EntityId == entityid && f.Name.NotIn(AttributeDefaults.SystemAttributes)
                && f.AttributeTypeName != AttributeTypeIds.PRIMARYKEY
                ).Sort(s => s.SortAscending(f => f.Name)))
            };
            return View(model);
        }

        [Description("创建实体转换关系")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateEntityMap(EditEntityMapModel model)
        {
            if (ModelState.IsValid)
            {
                //已存在的单据头映射
                var headEntityMap = _entityMapFinder.Find(model.SourceEntityId, model.EntityId);
                if (headEntityMap != null)
                {
                    return JError(T["entitymap_duplicated"]);
                }
                //已存在的单据体映射
                if (model.ChildAttributeMap.NotEmpty() && !model.ChildSourceEntityId.Equals(Guid.Empty) && !model.ChildTargetEntityId.Equals(Guid.Empty))
                {
                    var childEntityMap = _entityMapFinder.Find(model.ChildSourceEntityId, model.ChildTargetEntityId);
                    if (childEntityMap != null)
                    {
                        return JError(T["entitymap_duplicated"]);
                    }
                }
                var id = Guid.NewGuid();
                //单据头
                if (model.HeadAttributeMap.NotEmpty())
                {
                    var entMapTable = new EntityMap
                    {
                        ComponentState = 0,
                        SolutionId = SolutionId.Value,
                        OrganizationId = CurrentUser.OrganizationId,
                        CreatedBy = CurrentUser.SystemUserId,
                        CreatedOn = DateTime.Now,
                        EntityMapId = id,
                        SourceEntityId = model.SourceEntityId,
                        TargetEntityId = model.EntityId,
                        RelationShipName = model.RelationShipName,
                        MapType = model.MapType,
                        StateCode = RecordState.Enabled
                    };
                    var headAttributeMap = new List<AttributeMap>();
                    foreach (var item in model.HeadAttributeMap)
                    {
                        if (!item.SourceAttributeId.Equals(Guid.Empty))
                        {
                            var attributeMap = new AttributeMap
                            {
                                AttributeMapId = Guid.NewGuid(),
                                CanChange = item.CanChange,
                                EntityMapId = entMapTable.EntityMapId,
                                TargetAttributeId = item.TargetAttributeId,
                                SourceAttributeId = item.SourceAttributeId
                            };
                            if (model.HeadControlAttributeMap.NotEmpty())
                            {
                                var controlAttrMap = model.HeadControlAttributeMap.Find(n => n.SourceAttributeId == item.SourceAttributeId);
                                if (controlAttrMap != null)
                                {
                                    attributeMap.RemainAttributeId = controlAttrMap.RemainAttributeId;
                                    attributeMap.ClosedAttributeId = controlAttrMap.ClosedAttributeId;
                                }
                            }
                            headAttributeMap.Add(attributeMap);
                        }
                    }
                    if (headAttributeMap.IsEmpty())
                    {
                        return JError(T["entitymap_emptyheadattributemap"]);
                    }
                    _entityMapCreater.Create(entMapTable);
                    _attributeMapCreater.CreateMany(headAttributeMap);
                    //单据体
                    if (model.ChildAttributeMap.NotEmpty() && !model.ChildSourceEntityId.Equals(Guid.Empty) && !model.ChildTargetEntityId.Equals(Guid.Empty))
                    {
                        var childEntityMap = new EntityMap
                        {
                            ComponentState = 0,
                            SolutionId = SolutionId.Value,
                            OrganizationId = CurrentUser.OrganizationId,
                            CreatedBy = CurrentUser.SystemUserId,
                            CreatedOn = DateTime.Now,
                            EntityMapId = Guid.NewGuid(),
                            SourceEntityId = model.ChildSourceEntityId,
                            TargetEntityId = model.ChildTargetEntityId,
                            RelationShipName = model.ChildRelationShipName,
                            ParentEntityMapId = entMapTable.EntityMapId,
                            MapType = model.MapType,
                            StateCode = RecordState.Enabled
                        };
                        var childAttributeMap = new List<AttributeMap>();
                        foreach (var item in model.ChildAttributeMap)
                        {
                            if (!item.SourceAttributeId.Equals(Guid.Empty))
                            {
                                var attributeMap = new AttributeMap
                                {
                                    AttributeMapId = Guid.NewGuid(),
                                    CanChange = item.CanChange,
                                    EntityMapId = childEntityMap.EntityMapId,
                                    TargetAttributeId = item.TargetAttributeId,
                                    SourceAttributeId = item.SourceAttributeId
                                };
                                if (model.ChildControlAttributeMap.NotEmpty())
                                {
                                    var controlAttrMap = model.ChildControlAttributeMap.Find(n => n.SourceAttributeId == item.SourceAttributeId);
                                    if (controlAttrMap != null)
                                    {
                                        attributeMap.RemainAttributeId = controlAttrMap.RemainAttributeId;
                                        attributeMap.ClosedAttributeId = controlAttrMap.ClosedAttributeId;
                                    }
                                }
                                childAttributeMap.Add(attributeMap);
                            }
                        }
                        if (childAttributeMap.IsEmpty())
                        {
                            return JError(T["entitymap_emptychildattributemap"]);
                        }
                        _entityMapCreater.Create(childEntityMap);
                        _attributeMapCreater.CreateMany(childAttributeMap);
                    }
                }

                return CreateSuccess(new { id = id });
            }
            return CreateFailure(GetModelErrors());
        }

        [Description("编辑实体转换关系")]
        public IActionResult EditEntityMap(Guid entitymapid)
        {
            var headEntityMap = _entityMapFinder.FindById(entitymapid);
            EditEntityMapModel model = new EditEntityMapModel
            {
                EntityMapId = entitymapid,
                EntityId = headEntityMap.TargetEntityId,
                TargetEntityMetaData = _entityFinder.FindById(headEntityMap.TargetEntityId),
                SourceEntityMetaData = _entityFinder.FindById(headEntityMap.SourceEntityId),
                Attributes = _attributeFinder.Query(n => n
                .Where(f => f.EntityId == headEntityMap.TargetEntityId && f.Name.NotIn(AttributeDefaults.SystemAttributes)).Sort(s => s.SortAscending(f => f.Name))),
                SolutionId = SolutionId.Value,
                //单据头
                HeadEntityMap = headEntityMap,
                RelationShipName = headEntityMap.RelationShipName,
                HeadAttributeMap = _attributeMapFinder.Query(n => n.Where(f => f.EntityMapId == headEntityMap.EntityMapId)),
                SourceEntityId = headEntityMap.SourceEntityId,
                MapType = headEntityMap.MapType
            };
            model.HeadRelationShip = _relationShipFinder.FindByName(model.RelationShipName);
            //单据体
            var childEntityMap = _entityMapFinder.FindByParentId(headEntityMap.EntityMapId);
            if (childEntityMap != null)
            {
                model.ChildEntityMap = childEntityMap;
                model.ChildRelationShipName = childEntityMap.RelationShipName;
                model.ChildTargetEntityMetaData = _entityFinder.FindById(childEntityMap.TargetEntityId);
                model.ChildSourceEntityMetaData = _entityFinder.FindById(childEntityMap.SourceEntityId);
                model.ChildAttributeMap = _attributeMapFinder.Query(n => n.Where(f => f.EntityMapId == childEntityMap.EntityMapId));
                model.ChildTargetEntityId = childEntityMap.TargetEntityId;
                model.ChildSourceEntityId = childEntityMap.SourceEntityId;
            }
            return View(model);
        }

        [Description("编辑实体转换关系")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditEntityMap(EditEntityMapModel model)
        {
            if (ModelState.IsValid)
            {
                //原单据头
                var headEntityMap = _entityMapFinder.FindById(model.EntityMapId.Value);
                //原单据体
                var childEntityMap = _entityMapFinder.FindByParentId(headEntityMap.EntityMapId);
                //单据头
                if (model.HeadAttributeMap.NotEmpty())
                {
                    headEntityMap.MapType = model.MapType;
                    var headAttributeMap = new List<AttributeMap>();
                    foreach (var item in model.HeadAttributeMap)
                    {
                        if (!item.SourceAttributeId.Equals(Guid.Empty))
                        {
                            var attrMapTable = new AttributeMap
                            {
                                AttributeMapId = Guid.NewGuid(),
                                CanChange = item.CanChange,
                                EntityMapId = headEntityMap.EntityMapId,
                                TargetAttributeId = item.TargetAttributeId,
                                SourceAttributeId = item.SourceAttributeId
                            };
                            headAttributeMap.Add(attrMapTable);
                        }
                    }
                    if (headAttributeMap.IsEmpty())
                    {
                        return JError(T["entitymap_emptyheadattributemap"]);
                    }
                    //删除原有的
                    _entityMapDeleter.DeleteById(headEntityMap.EntityMapId);
                    _entityMapCreater.Create(headEntityMap);
                    _attributeMapCreater.CreateMany(headAttributeMap);
                    //单据体
                    if (model.ChildAttributeMap.NotEmpty() && !model.ChildSourceEntityId.Equals(Guid.Empty) && !model.ChildTargetEntityId.Equals(Guid.Empty))
                    {
                        childEntityMap = childEntityMap ?? new EntityMap();
                        childEntityMap.EntityMapId = childEntityMap != null ? childEntityMap.EntityMapId : Guid.NewGuid();
                        childEntityMap.ComponentState = childEntityMap != null ? childEntityMap.ComponentState : 0;
                        childEntityMap.SolutionId = childEntityMap != null ? childEntityMap.SolutionId : SolutionId.Value;
                        childEntityMap.OrganizationId = CurrentUser.OrganizationId;
                        childEntityMap.CreatedBy = CurrentUser.SystemUserId;
                        childEntityMap.CreatedOn = DateTime.Now;
                        childEntityMap.SourceEntityId = childEntityMap != null ? childEntityMap.SourceEntityId : model.ChildSourceEntityId;
                        childEntityMap.TargetEntityId = childEntityMap != null ? childEntityMap.TargetEntityId : model.ChildTargetEntityId;
                        childEntityMap.RelationShipName = childEntityMap != null ? childEntityMap.RelationShipName : model.ChildRelationShipName;
                        childEntityMap.ParentEntityMapId = headEntityMap.EntityMapId;
                        childEntityMap.StateCode = childEntityMap != null ? childEntityMap.StateCode : RecordState.Enabled;
                        var childAttributeMap = new List<AttributeMap>();
                        foreach (var item in model.ChildAttributeMap)
                        {
                            if (!item.SourceAttributeId.Equals(Guid.Empty))
                            {
                                var attrMapTable = new AttributeMap
                                {
                                    AttributeMapId = Guid.NewGuid(),
                                    CanChange = item.CanChange,
                                    EntityMapId = childEntityMap.EntityMapId,
                                    TargetAttributeId = item.TargetAttributeId,
                                    SourceAttributeId = item.SourceAttributeId
                                };
                                childAttributeMap.Add(attrMapTable);
                            }
                        }
                        if (childAttributeMap.IsEmpty())
                        {
                            return JError(T["entitymap_emptychildattributemap"]);
                        }
                        //删除原有的
                        if (childEntityMap != null)
                        {
                            _entityMapDeleter.DeleteById(childEntityMap.EntityMapId);
                        }
                        _entityMapCreater.Create(childEntityMap);
                        _attributeMapCreater.CreateMany(childAttributeMap);
                    }
                }

                return UpdateSuccess();
            }
            return UpdateFailure(GetModelErrors());
        }

        [Description("删除实体转换关系")]
        [HttpPost]
        public IActionResult DeleteEntityMap([FromBody]DeleteManyModel model)
        {
            return _entityMapDeleter.DeleteById(model.RecordId).DeleteResult(T);
        }

        [Description("清除实体转换关系")]
        public IActionResult ClearEntityMap(Guid entitymapid)
        {
            _entityMapDeleter.DeleteById(entitymapid);
            return JOk(T["clear_success"]);
        }

        [Description("设置实体转换关系可用状态")]
        [HttpPost]
        public IActionResult SetEntityMapState([FromBody]SetRecordStateModel model)
        {
            return _entityMapUpdater.UpdateState(model.RecordId, model.IsEnabled).UpdateResult(T);
        }
    }
}