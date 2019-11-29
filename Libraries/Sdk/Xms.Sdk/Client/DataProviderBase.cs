using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Authorization.Abstractions;
using Xms.Context;
using Xms.Core;
using Xms.Core.Data;
using Xms.Event.Abstractions;
using Xms.Identity;
using Xms.Infrastructure;
using Xms.Localization.Abstractions;
using Xms.Organization;
using Xms.Schema.Abstractions;
using Xms.Schema.Entity;
using Xms.Sdk.Abstractions;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Extensions;

namespace Xms.Sdk.Client
{
    /// <summary>
    /// 组织数据操作服务基类
    /// </summary>
    public abstract class DataProviderBase
    {
        protected readonly IEntityFinder _entityFinder;
        protected readonly IRoleObjectAccessEntityPermissionService _roleObjectAccessEntityPermissionService;
        protected readonly IPrincipalObjectAccessService _principalObjectAccessService;
        protected readonly IEventPublisher _eventPublisher;
        protected readonly IBusinessUnitService _businessUnitService;
        protected readonly IAppContext _appContext;
        protected readonly ICurrentUser _user;
        protected readonly ILocalizedTextProvider _loc;
        protected readonly LanguageCode _languageId;

        public DataProviderBase(
            IAppContext appContext
            , IEntityFinder entityFinder
            , IRoleObjectAccessEntityPermissionService roleObjectAccessEntityPermissionService
            , IPrincipalObjectAccessService principalObjectAccessService
            , IEventPublisher eventPublisher
            , IBusinessUnitService businessUnitService
            )
        {
            _appContext = appContext;
            _user = _appContext.GetFeature<ICurrentUser>();
            _languageId = _user?.UserSettings?.LanguageId ?? LanguageCode.CHS;
            _loc = _appContext.GetFeature<ILocalizedTextProvider>();
            _entityFinder = entityFinder;
            _roleObjectAccessEntityPermissionService = roleObjectAccessEntityPermissionService;
            _principalObjectAccessService = principalObjectAccessService;
            _businessUnitService = businessUnitService;
            _eventPublisher = eventPublisher;
        }

        #region transaction

        //public void BeginTransaction()
        //{
        //    _organizationDataProvider.BeginTransaction();
        //}

        //public void CommitTransaction()
        //{
        //    _organizationDataProvider.CommitTransaction();
        //}

        //public void RollBackTransaction()
        //{
        //    _organizationDataProvider.RollBackTransaction();
        //}

        #endregion transaction

        protected Schema.Domain.Entity GetEntityMetaData(string entityName)
        {
            Guard.NotEmpty(entityName, "entityName");
            var entityMetadata = _entityFinder.FindByName(entityName);
            if (entityMetadata == null)
            {
                throw new XmsException(string.Format(_loc["notfound_entityname"], entityName));
            }
            return entityMetadata;
        }

        protected void BindUserEntityPermissions(QueryBase q, AccessRightValue access)
        {
            //get entity permissions
            var roles = _user?.Roles?.Select(r => r.RoleId);
            if (!roles.Any())
            {
                OnException(_loc["notspecified_userroles"]);
            }
            var entities = q is QueryExpression ? (q as QueryExpression).GetAllEntityNames() : new List<string>() { q.EntityName };
            var entIds = _entityFinder.FindByNames(entities.ToArray()).Select(n => n.EntityId);
            _user.RoleObjectAccessEntityPermission = _roleObjectAccessEntityPermissionService.GetPermissions(entIds, roles, access);
        }

        /// <summary>
        /// 异常发生时执行的方法
        /// </summary>
        /// <param name="e"></param>
        public virtual bool OnException(Exception e)
        {
            //try
            //{
            //    ServiceLocator.Get<ILogService>()?.Error(e.Message, e);
            //}
            //catch { }
            //return false;
            throw new XmsException(e);
        }

