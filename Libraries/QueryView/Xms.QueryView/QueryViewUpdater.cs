using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Authorization.Abstractions;
using Xms.Context;
using Xms.Core;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Data.Provider;
using Xms.Event.Abstractions;
using Xms.Identity;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.QueryView.Abstractions;
using Xms.QueryView.Data;

namespace Xms.QueryView
{
    /// <summary>
    /// 视图更新服务
    /// </summary>
    public class QueryViewUpdater : IQueryViewUpdater
    {
        private readonly IQueryViewRepository _queryViewRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly IQueryViewDependency _dependencyService;
        private readonly IEventPublisher _eventPublisher;
        private readonly Caching.CacheManager<Domain.QueryView> _cacheService;
        private readonly IAppContext _appContext;
        private readonly ICurrentUser _currentUser;

        public QueryViewUpdater(IAppContext appContext
            , IQueryViewRepository queryViewRepository
            , ILocalizedLabelService localizedLabelService
            , IQueryViewDependency dependencyService
            , IEventPublisher eventPublisher
            )
        {
            _appContext = appContext;
            _currentUser = _appContext.GetFeature<ICurrentUser>();
            _queryViewRepository = queryViewRepository;
            _localizedLabelService = localizedLabelService;
            _dependencyService = dependencyService;
            _eventPublisher = eventPublisher;
            _cacheService = new Caching.CacheManager<Domain.QueryView>(_appContext.OrganizationUniqueName + ":queryviews", _appContext.PlatformSettings.CacheEnabled);
        }

        public bool Update(Domain.QueryView entity)
        {
            var original = _queryViewRepository.FindById(entity.QueryViewId);
            entity.ModifiedBy = _currentUser.SystemUserId;
            entity.ModifiedOn = DateTime.Now;
            var result = true;
            using (UnitOfWork.Build(_queryViewRepository.DbContext))
            {
                result = _queryViewRepository.Update(entity);
                _dependencyService.Update(entity);
                //localization
                _localizedLabelService.Update(entity.Name.IfEmpty(""), "LocalizedName", entity.QueryViewId, _appContext.BaseLanguage);
                _localizedLabelService.Update(entity.Description.IfEmpty(""), "Description", entity.QueryViewId, _appContext.BaseLanguage);
                //assigning roles
                if (original.AuthorizationEnabled || !entity.AuthorizationEnabled)
                {
                    _eventPublisher.Publish(new AuthorizationStateChangedEvent
                    {
                        ObjectId = new List<Guid> { entity.QueryViewId }
                        ,
                        State = false
                        ,
                        ResourceName = QueryViewDefaults.ModuleName
                    });
                }
                //set to cache
                _cacheService.SetEntity(entity);
            }
            return result;
        }

        public bool UpdateDefault(Guid entityId, Guid queryViewId)
        {
            Guard.NotEmpty(entityId, nameof(entityId));
            Guard.NotEmpty(queryViewId, nameof(queryViewId));
            //其他记录设置为非默认
            var context = UpdateContextBuilder.Build<Domain.QueryView>();
            context.Set(f => f.IsDefault, false);
            context.Where(f => f.EntityId == entityId);
            _queryViewRepository.Update(context);
            //设置为默认
            context = UpdateContextBuilder.Build<Domain.QueryView>();
            context.Set(f => f.IsDefault, true)
                .Set(f => f.AuthorizationEnabled, false)
                .Set(f => f.ModifiedOn, DateTime.Now)
                .Set(f => f.ModifiedBy, _currentUser.SystemUserId);
            context.Where(f => f.QueryViewId == queryViewId);
            var result = true;
            using (UnitOfWork.Build(_queryViewRepository.DbContext))
            {
                _queryViewRepository.Update(context);
                _eventPublisher.Publish(new AuthorizationStateChangedEvent
                {
                    ObjectId = new List<Guid> { queryViewId }
                    ,
                    State = false
                    ,
                    ResourceName = QueryViewDefaults.ModuleName
                });
                //set to cache
                var items = _queryViewRepository.Query(f => f.EntityId == entityId).ToList();
                foreach (var item in items)
                {
                    _cacheService.SetEntity(item);
                }
            }
            return result;
        }

        public bool UpdateState(IEnumerable<Guid> ids, bool isEnabled)
        {
            Guard.NotEmpty(ids, nameof(ids));
            var context = UpdateContextBuilder.Build<Domain.QueryView>();
            context.Set(f => f.StateCode, isEnabled ? RecordState.Enabled : RecordState.Disabled)
                .Set(f => f.ModifiedOn, DateTime.Now)
                .Set(f => f.ModifiedBy, _currentUser.SystemUserId);
            context.Where(f => f.QueryViewId.In(ids));
            var result = _queryViewRepository.Update(context);
            //cache
            if (result)
            {
                var items = _queryViewRepository.Query(f => f.QueryViewId.In(ids)).ToList();
                foreach (var item in items)
                {
                    _cacheService.SetEntity(item);
                }
            }
            return result;
        }

        public bool UpdateAuthorization(bool isAuthorization, params Guid[] id)
        {
            Guard.NotEmpty(id, nameof(id));
            var context = UpdateContextBuilder.Build<Domain.QueryView>();
            context.Set(f => f.AuthorizationEnabled, isAuthorization)
                .Set(f => f.ModifiedOn, DateTime.Now)
                .Set(f => f.ModifiedBy, _currentUser.SystemUserId);
            context.Where(f => f.QueryViewId.In(id));
            var result = true;
            using (UnitOfWork.Build(_queryViewRepository.DbContext))
            {
                result = _queryViewRepository.Update(context);
                _eventPublisher.Publish(new AuthorizationStateChangedEvent
                {
                    ObjectId = id.ToList()
                    ,
                    State = isAuthorization
                    ,
                    ResourceName = QueryViewDefaults.ModuleName
                });
                //set to cache
                var items = _queryViewRepository.Query(f => f.QueryViewId.In(id)).ToList();
                foreach (var item in items)
                {
                    _cacheService.SetEntity(item);
                }
            }
            return result;
        }
    }
}