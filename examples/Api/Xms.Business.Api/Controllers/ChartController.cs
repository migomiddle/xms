using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xms.Api.Core.Controller;
using Xms.Business.Api.Models;
using Xms.Business.DataAnalyse.Domain;
using Xms.Business.DataAnalyse.Visualization;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Schema.Entity;
using Xms.Solution;
using Xms.Web.Framework.Context;
using Xms.Core.Data;

namespace Xms.Business.Api.Controllers
{
    [Route("{org}/api/chart")]
    [ApiController]
    public class ChartController : ApiCustomizeControllerBase
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
        public IActionResult Get(ChartModel model)
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
            FilterContainer<Chart> filter = FilterContainerBuilder.Build<Chart>();
            filter.And(n => n.EntityId == model.EntityId);
            if (model.Name.IsNotEmpty())
            {
                filter.And(n => n.Name.Like(model.Name));
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
            return JOk(model);
        }

        [HttpGet("{id}")]
        [Description("图表编辑")]
        public IActionResult Get(Guid id)
        {
            EditChartModel model = new EditChartModel();
            if (!id.Equals(Guid.Empty))
            {
                var entity = _chartFinder.FindById(id);
                if (entity != null)
                {
                    entity.CopyTo(model);
                    model.ChartId = id;
                    return View(model);
                }
            }
            return NotFound();
        }
    }
}
