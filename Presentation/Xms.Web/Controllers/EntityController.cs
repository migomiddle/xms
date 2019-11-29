using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using Xms.Data.Export;
using Xms.Infrastructure.Utility;
using Xms.QueryView;
using Xms.Sdk.Abstractions.Query;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Models;

namespace Xms.Web.Controllers
{
    /// <summary>
    /// 实体数据控制器
    /// </summary>
    [Route("{org}/entity/[action]")]
    public class EntityController : AuthenticatedControllerBase
    {
        private readonly IQueryViewFinder _queryViewFinder;
        private readonly IDataExporter _dataExporter;

        public EntityController(IWebAppContext appContext
            , IQueryViewFinder queryViewFinder
            , IDataExporter dataExporter
            )
            : base(appContext)
        {
            _queryViewFinder = queryViewFinder;
            _dataExporter = dataExporter;
        }

        #region 列表

        [Description("导出记录")]
        public IActionResult Export([FromBody]EntityGridModel model)
        {
            QueryView.Domain.QueryView queryView = null;
            if (model.QueryViewId.HasValue && !model.QueryViewId.Equals(Guid.Empty))
            {
                queryView = _queryViewFinder.FindById(model.QueryViewId.Value);
            }
            else if (model.EntityId.HasValue && !model.EntityId.Value.Equals(Guid.Empty))
            {
                queryView = _queryViewFinder.FindEntityDefaultView(model.EntityId.Value);
            }
            else if (model.EntityName.IsNotEmpty())
            {
                queryView = _queryViewFinder.FindEntityDefaultView(model.EntityName);
            }
            else
            {
                return NotFound();
            }
            if (queryView == null)
            {
                return NotFound();
            }
            OrderExpression orderExp = null;
            if (model.IsSortBySeted)
            {
                orderExp = new OrderExpression(model.SortBy, model.SortDirection == 0 ? OrderType.Ascending : OrderType.Descending);
            }
            string path = _dataExporter.ToExcelFile(queryView, model.Filter, orderExp, queryView.Name, model.ExportType == 1, includeIndex: model.IncludeIndex, title: model.ExportTitle);
            if (path.IsEmpty())
            {
                return JError(T["list_nodata"]);
            }
            return JOk(path);
        }

        #endregion 列表

        #region 新建/编辑/查看记录

        [Description("更改记录状态")]
        [HttpPost]
        public IActionResult SetRecordState([FromBody]SetEntityRecordStateModel model)
        {
            if (model.RecordId.IsEmpty())
            {
                return NotSpecifiedRecord();
            }
            return View(model);
        }

        #endregion 新建/编辑/查看记录
    }
}