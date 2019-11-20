using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Linq;
using Xms.Business.SerialNumber;
using Xms.Core.Context;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;
using Xms.Web.Api.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;

namespace Xms.Web.Api
{
    [Route("{org}/api/[controller]")]
    public class SerialNumberRuleController : ApiControllerBase
    {
        private readonly ISerialNumberRuleFinder _serialNumberRuleFinder;

        public SerialNumberRuleController(IWebAppContext appContext
            , ISerialNumberRuleFinder serialNumberRuleFinder)
            : base(appContext)
        {
            _serialNumberRuleFinder = serialNumberRuleFinder;
        }

        [Description("解决方案组件")]
        [HttpGet("SolutionComponents")]
        public IActionResult SolutionComponents([FromQuery]GetSolutionComponentsModel model)
        {
            var data = _serialNumberRuleFinder.QueryPaged(x => x.Page(model.Page, model.PageSize), model.SolutionId, model.InSolution);
            if (data.Items.NotEmpty())
            {
                var result = data.Items.Select(x => (new SolutionComponentItem { ObjectId = x.SerialNumberRuleId, Name = x.Name, LocalizedName = x.Name, ComponentTypeName = SerialNumberRuleDefaults.ModuleName, CreatedOn = x.CreatedOn })).ToList();
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