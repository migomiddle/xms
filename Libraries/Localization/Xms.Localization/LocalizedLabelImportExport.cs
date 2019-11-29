using System;
using System.Collections.Generic;
using System.Data;
using Xms.Context;
using Xms.Infrastructure.Utility;
using Xms.Localization.Abstractions;
using Xms.Localization.Data;

namespace Xms.Localization
{
    /// <summary>
    /// 多语言标签导入导出服务
    /// </summary>
    public class LocalizedLabelImportExport : ILocalizedLabelImportExport
    {
        private readonly ILocalizedLabelRepository _localizedLabelRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly ILocalizedTextProvider _loc;
        private readonly ILanguageService _languageService;

        //private readonly IDataExporter _dataExporter;
        //private readonly IDataImporter _dataImporter;
        private readonly IAppContext _appContext;

        private readonly IWebHelper _webHelper;

        public LocalizedLabelImportExport(IAppContext appContext
            , ILocalizedLabelRepository localizedLabelRepository
            , ILocalizedLabelService localizedLabelService
            , ILanguageService languageService
            //, IDataExporter dataExporter
            //, IDataImporter dataImporter
            , IWebHelper webHelper
            )
        {
            _appContext = appContext;
            _localizedLabelRepository = localizedLabelRepository;
            _localizedLabelService = localizedLabelService;
            _loc = _appContext.GetFeature<ILocalizedTextProvider>();
            _languageService = languageService;
            //_dataExporter = dataExporter;
            //_dataImporter = dataImporter;
            _webHelper = webHelper;
        }

        public string Export(Guid solutionId, LanguageCode baseLanguageId)
        {
            var filePath = "/excel/labels_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            var languages = _languageService.Query(n => n.Sort(s => s.SortAscending(f => f.Name)));
            var datas = _localizedLabelRepository.Export(solutionId, baseLanguageId, languages);
            var path = _webHelper.MapPath(filePath);
            var columnNames = new Dictionary<string, string>();
            columnNames.Add("ObjectId", _loc["object"] + "ID");
            columnNames.Add("ObjectColumnName", _loc["name"]);
            columnNames.Add("LabelTypeCode", _loc["typename"]);
            var baseLang = languages.Find(n => n.UniqueId == (int)baseLanguageId);
            languages.RemoveAll(n => n.UniqueId == (int)baseLanguageId);
            languages.Insert(0, baseLang);
            foreach (var lg in languages)
            {
                columnNames.Add(lg.UniqueId.ToString(), lg.UniqueId.ToString());
            }
            //_dataExporter.RenderExcel(datas, "labels", path, columnNames);
            return filePath;
        }

        public bool Import(string file, Guid solutionId, LanguageCode baseLanguageId)
        {
            var data = new DataTable();// _dataImporter.ExcelToDataTable(file);
            var languages = _languageService.Query(n => n.Sort(s => s.SortAscending(f => f.Name)));
            foreach (DataRow row in data.Rows)
            {
                var objectId = Guid.Parse(row[0].ToString());
                var name = row[1].ToString();
                var typeCode = int.Parse(row[2].ToString());
                foreach (var lg in languages)
                {
                    if (data.Columns.Contains(lg.UniqueId.ToString()))
                    {
                        var label = row[lg.UniqueId.ToString()] != null ? row[lg.UniqueId.ToString()].ToString() : string.Empty;
                        var originalLabel = _localizedLabelService.Find(n => n.ObjectId == objectId && n.ObjectColumnName == name && (int)n.LanguageId == lg.UniqueId);
                        if (originalLabel != null)
                        {
                            if (label.IsEmpty())
                            {
                                _localizedLabelService.DeleteById(originalLabel.LocalizedLabelId);
                            }
                            else if (!label.IsCaseInsensitiveEqual(originalLabel.Label))
                            {
                                originalLabel.Label = label;
                                _localizedLabelService.Update(originalLabel);
                            }
                        }
                        else if (label.IsNotEmpty())
                        {
                            _localizedLabelService.Create(solutionId, label, typeCode.ToString(), name, objectId, (LanguageCode)Enum.Parse(typeof(LanguageCode), lg.UniqueId.ToString()));
                        }
                    }
                }
            }
            return true;
        }
    }
}