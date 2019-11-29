using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xms.Data.Export;
using Xms.Data.Import;
using Xms.Infrastructure.Utility;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;

namespace Xms.Web.Api
{
    /// <summary>
    /// 数据导入接口
    /// </summary>
    [Route("{org}/api/data/import")]
    public class DataImporterController : ApiControllerBase
    {
        private readonly IDataImporter _dataImporter;
        private readonly IDataExporter _dataExporter;
        private readonly IImportDataService _importDataService;
        private readonly IImportFileService _importFileService;
        private readonly IImportMapService _importMapService;

        public DataImporterController(IWebAppContext appContext
            , IDataImporter dataImporter
            , IDataExporter dataExporter
            , IImportDataService importDataService
            , IImportFileService importFileService
            , IImportMapService importMapService)
            : base(appContext)
        {
            _dataImporter = dataImporter;
            _dataExporter = dataExporter;
            _importDataService = importDataService;
            _importFileService = importFileService;
            _importMapService = importMapService;
        }

        [Description("重试")]
        [HttpGet("retry")]
        public IActionResult Retry(Guid importFileId)
        {
            _dataImporter.RetryFailures(importFileId);
            return JOk("");
        }

        [Description("导出失败记录")]
        [HttpGet("failures")]
        public IActionResult ExportFailures(Guid importFileId)
        {
            var file = _importFileService.FindById(importFileId);
            var map = _importMapService.FindById(file.ImportMapId);
            var datas = _importDataService.Query(x => x.Where(f => f.ImportFileId == importFileId && f.HasError == true));
            var data = datas.Select(x => new object().DeserializeFromJson(x.Data)).ToList();
            var header = new List<string>().DeserializeFromJson(file.HeaderRow);
            var mappings = new List<ColumnMapping>().DeserializeFromJson(map.MapCustomizations);
            var columns = new Dictionary<string, string>();
            foreach (var m in mappings)
            {
                columns.Add(m.Mapping.Attribute, m.Column);
            }
            var stream = _dataExporter.ToExcelStream(data, file.Name, columns);
            if (stream != null)
            {
                return File(stream.ToArray(), "application/vnd.ms-excel", file.Name);
            }
            return new EmptyResult();
        }
    }
}