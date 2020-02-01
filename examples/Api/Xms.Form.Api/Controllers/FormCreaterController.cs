using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using Xms.Api.Core.Controller;
using Xms.Authorization.Abstractions;
using Xms.Core;
using Xms.Form.Abstractions;
using Xms.Form.Api.Models;
using Xms.Form.Domain;
using Xms.Infrastructure.Utility;
using Xms.Schema.Entity;
using Xms.Solution;
using Xms.Web.Framework.Context;

namespace Xms.Form.Api.Controllers
{
    /// <summary>
    /// 表单创建接口
    /// </summary>
    [Route("{org}/api/form/create")]    
    public class FormCreaterController : ApiCustomizeControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IRoleObjectAccessService _roleObjectAccessService;
        private readonly ISystemFormCreater _systemFormCreater;
        private readonly ISystemFormDeleter _systemFormDeleter;
        private readonly ISystemFormFinder _systemFormFinder;
        private readonly ISystemFormUpdater _systemFormUpdater;
        public FormCreaterController(IWebAppContext appContext
            , ISolutionService solutionService
            , IEntityFinder entityFinder
            , IRoleObjectAccessService roleObjectAccessService
            , ISystemFormCreater systemFormCreater
            , ISystemFormDeleter systemFormDeleter
            , ISystemFormFinder systemFormFinder
            , ISystemFormUpdater systemFormUpdater)
            : base(appContext, solutionService)
        {
            _entityFinder = entityFinder;
            _roleObjectAccessService = roleObjectAccessService;
            _systemFormCreater = systemFormCreater;
            _systemFormDeleter = systemFormDeleter;
            _systemFormFinder = systemFormFinder;
            _systemFormUpdater = systemFormUpdater;
        }

        [HttpGet]
        [Description("新建表单")]
        public IActionResult Get(Guid entityid)
        {
            EditFormModel model = new EditFormModel
            {
                SolutionId = SolutionId.Value,
                EntityId = entityid
            };
            return JOk(model);
        }

        [HttpPost]        
        [Description("新建表单-保存")]
        public IActionResult Post(EditFormModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = new SystemForm();
                model.CopyTo(entity);
                entity.SystemFormId = Guid.NewGuid();
                entity.StateCode = RecordState.Enabled;
                entity.FormType = (int)FormType.Main;
                _systemFormCreater.Create(entity);
                return CreateSuccess(new { id = entity.SystemFormId });
            }
            return CreateFailure(GetModelErrors());
        }

    }
}
