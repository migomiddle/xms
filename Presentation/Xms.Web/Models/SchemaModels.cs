using System;

namespace Xms.Web.Models
{
    public class GetAttributesModel
    {
        public string[] AttributeTypeName { get; set; }
        public Guid EntityId { get; set; }
        public Guid SolutionId { get; set; }
        public bool FilterSysAttribute { get; set; }
    }
}