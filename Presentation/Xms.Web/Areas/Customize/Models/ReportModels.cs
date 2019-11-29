using Microsoft.AspNetCore.Http;
using System;
using Xms.Business.DataAnalyse.Domain;
using Xms.Web.Framework.Paging;

namespace Xms.Web.Customize.Models
{
    public class ReportModel : BasePaged<Report>
    {
        public Guid EntityId { get; set; }
        public string Name { get; set; }
        public Schema.Domain.Entity Entity { get; set; }
        public Guid SolutionId { get; set; }
    }

    public class EditReportModel
    {
        public Guid? ReportId { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public string DefaultFilterConfig { get; set; }

        public string CustomConfig { get; set; }

        public string QueryConfig { get; set; }

        public string BodyText { get; set; }

        public string FileName { get; set; }

        public bool IsPersonal { get; set; }

        public string BodyUrl { get; set; }

        public int TypeCode { get; set; }

        public bool IsCustomizable { get; set; }
        public Guid SolutionId { get; set; }

        public Guid EntityId { get; set; }

        public Guid RelatedEntityId { get; set; }
        public bool IsAuthorization { get; set; }
        public IFormFile ReportFile { get; set; }
    }

    public class SetReportAuthorizationStateModel
    {
        public Guid[] RecordId { get; set; }
        public bool IsAuthorization { get; set; }
    }
}