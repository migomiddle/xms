using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Xms.QueryView.Abstractions.Component;
using Xms.Sdk.Abstractions.Query;
using Xms.Web.Framework.Paging;

namespace Xms.QueryView.Api.Models
{
    public class QueryViewModel : BasePaged<QueryView.Domain.QueryView>
    {
        public Guid QueryViewId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? OwnerId { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsSimpleFilter { get; set; }

        public Guid EntityId { get; set; }
        public Schema.Domain.Entity Entity { get; set; }
        public Guid SolutionId { get; set; }
    }

    public class EditQueryViewModel
    {
        public string SaveType { get; set; }
        public Guid QueryViewId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public bool IsCustomButton { get; set; }
        public string FetchConfig { get; set; }
        public string LayoutConfig { get; set; }
        public string AggregateConfig { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid? ModifiedBy { get; set; }
        public Guid? OwnerId { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsSimpleFilter { get; set; }
        public Guid? TargetFormId { get; set; }
        public Guid EntityId { get; set; }
        public Schema.Domain.Entity EntityMetaData { get; set; }

        public GridDescriptor Grid { get; set; }

        public List<Schema.Domain.Entity> EntityList { get; set; }
        public List<Schema.Domain.Attribute> AttributeList { get; set; }
        public QueryExpression QueryExpression { get; set; }
        public Guid SolutionId { get; set; }
        public List<RibbonButton.Domain.RibbonButton> Buttons { get; set; }
        public List<Guid> ButtonId { get; set; }
        public bool IsAuthorization { get; set; }
        public List<Guid> AssignRoleId { get; set; }
    }

    public class SetQueryViewDefaultModel
    {
        public List<Guid> RecordId { get; set; }
        public Guid EntityId { get; set; }
        public bool IsDefault { get; set; }
    }

    public class SetViewAuthorizationStateModel
    {
        public Guid[] RecordId { get; set; }
        public bool IsAuthorization { get; set; }
    }
}
