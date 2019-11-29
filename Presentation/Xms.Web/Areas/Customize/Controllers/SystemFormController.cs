using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xms.Authorization.Abstractions;
using Xms.Core;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Form;
using Xms.Form.Abstractions;
using Xms.Form.Domain;
using Xms.Infrastructure.Utility;
using Xms.Schema.Entity;
using Xms.Solution;
using Xms.Web.Customize.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;

namespace Xms.Web.Customize.Controllers
{
    /// <summary>
    /// 表单管理控制器
    /// </summary>
    public class SystemFormController : CustomizeBaseController
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IRoleObjectAccessService _roleObjectAccessService;
        private readonly ISystemFormCreater _systemFormCreater;
        private readonly ISystemFormDeleter _systemFormDeleter;
        private readonly ISystemFormFinder _systemFormFinder;
        private readonly ISystemFormUpdater _systemFormUpdater;

        public SystemFormController(IWebAppContext appContext
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

        [Description("表单列表")]
        public IActionResult Index(FormModel model)
        {
            if (model.EntityId.Equals(Guid.Empty))
            {
                return NotFound();
            }
            var entity = _entityFinder.FindById(model.EntityId);
            if (entity == null)
            {
                return NotFound();
            }
            model.Entity = entity;
            if (!model.LoadData)
            {
                return DynamicResult(model);
            }

            FilterContainer<SystemForm> filter = FilterContainerBuilder.Build<SystemForm>();
            filter.And(n => n.EntityId == model.EntityId);
            if (model.Name.IsNotEmpty())
            {
                filter.And(n => n.Name.Like(model.Name));
            }

            if (model.GetAll)
            {
                model.Page = 1;
                model.PageSize = WebContext.PlatformSettings.MaxFetchRecords;
            }
            else if (!model.PageSizeBySeted && CurrentUser.UserSettings.PagingLimit > 0)
            {
                model.PageSize = CurrentUser.UserSettings.PagingLimit;
            }
            model.PageSize = model.PageSize > WebContext.PlatformSettings.MaxFetchRecords ? WebContext.PlatformSettings.MaxFetchRecords : model.PageSize;
            PagedList<SystemForm> result = _systemFormFinder.QueryPaged(x => x
                    .Page(model.Page, model.PageSize)
                    .Where(filter)
                    .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                    );

            model.Items = result.Items;
            model.TotalItems = result.TotalItems;
            model.SolutionId = SolutionId.Value;
            return DynamicResult(model);
        }

        [HttpGet]
        [Description("新建表单")]
        public IActionResult CreateForm(Guid entityid)
        {
            EditFormModel model = new EditFormModel
            {
                SolutionId = SolutionId.Value,
                EntityId = entityid
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("新建表单-保存")]
        public IActionResult CreateForm(EditFormModel model)
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

        [HttpGet]
        [Description("表单编辑")]
        public IActionResult EditForm(Guid id)
        {
            EditFormModel model = new EditFormModel();
            if (!id.Equals(Guid.Empty))
            {
                var entity = _systemFormFinder.FindById(id);
                if (entity != null)
                {
                    entity.CopyTo(model);
                    return View(model);
                }
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("表单信息保存")]
        public IActionResult EditForm(EditFormModel model)
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

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Description("表单复制")]
        public IActionResult CopyForm([FromBody]SetCopyFromModel model)
        {
            string msg = string.Empty;
            if (!model.SystemFormId.Equals(Guid.Empty))
            {
                var entity = _systemFormFinder.FindById(model.SystemFormId);
                if (entity != null)
                {
                    var newForm = new SystemForm();
                    entity.CopyTo(newForm);
                    newForm.Name = model.name.IfEmpty(entity.Name + " Copy");
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

        [Description("删除表单")]
        [HttpPost]
        public IActionResult DeleteForm([FromBody]DeleteManyModel model)
        {
            return _systemFormDeleter.DeleteById(model.RecordId).DeleteResult(T);
        }

        [Description("设置表单默认状态")]
        [HttpPost]
        public IActionResult SetFormDefault([FromBody]SetFormDefaultModel model)
        {
            return _systemFormUpdater.UpdateDefault(model.EntityId, model.RecordId.First()).UpdateResult(T);
        }

        [Description("设置表单可用状态")]
        [HttpPost]
        public IActionResult SetFormState([FromBody]SetRecordStateModel model)
        {
            return _systemFormUpdater.UpdateState(model.IsEnabled, model.RecordId).UpdateResult(T);
        }

        [Description("设置表单权限启用状态")]
        [HttpPost]
        public IActionResult SetFormAuthorizationState([FromBody]SetFormAuthorizationStateModel model)
        {
            return _systemFormUpdater.UpdateAuthorization(model.IsAuthorization, model.RecordId).UpdateResult(T);
        }

        [Description("设置表单按钮")]
        [HttpPost]
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