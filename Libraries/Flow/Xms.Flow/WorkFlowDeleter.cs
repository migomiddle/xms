using System;
using System.Linq;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Event.Abstractions;
using Xms.Flow.Core;
using Xms.Flow.Data;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.Solution;

namespace Xms.Flow
{
    /// <summary>
    /// 流程删除服务
    /// </summary>
    public class WorkFlowDeleter : IWorkFlowDeleter, ICascadeDelete<Schema.Domain.Entity>
    {
        private readonly IWorkFlowRepository _workFlowRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly ISolutionComponentService _solutionComponentService;
        private readonly IWorkFlowDependency _dependencyService;
        private readonly IEventPublisher _eventPublisher;

        public WorkFlowDeleter(IWorkFlowRepository workFlowRepository
            , ILocalizedLabelService localizedLabelService
            , ISolutionComponentService solutionComponentService
            , IWorkFlowDependency dependencyService
            , IEventPublisher eventPublisher)
        {
            _workFlowRepository = workFlowRepository;
            _localizedLabelService = localizedLabelService;
            _solutionComponentService = solutionComponentService;
            _dependencyService = dependencyService;
            _eventPublisher = eventPublisher;
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
            var deleteds = _workFlowRepository.Query(x => x.EntityId.In(entityIds));
            if (deleteds.NotEmpty())
            {
                DeleteCore(deleteds.ToArray());
            }
        }

        public bool DeleteById(params Guid[] id)
        {
            Guard.NotEmpty(id, nameof(id));
            var result = false;
            var deleteds = _workFlowRepository.Query(x => x.WorkFlowId.In(id));
            if (deleteds.NotEmpty())
            {
                result = DeleteCore(deleteds.ToArray());
            }
            return result;
        }

        private bool DeleteCore(params Domain.WorkFlow[] deleteds)
        {
            Guard.NotEmpty(deleteds, nameof(deleteds));
            var result = false;
            var ids = deleteds.Select(x => x.WorkFlowId).ToArray();
            using (UnitOfWork.Build(_workFlowRepository.DbContext))
            {
                result = _workFlowRepository.DeleteMany(ids);
                //solution component
                _solutionComponentService.DeleteObject(deleteds.First().SolutionId, WorkFlowDefaults.ModuleName, ids);
                //localization
                _localizedLabelService.DeleteByObject(ids);
                //dependency objects
                _dependencyService.Delete(ids);
                foreach (var item in deleteds)
                {
                    _eventPublisher.Publish(new ObjectDeletedEvent<Domain.WorkFlow>(WorkFlowDefaults.ModuleName, item));
                }
            }
            return result;
        }
    }
}