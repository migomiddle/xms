using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Xms.Localization.Abstractions;
using Xms.Solution;
using Xms.Web.Framework.Context;

namespace Xms.Web.Customize.Controllers
{
    /// <summary>
    /// 平台多语言标签
    /// </summary>
    public class LocalizationController : CustomizeBaseController
    {
        private readonly ILocalizedTextProvider _localizedTextProvider;

        public LocalizationController(IWebAppContext appContext
            , ISolutionService solutionService
            , ILocalizedTextProvider localizedTextProvider)
            : base(appContext, solutionService)
        {
            _localizedTextProvider = localizedTextProvider;
        }

        [Description("多语言显示标签")]
        public IActionResult Index()
        {
            return View();
        }
    }
}