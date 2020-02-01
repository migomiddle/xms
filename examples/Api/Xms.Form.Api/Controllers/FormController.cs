using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.Linq;
using Xms.Api.Core.Controller;
using Xms.Authorization.Abstractions;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Form.Abstractions;
using Xms.Form.Api.Models;
using Xms.Form.Domain;
using Xms.Infrastructure.Utility;
using Xms.Schema.Entity;
using Xms.Solution;
using Xms.Web.Framework.Context;

namespace Xms.Form.Api.Controllers
{
    /// <summary>
    /// 表单查询接口
    /// </summary>
    [Route("{org}/api/form")]    
    public class FormController : ApiCustomizeControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IRoleObjectAccessService _roleObjectAccessService;
        private readonly ISystemFormCreater _systemFormCreater;
        private readonly ISystemFormDeleter _systemFormDeleter;
        private readonly ISystemFormFinder _systemFormFinder;
        private readonly ISystemFormUpdater _systemFormUpdater;
        public FormController(IWebAppContext appContext
            , ISolutionService solutionService
            , IEntityFinder entityFinder
            , IRoleObjectAccessService roleObjectAccessService
            , ISystemFormCreater systemFormCreater
            , ISystemFormDeleter systemFormDeleter
            , ISystemFormFinder systemFormFinder
            , ISystemFormUpdater systemFormUpdater)
            : base(appContext, solutionService)
        {
            _entityFinder = entityFinder;
            _roleObjectAccessService = roleObjectAccessService;
            _systemFormCreater = systemFormCreater;
            _systemFormDeleter = systemFormDeleter;
            _systemFormFinder = systemFormFinder;
            _systemFormUpdater = systemFormUpdater;
        }

        [HttpGet]
        [Description("表单列表")]
        public IActionResult Get([FromQuery]FormModel model)
        {
            if (model.EntityId.Equals(Guid.Empty))
            {
                return NotFound();
            }
            var entity = _entityFinder.FindById(model.EntityId);
            if (entity == null)
            {
                return NotFound();
            }
            model.Entity = entity;
            FilterContainer<SystemForm> filter = FilterContainerBuilder.Build<SystemForm>();
            filter.And(n => n.EntityId == model.EntityId);
            if (model.Name.IsNotEmpty())
            {
                filter.And(n => n.Name.Like(model.Name));
            }
            if (model.GetAll)
            {
                model.Page = 1;
                model.PageSize = WebContext.PlatformSettings.MaxFetchRecords; 
            }
            else if (CurrentUser.UserSettings.PagingLimit > 0)
            {
                model.PageSize = CurrentUser.UserSettings.PagingLimit;
            }
            PagedList<SystemForm> result = _systemFormFinder.QueryPaged(x => x
                    .Page(model.Page, model.PageSize)
                    .Where(filter)
                    .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                    );

            model.Items = result.Items;
            model.TotalItems = result.TotalItems;            
            model.SolutionId = SolutionId.Value;
            return JOk(model);
        }

        [HttpGet("{id}")]
        [Description("查询单个表单")]
        public IActionResult Get(Guid id)
        {
            EditFormModel model = new EditFormModel();
            if (!id.Equals(Guid.Empty))
            {
                var entity = _systemFormFinder.FindById(id);
                if (entity != null)
                {
                    entity.CopyTo(model);
                    return JOk(model);
                }
            }
            return NotFound();
        }





    }
}
