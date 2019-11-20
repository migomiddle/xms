using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Query;

namespace Xms.Business.DataAnalyse.Report
{
    public interface IReportService
    {
        bool Create(Domain.Report entity);

        bool DeleteById(Guid id);

        bool DeleteById(IEnumerable<Guid> ids);

        Domain.Report Find(Expression<Func<Domain.Report, bool>> predicate);

        Domain.Report FindById(Guid id);

        DataTable GetChartData(ReportDescriptor report, IQueryResolver queryTranslator);

        DataTable GetData(ReportDescriptor report, IQueryResolver queryTranslator, FilterExpression filter = null);

        string GetFieldValueName(IQueryResolver queryTranslator, string field, Schema.Domain.Attribute attr = null);

        string GetGroupingName(ReportDescriptor report, IQueryResolver queryTranslator, string field, bool includeAlias = false);

        List<Domain.Report> QueryAuthorized(Func<QueryDescriptor<Domain.Report>, QueryDescriptor<Domain.Report>> container);

        PagedList<Domain.Report> QueryPaged(Func<QueryDescriptor<Domain.Report>, QueryDescriptor<Domain.Report>> container);

        PagedList<Domain.Report> QueryPaged(Func<QueryDescriptor<Domain.Report>, QueryDescriptor<Domain.Report>> container, Guid solutionId, bool existInSolution);

        bool Update(Domain.Report entity);

        bool UpdateAuthorization(IEnumerable<Guid> ids, bool isAuthorization);
    }
}