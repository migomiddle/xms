using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xms.Api.Core.Controller;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.RibbonButton.Api.Models;
using Xms.Schema.Entity;
using Xms.Solution;
using Xms.Web.Framework.Context;
using Xms.WebResource;
using Xms.Infrastructure.Utility;
using Xms.Core.Data;
using System.ComponentModel;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;

namespace Xms.RibbonButton.Api.Controllers
{
    /// <summary>
    /// 按钮删除接口
    /// </summary>
    [Route("{org}/api/ribbonbutton/delete")]
    public class RibbonButtonDeleterController : ApiCustomizeControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IRibbonButtonCreater _ribbonButtonCreater;
        private readonly IRibbonButtonUpdater _ribbonButtonUpdater;
        private readonly IRibbonButtonFinder _ribbonButtonFinder;
        private readonly IRibbonButtonDeleter _ribbonButtonDeleter;
        private readonly IWebResourceFinder _webResourceFinder;

        public RibbonButtonDeleterController(IWebAppContext appContext
            , ISolutionService solutionService
            , IEntityFinder entityFinder
            , IRibbonButtonCreater ribbonButtonCreater
            , IRibbonButtonFinder ribbonButtonFinder
            , IRibbonButtonUpdater ribbonButtonUpdater
            , IRibbonButtonDeleter ribbonButtonDeleter
            , IWebResourceFinder webResourceFinder)
          : base(appContext, solutionService)
        {
            _entityFinder = entityFinder;
            _ribbonButtonCreater = ribbonButtonCreater;
            _ribbonButtonFinder = ribbonButtonFinder;
            _ribbonButtonUpdater = ribbonButtonUpdater;
            _ribbonButtonDeleter = ribbonButtonDeleter;
            _webResourceFinder = webResourceFinder;
        }
                       
        [Description("删除按钮")]
        [HttpPost]
        public IActionResult Post(DeleteManyModel model)
        {
            return _ribbonButtonDeleter.DeleteById(model.RecordId).DeleteResult(T);
        }


    }
}