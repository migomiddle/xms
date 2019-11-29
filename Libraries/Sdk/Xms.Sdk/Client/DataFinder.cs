using System.Collections.Generic;
using System.Linq;
using Xms.Authorization.Abstractions;
using Xms.Context;
using Xms.Core;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Event.Abstractions;
using Xms.Infrastructure.Utility;
using Xms.Organization;
using Xms.Schema.Entity;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Data;

namespace Xms.Sdk.Client
{
    /// <summary>
    /// 数据查询服务
    /// </summary>
    public class DataFinder : DataProviderBase, IDataFinder
    {
        private readonly IOrganizationDataRetriever _organizationDataRetriever;

        public DataFinder(
            IAppContext appContext
            , IEntityFinder entityFinder
            , IRoleObjectAccessEntityPermissionService roleObjectAccessEntityPermissionService
            , IPrincipalObjectAccessService principalObjectAccessService
            , IEventPublisher eventPublisher
            , IBusinessUnitService businessUnitService
            , IOrganizationDataRetriever organizationDataRetriever
            )
            : base(appContext, entityFinder, roleObjectAccessEntityPermissionService, principalObjectAccessService, eventPublisher, businessUnitService)
        {
            _organizationDataRetriever = organizationDataRetriever;
        }

        /// <summary>
        /// 分页查询记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PagedList<Entity> RetrieveMultiple(QueryBase request, bool ignorePermissions = false)
        {
            VerifyQuery(request);
            if (!ignorePermissions)
            {
                //get entity permissions
                BindUserEntityPermissions(request, AccessRightValue.Read);
            }
            //SetRetrieveConditions(request as QueryExpression);
            return _organizationDataRetriever.RetrievePaged(request, ignorePermissions);
        }

        /// <summary>
        /// 查询所有记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public List<Entity> RetrieveAll(QueryBase request, bool ignorePermissions = false)
        {
            VerifyQuery(request);
            if (!ignorePermissions)
            {
                //get entity permissions
                BindUserEntityPermissions(request, AccessRightValue.Read);
            }
            return _organizationDataRetriever.RetrieveAll(request, ignorePermissions)?.ToList();
        }

        /// <summary>
        /// 查询一条记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Entity Retrieve(QueryBase request, bool ignorePermissions = false)
        {
            VerifyQuery(request);
            if (!ignorePermissions)
            {
                //get entity permissions
                BindUserEntityPermissions(request, AccessRightValue.Read);
            }
            return _organizationDataRetriever.Retrieve(request, ignorePermissions);
        }

        /// <summary>
        /// 校验查询参数
        /// </summary>
        /// <param name="query"></param>
        private void VerifyQuery(QueryBase query)
        {
            ColumnSet columnSet = null;
            string entityName = null;
            if (query is QueryExpression)
            {
                QueryExpression queryExpression = query as QueryExpression;
                columnSet = queryExpression.ColumnSet;
                entityName = queryExpression.EntityName;
            }
            else if (query is QueryByAttribute)
            {
                QueryByAttribute queryByAttribute = query as QueryByAttribute;
                columnSet = queryByAttribute.ColumnSet;
                entityName = queryByAttribute.EntityName;
            }
            if (columnSet == null || (!columnSet.AllColumns && columnSet.Columns.IsEmpty()))
            {
                OnException(_loc["sdk_retrieve_notspecified_column"]);
            }
            if (entityName.IsEmpty())
            {
                OnException(_loc["sdk_notspecified_entityname"]);
            }
        }
    }
}