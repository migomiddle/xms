using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xms.Configuration;
using Xms.Data.Import;
using Xms.Data.Import.Domain;
using Xms.File.Extensions;
using Xms.Infrastructure.Utility;
using Xms.Schema.Attribute;
using Xms.Schema.Entity;
using Xms.Session;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Models;

namespace Xms.Web.Controllers
{
    /// <summary>
    /// 数据导入控制器
    /// </summary>
    public class DataImportController : AuthenticatedControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IDataImporter _dataImporter;
        private readonly IImportMapService _importMapService;
        private readonly IImportFileService _importFileService;
        private readonly IFileTemplateProvider _fileTemplateProvider;
        private readonly IWebHelper _webHelper;
        private readonly ISessionService _sessionService;
        private readonly ISettingFinder _settingFinder;

        private string ImportDirectory
        {
            get
            {
                var path = _webHelper.MapPath("~/excel/import/");
                if (!System.IO.File.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                return path;
            }
        }

        public DataImportController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IAttributeFinder attributeFinder
            , IDataImporter dataImporter
            , IImportMapService importMapService
            , IImportFileService importFileService
            , IFileTemplateProvider fileTemplateProvider
            , IWebHelper webHelper
            , ISessionService sessionService
            , ISettingFinder settingFinder
            )
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _attributeFinder = attributeFinder;
            _dataImporter = dataImporter;
            _importMapService = importMapService;
            _importFileService = importFileService;
            _fileTemplateProvider = fileTemplateProvider;
            _webHelper = webHelper;
            _sessionService = sessionService;
            _settingFinder = settingFinder;
        }

        [Description("下载导入模板")]
        public IActionResult Template(Guid entityId)
        {
            var filePath = _fileTemplateProvider.Get(entityId);
            var fileName = System.IO.Path.GetFileName(filePath);
            return PhysicalFile(filePath, "application/excel", fileName);
        }

        [Description("导入数据向导")]
        public IActionResult Import(Guid entityId, DialogModel dm)
        {
            var entityMetadata = _entityFinder.FindById(entityId);
            if (entityMetadata == null)
            {
                return NotFound();
            }
            ImportModel model = new ImportModel
            {
                EntityId = entityId
                ,
                EntityName = entityMetadata.Name
                ,
                DataFileName = _sessionService.GetValue("importfile")
                ,
                DuplicateDetection = 1
            };
            ViewBag.dialogmodel = dm;
            return View(model);
        }

        [Description("导入数据-数据映射")]
        [HttpPost]
        public async Task<IActionResult> Mapping(ImportModel model, DialogModel dm)
        {
            string file = _sessionService.GetValue("importfile");
            if (file.IsEmpty() || (model.DataFile != null && !file.IsCaseInsensitiveEqual(model.DataFile.FileName)))
            {
                //model.DataFileName = model.DataFile.FileName;
                model.DataFileName = model.EntityName + "_" + DateTime.Now.ToString("yyyyMMddhhmmsssss") + System.IO.Path.GetExtension(model.DataFile.FileName);// model.DataFile.FileName;
                file = ImportDirectory + "/" + model.DataFileName;
                await model.DataFile.SaveAs(file, _settingFinder, _webHelper).ConfigureAwait(false);
                _sessionService.Set("importfile", model.DataFileName, 5);
            }
            else
            {
                model.DataFileName = file;
                file = ImportDirectory + "/" + file;
            }
            var columns = ExcelHelper.GetColumns(file);
            var attributes = _attributeFinder.FindByEntityId(model.EntityId);
            var mapData = new Dictionary<string, Schema.Domain.Attribute>();
            foreach (var c in columns)
            {
                var attr = attributes.Find(n => n.LocalizedName.IsCaseInsensitiveEqual(c));//匹配到系统字段
                mapData.Add(c, attr);
            }
            model.MapData = mapData;
            model.Attributes = attributes;
            model.ImportMaps = _importMapService.Query(x => x.Where(f => f.TargetEntityName == model.EntityName));
            ViewBag.dialogmodel = dm;
            return View(model);
        }

        [Description("导入数据-保存数据映射")]
        [HttpPost]
        public IActionResult SaveMap(ImportModel model)
        {
            if (model.ImportMapId.Equals(Guid.Empty))
            {
                var importMapEntity = new ImportMap
                {
                    Description = "",
                    ImportMapId = Guid.NewGuid(),
                    MapCustomizations = model.MapCustomizations.UrlDecode(),
                    MapType = 1,
                    Name = model.Name,
                    TargetEntityName = model.EntityName,
                    CreatedBy = CurrentUser.SystemUserId
                };
                _importMapService.Create(importMapEntity);
                model.ImportMapId = importMapEntity.ImportMapId;
            }
            return ImportData(model);
        }

        [Description("导入数据-保存数据文件")]
        [HttpPost]
        public IActionResult ImportData(ImportModel model)
        {
            var data = ExcelHelper.ToDataTable(ImportDirectory + "/" + model.DataFileName);
            var importFileEntity = new ImportFile
            {
                Content = data.Rows.Cast<DataRow>().Select(x => x.ItemArray).SerializeToJson(),
                DuplicateDetection = model.DuplicateDetection,
                FieldDelimiterCode = ",",
                HeaderRow = data.Columns.Cast<DataColumn>().Select(x => x.ColumnName).SerializeToJson(),
                ImportFileId = Guid.NewGuid(),
                ImportMapId = model.ImportMapId,
                IsFirstRowHeader = true,
                Name = model.DataFileName,
                RecordsOwnerId = CurrentUser.SystemUserId,
                Size = 1,
                TargetEntityName = model.EntityName,
                TotalCount = data.Rows.Count,
                CreatedBy = CurrentUser.SystemUserId
            };
            _importFileService.Create(importFileEntity);
            model.ImportFileId = importFileEntity.ImportFileId;
            return ImportConfirm(model);
        }

        [Description("导入数据-确认")]
        [HttpPost]
        public IActionResult ImportConfirm(ImportModel model)
        {
            return View("ImportConfirm", model);
        }

        [Description("导入数据-开始导入")]
        [HttpPost]
        public IActionResult Starting(ImportModel model)
        {
            _sessionService.Remove("importfile");
            var result = _dataImporter.Import(model.ImportFileId);
            return JOk(new { result.ImportFileId, result.SuccessCount, result.FailureCount });
        }
    }
}