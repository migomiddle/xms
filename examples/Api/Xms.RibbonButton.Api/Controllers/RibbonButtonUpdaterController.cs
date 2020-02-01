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
using Xms.Web.Framework.Mvc;

namespace Xms.RibbonButton.Api.Controllers
{
    /// <summary>
    /// 按钮更新接口
    /// </summary>
    [Route("{org}/api/ribbonbutton/update")]
    public class RibbonButtonUpdaterController : ApiCustomizeControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IRibbonButtonCreater _ribbonButtonCreater;
        private readonly IRibbonButtonUpdater _ribbonButtonUpdater;
        private readonly IRibbonButtonFinder _ribbonButtonFinder;
        private readonly IRibbonButtonDeleter _ribbonButtonDeleter;
        private readonly IWebResourceFinder _webResourceFinder;

        public RibbonButtonUpdaterController(IWebAppContext appContext
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
        
        [HttpPost]        
        [Description("按钮信息保存")]
        public IActionResult Post(EditRibbonButtonModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _ribbonButtonFinder.FindById(model.RibbonButtonId.Value);
                model.CopyTo(entity);

                _ribbonButtonUpdater.Update(entity);

                return UpdateSuccess(new { id = entity.RibbonButtonId });
            }
            return UpdateFailure(GetModelErrors());
        }

        [Description("设置按钮可用状态")]
        [HttpPost("setstate")]
        public IActionResult SetRibbonButtonState([FromBody]SetRecordStateModel model)
        {
            return _ribbonButtonUpdater.UpdateState( model.IsEnabled, model.RecordId).UpdateResult(T);
        }

    }
}