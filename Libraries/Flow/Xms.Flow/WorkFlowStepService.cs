using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Data.Provider;
using Xms.Dependency;
using Xms.Flow.Core;
using Xms.Flow.Data;
using Xms.Flow.Domain;
using Xms.Form.Abstractions;
using Xms.Infrastructure.Utility;

namespace Xms.Flow
{
    /// <summary>
    /// 工作流步骤服务
    /// </summary>
    public class WorkFlowStepService : IWorkFlowStepService, ICascadeDelete<WorkFlow>
    {
        private readonly IWorkFlowStepRepository _workFlowStepRepository;
        private readonly IDependencyService _dependencyService;
        private readonly IDependencyBatchBuilder _dependencyBatchBuilder;

        public WorkFlowStepService(IWorkFlowStepRepository workFlowStepRepository
            , IDependencyService dependencyService
            , IDependencyBatchBuilder dependencyBatchBuilder)
        {
            _workFlowStepRepository = workFlowStepRepository;
            _dependencyService = dependencyService;
            _dependencyBatchBuilder = dependencyBatchBuilder;
        }

        public bool Create(WorkFlowStep entity)
        {
            var result = true;
            using (UnitOfWork.Build(_workFlowStepRepository.DbContext))
            {
                result = _workFlowStepRepository.Create(entity);
                //依赖于表单
                if (!entity.FormId.Equals(Guid.Empty))
                {
                    _dependencyService.Create(WorkFlowDefaults.ModuleName, entity.WorkFlowId, FormDefaults.ModuleName, entity.FormId);
                }
            }

            return result;
        }

        public bool CreateMany(IList<WorkFlowStep> entities)
        {
            var result = true;
            using (UnitOfWork.Build(_workFlowStepRepository.DbContext))
            {
                _workFlowStepRepository.CreateMany(entities);
                //依赖于表单
                foreach (var item in entities)
                {
                    if (!item.FormId.Equals(Guid.Empty))
                    {
                        _dependencyBatchBuilder.Append(WorkFlowDefaults.ModuleName, item.WorkFlowId, FormDefaults.ModuleName, item.FormId);
                    }
                }
                _dependencyBatchBuilder.Save();
            }

            return result;
        }

        public bool Update(WorkFlowStep entity)
        {
            var original = _workFlowStepRepository.FindById(entity.WorkFlowStepId);
            if (original == null)
            {
                return false;
            }
            var result = true;
            using (UnitOfWork.Build(_workFlowStepRepository.DbContext))
            {
                result = _workFlowStepRepository.Update(entity);

                if (!original.FormId.Equals(Guid.Empty))
                {
                    //依赖于表单
                    _dependencyService.Update(WorkFlowDefaults.ModuleName, entity.WorkFlowId, FormDefaults.ModuleName, entity.FormId);
                }
            }
            return result;
        }

        public bool Update(Func<UpdateContext<WorkFlowStep>, UpdateContext<WorkFlowStep>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<WorkFlowStep>());
            return _workFlowStepRepository.Update(ctx);
        }

        public WorkFlowStep FindById(Guid id)
        {
            return _workFlowStepRepository.FindById(id);
        }

        public WorkFlowStep Find(Expression<Func<WorkFlowStep, bool>> predicate)
        {
            return _workFlowStepRepository.Find(predicate);
        }

        public bool DeleteById(params Guid[] id)
        {
            if (id.IsEmpty())
            {
                return false;
            }
            var deleteds = _workFlowStepRepository.Query(x => x.WorkFlowStepId.In(id));
            if (deleteds.IsEmpty())
            {
                return false;
            }
            var result = true;
            using (UnitOfWork.Build(_workFlowStepRepository.DbContext))
            {
                result = _workFlowStepRepository.DeleteMany(id);
                _dependencyService.DeleteByDependentId(WorkFlowDefaults.ModuleName, deleteds.Select(x => x.WorkFlowId).ToArray());
            }
            return result;
        }

        public bool DeleteByParentId(Guid parentid)
        {
            var result = true;
            using (UnitOfWork.Build(_workFlowStepRepository.DbContext))
            {
                result = _workFlowStepRepository.DeleteMany(x => x.WorkFlowId == parentid);
                _dependencyService.DeleteByDependentId(WorkFlowDefaults.ModuleName, parentid);
            }
            return result;
        }

        public PagedList<WorkFlowStep> QueryPaged(Func<QueryDescriptor<WorkFlowStep>, QueryDescriptor<WorkFlowStep>> container)
        {
            QueryDescriptor<WorkFlowStep> q = container(QueryDescriptorBuilder.Build<WorkFlowStep>());

            return _workFlowStepRepository.QueryPaged(q);
        }

        public List<WorkFlowStep> Query(Func<QueryDescriptor<WorkFlowStep>, QueryDescriptor<WorkFlowStep>> container)
        {
            QueryDescriptor<WorkFlowStep> q = container(QueryDescriptorBuilder.Build<WorkFlowStep>());

            return _workFlowStepRepository.Query(q)?.ToList();
        }

        /// <summary>
        /// 级联删除
        /// </summary>
        /// <param name="parent">被删除的审批流</param>
        public void CascadeDelete(params WorkFlow[] parent)
        {
            if (parent.NotEmpty())
            {
                using (UnitOfWork.Build(_workFlowStepRepository.DbContext))
                {
                    _workFlowStepRepository.DeleteMany(x => x.WorkFlowId.In(parent.Select(f => f.WorkFlowId)));
                    _dependencyService.DeleteByDependentId(WorkFlowDefaults.ModuleName, parent.Select(x => x.WorkFlowId).ToArray());
                }
            }
        }
    }
}