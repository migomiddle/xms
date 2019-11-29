using System;

namespace Xms.Web.Api.Models
{
    public class SetUserPersonalizationModel
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}