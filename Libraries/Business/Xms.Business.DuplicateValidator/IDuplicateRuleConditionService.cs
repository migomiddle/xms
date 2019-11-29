using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Business.DuplicateValidator.Domain;
using Xms.Core.Context;

namespace Xms.Business.DuplicateValidator
{
    public interface IDuplicateRuleConditionService
    {
        bool Create(DuplicateRuleCondition entity);

        bool CreateMany(List<DuplicateRuleCondition> entities);

        bool DeleteById(Guid id);

        bool DeleteById(List<Guid> ids);

        bool DeleteByParentId(Guid parentid);

        DuplicateRuleCondition Find(Expression<Func<DuplicateRuleCondition, bool>> predicate);

        DuplicateRuleCondition FindById(Guid id);

        List<DuplicateRuleCondition> Query(Func<QueryDescriptor<DuplicateRuleCondition>, QueryDescriptor<DuplicateRuleCondition>> container);

        PagedList<DuplicateRuleCondition> QueryPaged(Func<QueryDescriptor<DuplicateRuleCondition>, QueryDescriptor<DuplicateRuleCondition>> container);

        bool Update(Func<UpdateContext<DuplicateRuleCondition>, UpdateContext<DuplicateRuleCondition>> context);

        bool Update(DuplicateRuleCondition entity);
    }
}