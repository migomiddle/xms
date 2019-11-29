using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Form;
using Xms.Form.Abstractions;
using Xms.Form.Domain;
using Xms.Infrastructure.Utility;
using Xms.Solution;
using Xms.Web.Customize.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;

namespace Xms.Web.Customize.Controllers
{
    /// <summary>
    /// 仪表板控制器
    /// </summary>
    public class DashBoardController : CustomizeBaseController
    {
        private readonly ISystemFormCreater _systemFormCreater;
        private readonly ISystemFormUpdater _systemFormUpdater;
        private readonly ISystemFormFinder _systemFormFinder;
        private readonly ISystemFormDeleter _systemFormDeleter;

        public DashBoardController(IWebAppContext appContext
            , ISolutionService solutionService
            , ISystemFormCreater systemFormCreater
            , ISystemFormUpdater systemFormUpdater
            , ISystemFormFinder systemFormFinder
            , ISystemFormDeleter systemFormDeleter)
            : base(appContext, solutionService)
        {
            _systemFormCreater = systemFormCreater;
            _systemFormUpdater = systemFormUpdater;
            _systemFormFinder = systemFormFinder;
            _systemFormDeleter = systemFormDeleter;
        }

        [Description("仪表板列表")]
        public IActionResult Index(DashBoardModel model)
        {
            if (!model.LoadData)
            {
                return DynamicResult(model);
            }
            FilterContainer<SystemForm> filter = FilterContainerBuilder.Build<SystemForm>();
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
                , SolutionId.Value, true, FormType.Dashboard);

            model.Items = result.Items;
            model.TotalItems = result.TotalItems;
            model.SolutionId = SolutionId.Value;
            return DynamicResult(model);
        }

        [Description("新建仪表板")]
        public IActionResult CreateDashBoard()
        {
            EditDashBoardModel model = new EditDashBoardModel();
            model.SolutionId = SolutionId.Value;
            model.EntityId = Guid.Empty;
            return View(model);
        }

        [Description("新建仪表板")]
        [HttpPost]
        public IActionResult CreateDashBoard([FromBody]EditDashBoardModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = new SystemForm();
                model.CopyTo(entity);
                entity.SystemFormId = Guid.NewGuid();
                entity.SolutionId = SolutionId.Value;
                entity.EntityId = Guid.Empty;
                entity.StateCode = Core.RecordState.Enabled;
                entity.FormType = (int)FormType.Dashboard;
                entity.CreatedBy = CurrentUser.SystemUserId;
                entity.CreatedOn = DateTime.Now;
                return _systemFormCreater.Create(entity).CreateResult(T, new { id = entity.SystemFormId });
            }
            return CreateFailure(GetModelErrors());
        }

        [Description("编辑仪表板")]
        public IActionResult EditDashBoard(Guid id)
        {
            EditDashBoardModel model = new EditDashBoardModel();
            if (!id.Equals(Guid.Empty))
            {
                var entity = _systemFormFinder.FindById(id);
                if (entity != null)
                {
                    entity.CopyTo(model);
                    model.SystemFormId = id;
                    return View(model);
                }
            }
            return NotFound();
        }

        [Description("编辑仪表板")]
        [HttpPost]
        public IActionResult EditDashBoard([FromBody]EditDashBoardModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _systemFormFinder.FindById(model.SystemFormId);
                model.CopyTo(entity);
                entity.ModifiedBy = CurrentUser.SystemUserId;
                entity.ModifiedOn = DateTime.Now;
                _systemFormUpdater.Update(entity, true);
                return UpdateSuccess();
            }
            return JError(GetModelErrors());
        }

        [Description("删除仪表板")]
        [HttpPost]
        public IActionResult DeleteDashBoard([FromBody]DeleteManyModel model)
        {
            return _systemFormDeleter.DeleteById(model.RecordId).DeleteResult(T);
        }

        [Description("设置仪表板可用状态")]
        [HttpPost]
        public IActionResult SetDashBoardState([FromBody]SetDashBoardStateModel model)
        {
            return _systemFormUpdater.UpdateState(model.IsEnabled, model.RecordId).UpdateResult(T);
        }

        [Description("仪表板复制")]
        public IActionResult CopyDashboard(Guid systemFormId, string name)
        {
            if (!systemFormId.Equals(Guid.Empty))
            {
                var entity = _systemFormFinder.FindById(systemFormId);
                if (entity != null)
                {
                    var newForm = new SystemForm();
                    entity.CopyTo(newForm);
                    newForm.Name = name.IfEmpty(entity.Name + " Copy");
                    newForm.CreatedBy = CurrentUser.SystemUserId;
                    newForm.CreatedOn = DateTime.Now;
                    newForm.SystemFormId = Guid.NewGuid();
                    return _systemFormCreater.Create(newForm).CreateResult(T, new { id = entity.SystemFormId });
                }
            }
            return CreateFailure(GetModelErrors());
        }
    }
}