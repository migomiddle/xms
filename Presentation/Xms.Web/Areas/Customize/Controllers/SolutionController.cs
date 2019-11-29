using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Module.Core;
using Xms.Solution;
using Xms.Solution.Domain;
using Xms.Web.Customize.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;

namespace Xms.Web.Customize.Controllers
{
    /// <summary>
    /// 解决方案管理控制器
    /// </summary>
    public class SolutionController : CustomizeBaseController
    {
        private readonly ISolutionExporter _solutionExporter;
        private readonly ISolutionImporter _solutionImporter;

        public SolutionController(IWebAppContext appContext
            , ISolutionService solutionService
            , ISolutionExporter solutionExporter
            , ISolutionImporter solutionImporter
            )
            : base(appContext, solutionService)
        {
            _solutionExporter = solutionExporter;
            _solutionImporter = solutionImporter;
        }

        [Description("解决方案列表")]
        public IActionResult Index(SolutionModel model)
        {
            FilterContainer<Solution.Domain.Solution> container = FilterContainerBuilder.Build<Solution.Domain.Solution>();
            container.And(n => n.OrganizationId == CurrentUser.OrganizationId);
            if (model.Name.IsNotEmpty())
            {
                container.And(n => n.Name == model.Name);
            }
            if (!model.IsSortBySeted)
            {
                model.SortBy = "Name";
                model.SortDirection = (int)SortDirection.Asc;
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
            PagedList<Solution.Domain.Solution> result = _solutionService.QueryPaged(x => x
                    .Page(model.Page, model.PageSize)
                    .Where(container)
                    .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                    );

            model.Items = result.Items;
            model.TotalItems = result.TotalItems;
            return DynamicResult(model);
        }

        [Description("新建解决方案")]
        public IActionResult CreateSolution()
        {
            CreateSolutionModel model = new CreateSolutionModel();
            return View(model);
        }

        [Description("新建解决方案")]
        [HttpPost]
        public IActionResult CreateSolution(CreateSolutionModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = new Solution.Domain.Solution();
                model.CopyTo(entity);
                entity.SolutionId = Guid.NewGuid();
                entity.OrganizationId = CurrentUser.OrganizationId;
                entity.PublisherId = CurrentUser.SystemUserId;
                entity.CreatedBy = CurrentUser.SystemUserId;
                entity.CreatedOn = DateTime.Now;
                entity.InstalledOn = DateTime.Now;
                _solutionService.Create(entity);
                return CreateSuccess(new { id = entity.SolutionId });
            }
            return CreateFailure(GetModelErrors());
        }

        [Description("编辑解决方案")]
        public IActionResult EditSolution(Guid id)
        {
            var solution = _solutionService.FindById(id);
            if (null == solution)
            {
                return NotFound();
            }
            EditSolutionModel model = new EditSolutionModel();
            solution.CopyTo(model);
            return View(model);
        }

        [Description("编辑解决方案")]
        [HttpPost]
        public IActionResult EditSolution(EditSolutionModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _solutionService.FindById(model.SolutionId);
                model.CopyTo(entity);
                entity.ModifiedOn = DateTime.Now;
                entity.ModifiedBy = CurrentUser.SystemUserId;
                _solutionService.Update(entity);
                return UpdateSuccess();
            }
            return UpdateFailure(GetModelErrors());
        }

        [Description("导出解决方案")]
        public IActionResult ExportSolution(Guid id)
        {
            string file = string.Empty;
            _solutionExporter.Export(id, out file);
            var fileName = System.IO.Path.GetFileName(file);

            return PhysicalFile(file, "application/zip", fileName);
        }

        [Description("导入解决方案")]
        [HttpPost]
        public async Task<IActionResult> ImportSolution(IFormFile file)
        {
            if (file != null)
            {
                await _solutionImporter.ImportAsync(file).ConfigureAwait(false);
                return JOk(T["import_success"]);
            }
            return JError(T["notspecified_file"]);
        }

        [Description("删除解决方案")]
        [HttpPost]
        public IActionResult DeleteSolution([FromBody]DeleteManyModel model)
        {
            return _solutionService.DeleteById(model.RecordId).DeleteResult(T);
        }

        //[Description("解决方案组件")]
        //public IActionResult SolutionComponents(SolutionComponentModel model)
        //{
        //    FilterContainer<SolutionComponent> container = FilterBuilder.Build<SolutionComponent>();
        //    container.And(n => n.SolutionId == model.SolutionId);
        //    if (model.ComponentType.HasValue)
        //    {
        //        container.And(n => n.ComponentType == model.ComponentType.Value);
        //    }
        //    if (!model.IsSortBySeted)
        //    {
        //        model.SortBy = "Name";
        //        model.SortDirection = (int)SortDirection.Asc;
        //    }
        //    if (model.GetAll)
        //    {
        //        model.Page = 1;
        //        model.PageSize = 25000;
        //    }
        //    else if (CurrentUser.UserSettings.PagingLimit > 0)
        //    {
        //        model.PageSize = CurrentUser.UserSettings.PagingLimit;
        //    }
        //    PagedList<SolutionComponent> result = _solutionComponentService.GetComponents(x => x
        //        .Page(model.Page, model.PageSize)
        //        .Where(container)
        //        .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
        //        );

