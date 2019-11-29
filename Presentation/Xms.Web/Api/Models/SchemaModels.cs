using System;
using Xms.Web.Framework.Paging;

namespace Xms.Web.Api.Models
{
    public class RetrieveEntityModel : BasePaged<Schema.Domain.Entity>
    {
        public string[] Name { get; set; }
        public bool? IsLoged { get; set; }
        public bool? IsCustomizable { get; set; }
        public bool? IsAuthorization { get; set; }
        public bool? DuplicateEnabled { get; set; }
        public bool? WorkFlowEnabled { get; set; }
        public bool? BusinessFlowEnabled { get; set; }
        public Guid? SolutionId { get; set; }
    }
}