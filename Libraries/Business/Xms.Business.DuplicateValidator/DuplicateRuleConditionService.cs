using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Business.DuplicateValidator.Data;
using Xms.Business.DuplicateValidator.Domain;
using Xms.Context;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;

namespace Xms.Business.DuplicateValidator
{
    /// <summary>
    /// 重复检测规则条件服务
    /// </summary>
    public class DuplicateRuleConditionService : IDuplicateRuleConditionService, ICascadeDelete<DuplicateRule>
    {
        private readonly IDuplicateRuleConditionRepository _duplicateRuleConditionRepository;

        public DuplicateRuleConditionService(IAppContext appContext
            , IDuplicateRuleConditionRepository duplicateRuleConditionRepository)
        {
            _duplicateRuleConditionRepository = duplicateRuleConditionRepository;
        }

        public bool Create(DuplicateRuleCondition entity)
        {
            return _duplicateRuleConditionRepository.Create(entity);
        }

        public bool CreateMany(List<DuplicateRuleCondition> entities)
        {
            return _duplicateRuleConditionRepository.CreateMany(entities);
        }

        public bool Update(DuplicateRuleCondition entity)
        {
            return _duplicateRuleConditionRepository.Update(entity);
        }

        public bool Update(Func<UpdateContext<DuplicateRuleCondition>, UpdateContext<DuplicateRuleCondition>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<DuplicateRuleCondition>());
            return _duplicateRuleConditionRepository.Update(ctx);
        }

        public DuplicateRuleCondition FindById(Guid id)
        {
            return _duplicateRuleConditionRepository.FindById(id);
        }

        public DuplicateRuleCondition Find(Expression<Func<DuplicateRuleCondition, bool>> predicate)
        {
            return _duplicateRuleConditionRepository.Find(predicate);
        }

        public bool DeleteById(Guid id)
        {
            return _duplicateRuleConditionRepository.DeleteById(id);
        }

        public bool DeleteById(List<Guid> ids)
        {
            return _duplicateRuleConditionRepository.DeleteMany(ids);
        }

        public bool DeleteByParentId(Guid parentid)
        {
            return _duplicateRuleConditionRepository.DeleteMany(x => x.DuplicateRuleId == parentid);
        }

        public PagedList<DuplicateRuleCondition> QueryPaged(Func<QueryDescriptor<DuplicateRuleCondition>, QueryDescriptor<DuplicateRuleCondition>> container)
        {
            QueryDescriptor<DuplicateRuleCondition> q = container(QueryDescriptorBuilder.Build<DuplicateRuleCondition>());

            return _duplicateRuleConditionRepository.QueryPaged(q);
        }

        public List<DuplicateRuleCondition> Query(Func<QueryDescriptor<DuplicateRuleCondition>, QueryDescriptor<DuplicateRuleCondition>> container)
        {
            QueryDescriptor<DuplicateRuleCondition> q = container(QueryDescriptorBuilder.Build<DuplicateRuleCondition>());

            return _duplicateRuleConditionRepository.Query(q)?.ToList();
        }

        /// <summary>
        /// 级联删除
        /// </summary>
        /// <param name="parent">被删除的规则</param>
        public void CascadeDelete(params DuplicateRule[] parent)
        {
            if (parent.IsEmpty())
            {
                return;
            }
            var ids = parent.Select(x => x.DuplicateRuleId).ToArray();
            _duplicateRuleConditionRepository.DeleteMany(x => x.DuplicateRuleId.In(ids));
        }
    }
}