using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xms.Core.Data;
using Xms.Data.Import.Domain;
using Xms.Infrastructure.Utility;
using Xms.Schema.Attribute;
using Xms.Schema.Entity;
using Xms.Schema.Extensions;
using Xms.Schema.OptionSet;
using Xms.Schema.StringMap;
using Xms.Sdk.Abstractions;
using Xms.Sdk.Client;

namespace Xms.Data.Import
{
    /// <summary>
    /// 数据导入服务
    /// </summary>
    public class DataImporter : IDataImporter
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IOptionSetDetailFinder _optionSetDetailServiceFinder;
        private readonly IStringMapFinder _stringMapFinder;
        private readonly IWebHelper _webHelper;
        private readonly IDataCreater _dataCreater;
        private readonly IDataUpdater _dataUpdater;
        private readonly IDataFinder _dataFinder;
        private readonly IAggregateService _aggregateService;
        private readonly IImportFileService _importFileService;
        private readonly IImportMapService _importMapService;
        private readonly IImportDataService _importDataService;

        public DataImporter(IEntityFinder entityFinder
            , IAttributeFinder attributeFinder
            , IOptionSetDetailFinder optionSetDetailFinder
            , IStringMapFinder stringMapFinder
            , IWebHelper webHelper
            , IDataCreater dataCreater
            , IDataUpdater dataUpdater
            , IDataFinder dataFinder
            , IAggregateService aggregateService
            , IImportFileService importFileService
            , IImportDataService importDataService
            , IImportMapService importMapService)
        {
            _entityFinder = entityFinder;
            _attributeFinder = attributeFinder;
            _optionSetDetailServiceFinder = optionSetDetailFinder;
            _stringMapFinder = stringMapFinder;
            _webHelper = webHelper;
            _dataCreater = dataCreater;
            _dataUpdater = dataUpdater;
            _dataFinder = dataFinder;
            _aggregateService = aggregateService;
            _importFileService = importFileService;
            _importDataService = importDataService;
            _importMapService = importMapService;
        }

        public ImportFile Import(Guid importFileId)
        {
            ImportFile file = _importFileService.FindById(importFileId);
            ImportMap map = _importMapService.FindById(file.ImportMapId);
            var data = new List<dynamic>().DeserializeFromJson(file.Content);
            this.ImportCore(file, map, data, (rowIndex, d, rowError, errorType, recordId) =>
            {
                var importDataEntity = new ImportData
                {
                    Data = ((JArray)d).ToString(),
                    ErrorMessage = rowError.ToString(),
                    HasError = rowError.Length > 0,
                    ImportFileId = file.ImportFileId,
                    LineNumber = rowIndex,
                    RecordId = recordId,
                    ErrorType = errorType
                };
                _importDataService.Create(importDataEntity);
            });
            return file;
        }

        public ImportFile RetryFailures(Guid importFileId)
        {
            ImportFile file = _importFileService.FindById(importFileId);
            ImportMap map = _importMapService.FindById(file.ImportMapId);
            var datas = _importDataService.Query(x => x.Where(f => f.ImportFileId == importFileId && f.HasError == true));
            if (datas.IsEmpty())
            {
                return file;
            }
            var data = datas.Select(x => new object().DeserializeFromJson(x.Data)).ToList();
            this.ImportCore(file, map, data, (rowIndex, d, rowError, errorType, recordId) =>
            {
                var importDataEntity = datas[rowIndex - 1];
                importDataEntity.ErrorMessage = rowError.ToString();
                importDataEntity.HasError = rowError.Length > 0;
                importDataEntity.RecordId = recordId;
                importDataEntity.ErrorType = errorType;
                _importDataService.Update(importDataEntity);
            });
            return file;
        }

