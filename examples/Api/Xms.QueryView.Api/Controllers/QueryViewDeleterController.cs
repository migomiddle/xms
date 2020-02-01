using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Xms.Api.Core.Controller;
using Xms.Authorization.Abstractions;
using Xms.Schema.Entity;
using Xms.Sdk.Client;
using Xms.Solution;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;

namespace Xms.QueryView.Api.Controllers
{
    /// <summary>
    /// 视图删除接口
    /// </summary>
    [Route("{org}/api/queryview/delete")]
    public class QueryViewDeleterController : ApiCustomizeControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        
        private readonly IRoleObjectAccessService _roleObjectAccessService;
        private readonly IFetchDataService _fetchDataService;
        private readonly IQueryViewCreater _queryViewCreater;
        private readonly IQueryViewDeleter _queryViewDeleter;
        private readonly IQueryViewFinder _queryViewFinder;
        private readonly IQueryViewUpdater _queryViewUpdater;
        public QueryViewDeleterController(IWebAppContext appContext
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

        [Description("删除视图")]
        [HttpPost]
        public IActionResult Post(DeleteManyModel model)
        {
            return _queryViewDeleter.DeleteById(model.RecordId).DeleteResult(T);
        }
    }
}