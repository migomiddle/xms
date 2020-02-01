using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using Xms.Api.Core.Controller;
using Xms.Authorization.Abstractions;
using Xms.Core;
using Xms.Infrastructure.Utility;
using Xms.QueryView.Api.Models;
using Xms.Schema.Entity;
using Xms.Sdk.Client;
using Xms.Solution;
using Xms.Web.Framework.Context;

namespace Xms.QueryView.Api.Controllers
{
    /// <summary>
    /// 视图创建接口
    /// </summary>
    [Route("{org}/api/queryview/create")]
    public class QueryViewCreaterController : ApiCustomizeControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        
        private readonly IRoleObjectAccessService _roleObjectAccessService;
        private readonly IFetchDataService _fetchDataService;
        private readonly IQueryViewCreater _queryViewCreater;
        private readonly IQueryViewDeleter _queryViewDeleter;
        private readonly IQueryViewFinder _queryViewFinder;
        private readonly IQueryViewUpdater _queryViewUpdater;
        public QueryViewCreaterController(IWebAppContext appContext
            , ISolutionService solutionService
            , IEntityFinder entityFinder            
            , IRoleObjectAccessService roleObjectAccessService
            , IFetchDataService fetchDataService
            , IQueryViewCreater queryViewCreater
            , IQueryViewDeleter queryViewDeleter
            , IQueryViewFinder queryViewFinder
            , IQueryViewUpdater queryViewUpdater)
            : base(appContext, solutionService)
        {
            _entityFinder = entityFinder;            
            _roleObjectAccessService = roleObjectAccessService;
            _fetchDataService = fetchDataService;
            _queryViewCreater = queryViewCreater;
            _queryViewDeleter = queryViewDeleter;
            _queryViewFinder = queryViewFinder;
            _queryViewUpdater = queryViewUpdater;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("新建视图-保存")]
        public IActionResult Post(EditQueryViewModel model)
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
    }
}