using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.Linq;
using Xms.Api.Core.Controller;
using Xms.Authorization.Abstractions;
using Xms.Core;
using Xms.Infrastructure.Utility;
using Xms.QueryView.Api.Models;
using Xms.Schema.Entity;
using Xms.Sdk.Client;
using Xms.Solution;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;

namespace Xms.QueryView.Api.Controllers
{
    /// <summary>
    /// 视图更新接口
    /// </summary>
    [Route("{org}/api/queryview/update")]
    public class QueryViewUpdaterController : ApiCustomizeControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        
        private readonly IRoleObjectAccessService _roleObjectAccessService;
        private readonly IFetchDataService _fetchDataService;
        private readonly IQueryViewCreater _queryViewCreater;
        private readonly IQueryViewDeleter _queryViewDeleter;
        private readonly IQueryViewFinder _queryViewFinder;
        private readonly IQueryViewUpdater _queryViewUpdater;
        public QueryViewUpdaterController(IWebAppContext appContext
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
        [Description("视图信息保存")]
        public IActionResult Post(EditQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _queryViewFinder.FindById(model.QueryViewId);
                model.IsPrivate = entity.IsPrivate;
                model.IsDefault = entity.IsDefault;
                entity.StateCode = RecordState.Enabled;
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
                       
        [Description("设置视图默认状态")]        
        [HttpPost("setdefault")]
        public IActionResult SetQueryViewDefault([FromBody]SetQueryViewDefaultModel model)
        {
            return _queryViewUpdater.UpdateDefault(model.EntityId, model.RecordId.First()).UpdateResult(T);
        }

        [Description("设置视图可用状态")]
        [HttpPost("setstate")]
        public IActionResult SetQueryViewState([FromBody]SetRecordStateModel model)
        {
            return _queryViewUpdater.UpdateState(model.RecordId, model.IsEnabled).UpdateResult(T);
        }

        [Description("设置视图权限启用状态")]
        [HttpPost("setauthstate")]
        public IActionResult SetViewAuthorizationState([FromBody]SetViewAuthorizationStateModel model)
        {            
            return _queryViewUpdater.UpdateAuthorization(model.IsAuthorization , model.RecordId).UpdateResult(T);
        }

    }
}