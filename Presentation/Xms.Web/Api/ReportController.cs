using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Linq;
using Xms.Business.DataAnalyse.Report;
using Xms.Core.Context;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;
using Xms.Web.Api.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;

namespace Xms.Web.Api
{
    /// <summary>
    /// 报表接口
    /// </summary>
    [Route("{org}/api/[controller]")]
    public class ReportController : ApiControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IWebAppContext appContext
            , IReportService reportService)
            : base(appContext)
        {
            _reportService = reportService;
        }

        [Description("解决方案组件")]
        [HttpGet("SolutionComponents")]
        public IActionResult SolutionComponents([FromQuery]GetSolutionComponentsModel model)
        {
            var data = _reportService.QueryPaged(x => x.Page(model.Page, model.PageSize), model.SolutionId, model.InSolution);
            if (data.Items.NotEmpty())
            {
                var result = data.Items.Select(x => (new SolutionComponentItem { ObjectId = x.ReportId, Name = x.Name, LocalizedName = x.Name, ComponentTypeName = ReportDefaults.ModuleName, CreatedOn = x.CreatedOn })).ToList();
                return JOk(new PagedList<SolutionComponentItem>()
                {
                    CurrentPage = model.Page
                    ,
                    ItemsPerPage = model.PageSize
                    ,
                    Items = result
                    ,
                    TotalItems = data.TotalItems
                    ,
                    TotalPages = data.TotalPages
                });
            }
            return JOk(data);
        }
    }
}