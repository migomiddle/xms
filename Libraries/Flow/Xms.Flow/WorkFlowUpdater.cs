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
using Xms.Flow.Core;
using Xms.Flow.Data;
using Xms.Flow.Domain;
using Xms.Infrastructure.Utility;
using Xms.Localization;

namespace Xms.Flow
{
    /// <summary>
    /// 流程更新服务
    /// </summary>
    public class WorkFlowUpdater : IWorkFlowUpdater
    {
        private readonly IWorkFlowRepository _workFlowRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IAppContext _appContext;

        public WorkFlowUpdater(IAppContext appContext
            , IWorkFlowRepository workFlowRepository
            , ILocalizedLabelService localizedLabelService
            , IEventPublisher eventPublisher)
        {
            _appContext = appContext;
            _workFlowRepository = workFlowRepository;
            _localizedLabelService = localizedLabelService;
            _eventPublisher = eventPublisher;
        }

        public bool Update(WorkFlow entity)
        {
            var original = _workFlowRepository.FindById(entity.WorkFlowId);
            var result = true;
            using (UnitOfWork.Build(_workFlowRepository.DbContext))
            {
                result = _workFlowRepository.Update(entity);
                //localization
                _localizedLabelService.Update(entity.Name.IfEmpty(""), "LocalizedName", entity.WorkFlowId, _appContext.BaseLanguage);
                _localizedLabelService.Update(entity.Description.IfEmpty(""), "Description", entity.WorkFlowId, _appContext.BaseLanguage);

                //assigning roles
                if (original.AuthorizationEnabled != entity.AuthorizationEnabled)
                {
                    _eventPublisher.Publish(new AuthorizationStateChangedEvent
                    {
                        ObjectId = new List<Guid> { entity.WorkFlowId }
                        ,
                        State = false
                        ,
                        ResourceName = WorkFlowDefaults.ModuleName
                    });
                }
            }
            return result;
        }

        public bool UpdateState(bool isEnabled, params Guid[] id)
        {
            var context = UpdateContextBuilder.Build<WorkFlow>();
            context.Set(f => f.StateCode, isEnabled ? RecordState.Enabled : RecordState.Disabled);
            context.Where(f => f.WorkFlowId.In(id));
            return _workFlowRepository.Update(context);
        }

        public bool UpdateAuthorization(bool isAuthorization, params Guid[] id)
        {
            var context = UpdateContextBuilder.Build<WorkFlow>();
            context.Set(f => f.AuthorizationEnabled, isAuthorization);
            context.Where(f => f.WorkFlowId.In(id));
            var result = true;
            using (UnitOfWork.Build(_workFlowRepository.DbContext))
            {
                result = _workFlowRepository.Update(context);
                _eventPublisher.Publish(new AuthorizationStateChangedEvent
                {
                    ObjectId = id.ToList()
                    ,
                    State = isAuthorization
                    ,
                    ResourceName = WorkFlowDefaults.ModuleName
                });
            }
            return result;
        }
    }
}