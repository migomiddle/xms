using System;
using System.Data;
using Xms.Business.DataAnalyse.Report;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Query;

namespace Xms.Business.DataAnalyse.Data
{
    public interface IReportRepository : IRepository<Domain.Report>
    {
        string GetFieldValueName(IQueryResolver queryTranslator, string field, Schema.Domain.Attribute attr = null);

        string GetGroupingName(ReportDescriptor report, IQueryResolver queryTranslator, string field, bool includeAlias = false);

        DataTable GetChartData(ReportDescriptor report, IQueryResolver queryTranslator);

        DataTable GetData(ReportDescriptor report, IQueryResolver queryTranslator, FilterExpression filter = null);

        PagedList<Domain.Report> QueryPaged(QueryDescriptor<Domain.Report> q, int solutionComponentType, Guid solutionId, bool existInSolution);
    }
}