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
using Xms.Core;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Schema.Entity;
using Xms.Solution;
using Xms.Web.Framework.Context;

namespace Xms.Business.Api.Controllers
{
    [Route("{org}/api/chart/create")]
    [ApiController]
    public class ChartCreaterController : ApiCustomizeControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IChartCreater _chartCreater;
        private readonly IChartUpdater _chartUpdater;
        private readonly IChartFinder _chartFinder;
        private readonly IChartDeleter _chartDeleter;
        public ChartCreaterController(IWebAppContext appContext
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

        [HttpGet]
        [Description("新建图表")]
        public IActionResult Get(Guid entityid)
        {
            EditChartModel model = new EditChartModel
            {
                SolutionId = SolutionId.Value,
                EntityId = entityid
            };
            return View(model);
        }

        [HttpPost]        
        [Description("新建图表-保存")]
        public IActionResult Post(EditChartModel model)
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
    }
}
