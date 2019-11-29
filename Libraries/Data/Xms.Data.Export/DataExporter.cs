using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Xms.Context;
using Xms.Core.Data;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.QueryView;
using Xms.Schema.Attribute;
using Xms.Schema.Extensions;
using Xms.Schema.RelationShip;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Client;
using Xms.Sdk.Extensions;
using Xms.Security.Principal;

namespace Xms.Data.Export
{
    /// <summary>
    /// 数据导出服务
    /// </summary>
    public class DataExporter : IDataExporter
    {
        private readonly IAppContext _appContext;
        private readonly ISystemUserPermissionService _systemUserPermissionService;
        private readonly IFetchDataService _fetchDataService;
        private readonly IAggregateService _aggregateService;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IRelationShipFinder _relationShipFinder;
        private readonly IGridService _gridService;
        private readonly IWebHelper _webHelper;

        public DataExporter(IAppContext appContext
            , ISystemUserPermissionService systemUserPermissionService
            , IFetchDataService fetchDataService
            , IAggregateService aggregateService
            , IAttributeFinder attributeFinder
            , IRelationShipFinder relationShipFinder
            , IGridService gridService
            , IWebHelper webHelper
            )
        {
            _appContext = appContext;
            _systemUserPermissionService = systemUserPermissionService;
            _fetchDataService = fetchDataService;
            _aggregateService = aggregateService;
            _attributeFinder = attributeFinder;
            _relationShipFinder = relationShipFinder;
            _gridService = gridService;
            _webHelper = webHelper;
        }

        public MemoryStream ToExcelStream<T>(IList<T> datas, string name, Dictionary<string, string> columnNames = null, IList<string> hideColumns = null, string title = "", IList<Schema.Domain.Attribute> attributeList = null)
        {
            if (datas.NotEmpty())
            {
                Type type = datas.First().GetType();
                bool isDynamic = type == typeof(System.Dynamic.ExpandoObject);
                NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
                ISheet sheet = book.CreateSheet(name);
                int rowIndex = 0;
                if (title.IsNotEmpty())
                {
                    ICellStyle style = book.CreateCellStyle();
                    //设置单元格的样式：水平对齐居中
                    style.Alignment = HorizontalAlignment.Center;
                    IRow titleRow = sheet.CreateRow(rowIndex);
                    ICell titleCell = titleRow.CreateCell(0);
                    titleCell.SetCellValue(title);
                    titleCell.CellStyle = style;
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, columnNames.Count - 1));
                    rowIndex++;
                }

