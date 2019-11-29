using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Data.Provider;
using Xms.Dependency;
using Xms.Flow.Abstractions;
using Xms.Flow.Core;
using Xms.Flow.Data;
using Xms.Flow.Domain;
using Xms.Infrastructure.Utility;
using Xms.Schema.Abstractions;
using Xms.Schema.Attribute;

namespace Xms.Flow
{
    /// <summary>
    /// 业务流程阶段服务
    /// </summary>
    public class ProcessStageService : IProcessStageService, ICascadeDelete<WorkFlow>
    {
        private readonly IProcessStageRepository _processStageRepository;
        private readonly IDependencyService _dependencyService;
        private readonly IDependencyBatchBuilder _dependencyBatchBuilder;
        private readonly IAttributeFinder _attributeFinder;

        public ProcessStageService(IProcessStageRepository processStageRepository
            , IDependencyService dependencyService
            , IDependencyBatchBuilder dependencyBatchBuilder
            , IAttributeFinder attributeFinder)
        {
            _processStageRepository = processStageRepository;
            _dependencyService = dependencyService;
            _dependencyBatchBuilder = dependencyBatchBuilder;
            _attributeFinder = attributeFinder;
        }

        public bool Create(ProcessStage entity)
        {
            var result = true;
            using (UnitOfWork.Build(_processStageRepository.DbContext))
            {
                result = _processStageRepository.Create(entity);
                //依赖于实体
                _dependencyService.Create(WorkFlowDefaults.ModuleName, entity.WorkFlowId, EntityDefaults.ModuleName, entity.EntityId);
                //依赖于字段
                var st = new List<ProcessStep>().DeserializeFromJson(entity.Steps);
                if (st.NotEmpty())
                {
                    var attrNames = st.Select(x => x.AttributeName).ToArray();
                    var attributes = _attributeFinder.FindByName(entity.EntityId, attrNames);
                    var attrIds = attributes.Select(x => x.AttributeId).ToArray();
                    _dependencyService.Create(WorkFlowDefaults.ModuleName, entity.WorkFlowId, AttributeDefaults.ModuleName, attrIds);
                }
            }

            return result;
        }

        public bool CreateMany(IList<ProcessStage> entities)
        {
            var result = true;
            using (UnitOfWork.Build(_processStageRepository.DbContext))
            {
                result = _processStageRepository.CreateMany(entities);
                foreach (var entity in entities)
                {
                    //依赖于实体
                    _dependencyBatchBuilder.Append(WorkFlowDefaults.ModuleName, entity.WorkFlowId, EntityDefaults.ModuleName, entity.EntityId);
                    //依赖于字段
                    var st = new List<ProcessStep>().DeserializeFromJson(entity.Steps);
                    if (st.NotEmpty())
                    {
                        var attrNames = st.Select(x => x.AttributeName).ToArray();
                        var attributes = _attributeFinder.FindByName(entity.EntityId, attrNames);
                        var attrIds = attributes.Select(x => x.AttributeId).ToArray();
                        _dependencyBatchBuilder.Append(WorkFlowDefaults.ModuleName, entity.WorkFlowId, AttributeDefaults.ModuleName, attrIds);
                    }
                }
                _dependencyBatchBuilder.Save();
            }

            return result;
        }

        public bool Update(ProcessStage entity)
        {
            var result = true;
            using (UnitOfWork.Build(_processStageRepository.DbContext))
            {
                result = _processStageRepository.Update(entity);
                //依赖于字段
                var st = new List<ProcessStep>().DeserializeFromJson(entity.Steps);
                if (st.NotEmpty())
                {
                    var attrNames = st.Select(x => x.AttributeName).ToArray();
                    var attributes = _attributeFinder.FindByName(entity.EntityId, attrNames);
                    var attrIds = attributes.Select(x => x.AttributeId).ToArray();
                    _dependencyService.Update(WorkFlowDefaults.ModuleName, entity.WorkFlowId, AttributeDefaults.ModuleName, attrIds);
                }
            }

            return result;
        }

        public bool Update(Func<UpdateContext<ProcessStage>, UpdateContext<ProcessStage>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<ProcessStage>());
            return _processStageRepository.Update(ctx);
        }

        public ProcessStage FindById(Guid id)
        {
            return _processStageRepository.FindById(id);
        }

        public ProcessStage Find(Expression<Func<ProcessStage, bool>> predicate)
        {
            return _processStageRepository.Find(predicate);
        }

        public bool DeleteById(params Guid[] ids)
        {
            if (ids.IsEmpty())
            {
                return false;
            }
            var deleteds = _processStageRepository.Query(x => x.ProcessStageId.In(ids));
            if (deleteds.IsEmpty())
            {
                return false;
            }
            var result = true;
            using (UnitOfWork.Build(_processStageRepository.DbContext))
            {
                result = _processStageRepository.DeleteMany(ids);
                _dependencyService.DeleteByDependentId(WorkFlowDefaults.ModuleName, deleteds.Select(x => x.WorkFlowId).ToArray());
            }
            return result;
        }

        public bool DeleteByParentId(Guid parentid)
        {
            var result = true;
            using (UnitOfWork.Build(_processStageRepository.DbContext))
            {
                result = _processStageRepository.DeleteMany(x => x.WorkFlowId == parentid);
                _dependencyService.DeleteByDependentId(WorkFlowDefaults.ModuleName, parentid);
            }
            return result;
        }

        public PagedList<ProcessStage> QueryPaged(Func<QueryDescriptor<ProcessStage>, QueryDescriptor<ProcessStage>> container)
        {
            QueryDescriptor<ProcessStage> q = container(QueryDescriptorBuilder.Build<ProcessStage>());

            return _processStageRepository.QueryPaged(q);
        }

        public List<ProcessStage> Query(Func<QueryDescriptor<ProcessStage>, QueryDescriptor<ProcessStage>> container)
        {
            QueryDescriptor<ProcessStage> q = container(QueryDescriptorBuilder.Build<ProcessStage>());

            return _processStageRepository.Query(q)?.ToList();
        }

        /// <summary>
        /// 级联删除
        /// </summary>
        /// <param name="parent">被删除的审批流</param>
        public void CascadeDelete(params WorkFlow[] parent)
        {
            if (parent.NotEmpty())
            {
                using (UnitOfWork.Build(_processStageRepository.DbContext))
                {
                    _processStageRepository.DeleteMany(x => x.WorkFlowId.In(parent.Select(f => f.WorkFlowId)));
                    _dependencyService.DeleteByDependentId(WorkFlowDefaults.ModuleName, parent.Select(x => x.WorkFlowId).ToArray());
                }
            }
        }
    }
}