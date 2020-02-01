using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using Xms.Api.Core.Controller;
using Xms.Authorization.Abstractions;
using Xms.Core;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.QueryView.Api.Models;
using Xms.RibbonButton;
using Xms.RibbonButton.Abstractions;
using Xms.Schema.Entity;
using Xms.Sdk.Client;
using Xms.Solution;
using Xms.Web.Framework.Context;

namespace Xms.QueryView.Api.Controllers
{
    /// <summary>
    /// 视图查询接口
    /// </summary>
    [Route("{org}/api/queryview")]
    public class QueryViewController : ApiCustomizeControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IRibbonButtonFinder _ribbuttonFinder;
        private readonly IRoleObjectAccessService _roleObjectAccessService;
        private readonly IFetchDataService _fetchDataService;
        private readonly IQueryViewCreater _queryViewCreater;
        private readonly IQueryViewDeleter _queryViewDeleter;
        private readonly IQueryViewFinder _queryViewFinder;
        private readonly IQueryViewUpdater _queryViewUpdater;
        public QueryViewController(IWebAppContext appContext
            , ISolutionService solutionService
            , IEntityFinder entityFinder
            , IRibbonButtonFinder ribbuttonFinder
            , IRoleObjectAccessService roleObjectAccessService
            , IFetchDataService fetchDataService
            , IQueryViewCreater queryViewCreater
            , IQueryViewDeleter queryViewDeleter
            , IQueryViewFinder queryViewFinder
            , IQueryViewUpdater queryViewUpdater)
            : base(appContext, solutionService)
        {
            _entityFinder = entityFinder;
            _ribbuttonFinder = ribbuttonFinder;
            _roleObjectAccessService = roleObjectAccessService;
            _fetchDataService = fetchDataService;
            _queryViewCreater = queryViewCreater;
            _queryViewDeleter = queryViewDeleter;
            _queryViewFinder = queryViewFinder;
            _queryViewUpdater = queryViewUpdater;
        }


        [Description("视图列表")]
        public IActionResult Get(QueryViewModel model)
        {
            var entity = _entityFinder.FindById(model.EntityId);
            if (entity == null)
            {
                return NotFound();
            }
            model.Entity = entity;

            FilterContainer<QueryView.Domain.QueryView> container = FilterContainerBuilder.Build<QueryView.Domain.QueryView>();
            container.And(n => n.EntityId == model.EntityId);
            if (model.Name.IsNotEmpty())
            {
                container.And(n => n.Name == model.Name);
            }
            if (model.GetAll)
            {
                model.Page = 1;
                model.PageSize = 25000;
            }
            else if (CurrentUser.UserSettings.PagingLimit > 0)
            {
                model.PageSize = CurrentUser.UserSettings.PagingLimit;
            }
            PagedList<QueryView.Domain.QueryView> result = _queryViewFinder.QueryPaged(x => x
                .Page(model.Page, model.PageSize)
                .Where(container)
                .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                );

            model.Items = result.Items;
            model.TotalItems = result.TotalItems;            
            model.SolutionId = SolutionId.Value;
            return JOk(model);
        }

        [HttpGet("{id}")]
        [Description("新建视图")]
        public IActionResult Get(Guid entityid)
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
            EditQueryViewModel model = new EditQueryViewModel();
            model.SolutionId = SolutionId.Value;
            model.EntityId = entityid;
            model.EntityMetaData = entity;
            model.IsDisabled = false;
            model.Buttons = _ribbuttonFinder.Query(n => n
            .Where(f => f.EntityId == entityid && f.StateCode == RecordState.Enabled
            && (f.ShowArea == RibbonButtonArea.ListHead || f.ShowArea == RibbonButtonArea.ListRow)));
            return JOk(model);
        }

    }


}