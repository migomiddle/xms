using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xms.Api.Core.Controller;
using Xms.Authorization.Abstractions;
using Xms.Form.Abstractions;
using Xms.Form.Api.Models;
using Xms.Form.Domain;
using Xms.Infrastructure.Utility;
using Xms.Schema.Entity;
using Xms.Solution;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;

namespace Xms.Form.Api.Controllers
{
    /// <summary>
    /// 表单更新接口
    /// </summary>
    [Route("{org}/api/form/update")]    
    public class FormUpdaterController : ApiCustomizeControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IRoleObjectAccessService _roleObjectAccessService;
        private readonly ISystemFormCreater _systemFormCreater;
        private readonly ISystemFormDeleter _systemFormDeleter;
        private readonly ISystemFormFinder _systemFormFinder;
        private readonly ISystemFormUpdater _systemFormUpdater;
        public FormUpdaterController(IWebAppContext appContext
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

        [HttpPost]        
        [Description("表单信息保存")]
        public IActionResult Post(EditFormModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _systemFormFinder.FindById(model.SystemFormId);
                model.CopyTo(entity);
                entity.ModifiedBy = CurrentUser.SystemUserId;
                entity.ModifiedOn = DateTime.Now;
                _systemFormUpdater.Update(entity, true);
                return UpdateSuccess(new { id = entity.SystemFormId });
            }
            return UpdateFailure(GetModelErrors());
        }

        
        [HttpPost("copy")]
        [Description("表单复制")]
        public IActionResult CopyForm(Guid systemFormId, string name)
        {
            string msg = string.Empty;
            if (!systemFormId.Equals(Guid.Empty))
            {
                var entity = _systemFormFinder.FindById(systemFormId);
                if (entity != null)
                {
                    var newForm = new SystemForm();
                    entity.CopyTo(newForm);
                    newForm.Name = name.IfEmpty(entity.Name + " Copy");
                    newForm.IsDefault = false;
                    newForm.CreatedBy = CurrentUser.SystemUserId;
                    newForm.CreatedOn = DateTime.Now;
                    newForm.SystemFormId = Guid.NewGuid();
                    List<Guid> assignRolesId = null;
                    if (entity.AuthorizationEnabled)
                    {
                        var assignRoles = _roleObjectAccessService.Query(entity.SystemFormId, FormDefaults.ModuleName);
                        if (assignRoles.NotEmpty())
                        {
                            assignRolesId = assignRoles.Select(x => x.RoleId).ToList();
                        }
                    }
                    _systemFormCreater.Create(newForm);
                    return SaveSuccess(new { id = entity.SystemFormId });
                }
            }
            return SaveFailure();
        }

        [Description("设置表单默认状态")]
        [HttpPost("setdefault")]
        public IActionResult SetFormDefault([FromBody]SetFormDefaultModel model)
        {
            return _systemFormUpdater.UpdateDefault(model.EntityId, model.RecordId.First()).UpdateResult(T);
        }

        [Description("设置表单可用状态")]
        [HttpPost("setstate")]
        public IActionResult SetFormState([FromBody]SetRecordStateModel model)
        {
            return _systemFormUpdater.UpdateState(model.IsEnabled, model.RecordId).UpdateResult(T);
        }

        [Description("设置表单权限启用状态")]
        [HttpPost("setauthstate")]
        public IActionResult SetFormAuthorizationState([FromBody]SetFormAuthorizationStateModel model)
        {
            return _systemFormUpdater.UpdateAuthorization(model.IsAuthorization, model.RecordId).UpdateResult(T);
        }

        [Description("设置表单按钮")]
        [HttpPost("setbutton")]
        public IActionResult SetFormButtons([FromBody]SetFormButtonsModel model)
        {
            var entity = _systemFormFinder.FindById(model.RecordId);
            if (entity != null)
            {
                entity.IsCustomButton = model.IsCustomButton;
                entity.CustomButtons = model.IsCustomButton && model.Buttons.NotEmpty() ? model.Buttons.SerializeToJson() : "";
                return _systemFormUpdater.Update(entity, false).UpdateResult(T);
            }
            return NotFound();
        }
    }
}

