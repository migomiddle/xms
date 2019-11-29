using System;
using System.Collections.Generic;
using Xms.Context;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;

namespace Xms.Business.DataAnalyse.Report
{
    /// <summary>
    /// 报表导入服务
    /// </summary>
    [SolutionImportNode("reports")]
    public class ReportImporter : ISolutionComponentImporter<Domain.Report>
    {
        private readonly IReportService _reportService;
        private readonly IAppContext _appContext;

        public ReportImporter(IAppContext appContext
            , IReportService reportService)
        {
            _appContext = appContext;
            _reportService = reportService;
        }

        public bool Import(Guid solutionId, IList<Domain.Report> reports)
        {
            if (reports.NotEmpty())
            {
                foreach (var item in reports)
                {
                    var entity = _reportService.FindById(item.ReportId);
                    if (entity != null)
                    {
                        entity.DefaultFilterConfig = item.DefaultFilterConfig;
                        entity.CustomConfig = item.CustomConfig;
                        entity.QueryConfig = item.QueryConfig;
                        entity.BodyText = item.BodyText;
                        entity.BodyUrl = item.BodyUrl;
                        entity.FileName = item.FileName;
                        entity.TypeCode = item.TypeCode;
                        entity.EntityId = item.EntityId;
                        entity.RelatedEntityId = item.RelatedEntityId;
                        entity.Description = item.Description;
                        entity.Name = item.Name;
                        _reportService.Update(entity);
                    }
                    else
                    {
                        item.ComponentState = 0;
                        item.SolutionId = solutionId;
                        item.CreatedBy = _appContext.GetFeature<ICurrentUser>().SystemUserId;
                        _reportService.Create(item);
                    }
                }
            }
            return true;
        }
    }
}