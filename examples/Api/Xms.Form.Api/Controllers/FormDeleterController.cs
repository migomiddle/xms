using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Xms.Api.Core.Controller;
using Xms.Authorization.Abstractions;
using Xms.Schema.Entity;
using Xms.Solution;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;

namespace Xms.Form.Api.Controllers
{
    /// <summary>
    /// 表单删除接口
    /// </summary>
    [Route("{org}/api/form/delete")]    
    public class FormDeleterController : ApiCustomizeControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IRoleObjectAccessService _roleObjectAccessService;
        private readonly ISystemFormCreater _systemFormCreater;
        private readonly ISystemFormDeleter _systemFormDeleter;
        private readonly ISystemFormFinder _systemFormFinder;
        private readonly ISystemFormUpdater _systemFormUpdater;
        public FormDeleterController(IWebAppContext appContext
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

        [Description("删除表单")]
        [HttpPost]
        public IActionResult DeleteForm([FromBody]DeleteManyModel model)
        {
            return _systemFormDeleter.DeleteById(model.RecordId).DeleteResult(T);
        }
    }
}
