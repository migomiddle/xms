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

namespace Xms.RibbonButton.Api.Controllers
{
    /// <summary>
    /// 按钮新增接口
    /// </summary>
    [Route("{org}/api/ribbonbutton/create")]
    public class RibbonButtonCreaterController : ApiCustomizeControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IRibbonButtonCreater _ribbonButtonCreater;
        private readonly IRibbonButtonUpdater _ribbonButtonUpdater;
        private readonly IRibbonButtonFinder _ribbonButtonFinder;
        private readonly IRibbonButtonDeleter _ribbonButtonDeleter;
        private readonly IWebResourceFinder _webResourceFinder;

        public RibbonButtonCreaterController(IWebAppContext appContext
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
             
        [HttpGet]
        [Description("新建按钮")]
        public IActionResult Get(Guid entityid)
        {
            EditRibbonButtonModel model = new EditRibbonButtonModel
            {
                SolutionId = SolutionId.Value,
                EntityId = entityid,
                StateCode = Core.RecordState.Enabled
            };
            return JOk(model);
        }

        [HttpPost]        
        [Description("新建按钮-保存")]
        public IActionResult Post(EditRibbonButtonModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = new RibbonButton.Domain.RibbonButton();
                model.CopyTo(entity);
                entity.RibbonButtonId = Guid.NewGuid();
                entity.CreatedBy = CurrentUser.SystemUserId;
                _ribbonButtonCreater.Create(entity);

                return CreateSuccess(new { id = entity.RibbonButtonId });
            }
            return CreateFailure(GetModelErrors());
        }


    }
}