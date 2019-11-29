using System;
using System.Collections.Generic;
using Xms.Context;
using Xms.Flow.Domain;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;

namespace Xms.Flow
{
    /// <summary>
    /// 审批流导入服务
    /// </summary>
    [SolutionImportNode("workflows")]
    public class WorkFlowImporter : ISolutionComponentImporter<WorkFlow>
    {
        private readonly IWorkFlowFinder _workFlowFinder;
        private readonly IWorkFlowCreater _workFlowCreater;
        private readonly IWorkFlowUpdater _workFlowUpdater;
        private readonly IWorkFlowStepService _workFlowStepService;
        private readonly IProcessStageService _processStageService;
        private readonly IAppContext _appContext;

        public WorkFlowImporter(IAppContext appContext
            , IWorkFlowFinder workFlowFinder
            , IWorkFlowCreater workFlowCreater
            , IWorkFlowUpdater workFlowUpdater
            , IWorkFlowStepService workFlowStepService
            , IProcessStageService processStageService)
        {
            _appContext = appContext;
            _workFlowFinder = workFlowFinder;
            _workFlowCreater = workFlowCreater;
            _workFlowUpdater = workFlowUpdater;
            _workFlowStepService = workFlowStepService;
            _processStageService = processStageService;
        }

        public bool Import(Guid solutionId, IList<WorkFlow> workFlows)
        {
            if (workFlows.NotEmpty())
            {
                foreach (var item in workFlows)
                {
                    var entity = _workFlowFinder.FindById(item.WorkFlowId);
                    if (entity != null)
                    {
                        entity.Description = item.Description;
                        entity.Name = item.Name;
                        entity.StateCode = item.StateCode;
                        _workFlowUpdater.Update(entity);
                        //steps
                        if (item.Category == 1)
                        {
                            _workFlowStepService.DeleteByParentId(item.WorkFlowId);
                            foreach (var st in item.Steps)
                            {
                                st.WorkFlowId = item.WorkFlowId;
                            }
                            _workFlowStepService.CreateMany(item.Steps);
                        }
                        else if (item.Category == 2)
                        {
                            _processStageService.DeleteByParentId(item.WorkFlowId);
                            foreach (var st in item.Stages)
                            {
                                st.WorkFlowId = item.WorkFlowId;
                            }
                            _processStageService.CreateMany(item.Stages);
                        }
                    }
                    else
                    {
                        item.SolutionId = solutionId;
                        item.ComponentState = 0;
                        item.CreatedBy = _appContext.GetFeature<ICurrentUser>().SystemUserId;
                        item.OrganizationId = _appContext.OrganizationId;
                        _workFlowCreater.Create(item);
                        if (item.Category == 1 && item.Steps.NotEmpty())
                        {
                            _workFlowStepService.CreateMany(item.Steps);
                        }
                        else if (item.Category == 2 && item.Stages.NotEmpty())
                        {
                            _processStageService.CreateMany(item.Stages);
                        }
                    }
                }
            }
            return true;
        }
    }
}