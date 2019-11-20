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
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.RibbonButton.Abstractions;
using Xms.RibbonButton.Data;

namespace Xms.RibbonButton
{
    /// <summary>
    /// 按钮更新服务
    /// </summary>
    public class RibbonButtonUpdater : IRibbonButtonUpdater
    {
        private readonly IRibbonButtonRepository _ribbonButtonRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly IRibbonButtonDependency _dependencyService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IAppContext _appContext;

        public RibbonButtonUpdater(IAppContext appContext
            , IRibbonButtonRepository ribbonButtonRepository
            , ILocalizedLabelService localizedLabelService
            , IRibbonButtonDependency dependencyService
            , IEventPublisher eventPublisher)
        {
            _appContext = appContext;
            _ribbonButtonRepository = ribbonButtonRepository;
            _localizedLabelService = localizedLabelService;
            _dependencyService = dependencyService;
            _eventPublisher = eventPublisher;
        }

        public bool Update(Domain.RibbonButton entity)
        {
            var original = _ribbonButtonRepository.FindById(entity.RibbonButtonId);
            var result = true;
            using (UnitOfWork.Build(_ribbonButtonRepository.DbContext))
            {
                result = _ribbonButtonRepository.Update(entity);
                //依赖
                _dependencyService.Update(entity);
                //localization
                _localizedLabelService.Update(entity.Label.IfEmpty(""), "LocalizedName", entity.RibbonButtonId, this._appContext.BaseLanguage);
                _localizedLabelService.Update(entity.Description.IfEmpty(""), "Description", entity.RibbonButtonId, this._appContext.BaseLanguage);
                //assigning roles
                if (original.AuthorizationEnabled || !entity.AuthorizationEnabled)
                {
                    _eventPublisher.Publish(new AuthorizationStateChangedEvent
                    {
                        ObjectId = new List<Guid> { entity.RibbonButtonId }
                        ,
                        State = false
                        ,
                        ResourceName = RibbonButtonDefaults.ModuleName
                    });
                }
            }
            return result;
        }

        public bool UpdateState(bool isEnabled, params Guid[] id)
        {
            var context = UpdateContextBuilder.Build<Domain.RibbonButton>();
            context.Set(f => f.StateCode, isEnabled ? RecordState.Enabled : RecordState.Disabled);
            context.Where(f => f.RibbonButtonId.In(id));
            return _ribbonButtonRepository.Update(context);
        }

        public bool UpdateAuthorization(bool isAuthorization, params Guid[] id)
        {
            var context = UpdateContextBuilder.Build<Domain.RibbonButton>();
            context.Set(f => f.AuthorizationEnabled, isAuthorization);
            context.Where(f => f.RibbonButtonId.In(id));
            var result = true;
            using (UnitOfWork.Build(_ribbonButtonRepository.DbContext))
            {
                result = _ribbonButtonRepository.Update(context);
                _eventPublisher.Publish(new AuthorizationStateChangedEvent
                {
                    ObjectId = id.ToList()
                    ,
                    State = isAuthorization
                    ,
                    ResourceName = RibbonButtonDefaults.ModuleName
                });
            }
            return result;
        }
    }
}