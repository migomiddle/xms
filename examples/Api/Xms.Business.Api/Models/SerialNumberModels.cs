using System;
using Xms.Business.SerialNumber.Domain;
using Xms.Core;
using Xms.Web.Framework.Paging;

namespace Xms.Business.Api.Models
{
    public class SerialNumberModel : BasePaged<SerialNumberRule>
    {
        public Guid EntityId { get; set; }
        public string Name { get; set; }
        public Guid SolutionId { get; set; }
    }
    public class CreateSerialNumberModel
    {
        public string Name { get; set; }

        public Guid EntityId { get; set; }
        public string Description { get; set; }

        public RecordState StateCode { get; set; }
        public string Prefix { get; set; }

        public string Seprator { get; set; }
        public int DateFormatType { get; set; }
        public int IncrementLength { get; set; }
        public int Increment { get; set; }
        public Guid AttributeId { get; set; }
    }
    public class EditSerialNumberModel
    {
        public Guid SerialNumberRuleId { get; set; }
        public string Name { get; set; }

        public Guid EntityId { get; set; }
        public string EntityName { get; set; }
        public string Description { get; set; }

        public RecordState StateCode { get; set; }
        public string Prefix { get; set; }
        public string Seprator { get; set; }
        public int DateFormatType { get; set; }
        public int IncrementLength { get; set; }
        public int Increment { get; set; }
        public Guid AttributeId { get; set; }
        public string AttributeName { get; set; }
    }
}
