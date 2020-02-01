using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Paging;
using Xms.Business.DataAnalyse.Domain;

namespace Xms.Business.Api.Models
{
    public class ChartModel : BasePaged<Xms.Business.DataAnalyse.Domain.Chart>
    {
        public Guid EntityId { get; set; }
        public string Name { get; set; }
        public Schema.Domain.Entity Entity { get; set; }
        public Guid SolutionId { get; set; }
    }

    public class EditChartModel
    {
        public Guid EntityId { get; set; }
        public string Name { get; set; }
        public Guid SolutionId { get; set; }
        public string DataConfig { get; set; }
        public string PresentationConfig { get; set; }
        public Guid ChartId { get; set; }
    }
    public class SetChartStateModel : SetRecordStateModel
    {
        public Guid EntityId { get; set; }
    }
}
