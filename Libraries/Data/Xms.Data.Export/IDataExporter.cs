using System.Collections.Generic;
using System.Data;
using System.IO;
using Xms.Sdk.Abstractions.Query;

namespace Xms.Data.Export
{
    public interface IDataExporter
    {
        void SaveToExcel(DataTable dt, string filePath);

        string ToExcelFile(QueryView.Domain.QueryView queryView, FilterExpression filter, OrderExpression order, string fileName, bool includePrimaryKey = false, bool includeIndex = false, string title = "");

        MemoryStream ToExcelStream(QueryView.Domain.QueryView queryView, FilterExpression filter, OrderExpression order, string fileName, bool includePrimaryKey = false, bool includeIndex = false, string title = "");

        void SaveToExcel<T>(IList<T> datas, string name, string filePath, Dictionary<string, string> columnNames = null, IList<string> hideColumns = null, string title = "", IList<Schema.Domain.Attribute> attributeList = null);

        MemoryStream ToExcelStream<T>(IList<T> datas, string name, Dictionary<string, string> columnNames = null, IList<string> hideColumns = null, string title = "", IList<Schema.Domain.Attribute> attributeList = null);
    }
}