using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using Xms.Infrastructure.Utility;
using Xms.Solution;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;

namespace Xms.Web.Customize.Controllers
{
    /// <summary>
    /// 自定义管理基本控制器
    /// </summary>
    [Area("Customize")]
    public class CustomizeBaseController : AuthorizedControllerBase
    {
        protected readonly ISolutionService _solutionService;
        public Guid? SolutionId { get; set; }

        public CustomizeBaseController(IWebAppContext appContext
            , ISolutionService solutionService)
            : base(appContext)
        {
            _solutionService = solutionService;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            Solution.Domain.Solution solution = null;
            if (HttpContext.GetRouteOrQueryString("solutionid") != null)
            {
                SolutionId = Guid.Parse(HttpContext.GetRouteOrQueryString("solutionid"));
                if (SolutionId.HasValue && !SolutionId.Value.Equals(Guid.Empty))
                {
                    solution = _solutionService.FindById(SolutionId.Value);
                }
            }
            if (null == solution)
            {
                solution = _solutionService.Find(n => n.IsSystem == true);
                SolutionId = solution.SolutionId;
            }
            ViewBag.SolutionId = SolutionId.Value;
            ViewBag.SolutionName = solution.Name;
        }
    }
}