                IRow headerRow = sheet.CreateRow(rowIndex);
                rowIndex++;
                //ICellStyle numericStyle = book.CreateCellStyle();
                //numericStyle.Alignment = HorizontalAlignment.Right;
                //numericStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                var keys = new List<string>();
                if (columnNames.NotEmpty())
                {
                    keys = columnNames.Keys.ToList();
                }
                if (isDynamic)
                {
                    if (keys.IsEmpty() && datas.NotEmpty())
                    {
                        keys = (datas.First() as IDictionary<string, object>).Keys.ToList();
                        foreach (var item in keys)
                        {
                            columnNames.Add(item, item);
                        }
                    }
                    int columnIndex = 0;
                    foreach (IDictionary<string, object> data in datas)
                    {
                        IRow dataRow = sheet.CreateRow(rowIndex);
                        var line = data.ToList();
                        foreach (var p in keys)
                        {
                            var item = line.Find(n => n.Key.IsCaseInsensitiveEqual(p));
                            var a = p;
                            if (p.IndexOf('.') > 0)
                            {
                                a = p.Split('.')[1];
                            }
                            var attr = attributeList?.FirstOrDefault(n => n.Name.IsCaseInsensitiveEqual(a));
                            var cell = dataRow.CreateCell(columnIndex);
                            this.SetCellValue(cell, item.Value?.ToString(), attr);
                            //cell.CellStyle = numericStyle;
                            columnIndex++;
                        }
                        columnIndex = 0;
                        rowIndex++;
                    }
                }
                else if (type == typeof(JArray))
                {
                    int columnIndex = 0;
                    foreach (dynamic data in datas)
                    {
                        IRow dataRow = sheet.CreateRow(rowIndex);
                        var line = ((JArray)data);
                        foreach (var p in keys)
                        {
                            var item = line[columnIndex];
                            var a = p;
                            if (p.IndexOf('.') > 0)
                            {
                                a = p.Split('.')[1];
                            }
                            var attr = attributeList?.FirstOrDefault(n => n.Name.IsCaseInsensitiveEqual(a));
                            var cell = dataRow.CreateCell(columnIndex);
                            this.SetCellValue(cell, item, attr);
                            //cell.CellStyle = numericStyle;
                            columnIndex++;
                        }
                        columnIndex = 0;
                        rowIndex++;
                    }
                }
                else
                {
                    PropertyInfo[] pis = type.GetProperties();
                    string displayName = string.Empty;
                    int columnIndex = 0;
                    foreach (var p in pis)
                    {
                        displayName = p.Name;
                        if (columnNames.NotEmpty() && columnNames.ContainsKey(p.Name))
                        {
                            displayName = columnNames[p.Name];
                        }
                        headerRow.CreateCell(columnIndex).SetCellValue(displayName);
                        columnIndex++;
                    }
                    foreach (T data in datas)
                    {
                        IRow dataRow = sheet.CreateRow(rowIndex);
                        foreach (var p in pis)
                        {
                            dataRow.CreateCell(columnIndex).SetCellValue(p.GetValue(data, null).ToString());
                            columnIndex++;
                        }
                        columnIndex = 0;
                        rowIndex++;
                    }
                }
                //隐藏列
                int hcolumnIndex = 0;
                foreach (var p in keys)
                {
                    headerRow.CreateCell(hcolumnIndex).SetCellValue(columnNames[p]);
                    if (hideColumns.NotEmpty() && hideColumns.Any(n => n.IsCaseInsensitiveEqual(columnNames[p])))
                    {
                        sheet.SetColumnHidden(hcolumnIndex, true);
                    }
                    hcolumnIndex++;
                }
                // 写入到客户端
                MemoryStream ms = new MemoryStream();
                book.Write(ms);
                return ms;
            }
            return null;
        }

        private void ResolveDynamicData(ISheet sheet, int rowIndex, IList<dynamic> datas, Dictionary<string, string> columnNames = null, IList<string> hideColumns = null, IList<Schema.Domain.Attribute> attributeList = null)
        {
            IRow headerRow = sheet.CreateRow(rowIndex);
            var keys = new List<string>();
            if (columnNames.NotEmpty())
            {
                keys = columnNames.Keys.ToList();
            }
            else if (datas.NotEmpty())
            {
                keys = (datas.First() as IDictionary<string, object>).Keys.ToList();
                foreach (var item in keys)
                {
                    columnNames.Add(item, item);
                }
            }
            int piIndex = 0;
            foreach (var p in keys)
            {
                headerRow.CreateCell(piIndex).SetCellValue(columnNames[p]);
                if (hideColumns.NotEmpty() && hideColumns.Any(n => n.IsCaseInsensitiveEqual(columnNames[p])))
                {
                    sheet.SetColumnHidden(piIndex, true);
                }
                piIndex++;
            }
            foreach (IDictionary<string, object> data in datas)
            {
                piIndex = 0;
                IRow dataRow = sheet.CreateRow(rowIndex);
                var line = data.ToList();
                foreach (var p in keys)
                {
                    var item = line.Find(n => n.Key.IsCaseInsensitiveEqual(p));
                    var a = p;
                    if (p.IndexOf('.') > 0)
                    {
                        a = p.Split('.')[1];
                    }
                    var attr = attributeList?.FirstOrDefault(n => n.Name.IsCaseInsensitiveEqual(a));
                    var cell = dataRow.CreateCell(piIndex);
                    if (item.Value != null)
                    {
                        if (attr != null && (attr.TypeIsMoney() || attr.TypeIsFloat() || attr.TypeIsInt() || attr.TypeIsDecimal()))
                        {
                            cell.SetCellValue(double.Parse(item.Value.ToString()));
                        }
                        else
                        {
                            cell.SetCellValue(item.Value.ToString());
                        }
                    }
                    else
                    {
                        cell.SetCellValue(string.Empty);
                    }
                    //cell.CellStyle = numericStyle;
                    piIndex++;
                }
                rowIndex++;
            }
        }

        public void SaveToExcel<T>(IList<T> datas, string name, string filePath, Dictionary<string, string> columnNames = null, IList<string> hideColumns = null, string title = "", IList<Schema.Domain.Attribute> attributeList = null)
        {
            if (filePath.IsNotEmpty() && null != datas && datas.Count() > 0)
            {
                using (var ms = ToExcelStream(datas, name, columnNames, hideColumns, title, attributeList))
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        byte[] data = ms.ToArray();
                        fs.Write(data, 0, data.Length);
                        fs.Flush();
                    }
                }
            }
        }

        public void SaveToExcel(DataTable dt, string filePath)
        {
            if (filePath.IsNotEmpty() && null != dt && dt.Rows.Count > 0)
            {
                NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
                ISheet sheet = book.CreateSheet(dt.TableName);

                IRow headerRow = sheet.CreateRow(0);
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    headerRow.CreateCell(i).SetCellValue(dt.Columns[i].ColumnName);
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow dataRow = sheet.CreateRow(i + 1);
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        dataRow.CreateCell(j).SetCellValue(Convert.ToString(dt.Rows[i][j]));
                    }
                }
                // 写入到客户端
                using (MemoryStream ms = new MemoryStream())
                {
                    book.Write(ms);
                    using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        byte[] data = ms.ToArray();
                        fs.Write(data, 0, data.Length);
                        fs.Flush();
                    }
                    book = null;
                }
            }
        }

        public string ToExcelFile(QueryView.Domain.QueryView queryView, FilterExpression filter, OrderExpression order, string fileName, bool includePrimaryKey = false, bool includeIndex = false, string title = "")
        {
            fileName.IfEmpty(queryView.Name);
            fileName = Encoding.UTF8.GetString(fileName.ToByteArray());//防止linux下乱码
            var webPath = "/excel/" + fileName + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            var filePath = _webHelper.MapPath(webPath);
            //SaveToExcel(list, fileName, filePath, columnNames, hideColumns, title, _fetchDataService.QueryResolver.AttributeList);
            using (var ms = ToExcelStream(queryView, filter, order, fileName, includePrimaryKey, includeIndex, title))
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
            return webPath;
        }

        public MemoryStream ToExcelStream(QueryView.Domain.QueryView queryView, FilterExpression filter, OrderExpression order, string fileName, bool includePrimaryKey = false, bool includeIndex = false, string title = "")
        {
            _fetchDataService.GetMetaDatas(queryView.FetchConfig);
            if (filter != null)
            {
                _fetchDataService.QueryExpression.Criteria.AddFilter(filter);
            }
            if (order != null)
            {
                _fetchDataService.QueryExpression.Orders.Clear();
                _fetchDataService.QueryExpression.Orders.Add(order);
            }
            _fetchDataService.QueryExpression.PageInfo = new PagingInfo() { PageNumber = 1, PageSize = 99999 };
            var data = _fetchDataService.Execute(_fetchDataService.QueryExpression);
            if (data.TotalItems == 0)
            {
                return null;
            }
            Dictionary<string, string> columnNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            List<string> hideColumns = new List<string>();
            var grid = _gridService.Build(queryView, _fetchDataService.QueryResolver.EntityList, _fetchDataService.QueryResolver.AttributeList);
            if (includeIndex)
            {
                columnNames.Add("rownum", "序号");
                if (data.Items.NotEmpty())
                {
                    for (int i = 0; i < data.Items.Count; i++)
                    {
                        data.Items[i].RowNum = i + 1;
                    }
                }
            }
            if (includePrimaryKey)
            {
                var pk = _fetchDataService.QueryResolver.AttributeList.Find(n => n.EntityName.IsCaseInsensitiveEqual(_fetchDataService.QueryResolver.MainEntity.Name) && n.TypeIsPrimaryKey());//new Schema.AttributeService(user).Find(n => n.AttributeTypeName == AttributeTypeIds.PRIMARYKEY && n.EntityName == fetchService.QueryResolver.MainEntity.Name);
                columnNames.Add(pk.Name, pk.Name);
                hideColumns.Add(pk.Name);
            }
            foreach (var cell in grid.Rows[0].Cells)
            {
                var columnName = cell.Name.IndexOf(".") > 0 ? cell.Name.Split('.')[1] : cell.Name;
                var item = _fetchDataService.QueryResolver.AttributeList.Find(n => n.Name.IsCaseInsensitiveEqual(columnName) && n.EntityName.IsCaseInsensitiveEqual(cell.EntityName));
                var label = cell.Label;//item.LocalizedName;
                if (cell.Label.IsEmpty())
                {
                    if (!item.EntityId.Equals(_fetchDataService.QueryResolver.MainEntity.EntityId))
                    {
                        var le = _fetchDataService.QueryExpression.FindLinkEntityByName(item.EntityName);
                        if (le != null)
                        {
                            var relationship = _fetchDataService.QueryResolver.RelationShipList.Find(n => n.Name.IsCaseInsensitiveEqual(cell.Name.Split('.')[0]));
                            _relationShipFinder.WrapLocalizedLabel(relationship);
                            label += "(" + relationship.ReferencingAttributeLocalizedName + ")";
                            columnName = le.EntityAlias + "." + columnName;
                        }
                    }
                    else
                    {
                        label = item.LocalizedName;
                    }
                }
                if (item.TypeIsPrimaryKey())
                {
                    var pkField = _fetchDataService.QueryResolver.AttributeList.Find(n => n.EntityName.IsCaseInsensitiveEqual(_fetchDataService.QueryResolver.MainEntity.Name) && n.IsPrimaryField);
                    columnNames.Add(pkField.Name, label);
                }
                else if (item.TypeIsBit() || item.TypeIsState() || item.TypeIsStatus() || item.TypeIsPickList() || item.TypeIsLookUp() || item.TypeIsOwner() || item.TypeIsCustomer())
                {
                    columnNames.Add(cell.Name + "Name", label);
                }
                else
                {
                    columnNames.Add(cell.Name, label);
                }
            }
            //字段权限
            if (_fetchDataService.QueryResolver.AttributeList.Count(x => x.AuthorizationEnabled) > 0)
            {
                var noneRead = _systemUserPermissionService.GetNoneReadFields(_appContext.GetFeature<ICurrentUser>().SystemUserId, _fetchDataService.QueryResolver.AttributeList.Where(x => x.AuthorizationEnabled).Select(x => x.AttributeId).ToList());
                if (noneRead.NotEmpty())
                {
                    var noneReadAttrs = _attributeFinder.Query(x => x.Where(f => f.AttributeId.In(noneRead)));
                    foreach (var d in data.Items)
                    {
                        var _this = d as IDictionary<string, object>;
                        foreach (var nr in noneReadAttrs)
                        {
                            _this[nr.Name.ToLower()] = null;
                            if (nr.TypeIsBit() || nr.TypeIsState() || nr.TypeIsPickList() || nr.TypeIsStatus()
                                || nr.TypeIsLookUp() || nr.TypeIsOwner() || nr.TypeIsCustomer())
                            {
                                _this[nr.Name.ToLower() + "name"] = null;
                            }
                        }
                    }
                }
            }
            var list = data.Items;
            //aggregation
            if (list.NotEmpty() && queryView.AggregateConfig.IsNotEmpty())
            {
                var aggFields = new List<AggregateExpressionField>().DeserializeFromJson(queryView.AggregateConfig);
                if (aggFields.NotEmpty())
                {
                    var aggExp = new AggregateExpression
                    {
                        ColumnSet = _fetchDataService.QueryExpression.ColumnSet,
                        Criteria = _fetchDataService.QueryExpression.Criteria,
                        EntityName = _fetchDataService.QueryExpression.EntityName,
                        LinkEntities = _fetchDataService.QueryExpression.LinkEntities,
                        AggregateFields = aggFields
                    };
                    var aggDatas = _aggregateService.Execute(aggExp);
                    var aggData = aggDatas.NotEmpty() ? aggDatas.First() : null;
                    if (aggData != null)
                    {
                        var tmpRow = list.First() as IDictionary<string, object>;
                        var aggRow = new Dictionary<string, object>();
                        var aggDataRow = aggData as IDictionary<string, object>;
                        foreach (var r in tmpRow)
                        {
                            var aggf = aggFields.Find(n => n.AttributeName.IsCaseInsensitiveEqual(r.Key));
                            if (aggf != null)
                            {
                                aggRow[r.Key] = aggDataRow[aggf.AttributeName.ToLower()];
                            }
                            else
                            {
                                aggRow[r.Key] = null;
                            }
                        }
                        list.Add(aggRow);
                    }
                }
            }
            return ToExcelStream(list, fileName, columnNames, hideColumns, title, _fetchDataService.QueryResolver.AttributeList);
        }

        //private void ToExcel(QueryView.Domain.QueryView queryView, FilterExpression filter, OrderExpression order, string fileName, bool includePrimaryKey = false, bool includeIndex = false, string title = "")
        //{
        //}

        private void SetCellValue(ICell cell, object value, Schema.Domain.Attribute attr)
        {
            if (value != null)
            {
                if (attr != null && (attr.TypeIsMoney() || attr.TypeIsSmallMoney() || attr.TypeIsFloat() || attr.TypeIsDecimal() || attr.TypeIsInt() || attr.TypeIsSmallInt()))
                {
                    cell.SetCellValue(double.Parse((string)value));
                    cell.SetCellType(CellType.Numeric);
                }
                else
                {
                    cell.SetCellValue((string)value);
                }
            }
            else
            {
                cell.SetCellValue(string.Empty);
            }
        }
    }
}