using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xms.Dependency;
using Xms.Dependency.Abstractions;
using Xms.Solution;
using Xms.Web.Customize.Controllers;
using Xms.Web.Customize.Models;
using Xms.Web.Framework.Context;

namespace Xms.Web.Areas.Customize.Controllers
{
    /// <summary>
    /// 依赖关系控制器
    /// </summary>
    public class DependencyController : CustomizeBaseController
    {
        private readonly IDependencyLookupFactory _dependencyLookupFactory;

        public DependencyController(IWebAppContext appContext
            , ISolutionService solutionService
            , IDependencyLookupFactory dependencyLookupFactory
            ) : base(appContext, solutionService)
        {
            _dependencyLookupFactory = dependencyLookupFactory;
        }

        [Route("/error/dependentexception")]
        public IActionResult DependentComponents([FromBody]List<DependentDescriptor> dependents)
        {
            DependentComponentsModel model = new DependentComponentsModel
            {
                Items = dependents
            };
            return View(model);
        }

        [Description("查看依赖项")]
        public IActionResult CheckDependents(int requiredComponentType, Guid requiredId)
        {
            DependentComponentsModel model = new DependentComponentsModel
            {
                Items = _dependencyLookupFactory.GetDependents(requiredComponentType, requiredId)
            };
            return View("DependentComponents", model);
        }
    }
}