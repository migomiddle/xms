using System;

namespace Xms.Web.Api.Models
{
    public class ObjectOptionValueModel
    {
        public Guid ObjectId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public int ObjectTypeCode { get; set; }
    }
}