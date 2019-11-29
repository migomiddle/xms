using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Logging.DataLog;
using Xms.Logging.DataLog.Domain;
using Xms.Schema.Attribute;
using Xms.Schema.Entity;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Models;

namespace Xms.Web.Controllers
{
    /// <summary>
    /// 实体数据日志控制器
    /// </summary>
    [Route("{org}/entity/[action]")]
    public class EntityLogsController : AuthenticatedControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IEntityLogService _entityLogService;

        public EntityLogsController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IAttributeFinder attributeFinder
            , IEntityLogService entityLogService)
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _attributeFinder = attributeFinder;
            _entityLogService = entityLogService;
        }

        #region 日志

        [Description("实体日志列表")]
        public IActionResult EntityLogs(EntityLogsModel model)
        {
            model.Attributes = _attributeFinder.FindByEntityId(model.EntityId);
            FilterContainer<EntityLog> container = FilterContainerBuilder.Build<EntityLog>();
            container.And(n => n.OrganizationId == CurrentUser.OrganizationId);
            container.And(n => n.EntityId == model.EntityId);
            if (model.OperationType.HasValue)
            {
                container.And(n => n.OperationType == model.OperationType.Value);
            }
            if (model.GetAll)
            {
                List<EntityLog> result = _entityLogService.Query(x => x
                    .Page(model.Page, model.PageSize)
                    .Where(container)
                    .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                    );

                model.Items = result;
                model.TotalItems = result.Count;
            }
            else
            {
                PagedList<EntityLog> result = _entityLogService.QueryPaged(x => x
                    .Page(model.Page, model.PageSize)
                    .Where(container)
                    .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                    );

                model.Items = result.Items;
                model.TotalItems = result.TotalItems;
            }
            return DynamicResult(model, $"~/Views/Entity/{WebContext.ActionName}.cshtml");
        }

        [Description("清空实体日志")]
        //[ValidateAntiForgeryToken]
        public IActionResult ClearEntityLogs(Guid entityId)
        {
            _entityLogService.Clear(entityId);
            return JsonResult(T["operation_success"]);
        }

        #endregion 日志
    }
}