        public virtual bool OnException(string message)
        {
            throw new XmsException(message);
        }

        /// <summary>
        /// 校验实体权限
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="access"></param>
        /// <param name="entityMetadata"></param>
        protected void VerifyEntityPermission(Entity entity, AccessRightValue access, Schema.Domain.Entity entityMetadata = null)
        {
            entityMetadata = entityMetadata ?? GetEntityMetaData(entity.Name);
            //authorization disabled or user is administrator group
            if (!entityMetadata.AuthorizationEnabled || _user.IsSuperAdmin)
            {
                return;
            }

            bool hasPermission = false;
            //operation permission
            var roleEntityPermission = _roleObjectAccessEntityPermissionService.FindUserPermission(entity.Name, _user.LoginName, access);
            //if (roleEntityPermission == null) return;
            //permission depth
            var depth = "none";
            if (roleEntityPermission != null)
            {
                var data = entity.UnWrapAttributeValue();
                if (entityMetadata.EntityMask == EntityMaskEnum.Organization)
                {
                    var b = data.GetGuidValue("organizationid");
                    hasPermission = (roleEntityPermission.AccessRightsMask != EntityPermissionDepth.None && this._user.OrganizationId.Equals(b));
                }
                else
                {
                    var ownerIdType = data.GetIntValue("owneridtype");
                    //full
                    if (roleEntityPermission.AccessRightsMask == EntityPermissionDepth.Organization)
                    {
                        hasPermission = true;
                    }
                    else if (ownerIdType == (int)OwnerTypes.SystemUser)
                    {
                        //basic
                        if (roleEntityPermission.AccessRightsMask == EntityPermissionDepth.Self)
                        {
                            hasPermission = data.GetGuidValue("ownerid").Equals(this._user.SystemUserId);
                        }
                        //local
                        else if (roleEntityPermission.AccessRightsMask == EntityPermissionDepth.BusinessUnit)
                        {
                            var b = data.GetGuidValue("owningbusinessunit");
                            hasPermission = _user.BusinessUnitId.Equals(b);
                        }
                        //deep
                        else if (roleEntityPermission.AccessRightsMask == EntityPermissionDepth.BusinessUnitAndChild)
                        {
                            var b = data.GetGuidValue("owningbusinessunit");
                            hasPermission = _businessUnitService.IsChild(this._user.BusinessUnitId, b);
                        }
                    }
                    else if (ownerIdType == (int)OwnerTypes.Team)
                    {
                        //basic
                        if (roleEntityPermission.AccessRightsMask == EntityPermissionDepth.Self)
                        {
                            hasPermission = data.GetGuidValue("ownerid").Equals(this._user.SystemUserId);
                        }
                        //local
                        else if (roleEntityPermission.AccessRightsMask == EntityPermissionDepth.BusinessUnit)
                        {
                            var b = data.GetGuidValue("owningbusinessunit");
                            hasPermission = _user.BusinessUnitId.Equals(b);
                        }
                        //deep
                        else if (roleEntityPermission.AccessRightsMask == EntityPermissionDepth.BusinessUnitAndChild)
                        {
                            var b = data.GetGuidValue("owningbusinessunit");
                            hasPermission = _businessUnitService.IsChild(this._user.BusinessUnitId, b);
                        }
                    }
                }
                depth = roleEntityPermission.AccessRightsMask.ToString();
            }
            //shared permission
            if (!hasPermission)
            {
                var objectId = entity.GetIdValue();
                var poa = _principalObjectAccessService.Find(n => n.ObjectId == objectId && n.AccessRightsMask == access);
                hasPermission = poa != null;
            }
            if (!hasPermission)
            {
                var msg = Enum.GetName(typeof(AccessRightValue), access);
                msg = _loc["security_" + msg];
                OnException(string.Format(_loc["security_noentitypermission"] + " " + depth, entityMetadata.LocalizedName, msg));
            }
        }
    }
}