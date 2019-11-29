using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.Linq;
using Xms.Business.DataAnalyse.Visualization;
using Xms.Core;
using Xms.Form;
using Xms.Infrastructure.Utility;
using Xms.QueryView;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Models;

namespace Xms.Web.Controllers
{
    /// <summary>
    /// 控件展示控制器
    /// </summary>
    public class ComponentController : AuthenticatedControllerBase
    {
        private readonly ISystemFormFinder _systemFormFinder;

        public ComponentController(IWebAppContext appContext
            , ISystemFormFinder systemFormFinder)
            : base(appContext)
        {
            _systemFormFinder = systemFormFinder;
        }

        #region 控件展示

        [Description("仪表板展示")]
        public IActionResult RenderDashBoard(Guid? id)
        {
            RenderDashBoardModel model = new RenderDashBoardModel();
            model.FormList = _systemFormFinder.QueryAuthorized(n => n
                .Where(f => f.StateCode == RecordState.Enabled)
                .Sort(s => s.SortAscending(f => f.Name))
                .SetUserContext(CurrentUser)
                , Form.Abstractions.FormType.Dashboard
            );
            if (id.HasValue && !id.Equals(Guid.Empty))
            {
                var entity = model.FormList.Find(n => n.SystemFormId == id.Value);
                if (entity != null)
                {
                    model.Id = id.Value;
                    model.FormEntity = entity;
                }
            }
            else if (!CurrentUser.UserSettings.DefaultDashboardId.Equals(Guid.Empty))
            {
                var entity = model.FormList?.Find(n => n.SystemFormId == CurrentUser.UserSettings.DefaultDashboardId);
                if (entity != null)
                {
                    model.Id = CurrentUser.UserSettings.DefaultDashboardId;
                    model.FormEntity = entity;
                }
            }
            if (model.FormEntity == null && model.FormList.NotEmpty())
            {
                model.FormEntity = model.FormList.First();
                model.Id = model.FormEntity.SystemFormId;
            }
            if (model.FormEntity == null)
            {
                return PromptView(T["notfound_record"]);
            }
            return View(model);
        }

        #endregion 控件展示
    }

    [Route("{org}/Component/[action]")]
    public class ChartRenderController : AuthenticatedControllerBase
    {
        private readonly IQueryViewFinder _queryViewFinder;
        private readonly IChartBuilder _chartBuilder;
        private readonly IChartFinder _chartFinder;

        public ChartRenderController(IWebAppContext appContext
            , IQueryViewFinder queryViewFinder
            , IChartBuilder chartBuilder
            , IChartFinder chartFinder)
            : base(appContext)
        {
            _queryViewFinder = queryViewFinder;
            _chartBuilder = chartBuilder;
            _chartFinder = chartFinder;
        }

        [Description("图表展示")]
        [HttpPost]
        public IActionResult RenderChart([FromBody]RenderChartModel model)
        {
            QueryView.Domain.QueryView view = null;
            if (model.QueryId.Equals(Guid.Empty) && model.EntityName.IsNotEmpty())
            {
                view = _queryViewFinder.FindEntityDefaultView(model.EntityName);
                if (view != null)
                {
                    model.QueryId = view.QueryViewId;
                }
            }
            else if (!model.QueryId.Equals(Guid.Empty))
            {
                view = _queryViewFinder.FindById(model.QueryId);
            }
            if (view == null)
            {
                return NotFound();
            }
            var chartEntity = _chartFinder.FindById(model.ChartId);
            var result = _chartBuilder.Build(view, chartEntity, model.Filter, model.Groups.NotEmpty() ? model.Groups.Last() : string.Empty);
            model.Chart = result.Chart;
            model.ChartData = result.ChartData;
            model.ChartEntity = chartEntity;
            model.Attributes = result.Attributes;
            model.DataSource = result.DataSource;

            return View($"~/Views/Component/{WebContext.ActionName}.cshtml", model);
        }
    }
}