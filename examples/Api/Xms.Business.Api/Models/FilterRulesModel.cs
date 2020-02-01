using System;
using Xms.Business.Filter.Domain;
using Xms.Business.SerialNumber.Domain;
using Xms.Core;
using Xms.Web.Framework.Paging;

namespace Xms.Business.Api.Models
{
    public class FilterRulesModel : BasePaged<FilterRule>
    {
        public Guid EntityId { get; set; }
    }

    public class CreateFilterRuleModel
    {
        public Guid EntityId { get; set; }
        public string Name { get; set; }
        //public List<ConditionExpression> Conditions { get; set; }
        //public LogicalOperator LogicalOperator { get; set; }
        public string Conditions { get; set; }
        public string ToolTip { get; set; }
        public string EventName { get; set; }
        public RecordState StateCode { get; set; }
    }
    public class EditFilterRuleModel
    {
        public Guid FilterRuleId { get; set; }
        public Guid EntityId { get; set; }
        public string Name { get; set; }
        public string Conditions { get; set; }
        public string ToolTip { get; set; }
        public string EventName { get; set; }
        public RecordState StateCode { get; set; }
    }
}
