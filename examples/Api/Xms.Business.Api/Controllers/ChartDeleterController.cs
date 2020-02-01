using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xms.Api.Core.Controller;
using Xms.Business.Api.Models;
using Xms.Business.DataAnalyse.Visualization;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Schema.Entity;
using Xms.Solution;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;

namespace Xms.Business.Api.Controllers
{
    [Route("{org}/api/chart/delete")]
    [ApiController]
    public class ChartDeleterController : ApiCustomizeControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IChartCreater _chartCreater;
        private readonly IChartUpdater _chartUpdater;
        private readonly IChartFinder _chartFinder;
        private readonly IChartDeleter _chartDeleter;
        public ChartDeleterController(IWebAppContext appContext
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

        [Description("删除图表")]
        [HttpPost]
        public IActionResult Post([FromBody]DeleteManyModel model)
        {
            return _chartDeleter.DeleteById(model.RecordId).DeleteResult(T);
        }
    }
}
