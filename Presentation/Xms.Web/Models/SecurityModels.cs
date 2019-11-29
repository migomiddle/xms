using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xms.Core;
using Xms.Core.Data;
using Xms.Localization.Domain;
using Xms.Security.Domain;
using Xms.SiteMap.Domain;
using Xms.Web.Framework.Paging;

namespace Xms.Web.Models
{
    public class PrivilegeModel : BasePaged<Privilege>
    {
        public string DisplayName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string SystemName { get; set; }

        public string ClassName { get; set; }
        public string MethodName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int ParentPrivilegeId { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool? IsEnable { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool? IsShowAsMenu { get; set; }

        public int? Level { get; set; }
    }

    public class EditPrivilegeModel
    {
        public Guid? PrivilegeId { get; set; }

        [Required]
        [StringLength(20)]
        public string DisplayName { get; set; }

        //[Required]
        [StringLength(50)]
        public string SystemName { get; set; }

        //[Required]
        [StringLength(50)]
        public string ClassName { get; set; }

        //[Required]
        [StringLength(50)]
        public string MethodName { get; set; }

        public Guid? ParentPrivilegeId { get; set; }
        public string ParentPrivilegeName { get; set; }

        //[Required]
        [StringLength(200)]
        public string Url { get; set; }

        public string OpenTarget { get; set; }
        public int? DisplayOrder { get; set; }

        [Required]
        public bool AuthorizationEnabled { get; set; }

        [Required]
        public bool IsVisibled { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// 小图标
        /// </summary>
        public string SmallIcon { get; set; }

        /// <summary>
        /// 大图标
        /// </summary>
        public string BigIcon { get; set; }
    }

    public class RoleModel : BasePaged<Role>
    {
        public List<Guid> UserId { get; set; }
        public List<Guid> TeamId { get; set; }

        public string Name { get; set; }

        public RecordState? State { get; set; }

        public List<Guid> SelectedRoles { get; set; }
    }

    public class RoleDialogModel : BasePaged<Role>
    {
        public List<Guid> UserId { get; set; }
        public List<Guid> TeamId { get; set; }

        public List<Guid> SelectedRoles { get; set; }
        public string InputId { get; set; }
        public bool SingleMode { get; set; }

        public string CallBack { get; set; } = "function(){}";
    }

    public class EditRoleModel
    {
        public Guid? RoleId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public RecordState State { get; set; }

        public bool IsEnabled { get; set; }
    }

    public class EntityPermissionsModel : BasePaged<EntityPermission>
    {
        public Guid? EntityPermissionId { get; set; }
        public Guid? EntityId { get; set; }

        public string Name { get; set; }
        public string EntityName { get; set; }

        public RecordState? State { get; set; }
    }

    public class EditEntityPermissionModel
    {
        public Guid? EntityPermissionId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public string Description { get; set; }
        public Guid EntityId { get; set; }
        public AccessRightValue AccessRight { get; set; }
        public string EntityName { get; set; }

        public RecordState State { get; set; }
        public Schema.Domain.Entity Entity { get; set; }

        public SelectList PermissionTypes { get; set; }
    }

    public class EditRoleEntityPermissionsModel
    {
        public List<Guid> EntityPermissionId { get; set; }
        public List<EntityPermissionDepth> Mask { get; set; }
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
        public List<RoleObjectAccess> RoleObjectAccess { get; set; }

        public List<EntityPermission> EntityPermissions { get; set; }

        public List<Schema.Domain.Entity> Entities { get; set; }
        public string ResourceName { get; set; }
        public List<Entity> EntityGroups { get; set; }
    }

    public class EditRoleButtonPermissionsModel
    {
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
        public List<RoleObjectAccess> RoleObjectAccesses { get; set; }
        public string ResourceName { get; set; }

        public ResourceOwner ResourceOwnerDescriptor { get; set; }
    }

    public class EditRolePermissionsModel
    {
        public List<Guid> ObjectId { get; set; }

        public Guid RoleId { get; set; }

        public Role Role { get; set; }

        public string ResourceName { get; set; }

        public ResourceOwner ResourceOwnerDescriptor { get; set; }

        public List<RoleObjectAccess> RoleObjectAccess { get; set; }
        public List<int> Mask { get; set; }
    }

    public class UsersInRoleModel : BasePaged<SystemUserRoles>
    {
        public Guid RoleId { get; set; }
    }

    public class RoleObjectAccessListModel : DialogModel
    {
        public bool EnabledAuthorization { get; set; }
        public string ObjectTypeName { get; set; }
        public Guid ObjectId { get; set; }
        public List<Role> Roles { get; set; }
        public List<RoleObjectAccess> Accesses { get; set; }
    }

    public class RoleObjectAccessModel
    {
        public bool EnabledAuthorization { get; set; }

        public string ObjectTypeName { get; set; }
        public Guid ObjectId { get; set; }
        public List<Guid> AssignRoleId { get; set; }
    }

    public class EditUserPasswordModel
    {
        public Guid SystemUserId { get; set; }
        public string Name { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }
    }

    public class UserSettingsModel
    {
        public Guid SystemUserSettingsId { get; set; }
        public int PagingLimit { get; set; }

        public int LanguageUniqueId { get; set; }
        public Guid CurrencyId { get; set; }
        public bool EnabledNotification { get; set; }
        public Schema.Domain.Entity EntityMeta { get; set; }
        public List<Schema.Domain.Attribute> AttributesMeta { get; set; }

        public List<Language> Languages { get; set; }

        public Core.Data.Entity EntityDatas { get; set; }
    }

    public class ResetMyPasswordModel
    {
        public string Name { get; set; }

        [Required]
        public string OriginalPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }
    }

    public class AddUserRolesModel
    {
        public List<Guid> UserId { get; set; }
        public List<Guid> RoleId { get; set; }
    }

    public class RemoveUserRolesModel
    {
        public List<Guid> UserId { get; set; }
        public Guid RoleId { get; set; }
    }

    public class PrivilegeResourceModel
    {
        public string ResourceName { get; set; }
        public ResourceOwner ResourceOwnerDescriptor { get; set; }
    }
}