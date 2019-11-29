using Xms.Context;
using Xms.Data.Abstractions;
using Xms.Flow.Core;
using Xms.Flow.Data;
using Xms.Flow.Domain;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.Solution;

namespace Xms.Flow
{
    /// <summary>
    /// 流程创建服务
    /// </summary>
    public class WorkFlowCreater : IWorkFlowCreater
    {
        private readonly IWorkFlowRepository _workFlowRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly ISolutionComponentService _solutionComponentService;
        private readonly IWorkFlowDependency _dependencyService;
        private readonly IAppContext _appContext;

        public WorkFlowCreater(IAppContext appContext
            , IWorkFlowRepository workFlowRepository
            , ILocalizedLabelService localizedLabelService
            , ISolutionComponentService solutionComponentService
            , IWorkFlowDependency dependencyService)
        {
            _appContext = appContext;
            _workFlowRepository = workFlowRepository;
            _localizedLabelService = localizedLabelService;
            _solutionComponentService = solutionComponentService;
            _dependencyService = dependencyService;
        }

        public bool Create(WorkFlow entity)
        {
            entity.OrganizationId = _appContext.OrganizationId;
            var result = true;
            using (UnitOfWork.Build(_workFlowRepository.DbContext))
            {
                result = _workFlowRepository.Create(entity);
                //solution component
                _solutionComponentService.Create(entity.SolutionId, entity.WorkFlowId, WorkFlowDefaults.ModuleName);
                //依赖于实体
                _dependencyService.Create(entity);
                //本地化标签
                _localizedLabelService.Create(entity.SolutionId, entity.Name.IfEmpty(""), WorkFlowDefaults.ModuleName, "LocalizedName", entity.WorkFlowId, _appContext.BaseLanguage);
                _localizedLabelService.Create(entity.SolutionId, entity.Description.IfEmpty(""), WorkFlowDefaults.ModuleName, "Description", entity.WorkFlowId, _appContext.BaseLanguage);
            }
            return result;
        }
    }
}