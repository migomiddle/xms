using System.Collections.Generic;
using System.Linq;
using Xms.Authorization.Abstractions;
using Xms.Context;
using Xms.Core;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Schema.Entity;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Extensions;
using Xms.Sdk.Query;
using Xms.Security.Principal;

namespace Xms.Sdk.Client
{
    /// <summary>
    /// 统计汇总服务
    /// </summary>
    public class AggregateService : IAggregateService
    {
        private readonly IAggregateExpressionResolver _aggregateExpressionResolver;
        protected readonly IRoleObjectAccessEntityPermissionService _roleObjectAccessEntityPermissionService;
        private readonly ISystemUserRolesService _systemUserRolesService;
        private readonly IEntityFinder _entityFinder;
        private readonly IAppContext _appContext;

        private readonly ICurrentUser _user;

        public AggregateService(
            IAppContext appContext,
            IRoleObjectAccessEntityPermissionService roleObjectAccessEntityPermissionService,
            ISystemUserRolesService systemUserRolesService,
            IEntityFinder entityFinder,
            IAggregateExpressionResolver aggregateExpressionResolver)
        {
            _appContext = appContext;
            _roleObjectAccessEntityPermissionService = roleObjectAccessEntityPermissionService;
            _systemUserRolesService = systemUserRolesService;
            _aggregateExpressionResolver = aggregateExpressionResolver;
            _entityFinder = entityFinder;
            _user = _appContext.GetFeature<ICurrentUser>();
        }

        private void BindUserEntityPermissions(QueryExpression query)
        {
            if (this._user != null)
            {
                if (!this._user.IsSuperAdmin && this._user.Roles.IsEmpty())
                {
                    this._user.Roles = _systemUserRolesService.Query(n => n.Where(f => f.SystemUserId == this._user.SystemUserId));
                }
            }

            if (this._user != null && !this._user.IsSuperAdmin && this._user.Roles.NotEmpty())
            {
                var roles = this._user.Roles.Select(r => r.RoleId);
                var entities = query.GetAllEntityNames();
                var entIds = _entityFinder.FindByNames(entities.ToArray()).Select(n => n.EntityId);
                this._user.RoleObjectAccessEntityPermission = _roleObjectAccessEntityPermissionService.GetPermissions(entIds, roles, AccessRightValue.Read);
            }
        }

        public List<dynamic> Execute(AggregateExpression agg)
        {
            BindUserEntityPermissions(agg);
            return _aggregateExpressionResolver.Execute(agg);
        }

        public List<dynamic> Execute(QueryExpression query, Dictionary<string, AggregateType> attributeAggs, List<string> groupFields)
        {
            BindUserEntityPermissions(query);
            return _aggregateExpressionResolver.Execute(query, attributeAggs, groupFields);
        }

        public List<dynamic> GroupingTop(int top, string groupField, QueryExpression query, OrderExpression order)
        {
            BindUserEntityPermissions(query);
            return _aggregateExpressionResolver.GroupingTop(top, query, groupField, order);
        }
    }
}