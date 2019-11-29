using System;
using System.ComponentModel.DataAnnotations;
using Xms.Schema.Abstractions;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Paging;

namespace Xms.Web.Customize.Models
{
    public class EntityModel : BasePaged<Schema.Domain.Entity>
    {
        public string Name { get; set; }
        public int ObjectTypeCode { get; set; }
        public bool IsLoged { get; set; }
        public bool IsCustomizable { get; set; }
        public bool IsAuthorization { get; set; }
        public string LocalizedName { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }

        public Guid SolutionId { get; set; }
        public bool LoadData { get; set; }
        public Guid EntityGroupId { get; set; }
    }

    public class CreateEntityModel
    {
        public Guid EntityId { get; set; }

        [Required]
        public string Name { get; set; }

        public bool LogEnabled { get; set; }
        public bool AuthorizationEnabled { get; set; }
        public bool WorkFlowEnabled { get; set; }
        public EntityMaskEnum EntityMask { get; set; }
        public Guid? ParentEntityId { get; set; }

        [Required]
        public string LocalizedName { get; set; }

        public bool DuplicateEnabled { get; set; }
        public Guid SolutionId { get; set; }
        public string Description { get; set; }
        public bool BusinessFlowEnabled { get; set; }
        public string[] DefaultAttributes { get; set; }
        public Guid[] DefaultButtons { get; set; }
        public bool CreateDefaultForm { get; set; }
        public bool CreateDefaultView { get; set; }
        public Guid[] EntityGroupId { get; set; }
    }

    public class EditEntityModel
    {
        public Schema.Domain.Entity Entity { get; set; }
        public Guid EntityId { get; set; }
        public bool LogEnabled { get; set; }
        public bool AuthorizationEnabled { get; set; }
        public bool IsCustomizable { get; set; }
        public bool WorkFlowEnabled { get; set; }

        [Required]
        public string LocalizedName { get; set; }

        public bool DuplicateEnabled { get; set; }
        public Guid SolutionId { get; set; }
        public string Description { get; set; }
        public bool BusinessFlowEnabled { get; set; }

        public EntityMaskEnum EntityMask { get; set; }

        public string ParentEntityLocalizedName { get; set; }

        public Guid[] EntityGroupId { get; set; }
    }

    public class DeleteEntityModel : DeleteManyModel
    {
        public bool DeleteTable { get; set; }
    }
}