        //    model.Items = result.Items;
        //    model.TotalItems = result.TotalItems;
        //    return DynamicResult(model);
        //}

        //[Description("添加解决方案组件")]
        //[HttpPost]
        //public IActionResult CreateSolutionComponent([FromBody]CreateSolutionComponentModel model)
        //{
        //    if (model.SolutionId.Equals(Guid.Empty) || model.ObjectId.IsEmpty())
        //    {
        //        return JsonError(T["parameter_error"]);
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        List<SolutionComponent> entities = new List<SolutionComponent>();
        //        foreach (var item in model.ObjectId)
        //        {
        //            if (!item.Equals(Guid.Empty))
        //            {
        //                var existsCom = _solutionComponentService.Find(n => n.ObjectId == item && n.ComponentType == model.ComponentType);
        //                if (existsCom == null)
        //                {
        //                    SolutionComponent entity = DomainCreator.Get<SolutionComponent>();
        //                    entity.ComponentType = model.ComponentType;
        //                    entity.CreatedBy = CurrentUser.SystemUserId;
        //                    entity.ObjectId = item;
        //                    entity.SolutionId = model.SolutionId;
        //                    entity.SolutionComponentId = Guid.NewGuid();
        //                    entities.Add(entity);
        //                }
        //            }
        //        }
        //        if (entities.NotEmpty())
        //        {
        //            _solutionComponentService.CreateMany(entities);
        //            return JsonSuccess(T["added_success"]);
        //        }
        //    }
        //    return JsonError(T["added_error"] + ":" + GetModelErrors());
        //}

