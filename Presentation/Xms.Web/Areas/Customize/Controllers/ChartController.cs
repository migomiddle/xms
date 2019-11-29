using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xms.Business.DataAnalyse.Domain;
using Xms.Business.DataAnalyse.Visualization;
using Xms.Core;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Schema.Entity;
using Xms.Solution;
using Xms.Web.Customize.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;

namespace Xms.Web.Customize.Controllers
{
    /// <summary>
    /// 图表管理控制器
    /// </summary>
    public class ChartController : CustomizeBaseController
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IChartCreater _chartCreater;
        private readonly IChartUpdater _chartUpdater;
        private readonly IChartFinder _chartFinder;
        private readonly IChartDeleter _chartDeleter;

        public ChartController(IWebAppContext appContext
            , ISolutionService solutionService
            , IEntityFinder entityFinder
            , IChartCreater chartCreater
            , IChartUpdater chartUpdater
            , IChartFinder chartFinder
            , IChartDeleter chartDeleter)
            : base(appContext, solutionService)
        {
            _entityFinder = entityFinder;
            _chartCreater = chartCreater;
            _chartUpdater = chartUpdater;
            _chartFinder = chartFinder;
            _chartDeleter = chartDeleter;
        }

        [Description("图表列表")]
        public IActionResult Index(ChartModel model)
        {
            if (!model.LoadData)
            {
                return DynamicResult(model);
            }

            FilterContainer<Chart> filter = FilterContainerBuilder.Build<Chart>();
            if (model.Name.IsNotEmpty())
            {
                filter.And(n => n.Name.Like(model.Name));
            }
            if (!model.EntityId.Equals(Guid.Empty))
            {
                filter.And(n => n.EntityId == model.EntityId);
            }
            if (model.GetAll)
            {
                List<Chart> result = _chartFinder.Query(x => x
                    .Page(model.Page, model.PageSize)
                    .Where(filter)
                    .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                    );

                model.Items = result;
                model.TotalItems = result.Count;
            }
            else
            {
                if (CurrentUser.UserSettings.PagingLimit > 0)
                {
                    model.PageSize = CurrentUser.UserSettings.PagingLimit;
                }
                PagedList<Chart> result = _chartFinder.QueryPaged(x => x
                    .Page(model.Page, model.PageSize)
                    .Where(filter)
                    .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                    );

                model.Items = result.Items;
                model.TotalItems = result.TotalItems;
            }
            model.SolutionId = SolutionId.Value;
            return DynamicResult(model);
        }

        [HttpGet]
        [Description("新建图表")]
        public IActionResult CreateChart(Guid entityid)
        {
            EditChartModel model = new EditChartModel
            {
                SolutionId = SolutionId.Value,
                EntityId = entityid
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("新建图表-保存")]
        public IActionResult CreateChart(EditChartModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = new Chart();
                model.CopyTo(entity);
                entity.ChartId = Guid.NewGuid();
                entity.StateCode = RecordState.Enabled;
                entity.OrganizationId = CurrentUser.OrganizationId;
                _chartCreater.Create(entity);

                return CreateSuccess(new { id = entity.ChartId });
            }
            var msg = GetModelErrors(ModelState);
            return CreateFailure(msg);
        }

        [HttpGet]
        [Description("图表编辑")]
        public IActionResult EditChart(Guid id)
        {
            EditChartModel model = new EditChartModel();
            if (!id.Equals(Guid.Empty))
            {
                var entity = _chartFinder.FindById(id);
                if (entity != null)
                {
                    entity.CopyTo(model);
                    model.ChartId = id;
                    model.EntityMeta = _entityFinder.FindById(entity.EntityId);
                    return View(model);
                }
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("图表信息保存")]
        public IActionResult EditChart(EditChartModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _chartFinder.FindById(model.ChartId);
                model.CopyTo(entity);
                entity.OrganizationId = CurrentUser.OrganizationId;
                _chartUpdater.Update(entity);
                return UpdateSuccess(new { id = entity.ChartId });
            }
            var msg = GetModelErrors(ModelState);
            return UpdateFailure(msg);
        }

        [Description("删除图表")]
        [HttpPost]
        public IActionResult DeleteChart([FromBody]DeleteManyModel model)
        {
            return _chartDeleter.DeleteById(model.RecordId).DeleteResult(T);
        }

        [Description("设置图表可用状态")]
        [HttpPost]
        public IActionResult SetChartState([FromBody]SetChartStateModel model)
        {
            return _chartUpdater.UpdateState(model.RecordId, model.IsEnabled).UpdateResult(T);
        }
    }
}