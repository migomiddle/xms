using System;
using System.Linq;
using Xms.Dependency;
using Xms.Infrastructure.Utility;
using Xms.QueryView.Abstractions;
using Xms.RibbonButton.Abstractions;
using Xms.Schema.Abstractions;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Extensions;

namespace Xms.QueryView
{
    /// <summary>
    /// 视图依赖服务
    /// </summary>
    public class QueryViewDependency : IQueryViewDependency
    {
        private readonly IDependencyService _dependencyService;
        private readonly IQueryMetadataFinder _queryMetadataFinder;

        public QueryViewDependency(IDependencyService dependencyService
            , IQueryMetadataFinder queryMetadataFinder)
        {
            _dependencyService = dependencyService;
            _queryMetadataFinder = queryMetadataFinder;
        }

        public bool Create(Domain.QueryView entity)
        {
            //依赖于字段
            var queryExp = new QueryExpression().DeserializeFromJson(entity.FetchConfig);
            var requiredAttributes = _queryMetadataFinder.GetAttributes(queryExp);
            _dependencyService.Create(QueryViewDefaults.ModuleName, entity.QueryViewId, AttributeDefaults.ModuleName, requiredAttributes.Select(x => x.AttributeId).ToArray());

            //依赖于按钮
            if (entity.IsCustomButton && entity.CustomButtons.IsNotEmpty())
            {
                var buttonIds = new Guid[] { }.DeserializeFromJson(entity.CustomButtons);
                if (buttonIds.NotEmpty())
                {
                    _dependencyService.Create(QueryViewDefaults.ModuleName, entity.QueryViewId, RibbonButtonDefaults.ModuleName, buttonIds);
                }
            }
            return true;
        }

        public bool Update(Domain.QueryView entity)
        {
            //依赖于字段
            var queryExp = new QueryExpression().DeserializeFromJson(entity.FetchConfig);
            var requiredAttributes = _queryMetadataFinder.GetAttributes(queryExp);
            _dependencyService.Update(QueryViewDefaults.ModuleName, entity.QueryViewId, AttributeDefaults.ModuleName, requiredAttributes.Select(x => x.AttributeId).ToArray());
            //依赖于按钮
            if (entity.IsCustomButton && entity.CustomButtons.IsNotEmpty())
            {
                var buttonIds = new Guid[] { }.DeserializeFromJson(entity.CustomButtons);
                if (buttonIds.NotEmpty())
                {
                    _dependencyService.Update(QueryViewDefaults.ModuleName, entity.QueryViewId, RibbonButtonDefaults.ModuleName, buttonIds);
                }
            }
            return true;
        }

        public bool Delete(params Guid[] id)
        {
            return _dependencyService.DeleteByDependentId(QueryViewDefaults.ModuleName, id); ;
        }
    }
}