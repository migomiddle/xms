using System;
using Xms.Dependency;
using Xms.Flow.Core;
using Xms.Schema.Abstractions;

namespace Xms.Flow
{
    /// <summary>
    /// 流程依赖服务
    /// </summary>
    public class WorkFlowDependency : IWorkFlowDependency
    {
        private readonly IDependencyService _dependencyService;

        public WorkFlowDependency(IDependencyService dependencyService)
        {
            _dependencyService = dependencyService;
        }

        public bool Create(Domain.WorkFlow entity)
        {
            //依赖于字段
            return _dependencyService.Create(WorkFlowDefaults.ModuleName, entity.WorkFlowId, EntityDefaults.ModuleName, entity.EntityId);
        }

        public bool Update(Domain.WorkFlow entity)
        {
            //依赖于字段
            return _dependencyService.Update(WorkFlowDefaults.ModuleName, entity.WorkFlowId, EntityDefaults.ModuleName, entity.EntityId);
        }

        public bool Delete(params Guid[] id)
        {
            return _dependencyService.DeleteByDependentId(WorkFlowDefaults.ModuleName, id); ;
        }
    }
}