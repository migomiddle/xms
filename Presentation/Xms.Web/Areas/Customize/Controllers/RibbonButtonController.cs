using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.RibbonButton;
using Xms.RibbonButton.Abstractions;
using Xms.Schema.Entity;
using Xms.Solution;
using Xms.Web.Customize.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;
using Xms.WebResource;

namespace Xms.Web.Customize.Controllers
{
    /// <summary>
    /// 按钮管理控制器
    /// </summary>
    public class RibbonButtonController : CustomizeBaseController
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IRibbonButtonCreater _ribbonButtonCreater;
        private readonly IRibbonButtonUpdater _ribbonButtonUpdater;
        private readonly IRibbonButtonFinder _ribbonButtonFinder;
        private readonly IRibbonButtonDeleter _ribbonButtonDeleter;
        private readonly IWebResourceFinder _webResourceFinder;

        public RibbonButtonController(IWebAppContext appContext
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

        [Description("按钮列表")]
        public IActionResult Index(RibbonButtonModel model)
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

            FilterContainer<RibbonButton.Domain.RibbonButton> filter = FilterContainerBuilder.Build<RibbonButton.Domain.RibbonButton>();
            filter.And(n => n.EntityId == model.EntityId);
            if (model.ShowArea.HasValue)
            {
                filter.And(n => n.ShowArea == model.ShowArea.Value);
            }
            if (model.Label.IsNotEmpty())
            {
                filter.And(n => n.Label.Like(model.Label));
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
            if (!model.IsSortBySeted)
            {
                model.SortBy = "showarea";
            }
            model.PageSize = model.PageSize > WebContext.PlatformSettings.MaxFetchRecords ? WebContext.PlatformSettings.MaxFetchRecords : model.PageSize;
            PagedList<RibbonButton.Domain.RibbonButton> result = _ribbonButtonFinder.QueryPaged(x => x
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
        [Description("新建按钮")]
        public IActionResult CreateRibbonButton(Guid entityid)
        {
            EditRibbonButtonModel model = new EditRibbonButtonModel
            {
                SolutionId = SolutionId.Value,
                EntityId = entityid,
                StateCode = Core.RecordState.Enabled,
                ShowLabel = true
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("新建按钮-保存")]
        public IActionResult CreateRibbonButton(EditRibbonButtonModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = new RibbonButton.Domain.RibbonButton();
                model.CopyTo(entity);
                entity.RibbonButtonId = Guid.NewGuid();
                entity.CreatedBy = CurrentUser.SystemUserId;
                if (entity.ShowArea != RibbonButtonArea.Form && entity.ShowArea != RibbonButtonArea.ListRow)
                {
                    entity.CommandRules = string.Empty;
                }
                _ribbonButtonCreater.Create(entity);

                return CreateSuccess(new { id = entity.RibbonButtonId });
            }
            return CreateFailure(GetModelErrors());
        }

        [HttpGet]
        [Description("按钮编辑")]
        public IActionResult EditRibbonButton(Guid id)
        {
            EditRibbonButtonModel model = new EditRibbonButtonModel();
            if (!id.Equals(Guid.Empty))
            {
                var entity = _ribbonButtonFinder.FindById(id);
                if (entity != null)
                {
                    entity.CopyTo(model);
                    model.WebResourceName = entity.JsLibrary;
                    if (entity.JsLibrary.IsNotEmpty() && entity.JsLibrary.StartsWith("$webresource:"))
                    {
                        var wr = _webResourceFinder.FindById(Guid.Parse(entity.JsLibrary.Replace("$webresource:", "")));
                        if (wr != null)
                        {
                            model.WebResourceName = wr.Name;
                        }
                        else
                        {
                            model.JsLibrary = string.Empty;
                            model.JsAction = string.Empty;
                        }
                    }
                    return View(model);
                }
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("按钮信息保存")]
        public IActionResult EditRibbonButton(EditRibbonButtonModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _ribbonButtonFinder.FindById(model.RibbonButtonId.Value);
                model.CopyTo(entity);
                if (entity.ShowArea != RibbonButtonArea.Form && entity.ShowArea != RibbonButtonArea.ListRow)
                {
                    entity.CommandRules = string.Empty;
                }

                _ribbonButtonUpdater.Update(entity);

                return UpdateSuccess(new { id = entity.RibbonButtonId });
            }
            return UpdateFailure(GetModelErrors());
        }

        [Description("设置按钮可用状态")]
        [HttpPost]
        public IActionResult SetRibbonButtonState([FromBody]SetRecordStateModel model)
        {
            return _ribbonButtonUpdater.UpdateState(model.IsEnabled, model.RecordId).UpdateResult(T);
        }

        [Description("删除按钮")]
        [HttpPost]
        public IActionResult DeleteRibbonButton([FromBody]DeleteManyModel model)
        {
            return _ribbonButtonDeleter.DeleteById(model.RecordId).DeleteResult(T);
        }

        /// <summary>
        /// 按钮对话框
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dm"></param>
        /// <returns></returns>
        [Description("按钮对话框")]
        [HttpPost]
        public IActionResult Dialog([FromBody]CustomButtonsDialogModel model)
        {
            FilterContainer<RibbonButton.Domain.RibbonButton> container = FilterContainerBuilder.Build<RibbonButton.Domain.RibbonButton>();
            container.And(n => n.EntityId == model.EntityId);
            if (model.ButtonArea.HasValue)
            {
                container.And(n => n.ShowArea == model.ButtonArea.Value);
            }
            var result = _ribbonButtonFinder.Query(x => x
                .Where(container)
            );
            model.Buttons = result;

            return View(model);
        }
    }
}