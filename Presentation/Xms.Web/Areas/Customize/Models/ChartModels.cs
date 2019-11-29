using System;
using Xms.Business.DataAnalyse.Domain;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Paging;

namespace Xms.Web.Customize.Models
{
    public class ChartModel : BasePaged<Chart>
    {
        public Guid EntityId { get; set; }
        public string Name { get; set; }
        public Guid SolutionId { get; set; }
        public bool LoadData { get; set; }
    }

    public class EditChartModel
    {
        public Guid EntityId { get; set; }
        public string Name { get; set; }
        public Guid SolutionId { get; set; }
        public string DataConfig { get; set; }
        public string PresentationConfig { get; set; }
        public Guid ChartId { get; set; }
        public Schema.Domain.Entity EntityMeta { get; set; }
    }

    public class SetChartStateModel : SetRecordStateModel
    {
        public Guid EntityId { get; set; }
    }
}