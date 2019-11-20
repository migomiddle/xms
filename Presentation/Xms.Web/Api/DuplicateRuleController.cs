using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Linq;
using Xms.Business.DuplicateValidator;
using Xms.Core.Context;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;
using Xms.Web.Api.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;

namespace Xms.Web.Api
{
    /// <summary>
    /// 重复检测规则接口
    /// </summary>
    [Route("{org}/api/duplicaterule")]
    public class DuplicateRuleController : ApiControllerBase
    {
        private readonly IDuplicateRuleFinder _duplicateRuleFinder;

        public DuplicateRuleController(IWebAppContext appContext
            , IDuplicateRuleFinder duplicateRuleFinder)
            : base(appContext)
        {
            _duplicateRuleFinder = duplicateRuleFinder;
        }

        [Description("解决方案组件")]
        [HttpGet("SolutionComponents")]
        public IActionResult SolutionComponents([FromQuery]GetSolutionComponentsModel model)
        {
            var data = _duplicateRuleFinder.QueryPaged(x => x.Page(model.Page, model.PageSize), model.SolutionId, model.InSolution);
            if (data.Items.NotEmpty())
            {
                var result = data.Items.Select(x => (new SolutionComponentItem { ObjectId = x.DuplicateRuleId, Name = x.Name, LocalizedName = x.Name, ComponentTypeName = DuplicateRuleDefaults.ModuleName, CreatedOn = x.CreatedOn })).ToList();
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