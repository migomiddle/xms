using System;
using System.Data;
using Xms.Business.DataAnalyse.Report;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data;
using Xms.Infrastructure.Utility;
using Xms.Schema.Extensions;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Data;
using Xms.Sdk.Query;

namespace Xms.Business.DataAnalyse.Data
{
    /// <summary>
    /// 报表仓储
    /// </summary>
    public class ReportRepository : DefaultRepository<Domain.Report>, IReportRepository
    {
        public ReportRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        #region implements

        public string GetFieldValueName(IQueryResolver queryTranslator, string field, Schema.Domain.Attribute attr = null)
        {
            if (attr == null)
            {
                var alias = queryTranslator.AttributeAliasList.Find(n => n.Alias.IsCaseInsensitiveEqual(field));
                if (alias != null)
                {
                    attr = queryTranslator.AttributeList.Find(n => n.Name.IsCaseInsensitiveEqual(alias.Name));
                }
            }
            field = attr.GetNameField(field);
            return field.ToLower();
        }

        public string GetGroupingName(ReportDescriptor report, IQueryResolver queryTranslator, string field, bool includeAlias = false)
        {
            var groupObj = report.CustomReport.Groupings.Find(n => n.Field.IsCaseInsensitiveEqual(field));
            if (groupObj != null && groupObj.DateGrouping.HasValue)
            {
                if (includeAlias)
                {
                    var alias = queryTranslator.AttributeAliasList.Find(n => n.Alias.IsCaseInsensitiveEqual(field));
                    if (alias != null)
                    {
                        field = alias.EntityAlias + "." + alias.Name;
                    }
                }
                field = DataFieldExpressionHelper.GetDateGroupingExpression(groupObj.DateGrouping.Value, field);
            }
            else if (groupObj != null && !groupObj.DateGrouping.HasValue)
            {
                field = GetFieldValueName(queryTranslator, field);
            }
            return field;
        }

        private string GetChartDataSqlString(ReportDescriptor report, IQueryResolver queryTranslator)
        {
            //drillthrough, aggregation
            var columnField = report.CustomReport.Chart.ColumnAxis.Field;
            var nameField = GetFieldValueName(queryTranslator, columnField);
            if (!columnField.IsCaseInsensitiveEqual(nameField))
            {
                nameField = "," + nameField;
            }
            else
            {
                nameField = string.Empty;
            }
            columnField = GetGroupingName(report, queryTranslator, columnField);
            var sql = "SELECT ";
            var orderby = "";
            if (report.CustomReport.Filter != null)
            {
                sql += " TOP " + report.CustomReport.Filter.Value;
                orderby = " ORDER BY " + report.CustomReport.Filter.Field + (report.CustomReport.Filter.Operator == FilterOperator.TopN ? " DESC" : "");
            }
            sql += columnField + " AS " + report.CustomReport.Chart.ColumnAxis.Field + nameField;
            foreach (var item in report.CustomReport.Chart.ValueAxes)
            {
                var column = report.CustomReport.Columns.Find(n => n.Field.IsCaseInsensitiveEqual(item.Field));
                sql += string.Format(",{0} AS {1}", DataFieldExpressionHelper.GetAggregationExpression(column.SummaryValue.Value, item.Field), item.Field);
            }
            sql += string.Format(" FROM ({0}) a GROUP BY {1} {2}", queryTranslator.ToSqlString(), columnField + nameField, orderby);
            return sql;
        }

        public DataTable GetChartData(ReportDescriptor report, IQueryResolver queryTranslator)
        {
            var ds = new DataVisitor(DbContext).ExecuteQueryDataSet(GetChartDataSqlString(report, queryTranslator), queryTranslator.Parameters.Args.ToArray());
            return ds.Tables[0];
        }

        public DataTable GetData(ReportDescriptor report, IQueryResolver queryTranslator, FilterExpression filter = null)
        {
            if (filter != null)
            {
                //设置过滤条件中特殊字段的名称
                //foreach (var item in filter.Conditions)
                //{
                //    item.AttributeName = GetFieldValueName(queryTranslator, item.AttributeName);
                //}
                report.CustomReport.Query.Criteria = filter;//.AddFilter(filter);
            }
            var sql = queryTranslator.ToSqlString();
            //获取数据
            var ds = new DataVisitor(DbContext).ExecuteQueryDataSet(sql, queryTranslator.Parameters.Args.ToArray());
            return ds.Tables[0];
        }

        public PagedList<Domain.Report> QueryPaged(QueryDescriptor<Domain.Report> q, int solutionComponentType, Guid solutionId, bool existInSolution)
        {
            if (q.QueryText.IsNotEmpty())
            {
                q.QueryText += " AND ";
            }
            q.QueryText += "ReportId " + (existInSolution ? "" : "NOT") + " IN(SELECT ObjectId FROM SolutionComponent WHERE SolutionId=@" + q.Parameters.Count;
            q.Parameters.Add(new QueryParameter("@" + q.Parameters.Count, solutionId));
            q.QueryText += " and ComponentType = @" + q.Parameters.Count;
            q.Parameters.Add(new QueryParameter("@" + q.Parameters.Count, solutionComponentType));
            q.QueryText += ")";
            return base.QueryPaged(q);
        }

        #endregion implements
    }
}