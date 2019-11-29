using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.Linq;
using Xms.Core;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.QueryView;
using Xms.RibbonButton;
using Xms.RibbonButton.Abstractions;
using Xms.Schema.Entity;
using Xms.Sdk.Client;
using Xms.Solution;
using Xms.Web.Customize.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;
using Xms.WebResource;

namespace Xms.Web.Customize.Controllers
{
    /// <summary>
    /// 视图管理控制器
    /// </summary>
    public class QueryViewController : CustomizeBaseController
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IRibbonButtonFinder _ribbuttonFinder;
        private readonly IFetchDataService _fetchDataService;
        private readonly IQueryViewCreater _queryViewCreater;
        private readonly IQueryViewDeleter _queryViewDeleter;
        private readonly IQueryViewFinder _queryViewFinder;
        private readonly IQueryViewUpdater _queryViewUpdater;
        private readonly IWebResourceFinder _webResourceFinder;

        public QueryViewController(IWebAppContext appContext
            , ISolutionService solutionService
            , IEntityFinder entityFinder
            , IRibbonButtonFinder ribbuttonFinder
            , IFetchDataService fetchDataService
            , IQueryViewCreater queryViewCreater
            , IQueryViewDeleter queryViewDeleter
            , IQueryViewFinder queryViewFinder
            , IQueryViewUpdater queryViewUpdater
            , IWebResourceFinder webResourceFinder)
            : base(appContext, solutionService)
        {
            _entityFinder = entityFinder;
            _ribbuttonFinder = ribbuttonFinder;
            _fetchDataService = fetchDataService;
            _queryViewCreater = queryViewCreater;
            _queryViewDeleter = queryViewDeleter;
            _queryViewFinder = queryViewFinder;
            _queryViewUpdater = queryViewUpdater;
            _webResourceFinder = webResourceFinder;
        }

        [Description("视图列表")]
        public IActionResult Index(QueryViewModel model)
        {
            var entity = _entityFinder.FindById(model.EntityId);
            if (entity == null)
            {
                return NotFound();
            }
            model.Entity = entity;
            if (!model.LoadData)
            {
                return DynamicResult(model);
            }

            FilterContainer<QueryView.Domain.QueryView> container = FilterContainerBuilder.Build<QueryView.Domain.QueryView>();
            container.And(n => n.EntityId == model.EntityId);
            if (model.Name.IsNotEmpty())
            {
                container.And(n => n.Name == model.Name);
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
            PagedList<QueryView.Domain.QueryView> result = _queryViewFinder.QueryPaged(x => x
                .Page(model.Page, model.PageSize)
                .Where(container)
                .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                );

            model.Items = result.Items;
            model.TotalItems = result.TotalItems;
            model.SolutionId = SolutionId.Value;
            return DynamicResult(model);
        }

        [HttpGet]
        [Description("新建视图")]
        public IActionResult CreateQueryView(Guid entityid)
        {
            if (entityid.Equals(Guid.Empty))
            {
                return NotFound();
            }
            var entity = _entityFinder.FindById(entityid);
            if (entity == null)
            {
                return NotFound();
            }
            EditQueryViewModel model = new EditQueryViewModel
            {
                SolutionId = SolutionId.Value,
                EntityId = entityid,
                EntityMetaData = entity,
                StateCode = RecordState.Enabled,
                Buttons = _ribbuttonFinder.Query(n => n
                .Where(f => f.EntityId == entityid && f.StateCode == RecordState.Enabled
                && (f.ShowArea == RibbonButtonArea.ListHead || f.ShowArea == RibbonButtonArea.ListRow)))
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("新建视图-保存")]
        public IActionResult CreateQueryView(EditQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = new QueryView.Domain.QueryView();
                model.CopyTo(entity);
                entity.StateCode = RecordState.Enabled;
                entity.IsDefault = false;
                entity.IsPrivate = false;
                entity.QueryViewId = Guid.NewGuid();
                entity.CreatedBy = CurrentUser.SystemUserId;
                if (model.IsCustomButton && model.ButtonId.NotEmpty())
                {
                    entity.CustomButtons = model.ButtonId.SerializeToJson();
                }
                entity.AggregateConfig = string.Empty;
                _queryViewCreater.Create(entity);
                return CreateSuccess(new { id = entity.QueryViewId });
            }
            return CreateFailure(GetModelErrors());
        }

        [HttpGet]
        [Description("视图编辑")]
        public IActionResult EditQueryView(Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                return NotFound();
            }
            EditQueryViewModel model = new EditQueryViewModel();
            var entity = _queryViewFinder.FindById(id);
            if (null == entity)
            {
                return NotFound();
            }
            var entityMeta = _entityFinder.FindById(entity.EntityId);
            if (null == entityMeta)
            {
                return NotFound();
            }
            entity.CopyTo(model);
            model.EntityMetaData = entityMeta;
            _fetchDataService.GetMetaDatas(entity.FetchConfig);
            model.EntityList = _fetchDataService.QueryResolver.EntityList;
            model.AttributeList = _fetchDataService.QueryResolver.AttributeList;
            model.Grid = new GridBuilder(entity).Grid;
            model.QueryExpression = _fetchDataService.QueryExpression;
            model.Buttons = _ribbuttonFinder.Query(n => n
            .Where(f => f.EntityId == model.EntityId && f.StateCode == RecordState.Enabled
            && (f.ShowArea == RibbonButtonArea.ListHead || f.ShowArea == RibbonButtonArea.ListRow)));
            if (model.Grid.ClientResources.NotEmpty())
            {
                model.WebResources = _webResourceFinder.Query(x => x.Select(s => new { s.WebResourceId, s.Name }).Where(f => f.WebResourceId.In(model.Grid.ClientResources)));
            }
            if (model.IsCustomButton && entity.CustomButtons.IsNotEmpty())
            {
                model.ButtonId = model.ButtonId.DeserializeFromJson(entity.CustomButtons);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("视图信息保存")]
        public IActionResult EditQueryView(EditQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _queryViewFinder.FindById(model.QueryViewId);
                model.IsPrivate = entity.IsPrivate;
                model.IsDefault = entity.IsDefault;
                model.StateCode = entity.StateCode;
                model.CopyTo(entity);
                if (model.IsCustomButton && model.ButtonId.NotEmpty())
                {
                    entity.CustomButtons = model.ButtonId.SerializeToJson();
                }
                else
                {
                    entity.CustomButtons = string.Empty;
                }
                if (model.SaveType == "saveas")
                {
                    entity.QueryViewId = Guid.NewGuid();
                    entity.IsDefault = false;
                    entity.CreatedOn = DateTime.Now;
                    entity.AggregateConfig = string.Empty;
                    _queryViewCreater.Create(entity);
                }
                else
                {
                    _queryViewUpdater.Update(entity);
                }
                return UpdateSuccess(new { id = entity.QueryViewId });
            }
            return UpdateFailure(GetModelErrors());
        }