        private bool ImportCore(ImportFile file, ImportMap map, List<dynamic> data, Action<int, object, string, int, Guid> itemProcceed)
        {
            var mappings = new List<ColumnMapping>().DeserializeFromJson(map.MapCustomizations);
            var columns = new List<string>().DeserializeFromJson(file.HeaderRow);
            var attributes = _attributeFinder.FindByEntityName(file.TargetEntityName);
            //更新记录时，根据哪些字段查询已存在的记录
            var updatePrimaryFields = new List<string> { attributes.Find(x => x.IsPrimaryField).Name };
            updatePrimaryFields.AddRange(mappings.Where(x => x.IsUpdatePrimaryField).Select(x => x.Mapping.Attribute));
            int rowIndex = 0, failureCount = 0;
            foreach (var d in data)
            {
                rowIndex++;
                StringBuilder rowError = new StringBuilder();
                var entity = new Entity(file.TargetEntityName);
                var retrieveFilter = new Dictionary<string, object>();
                for (int i = 0; i < columns.Count; i++)
                {
                    var value = d[i];
                    var mapping = mappings[i].Mapping;
                    //字段
                    var attr = attributes.Find(x => x.Name.IsCaseInsensitiveEqual(mapping.Attribute));
                    //是否类型
                    if (attr.TypeIsBit() || attr.TypeIsState())
                    {
                        if (value == null)
                        {
                            if (mapping.NullHandle == "ignore")
                            {
                            }
                            else if (mapping.NullHandle == "defaultvalue")
                            {
                                value = attr.DefaultValue;
                            }
                            else if (mapping.NullHandle.IsInteger())
                            {
                                var item = _stringMapFinder.Find(x => x.AttributeId == attr.AttributeId && x.Value == int.Parse(mapping.NullHandle));
                                if (item != null)
                                {
                                    value = item.Value;
                                }
                                else
                                {
                                    //失败记录
                                    rowError.AppendLine($"\"{columns[i]}\"的值\"{value}\"不存在");
                                }
                            }
                        }
                        else
                        {
                            var v = (string)value;
                            var item = _stringMapFinder.Find(x => x.AttributeId == attr.AttributeId && x.Name == v);
                            if (item != null)
                            {
                                value = item.Value;
                            }
                            else
                            {
                                //失败记录
                                rowError.AppendLine($"\"{columns[i]}\"的值\"{value}\"不存在");
                            }
                        }
                    }
                    //选项集类型
                    else if (attr.TypeIsPickList() || attr.TypeIsStatus())
                    {
                        if (value == null)
                        {
                            if (mapping.NullHandle == "ignore")
                            {
                            }
                            else if (mapping.NullHandle == "defaultvalue")
                            {
                                value = attr.DefaultValue;
                            }
                            else if (((string)value).IsInteger())
                            {
                                var item = attr.PickLists.Find(x => x.Value == (int)value);
                                if (item != null)
                                {
                                    value = new OptionSetValue(item.Value);
                                }
                                else
                                {
                                    //失败记录
                                    rowError.AppendLine($"\"{columns[i]}\"的值\"{value}\"不存在");
                                }
                            }
                        }
                        else
                        {
                            var v = (string)value;
                            var item = _optionSetDetailServiceFinder.Find(x => x.OptionSetId == attr.OptionSetId && x.Name == v);
                            if (item != null)
                            {
                                value = new OptionSetValue(item.Value);
                            }
                            else
                            {
                                //失败记录
                                rowError.AppendLine($"\"{columns[i]}\"的值\"{value}\"不存在");
                            }
                        }
                    }
                    //引用类型
                    else if (attr.TypeIsLookUp() || attr.TypeIsOwner() || attr.TypeIsCustomer())
                    {
                        if (value != null)
                        {
                            var criteria = new Dictionary<string, object>();
                            if (((string)value).IsGuid())
                            {
                                criteria.Add(entity.IdName, value);
                            }
                            if (mapping.LookupName.IsNotEmpty())
                            {
                                criteria.Add(mapping.LookupName, value);
                            }
                            else
                            {
                                var primaryField = _attributeFinder.Find(x => x.EntityId == attr.ReferencedEntityId && x.IsPrimaryField == true);
                                criteria.Add(primaryField.Name, value);
                            }
                            var filter = new Sdk.Abstractions.Query.FilterExpression();
                            foreach (var c in criteria)
                            {
                                filter.AddCondition(c.Key, Sdk.Abstractions.Query.ConditionOperator.Equal, c.Value);
                            }
                            var matchedCount = _aggregateService.Count(attr.ReferencedEntityName, filter);
                            if (matchedCount > 1)
                            {
                                //失败记录
                                rowError.AppendLine($"\"{columns[i]}\"的值\"{value}\"不唯一");
                            }
                            else
                            {
                                var referenced = _dataFinder.RetrieveByAttribute(attr.ReferencedEntityName, criteria, new List<string> { attr.ReferencedEntityName + "Id" });
                                if (referenced.NotEmpty())
                                {
                                    if (attr.TypeIsOwner())
                                    {
                                        value = new OwnerObject(OwnerTypes.SystemUser, referenced.Id);
                                    }
                                    else
                                    {
                                        value = new EntityReference(attr.ReferencedEntityName, referenced.Id);
                                    }
                                }
                                else
                                {
                                    //失败记录
                                    rowError.AppendLine($"\"{columns[i]}\"的值\"{value}\"不存在");
                                }
                            }
                        }
                    }
                    //金额类型
                    else if (attr.TypeIsMoney() || attr.TypeIsSmallMoney())
                    {
                        if (value != null)
                        {
                            if (decimal.TryParse((string)value, out decimal v))
                            {
                                value = new Money(v);
                            }
                            else
                            {
                                //失败记录
                                rowError.AppendLine($"\"{columns[i]}\"的值\"{value}\"格式错误");
                            }
                        }
                    }
                    //浮点型类型
                    else if (attr.TypeIsDecimal() || attr.TypeIsFloat())
                    {
                        if (value != null)
                        {
                            if (!decimal.TryParse((string)value, out _))
                            {
                                //失败记录
                                rowError.AppendLine($"\"{columns[i]}\"的值\"{value}\"格式错误");
                            }
                        }
                    }
                    //整型类型
                    else if (attr.TypeIsInt())
                    {
                        if (value != null)
                        {
                            if (!((string)value).IsInteger())
                            {
                                //失败记录
                                rowError.AppendLine($"\"{columns[i]}\"的值\"{value}\"格式错误");
                            }
                        }
                    }
                    //日期类型
                    else if (attr.TypeIsDateTime() || attr.TypeIsSmallDateTime())
                    {
                        if (value != null)
                        {
                            if (!((string)value).IsDateTime())
                            {
                                //失败记录
                                rowError.AppendLine($"\"{columns[i]}\"的值\"{value}\"格式错误");
                            }
                        }
                    }
                    else if (value != null)
                    {
                        value = (string)value;
                    }
                    if (rowError.Length == 0 && value != null)
                    {
                        entity.SetAttributeValue(attr.Name, value);
                        if (updatePrimaryFields.Exists(x => x.IsCaseInsensitiveEqual(attr.Name)))
                        {
                            retrieveFilter.Add(attr.Name, value);
                        }
                    }
                }
                int errorType = rowError.Length > 0 ? 2 : 0;//错误类型：数据=2
                //重复数据的操作
                if (retrieveFilter.Count > 0)
                {
                    var existsEntity = _dataFinder.RetrieveByAttribute(file.TargetEntityName, retrieveFilter, updatePrimaryFields, true);
                    //不导入重复数据
                    if (file.DuplicateDetection == 1 && existsEntity.NotEmpty())
                    {
                        rowError.AppendLine("记录已存在");
                    }
                    //覆盖导入
                    else if (file.DuplicateDetection == 2 && existsEntity.NotEmpty())
                    {
                        entity.SetIdValue(existsEntity.Id);
                    }
                    //仅覆盖重复数据
                    else if (file.DuplicateDetection == 3)
                    {
                        if (existsEntity.NotEmpty())
                        {
                            entity.SetIdValue(existsEntity.Id);
                        }
                        else
                        {
                            rowError.AppendLine("记录不存在");
                        }
                    }
                }
                Guid recordId = entity.Id;
                //本行数据验证通过
                if (rowError.Length == 0)
                {
                    try
                    {
                        if (recordId.IsEmpty())
                        {
                            recordId = _dataCreater.Create(entity);
                        }
                        else
                        {
                            _dataUpdater.Update(entity);
                        }
                    }
                    catch (Exception e)
                    {
                        errorType = 1;//错误类型：系统
                        rowError.AppendLine(e.Message);
                    }
                }
                if (rowError.Length > 0)
                {
                    failureCount++;
                }
                //每行导入结果
                itemProcceed(rowIndex, d, rowError.ToString(), errorType, recordId);
            }
            //文件导入结果
            file.SuccessCount = data.Count - failureCount;
            file.FailureCount = failureCount;
            _importFileService.Update(file);
            return true;
        }
    }
}