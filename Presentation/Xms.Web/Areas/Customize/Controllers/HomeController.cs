using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Flow;
using Xms.Plugin;
using Xms.Schema.Entity;
using Xms.Schema.OptionSet;
using Xms.Solution;
using Xms.Solution.Abstractions;
using Xms.Web.Customize.Models;
using Xms.Web.Framework.Context;

namespace Xms.Web.Customize.Controllers
{
    public class HomeController : CustomizeBaseController
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IOptionSetFinder _optionSetFinder;
        private readonly IEntityPluginFinder _entityPluginFinder;
        private readonly IWorkFlowFinder _workFlowFinder;
        private readonly ISolutionComponentService _solutionComponentService;

        public HomeController(IWebAppContext appContext
            , ISolutionService solutionService
            , IEntityFinder entityFinder
            , IOptionSetFinder optionSetFinder
            , IEntityPluginFinder entityPluginFinder
            , IWorkFlowFinder workFlowFinder
            , ISolutionComponentService solutionComponentService) : base(appContext, solutionService)
        {
            _entityFinder = entityFinder;
            _optionSetFinder = optionSetFinder;
            _entityPluginFinder = entityPluginFinder;
            _workFlowFinder = workFlowFinder;
            _solutionComponentService = solutionComponentService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Description("自定义首页")]
        public IActionResult HomePage()
        {
            HomePageModel model = new HomePageModel();
            FilterContainer<Solution.Domain.SolutionComponent> filter = FilterContainerBuilder.Build<Solution.Domain.SolutionComponent>();
            filter.And(x => x.SolutionId == SolutionId.Value);
            var data = _solutionComponentService.Query(n => n.Select(s => new { s.SolutionComponentId, s.ComponentType, s.SolutionId }).Where(filter));
            var sortedDescriptors = SolutionComponentCollection.SortedDescriptors;
            List<HomePageSolutionComponentModel> solutionComponents = new List<HomePageSolutionComponentModel>();
            var group = data.GroupBy(x => x.ComponentType);
            foreach (var item in group)
            {
                var sortedDescriptor = sortedDescriptors.First(x => x.Module.Identity == item.Key);
                solutionComponents.Add(new HomePageSolutionComponentModel()
                {
                    Name = sortedDescriptor.Module.Name,
                    LocalizedName = sortedDescriptor.Module.LocalizedName,
                    TotalCount = item.Count()
                });
            }
            model.SolutionComponents = solutionComponents;
            model.SolutionId = SolutionId;
            return View(model);
        }
    }
}