        [Description("视图复制")]
        public IActionResult CopyQueryView(Guid queryViewId, string name)
        {
            if (!queryViewId.Equals(Guid.Empty))
            {
                var entity = _queryViewFinder.FindById(queryViewId);
                var newQueryView = new QueryView.Domain.QueryView();
                entity.CopyTo(newQueryView);
                newQueryView.QueryViewId = Guid.NewGuid();
                newQueryView.Name = name.IfEmpty(entity.Name + " Copy");
                newQueryView.IsDefault = false;
                newQueryView.CreatedOn = DateTime.Now;
                newQueryView.CreatedBy = CurrentUser.SystemUserId;
                _queryViewCreater.Create(newQueryView);
                return SaveSuccess(new { id = entity.QueryViewId });
            }
            return SaveFailure();
        }

        [Description("删除视图")]
        [HttpPost]
        public IActionResult DeleteQueryView([FromBody]DeleteManyModel model)
        {
            return _queryViewDeleter.DeleteById(model.RecordId).DeleteResult(T);
        }

        [Description("设置视图默认状态")]
        [HttpPost]
        public IActionResult SetQueryViewDefault([FromBody]SetQueryViewDefaultModel model)
        {
            return _queryViewUpdater.UpdateDefault(model.EntityId, model.RecordId.First()).UpdateResult(T);
        }

        [Description("设置视图可用状态")]
        [HttpPost]
        public IActionResult SetQueryViewState([FromBody]SetRecordStateModel model)
        {
            return _queryViewUpdater.UpdateState(model.RecordId, model.IsEnabled).UpdateResult(T);
        }

        [Description("设置视图权限启用状态")]
        [HttpPost]
        public IActionResult SetViewAuthorizationState([FromBody]SetViewAuthorizationStateModel model)
        {
            return _queryViewUpdater.UpdateAuthorization(model.IsAuthorization, model.RecordId).UpdateResult(T);
        }
    }
}