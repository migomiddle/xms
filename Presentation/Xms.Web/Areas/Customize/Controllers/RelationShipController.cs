using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Schema.Abstractions;
using Xms.Schema.Entity;
using Xms.Schema.RelationShip;
using Xms.Solution;
using Xms.Web.Customize.Models;
using Xms.Web.Framework.Context;

namespace Xms.Web.Customize.Controllers
{
    /// <summary>
    /// 关系管理控制器
    /// </summary>
    public class RelationShipController : CustomizeBaseController
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IRelationShipUpdater _relationShipUpdater;
        private readonly IRelationShipFinder _relationShipFinder;

        public RelationShipController(IWebAppContext appContext
            , ISolutionService solutionService
            , IEntityFinder entityFinder
            , IRelationShipUpdater relationShipUpdater
            , IRelationShipFinder relationShipFinder)
            : base(appContext, solutionService)
        {
            _entityFinder = entityFinder;
            _relationShipUpdater = relationShipUpdater;
            _relationShipFinder = relationShipFinder;
        }

        [Description("关系列表")]
        public IActionResult Index(RelationShipsModel model)
        {
            if (!model.LoadData)
            {
                return DynamicResult(model);
            }
            FilterContainer<Schema.Domain.RelationShip> filter = FilterContainerBuilder.Build<Schema.Domain.RelationShip>();
            if (model.Type == RelationShipType.OneToMany)
            {
                filter.And(n => n.ReferencedEntityId == model.EntityId);
            }
            else if (model.Type == RelationShipType.ManyToOne)
            {
                filter.And(n => n.ReferencingEntityId == model.EntityId);
            }
            else if (model.Type == RelationShipType.ManyToMany)
            {
                filter.And(n => n.ReferencingEntityId == model.EntityId);
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
            PagedList<Schema.Domain.RelationShip> result = _relationShipFinder.QueryPaged(x => x
                .Page(model.Page, model.PageSize)
                .Where(filter)
                .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection)));
            _relationShipFinder.WrapLocalizedLabel(result.Items);
            model.Items = result.Items;
            model.TotalItems = result.TotalItems;
            model.SolutionId = SolutionId.Value;
            model.Entity = _entityFinder.FindById(model.EntityId);
            return DynamicResult(model);
        }

        [Description("配置关系")]
        [HttpGet]
        public IActionResult EditRelationShip(Guid id)
        {
            EditRelationShipModel model = new EditRelationShipModel
            {
                RelationShipId = id,
                RelationShipMeta = _relationShipFinder.FindById(id)
            };
            model.RelationShipMeta.CopyTo(model);
            return View(model);
        }

        [Description("配置关系")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditRelationShip(EditRelationShipModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _relationShipFinder.FindById(model.RelationShipId);
                entity.CascadeAssign = model.CascadeAssign;
                entity.CascadeDelete = model.CascadeDelete;
                entity.CascadeLinkMask = model.CascadeLinkMask;
                entity.CascadeShare = model.CascadeShare;
                entity.CascadeUnShare = model.CascadeUnShare;
                _relationShipUpdater.Update(entity);
                return SaveSuccess();
            }
            return SaveFailure(GetModelErrors());
        }
    }
}