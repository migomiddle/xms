using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xms.Configuration;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.File.Extensions;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.Localization.Domain;
using Xms.Solution;
using Xms.Web.Customize.Models;
using Xms.Web.Framework.Context;

namespace Xms.Web.Customize.Controllers
{
    /// <summary>
    /// 多语言标签管理控制器
    /// </summary>
    public class LocalizedLabelController : CustomizeBaseController
    {
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedLabelImportExport _localizedLabelImportExport;
        private readonly IWebHelper _webHelper;
        private readonly ISettingFinder _settingFinder;

        public LocalizedLabelController(IWebAppContext appContext
            , ILocalizedLabelService localizedLabelService
            , ISolutionService solutionService
            , ILanguageService languageService
            , ILocalizedLabelImportExport localizedLabelImportExport
            , IWebHelper webHelper
            , ISettingFinder settingFinder)
            : base(appContext, solutionService)
        {
            _localizedLabelService = localizedLabelService;
            _languageService = languageService;
            _localizedLabelImportExport = localizedLabelImportExport;
            _webHelper = webHelper;
            _settingFinder = settingFinder;
        }

        [Description("多语言显示标签")]
        public IActionResult Index(LocalizationLabelsModel model)
        {
            return ToBePerfected();
            //if (!model.IsSortBySeted)
            //{
            //    model.SortBy = "ObjectColumnName";
            //}

            //if (model.GetAll)
            //{
            //    model.Page = 1;
            //    model.PageSize = WebContext.PlatformSettings.MaxFetchRecords;
            //}
            //else if (!model.PageSizeBySeted && CurrentUser.UserSettings.PagingLimit > 0)
            //{
            //    model.PageSize = CurrentUser.UserSettings.PagingLimit;
            //}
            //model.PageSize = model.PageSize > WebContext.PlatformSettings.MaxFetchRecords ? WebContext.PlatformSettings.MaxFetchRecords : model.PageSize;
            //FilterContainer<LocalizedLabel> filter = FilterContainerBuilder.Build<LocalizedLabel>();
            //if (model.TypeCode.HasValue)
            //{
            //    filter.And(n => n.LabelTypeCode == model.TypeCode.Value);
            //}

            //var result = _localizedLabelService.QueryPaged(n => n.Page(model.Page, model.PageSize)
            //.Where(filter)
            //.Sort(s => s.OnFile(model.SortBy).ByDirection(model.SortDirection)));

            //model.Items = result.Items;
            //model.TotalItems = result.TotalItems;
            //model.SolutionId = SolutionId.Value;
            //return DynamicResult(model);
        }

        [Description("更新多语言显示标签")]
        public IActionResult UpdateLocalizationLabel(Guid objectId, string columnName)
        {
            UpdateLocalizationLabelModel model = new UpdateLocalizationLabelModel
            {
                ObjectLabels = _localizedLabelService.Query(n => n.Where(f => f.ObjectColumnName == columnName && f.ObjectId == objectId)),
                Languages = _languageService.Query(n => n.Sort(s => s.SortAscending(f => f.Name))),
                ObjectColumnName = columnName,
                ObjectId = objectId
            };
            return View(model);
        }

        [Description("更新多语言显示标签")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateLocalizationLabel(UpdateLocalizationLabelModel model)
        {
            if (ModelState.IsValid)
            {
                var labels = _localizedLabelService.Query(n => n.Where(f => f.ObjectColumnName == model.ObjectColumnName && f.ObjectId == model.ObjectId));
                int i = 0;

                foreach (var item in model.Label)
                {
                    var original = labels.Find(n => (int)n.LanguageId == model.LanguageId[i]);
                    if (original != null)
                    {
                        if (item.IsEmpty())
                        {
                            //delete
                            _localizedLabelService.DeleteById(original.LocalizedLabelId);
                        }
                        else
                        {
                            //update
                            _localizedLabelService.Update(n => n.Set(f => f.Label, item).Where(f => f.LocalizedLabelId == original.LocalizedLabelId));
                        }
                    }
                    else
                    {
                        //create
                        //_localizedLabelService.Create(SolutionDefaults.DefaultSolutionId, item, labels.First().LabelTypeCode, model.ObjectColumnName, model.ObjectId, (LanguageEnum)Enum.ToObject(typeof(LanguageEnum), model.LanguageId[i]));
                    }
                    i++;
                }

                return UpdateSuccess();
            }
            return UpdateFailure(GetModelErrors());
        }

        [Description("导出多语言标签")]
        public IActionResult ExportLocalizationLabels()
        {
            var file = _localizedLabelImportExport.Export(SolutionId.Value, CurrentUser.OrgInfo.LanguageId);
            var fileName = System.IO.Path.GetFileName(file);
            return File(file, "application/zip", fileName);
        }

        [Description("导入多语言标签")]
        [HttpPost]
        public async Task<IActionResult> ImportLocalizationLabels(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var dir = _webHelper.MapPath("~/excel", true);
                var path = dir + "/" + "labels_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
                await file.SaveAs(path, _settingFinder, _webHelper).ConfigureAwait(false);
                _localizedLabelImportExport.Import(path, SolutionId.Value, CurrentUser.OrgInfo.LanguageId);
                return JOk(T["import_success"]);
            }
            return JError(T["notspecified_file"]);
        }
    }
}