        //[Description("删除解决方案组件")]
        //[HttpPost]
        //public IActionResult DeleteSolutionComponent([FromBody]DeleteManyModel model)
        //{
        //    return _solutionComponentService.DeleteById(model.RecordId).DeleteResult(T);
        //}
    }

    //[Route("{org}/[area]/solution/[action]")]
    //public class CreateSolutionController : CustomizeBaseController
    //{
    //    public CreateSolutionController(IWebAppContext appContext
    //        , ISolutionService solutionService)
    //        : base(appContext, solutionService)
    //    {
    //    }

    //    [Description("新建解决方案")]
    //    public IActionResult CreateSolution()
    //    {
    //        CreateSolutionModel model = new CreateSolutionModel();
    //        return View($"~/Areas/{WebContext.Area}/Views/Solution/{WebContext.ActionName}.cshtml", model);
    //    }

    //    [Description("新建解决方案")]
    //    [HttpPost]
    //    public IActionResult CreateSolution(CreateSolutionModel model)
    //    {
    //        string msg = string.Empty;
    //        if (ModelState.IsValid)
    //        {
    //            var entity = new Solution.Domain.Solution();
    //            model.CopyTo(entity);
    //            entity.SolutionId = Guid.NewGuid();
    //            entity.OrganizationId = CurrentUser.OrganizationId;
    //            entity.PublisherId = CurrentUser.SystemUserId;
    //            entity.CreatedBy = CurrentUser.SystemUserId;
    //            entity.CreatedOn = DateTime.Now;
    //            entity.InstalledOn = DateTime.Now;
    //            _solutionService.Create(entity);
    //            return JOk(T["created_success"], new { id = entity.SolutionId });
    //        }
    //        msg = GetModelErrors(ModelState);
    //        return JError(T["created_error"] + ": " + msg);
    //    }
    //}

    [Route("{org}/[area]/solution/[action]")]
    public class EditSolutionController : CustomizeBaseController
    {
        public EditSolutionController(IWebAppContext appContext
            , ISolutionService solutionService)
            : base(appContext, solutionService)
        {
        }

        [Description("编辑解决方案")]
        public IActionResult EditSolution(Guid id)
        {
            var solution = _solutionService.FindById(id);
            if (null == solution)
            {
                return NotFound();
            }
            EditSolutionModel model = new EditSolutionModel();
            solution.CopyTo(model);
            return View($"~/Areas/{WebContext.Area}/Views/Solution/{WebContext.ActionName}.cshtml", model);
        }

        [Description("编辑解决方案")]
        [HttpPost]
        public IActionResult EditSolution(EditSolutionModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _solutionService.FindById(model.SolutionId);
                model.CopyTo(entity);
                entity.ModifiedOn = DateTime.Now;
                entity.ModifiedBy = CurrentUser.SystemUserId;
                _solutionService.Update(entity);
                return UpdateSuccess();
            }
            return UpdateFailure(GetModelErrors());
        }
    }

    //[Route("{org}/[area]/solution/[action]")]
    //public class ImportSolutionController : CustomizeBaseController
    //{
    //    private readonly ISolutionImporter _solutionImporter;
    //    private readonly IWebHelper _webHelper;
    //    public ImportSolutionController(Framework.Context.IWebAppContext appContext
    //        , ISolutionService solutionService
    //        , ISolutionImporter solutionImporter
    //        , IWebHelper webHelper)
    //        : base(appContext, solutionService)
    //    {
    //        _solutionImporter = solutionImporter;
    //        _webHelper = webHelper;
    //    }

    //    [Description("导入解决方案")]
    //    [HttpPost]
    //    public async Task<IActionResult> ImportSolution(List<IFormFile> files)
    //    {
    //        if (files.NotEmpty())
    //        {
    //            var postedfile = files[0];
    //            var dir = _webHelper.MapPath("~/solution/import/" + DateTime.Now.ToString("yyMMddhhmmss"));
    //            var d = System.IO.Directory.CreateDirectory(dir);
    //            var file = dir + "/" + postedfile.FileName;
    //            await postedfile.SaveAs(file, _webHelper);
    //            _solutionImporter.Import(file, CurrentUser.SystemUserId);
    //            return JsonSuccess(T["import_success"]);
    //        }
    //        return JsonError(T["notspecified_file"]);
    //    }
    //}

    //[Route("{org}/[area]/solution/[action]")]
    //public class ExportSolutionController : CustomizeBaseController
    //{
    //    private readonly ISolutionExporter _solutionExporter;
    //    public ExportSolutionController(Framework.Context.IWebAppContext appContext
    //        , ISolutionService solutionService
    //        , ISolutionExporter solutionExporter)
    //        : base(appContext, solutionService)
    //    {
    //        _solutionExporter = solutionExporter;
    //    }

    //    [Description("导出解决方案")]
    //    public IActionResult ExportSolution(Guid id)
    //    {
    //        string file = string.Empty;
    //        _solutionExporter.Export(id, out file);
    //        var fileName = System.IO.Path.GetFileName(file);

    //        return File(file, "application/zip", fileName);
    //    }
    //}

    [Route("{org}/[area]/solution/[action]")]
    public class SolutionComponentController : CustomizeBaseController
    {
        private readonly ISolutionComponentService _solutionComponentService;

        public SolutionComponentController(IWebAppContext appContext
            , ISolutionService solutionService
            , ISolutionComponentService solutionComponentService)
            : base(appContext, solutionService)
        {
            _solutionComponentService = solutionComponentService;
        }

        [Description("解决方案组件")]
        public IActionResult Components(SolutionComponentModel model)
        {
            if (!model.IsSortBySeted)
            {
                model.SortBy = "Name";
                model.SortDirection = (int)SortDirection.Asc;
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
            return DynamicResult(model, $"~/Areas/{WebContext.Area}/Views/Solution/{WebContext.ActionName}.cshtml");
        }

        [Description("添加解决方案组件")]
        [HttpPost]
        public IActionResult CreateSolutionComponent([FromBody]CreateSolutionComponentModel model)
        {
            if (model.SolutionId.Equals(Guid.Empty) || model.ObjectId.IsEmpty())
            {
                return JError(T["parameter_error"]);
            }
            if (ModelState.IsValid)
            {
                List<SolutionComponent> entities = new List<SolutionComponent>();
                foreach (var item in model.ObjectId)
                {
                    if (!item.Equals(Guid.Empty))
                    {
                        var comDesc = ModuleCollection.GetDescriptor(model.ComponentType);
                        var existsCom = _solutionComponentService.Find(n => n.ObjectId == item && n.ComponentType == comDesc.Identity);
                        if (existsCom == null)
                        {
                            SolutionComponent entity = new SolutionComponent
                            {
                                ComponentType = comDesc.Identity,
                                CreatedBy = CurrentUser.SystemUserId,
                                ObjectId = item,
                                SolutionId = model.SolutionId
                            };
                            entities.Add(entity);
                        }
                    }
                }
                if (entities.NotEmpty())
                {
                    _solutionComponentService.CreateMany(entities);
                    return JOk(T["added_success"]);
                }
            }
            return JError(T["added_error"] + ":" + GetModelErrors());
        }

        [Description("删除解决方案组件")]
        [HttpPost]
        public IActionResult DeleteSolutionComponent([FromBody]DeleteSolutionComponentModel model)
        {
            return _solutionComponentService.DeleteObject(model.SolutionId, model.ComponentType, model.RecordId).DeleteResult(T);
        }

        /// <summary>
        /// 解决方案组件对话框
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description("解决方案组件对话框")]
        [HttpPost]
        public IActionResult ComponentsDialog([FromBody]SolutionComponentDialogModel model)
        {
            model.ComponentDescriptor = Solution.Abstractions.SolutionComponentCollection.GetDescriptor(model.ComponentType);

            return View($"~/Areas/{WebContext.Area}/Views/Solution/{WebContext.ActionName}.cshtml", model);
        }
    }
}