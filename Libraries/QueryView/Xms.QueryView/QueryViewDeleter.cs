using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Context;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Dependency;
using Xms.Event.Abstractions;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.QueryView.Abstractions;
using Xms.QueryView.Data;

namespace Xms.QueryView
{
    /// <summary>
    /// 视图删除服务
    /// </summary>
    public class QueryViewDeleter : IQueryViewDeleter, ICascadeDelete<Schema.Domain.Entity>
    {
        private readonly IQueryViewRepository _queryViewRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly IQueryViewDependency _dependencyService;
        private readonly IDependencyChecker _dependencyChecker;
        private readonly IEventPublisher _eventPublisher;
        private readonly IEnumerable<ICascadeDelete<Domain.QueryView>> _cascadeDeletes;
        private readonly Caching.CacheManager<Domain.QueryView> _cacheService;
        private readonly IAppContext _appContext;

        public QueryViewDeleter(IAppContext appContext
            , IQueryViewRepository queryViewRepository
            , ILocalizedLabelService localizedLabelService
            , IQueryViewDependency dependencyService
            , IDependencyChecker dependencyChecker
            , IEventPublisher eventPublisher
            , IEnumerable<ICascadeDelete<Domain.QueryView>> cascadeDeletes)
        {
            _appContext = appContext;
            _queryViewRepository = queryViewRepository;
            _localizedLabelService = localizedLabelService;
            _dependencyService = dependencyService;
            _dependencyChecker = dependencyChecker;
            _eventPublisher = eventPublisher;
            _cascadeDeletes = cascadeDeletes;
            _cacheService = new Caching.CacheManager<Domain.QueryView>(_appContext.OrganizationUniqueName + ":queryviews", _appContext.PlatformSettings.CacheEnabled);
        }

        public bool DeleteById(params Guid[] id)
        {
            Guard.NotEmpty(id, nameof(id));
            var result = true;
            var deleteds = _queryViewRepository.Query(x => x.QueryViewId.In(id));
            if (deleteds.NotEmpty())
            {
                result = DeleteCore(deleteds, (deleted) =>
                {
                    if (deleted.IsDefault)
                    {
                        return false;
                    }
                    //检查依赖项
                    _dependencyChecker.CheckAndThrow<Domain.QueryView>(QueryViewDefaults.ModuleName, deleted.QueryViewId);
                    return true;
                });
            }
            return result;
        }

        /// <summary>
        /// 级联删除
        /// </summary>
        /// <param name="parent">被删除的实体</param>
        public void CascadeDelete(params Schema.Domain.Entity[] parent)
        {
            if (parent.IsEmpty())
            {
                return;
            }
            var entityIds = parent.Select(x => x.EntityId).ToArray();
            var views = _queryViewRepository.Query(x => x.EntityId.In(entityIds));
            if (views.NotEmpty())
            {
                DeleteCore(views, null);
            }
        }

        private bool DeleteCore(IEnumerable<Domain.QueryView> deleteds, Func<Domain.QueryView, bool> validation)
        {
            Guard.NotEmpty(deleteds, nameof(deleteds));
            var result = true;
            if (validation != null)
            {
                foreach (var deleted in deleteds)
                {
                    result = validation?.Invoke(deleted) ?? true;
                }
            }
            if (result)
            {
                var ids = deleteds.Select(x => x.QueryViewId).ToArray();
                using (UnitOfWork.Build(_queryViewRepository.DbContext))
                {
                    //cascade delete
                    _cascadeDeletes?.ToList().ForEach((x) => { x.CascadeDelete(deleteds.ToArray()); });
                    _queryViewRepository.DeleteMany(ids);
                    //删除依赖项
                    _dependencyService.Delete(ids);
                    //localization
                    _localizedLabelService.DeleteByObject(ids);
                    //remove from cache
                    foreach (var item in deleteds)
                    {
                        _cacheService.RemoveEntity(item);
                    }
                    foreach (var item in deleteds)
                    {
                        _eventPublisher.Publish(new ObjectDeletedEvent<Domain.QueryView>(QueryViewDefaults.ModuleName, item));
                    }
                }
            }
            return result;
        }
    }
}