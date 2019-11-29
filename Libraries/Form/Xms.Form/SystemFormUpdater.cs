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
using Xms.Form.Abstractions;
using Xms.Form.Data;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Localization;

namespace Xms.Form
{
    /// <summary>
    /// 表单更新服务
    /// </summary>
    public class SystemFormUpdater : ISystemFormUpdater
    {
        private readonly ISystemFormRepository _systemFormRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly Caching.CacheManager<Domain.SystemForm> _cacheService;
        private readonly IFormService _formService;
        private readonly ISystemFormDependency _dependencyService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IAppContext _appContext;

        public SystemFormUpdater(IAppContext appContext
            , ISystemFormRepository systemFormRepository
            , ILocalizedLabelService localizedLabelService
            , IFormService formService
            , ISystemFormDependency dependencyService
            , IEventPublisher eventPublisher)
        {
            _appContext = appContext;
            _systemFormRepository = systemFormRepository;
            _localizedLabelService = localizedLabelService;
            _formService = formService;
            _dependencyService = dependencyService;
            _eventPublisher = eventPublisher;
            _cacheService = new Caching.CacheManager<Domain.SystemForm>(_appContext.OrganizationUniqueName + ":systemforms", _appContext.PlatformSettings.CacheEnabled);
        }

        public bool Update(Domain.SystemForm entity, bool updatedConfig)
        {
            var original = _systemFormRepository.FindById(entity.SystemFormId);
            var result = true;
            _formService.Init(entity);
            using (UnitOfWork.Build(_systemFormRepository.DbContext))
            {
                result = _systemFormRepository.Update(entity);
                _dependencyService.Update(entity);
                //localization
                _localizedLabelService.Update(entity.Name.IfEmpty(""), "LocalizedName", entity.SystemFormId, _appContext.BaseLanguage);
                _localizedLabelService.Update(entity.Description.IfEmpty(""), "Description", entity.SystemFormId, _appContext.BaseLanguage);
                if (updatedConfig)
                {
                    _formService.UpdateLocalizedLabel(original);
                }
                //assigning roles
                if (original.AuthorizationEnabled || !entity.AuthorizationEnabled)
                {
                    _eventPublisher.Publish(new AuthorizationStateChangedEvent
                    {
                        ObjectId = new List<Guid> { entity.SystemFormId }
                        ,
                        State = false
                        ,
                        ResourceName = FormDefaults.ModuleName
                    });
                }
                //set to cache
                _cacheService.SetEntity(entity);
            }
            return result;
        }

        public bool UpdateDefault(Guid entityId, Guid systemFormId)
        {
            Guard.NotEmpty(entityId, nameof(entityId));
            Guard.NotEmpty(systemFormId, nameof(systemFormId));
            //其他记录设置为非默认
            var context = UpdateContextBuilder.Build<Domain.SystemForm>();
            context.Set(f => f.IsDefault, false);
            context.Where(f => f.EntityId == entityId && f.SystemFormId != systemFormId);
            _systemFormRepository.Update(context);
            //设置为默认
            context = UpdateContextBuilder.Build<Domain.SystemForm>();
            context.Set(f => f.IsDefault, true);
            context.Set(f => f.AuthorizationEnabled, false);
            context.Where(f => f.SystemFormId == systemFormId);
            var result = true;
            using (UnitOfWork.Build(_systemFormRepository.DbContext))
            {
                result = _systemFormRepository.Update(context);
                _eventPublisher.Publish(new AuthorizationStateChangedEvent
                {
                    ObjectId = new List<Guid> { systemFormId }
                    ,
                    State = false
                    ,
                    ResourceName = FormDefaults.ModuleName
                });
                //set to cache
                var items = _systemFormRepository.Query(f => f.EntityId == entityId).ToList();
                foreach (var item in items)
                {
                    _cacheService.SetEntity(item);
                }
            }
            return result;
        }

        public bool UpdateState(bool isEnabled, params Guid[] id)
        {
            Guard.NotEmpty(id, nameof(id));
            var context = UpdateContextBuilder.Build<Domain.SystemForm>();
            context.Set(f => f.StateCode, isEnabled ? RecordState.Enabled : RecordState.Disabled);
            context.Where(f => f.SystemFormId.In(id));
            var result = true;
            using (UnitOfWork.Build(_systemFormRepository.DbContext))
            {
                result = _systemFormRepository.Update(context);
                //cache
                var items = _systemFormRepository.Query(f => f.SystemFormId.In(id)).ToList();
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
            var entities = _systemFormRepository.Query(x => x.SystemFormId.In(id));
            if (entities.IsEmpty())
            {
                return false;
            }
            var result = true;
            using (UnitOfWork.Build(_systemFormRepository.DbContext))
            {
                var context = UpdateContextBuilder.Build<Domain.SystemForm>();
                context.Set(f => f.AuthorizationEnabled, isAuthorization);
                context.Where(f => f.SystemFormId.In(id) && f.IsDefault == false);
                result = _systemFormRepository.Update(context);
                _eventPublisher.Publish(new AuthorizationStateChangedEvent
                {
                    ObjectId = id.ToList()
                    ,
                    State = isAuthorization
                    ,
                    ResourceName = entities.First().FormType == (int)FormType.Dashboard ? DashBoardDefaults.ModuleName : FormDefaults.ModuleName
                });
                //set to cache
                var items = _systemFormRepository.Query(f => f.SystemFormId.In(id)).ToList();
                foreach (var item in items)
                {
                    _cacheService.SetEntity(item);
                }
                return result;
            }
        }
    }
}