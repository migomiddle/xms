using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using Xms.Infrastructure.Utility;
using Xms.Solution;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;

namespace Xms.Api.Core.Controller
{
    /// <summary>
    /// 自定义管理基本控制器
    /// </summary>
    [Area("Customize")]
    public class ApiCustomizeControllerBase : ApiControllerBase
    {
        protected readonly ISolutionService _solutionService;

        protected Guid? _solutionId;
        protected string _solutionName;

        public Guid? SolutionId { get; set; }
        public ApiCustomizeControllerBase(IWebAppContext appContext
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
            _solutionId = SolutionId.Value;
            _solutionName = solution.Name;
        }
    }
}